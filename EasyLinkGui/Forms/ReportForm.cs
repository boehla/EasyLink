using BrightIdeasSoftware;
using EasyLinkLib;
using Newtonsoft.Json.Linq;
using QRCoder;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace EasyLinkGui {
    public partial class ReportForm : Form {
        MainForm mf = null;

        public class ReportData {
            public string EasyLinkVersion = "";
            public string Name = "";
            public string Id = "";
            public double AP = 0;
            public double Area = 0;
            public int FieldCount = 0;

            public List<PortalInfo> DestroyPortals = null;
            public List<Link> LinkList = null;
            public List<Item> RequireList = null;
        }

        public ReportData rd = null;

        public ReportForm(MainForm mf) {
            InitializeComponent();
            this.mf = mf;

            rd = new ReportData();
            rd.EasyLinkVersion = AboutForm.GetVersion();
            rd.Name = mf.LastGroupNameSave;
            rd.AP = mf.GameState.getAPScore();
            rd.Area = mf.GameState.TotalArea;
            rd.Id = CreateMD5(rd.Name.ToLower());
            rd.FieldCount = mf.GameState.TotalFields;

            rd.LinkList = mf.GameState.getTotalLinkList();
            olvLinks.SetObjects(compressLinkList(rd.LinkList));

            rd.DestroyPortals = mf.getDestoryPortals();
            olvDestroy.SetObjects(rd.DestroyPortals);

            rd.RequireList = getRequireItems();
            olvRequire.SetObjects(rd.RequireList);

            olvRequire.Sort(olvRequireQuantity, SortOrder.Descending);
        }


        private void ReportForm_Load(object sender, EventArgs e) {
            loadPreview();

            JObject ob = JObject.FromObject(rd);
            string apidata = ob.ToString();

            putDataToProxy(apidata);
        }

        HttpWebRequest request;
        public void putDataToProxy(string data) {
            try {
                data = Base64Encode(data);
                string hash = CreateMD5(data + mf.ingressDatabase.Settings.EasyLinkPassword);
                string url = mf.ingressDatabase.Settings.EasyLinkProxyHost + "SetData";

                lProxyStatus.Text = string.Format("Starting uploading to {0}", url);

                request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.0.4) Gecko/20060508 Firefox/1.5.0.4";
                request.Timeout = 10000;
                request.Method = "POST";
                request.ContentType = "application/json";
                Stream dataStream = request.GetRequestStream();
                JObject obt = new JObject();
                obt["key"] = hash;
                obt["data"] = data;
                string reqdat = obt.ToString(Newtonsoft.Json.Formatting.None);
                byte[] dat = Encoding.ASCII.GetBytes(reqdat);
                dataStream.Write(dat, 0, dat.Length);
                dataStream.Close();
                request.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);
            } catch (Exception ex) {
                Lib.Logging.logException("", ex);
                lProxyStatus.Text = string.Format("Uploading failed...: {0}", ex.Message);
            }
        }

        void StartWebRequest() {
            request.BeginGetResponse(new AsyncCallback(FinishWebRequest), null);
        }

        void FinishWebRequest(IAsyncResult result) {
            try {
                request.EndGetResponse(result);

                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();

                reader.Close();
                dataStream.Close();
                response.Close();

                JObject ob = JObject.Parse(responseFromServer);
                string key = Lib.Converter.toString(ob["SetDataResult"]["Result"]);

                if (key.Length <= 0) {
                    Lib.Logging.log("Unable to get key from proxy!");
                    return;
                }
                lProxyStatus.Text = string.Format("Upload successfully!");

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(mf.ingressDatabase.Settings.EasyLinkProxyHost + "GetData/" + key, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                pbQrcode.Image = qrCode.GetGraphic(20);
            } catch (Exception ex) {
                Lib.Logging.logException("", ex);
                lProxyStatus.Text = string.Format("Uploading failed...: {0}", ex.Message);
            }
        }
    

        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData) {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public ListViewPrinter getListPrinter(ObjectListView olv) {
            ListViewPrinter ret = new ListViewPrinter();

            if(olv == olvLinks) {
                ret.Header = "Link list";
            } else if (olv == olvDestroy) {
                ret.Header = "Destory list";
            } else if (olv == olvRequire) {
                ret.Header = "Require list";
            }

            ret.IsShrinkToFit = true;
            ret.ListView = olv;

            return ret;
        }
        public static string CreateMD5(string input) {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create()) {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++) {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }



        private List<Link> compressLinkList(List<Link> input) {
            List<Link> ret = new List<Link>();
            
            for (int i = 0; i < input.Count; i++) {
                Link last = null;
                if (ret.Count > 0) last = ret[ret.Count - 1];
                if (last != null && last.P1 == input[i].P1) {
                    PortalInfo lastLinkP = last.P2;
                    Link tmpLink = new Link();
                    tmpLink.P1 = input[i].P1;
                    tmpLink.P2 = new PortalInfo();
                    tmpLink.P2 = new PortalInfo();
                    tmpLink.P2.Name = lastLinkP.Name + " + " + input[i].P2.Name;
                    ret.RemoveAt(ret.Count - 1);
                    ret.Add(tmpLink);
                } else {
                    ret.Add(input[i]);
                }
            }
            return ret;
        }

        public List<Item> getRequireItems() {
            Dictionary<string, Item> ret = new Dictionary<string, Item>();

            foreach (Link link in mf.GameState.getTotalLinkList()) {
                Item item = new Item("Key");
                item.Subname = link.P2.Name;
                if (!ret.ContainsKey(item.ItemKey)) ret[item.ItemKey] = item;
                ret[item.ItemKey].Quantity++;
            }
            foreach(PortalInfo p in rd.DestroyPortals) {
                if (p.Team == IngressTeam.None) continue;
                Item item = new Item("Weapon");

                if(p.Team != mf.ingressDatabase.Settings.Team) {
                    item.Subname = "Xmp";
                } else if(p.Team == IngressTeam.Resistance) {
                    item.Subname = "JARVIS Virus";
                } else if(p.Team == IngressTeam.Enlightened) {
                    item.Subname = "ADA Refactor";
                }
                
                if (!ret.ContainsKey(item.ItemKey)) ret[item.ItemKey] = item;
                ret[item.ItemKey].Quantity++;
            }
            return ret.Values.ToList();
        }

        public class Item {
            public string ItemName { get; set; }
            public string Subname { get; set; }
            public int Quantity { get; set; }

            public Item(string name) {
                this.ItemName = name;
            }

            public string ItemKey {
                get { return string.Join(";", this.ItemName, this.Subname); }
            }
        }

        // Author: http://www.csharp-examples.net/combine-multiple-printdocuments/
        public class MultiPrintDocument : PrintDocument {
            private PrintDocument[] _documents;
            private int _docIndex;
            private PrintEventArgs _args;

            // constructors
            public MultiPrintDocument(params PrintDocument[] documents) {
                _documents = documents;
            }

            // overidden methods
            protected override void OnBeginPrint(PrintEventArgs e) {
                base.OnBeginPrint(e);
                if (_documents.Length == 0)
                    e.Cancel = true;

                if (e.Cancel) return;

                _args = e;
                _docIndex = 0;  // reset current document index
                CallMethod(_documents[_docIndex], "OnBeginPrint", e);
            }

            protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e) {
                e.PageSettings = _documents[_docIndex].DefaultPageSettings;
                CallMethod(_documents[_docIndex], "OnQueryPageSettings", e);
                base.OnQueryPageSettings(e);
            }

            protected override void OnPrintPage(PrintPageEventArgs e) {
                CallMethod(_documents[_docIndex], "OnPrintPage", e);
                base.OnPrintPage(e);
                if (e.Cancel) return;
                if (!e.HasMorePages) {
                    CallMethod(_documents[_docIndex], "OnEndPrint", _args);
                    if (_args.Cancel) return;
                    _docIndex++;  // increments the current document index

                    if (_docIndex < _documents.Length) {
                        // says that it has more pages (in others documents)
                        e.HasMorePages = true;
                        CallMethod(_documents[_docIndex], "OnBeginPrint", _args);
                    }
                }
            }

            // use reflection to call protected methods of child documents
            private void CallMethod(PrintDocument document, string methodName, object args) {
                typeof(PrintDocument).InvokeMember(methodName,
                  BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                  null, document, new object[] { args });
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e) {
            loadPreview();
        }
        private void loadPreview() {
            if (tabControl1.SelectedTab == tabPreview) {
                printPreviewControl1.Document = new MultiPrintDocument(getListPrinter(olvLinks), getListPrinter(olvDestroy), getListPrinter(olvRequire));

            }
        }

        private void bPrint_Click(object sender, EventArgs e) {
            PrintDialog pdi = new PrintDialog();
            
            if (pdi.ShowDialog() == DialogResult.OK) {
                pdi.Document = printPreviewControl1.Document;
                printPreviewControl1.Document.PrinterSettings = pdi.PrinterSettings;
                printPreviewControl1.Document.Print();
            }
        }

        private void bGenPDF_Click(object sender, EventArgs e) {
            string pdfPrinter = "";
            foreach (String strPrinter in PrinterSettings.InstalledPrinters) {
                if (strPrinter.Contains("PDF")) {
                    pdfPrinter = strPrinter;
                }
            }
            if(pdfPrinter.Length <= 0) {
                MessageBox.Show("No pdf printer found!");
                return;
            }
            printPreviewControl1.Document.PrinterSettings.PrinterName = pdfPrinter;
            printPreviewControl1.Document.Print();
        }
    }


}
