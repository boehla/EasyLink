using EasyLinkLib;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasyLinkLib.geohelper;

namespace EasyLinkLib {
    public class PortalInfo {
        [BsonId]
        public string Guid { get; set; }

        public string Name { get; set; }
        public DateTime Lastrefresh { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public bool Enabled { get; set; }
        public bool ShowLabel { get; set; }

        public bool ReverseGeoCodingDone { get; set; }
        // "road":"Schwarzachtobelstraße","suburb":"Hof","village":"Alberschwende","county":"Bregenz","state":"Vorarlberg","postcode":"6861","country":"Austria"
        public string Road { get; set; }
        public string Suburb { get; set; }
        public string Village { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public int Postcode { get; set; }
        public string Country { get; set; }

        public string AddressName { get; set; }

        public IngressTeam Team { get; set; }
        public int Level { get; set; }
        public double Health { get; set; }
        public int ResCount { get; set; }
        public string Image { get; set; }
        public bool Mission { get; set; }

        public string MapTile { get; set; }

        [BsonIgnore]
        public PointD Pos {
            get { return new PointD(X, Y); }
            set {
                this.X = value.X;
                this.Y = value.Y;
            }
        }

        public override string ToString() {
            return this.Name;
        }

        public override bool Equals(object obj) {
            if (obj == null) return false;
            if (!(obj is PortalInfo)) return false;

            PortalInfo pi = (PortalInfo)obj;
            return this.Guid.Equals(pi.Guid);
        }

    }
    public class Group {
        [BsonId]
        public string Name { get; set; }

        public List<string> Guids { get; set; }

        public List<string> PreLinksP1 { get; set; }
        public List<string> PreLinksP2 { get; set; }

        public List<string> AnchorGuids { get; set; }

        public List<string> DestroyGuids { get; set; }
    }
    
    public class LinkEntityDataset {
        [BsonId]
        public string Guid { get; set; }

        public int Team { get; set; }

        public DateTime LastRefresh { get; set; }

        public double OPosX { get; set; }
        public double OPosY { get; set; }
        public string OGuid { get; set; }

        public double DPosX { get; set; }
        public double DPosY { get; set; }
        public string DGuid { get; set; }

        public string MapTile { get; set; }
    }
    public class SettingsDataset {
        [BsonId]
        public string SettingsName { get; set; }

        public IngressTeam Team { get; set; }
        public string EasyLinkProxyHost { get; set; }
        public string EasyLinkPassword { get; set; }

        public SettingsDataset() {
            this.SettingsName = "Default";
            this.Team = IngressTeam.Resistance;
            this.EasyLinkProxyHost = "http://hut.keinbrot.com:3950/";
            this.EasyLinkPassword = "n47Y7JUNDKpQ2y7EaQfd";
        }
    }
    public class IngressDatabase
    {
        LiteDatabase db = null;

        // Tables
        LiteCollection<PortalInfo> AllPortals = null;

        LiteCollection<Group> groups = null;
        LiteCollection<LinkEntityDataset> otherLinks = null;

        LiteCollection<SettingsDataset> settings = null;
        public IngressDatabase(string filename) {
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filename));
            db = new LiteDatabase(filename);
            var engine = db.Engine;
            if (engine.UserVersion == 0) { // first 
                engine.UserVersion = 1;
            }

            AllPortals = db.GetCollection<PortalInfo>("allportals");
            AllPortals.EnsureIndex(x => x.Guid);

            groups = db.GetCollection<Group>("groups");
            groups.EnsureIndex(x => x.Name);

            otherLinks = db.GetCollection<LinkEntityDataset>("otherlinks");
            otherLinks.EnsureIndex(x => x.Guid);

            settings = db.GetCollection<SettingsDataset>("settings");
            settings.EnsureIndex(x => x.SettingsName);
        }

