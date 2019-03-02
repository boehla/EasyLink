using BrightIdeasSoftware;
using EasyLinkLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyLinkGui {
    public partial class ReportForm : Form {
        MainForm mf = null;
        List<PortalInfo> destroyPortals = null;
        public ReportForm(MainForm mf) {
            InitializeComponent();
            this.mf = mf;

            

            olvLinks.SetObjects(compressLinkList( mf.GameState.getTotalLinkList()));
            destroyPortals = mf.getDestoryPortals();
            olvDestroy.SetObjects(destroyPortals);
            olvRequire.SetObjects(getRequireItems());

            olvRequire.Sort(olvRequireQuantity, SortOrder.Descending);
        }

        private void ReportForm_Load(object sender, EventArgs e) {
            loadPreview();
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

        private List<Link> compressLinkList2(List<Link> input) {
            List<Link> ret = new List<Link>();

            if (mf.GameState.Global.Anchors.Count != 2) return input;
            for(int i = 0; i < input.Count; i++) {
                if(i < input.Count - 1 && input[i].P1 == input[i + 1].P1 && mf.GameState.Global.AnchorsPortals.Contains(input[i].P2) && mf.GameState.Global.AnchorsPortals.Contains(input[i + 1].P2)) {
                    Link tmpLink = new Link();
                    tmpLink.P1 = input[i].P1;
                    tmpLink.P2 = new PortalInfo();
                    tmpLink.P2.Name = "Anchors";
                    ret.Add(tmpLink);
                    i++;
                } else {
                    ret.Add(input[i]);
                }
            }
            return ret;
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
            foreach(PortalInfo p in destroyPortals) {
                if (p.Team == IngressTeam.None) continue;
                Item item = new Item("Wapeon");

                if(p.Team != mf.Settings.Team) {
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
                printPreviewControl1.Document.Print();
            } else {
                MessageBox.Show("Print Cancelled");
            }

            return;
            printPreviewControl1.Document.Print();
            printPreviewDialog1.Document = printPreviewControl1.Document;
            printPreviewDialog1.ShowDialog();
        }
    }


}
