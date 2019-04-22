using BrightIdeasSoftware;
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
        bool cancel = false;

        enum BitmapIcon { normalPortalNeutral, normalPortalEn, normalPortalRes, inFieldPortal, notLinkableToAnchor, filtered, disabledPortalNeutral, disabledPortalEn, disabledPortalRes, selectPoly };
        Dictionary<BitmapIcon, Bitmap> bitMapBuffer = new Dictionary<BitmapIcon, Bitmap>();

        enum MapOverlay { gamePortals, gameLinks, gameFields, gameWay, externLinks, selected, disabled };
        Dictionary<MapOverlay, GMapOverlay> overLays = new Dictionary<MapOverlay, GMapOverlay>();

        ContextMenu context = null;

        GameState gs = new GameState();
        GamePrinter dr = null;
        List<GameState> nextSols = new List<GameState>();
        Lib.Options opts = new Lib.Options("dat\\BestField.json");

        List<GameState> sameBest = new List<GameState>();

        public IngressDatabase ingressDatabase = null;
        
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
        public SettingsDataset Settings { get; set; }

        public MainForm() {
            InitializeComponent();

            Lib.Logging.log("Applicatoin started..");
            try {
                opts.load();
            }catch(Exception ex) {
                Lib.Logging.logException("",ex);
            }
            
            
            dr = new GamePrinter(gs);

            ingressDatabase = new IngressDatabase("dat\\db.bin");
            Settings = ingressDatabase.getSettings();

           List <PortalInfo> nodes = new List<PortalInfo>();

            opts.addUIElement(nudThreadCount);
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




        

        SharedCalcData shared = new SharedCalcData();
        private void calculateBestMultiThread() {
            Lib.Logging.log("start calculateBestMultiThread");
            shared = new SharedCalcData();
            if (gs.Global.AnchorsPortals.Count == 2) {
                PortalInfo p1 = gs.Global.AnchorsPortals[0];
                PortalInfo p2 = gs.Global.AnchorsPortals[1];
                gs.addLink(p1.Guid, p2.Guid);
            }

            lock (shared) {
                if (shared.RunningThreads > 0) {
                    Lib.Logging.log("ignore calulate command because there are still some threads running!!");
                }

                shared.toDo.Add(0, gs);
                shared.allGamesViewed.Add(gs.GetLongHashCode(), false);
                shared.startCalc = DateTime.UtcNow;
            }

            List<PointD> allPoints = new List<PointD>();
            Dictionary<PointD, PortalInfo> allMap = new Dictionary<PointD, PortalInfo>();
            Dictionary<PointD, bool> anchors = new Dictionary<PointD, bool>();
            foreach (PortalInfo item in GameState.Global.AnchorsPortals) {
                anchors[item.Pos] = true;
                allMap[item.Pos] = item;
            }
            foreach (PortalInfo pInfo in this.GameState.PortalInfos) {
                if (!anchors.ContainsKey(pInfo.Pos)) {
                }
                allMap[pInfo.Pos] = pInfo;
                allPoints.Add(pInfo.Pos);
            }

            List<List<PortalInfo>> layers = new List<List<PortalInfo>>();
            while (allPoints.Count > GameState.Global.AnchorsPortals.Count) {
                layers.Add(new List<PortalInfo>());


                List<PointD> hull = ConvexHull.MakeConvexHull(allPoints);

                List<PointD> toDel = new List<PointD>();
                foreach (PointD p in allPoints) {
                    if (hull.Contains(p)) continue;
                    if (!geohelper.PointInPolygon(hull.ToArray(), p.X, p.Y)) toDel.Add(p);
                }
                foreach (PointD item in toDel) {
                    allPoints.Remove(item);
                }


                foreach (PointD hullp in hull) {
                    if (!anchors.ContainsKey(hullp)) {
                        allPoints.Remove(hullp);
                        layers[layers.Count - 1].Add(allMap[hullp]);
                    }
                }
                if (hull.Count <= 2) break;
            }
            SortedList<float, GameState> toDo = new SortedList<float, GameState>(new DuplicateKeyComparer<float>());
            toDo.Add(10000, gs.clone());
            GameState best = gs;
            for(int i = 0; i < layers.Count; i++) {
                List<PortalInfo> layerPortals = layers[layers.Count - 1 - i];
                List<GameState> newGames = new List<GameState>();
                foreach (GameState game in toDo.Values) {
                    PortalInfo nearest = null;
                    double nearestDist = double.MaxValue;
                    foreach (PortalInfo item in layerPortals) {
                    
                    
                        lock (shared) {
                            shared.lastHandled = game;
                        }
                        if (game.linkToAllAnchorsPossible(item)) {
                            double curDist = double.MaxValue * 0.9;
                            if(game.Parent.LastLinks.Count > 0) {
                                PortalInfo lastPortal = game.Parent.LastLinks[game.Parent.LastLinks.Count - 1].P1;
                                curDist = geohelper.calculateDistance(lastPortal, item);
                            }
                            if (nearestDist > curDist) {
                                nearest = item;
                                nearestDist = curDist;
                            }
                        }
                    }
                    if(nearest != null) {
                        game.linkToAllAnchors(nearest);
                        if (best.getAPScore() < game.getAPScore()) {
                            best = game;
                            lock (shared) {
                                shared.bestGame = best;
                            }
                        }
                            /*
                            GameState tmpGame = game.clone();
                            tmpGame.linkToAllAnchors(nearest);
                            newGames.Add(tmpGame);
                            if (best.getAPScore() < tmpGame.getAPScore()) {
                                best = tmpGame;
                                lock (shared) {
                                    shared.bestGame = best;
                                }
                            }*/
                        }
                }
                foreach (GameState ngame in newGames) {
                    //toDo.Add(ngame.TotalFields, ngame);
                }
                while (toDo.Count > 500) toDo.RemoveAt(toDo.Count - 1);
            }
            //gs = best;
            /*
            for (int i = 0; i < (int)nudThreadCount.Value; i++) {
                Thread d = new Thread(CalcThread);
                d.IsBackground = true;
                d.Priority = ThreadPriority.BelowNormal;


                lock (shared) {
                    shared.RunningThreads++;
                }
                d.Start();
            }
            */
        }
        static int nextthreadid = 0;
        public void CalcThread() {
            bool alreadyCountDown = false;
            int threadid = nextthreadid++;
            try {
                Lib.Logging.log("thread.txt", "Starting new Thread: " + threadid);
                int countEmpty = 0;
                while (!cancel) {
                    GameState curToDo = null;
                    lock (shared) {
                        int targetThreads = (int)nudThreadCount.Value;
                        while (targetThreads > shared.RunningThreads) {
                            Thread d = new Thread(CalcThread);
                            d.IsBackground = true;
                            d.Priority = ThreadPriority.BelowNormal;
                            shared.RunningThreads++;
                            d.Start();
                        }
                        if (shared.RunningThreads > targetThreads) {
                            alreadyCountDown = true;
                            shared.RunningThreads--;
                            return;
                        }
                        if (shared.toDo.Count > 0) {
                            curToDo = shared.toDo.ElementAt(0).Value;
                            shared.toDo.RemoveAt(0);
                            countEmpty = 0;
                        }
                    }
                    if (curToDo == null) {
                        Thread.Sleep(1000);
                        countEmpty++;
                        if (countEmpty > 5) return;
                        continue;
                    }
                    lock (shared) {
                        shared.lastHandled = curToDo;
                    }
                    Lib.Performance.setWatch("GetAllPossible", true);
                    List<GameState> nextgs = curToDo.getAllPossible();
                    Lib.Performance.setWatch("GetAllPossible", false);


                    foreach (GameState item in nextgs) {
                        long hash = item.GetLongHashCode();
                        float gamescore = (float)item.getGameScore();
                        float searchscore = (float)item.getSearchScore();
                        lock (shared) {
                            if (shared.bestGame == null || gamescore >= shared.bestVal) {
                                shared.bestVal = gamescore;
                                shared.bestGame = item;
                                shared.resultTime = DateTime.UtcNow;
                            } else {
                                if (shared.allGamesViewed.ContainsKey(hash)) continue;
                            }
                            shared.toDo.Add(searchscore, item);

                            while (shared.toDo.Count > 10000) {
                                shared.toDo.RemoveAt(shared.toDo.Count - 1);
                            }

                            shared.allGamesViewed[hash] = true;
                        }
                    }
                }
            } catch (Exception ex) {
                Lib.Logging.logException("", ex);
            } finally {
                Lib.Logging.log("thread.txt", "Stop thread: " + threadid);
                lock (shared) {
                    if (!alreadyCountDown) shared.RunningThreads--;
                    if (shared.RunningThreads == 0) {
                        shared = new SharedCalcData();
                    }
                }
            }
        }

        private void refresh() {
            refreshGoogleMaps();

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("TotalNodes: {0:n0}\r\n", gs.PortalInfos.Count));
            sb.Append(string.Format("TotalAP: {0:n0}\r\n", gs.getAPScore()));
            sb.Append(string.Format("TotalSearchScore: {0}\r\n", gs.getSearchScore()));
            sb.Append(string.Format("TotalGameScore: {0}\r\n", gs.getGameScore()));
            sb.Append(string.Format("TotalLinks: {0}\r\n", gs.TotalLinks));
            sb.Append(string.Format("TotalFields: {0}\r\n", gs.Fields.Count));
            sb.Append(string.Format("TotalArea: {0:0.0}km²\r\n", gs.TotalArea / 1000 / 1000));
            sb.Append(string.Format("TotalWay: {0:0}m\r\n", gs.TotalWay));
            lock (shared) {
                sb.Append(string.Format("CalcTime: {0}\r\n", Lib.Converter.formatTimeSpan(shared.resultTime - shared.startCalc)));

            }
            tbGameInfos.Text = sb.ToString();

            List<Link> linkList = gs.getTotalLinkList();

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
                overLays[suit] = new GMapOverlay();
                gmap.Overlays.Add(overLays[suit]);

                clbMapLayers.Items.Add(new MapLayerCheckItem(suit.ToString(), suit), opts.get("MapLayerCheckItem_" + (int)suit, true).BoolValue);
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


        private void bCalc_Click(object sender, EventArgs e) {
            //Thread d = new Thread(calculateBest);
            bCalc.Enabled = false;
            bCalcStop.Enabled = true;
            cancel = false;
            Thread d = new Thread(calculateBestMultiThread);
            d.Name = "calculateBest";
            d.IsBackground = true;
            d.Start();

        }
        GameState lastPrinted = null;
        private void timer_Tick(object sender, EventArgs e) {
            long totaltested = 0;
            double highestTodo = 0;
            int todocount = 0;
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
            if (gs.Parent != null) {
                gs = gs.Parent;
            }
        }

        private void pbDraw_SizeChanged(object sender, EventArgs e) {
            refresh();
        }
        IngressView mv = null;
        bool doClosing = false;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            doClosing = true;
            cancel = true;
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
            ingressDatabase.addEntities(entities);
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
                        string url = string.Format("http://www.simmamap.com/nominatim/reverse.php?format=json&lon={0}&lat={1}", Lib.Converter.toString(todo.X), Lib.Converter.toString(todo.Y));
                        var request = (HttpWebRequest)HttpWebRequest.Create(url);
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
            bool calcing = shared.RunningThreads == 0;
            bCalc.Enabled = calcing;
            bCalcStop.Enabled = !calcing;
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
                    marker.ToolTipText = ni.Name;
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
                        polygon.Stroke = new Pen(CoreEntity.getTeamColor(Settings.Team), 1);
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
                    polygon.Fill = new SolidBrush(Color.FromArgb(15, CoreEntity.getTeamColor(Settings.Team)));
                    polygon.Stroke = new Pen(CoreEntity.getTeamColor(Settings.Team), 1);
                    overLays[MapOverlay.gameFields].Polygons.Add(polygon);
                    // gmap.Overlays.Add(polyOverlay);
                }

                PortalInfo fromPortal = null;
                points = new List<PointLatLng>();
                foreach (Link link in gs.getTotalLinkList()) {
                    PortalInfo newPortal = link.P1;
                    if (fromPortal != null && newPortal != fromPortal) {
                        points.Add(new PointLatLng(newPortal.Pos.Y, newPortal.Pos.X));
                    }
                    fromPortal = link.P1;
                }
                overLays[MapOverlay.gameWay].Clear();
                GMapRoute todoroute = new GMapRoute(points, "mypolygon");
                todoroute.Stroke = new Pen(Color.Orange, 3);
                overLays[MapOverlay.gameWay].Routes.Add(todoroute);

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
                            marker.ToolTipText = pid.Name;
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
                        polygon.Stroke = new Pen(Color.DarkRed, 3);
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

                // only for debugging:
                List<PointD> allPoints = new List<PointD>();
                Dictionary<PointD, bool> anchors = new Dictionary<PointD, bool>();
                foreach (PortalInfo item in GameState.Global.AnchorsPortals) {
                    anchors[item.Pos] = true;
                }
                foreach (PortalInfo pInfo in this.GameState.PortalInfos) {
                    if (!anchors.ContainsKey(pInfo.Pos)){
                        if(gs.TotalLinks == 0) {
                            if (!gs.getPortalDataByGuid(pInfo.Guid).OutLinkPossible) continue;
                        } else {
                            if (gs.getPortalDataByGuid(pInfo.Guid).OutLinkPossible) continue;
                        }
                        
                        //if (!gs.linkToAllAnchorsPossible(pInfo)) continue;
                    }
                    allPoints.Add(pInfo.Pos);
                }

                List<PointD> tmphull = ConvexHull.MakeConvexHull(allPoints);

                if (tmphull.Count > 0 && anchors.Count == 1) {
                    List<Triangle> remainTriangles = new List<Triangle>();
                    Triangle tmptr = new Triangle();
                    tmptr.subHull = tmphull;
                    remainTriangles.Add(tmptr);

                    while (remainTriangles.Count > 0) {
                        Triangle tr = remainTriangles[0];
                        remainTriangles.RemoveAt(0);

                        points = new List<PointLatLng>();
                        foreach (PointD hullp in tr.subHull) {
                            points.Add(new PointLatLng(hullp.Y, hullp.X));
                            if (!anchors.ContainsKey(hullp)) allPoints.Remove(hullp);

                            GMapRoute tmp = new GMapRoute(new PointLatLng[] { new PointLatLng(hullp.Y, hullp.X), new PointLatLng(anchors.Keys.ToArray()[0].Y, anchors.Keys.ToArray()[0].X) }, "mypolygon");
                            tmp.Stroke = new Pen(Color.Blue, 1);
                            //overLays[MapOverlay.externLinks].Routes.Add(tmp);
                        }
                        List<PointD> toDel = new List<PointD>();
                        foreach (PointD p in allPoints) {
                            if (tr.subHull.Contains(p)) continue;
                            if (!geohelper.PointInPolygon(tr.subHull.ToArray(), p.X, p.Y)) toDel.Add(p);
                        }
                        foreach (PointD item in toDel) {
                            //allPoints.Remove(item);
                        }
                        points.Add(new PointLatLng(tr.subHull[0].Y, tr.subHull[0].X));
                        GMapRoute hullPoly = new GMapRoute(points, "mypolygon");
                        hullPoly.Stroke = new Pen(Color.Black, 1);
                        //overLays[MapOverlay.externLinks].Routes.Add(hullPoly);

                        while (!tr.subHull[0].Equals(anchors.Keys.ToArray()[0])) {
                            tr.subHull.Insert(0, tr.subHull[tr.subHull.Count - 1]);
                            tr.subHull.RemoveAt(tr.subHull.Count - 1);
                        }
                        for (int i = 1; i < tr.subHull.Count - 1; i++) {
                            List<PointD> inTriangle = new List<PointD>();
                            PointD p1 = tr.subHull[i];
                            PointD p2 = tr.subHull[i + 1];
                            PointD[] tri = new PointD[] { p1, p2, anchors.Keys.ToArray()[0] };
                            foreach (PointD point in allPoints) {

                                if (geohelper.PointInPolygon(tri, point.X, point.Y)) {
                                    inTriangle.Add(point);
                                }
                            }
                            if (inTriangle.Count < 2) {
                                if (inTriangle.Count == 1) {
                                    Triangle tmpt = new Triangle();
                                    tmpt.p1 = p1;
                                    tmpt.p2 = p2;
                                    tmpt.singlePoint = inTriangle[0];
                                    tr.subTriangles.Add(tmpt);
                                }
                                continue;
                            }
                            inTriangle.Add(anchors.Keys.ToArray()[0]);
                            List<PointD> subHull = ConvexHull.MakeConvexHull(inTriangle);
                            if (subHull.Count >= 3) {
                                Triangle tmpt = new Triangle();
                                tmpt.p1 = p1;
                                tmpt.p2 = p2;
                                tmpt.subHull = subHull;
                                remainTriangles.Add(tmpt);
                                tr.subTriangles.Add(tmpt);
                            } else {
                                Triangle tmpt = new Triangle();
                                tmpt.p1 = p1;
                                tmpt.p2 = p2;
                                tmpt.subHull = subHull;
                                tr.subTriangles.Add(tmpt);
                            }

                            points = new List<PointLatLng>();
                            foreach (PointD hullp in subHull) {
                                points.Add(new PointLatLng(hullp.Y, hullp.X));
                            }
                            points.Add(new PointLatLng(subHull[0].Y, subHull[0].X));
                            hullPoly = new GMapRoute(points, "mypolygon");
                            hullPoly.Stroke = new Pen(Color.Brown, 1);
                            //overLays[MapOverlay.externLinks].Routes.Add(hullPoly);
                        }
                    }
                    foreach (PortalInfo item in GameState.Global.pInfos) {
                        GameState.addLink(item.Guid, GameState.Global.AnchorsPortals[0].Guid);
                    }
                    linkFromTriangle(0, tmptr, this.GameState);
                }

                gmap.Refresh();

                refreshLinkList();
            }
        }

        void linkFromTriangle(int lvl, Triangle tr, GameState game) {
            foreach (Triangle triangle in tr.subTriangles) {
                linkFromTriangle(lvl + 1, triangle, game);
                if (lvl == 0 && triangle.p1.Valid && triangle.p2.Valid) { // total convex
                    int p1 = game.Global.PortalMappingPointD[triangle.p1];
                    int p2 = game.Global.PortalMappingPointD[triangle.p2];
                    game.addLink(p1, p2);
                }
            }
            if (tr.singlePoint.Valid) {
                int p1 = game.Global.PortalMappingPointD[tr.p1];
                int sp = game.Global.PortalMappingPointD[tr.singlePoint];
                game.addLink(sp, p1);
            }
            if(tr.subHull.Count > 0) {

                for (int i = 0; i < tr.subHull.Count; i++) {
                    
                    int hull = game.Global.PortalMappingPointD[tr.subHull[i]];
                    if (tr.p1.Valid) {
                        int p1 = game.Global.PortalMappingPointD[tr.p1];
                        game.addLink(hull, p1);
                    }

                    if(i > 0) {
                        int lasthull = game.Global.PortalMappingPointD[tr.subHull[i - 1]];
                        game.addLink(hull, lasthull);
                    }
                    foreach (Triangle item in tr.subTriangles) {
                        if (item.p2.Equals(tr.subHull[i])) {
                            if (item.singlePoint.Valid) {
                                int p2 = game.Global.PortalMappingPointD[item.p2];
                                int sp = game.Global.PortalMappingPointD[item.singlePoint];
                                game.addLink(p2, sp);
                            }
                        }
                    }
                }
                
                if (tr.p2.Valid) {
                    //int p1 = game.Global.PortalMappingPointD[tr.p1];
                    //int p2 = game.Global.PortalMappingPointD[tr.p2];
                    //game.addLink(p2, p1);
                    for (int i = 0; i < tr.subHull.Count; i++) {
                        int p2 = game.Global.PortalMappingPointD[tr.p2];
                        int hull = game.Global.PortalMappingPointD[tr.subHull[i]];
                        game.addLink(p2, hull);
                    }
                }

            }
        }

        class Triangle {
            public PointD p1;
            public PointD p2;

            public List<PointD> subHull = new List<PointD>();
            public List<Triangle> subTriangles = new List<Triangle>();

            public PointD singlePoint;
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
                PortalInfo ni = (PortalInfo)item.Tag;

                if (ingressDatabase.getByGuid(ni.Guid) != null) { // Check if we have this portal already in database, otherwise it is a tmp portal (existing link)
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
            GameState newgs = gs.clone();
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
            cancel = true;
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
            SettingsForm sf = new SettingsForm(Settings);
            if(sf.ShowDialog() == DialogResult.OK) {
                this.Settings = sf.getNewSettings();
                ingressDatabase.setSettings(this.Settings);
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
                polygon.Stroke = new Pen(CoreEntity.getTeamColor(Settings.Team), 1);
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
    class SharedCalcData {
        public float bestVal = 0;
        public GameState bestGame = null;
        public Dictionary<long, bool> allGamesViewed = new Dictionary<long, bool>();
        public SortedList<float, GameState> toDo = new SortedList<float, GameState>(new DuplicateKeyComparer<float>());

        public DateTime startCalc;
        public DateTime resultTime;

        public GameState lastHandled = null;

        public int RunningThreads = 0;
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
        static int p = 0;
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
