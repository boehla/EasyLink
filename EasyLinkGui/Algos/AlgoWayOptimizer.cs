using EasyLinkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasyLinkLib.geohelper;

namespace EasyLinkGui.Algos {
    class AlgoWayOptimizer {
        GameState gs = null;
        LinkPlan lp = null;

        Dictionary<PointD, int> index = new Dictionary<PointD, int>();


        public AlgoWayOptimizer(GameState gs) {
            LinkPlan nwLink = new LinkPlan(gs.PortalInfos.Count);
            for(int i = 0; i < gs.PortalData.Count; i++) { 
                foreach (KeyValuePair<int, bool> link in gs.PortalData[i].SideLinks) {
                    if (link.Value) {
                        nwLink.addLink(i, link.Key);
                    }
                }
            }
            init(gs, nwLink);
        }

        public AlgoWayOptimizer(GameState gs, LinkPlan linkPlan) {
            init(gs, linkPlan);
            
        }

        private void init(GameState gs, LinkPlan linkPlan) {
            this.gs = gs;

            index = new Dictionary<PointD, int>();
            for (int i = 0; i < gs.PortalInfos.Count; i++) {
                PortalInfo pInfo = gs.PortalInfos[i];
                index[pInfo.Pos] = i;

            }
            this.lp = linkPlan;



            List<int> remPortals = getConvexLinkOrder();
        }

        public GameState linkConvex() {
            return link(getConvexLinkOrder(), lp);
        }

        private GameState link(List<int> remPortals, LinkPlan linkPlan) {
            GameState ret = gs.DeepClone();
            ret.loadPortals(gs.PortalInfos);

            List<int> capturedPortals = new List<int>();
            LinkPlan alreadyLinked = new LinkPlan(linkPlan.Portals.Length);

            while (remPortals.Count > 0) {
                int lowest = remPortals[0];
                remPortals.RemoveAt(0);

                SortedList<double, Triangle> triangleSize = new SortedList<double, Triangle>();
                foreach (int cap in capturedPortals) {
                    if (linkPlan.Portals[cap].Links.Contains(lowest)) {
                        alreadyLinked.addLink(lowest, cap);
                    }
                }
                foreach (int p2 in alreadyLinked.Portals[lowest].Links) {
                    foreach (int p3 in alreadyLinked.Portals[p2].Links) {
                        if (alreadyLinked.Portals[p3].Links.Contains(lowest)) {
                            int[] triangleKey = new int[] { lowest, p2, p3 };
                            Array.Sort(triangleKey);
                            Triangle newTr = new Triangle(triangleKey);
                            double size = geohelper.calculateArea(ret, new Field(newTr.P1, newTr.P2, newTr.P3));
                            size = 1 / size;
                            if (triangleSize.ContainsKey(size)) continue;
                            triangleSize.Add(size, newTr);
                        }
                    }
                }
                foreach (Triangle item in triangleSize.Values) {
                    if (ret.addLink(lowest, item.P1)) {
                        linkPlan.removeLink(lowest, item.P1);
                    }
                    if (ret.addLink(lowest, item.P2)) {
                        linkPlan.removeLink(lowest, item.P2);
                    }
                    if (ret.addLink(lowest, item.P3)) {
                        linkPlan.removeLink(lowest, item.P3);
                    }
                }
                foreach (int cap in capturedPortals) {
                    if (linkPlan.Portals[cap].Links.Contains(lowest)) {
                        if (ret.addLink(lowest, cap)) {
                            linkPlan.removeLink(lowest, cap);
                        } else {

                        }

                    }
                }
                remPortals.Remove(lowest);
                capturedPortals.Add(lowest);
            }

            return ret;
        }

        private List<int> getConvexLinkOrder2() {
            List<int> ret = new List<int>();
            List<int> remPortals = new List<int>();
            for (int i = 0; i < gs.PortalInfos.Count; i++) {
                remPortals.Add(i);
            }

            while (remPortals.Count > 0) {
                int lowest = -1;
                foreach (int item in remPortals) {
                    if (lowest < 0 || gs.PortalInfos[lowest].Pos.Y > gs.PortalInfos[item].Pos.Y) {
                        lowest = item;
                    }
                }
                ret.Add(lowest);
                remPortals.Remove(lowest);
            }
            return ret;
        }

        private List<int> getConvexLinkOrder() {
            List<int> ret = new List<int>();

            List<PointD> remPortals = new List<PointD>();
            for (int i = 0; i < gs.PortalInfos.Count; i++) {
                remPortals.Add(gs.PortalInfos[i].Pos);
            }


            List<PointD> mainHull = ConvexHull.MakeConvexHull(remPortals);

            foreach (PortalInfo item in gs.Global.AnchorsPortals) {
                if (mainHull.Contains(item.Pos)) {
                    remPortals.Remove(item.Pos);
                }
                
            }



            double bestWay = double.MaxValue;

            foreach (PointD startD in mainHull) {
                PointD curPoint = startD;
                List<int> curWay = new List<int>();
                double dist = 0;

                remPortals = new List<PointD>();
                for (int i = 0; i < gs.PortalInfos.Count; i++) {
                    remPortals.Add(gs.PortalInfos[i].Pos);
                }


                while (remPortals.Count > 0) {
                    PointD nextPoint = remPortals[0];
                    double lowestdiff = double.MaxValue;
                    if (remPortals.Count >= 3) {
                        List<PointD> hull = ConvexHull.MakeConvexHull(remPortals);

                        foreach (PointD pointD in hull) {
                            double curDiff = geohelper.CalcDistance(pointD.X, pointD.Y, curPoint.X, curPoint.Y);
                            if (curDiff < lowestdiff) {
                                nextPoint = pointD;
                                lowestdiff = curDiff;
                            }
                        }

                    } else {
                        foreach (PointD pointD in remPortals) {
                            double curDiff = geohelper.CalcDistance(pointD.X, pointD.Y, curPoint.X, curPoint.Y);
                            if (curDiff < lowestdiff) {
                                nextPoint = pointD;
                                lowestdiff = curDiff;
                            }
                        }

                    }
                    dist += lowestdiff;
                    curWay.Add(index[nextPoint]);
                    remPortals.Remove(nextPoint);
                    curPoint = nextPoint;
                }
                curWay.Reverse();

                if (dist < bestWay) {
                    bestWay = dist;
                    ret = curWay;
                }
            }


            return ret;
        }
    }

    public class LinkPlan {
        public Portal[] Portals;
        public int CountLinks = 0;

        public LinkPlan(int portalCount) {
            this.Portals = new Portal[portalCount];
            for (int i = 0; i < portalCount; i++) {
                this.Portals[i] = new Portal();
            }
        }

        public void addLink(int p1, int p2) {
            CountLinks++;
            Portals[p1].Links.Add(p2);
            Portals[p2].Links.Add(p1);
        }

        public void removeLink(int p1, int p2) {
            CountLinks--;
            Portals[p1].Links.Remove(p2);
            Portals[p2].Links.Remove(p1);
        }


        public class Portal {
            public HashSet<int> Links { get; set; } = new HashSet<int>();
        }


    }
}
