﻿using BrightIdeasSoftware;
using EasyLinkLib;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using static EasyLinkLib.geohelper;

namespace EasyLinkGui {
    public partial class MainForm : Form {
        enum BitmapIcon { normalPortalNeutral, normalPortalEn, normalPortalRes, inFieldPortal, notLinkableToAnchor, filtered, disabledPortalNeutral, disabledPortalEn, disabledPortalRes, selectPoly };
        Dictionary<BitmapIcon, Bitmap> bitMapBuffer = new Dictionary<BitmapIcon, Bitmap>();

        enum MapOverlay {  gameLinks, gameFields, gameWay, externLinks, selected, disabled, carCharge, gamePortals };
        Dictionary<MapOverlay, GMapOverlay> overLays = new Dictionary<MapOverlay, GMapOverlay>();

        ContextMenu context = null;

        GameState gs = new GameState();
        List<GameState> nextSols = new List<GameState>();
        Lib.Options opts = new Lib.Options(Path.Combine(MainFolder, "BestField.json"));

        List<GameState> sameBest = new List<GameState>();

        public IngressDatabase ingressDatabase = null;

        public static string MainFolder {
            get {
                string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(folder, "EasyLink");
            }
        }
        
        public string LastGroupNameSave {
            get {
                return opts.get("LastGroupNameSave", "").Value;
            }
            set {
                if (value.Equals("AutoSave")) return;
                opts.set("LastGroupNameSave", value);
            }
        }
        public int MapProviderID {
            get {
                return opts.get("MapProvider", GMap.NET.MapProviders.GoogleMapProvider.Instance.DbId).IntValue;
            }
            set {
                opts.set("MapProvider", value);
            }
        }
        public GameState GameState {
            get { return this.gs; }
        }

        public MainForm() {
            InitializeComponent();

            Lib.Logging.log("Applicatoin started..");
            try {
                opts.load();
            } catch (Exception ex) {
                Lib.Logging.logException("", ex);
            }
            ingressDatabase = new IngressDatabase(Path.Combine(MainFolder,  "db.bin"));
            gs.loadSettings(ingressDatabase.Settings);

            List<PortalInfo> nodes = new List<PortalInfo>();

            opts.loadUI();

            Thread thread = new Thread(startReverseGeocode);
            thread.IsBackground = true;
            thread.Name = "startReverseGeocode";
            thread.Start();
        }

        public void checkHashcode(Dictionary<GameState, bool> test) {
            int cols = 0;
            Dictionary<int, GameState> hashcodes = new Dictionary<int, GameState>();
            foreach (GameState item in test.Keys) {
                if (hashcodes.ContainsKey(item.GetHashCode())) {
                    cols++;
                }
                hashcodes[item.GetHashCode()] = item;
            }
            Lib.Logging.log(string.Format("{0:0.0000%} Collisions", cols * 1d / test.Count));
        }

        private Bitmap getIcon(BitmapIcon b) {
            if (bitMapBuffer.ContainsKey(b)) return bitMapBuffer[b];
            Bitmap ret = null;

            int size = 10;
            switch (b) {
                case BitmapIcon.filtered:
                    ret = DrawFilledCircle(Color.Green, size * 2, size * 2);
                    break;
                case BitmapIcon.disabledPortalNeutral:
                    ret = DrawEmptyCircle(Color.FromArgb(150, Color.Black), size, size);
                    break;
                case BitmapIcon.disabledPortalEn:
                    ret = DrawEmptyCircle(Color.FromArgb(150, CoreEntity.getTeamColor(IngressTeam.Enlightened)), size, size);
                    break;
                case BitmapIcon.disabledPortalRes:
                    ret = DrawEmptyCircle(Color.FromArgb(150, CoreEntity.getTeamColor(IngressTeam.Resistance)), size, size);
                    break;
                case BitmapIcon.inFieldPortal:
                    ret = DrawFilledCircle(Color.DarkGray, size, size);
                    break;
                case BitmapIcon.notLinkableToAnchor:
                    ret = DrawFilledCircle(Color.LightPink, size, size);
                    break;
                case BitmapIcon.normalPortalEn:
                    ret = DrawFilledCircle(CoreEntity.getTeamColor(IngressTeam.Enlightened), size, size);
                    break;
                case BitmapIcon.normalPortalRes:
                    ret = DrawFilledCircle(CoreEntity.getTeamColor(IngressTeam.Resistance), size, size);
                    break;
                default:
                case BitmapIcon.normalPortalNeutral:
                    ret = DrawFilledCircle(Color.Black, size, size);
                    break;
            }
            bitMapBuffer[b] = ret;
            return ret;
        }       

        private void refresh() {
            refreshGoogleMaps();

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("TotalNodes: {0:n0}\r\n", gs.PortalInfos.Count));
            sb.Append(string.Format("TotalAP: {0:n0}\r\n", gs.getAPScore()));
            //sb.Append(string.Format("TotalSearchScore: {0}\r\n", gs.getSearchScore()));
            //sb.Append(string.Format("TotalGameScore: {0:n0}\r\n", gs.getGameScore()));
            sb.Append(string.Format("TotalLinks: {0:n0}\r\n", gs.TotalLinks));
            sb.Append(string.Format("TotalFields: {0:n0}\r\n", gs.Fields.Count));
            sb.Append(string.Format("TotalArea: {0:n1}km²\r\n", gs.TotalArea / 1000 / 1000));
            sb.Append(string.Format("TotalWay: {0:n0}m\r\n", gs.TotalWay));

            sb.Append(string.Format("MaxOutgoingLinks: {0:n0} of {1:n0}\r\n", gs.MaxOutgoingLinks, gs.MaxOutgoingLinksAllowed));
            sb.Append(string.Format("MaxIncomingLinks: {0:n0}\r\n", gs.MaxIncomingLinks));

            tbGameInfos.Text = sb.ToString();

            refreshPortals();
        }

        private void Form1_Load(object sender, EventArgs e) {
            lbMapProviders.Items.Add(new MapProvider(GMap.NET.MapProviders.GoogleMapProvider.Instance));
            lbMapProviders.Items.Add(new MapProvider(GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance));
            lbMapProviders.Items.Add(new MapProvider(GMap.NET.MapProviders.GoogleTerrainMapProvider.Instance));

            lbMapProviders.Items.Add(new MapProvider(GMap.NET.MapProviders.BingOSMapProvider.Instance));
            lbMapProviders.Items.Add(new MapProvider(GMap.NET.MapProviders.BingHybridMapProvider.Instance));
            lbMapProviders.Items.Add(new MapProvider(GMap.NET.MapProviders.BingSatelliteMapProvider.Instance));
            
            lbMapProviders.Items.Add(new MapProvider(GMap.NET.MapProviders.OpenStreetMapProvider.Instance));
            lbMapProviders.Items.Add(new MapProvider(GMap.NET.MapProviders.OpenStreet4UMapProvider.Instance));
            
            gmap.ShowCenter = false;
            gmap.MaxZoom = 19;
            gmap.MinZoom = 5;
            gmap.Zoom = 13;
            gmap.CanDragMap = true;
            gmap.DragButton = MouseButtons.Left;
            gmap.MouseWheelZoomEnabled = true;
            gmap.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            gmap.DisableFocusOnMouseEnter = true;
            gmap.IgnoreMarkerOnMouseWheel = true;
            gmap.MapScaleInfoEnabled = true;

            MapProvider curProv = null;
            foreach (MapProvider item in lbMapProviders.Items) {
                if (item.ID.Equals(MapProviderID)) {
                    gmap.MapProvider = item.Provider;
                    curProv = item;
                }
            }
            if(curProv == null) {
                curProv = (MapProvider)lbMapProviders.Items[0];
            }
            lbMapProviders.SelectedItem = curProv;
            gmap.MapProvider = curProv.Provider;

            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;

            gmap.Position = new PointLatLng((double)opts.get("gmap_pos_lat", 47.45043).DecimalValue, (double)opts.get("gmap_pos_lon", 9.83109).DecimalValue);
            gmap.Zoom = (double)opts.get("gmap_zoom", gmap.Zoom).DecimalValue;

            foreach (MapOverlay suit in (MapOverlay[])Enum.GetValues(typeof(MapOverlay))) {
                bool val = opts.get("MapLayerCheckItem_" + (int)suit, true).BoolValue;
                overLays[suit] = new GMapOverlay();
                overLays[suit].IsVisibile = val;
                gmap.Overlays.Add(overLays[suit]);
                
                clbMapLayers.Items.Add(new MapLayerCheckItem(suit.ToString(), suit), val);
            }
            loadGroup("AutoSave");
            refreshGroupList();
            addEntities(ingressDatabase.getAllOtherLinks());

            refreshDestryPortalList();
            
            refresh();
        }

