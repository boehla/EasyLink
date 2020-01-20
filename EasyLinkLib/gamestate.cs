using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasyLinkLib.geohelper;

namespace EasyLinkLib {
    public class GlobData {
        public List<PortalInfo> pInfos = new List<PortalInfo>();
        public Border borders = new Border();
        public LinkLookupTbl linkLookupTbl = null;

        //public List<int> Anchors = new List<int>();
        public List<PortalInfo> AnchorsPortals = new List<PortalInfo>();

        public Dictionary<string, int> PortalMapping = new Dictionary<string, int>();
        public Dictionary<PointD, int> PortalMappingPointD = new Dictionary<PointD, int>();
    }

    public class Link {
        public int Index;
        public PortalInfo P1;
        public PortalInfo P2;
        public Link(PortalInfo p1, PortalInfo p2) {
            this.P1 = p1;
            this.P2 = p2;
        }

        public override string ToString() {
            return string.Format("{0} -> {1}", P1, P2);
        }
        public Link() {

        }
    }

    public class GameState : ICloneable {
        // global Info
        GlobData glob = new GlobData();

        private List<Portal> pData = new List<Portal>();
        private List<Field> fields = new List<Field>();
        private Dictionary<Field, bool> fieldsDic = new Dictionary<Field, bool>();
        private int linkCount = 0;
        private float totalWay = 0;
        private float totalArea = 0;
        private bool _changed = false;

        private GameState parent = null;
        public List<Link> LastLinks { get; set; }
        

        public GameState Parent {
            get { return this.parent; }
            set { this.parent = value; }
        }

        public List<PortalInfo> PortalInfos{
            get {
                if (this.glob == null) return new List<PortalInfo>();
                return this.glob.pInfos; }
        }
        public List<Portal> PortalData {
            get { return this.pData; }
        }
        public GlobData Global {
            get { return this.glob; }
        }
        public Border Borders {
            get {
                if (this.glob == null) return null;
                return this.glob.borders; }
        }
        public List<Field> Fields {
            get { return this.fields; }
        }
        /// <summary>
        /// in m
        /// </summary>
        public float TotalWay {
            get { return this.totalWay; }
        }
        /// <summary>
        /// in m²
        /// </summary>
        public float TotalArea {
            get { return this.totalArea; }
        }
        public int TotalLinks {
            get { return linkCount; }
        }
        public int TotalFields {
            get { return this.fields.Count; }
        }

        public PortalInfo getPortalByGuid(string guid) {
            if (!Global.PortalMapping.ContainsKey(guid)) return null;
            return PortalInfos[Global.PortalMapping[guid]];
        }
        public int getIndexByGuid(string guid) {
            if (!Global.PortalMapping.ContainsKey(guid)) return -1;
            return Global.PortalMapping[guid];
        }
        public Portal getPortalDataByGuid(string guid) {
            if (!Global.PortalMapping.ContainsKey(guid)) return null;
            return pData[Global.PortalMapping[guid]];
        }
        public bool HasChanges {
            get {
                bool ret = _changed;
                _changed = false;
                return ret;
            }
        }

        public void loadPortals(List<PortalInfo> pInfos) {

            glob = new GlobData();
            this.glob.pInfos = pInfos;
            pData = new List<Portal>(pInfos.Count);
            fields = new List<Field>();
            glob.borders = new Border();
            glob.PortalMapping.Clear();
            for(int i = 0; i < pInfos.Count; i++) {
                pData.Add(new Portal());
                glob.borders.addValue(pInfos[i].Pos);
                glob.PortalMapping[pInfos[i].Guid] = i;
                glob.PortalMappingPointD[pInfos[i].Pos] = i;
            }
            glob.linkLookupTbl = new LinkLookupTbl(this);
        }
        public void loadGroup(Group p) {
            this.Global.AnchorsPortals.Clear();
            foreach (string item in p.AnchorGuids) {
                for (int i = 0; i < this.PortalInfos.Count; i++) {
                    if (this.PortalInfos[i].Guid.Equals(item)) {
                        this.Global.AnchorsPortals.Add(this.getPortalByGuid(item));
                        this.PortalData[i].KeysLeft = 1000000;
                    }
                }
            }

            if (p.PreLinksP1 == null || p.PreLinksP1.Count <= 0) return;
            Dictionary<string, int> cacheId = new Dictionary<string, int>();
            for(int i = 0; i < this.PortalInfos.Count; i++) {
                cacheId.Add(this.PortalInfos[i].Guid, i);
            }

            for(int i = 0; i < p.PreLinksP1.Count; i++) {
                string guid1 = p.PreLinksP1[i];
                string guid2 = p.PreLinksP2[i];
                if (guid1 == null || guid2 == null) continue;
                if(!cacheId.ContainsKey(guid1) || !cacheId.ContainsKey(guid2)) {
                    p.PreLinksP1.Clear();
                    p.PreLinksP2.Clear();
                    break;
                } else {
                    this.addLink(cacheId[guid1], cacheId[guid2]);
                }
                
            }
        }

