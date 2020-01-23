using EasyLinkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasyLinkLib.geohelper;

namespace EasyLinkGui.Algos {
    class Algo2Anchor : AlgoDummy {

        internal override GameState getBestGame(GameState gs) {
            List<PortalInfo> allPoints = new List<PortalInfo>();

            if (gs.Global.AnchorsPortals.Count != 2) return gs;

                int p1 = gs.getIndexByGuid(gs.Global.AnchorsPortals[0].Guid);
                int p2 = gs.getIndexByGuid(gs.Global.AnchorsPortals[1].Guid);
                if (!gs.PortalData[p1].SideLinks.ContainsKey(p2)) {
                    gs.addLink(p1, p2);
                    gs = gs.DeepClone();
                }

            double midX = (gs.Global.AnchorsPortals[1].Pos.X + gs.Global.AnchorsPortals[1].Pos.X) / 2;
            double midY = (gs.Global.AnchorsPortals[0].Pos.Y + gs.Global.AnchorsPortals[1].Pos.Y) / 2;

            Vector a1 = new Vector(gs.Global.AnchorsPortals[0].Pos);
            Vector a2 = new Vector(gs.Global.AnchorsPortals[1].Pos);

            Vector a1a2 = a2 - a1;
            Vector a2a1 = a1 - a2;

            List<PortalInfo> side1 = new List<PortalInfo>();
            List<PortalInfo> side2 = new List<PortalInfo>();
            foreach (PortalInfo pInfo in gs.PortalInfos) {
                if (pInfo.Guid.Equals(gs.Global.AnchorsPortals[0].Guid)) continue;
                if (pInfo.Guid.Equals(gs.Global.AnchorsPortals[1].Guid)) continue;

                allPoints.Add(pInfo);

                switch(geohelper.findSide(pInfo, gs.Global.AnchorsPortals[0], gs.Global.AnchorsPortals[1])) {
                    case -1:
                        side1.Add(pInfo);
                        break;
                    case 1:
                        side2.Add(pInfo);
                        break;
                    default:
                        throw new Exception("what a point on the line???");
                }
            }
            if (side1.Count > side2.Count) allPoints = side1;
            else allPoints = side2;

            AnchorAngle[] aDists = new AnchorAngle[allPoints.Count];

            int c = 0;
            foreach (PortalInfo pInfo in allPoints) {
                Vector p = new Vector(pInfo.Pos);

                double angleA1 = Math.Abs(a1a2.getAngleBetween(a1 - p));
                double angleA2 = Math.Abs((a2 - p).getAngleBetween(a2a1)); // not sure if this Math.abs is save=!=!

                aDists[c] = new AnchorAngle(angleA1, angleA2);

                if (pInfo.Guid.Equals("e87a5eca8d9943978ef63fae2ef76f1b.16")) {

                }

                c++;
            }
            Dictionary<int, List<int>> tree = new Dictionary<int, List<int>>();
            for (int i1 = 0; i1 < allPoints.Count; i1++) {
                tree[i1] = new List<int>();
            }

            for (int i1 = 0; i1 < allPoints.Count; i1++) {
                for (int i2 = i1 + 1; i2 < allPoints.Count; i2++) {
                    AnchorAngle dist1 = aDists[i1];
                    AnchorAngle dist2 = aDists[i2];
                    if (dist1.BothLower(dist2)) {
                        tree[i1].Add(i2);
                    }
                    if (dist2.BothLower(dist1)) {
                        tree[i2].Add(i1);
                    }
                }
            }

            List<int> exitNodes = new List<int>();
            for (int i1 = 0; i1 < allPoints.Count; i1++) {
                if(tree[i1].Count == 0) {
                    exitNodes.Add(i1);
                    //gs.addLink(gs.Global.AnchorsPortals[0].Guid, allPoints[i1].Guid);
                }
            }
            int bestDept = 0;
            for(int startNode = 0; startNode < allPoints.Count; startNode++) { 

                int dept = 0;
                Dictionary<int, int> nextNodes = new Dictionary<int, int>();
                nextNodes.Add(startNode, -1);

                Dictionary<int, int> curNodes = new Dictionary<int, int>();
                List<Dictionary<int, int>> allLayers = new List<Dictionary<int, int>>();
                while(nextNodes.Count > 0) {
                    dept++;
                    curNodes = nextNodes;
                    allLayers.Insert(0, nextNodes);
                    nextNodes = new Dictionary<int, int>();

                    foreach (KeyValuePair<int, int> leafs in curNodes) {
                        foreach (int item in tree[leafs.Key]) {
                            nextNodes[item] = leafs.Key;
                        }
                    }
                }

                if(bestDept < dept) {
                    int curPortal = allLayers[0].Keys.First();
                    List<int> linkList = new List<int>();
                    foreach (Dictionary<int, int> item in allLayers) {
                        linkList.Insert(0, curPortal);
                        curPortal = item[curPortal];
                    }
                    GameState curBest = gs.DeepClone();
                    foreach (int p in linkList) {
                        curBest.linkToAllAnchors(allPoints[p]);
                    }
                    this.newBestGame(curBest);
                    bestDept = dept;
                }

            }
            

            return this.Best;
        }
        struct AnchorAngle {
            public double A1;
            public double A2;

            public AnchorAngle(double dist1, double dist2) {
                this.A1 = dist1;
                this.A2 = dist2;
            }

            public bool BothHigher(AnchorAngle other) {
                if (other.A1 < this.A1) return false;
                if (other.A2 < this.A2) return false;
                return true;
            }

            public bool BothLower(AnchorAngle other) {
                if (other.A1 > this.A1) return false;
                if (other.A2 > this.A2) return false;
                return true;
            }
        }
    }
}
