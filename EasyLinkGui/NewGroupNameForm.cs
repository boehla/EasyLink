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
    public partial class NewGroupNameForm : Form {
        public NewGroupNameForm() {
            InitializeComponent();
        }

        public string GroupName {
            get { return this.tbGroupName.Text; }
            set {
                this.tbGroupName.Text = value;
                this.tbGroupName.SelectAll();
            }
        }

        private void bOk_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void bCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
