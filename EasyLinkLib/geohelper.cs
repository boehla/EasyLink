using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasyLinkLib.geohelper;

namespace EasyLinkLib {
    public class geohelper {

        private static bool nearlyEqual(double x, double y) {
            return (Math.Abs(x - y) < 0.00001f);
        }
        public static int findSide(PortalInfo n1, PortalInfo n2, PortalInfo n3) {
            return findSide(n1.Pos.X, n1.Pos.Y, n2.Pos.X, n2.Pos.Y, n3.Pos.X, n3.Pos.Y);
        }
        public static int findSide(
        double ax, double ay,
        double bx, double by,
        double cx, double cy) {

            if (nearlyEqual(bx - ax, 0)) { // vertical line
                if (cx < bx) {
                    return by > ay ? 1 : -1;
                }
                if (cx > bx) {
                    return by > ay ? -1 : 1;
                }
                return 0;
            }
            if (nearlyEqual(by - ay, 0)) { // horizontal line
                if (cy < by) {
                    return bx > ax ? -1 : 1;
                }
                if (cy > by) {
                    return bx > ax ? 1 : -1;
                }
                return 0;
            }
            double slope = (by - ay) / (bx - ax);
            double yIntercept = ay - ax * slope;
            double cSolution = (slope * cx) + yIntercept;
            if (slope != 0) {
                if (cy > cSolution) {
                    return bx > ax ? 1 : -1;
                }
                if (cy < cSolution) {
                    return bx > ax ? -1 : 1;
                }
                return 0;
            }
            return 0;
        }


        public struct PointD {
            public double X;
            public double Y;
            public PointD(double X, double Y) {
                this.X = X;
                this.Y = Y;
            }

            public override string ToString() {
                return string.Format("{0} / {1}", this.Y, this.X);
            }
        }

