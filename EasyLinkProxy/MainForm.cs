using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyLinkProxy {
    public partial class MainForm : Form {
        [ServiceContract]
        public interface IDemoService {
            [OperationContract]
            [WebGet(UriTemplate = "GetData/{key}", ResponseFormat = WebMessageFormat.Json)]
            EasyWebResponse GetData(string key);

            [OperationContract]
            [WebInvoke(UriTemplate = "SetData", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json)]
            EasyWebResponse SetData(string key, string data);
        }
        public class DemoService : IDemoService {
            public static string Password = "";
            public static Lib.ValueBuffer Valbuffer = new Lib.ValueBuffer();

            public EasyWebResponse GetData(string key) {
                string ret = Lib.Converter.toString(Valbuffer.getVal(key));
                Lib.Logging.log("getdata.txt", string.Format("{0}: {1:n0}b", key, ret.Length));
                return new EasyWebResponse(ret);
            }

            public EasyWebResponse SetData(string hash, string data) {
                string key = CreateMD5(data + Password).ToLower();
                if (key.Equals(hash.ToLower())) {
                    Lib.Logging.log("setdata.txt", string.Format("{0}: {1:n0}b", key, data.Length));
                    Valbuffer.setVal(key, data, TimeSpan.FromHours(24));
                } else {
                    string text = string.Format("Wrong hash!");
                    Lib.Logging.log("setdata.txt", text);
                    return new EasyWebResponse(text, EasyWebResponse.EasyWebResponseErrors.wrongpassword);
                }
                return new EasyWebResponse(key);
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
        }

        public static Encoding encoding = Encoding.UTF8;

        ServiceHost serviceHost = null;

        Lib.Options opts = new Lib.Options("EasyNameProxy.json");

        public MainForm(string[] args) {
            InitializeComponent();

            Lib.Logging.log("Application started");

            opts.addUIElement(tbUrl);
            opts.addUIElement(tbPassword);
            opts.load();
            opts.loadUI();

            startWebService();
        }
        WebServiceHost host = null;
        public void startWebService() {
            string text = "";
            try {
                DemoService.Password = tbPassword.Text;
                string url = tbUrl.Text;

                try {
                    if (serviceHost != null) serviceHost.Close();
                } catch { }

                host = new WebServiceHost(typeof(DemoService), new Uri(url));
                WebHttpBinding bhb = new WebHttpBinding();
                bhb.MaxReceivedMessageSize = int.MaxValue;
                ServiceEndpoint ep = host.AddServiceEndpoint(typeof(IDemoService), bhb, "");
                host.Open();

                text = "Service successfully started on " + url;
            } catch (Exception ex) {
                Lib.Logging.logException("", ex);
                text = "Service error: " + ex.Message;
            }
            lStatus.Text = text;
            Lib.Logging.log(text);
        }

        private void bDebug_Click(object sender, EventArgs e) {
            Lib.Logging.showForm();
        }

        private void MainForm_Load(object sender, EventArgs e) {

        }

        private void bRestart_Click(object sender, EventArgs e) {
            startWebService();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            opts.saveUI();
            opts.saveIfNeeded();
        }
    }

    public class EasyWebResponse {
        public enum EasyWebResponseErrors { notfound = 1, wrongpassword }

        public int ErrorCode = 0;
        public string Error = "";
        public string Result = "";

        public EasyWebResponse() {
        }
        public EasyWebResponse(string result) {
            this.Result = result;
        }
        public EasyWebResponse(string error, EasyWebResponseErrors errorcode) {
            this.Error = error;
            this.ErrorCode = (int)errorcode;
        }
    }

}
