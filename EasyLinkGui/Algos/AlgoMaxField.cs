using EasyLinkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasyLinkLib.geohelper;

namespace EasyLinkGui.Algos {
    class AlgoMaxField : AlgoDummy {
        private CustSettings custSettings = new CustSettings();

        internal override void init() {
            base.init();
            this.Settings = custSettings;
        }
        Dictionary<PointD, int> index = new Dictionary<PointD, int>();
        internal override GameState getBestGame(GameState gs) {

            Random r = new Random(0);
            List<PointD> allPoints = new List<PointD>();

            index = new Dictionary<PointD, int>();
            for (int i = 0; i < gs.PortalInfos.Count; i++) {
                PortalInfo pInfo = gs.PortalInfos[i];
                allPoints.Add(pInfo.Pos);
                index[pInfo.Pos] = i;

            }
            List<PointD> remPoints = new List<PointD>(allPoints);

            List<PointD> hull = ConvexHull.MakeConvexHull(allPoints);
            foreach (PointD item in hull) {
                remPoints.Remove(item);
            }

            LinkPlan linkPlan = new LinkPlan(gs.PortalInfos.Count);

            for (int i = 0; i < hull.Count; i++) {
                //gs.addLink(index[hull[(i - 1 + hull.Count) % hull.Count]], index[hull[i]]);
                linkPlan.addLink(index[hull[(i - 1 + hull.Count) % hull.Count]], index[hull[i]]);
            }
            while (hull.Count > 3) {
                int mid = r.Next(0, hull.Count);
                int ind1 = (mid - 1 + hull.Count) % hull.Count;
                int ind2 = (mid + 1 + hull.Count) % hull.Count;
                //gs.addLink(index[hull[ind1]], index[hull[ind2]]);
                linkPlan.addLink(index[hull[ind1]], index[hull[ind2]]);
                hull.RemoveAt(mid);
            }
            List<Triangle> remTriangles = new List<Triangle>();
            for (int p1 = 0; p1 < linkPlan.Portals.Length; p1++) {
                foreach (int p2 in linkPlan.Portals[p1].Links) {
                    foreach (int p3 in linkPlan.Portals[p2].Links) {
                        if (linkPlan.Portals[p3].Links.Contains(p1)) {
                            int[] triangleKey = new int[] { p1, p2, p3 };
                            Array.Sort(triangleKey);
                            Triangle newTr = new Triangle(triangleKey);
                            if (remTriangles.Contains(newTr)) continue;
                            remTriangles.Add(newTr);
                        }
                    }
                }
            }

            while (remTriangles.Count > 0) {
                Triangle curTriangle = remTriangles[0];
                remTriangles.RemoveAt(0);

                foreach (PointD remPoint in remPoints) {
                    if (geohelper.PointInPolygon(new PointD[] { allPoints[curTriangle.P1], allPoints[curTriangle.P2], allPoints[curTriangle.P3] }, remPoint)) {
                        linkPlan.addLink(curTriangle.P1, index[remPoint]);
                        linkPlan.addLink(curTriangle.P2, index[remPoint]);
                        linkPlan.addLink(curTriangle.P3, index[remPoint]);

                        for (int i = 0; i < 3; i++) {
                            int[] triangleKey = new int[] { curTriangle.P1, curTriangle.P2, curTriangle.P3 };
                            triangleKey[i] = index[remPoint];
                            Array.Sort(triangleKey);
                            Triangle newTr = new Triangle(triangleKey);
                            remTriangles.Add(newTr);
                        }

                        remPoints.Remove(remPoint);
                        break;
                    }
                }

            }



            AlgoWayOptimizer wopt = new AlgoWayOptimizer(gs, linkPlan);

            gs = wopt.linkConvex();


            this.newBestGame(gs);

            return gs;
        }
    }
    public struct Triangle {
        public int P1 { get; set; }
        public int P2 { get; set; }
        public int P3 { get; set; }

        public Triangle(int[] pp) {
            this.P1 = pp[0];
            this.P2 = pp[1];
            this.P3 = pp[2];
        }
    }

    class CustSettings {
        public int maxLinkCount { get; set; } = 10000;
    }
}