        public static List<int> getPointsInArea(GameState gs, Field f) {
            List<int> ret = new List<int>();

            List<PointD> ply = new List<PointD>();
            foreach (int fid in f.NodesIds) {
                ply.Add(gs.PortalInfos[fid].Pos);
            }
            PointD[] plyArr = ply.ToArray();
            for(int i = 0; i < gs.PortalData.Count; i++) {
                if (f.NodesIds.Contains(i)) continue;
                if (gs.PortalData[i].InTriangle) continue; // Already in!
                if (PointInPolygon(plyArr, gs.PortalInfos[i])) ret.Add(i);
            }
            return ret;
        }
        public static bool PointInPolygon(PointD[] Points, PortalInfo ni) {
            return PointInPolygon(Points, ni.Pos.X, ni.Pos.Y);
        }
            public static bool PointInPolygon(PointD[] Points, double X, double Y) {
            // Get the angle between the point and the
            // first and last vertices.
            int max_point = Points.Length - 1;
            double total_angle = GetAngle(
                Points[max_point].X, Points[max_point].Y,
                X, Y,
                Points[0].X, Points[0].Y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < max_point; i++) {
                total_angle += GetAngle(
                    Points[i].X, Points[i].Y,
                    X, Y,
                    Points[i + 1].X, Points[i + 1].Y);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            return (Math.Abs(total_angle) > 0.000001);
        }
        // Return the cross product AB x BC.
        // The cross product is a vector perpendicular to AB
        // and BC having length |AB| * |BC| * Sin(theta) and
        // with direction given by the right-hand rule.
        // For two vectors in the X-Y plane, the result is a
        // vector with X and Y components 0 so the Z component
        // gives the vector's length and direction.
        public static double CrossProductLength(double Ax, double Ay,
            double Bx, double By, double Cx, double Cy) {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }
        // Return the angle ABC.
        // Return a value between PI and -PI.
        // Note that the value is the opposite of what you might
        // expect because Y coordinates increase downward.
        public static double GetAngle(double Ax, double Ay,
            double Bx, double By, double Cx, double Cy) {
            // Get the dot product.
            double dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            double cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.
            return Math.Atan2(cross_product, dot_product);
        }
        // Return the dot product AB · BC.
        // Note that AB · BC = |AB| * |BC| * Cos(theta).
        private static double DotProduct(double Ax, double Ay,
            double Bx, double By, double Cx, double Cy) {
            // Get the vectors' coordinates.
            double BAx = Ax - Bx;
            double BAy = Ay - By;
            double BCx = Cx - Bx;
            double BCy = Cy - By;

            // Calculate the dot product.
            return (BAx * BCx + BAy * BCy);
        }
        /*
        public static bool IsInPolygon(PointF[] poly, NodeInfo point) {
            var coef = poly.Skip(1).Select((p, i) =>
                                            (point.LatY - poly[i].Y) * (p.X - poly[i].X)
                                          - (point.LonX - poly[i].X) * (p.Y - poly[i].Y))
                                    .ToList();

            if (coef.Any(p => p == 0))
                return true;

            for (int i = 1; i < coef.Count(); i++) {
                if (coef[i] * coef[i - 1] < 0)
                    return false;
            }
            return true;
        } */
        /// <summary>
		/// Calculate the distance between two gps-positions in METERS.
		/// </summary>
		/// <param name="long1">the longitude (X) coordinate of point A in degrees</param>
		/// <param name="lat1">the latitude (Y) coordinate of point A in degrees</param>
		/// <param name="long2">the longitude (X) coordinate of point B in degrees</param>
		/// <param name="lat2">the latitude (Y) coordinate of point B in degrees</param>
		/// <returns>The distance in meters. -1 in case of invalid arguments.</returns>
        public static double CalcDistance(double long1, double lat1, double long2, double lat2) {
            double r = 6371000;
            double aa = Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) * Math.Cos(long1 * Math.PI / 180) * Math.Cos(long2 * Math.PI / 180);
            double bb = Math.Cos(lat1 * Math.PI / 180) * Math.Sin(long1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) * Math.Sin(long2 * Math.PI / 180);
            double cc = Math.Sin(lat1 * Math.PI / 180) * Math.Sin(lat2 * Math.PI / 180);

            if ((aa + bb + cc) > 1 || (aa + bb + cc) < -1) return 0;  //invalid for Math.Acos
            else return Math.Acos(aa + bb + cc) * r;
        }
        public static double CalcDistance(PortalInfo n1, PortalInfo n2) {
            return CalcDistance(n1.Pos.X, n1.Pos.Y, n2.Pos.X, n2.Pos.Y);
        }