        private Bitmap DrawFilledCircle(Color c, int x, int y) {
            Bitmap bmp = new Bitmap(x, y);
            int border = 1;
            using (Graphics graph = Graphics.FromImage(bmp)) {
                Rectangle ImageSize = new Rectangle(border, border, x - border * 2, y - border * 2);
                graph.FillEllipse(new SolidBrush(c), ImageSize);
            }
            return bmp;
        }
        private Bitmap DrawEmptyCircle(Color c, int x, int y) {
            Bitmap bmp = new Bitmap(x, y);
            int border = 1;
            using (Graphics graph = Graphics.FromImage(bmp)) {
                Pen p = new Pen(c);

                Rectangle ImageSize = new Rectangle(border, border, x - border * 2, y- border*2);
                graph.DrawEllipse(p, ImageSize);
            }
            return bmp;
        }

        private int searchNode(PointF pf) {
            int ret = -1;
            float minDist = float.MaxValue;
            for (int i = 0; i < gs.PortalData.Count; i++) {
                float dist = (float)((pf.X - gs.PortalInfos[i].Pos.X) * (pf.X - gs.PortalInfos[i].Pos.X) + (pf.Y - gs.PortalInfos[i].Pos.Y) * (pf.Y - gs.PortalInfos[i].Pos.Y));
                if (dist < minDist) {
                    minDist = dist;
                    ret = i;
                }
            }
            return ret;
        }


        private void onCalcFinishedMain(GameState gs) {
            if (gs != null) {
                this.gs = gs;
                refreshGoogleMaps();
            }
            
            bCalc.Enabled = true;
            bCalcStop.Enabled = false;
        }
        private void onNewBestGameMain(GameState gs) {
            if (gs == null) return;
            this.gs = gs;
            refreshGoogleMaps();
        }
        public void onCalculationFinished(GameState gs) {
            try {
                this.Invoke(new Action(() => onCalcFinishedMain(gs)));
            }catch(Exception ex) {
                Lib.Logging.logException("", ex);
            }
        }
        public void OnNewBestGameState(GameState gs) {
            try {
                this.Invoke(new Action(() => onNewBestGameMain(gs)));
            } catch (Exception ex) {
                Lib.Logging.logException("", ex);
            }
        }
        Algos.AlgoDummy autoLinkAlgo = null;
        private void bCalc_Click(object sender, EventArgs e) {
            //Thread d = new Thread(calculateBest);
            if (gs.Global.AnchorsPortals.Count == 1) {
                autoLinkAlgo = new Algos.OneAnchorMaxField();
            } else if (gs.Global.AnchorsPortals.Count == 2) {
                autoLinkAlgo = new Algos.Algo2Anchor();
            } else {
                autoLinkAlgo = new Algos.AlgoMaxField();
            }
            autoLinkAlgo.OnCalculationFinish += onCalculationFinished;
            autoLinkAlgo.OnNewBestGameState += OnNewBestGameState;
            autoLinkAlgo.startCalc(gs);


            bCalc.Enabled = false;
            bCalcStop.Enabled = true;
        }
        GameState lastPrinted = null;
        private void timer_Tick(object sender, EventArgs e) {
            long totaltested = 0;
            double highestTodo = 0;
            int todocount = 0;
            /* TODO
            lock (shared) {
                if (cbShowLastHandled.Checked && shared.lastHandled != null) {
                    gs = shared.lastHandled;
                } else if (shared.bestGame != null) {
                    gs = shared.bestGame;
                    //shared.bestGame = null;
                }
                totaltested = shared.allGamesViewed.Count;
                todocount = shared.toDo.Count;
                highestTodo = todocount > 0 ? shared.toDo.ElementAt(0).Value.getSearchScore() : -1;
            } */
            if (cbShowLastHandled.Checked) {
                if(autoLinkAlgo.LastHandled != null && this.gs != autoLinkAlgo.LastHandled) {
                    this.gs = autoLinkAlgo.LastHandled;
                    this.refreshGoogleMaps();
                }
            }
            tsslTotalTested.Text = string.Format("TotalTested: {0:n0}; TodoCount={1:n0} HighestTodo={2:n0}", totaltested, todocount, highestTodo);
            if (lastPrinted != gs || gs.HasChanges) {
                refresh();
                lastPrinted = gs;
                GameState rec = gs;
            }
            List<PortalInfo> refreshPortals = new List<PortalInfo>();
            lock (sharedGeo) {
                while (sharedGeo.finisedGeoPortals.Count > 0) {
                    refreshPortals.Add(sharedGeo.finisedGeoPortals[0]);
                    sharedGeo.finisedGeoPortals.RemoveAt(0);
                }
            }
            if (refreshPortals.Count > 0) olvPortals.RefreshObject(refreshPortals);
        }

        private string maxLength(string va, int max) {
            if (va.Length <= max) return va;
            return va.Substring(0, max);
        }

        private void bShowParent_Click(object sender, EventArgs e) {
            gs.removeLastLink();
            refresh();
        }

