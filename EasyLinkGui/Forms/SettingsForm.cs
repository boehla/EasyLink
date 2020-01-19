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
        public SettingsDataset Settings {
            get {
                return (SettingsDataset)pbSettings.SelectedObject;
            }
            set {
                pbSettings.SelectedObject = value.Clone();
            }
        }

        public SettingsForm(SettingsDataset settings) {
            InitializeComponent();

            this.Settings = (SettingsDataset)settings.Clone();
        }

        private void bAccept_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            loadIntoSettings();
            this.Close();
        }

        private void bCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e) {
            loadIntoUI();
        }

        private void loadIntoUI() {
            if (Settings.Team == IngressTeam.Enlightened) rbEnlightened.Checked = true;
            else rbResistance.Checked = true;
        }
        public void loadIntoSettings() {
            if (rbEnlightened.Checked) Settings.Team = IngressTeam.Enlightened;
            else Settings.Team = IngressTeam.Resistance;
        }

        private void BDefaultValues_Click(object sender, EventArgs e) {
            this.Settings = new SettingsDataset();
            loadIntoUI();
        }
    }
}