        public static double calculateDistance(PortalInfo n1, PortalInfo n2) {
            return CalcDistance(n1.Pos.X, n1.Pos.Y, n2.Pos.X, n2.Pos.Y);


            //return (n1.LonX - n2.LonX) * (n1.LonX - n2.LonX) + (n1.LatY - n2.LatY) * (n1.LatY - n2.LatY);
        }
        public static double calculateAreaOld(GameState gs, Field f) {
            PortalInfo p1 = gs.PortalInfos[f.NodesIds[0]];
            PortalInfo p2 = gs.PortalInfos[f.NodesIds[1]];
            PortalInfo p3 = gs.PortalInfos[f.NodesIds[2]];

            double l1 = (float)Math.Sqrt((p1.Pos.X - p2.Pos.X) * (p1.Pos.X - p2.Pos.X) + (p1.Pos.Y - p2.Pos.Y) * (p1.Pos.Y - p2.Pos.Y));
            double l2 = (float)Math.Sqrt((p3.Pos.X - p2.Pos.X) * (p3.Pos.X - p2.Pos.X) + (p3.Pos.Y - p2.Pos.Y) * (p3.Pos.Y - p2.Pos.Y));
            double l3 = (float)Math.Sqrt((p1.Pos.X - p3.Pos.X) * (p1.Pos.X - p3.Pos.X) + (p1.Pos.Y - p3.Pos.Y) * (p1.Pos.Y - p3.Pos.Y));
            
            return (l1 + l2 + l3) * (-l1 + l2 + l3) * (l1 - l2 + l3) * (l1 + l2 - l3);
        }
        public static double calculateArea(GameState gs, Field f) {
            PortalInfo p1 = gs.PortalInfos[f.NodesIds[0]];
            PortalInfo p2 = gs.PortalInfos[f.NodesIds[1]];
            PortalInfo p3 = gs.PortalInfos[f.NodesIds[2]];



            double a = CalcDistance(p1, p2);
            double b = CalcDistance(p2, p3);
            double c = CalcDistance(p3, p1);
            double s = (a + b + c) / 2;


            return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
        }
        public static bool crossLink(GameState gs, PointD p1, PointD p2) {
            for (int i = 0; i < gs.PortalInfos.Count; i++) {
                foreach (KeyValuePair<int, bool> item in gs.PortalData[i].SideLinks) {
                    //if (!item.Value) continue;
                    //Point p = LineIntersection.FindIntersection(new Line(gs.PortalInfos[p1id], gs.PortalInfos[p2id]), new Line(gs.PortalInfos[i], gs.PortalInfos[item.Key]), 0.00000000000);
                    bool inters = FindIntersection(new Vector(p1), new Vector(p2), new Vector(gs.PortalInfos[i]), new Vector(gs.PortalInfos[item.Key]));
                    //if (!p.Equals(default(Point))) return true;
                    if (inters) return true;
                }

            }
            return false;
        }
        public static bool crossLink(GameState gs, int p1id, int p2id) {
            for (int i = 0; i < gs.PortalInfos.Count; i++) {
                if (i == p1id || i == p2id) continue;
                foreach (KeyValuePair<int, bool> item in gs.PortalData[i].SideLinks) {
                    //if (!item.Value) continue;
                    //Point p = LineIntersection.FindIntersection(new Line(gs.PortalInfos[p1id], gs.PortalInfos[p2id]), new Line(gs.PortalInfos[i], gs.PortalInfos[item.Key]), 0.00000000000);
                    bool inters = FindIntersection(gs.PortalInfos[p1id], gs.PortalInfos[p2id], gs.PortalInfos[i], gs.PortalInfos[item.Key]);
                    //if (!p.Equals(default(Point))) return true;
                    if (inters) return true;
                }
                
            }
            return false;
        }
        public static bool FindIntersection2(PortalInfo s1, PortalInfo e1, PortalInfo s2, PortalInfo e2) {
            double a1 = e1.Pos.Y - s1.Pos.Y;
            double b1 = s1.Pos.X - e1.Pos.X;
            double c1 = a1 * s1.Pos.X + b1 * s1.Pos.Y;

            double a2 = e2.Pos.Y - s2.Pos.Y;
            double b2 = s2.Pos.X - e2.Pos.X;
            double c2 = a2 * s2.Pos.X + b2 * s2.Pos.Y;

            double delta = a1 * b2 - a2 * b1;
            //If lines are parallel, the result will be (NaN, NaN).
            if (delta == 0) return false;
            PointD sp = new PointD((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);


            return delta != 0;
        }
        public static bool FindIntersection(PortalInfo s1, PortalInfo e1, PortalInfo s2, PortalInfo e2) {
            return FindIntersection(new Vector(s1), new Vector(e1), new Vector(s2), new Vector(e2));
        }
        public static bool FindIntersection(Vector p, Vector p2, Vector q, Vector q2) {
            Vector intersection = new Vector();

            bool considerCollinearOverlapAsIntersect = true;

            var r = p2 - p;
            var s = q2 - q;
            var rxs = r.Cross(s);
            var qpxr = (q - p).Cross(r);

            // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
            if (rxs.IsZero() && qpxr.IsZero()) {
                // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
                // then the two lines are overlapping,
                if (considerCollinearOverlapAsIntersect)
                    if ((0 <= (q - p) * r && (q - p) * r <= r * r) || (0 <= (p - q) * s && (p - q) * s <= s * s))
                        return true;

                // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
                // then the two lines are collinear but disjoint.
                // No need to implement this expression, as it follows from the expression above.
                return false;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (rxs.IsZero() && !qpxr.IsZero())
                return false;

            // t = (q - p) x s / (r x s)
            var t = (q - p).Cross(s) / rxs;

            // u = (q - p) x r / (r x s)

            var u = (q - p).Cross(r) / rxs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
            // the two line segments meet at the point p + t r = q + u s.
            if (!rxs.IsZero() && (0 <= t && t <= 1) && (0 <= u && u <= 1)) {
                // We can calculate the intersection point using either t or u.
                intersection = p + t * r;

                if (intersection.Equals(p)) return false;
                if (intersection.Equals(p2)) return false;
                if (intersection.Equals(q)) return false;
                if (intersection.Equals(q2)) return false;

                // An intersection was found.
                return true;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return false;
        }

    }

    public class Vector {
        public double X;
        public double Y;

        public Vector(PortalInfo ni) {
            this.X = ni.Pos.X;
            this.Y = ni.Pos.Y;
        }
        public Vector(PointD d) {
            this.X = d.X;
            this.Y = d.Y;
        }
        // Constructors.
        public Vector(double x, double y) { X = x; Y = y; }
        public Vector() : this(double.NaN, double.NaN) { }

        public static Vector operator -(Vector v, Vector w) {
            return new Vector(v.X - w.X, v.Y - w.Y);
        }

        public static Vector operator +(Vector v, Vector w) {
            return new Vector(v.X + w.X, v.Y + w.Y);
        }

        public static double operator *(Vector v, Vector w) {
            return v.X * w.X + v.Y * w.Y;
        }

        public static Vector operator *(Vector v, double mult) {
            return new Vector(v.X * mult, v.Y * mult);
        }

        public static Vector operator *(double mult, Vector v) {
            return new Vector(v.X * mult, v.Y * mult);
        }

        public double Cross(Vector v) {
            return X * v.Y - Y * v.X;
        }

        public override bool Equals(object obj) {
            var v = (Vector)obj;
            return (X - v.X).IsZero() && (Y - v.Y).IsZero();
        }

        public override int GetHashCode() {
            return X.GetHashCode() + Y.GetHashCode() * 31;
        }
    }
    public static class Extensions {
        private const double Epsilon = 1e-10;

        public static bool IsZero(this double d) {
            return Math.Abs(d) < Epsilon;
        }
    }
    public class Border {
        public double LatYMax = float.MinValue;
        public double LatYMin = float.MaxValue;
        public double LonXMax = float.MinValue;
        public double LonXMin = float.MaxValue;

        public double LonXDiff {
            get { return LonXMax - LonXMin; }
        }
        public double LatYDiff {
            get { return LatYMax - LatYMin; }
        }
        public void addValue(PointD p) {
            this.addValue(p.X, p.Y);
        }
        public void addValue(double lonX, double latY) {
            LonXMax = Math.Max(lonX, LonXMax);
            LonXMin = Math.Min(lonX, LonXMin);

            LatYMax = Math.Max(latY, LatYMax);
            LatYMin = Math.Min(latY, LatYMin);
        }
        public bool isOutside(PointD p) {
            return this.isOutside(p.X, p.Y);
        }
        public bool isOutside(double lonX, double latY) {
            if (lonX > this.LonXMax) return true;
            if (lonX < this.LonXMax) return true;
            if (latY > this.LatYMax) return true;
            if (latY < this.LatYMax) return true;
            return false;
        }
    }




    public struct Line {
        public double x1 { get; set; }
        public double y1 { get; set; }

        public double x2 { get; set; }
        public double y2 { get; set; }

        public Line(PortalInfo p1, PortalInfo p2) {
            this.x1 = p1.Pos.X;
            this.y1 = p1.Pos.Y;
            this.x2 = p2.Pos.X;
            this.y2 = p2.Pos.Y;
        }
        public Line(double x1, double y1, double x2, double y2) {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
    }

    public class LineIntersection {
        //  Returns Point of intersection if do intersect otherwise default Point (null)
        public static PointD FindIntersection2(Line lineA, Line lineB, double tolerance = 0.001) {
            double x1 = lineA.x1, y1 = lineA.y1;
            double x2 = lineA.x2, y2 = lineA.y2;

            double x3 = lineB.x1, y3 = lineB.y1;
            double x4 = lineB.x2, y4 = lineB.y2;

            // equations of the form x = c (two vertical lines)
            if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance && Math.Abs(x1 - x3) < tolerance) {
                throw new Exception("Both lines overlap vertically, ambiguous intersection points.");
            }

            //equations of the form y=c (two horizontal lines)
            if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance && Math.Abs(y1 - y3) < tolerance) {
                throw new Exception("Both lines overlap horizontally, ambiguous intersection points.");
            }

            //equations of the form x=c (two vertical lines)
            if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance) {
                return default(PointD);
            }

            //equations of the form y=c (two horizontal lines)
            if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance) {
                return default(PointD);
            }

            //general equation of line is y = mx + c where m is the slope
            //assume equation of line 1 as y1 = m1x1 + c1 
            //=> -m1x1 + y1 = c1 ----(1)
            //assume equation of line 2 as y2 = m2x2 + c2
            //=> -m2x2 + y2 = c2 -----(2)
            //if line 1 and 2 intersect then x1=x2=x & y1=y2=y where (x,y) is the intersection point
            //so we will get below two equations 
            //-m1x + y = c1 --------(3)
            //-m2x + y = c2 --------(4)

            double x, y;

            //lineA is vertical x1 = x2
            //slope will be infinity
            //so lets derive another solution
            if (Math.Abs(x1 - x2) < tolerance) {
                //compute slope of line 2 (m2) and c2
                double m2 = (y4 - y3) / (x4 - x3);
                double c2 = -m2 * x3 + y3;

                //equation of vertical line is x = c
                //if line 1 and 2 intersect then x1=c1=x
                //subsitute x=x1 in (4) => -m2x1 + y = c2
                // => y = c2 + m2x1 
                x = x1;
                y = c2 + m2 * x1;
            }
            //lineB is vertical x3 = x4
            //slope will be infinity
            //so lets derive another solution
            else if (Math.Abs(x3 - x4) < tolerance) {
                //compute slope of line 1 (m1) and c2
                double m1 = (y2 - y1) / (x2 - x1);
                double c1 = -m1 * x1 + y1;

                //equation of vertical line is x = c
                //if line 1 and 2 intersect then x3=c3=x
                //subsitute x=x3 in (3) => -m1x3 + y = c1
                // => y = c1 + m1x3 
                x = x3;
                y = c1 + m1 * x3;
            }
            //lineA & lineB are not vertical 
            //(could be horizontal we can handle it with slope = 0)
            else {
                //compute slope of line 1 (m1) and c2
                double m1 = (y2 - y1) / (x2 - x1);
                double c1 = -m1 * x1 + y1;

                //compute slope of line 2 (m2) and c2
                double m2 = (y4 - y3) / (x4 - x3);
                double c2 = -m2 * x3 + y3;

                //solving equations (3) & (4) => x = (c1-c2)/(m2-m1)
                //plugging x value in equation (4) => y = c2 + m2 * x
                x = (c1 - c2) / (m2 - m1);
                y = c2 + m2 * x;

                float zero = 0.0000000001f;
                //verify by plugging intersection point (x, y)
                //in orginal equations (1) & (2) to see if they intersect
                //otherwise x,y values will not be finite and will fail this check
                if (!(Math.Abs(-m1 * x + y - c1) <= zero
                    && Math.Abs(-m2 * x + y - c2) <= zero)) {
                    return default(PointD);
                }
            }

            //x,y can intersect outside the line segment since line is infinitely long
            //so finally check if x, y is within both the line segments
            if (IsInsideLine(lineA, x, y) &&
                IsInsideLine(lineB, x, y)) {
                return new PointD { X = x, Y = y };
            }

            //return default null (no intersection)
            return default(PointD);

        }

        // Returns true if given point(x,y) is inside the given line segment
        private static bool IsInsideLine(Line line, double x, double y) {
            return (x >= line.x1 && x <= line.x2
                        || x >= line.x2 && x <= line.x1)
                   && (y >= line.y1 && y <= line.y2
                        || y >= line.y2 && y <= line.y1);
        }
    }


    
}