        public List<Link> getTotalLinkList() {
            List<Link> ret = new List<Link>();
            GameState rec = this;
            while(rec != null) {
                if (rec.LastLinks != null) {
                    for (int i = 0; i < rec.LastLinks.Count; i++) {
                        ret.Insert(0, rec.LastLinks[rec.LastLinks.Count - 1 - i]);
                    }
                }
                rec = rec.parent;
            }
            for(int i = 0; i < ret.Count; i++) {
                ret[i].Index = i;
            }
            return ret;
        }
        public bool isLinkCrossing(PointD p1, PointD p2) {
            return geohelper.crossLink(this, p1, p2);
        }
        public bool checkLink(string p1id, string p2id) {
            return checkLink(glob.PortalMapping[p1id], glob.PortalMapping[p2id]);
        }
        public bool checkLink(int p1id, int p2id) {
            if (p1id == p2id) return false;
            if (p1id < 0 || p2id < 0) return false;
            if (p1id >= pData.Count || p2id >= pData.Count) return false;

            Portal p1 = this.pData[p1id];
            Portal p2 = this.pData[p2id];

            if (p2.KeysLeft <= 0) return false;
            if (p1.SideLinks.ContainsKey(p2id)) return false;
            if (!p1.OutLinkPossible) return false;

            //if(geohelper.crossLink(this, p1id, p2id)) return "cannot link because it would cross other link!";
            if (glob.linkLookupTbl.crossLink(this, p1id, p2id)) {
                if (Global.AnchorsPortals.Count == 2) {
                    //this.PortalData[p1id].InTriangle = true;
                }
                return false;
            }

            return true;
        }
        public bool linkToAllAnchorsPossible(PortalInfo pi) {
            foreach (PortalInfo anchor in Global.AnchorsPortals) {
                if (!checkLink(pi.Guid, anchor.Guid)) {
                    return false;
                }
            }
            return true;
        }
        public bool linkToAllAnchors(PortalInfo pi) {
            foreach (PortalInfo anchor in Global.AnchorsPortals) {
                if (!addLink(pi.Guid, anchor.Guid)) {
                    return false;
                }
            }
            return true;
        }
        public bool addLink(string p1id, string p2id) {
            if ((getIndexByGuid(p1id) < 0) && (getIndexByGuid(p2id) < 0)) return false;
            if (!Global.PortalMapping.ContainsKey(p1id)) return false;
            if (!Global.PortalMapping.ContainsKey(p2id)) return false;
            return addLink(Global.PortalMapping[p1id], Global.PortalMapping[p2id]);
        }
        public bool addLink(int p1id, int p2id) {
            if (!checkLink(p1id, p2id)) return false;

            _cachedHashcode = 0;

            Portal p1 = this.pData[p1id];
            Portal p2 = this.pData[p2id];

            p1.SideLinks.Add(p2id, true); // outside
            p1.Outlinkscount++;
            p2.SideLinks.Add(p1id, false); // inside
            linkCount++;

            p2.KeysLeft--;

            if(this.Parent != null && this.Parent.LastLinks != null && this.Parent.LastLinks.Count > 0) {
                PortalInfo lastPortal = this.Parent.LastLinks[this.Parent.LastLinks.Count - 1].P1;
                this.totalWay += (float)geohelper.calculateDistance(lastPortal, this.glob.pInfos[p1id]);
                p1.KeysLeft += 4;
            }
            if (this.LastLinks == null) this.LastLinks = new List<Link>();
            this.LastLinks.Add(new Link(this.glob.pInfos[p1id], this.glob.pInfos[p2id]));

            Field rightf = null;
            Field leftf = null;
            float rightsize = float.MaxValue;
            float leftsize = float.MaxValue;
            
            foreach (int oid in p1.SideLinks.Keys) {
                if (this.pData[oid].SideLinks.ContainsKey(p2id)) {
                    Field f = new Field(p1id, p2id, oid);
                    
                    int side = geohelper.findSide(this.glob.pInfos[p1id], this.glob.pInfos[p2id], this.glob.pInfos[oid]);
                    float size = (float)geohelper.calculateArea(this, f);
                    if(side == 1) {
                        if(rightf == null || size < rightsize) {
                            rightsize = size;
                            rightf = f;
                        }
                    }else if (side == -1) {
                        if (leftf == null || size < leftsize) {
                            leftsize = size;
                            leftf = f;
                        }
                    }

                }
            }
            foreach (Field f in new Field[] { rightf, leftf }) {
                if (f == null) continue;
                List<int> intr = geohelper.getPointsInArea(this, f);
                foreach (int item in intr) {
                    this.PortalData[item].InTriangle = true;
                }
                this.fields.Add(f);
                f.Size = geohelper.calculateArea(this, f);
                this.totalArea += (float)f.Size;
            }
            _changed = true;

            /*

            }*/

            return true;
        }

