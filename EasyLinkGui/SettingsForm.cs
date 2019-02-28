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
    public partial class SettingsForm : Form {
        private SettingsDataset settings = new SettingsDataset();

        public SettingsForm(SettingsDataset settings) {
            this.settings = settings;
            InitializeComponent();
        }

        private void bAccept_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void bCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e) {
            if (settings.Team == IngressTeam.Enlightened) rbEnlightened.Checked = true;
            else rbResistance.Checked = true;
        }

        public SettingsDataset getNewSettings() {
            if (rbEnlightened.Checked) settings.Team = IngressTeam.Enlightened;
            else settings.Team = IngressTeam.Resistance;
            return settings;
        }
    }
}
