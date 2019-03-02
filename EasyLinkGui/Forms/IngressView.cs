using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.Handler;
using CefSharp.WinForms;
using Newtonsoft.Json.Linq;
using EasyLinkLib;

namespace EasyLinkGui {
    public partial class IngressView : Form {
        MainForm mf = null;
        public List<string> responses = new List<string>();

        public void addResponse(string resp) {
            lock (responses) {
                responses.Add(resp);
            }
        }

        private  ChromiumWebBrowser browser;
        public IngressView(MainForm mf) {
            this.mf = mf;
            InitializeComponent();

            CefSettings settings = new CefSettings();
            settings.CachePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\CEF";

            if (!Cef.IsInitialized) {
                Cef.Initialize(settings);
            }



            browser = new ChromiumWebBrowser(@"https://intel.ingress.com/intel") {
                Dock = DockStyle.Fill,
            };
            pWebView.Controls.Add(browser);

            var requestHandler = new RequestHandler(this);
            browser.RequestHandler = requestHandler;
        }

        private void timer1_Tick(object sender, EventArgs e) {
            convertData();
        }

        private void convertData() {
            List<CoreEntity> ret = new List<CoreEntity>();
            if (!cbLoadPortals.Checked && !cbLoadLinks.Checked) {
                lock (responses) {
                    responses.Clear();
                }
                return;
            }
            List<string> locresp = new List<string>();
            lock (responses) {
                while(responses.Count > 0) {
                    locresp.Add(responses[0]);
                    responses.RemoveAt(0);
                }
            }
            foreach (string resp in locresp) {
                JObject job = JObject.Parse(resp);
                foreach (JProperty maps in job["result"]["map"]) {
                    if (maps.Value["gameEntities"] == null) continue;
                    foreach (JToken gameentity in maps.Value["gameEntities"]) {
                        switch (Lib.Converter.toString(gameentity[2][0])) {
                            case "p": if(cbLoadPortals.Checked) ret.Add(new PortalEntity(gameentity)); break;
                            case "e": if(cbLoadLinks.Checked) ret.Add(new LinkEntity(gameentity)); break;
                            //case "r": if(cbLoadPortals.Checked) ret.Add(new PortalEntity(gameentity)); break;
                        }
                    }
                }
            }
            if(ret.Count > 0) mf.addEntities(ret);
        }

        #region responseFetcher
        public class MemoryStreamResponseFilter : IResponseFilter {
            public MemoryStreamResponseFilter (IngressView mv) {
                this.mv = mv;
            }
            IngressView mv = null;
            private MemoryStream memoryStream;
            string resp = "";

            bool IResponseFilter.InitFilter() {
                //NOTE: We could initialize this earlier, just one possible use of InitFilter
                memoryStream = new MemoryStream();

                return true;
            }

            FilterStatus IResponseFilter.Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten) {
                if (dataIn == null) {
                    dataInRead = 0;
                    dataOutWritten = 0;

                    return FilterStatus.Done;
                }

                //Calculate how much data we can read, in some instances dataIn.Length is
                //greater than dataOut.Length
                dataInRead = Math.Min(dataIn.Length, dataOut.Length);
                dataOutWritten = dataInRead;

                var readBytes = new byte[dataInRead];
                dataIn.Read(readBytes, 0, readBytes.Length);
                dataOut.Write(readBytes, 0, readBytes.Length);

                //Write buffer to the memory stream
                memoryStream.Write(readBytes, 0, readBytes.Length);

                //If we read less than the total amount avaliable then we need
                //return FilterStatus.NeedMoreData so we can then write the rest
                if (dataInRead < dataIn.Length) {
                    return FilterStatus.NeedMoreData;
                }
                //byte[] all = new byte[memoryStream.Length];
                //memoryStream.Read(all, 0, all.Length);

                string ret = Encoding.UTF8.GetString(memoryStream.ToArray());
                memoryStream = new MemoryStream();

                resp += ret;
                if(!resp.EndsWith("}}}}")) {
                    return FilterStatus.NeedMoreData;
                }

                mv.addResponse(resp);

                return FilterStatus.Done;
            }

            void IDisposable.Dispose() {
                memoryStream.Dispose();
                memoryStream = null;
            }

            public byte[] Data {
                get { return memoryStream.ToArray(); }
            }
        }

        public class RequestHandler : DefaultRequestHandler {
            IngressView mv = null;

            public static readonly string VersionNumberString = String.Format("Chromium: {0}, CEF: {1}, CefSharp: {2}",
                Cef.ChromiumVersion, Cef.CefVersion, Cef.CefSharpVersion);