        public void addEntities(List<CoreEntity> coreList) {
            List<PortalInfo> upportals = new List<PortalInfo>();
            foreach (CoreEntity ent in coreList) {
                if (ent is PortalEntity) {
                    PortalEntity pent = (PortalEntity)ent;
                    PortalInfo p = AllPortals.FindOne(x => x.Guid.Equals(ent.Guid));
                    if (p == null) {
                        p = new PortalInfo();
                        p.Enabled = false;
                        p.Guid = pent.Guid;
                        p.ShowLabel = false;

                        p.AddressName = "";
                        p.Country = "";
                        p.County = "";
                        p.Road = "";
                        p.Suburb = "";
                        p.Village = "";
                    }
                    p.Lastrefresh = DateTime.UtcNow;
                    p.Pos = pent.Pos;
                    p.Name = pent.Name;

                    p.Team = pent.Team;
                    p.Level = pent.Level;
                    p.Health = pent.Health;
                    p.ResCount = pent.ResCount;
                    p.Image = pent.Image;
                    p.Mission = pent.Mission;
                    p.MapTile = pent.MapTile;

                    upportals.Add(p);
                }
                if (ent is LinkEntity) {
                    LinkEntity le = (LinkEntity)ent;
                    otherLinks.Upsert(le.parseToDataset());
                    PortalInfo p = AllPortals.FindById(le.OGuid);
                    if (p != null) {
                        p.Team = le.Team;
                        AllPortals.Upsert(p);
                    }
                    p = AllPortals.FindById(le.DGuid);
                    if (p != null) {
                        p.Team = le.Team;
                        AllPortals.Upsert(p);
                    }


                }

            }
            AllPortals.Upsert(upportals);
        }
        public void ClearAll() {
            AllPortals.Delete(true);
        }
        public void ClearDisabled() {
            AllPortals.Delete(x => !x.Enabled);
        }

        public List<PortalInfo> getAllEnabled() {
            return AllPortals.Find(x => x.Enabled).ToList();
        }
        public List<PortalInfo> getAllDisabled() {
            return AllPortals.Find(x => !x.Enabled).ToList();
        }
        public List<PortalInfo> getAll() {
            return AllPortals.FindAll().ToList();
        }
        public PortalInfo getByGuid(string guid) {
            return AllPortals.FindById(guid);
        }

        public void updatePortals(List<PortalInfo> pi) {
            AllPortals.Update(pi);
        }
        public void updatePortals(PortalInfo pi) {
            AllPortals.Update(pi);
        }
       

        public List<Group> getAllGroups() {
            return groups.FindAll().ToList();
        }
        public void upsertGroup(Group g) {
            groups.Upsert(g);
        }
        public void deleteGroup(Group g) {
            groups.Delete(x => x.Name.Equals(g.Name));
        }
        public Group getGroup(string groupname) {
            return groups.FindOne(x => x.Name.Equals(groupname));
        }
        public void deleteAllOtherLinks() {
            otherLinks.Delete(true);
        }
        public List<CoreEntity> getAllOtherLinks() {
            otherLinks.Delete(x => x.LastRefresh.AddHours(12) < DateTime.UtcNow);
            List<CoreEntity> ret = new List<CoreEntity>();
            foreach (LinkEntityDataset ent in otherLinks.FindAll()) {
                ret.Add(new LinkEntity(ent));
            }
            return ret;
        }
        public void updateOtherLinks(List<CoreEntity> entities) {
            List<LinkEntityDataset> upsertList = new List<LinkEntityDataset>();
            foreach (CoreEntity entity in entities) {
                if(entity is LinkEntity) {
                    upsertList.Add(((LinkEntity)entity).parseToDataset());
                }
            }
            otherLinks.Upsert(upsertList);
        }

        public void makeBackup(string filename) { 
            //db.FileStorage.
        }

        public SettingsDataset getSettings() {
            SettingsDataset ret = settings.FindById("Default");
            if (ret == null) ret = new SettingsDataset();
            return ret;
        }
        public void setSettings(SettingsDataset settingsset) {
            settings.Upsert(settingsset);
        }
    }
}
