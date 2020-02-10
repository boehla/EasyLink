using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyLinkGui {
    public partial class AboutForm : Form {
        public AboutForm() {
            InitializeComponent();
        }

        private void llGithubLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
        }

        private void AboutForm_Load(object sender, EventArgs e) {
            lVersion.Text = getAppInfo();
        }
        static public string GetVersion() {
            return "0.5.0.5";
        }

        public static string getAppInfo() {
            return string.Format("Application {0}, Version {1}", "EasyLink", GetVersion());
        }
    }
}