        public List<GameState> getAllPossible() {
            List<GameState> ret = new List<GameState>();

            GameState gs = this.DeepClone();
            gs.Parent = this;

            if(glob.AnchorsPortals != null && glob.AnchorsPortals.Count > 0) {
                if (glob.AnchorsPortals.Count == 2) {
                    for (int p1 = 0; p1 < this.pData.Count; p1++) {
                        if (!gs.pData[p1].OutLinkPossible) continue;
                        if (this.glob.AnchorsPortals.Contains(gs.PortalInfos[p1])) continue;
                        gs = this.DeepClone();
                        gs.Parent = this;
                        bool allSucessfully = true;
                        foreach (PortalInfo p2Portal in this.Global.AnchorsPortals) {
                            int p2 = Global.PortalMapping[p2Portal.Guid];
                            if (p1 == p2) continue;
                            
                            if (!gs.addLink(p1, p2)) {
                                allSucessfully = false;
                                break;
                            }
                        }
                        if (allSucessfully) {
                            ret.Add(gs);
                            gs = this.DeepClone();
                            gs.Parent = this;
                        } else {
                            gs = this.DeepClone();
                            gs.Parent = this;
                        }
                    }
                } else if (glob.AnchorsPortals.Count == 1) {
                    int anchor = glob.PortalMapping[glob.AnchorsPortals[0].Guid];
                    for (int p1 = 0; p1 < this.pData.Count; p1++) {
                        if (p1 == anchor) continue;
                        if (!gs.pData[p1].OutLinkPossible) continue;
                        if (gs.pData[p1].SideLinks.ContainsKey(anchor)) {
                            for (int p2 = 0; p2 < this.pData.Count; p2++) {
                                if (p1 == p2) continue;
                                if (gs.addLink(p1, p2)) {
                                    ret.Add(gs);
                                    gs = this.DeepClone();
                                    gs.Parent = this;
                                }
                            }
                        } else {
                            if (gs.addLink(p1, anchor)) {
                                ret.Add(gs);
                                gs = this.DeepClone();
                                gs.Parent = this;
                            }
                        }
                    }
                }
            } else {
                for (int p1 = 0; p1 < this.pData.Count; p1++) {
                    if (!gs.pData[p1].OutLinkPossible) continue;
                    for (int p2 = 0; p2 < this.pData.Count; p2++) {
                        if (p1 == p2) continue;
                        if (gs.addLink(p1, p2)) {
                            ret.Add(gs);
                            gs = this.DeepClone();
                            gs.Parent = this;
                        }
                    }
                }
            }


            return ret;
        }
        public object Clone() {
            GameState ret = (GameState)this.MemberwiseClone();

            ret.LastLinks = null;

