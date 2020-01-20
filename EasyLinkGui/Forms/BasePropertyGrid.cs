using EasyLinkLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyLinkGui {
    public partial class BaseSettingsForm : Form {

        public BaseSettingsForm(object settings) {
            InitializeComponent();

            pbSettings.SelectedObject = settings;
        }

        private void BOk_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
