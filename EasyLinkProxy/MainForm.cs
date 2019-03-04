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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyLinkProxy {
    public partial class MainForm : Form {
        
       
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

        public void startWebService() {
            string text = "";
            try {
                EasyLinkService.Password = tbPassword.Text;
                string url = tbUrl.Text;
                var adrs = new Uri[1];
                adrs[0] = new Uri(url);

                try {
                    if (serviceHost != null) serviceHost.Close();
                }catch{ }

                serviceHost = new ServiceHost(typeof(EasyLinkService), adrs);
                serviceHost.Open();

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

}
