using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyLinkGui.Forms {
    public partial class SvgExport : Form {
        public enum ExportOptions { gameLinks, externLinks}

        public List<ExportOptions> Options {
            get {
                List<ExportOptions> ret = new List<ExportOptions>();

                if (cbExternLinks.Checked) ret.Add(ExportOptions.externLinks);
                if (cbGameLinks.Checked) ret.Add(ExportOptions.gameLinks);

                return ret;
            }
        }

        public SvgExport() {
            InitializeComponent();
        }

        private void Button2_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