        private void pbDraw_SizeChanged(object sender, EventArgs e) {
            refresh();
        }
        IngressView mv = null;
        bool doClosing = false;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            doClosing = true;
            opts.saveUI();
            opts.saveIfNeeded();
            saveGroup("AutoSave");
            gmap.Dispose();
            //ingressDatabase.Close();
            Application.Exit();
        }
        private void bCreateGameState_Click(object sender, EventArgs e) {
            loadGameState();
        }
        private void loadGameState() {
            Group p = getGroup();
            p.PreLinksP1.Clear();
            p.PreLinksP2.Clear();

            gs = new GameState();
            gs.loadPortals(ingressDatabase.getAllEnabled());
            gs.loadGroup(p);
            gs.loadSettings(ingressDatabase.Settings);

            refreshAnchorList();
            refresh();
            refreshDisabledPortals();
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e) {
            ingressDatabase.ClearAll();
            refreshPortals();
        }

        private void clearDisabledToolStripMenuItem_Click(object sender, EventArgs e) {
            ingressDatabase.ClearDisabled();
            refreshPortals();
        }
        DateTime lastSearchInput = DateTime.UtcNow;
        private void tbDbSearch_KeyUp(object sender, KeyEventArgs e) {
            lastSearchInput = DateTime.UtcNow;
        }

        private void bDbInvert_Click(object sender, EventArgs e) {
            List<PortalInfo> changed = new List<PortalInfo>();
            for (int i = 0; i < olvPortals.Items.Count; i++) {
                OLVListItem item = (OLVListItem)olvPortals.Items[i];
                ((PortalInfo)item.RowObject).Enabled = !((PortalInfo)item.RowObject).Enabled;
                changed.Add(((PortalInfo)item.RowObject));
            }
            ingressDatabase.updatePortals(changed);
            refreshPortals();
        }

        private void bDbEnable_Click(object sender, EventArgs e) {
            List<PortalInfo> changed = new List<PortalInfo>();

            for (int i = 0; i < olvPortals.Items.Count; i++) {
                OLVListItem item = (OLVListItem)olvPortals.Items[i];
                ((PortalInfo)item.RowObject).Enabled = true;
                changed.Add(((PortalInfo)item.RowObject));
            }
            ingressDatabase.updatePortals(changed);
            refreshPortals();

        }
        List<PortalInfo> currentList = null;
        SharedGeoData sharedGeo = new SharedGeoData();
        private void refreshPortals() {
            currentList = ingressDatabase.getAll();
            olvPortals.SetObjects(currentList);
            lock (sharedGeo) {
                sharedGeo.openGeoPortals.Clear();
            }
            foreach (PortalInfo portal in currentList) {
                if (!portal.ReverseGeoCodingDone) {
                    lock (sharedGeo) {
                        sharedGeo.openGeoPortals.Add(portal);
                    }
                }
            }
            refreshDisabledPortals();
        }

        private void bDbDisable_Click(object sender, EventArgs e) {
            List<PortalInfo> changed = new List<PortalInfo>();
            for (int i = 0; i < olvPortals.Items.Count; i++) {
                OLVListItem item = (OLVListItem)olvPortals.Items[i];
                ((PortalInfo)item.RowObject).Enabled = false;
                changed.Add(((PortalInfo)item.RowObject));
            }
            ingressDatabase.updatePortals(changed);
            refreshPortals();
        }

        Dictionary<string, LinkEntity> externLinks = new Dictionary<string, LinkEntity>();
        public void addEntities(List<CoreEntity> entities) {
            Lib.Performance.setWatch("ingressDatabase.addEntities", true);
            ingressDatabase.addEntities(entities);
            Lib.Performance.setWatch("ingressDatabase.addEntities", false);
            bool refreshExternLinks = false;
            foreach (CoreEntity ent in entities) {
                if (ent is LinkEntity) {
                    externLinks[ent.Guid] = (LinkEntity)ent;
                    refreshExternLinks = true;
                }
            }
            if (refreshExternLinks) {
                refreshGoogleMaps();
                ingressDatabase.updateOtherLinks(entities);
            }
            refreshPortals();
        }

        private void startReverseGeocode() {

            while (!doClosing) {
                try {
                    PortalInfo todo = null;
                    lock (sharedGeo) {
                        if (sharedGeo.openGeoPortals.Count > 0) todo = sharedGeo.openGeoPortals[0];
                    }
                    if (todo != null) {
                        //string url = string.Format("https://nominatim.openstreetmap.org/reverse?format=json&lon={0}&lat={1}", Lib.Converter.toString(todo.X), Lib.Converter.toString(todo.Y));
                        string url = string.Format(ingressDatabase.Settings.OsmNominatimUrl, Lib.Converter.toString(todo.X), Lib.Converter.toString(todo.Y));
                        var request = (HttpWebRequest)HttpWebRequest.Create(url);
                        request.UserAgent = AboutForm.getAppInfo() + "; https://github.com/boehla/EasyLink";
                        request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                        string resp = StreamToString(request.GetResponse().GetResponseStream());
                        JObject ob = JObject.Parse(resp);
                        JToken obAdress = JObject.Parse(resp)["address"];
                        lock (sharedGeo) {
                            if (sharedGeo.openGeoPortals.Count > 0 && todo == sharedGeo.openGeoPortals[0]) {
                                todo.ReverseGeoCodingDone = true;
                                todo.AddressName = Lib.Converter.toString(ob["display_name"]);
                                todo.Country = Lib.Converter.toString(obAdress["country"]);
                                todo.Road = Lib.Converter.toString(obAdress["road"]);
                                todo.Postcode = Lib.Converter.toInt(obAdress["postcode"]);
                                todo.Suburb = Lib.Converter.toString(obAdress["suburb"]);
                                if (todo.Suburb.Length <= 0) todo.Suburb = Lib.Converter.toString(obAdress["state"]);
                                todo.County = Lib.Converter.toString(obAdress["county"]);
                                todo.Village = Lib.Converter.toString(obAdress["village"]);
                                if (todo.Village.Length <= 0) todo.Village = Lib.Converter.toString(obAdress["town"]);
                                if (todo.Village.Length <= 0) todo.Village = Lib.Converter.toString(obAdress["city"]);
                                if (todo.Village.Length <= 0) todo.Village = Lib.Converter.toString(obAdress["city_district"]);
                                todo.State = Lib.Converter.toString(obAdress["state"]);
                                ingressDatabase.updatePortals(todo);
                                sharedGeo.openGeoPortals.RemoveAt(0);
                                sharedGeo.finisedGeoPortals.Add(todo);
                            }
                        }
                    } else {
                        Thread.Sleep(10000);
                    }
                } catch (Exception ex) {
                    Lib.Logging.logException("", ex);
                } finally {
                    if (ingressDatabase.Settings.OsmNominatimUrl.Contains("openstreetmap.org")) {
                        Thread.Sleep(1000); // No heavy uses (an absolute maximum of 1 request per second).
                    }
                    //Thread.Sleep(2000);
                }
            }
        }
        public static string StreamToString(Stream stream) {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
                return reader.ReadToEnd();
            }
        }

        private void timerFast_Tick(object sender, EventArgs e) {
            if (lastSearchInput < DateTime.UtcNow.AddMilliseconds(-100)) {
                lastSearchInput = DateTime.MaxValue;
                string search = tbDbSearch.Text;

                if (search.Length > 0) {
                    olvPortals.ModelFilter = new ModelFilter(delegate (object x) {
                        PortalInfo myFile = x as PortalInfo;
                        return myFile.Name.ToLowerInvariant().Contains(search.ToLowerInvariant()) || (myFile.AddressName != null && myFile.AddressName.ToLowerInvariant().Contains(search.ToLowerInvariant()));
                    });
                } else {
                    olvPortals.ModelFilter = null;
                }
            }
            //bool calcing = shared.RunningThreads == 0; // TODO
            //bCalc.Enabled = calcing;
            //bCalcStop.Enabled = !calcing;
        }
        public static List<Color> GetGradientColors(Color start, Color end, int steps) {
            return GetGradientColors(start, end, steps, 0, steps - 1);
        }

        public static List<Color> GetGradientColors(Color start, Color end, int steps, int firstStep, int lastStep) {
            var colorList = new List<Color>();
            if (steps <= 0 || firstStep < 0 || lastStep > steps - 1)
                return colorList;

            double aStep = (double)(end.A - start.A) / steps;
            double rStep = (double)(end.R - start.R) / steps;
            double gStep = (double)(end.G - start.G) / steps;
            double bStep = (double)(end.B - start.B) / steps;

            for (int i = firstStep; i < lastStep; i++) {
                var a = start.A + (int)(aStep * i);
                var r = start.R + (int)(rStep * i);
                var g = start.G + (int)(gStep * i);
                var b = start.B + (int)(bStep * i);
                colorList.Add(Color.FromArgb(a, r, g, b));
            }

            return colorList;
        }
        Dictionary<string, PortalInfo> tmpPortals = new Dictionary<string, PortalInfo>();
        private void refreshGoogleMaps() {
            if (gs != null && gmap != null && gmap.Overlays.Count > 0) {
                overLays[MapOverlay.gamePortals].Clear();

                Dictionary<string, bool> portalsOnMap = new Dictionary<string, bool>();
                Dictionary<string, bool> filtered = new Dictionary<string, bool>();
                if (tbDbSearch.Text.Length > 0) {
                    for (int i = 0; i < olvPortals.Items.Count; i++) {
                        PortalInfo ni = (PortalInfo)((OLVListItem)olvPortals.Items[i]).RowObject;
                        filtered[ni.Guid] = true;
                    }
                }
                for (int i = 0; i < gs.PortalData.Count; i++) {
                    PortalInfo ni = gs.PortalInfos[i];
                    Bitmap img = getIcon(BitmapIcon.normalPortalNeutral);
                    if (ni.Team != IngressTeam.None && ni.Lastrefresh.AddDays(1) > DateTime.UtcNow) {
                        if (ni.Team == IngressTeam.Enlightened) {
                            img = getIcon(BitmapIcon.normalPortalEn);
                        } else if (ni.Team == IngressTeam.Resistance) {
                            img = getIcon(BitmapIcon.normalPortalRes);
                        }
                    }

                    if (filtered.ContainsKey(ni.Guid)) img = getIcon(BitmapIcon.filtered);
                    else if (gs.PortalData[i].InTriangle) {
                        img = getIcon(BitmapIcon.inFieldPortal);
                    } else if (gs.Global.AnchorsPortals.Count == 2) {
                        foreach (PortalInfo anchor in gs.Global.AnchorsPortals) {
                            if (!gs.checkLink(gs.PortalInfos[i].Guid, anchor.Guid)) {
                                img = getIcon(BitmapIcon.notLinkableToAnchor);
                                break;
                            }
                        }
                    }
                    GMarkerGoogle marker =  new GMarkerGoogle(new PointLatLng(ni.Pos.Y, ni.Pos.X), img);
                    //GmapMarkerWithLabel marker = new GmapMarkerWithLabel(new PointLatLng(ni.Pos.Y, ni.Pos.X), ni.Name, img, gmap);
                    portalsOnMap[ni.Guid] = true;
                    marker.Tag = ni;
                    string toolTip = ni.Name;
                    Portal pidDat = gs.PortalData[i];
                    if (pidDat != null) {
                        if (pidDat.OutgoingLinkCount > 0) toolTip += string.Format("\r\nOutlinks: {0}", pidDat.OutgoingLinkCount);
                        if (pidDat.IncomingLinkCount > 0) toolTip += string.Format("\r\nInlinks: {0}", pidDat.IncomingLinkCount);
                    }
                    marker.ToolTipText = toolTip;
                    marker.Offset = new Point(-img.Width / 2, -img.Height / 2);
                    //GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(ni.Pos.Y, ni.Pos.X), GMarkerGoogleType.blue_pushpin);
                    overLays[MapOverlay.gamePortals].Markers.Add(marker);
                }
                List<PointLatLng> points = new List<PointLatLng>();

                overLays[MapOverlay.gameLinks].Clear();
                Pen linkPen = new Pen(new SolidBrush(Color.Blue), 3);
                for (int i = 0; i < gs.PortalInfos.Count; i++) {
                    PortalInfo p1 = gs.PortalInfos[i];
                    foreach (KeyValuePair<int, bool> target in gs.PortalData[i].SideLinks) {
                        if (!target.Value) continue; // insideLink, draw only outside.

                        PortalInfo p2 = gs.PortalInfos[target.Key];

                        points = new List<PointLatLng>();
                        points.Add(new PointLatLng(p1.Pos.Y, p1.Pos.X));
                        points.Add(new PointLatLng(p2.Pos.Y, p2.Pos.X));
                        GMapRoute polygon = new GMapRoute(points, "mypolygon");
                        polygon.Stroke = new Pen(CoreEntity.getTeamColor(ingressDatabase.Settings.Team), 1);
                        overLays[MapOverlay.gameLinks].Routes.Add(polygon);
                    }
                }

                overLays[MapOverlay.gameFields].Clear();
                foreach (Field f in gs.Fields) {
                    points = new List<PointLatLng>();
                    foreach (int pid in f.NodesIds) {
                        PortalInfo p1 = gs.PortalInfos[pid];
                        points.Add(new PointLatLng(p1.Pos.Y, p1.Pos.X));
                    }
                    GMapPolygon polygon = new GMapPolygon(points, "mypolygon");
                    polygon.Fill = new SolidBrush(Color.FromArgb(15, CoreEntity.getTeamColor(ingressDatabase.Settings.Team)));
                    polygon.Stroke = new Pen(CoreEntity.getTeamColor(ingressDatabase.Settings.Team), 1);
                    overLays[MapOverlay.gameFields].Polygons.Add(polygon);
                    // gmap.Overlays.Add(polyOverlay);
                }
                overLays[MapOverlay.gameWay].Clear();
                
                
                PortalInfo lastPortal = null;
                points = new List<PointLatLng>();
                foreach (Link link in gs.getTotalLinkList()) {
                    PortalInfo newPortal = link.P1;
                    if (lastPortal == null || newPortal != lastPortal) {
                        points.Add(new PointLatLng(newPortal.Pos.Y, newPortal.Pos.X));
                    }
                    lastPortal = link.P1;
                }
                if (points.Count >= 2) {
                    List<Color> colorList = GetGradientColors(Color.Green, Color.Red, points.Count);
                    for (int i = 1; i < points.Count; i++) {
                        GMapRoute todoroute = new GMapRoute(new List<PointLatLng>() { points[i - 1], points[i] }, "mypolygon");
                        todoroute.Stroke = new Pen(colorList[i - 1], 3);
                        overLays[MapOverlay.gameWay].Routes.Add(todoroute);
                    }
                }               

                overLays[MapOverlay.externLinks].Clear();
                tmpPortals.Clear();
                int remainDestorys = 0;
                foreach (LinkEntity link in externLinks.Values) {
                    PortalInfo p1tmp = ingressDatabase.getByGuid(link.OGuid);
                    if (p1tmp == null) {
                        p1tmp = new PortalInfo();
                        p1tmp.Guid = p1tmp.Name = link.OGuid;
                        p1tmp.Pos = new geohelper.PointD(link.OPos.X, link.OPos.Y);
                        p1tmp.Team = link.Team;
                        tmpPortals[p1tmp.Guid] = p1tmp;
                    }
                    PortalInfo p2tmp = ingressDatabase.getByGuid(link.DGuid);
                    if (p2tmp == null) {
                        p2tmp = new PortalInfo();
                        p2tmp.Guid = p2tmp.Name = link.DGuid;
                        p2tmp.Pos = new geohelper.PointD(link.DPos.X, link.DPos.Y);
                        p2tmp.Team = link.Team;
                        tmpPortals[p2tmp.Guid] = p2tmp;
                    }

                    foreach (PortalInfo pid in new PortalInfo[] { p1tmp, p2tmp }) {
                        if (!portalsOnMap.ContainsKey(pid.Guid)) {
                            Bitmap icon = getIcon(BitmapIcon.disabledPortalNeutral);
                            if (pid.Team != IngressTeam.None && pid.Lastrefresh.AddDays(1) > DateTime.UtcNow) {
                                if (pid.Team == IngressTeam.Enlightened) {
                                    icon = getIcon(BitmapIcon.disabledPortalEn);
                                } else if (pid.Team == IngressTeam.Resistance) {
                                    icon = getIcon(BitmapIcon.disabledPortalRes);
                                }
                            }                       
                            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(pid.Pos.Y, pid.Pos.X), icon);
                            marker.Tag = pid;
                            string toolTip = "";
                            toolTip = pid.Name;
                            Portal pidDat = gs.getPortalDataByGuid(pid.Guid);
                            if (pidDat != null) {
                                if (pidDat.OutgoingLinkCount > 0) toolTip += string.Format("\r\nOutlinks: {0}", pidDat.OutgoingLinkCount); 
                                if (pidDat.IncomingLinkCount > 0) toolTip += string.Format("\r\nInlinks: {0}", pidDat.IncomingLinkCount);
                            }
                            marker.ToolTipText = toolTip;
                            marker.Offset = new Point(-icon.Width / 2, -icon.Height / 2);
                            //GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(ni.Pos.Y, ni.Pos.X), GMarkerGoogleType.blue_pushpin);
                            overLays[MapOverlay.externLinks].Markers.Add(marker);
                        }
                    }

                    points = new List<PointLatLng>();
                    points.Add(new PointLatLng(link.OPos.Y, link.OPos.X));
                    points.Add(new PointLatLng(link.DPos.Y, link.DPos.X));

                    bool isCrossing = gs.isLinkCrossing(link.OPos, link.DPos);

                    GMapRoute polygon = new GMapRoute(points, "mypolygon");
                    polygon.Stroke = null;

                    if (destroyPortals.ContainsKey(link.OGuid) || destroyPortals.ContainsKey(link.DGuid)) {
                        polygon.Stroke = new Pen(ChangeColorBrightness(CoreEntity.getTeamColor(link.Team), -0.5f), 3);
                        polygon.Stroke.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    } else if (isCrossing) {
                        remainDestorys++;
                        polygon.Stroke = new Pen(Color.Red, 3);
                    } else {
                        polygon.Stroke = new Pen(link.Color, 2);
                    }
                    lDestroyStatus.Text = string.Format("Remaining crossing links: {0}", remainDestorys);
                    lDestroyStatus.ForeColor = remainDestorys <= 0 ? Color.DarkGreen : Color.DarkRed;
                    overLays[MapOverlay.externLinks].Routes.Add(polygon);
                }

                gmap.Refresh();

                refreshLinkList();
            }
        }

        /// <summary>
        /// Creates color with corrected brightness.
        /// </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="Color"/> structure.
        /// </returns>
        public static Color ChangeColorBrightness(Color color, float correctionFactor) {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0) {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            } else {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }



        private void gmap_OnMapDrag() {
            saveGMap();
        }

        private void gmap_OnMapZoomChanged() {
            saveGMap();
        }

        private void saveGMap() {
            opts.set("gmap_pos_lat", gmap.Position.Lat);
            opts.set("gmap_pos_lon", gmap.Position.Lng);
            opts.set("gmap_zoom", gmap.Zoom);
            //opts.saveIfNeeded();
        }

        private void gmap_OnMarkerEnter(GMapMarker item) {
            if (item.Tag is PortalInfo) {
                PortalInfo ni = (PortalInfo)item.Tag;
                tsslMousePosition.Text = string.Format("{0}", ni.Name);
            }
        }

        string startPortal = "";
        private void gmap_OnMarkerClick(GMapMarker item, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                context = new ContextMenu();
                MenuItem mn = new MenuItem();
                if (!(item.Tag is PortalInfo)) return;
                PortalInfo ni = (PortalInfo)item.Tag;

                if (ingressDatabase.getByGuid(ni.Guid) != null) { // Check if we have this portal already in database, otherwise it is a tmp portal (existing link)
                    if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift)) {
                        linkToAnchors(ni.Guid);
                        return;
                    }

                    mn.Tag = item;
                    mn.Text = "Add as anchor";
                    mn.Click += Mn_AnchorClick;
                    context.MenuItems.Add(mn);

                    context.MenuItems.Add("-");

                    if (gs.Global.AnchorsPortals.Count > 0) {
                        mn = new MenuItem();
                        mn.Tag = item;
                        mn.Text = "Link to anchors";
                        mn.Click += Mn_LinkToAnchors;
                        context.MenuItems.Add(mn);

                        context.MenuItems.Add("-");
                    }

                    if (startPortal.Length > 0) {
                        PortalInfo startni = ingressDatabase.getByGuid(startPortal);
                        mn = new MenuItem();
                        mn.Tag = item;
                        mn.Text = "Link from " + startni.Name + " to here";
                        mn.Click += Mn_LinkToClick;
                        context.MenuItems.Add(mn);
                    }

                    mn = new MenuItem();
                    mn.Tag = item;
                    mn.Text = "Link from here";
                    mn.Click += Mn_LinkFromClick;
                    context.MenuItems.Add(mn);

                    context.MenuItems.Add("-");

                    mn = new MenuItem();
                    mn.Tag = item;
                    mn.Text = "Destroy";
                    mn.Click += Mn_DestroyClick;
                    context.MenuItems.Add(mn);

                    context.MenuItems.Add("-");

                    if (ni.Enabled) {
                        mn = new MenuItem();
                        mn.Tag = item;
                        mn.Text = "Disable";
                        mn.Click += Mn_DeactiveClick;
                        context.MenuItems.Add(mn);
                    } else {
                        mn = new MenuItem();
                        mn.Tag = item;
                        mn.Text = "Enable";
                        mn.Click += Mn_EnableClick;
                        context.MenuItems.Add(mn);
                    }
                } else {
                    mn = new MenuItem();
                    mn.Tag = item;
                    mn.Text = "Destroy";
                    mn.Click += Mn_DestroyClick;
                    context.MenuItems.Add(mn);
                }
                context.Show(gmap, e.Location);
            }
        }
        Dictionary<string, bool> anchors = new Dictionary<string, bool>();
        Dictionary<string, bool> destroyPortals = new Dictionary<string, bool>();
        private void refreshAnchorList() {
            olvDeleteColumn.AspectGetter = delegate {
                return "Delete";
            };
            List<PortalInfo> anchorsList = new List<PortalInfo>();
            foreach (string guid in anchors.Keys) {
                anchorsList.Add(ingressDatabase.getByGuid(guid));
            }
            if (gs == null || gs.Global == null) return;
            olvAnchors.SetObjects(anchorsList);
            GameState.loadGroup(getGroup());
        }
        private void refreshDestryPortalList() {
            olvDeleteColumn2.AspectGetter = delegate {
                return "Delete";
            };
            olvDestroy.SetObjects(getDestoryPortals());
            refreshGoogleMaps();
        }
        public List<PortalInfo> getDestoryPortals() {
            List<PortalInfo> destroyList = new List<PortalInfo>();
            foreach (string guid in destroyPortals.Keys) {
                PortalInfo ni = ingressDatabase.getByGuid(guid);
                if (ni == null && tmpPortals.ContainsKey(guid)) ni = tmpPortals[guid];
                if (ni == null) continue;
                destroyList.Add(ni);
            }
            return destroyList;
        }

        private void Mn_AnchorClick(object sender, EventArgs e) {
            if (sender is MenuItem) {
                MenuItem mn = (MenuItem)sender;
                GMapMarker marker = (GMapMarker)mn.Tag;
                PortalInfo pid = (PortalInfo)marker.Tag;
                anchors[pid.Guid] = true;
                refreshAnchorList();

                pid.Enabled = true;
                ingressDatabase.updatePortals(pid);
                loadGameState();
                refreshDisabledPortals();
            }
        }
        private void Mn_LinkFromClick(object sender, EventArgs e) {
            if (sender is MenuItem) {
                MenuItem mn = (MenuItem)sender;
                GMapMarker marker = (GMapMarker)mn.Tag;
                PortalInfo pid = (PortalInfo)marker.Tag;
                startPortal = pid.Guid;
            }
        }
        private void Mn_LinkToClick(object sender, EventArgs e) {
            if (sender is MenuItem) {
                MenuItem mn = (MenuItem)sender;
                GMapMarker marker = (GMapMarker)mn.Tag;
                PortalInfo pid = (PortalInfo)marker.Tag;
                if (gs.addLink(startPortal, pid.Guid)) refresh();
            }
        }
        private void Mn_DestroyClick(object sender, EventArgs e) {
            if (sender is MenuItem) {
                MenuItem mn = (MenuItem)sender;
                GMapMarker marker = (GMapMarker)mn.Tag;
                PortalInfo pid = (PortalInfo)marker.Tag;
                destroyPortals[pid.Guid] = true;
                refreshDestryPortalList();
            }
        }
        private void Mn_EnableClick(object sender, EventArgs e) {
            if (sender is MenuItem) {
                MenuItem mn = (MenuItem)sender;
                GMapMarker marker = (GMapMarker)mn.Tag;
                PortalInfo pid = (PortalInfo)marker.Tag;
                pid.Enabled = true;
                ingressDatabase.updatePortals(pid);
                loadGameState();
                refreshDisabledPortals();
            }
        }
        private void Mn_DeactiveClick(object sender, EventArgs e) {
            if (sender is MenuItem) {
                MenuItem mn = (MenuItem)sender;
                GMapMarker marker = (GMapMarker)mn.Tag;
                PortalInfo pid = (PortalInfo)marker.Tag;
                pid.Enabled = false;
                ingressDatabase.updatePortals(pid);
                loadGameState();

                if (anchors.ContainsKey(pid.Guid)) {
                    anchors.Remove(pid.Guid);
                    refreshAnchorList();
                }
            }
        }
        private void Mn_LinkToAnchors(object sender, EventArgs e) {
            if (sender is MenuItem) {
                MenuItem mn = (MenuItem)sender;
                GMapMarker marker = (GMapMarker)mn.Tag;
                PortalInfo pid = (PortalInfo)marker.Tag;
                linkToAnchors(pid.Guid);
            }
        }
        private void linkToAnchors(string id) {
            GameState newgs = gs.DeepClone();
            newgs.Parent = gs;
            bool allsuc = true;
            if (gs.Global.AnchorsPortals.Count == 2) {
                if (!gs.getPortalDataByGuid(gs.Global.AnchorsPortals[0].Guid).SideLinks.ContainsKey(gs.getIndexByGuid(gs.Global.AnchorsPortals[1].Guid))) {
                    allsuc &= newgs.addLink(gs.Global.AnchorsPortals[0].Guid, gs.Global.AnchorsPortals[1].Guid);
                }
            }
            foreach (PortalInfo lid in gs.Global.AnchorsPortals) {
                if (!newgs.addLink(id, lid.Guid)) {
                    allsuc = false;
                    break;
                }
            }
            if (allsuc) {
                gs = newgs;
                refresh();
            }
        }

        private void gmap_OnMarkerDoubleClick(GMapMarker marker, MouseEventArgs e) {
            PortalInfo pid = (PortalInfo)marker.Tag;
            if (!gs.Global.PortalMapping.ContainsKey(pid.Guid)) return;
            linkToAnchors(pid.Guid);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e) {

        }

        private void olv_ItemsChanged(object sender, ItemsChangedEventArgs e) {

        }

        private void olv_ItemChecked(object sender, ItemCheckedEventArgs e) {
            OLVListItem item = (OLVListItem)e.Item;
            ingressDatabase.updatePortals((PortalInfo)item.RowObject);
            refreshPortals();
        }
        
        
        private void bSaveGroup_Click(object sender, EventArgs e) {
            NewGroupNameForm ngnn = new NewGroupNameForm();
            if (LastGroupNameSave.Length > 0) ngnn.GroupName = LastGroupNameSave;
            if (ngnn.ShowDialog() == DialogResult.OK) {
                saveGroup(ngnn.GroupName);
            }
        }

        private void saveGroup(string groupname) {
            Group gp = ingressDatabase.getGroup(groupname);
            if (gp == null) {
                gp = new Group();
                gp.Name = groupname;
            }
            gp.Guids = new List<string>();
            foreach (PortalInfo portalInfo in ingressDatabase.getAllEnabled()) {
                gp.Guids.Add(portalInfo.Guid);
            }
            gp.AnchorGuids = new List<string>();
            foreach (string anguid in anchors.Keys) {
                gp.AnchorGuids.Add(anguid);
            }
            gp.DestroyGuids = destroyPortals.Keys.ToList();
            List<Link> linkList = gs.getTotalLinkList();
            gp.PreLinksP1 = new List<string>();
            gp.PreLinksP2 = new List<string>();
            for (int i = 0; i < linkList.Count; i++) {
                gp.PreLinksP1.Add(linkList[i].P1.Guid);
                gp.PreLinksP2.Add(linkList[i].P2.Guid);
            }
            LastGroupNameSave = groupname;
            ingressDatabase.upsertGroup(gp);
            refreshGroupList();
            refreshAnchorList();
        }
        private Group getGroup() {
            Group gp = new Group();
            gp.Name = "";
            gp.Guids = new List<string>();
            foreach (PortalInfo portalInfo in ingressDatabase.getAllEnabled()) {
                gp.Guids.Add(portalInfo.Guid);
            }
            gp.AnchorGuids = new List<string>();
            foreach (string anguid in anchors.Keys) {
                gp.AnchorGuids.Add(anguid);
            }
            gp.DestroyGuids = destroyPortals.Keys.ToList();
            List<Link> linkList = gs.getTotalLinkList();
            gp.PreLinksP1 = new List<string>();
            gp.PreLinksP2 = new List<string>();
            for (int i = 0; i < linkList.Count; i++) {
                gp.PreLinksP1.Add(linkList[i].P1.Guid);
                gp.PreLinksP2.Add(linkList[i].P2.Guid);
            }
            return gp;
        }
        private void loadGroup(string groupname) {
            Group p = ingressDatabase.getGroup(groupname);
            if (p == null) return;
            Dictionary<string, bool> enabledPortals = new Dictionary<string, bool>();
            foreach (string guid in p.Guids) {
                enabledPortals[guid] = true;
            }

            LastGroupNameSave = p.Name;
            List<PortalInfo> allP = ingressDatabase.getAll();
            foreach (PortalInfo portal in allP) {
                portal.Enabled = enabledPortals.ContainsKey(portal.Guid);
            }
            anchors.Clear();
            if (p.AnchorGuids != null) {
                foreach (string anguid in p.AnchorGuids) {
                    anchors[anguid] = true;
                }
            }
            destroyPortals.Clear();
            if (p.DestroyGuids != null) {
                foreach (string destroyPortal in p.DestroyGuids) {
                    destroyPortals[destroyPortal] = true;
                }
            }
            ingressDatabase.updatePortals(allP);
            gs = new GameState();
            gs.loadPortals(ingressDatabase.getAllEnabled());
            gs.loadGroup(p);
            gs.loadSettings(ingressDatabase.Settings);
            refreshAnchorList();
            refreshDestryPortalList();
            refresh();

            this.Text = "EasyLink - " + LastGroupNameSave;
        }
        private void refreshLinkList() {
            if (gs == null) return;
            olvLinks.SetObjects(gs.getTotalLinkList());
        }
        private void refreshGroupList() {
            olvDeleteColumn1.AspectGetter = delegate {
                return "Delete";
            };
            olvGroup.SetObjects(ingressDatabase.getAllGroups());

        }
        private void bLoadGroup_Click(object sender, EventArgs e) {
            Group p = (Group)olvGroup.SelectedItem.RowObject;
            loadGroup(p.Name);
        }

        private void olvGroup_SelectedIndexChanged(object sender, EventArgs e) {
            OLVListItem olv = olvGroup.SelectedItem;
            if (olv == null) return;
            LastGroupNameSave = ((Group)olv.RowObject).Name;
        }

        private void olvGroup_CellEditStarting(object sender, CellEditEventArgs e) {
            if (e.Column == olvDeleteColumn1) {
                e.Cancel = true;        // we don't want to edit anything
                Group pi = (Group)e.RowObject;
                ingressDatabase.deleteGroup(pi);
                refreshGroupList();
            } else if (e.Column == olvColumnGroupName) {
                e.Cancel = true;
                bLoadGroup_Click(sender, null);
            }
        }

        private void bCalcStop_Click(object sender, EventArgs e) {
            // TODO call algo cancel
            if (autoLinkAlgo != null) autoLinkAlgo.cancel();
        }

        private void tcMain_SelectedIndexChanged(object sender, EventArgs e) {
            if (tcMain.SelectedTab == tabMap) {
                refreshGoogleMaps();
            }
        }

        private void olvDestroy_ColumnClick(object sender, ColumnClickEventArgs e) {
            if (olvDestroy.Columns[e.Column] == olvDeleteColumn2) {
                destroyPortals.Clear();
                refreshDestryPortalList();
                refreshGoogleMaps();
            }
        }

        private void showDebugFormToolStripMenuItem_Click(object sender, EventArgs e) {
            Lib.Logging.showForm();
        }

        private void loadFromIntelMapToolStripMenuItem_Click(object sender, EventArgs e) {
            if (mv == null || mv.IsDisposed) mv = new IngressView(this);
            mv.Show();
            externLinks.Clear();
            ingressDatabase.deleteAllOtherLinks();
        }

        private void olv_FormatRow(object sender, FormatRowEventArgs e) {
            if (e.Item.RowObject is PortalInfo) {
                PortalInfo pi = (PortalInfo)e.Item.RowObject;
                Color c = Color.White;
                switch (pi.Team) {
                    case IngressTeam.Enlightened:
                        c = Color.LightGreen;
                        break;
                    case IngressTeam.Resistance:
                        c = Color.LightBlue;
                        break;
                }
                e.Item.BackColor = c;
            }
        }

        private void olvLinks_Resize(object sender, EventArgs e) {
            ObjectListView olv = (ObjectListView)sender;
            for (int i = 0; i < olv.Columns.Count - 1; i++) {
                olv.Columns[i].Width = olv.Width / olv.Columns.Count;
            }
        }

        ReportForm rf = null;
        private void generateReportToolStripMenuItem_Click(object sender, EventArgs e) {
            if(rf == null || rf.IsDisposed) rf = new ReportForm(this);
            //rf.ShowDialog();
            rf.Show();
        }
         
        private void editToolStripMenuItem_Click(object sender, EventArgs e) {
            SettingsForm sf = new SettingsForm(ingressDatabase.Settings);
            if(sf.ShowDialog() == DialogResult.OK) {
                ingressDatabase.Settings = sf.Settings;
                gs.loadSettings(sf.Settings);
            }
        }

        AboutForm abForm = null;
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            if (abForm == null || abForm.IsDisposed) abForm = new AboutForm();
            abForm.Show();
        }

        private void olvAnchors_CellClick(object sender, CellClickEventArgs e) {
            if (e.Column == olvDeleteColumn) {
                PortalInfo pi = (PortalInfo)e.Item.RowObject;
                anchors.Remove(pi.Guid);
                refreshAnchorList();
            }
        }

        private void olvDestroy_CellClick(object sender, CellClickEventArgs e) {
            if (e.Column == olvDeleteColumn2) {
                PortalInfo pi = (PortalInfo)e.Item.RowObject;
                if (pi == null) return;
                destroyPortals.Remove(pi.Guid);
                refreshDestryPortalList();
                refreshGoogleMaps();
            }
        }

        private void Mn_AddPolyClick(object sender, EventArgs e) {
            if (sender is MenuItem) {
                MenuItem mn = (MenuItem)sender;
                PointLatLng pos = (PointLatLng)mn.Tag;
                tmpPoly.Add(pos);
                refreshMarkPoly();
            }
        }
        private void Mn_DeletePolyClick(object sender, EventArgs e) {
            if (sender is MenuItem) {
                tmpPoly.Clear();
                refreshMarkPoly();
            }
        }
        private void Mn_DisableInPolyClick(object sender, EventArgs e) {
            if (sender is MenuItem) {
                PointD[] poly = new PointD[tmpPoly.Count];
                for(int i = 0; i < tmpPoly.Count; i++) {
                    poly[i] = new PointD(tmpPoly[i].Lng, tmpPoly[i].Lat);
                }
                for(int i = 0; i < gs.PortalInfos.Count; i++) {
                    PortalInfo pid = gs.PortalInfos[i];
                    if (geohelper.PointInPolygon(poly, pid)) {
                        pid.Enabled = false;
                        ingressDatabase.updatePortals(pid);
                    }
                }
                loadGameState();
            }
        }
        private void Mn_EnableInPolyClick(object sender, EventArgs e) {
            if (sender is MenuItem) {
                PointD[] poly = new PointD[tmpPoly.Count];
                Border b = new Border();
                for (int i = 0; i < tmpPoly.Count; i++) {
                    poly[i] = new PointD(tmpPoly[i].Lng, tmpPoly[i].Lat);
                    b.addValue(poly[i]);
                }
                List<PortalInfo> allPortals = ingressDatabase.getAll();
                foreach (PortalInfo pid in allPortals) {
                    bool isOut = b.isOutside(pid.Pos);
                    if (!isOut) isOut = !geohelper.PointInPolygon(poly, pid);
                    if(!isOut && !pid.Enabled) {
                        pid.Enabled = true;
                        ingressDatabase.updatePortals(pid);
                    }
                }
                loadGameState();
            }
        }
        List<PointLatLng> tmpPoly = new List<PointLatLng>();
        private void gmap_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                context = new ContextMenu();

                PointLatLng p = gmap.FromLocalToLatLng(e.X, e.Y);

                MenuItem mn = new MenuItem();
                mn = new MenuItem();
                mn.Tag = p;
                mn.Text = "Add to poly";
                mn.Click += Mn_AddPolyClick;
                context.MenuItems.Add(mn);

                mn = new MenuItem();
                mn.Text = "Delete Poly";
                mn.Click += Mn_DeletePolyClick;
                context.MenuItems.Add(mn);

                context.MenuItems.Add("-");

                if (tmpPoly.Count >= 3) {
                    mn = new MenuItem();
                    mn.Text = "Disable in poly";
                    mn.Click += Mn_DisableInPolyClick;
                    context.MenuItems.Add(mn);
                }

                if (tmpPoly.Count >= 3) {
                    mn = new MenuItem();
                    mn.Text = "Enable in poly";
                    mn.Click += Mn_EnableInPolyClick;
                    context.MenuItems.Add(mn);
                }

                context.Show(gmap, e.Location);
            }
        }
        private void refreshDisabledPortals() {
            overLays[MapOverlay.disabled].Clear();
            
            foreach (PortalInfo ni in ingressDatabase.getAllDisabled()) {
                Bitmap img = getIcon(BitmapIcon.disabledPortalNeutral);
                if (ni.Team != IngressTeam.None && ni.Lastrefresh.AddDays(1) > DateTime.UtcNow) {
                    if (ni.Team == IngressTeam.Enlightened) {
                        img = getIcon(BitmapIcon.disabledPortalEn);
                    } else if (ni.Team == IngressTeam.Resistance) {
                        img = getIcon(BitmapIcon.disabledPortalRes);
                    }
                }

                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(ni.Pos.Y, ni.Pos.X), img);
                marker.Tag = ni;
                marker.ToolTipText = ni.Name;
                marker.Bitmap = img;
                marker.Offset = new Point(-img.Width / 2, -img.Height / 2);
                overLays[MapOverlay.disabled].Markers.Add(marker);
            }
        }
        private void refreshMarkPoly() {
            overLays[MapOverlay.selected].Clear();

            foreach (PointLatLng pid in tmpPoly) {
                GMarkerGoogle marker = new GMarkerGoogle(pid, getIcon(BitmapIcon.selectPoly));
                marker.Tag = pid;
                overLays[MapOverlay.selected].Markers.Add(marker);
            }

            if (tmpPoly.Count > 2) {
                GMapPolygon polygon = new GMapPolygon(tmpPoly, "mypolygon");
                polygon.Stroke = null;

                polygon.Fill = new SolidBrush(Color.FromArgb(30, Color.Yellow));
                polygon.Stroke = new Pen(CoreEntity.getTeamColor(ingressDatabase.Settings.Team), 1);
                overLays[MapOverlay.selected].Polygons.Add(polygon);
            }
        }

        private void gmap_MouseDoubleClick(object sender, MouseEventArgs e) {
            PointLatLng pt = gmap.FromLocalToLatLng(e.X, e.Y);

            //gmap.Position = pt;

            if (e.Button.Equals(MouseButtons.Left)) {
                gmap.Zoom += 1;
            } else if (e.Button.Equals(MouseButtons.Right)) {
                gmap.Zoom -= 1;
            }
        }

        private void bToogleLayers_Click(object sender, EventArgs e) {
            clbMapLayers.Visible = !clbMapLayers.Visible;
        }

        private void clbMapLayers_ItemCheck(object sender, ItemCheckEventArgs e) {
            MapLayerCheckItem mo = (MapLayerCheckItem)clbMapLayers.Items[e.Index];
            bool ischeckd = e.NewValue == CheckState.Checked;
            opts.set("MapLayerCheckItem_" + (int)mo.Overlay, ischeckd);
            overLays[mo.Overlay].IsVisibile = ischeckd;
        }

        class MapLayerCheckItem {
            public string Name = "";
            public MapOverlay Overlay;

            public MapLayerCheckItem(string name, MapOverlay overlay) {
                this.Name = name;
                this.Overlay = overlay;
            }

            public override string ToString() {
                return this.Name;
            }
        }

        private void bToogleProviders_Click(object sender, EventArgs e) {
            lbMapProviders.Visible = !lbMapProviders.Visible;
        }

        private void lbMapProviders_SelectedValueChanged(object sender, EventArgs e) {
            MapProvider pro = (MapProvider)lbMapProviders.SelectedItem;
            gmap.MapProvider = pro.Provider;
            MapProviderID = pro.ID;
        }

        private void ExportSvgToolStripMenuItem_Click(object sender, EventArgs e) {
            Forms.SvgExport svgExport = new Forms.SvgExport();

            if (svgExport.ShowDialog() == DialogResult.Cancel) return;

            saveFileDialogExportSvg.AddExtension = true;
            saveFileDialogExportSvg.Filter = "Vector grafic (*.svg)|*.svg";
            saveFileDialogExportSvg.FileName = LastGroupNameSave + ".svg";
            saveFileDialogExportSvg.DefaultExt = ".svg";
            if (saveFileDialogExportSvg.ShowDialog() != DialogResult.OK) return;

            StreamWriter fout = new StreamWriter(saveFileDialogExportSvg.FileName);

            PointD refPoint = new PointD(gmap.ViewArea.LocationTopLeft.Lng, gmap.ViewArea.LocationTopLeft.Lat);
            PointD rightBottom = convertPoint(refPoint, new PointD(gmap.ViewArea.LocationRightBottom.Lng, gmap.ViewArea.LocationRightBottom.Lat));
            fout.WriteLine(string.Format(CultureInfo.InvariantCulture, @"<?xml version=""1.0"" encoding=""UTF-8""?>
<svg xmlns=""http://www.w3.org/2000/svg""
	version=""1.1"" baseProfile=""full""
	width=""700px"" height=""400px"" viewBox=""{0} {1} {2} {3}"">
", 0, 0, rightBottom.X, rightBottom.Y));

            List<EportLinkData> links = new List<EportLinkData>();

            foreach (Forms.SvgExport.ExportOptions item in svgExport.Options) {
                switch (item) {
                    case Forms.SvgExport.ExportOptions.externLinks:
                        foreach (LinkEntity link in externLinks.Values) {
                            if (!pointInRec(gmap.ViewArea, link.DPos) && !pointInRec(gmap.ViewArea, link.OPos)) continue;
                            //PointD start = convertPoint(refPoint, link.OPos);
                            //PointD end = convertPoint(refPoint, link.DPos);
                            links.Add(new EportLinkData(link.OPos, link.DPos));
                        }
                        break;
                    case Forms.SvgExport.ExportOptions.gameLinks:
                        foreach (Link link in gs.getTotalLinkList()) {
                            if (!pointInRec(gmap.ViewArea, link.P1.Pos) && !pointInRec(gmap.ViewArea, link.P2.Pos)) continue;
                            //PointD start = convertPoint(refPoint, link.P1.Pos);
                            //PointD end = convertPoint(refPoint, link.P2.Pos);
                            links.Add(new EportLinkData(link.P1.Pos, link.P2.Pos));
                        }
                        break;
                }
            }
            PointD[] viewRec = new PointD[4];
            viewRec[0] = new PointD(gmap.ViewArea.LocationRightBottom.Lng, gmap.ViewArea.LocationRightBottom.Lat);
            viewRec[1] = new PointD(gmap.ViewArea.LocationTopLeft.Lng, gmap.ViewArea.LocationRightBottom.Lat);
            viewRec[2] = new PointD(gmap.ViewArea.LocationTopLeft.Lng, gmap.ViewArea.LocationTopLeft.Lat);
            viewRec[3] = new PointD(gmap.ViewArea.LocationRightBottom.Lng, gmap.ViewArea.LocationTopLeft.Lat);
            foreach (EportLinkData link in links) {
                if (!geohelper.PointInPolygon(viewRec, link.Origin)) {
                    for (int i = 0; i < 4; i++) {
                        Vector vector = geohelper.FindIntersection(viewRec[i % viewRec.Length], viewRec[(i + 1) % viewRec.Length], link.Origin, link.Destination);
                        if (vector != null) {
                            link.Origin = vector.toPointD();
                            break;
                        }
                    }

                }
                if (!geohelper.PointInPolygon(viewRec, link.Destination)) {
                    for (int i = 0; i < 4; i++) {
                        Vector vector = geohelper.FindIntersection(viewRec[i % viewRec.Length], viewRec[(i + 1) % viewRec.Length], link.Origin, link.Destination);
                        if (vector != null) {
                            link.Destination = vector.toPointD();
                            break;
                        }
                    }

                }
                PointD start = convertPoint(refPoint, link.Origin);
                PointD end = convertPoint(refPoint, link.Destination);
                fout.WriteLine(string.Format(CultureInfo.InvariantCulture, @"<line x1=""{0}"" y1=""{1}"" x2=""{2}"" y2=""{3}"" stroke=""black"" stroke-width=""1px""/>", start.X, start.Y, end.X, end.Y));
            }
            fout.Write("</svg>");
            fout.Close();

        }
        class EportLinkData {
            public PointD Origin { get; set; }
            public PointD Destination { get; set; }

            public EportLinkData(PointD origin, PointD dest) {
                this.Origin = origin;
                this.Destination = dest;
            }
        }

        private PointD convertPoint(PointD refPoint, PointD p) {
                double distX = CalcDistance(refPoint.X, refPoint.Y, p.X, refPoint.Y);
                double distY = CalcDistance(refPoint.X, refPoint.Y, refPoint.X, p.Y);
                if (refPoint.X - p.X > 0) distX *= -1;
                if (refPoint.Y - p.Y < 0) distY *= -1;

                return new PointD(distX, distY);
        }
        private bool pointInRec(RectLatLng rec, PointD p) {
            RectLatLng other = new RectLatLng(new PointLatLng(p.Y, p.X), new SizeLatLng(0, 0));
            return rec.IntersectsWith(other);
        }
        private void Gmap_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyCode == Keys.Back) {
                bShowParent_Click(null, null);
            }
        }

        private void BRefreshCharging_Click(object sender, EventArgs e) {
            double sw_lat = gmap.ViewArea.LocationRightBottom.Lat;
            double sw_lng = gmap.ViewArea.LocationTopLeft.Lng;
            double ne_lat = gmap.ViewArea.LocationTopLeft.Lat;
            double ne_lng = gmap.ViewArea.LocationRightBottom.Lng;

            string url = string.Format(CultureInfo.InvariantCulture, "https://api.goingelectric.de/chargepoints/?key={0}&sw_lat={1}&sw_lng={2}&ne_lat={3}&ne_lng={4}", "0ed798d8663dee421b81ec1971100cde", sw_lat, sw_lng, ne_lat, ne_lng);


            //url = string.Format(CultureInfo.InvariantCulture, "https://api.goingelectric.de/chargepoints/?key={0}&lat={1}&lng={2}&radius=20", "0ed798d8663dee421b81ec1971100cde", gmap.ViewArea.LocationMiddle.Lat, gmap.ViewArea.Lng);

            string data = Lib.HttpRequest.makeHttpRequestGet(url);
            JObject ob = JObject.Parse(data);

            if (Lib.JsonExtensions.IsNullOrEmpty(ob["chargelocations"])) return;

            GMapOverlay ov = overLays[MapOverlay.carCharge];
            ov.Clear();

            foreach (JObject item in ob["chargelocations"]) {
                double lat = Lib.Converter.toDouble(item["coordinates"]["lat"]);
                double lon = Lib.Converter.toDouble(item["coordinates"]["lng"]);

                string name = Lib.Converter.toString(item["name"]);
                string network = Lib.Converter.toString(item["network"]);

                name += "(" + network + ")";

                GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lat, lon), GMap.NET.WindowsForms.Markers.GMarkerGoogleType.blue_pushpin);
                //GmapMarkerWithLabel marker = new GmapMarkerWithLabel(new PointLatLng(ni.Pos.Y, ni.Pos.X), ni.Name, img, gmap);
                //marker.Tag = ni;
                marker.ToolTipText = name;
                //marker.Offset = new Point(-img.Width / 2, -img.Height / 2);
                //GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(ni.Pos.Y, ni.Pos.X), GMarkerGoogleType.blue_pushpin);
                ov.Markers.Add(marker);
            }
        }

        private void ClearExernLinksToolStripMenuItem_Click(object sender, EventArgs e) {
            ingressDatabase.deleteAllOtherLinks();
            externLinks.Clear();
            refreshGoogleMaps();
        }

        private void CbShowLastHandled_CheckedChanged(object sender, EventArgs e) {
            if (!cbShowLastHandled.Checked) {
                if (autoLinkAlgo.Best != null && this.gs != autoLinkAlgo.Best) {
                    this.gs = autoLinkAlgo.Best;
                    this.refreshGoogleMaps();
                }
            }
        }

        private void BAlgoSettings_Click(object sender, EventArgs e) {
            if (autoLinkAlgo == null) return;
            BaseSettingsForm bSettings = new BaseSettingsForm(autoLinkAlgo.Settings);
            bSettings.ShowDialog();
        }
    }
    public class DuplicateKeyComparer<TKey>
        :
     IComparer<TKey> where TKey : IComparable {
        #region IComparer<TKey> Members

        public int Compare(TKey x, TKey y) {
            int result = x.CompareTo(y) * -1;

            if (result == 0)
                return 1;   // Handle equality as beeing greater
            else
                return result;
        }

        #endregion
    }
    
    class SharedGeoData {
        public List<PortalInfo> openGeoPortals = new List<PortalInfo>();
        public List<PortalInfo> finisedGeoPortals = new List<PortalInfo>();
    }
    public class GmapMarkerWithLabel : GMarkerGoogle, ISerializable {
        private Font font;
        private GMarkerGoogle innerMarker;
        private GMapControl gmap = null;
        private int markerHeight = 0;

        public string Caption;

        public GmapMarkerWithLabel(PointLatLng p, string caption, Bitmap img, GMapControl gmap)
            : base(p, img) {
            font = new Font("Arial", 10);
            innerMarker = new GMarkerGoogle(p, img);
            this.gmap = gmap;
            markerHeight = img.Height;

            Caption = caption;
        }
        public override void OnRender(Graphics g) {
            base.OnRender(g);
            if(gmap.Zoom >= 17) g.DrawString(Caption, font, Brushes.Black, new PointF(this.LocalPosition.X, this.LocalPosition.Y + markerHeight));
            return;
            
        }

        public override void Dispose() {
            if (innerMarker != null) {
                innerMarker.Dispose();
                innerMarker = null;
            }

            base.Dispose();
        }

        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
        }

        protected GmapMarkerWithLabel(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        #endregion
    }

    public class MapProvider {
        public int ID {
            get { return this.Provider.DbId; }
        }
        public string Description;
        public GMap.NET.MapProviders.GMapProvider Provider;

        public MapProvider(GMap.NET.MapProviders.GMapProvider provider) {
            this.Provider = provider;
        }
        public MapProvider(string desc, GMap.NET.MapProviders.GMapProvider provider) {
            this.Description = desc;
            this.Provider = provider;
        }

        public override string ToString() {
            if (!Lib.Converter.isEmpty(this.Description)) return this.Description;
            return Provider.Name;
        }
    }

}