            private Dictionary<UInt64, MemoryStreamResponseFilter> responseDictionary = new Dictionary<UInt64, MemoryStreamResponseFilter>();

            public override bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect) {
                return false;
            }

            public override bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture) {
                return false;
            }

            public RequestHandler (IngressView mv) : base() {
                this.mv = mv;
            }

            public override bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback) {
                //NOTE: If you do not wish to implement this method returning false is the default behaviour
                // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.
                //callback.Dispose();
                //return false;

                //NOTE: When executing the callback in an async fashion need to check to see if it's disposed
                if (!callback.IsDisposed) {
                    using (callback) {
                        //To allow certificate
                        //callback.Continue(true);
                        //return true;
                    }
                }

                return false;
            }

            public override void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath) {
                // TODO: Add your own code here for handling scenarios where a plugin crashed, for one reason or another.
            }

            public override CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback) {
                Uri url;
                if (Uri.TryCreate(request.Url, UriKind.Absolute, out url) == false) {
                    //If we're unable to parse the Uri then cancel the request
                    // avoid throwing any exceptions here as we're being called by unmanaged code
                    return CefReturnValue.Cancel;
                }

                //Example of how to set Referer
                // Same should work when setting any header

                // For this example only set Referer when using our custom scheme

                //Example of setting User-Agent in every request.
                //var headers = request.Headers;

                //var userAgent = headers["User-Agent"];
                //headers["User-Agent"] = userAgent + " CefSharp";

                //request.Headers = headers;

                //NOTE: If you do not wish to implement this method returning false is the default behaviour
                // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.
                //callback.Dispose();
                //return false;

                //NOTE: When executing the callback in an async fashion need to check to see if it's disposed
                if (!callback.IsDisposed) {
                    using (callback) {
                        if (request.Method == "POST") {
                            using (var postData = request.PostData) {
                                if (postData != null) {
                                    var elements = postData.Elements;

                                    var charSet = request.GetCharSet();

                                    foreach (var element in elements) {
                                        if (element.Type == PostDataElementType.Bytes) {
                                            var body = element.GetBody(charSet);
                                        }
                                    }
                                }
                            }
                        }

                        //Note to Redirect simply set the request Url
                        //if (request.Url.StartsWith("https://www.google.com", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    request.Url = "https://github.com/";
                        //}

                        //Callback in async fashion
                        //callback.Continue(true);
                        //return CefReturnValue.ContinueAsync;
                    }
                }

                return CefReturnValue.Continue;
            }

            public override bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback) {
                //NOTE: If you do not wish to implement this method returning false is the default behaviour
                // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.

                callback.Dispose();
                return false;
            }

            public override bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback) {
                //NOTE: If you do not wish to implement this method returning false is the default behaviour
                // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.
                callback.Dispose();
                return false;
            }

            public override void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status) {
                // TODO: Add your own code here for handling scenarios where the Render Process terminated for one reason or another.
                //browserControl.Load(CefExample.RenderProcessCrashedUrl);
            }

            public override bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback) {
                //NOTE: If you do not wish to implement this method returning false is the default behaviour
                // We also suggest you explicitly Dispose of the callback as it wraps an unmanaged resource.
                //callback.Dispose();
                //return false;

                //NOTE: When executing the callback in an async fashion need to check to see if it's disposed
                if (!callback.IsDisposed) {
                    using (callback) {
                        //Accept Request to raise Quota
                        //callback.Continue(true);
                        //return true;
                    }
                }

                return false;
            }

            public override void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl) {
                //Example of how to redirect - need to check `newUrl` in the second pass
                //if (request.Url.StartsWith("https://www.google.com", StringComparison.OrdinalIgnoreCase) && !newUrl.Contains("github"))
                //{
                //    newUrl = "https://github.com";
                //}
            }

            public override bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url) {
                return url.StartsWith("mailto");
            }

            public override void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser) {

            }

            public override bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response) {
                //NOTE: You cannot modify the response, only the request
                // You can now access the headers
                //var headers = response.Headers;

                return false;
            }
            //MemoryStreamResponseFilter memFilter;
            public override IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response) {
                var url = new Uri(request.Url);

                if (url.Equals("https://intel.ingress.com/r/getEntities")) {
                    return new MemoryStreamResponseFilter(mv);
                    //if (memFilter == null) memFilter = new MemoryStreamResponseFilter();
                    //memFilter;
                }
                return null;
            }

            public override void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength) {
                var url = new Uri(request.Url);

            }
        }

        #endregion

        private void IngressView_Load(object sender, EventArgs e) {

        }
    }

    
}