            ret.pData = new List<Portal>();
            foreach (Portal nd in this.pData) {
                ret.pData.Add(nd.clone());
            }
            ret.fields = new List<Field>();
            ret.fieldsDic = new Dictionary<Field, bool>();
            foreach (Field gf in this.fields) {
                ret.fields.Add(gf);
                ret.fieldsDic.Add(gf, true);
            }

            ret.parent = this;

            return ret;
        }

        public GameState DeepClone() {
            return (GameState)this.Clone();
        }

        public override bool Equals(object obj) {
            if (obj == null) return false;
            if (!(obj is GameState)) return false;

            GameState gs = (GameState)obj;
            if (gs.linkCount != this.linkCount) return false;
            if (gs.fields.Count != this.fields.Count) return false;
            if (gs.pData.Count != this.pData.Count) return false;
            if (Math.Abs(gs.totalWay - this.totalWay) > 1) return false;

            for (int i = 0; i < this.pData.Count; i++) {
                Portal n1 = gs.pData[i];
                Portal n2 = this.pData[i];
                if (!n1.Equals(n2)) return false;
            }
            /*
            for (int i = 0; i < this.Fields.Count; i++) {
                if (!gs.fields[i].Equals(this.fields[i])) return false;
            }*/
            foreach(Field f in this.fields) {
                if (!gs.fieldsDic.ContainsKey(f)) return false;
            }
            return true;
        }

        public override int GetHashCode() {
            return (int)GetHashCode();
        }

        long _cachedHashcode = 0;
        public long GetLongHashCode() {
            if (_cachedHashcode != 0) return _cachedHashcode;
            long ret = 0;

            
            for (int i = 0; i < this.pData.Count; i++) {
                Portal n1 = this.pData[i];
                foreach (KeyValuePair<int, bool> linkp in n1.SideLinks) {
                    //if (!linkp.Value) continue;
                    ret ^= Zobrist.getValue("node" + i, linkp.Key);
                }
                
            }
            ret ^= Zobrist.getValue("linkcount", linkCount);
            
            foreach (Field f in this.fields) {
                ret ^= f.GetHashCode();
            }
            ret ^= totalWay.GetHashCode();
            _cachedHashcode = ret;
            return ret;
        }

        public int getAPScore() {
            int ret = 0;

            foreach (Portal nd in this.pData) {
                ret += (nd.Outlinkscount * 313);
            }
            ret += this.fields.Count * 1250;

            return ret;
        }

        public override string ToString() {
            return string.Format("way={0}; totalSearchScore: {1}; GameScore: {2}", this.totalWay, this.getSearchScore(), this.getAPScore());
        }
        public double getGameScore() {
            return this.getAPScore();
           // return getSearchScore();
        }

        public double CustSearchScore { get; set; } = double.MinValue;
        public double getSearchScore() {
            if (this.CustSearchScore > double.MinValue) return this.CustSearchScore;
            //int possibleSolls = this.getAllPossible().Count;

                //return TotalLinks * 100 - totalWay + this.getAPScore();
                //return this.getAPScore() - (totalWay);

                //return 1f * this.getAPScore() - totalWay - TotalLinks * 500;
                /*
                return -TotalWay;

                List<Link> llist = getTotalLinkList();
                int p1 = -1;
                int p2 = -1;
                foreach (Link item in llist) {
                    if(p1 == -1) {
                        p1 = item.P1;
                    }else if(p1 != item.P1 && p2 == -1) {
                        p2 = item.P1;
                    }
                }
                if(p1 != -1 && p2 != -1) {
                    return -geohelper.CalcDistance(PortalInfos[p1], PortalInfos[p2]);
                }
                /*
                if (this.getTotalLinkList()) {
                    int from = this.Parent.LastLinks[this.Parent.LastLinks.Count - 1].P1;
                    int to = this.LastLinks[this.Parent.LastLinks.Count - 1].P1;
                    return -geohelper.CalcDistance(PortalInfos[from], PortalInfos[to]);
                }*/
            return this.getAPScore(); // perfekt for 2 anchors
            //return -TotalWay + this.getAPScore();
            //return this.getAPScore() / totalWay;
            //return this.getAPScore() / (TotalLinks + 1); // perfekt for 2 anchors
            //return -TotalWay / (TotalLinks);

            int gen = 0;
            GameState gs = this;
            while(gs.Parent != null) {
                gen++;
                gs = gs.Parent;
            }
            return -totalWay / (TotalLinks / 2);
            //return totalArea  / 1000 / 1000 - totalWay / 100;
            //return totalArea / TotalLinks;
            
            //return this.Fields.Count * 10 - totalWay / 100 - TotalLinks / 5d; // really good for now  // 235-0;179-0;179-235
            //return this.Fields.Count * 15 - totalWay / 100 - TotalLinks / 2d;
            double tot = 0;
            for(int i = 1; i < this.fields.Count; i++) {
                //double sizediff = this.fields[i].Size - this.fields[i - 1].Size;
                //tot += sizediff;
            }

           

            int isBadPortal = 0;
            for(int i = 0; i < this.PortalInfos.Count; i++) {
                if (this.PortalData[i].SideLinks.Count <= 0 && this.PortalData[i].InTriangle) isBadPortal++;
            }

            return getAPScore() - totalWay;
            return -isBadPortal;

            //return - TotalWay + (isGoodPortal - TotalLinks / 2) * 10000;
        }

    }
    public class Portal {
        public int KeysLeft = 999;
        public bool InTriangle = false;
        public Dictionary<int, bool> SideLinks = new Dictionary<int, bool>();
        public int Outlinkscount = 0;

