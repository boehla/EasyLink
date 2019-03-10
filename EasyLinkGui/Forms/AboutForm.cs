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
            Assembly assem = Assembly.GetEntryAssembly();
            AssemblyName assemName = assem.GetName();
            Version ver = assemName.Version;
            lVersion.Text = string.Format("Application {0}, Version {1}", assemName.Name, GetVersion());
        }
        static public string GetVersion() {
            if (ApplicationDeployment.IsNetworkDeployed) {
                return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }

            return "Debug";
        }
    }
}
