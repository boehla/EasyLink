using EasyLinkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasyLinkLib.geohelper;

namespace EasyLinkGui.Algos {
    class AlgoConvex : AlgoDummy {

        internal override GameState getBestGame(GameState gs) {
            List<PointD> allPoints = new List<PointD>();

            foreach (PortalInfo pInfo in gs.PortalInfos) {
                allPoints.Add(pInfo.Pos);
            }

            List<PointD> tmphull = ConvexHull.MakeConvexHull(allPoints);

            List<Triangle> remainTriangles = new List<Triangle>();
            Triangle tmptr = new Triangle();
            tmptr.subHull = tmphull;
            remainTriangles.Add(tmptr);
            /*

            while (remainTriangles.Count > 0) {
                Triangle tr = remainTriangles[0];
                remainTriangles.RemoveAt(0);

                
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
                }
            }
            */
            foreach (PortalInfo item in gs.Global.pInfos) {
                gs.addLink(item.Guid, gs.Global.AnchorsPortals[0].Guid);
            }

            linkFromTriangle(0, tmptr, gs);

            return gs;
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
            if (tr.subHull.Count > 0) {

                for (int i = 0; i < tr.subHull.Count; i++) {

                    int hull = game.Global.PortalMappingPointD[tr.subHull[i]];
                    if (tr.p1.Valid) {
                        int p1 = game.Global.PortalMappingPointD[tr.p1];
                        game.addLink(hull, p1);
                    }

                    if (i > 0) {
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
    }
}