        public bool OutLinkPossible {
            get {
                if (InTriangle) return false;
                if (Outlinkscount >= 24) return false;
                return true;
            }
        }

        public Portal clone() {
            Portal ret = new Portal();

            ret.InTriangle = this.InTriangle;
            foreach (KeyValuePair<int, bool> item in this.SideLinks) {
                ret.SideLinks.Add(item.Key, item.Value);
            }
            ret.Outlinkscount = this.Outlinkscount;
            ret.KeysLeft = this.KeysLeft;
            return ret;
        }

        public override bool Equals(object obj) {
            if (obj == null) return false;
            if (!(obj is Portal)) return false;

            Portal nd = (Portal)obj;

            if (nd.InTriangle != this.InTriangle) return false;
            if (nd.SideLinks.Count != this.SideLinks.Count) return false;
            if (nd.Outlinkscount != this.Outlinkscount) return false;
            if (nd.KeysLeft != this.KeysLeft) return false;
            foreach (KeyValuePair<int, bool> link in nd.SideLinks) {
                if (!this.SideLinks.ContainsKey(link.Key) || this.SideLinks[link.Key] != nd.SideLinks[link.Key]) return false;
            }
            return true;
        }

        public override int GetHashCode() {
            int ret = 0;

            foreach (KeyValuePair<int, bool> link in this.SideLinks) {
                if (!link.Value) continue;
                ret ^=  (int)Zobrist.getValue("Node", link.Key);
            }
            ret ^= (int)Zobrist.getValue("KeysLeff", KeysLeft);

            ret = ret << 1;
            if (this.InTriangle) ret |= 1;

            return ret;
        }
    }
    public class Field {
        public int[] NodesIds;
        public double Size = -1;
        
        public Field(int p1, int p2, int p3) {
            List<int> nods = new List<int>() { p1, p2, p3 };
            nods.Sort();
            NodesIds = nods.ToArray();
        }

        public Field clone() {
            Field ret = new Field(NodesIds[0], NodesIds[1], NodesIds[2]);
            return ret;
        }

        public override bool Equals(object obj) {
            if (obj == null) return false;
            if (!(obj is Field)) return false;

            Field f = (Field)obj;

            if (f.NodesIds.Length != this.NodesIds.Length) return false;
            for(int i = 0; i < f.NodesIds.Length; i++) {
                if (f.NodesIds[i] != this.NodesIds[i]) return false;
            }
            return true;
        }

        public override int GetHashCode() {
            int ret = 0;

            for(int i = 0; i < this.NodesIds.Length; i++) {
                //ret += this.NodesIds[i] * 31;
                ret ^= (int)Zobrist.getValue("Field", this.NodesIds[i]);
            }
            return ret;
        }

        public override string ToString() {
            return string.Format("{0} / {1} / {2}", this.NodesIds[0], this.NodesIds[1], this.NodesIds[2]);
        }
    }
}
