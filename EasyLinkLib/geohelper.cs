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
        // Author: Michael Myers
        // https://stackoverflow.com/questions/1560492/how-to-tell-whether-a-point-is-to-the-right-or-left-side-of-a-line
        //
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
                return string.Format("{0},{1}", Lib.Converter.toString(this.Y), Lib.Converter.toString(this.X));
            }
            public bool Valid {
                get { return Math.Abs(X) + Math.Abs(Y) > 0.00001; }
            }
        }
        public struct RectangleD {
            public double Top;
            public double Bottom;
            public double Left;
            public double Right;
            public RectangleD(double Left, double Right, double Bottom, double Top) {
                this.Left = Left;
                this.Right = Right;
                this.Bottom = Bottom;
                this.Top = Top;
            }

            public override string ToString() {
                return string.Format("{0} / {1} / {2} / {3}", this.Left, this.Right, this.Bottom, this.Top);
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
        public static bool PointInPolygon(PointD[] Points, PointD p) {
            return PointInPolygon(Points, p.X, p.Y);
        }
        public static bool PointInPolygon(PointD[] Points, PortalInfo ni) {
            return PointInPolygon(Points, ni.Pos.X, ni.Pos.Y);
        }
        public static double GetAngle(PointD p1, PointD p2) {
            double xDiff = p2.X - p1.X;
            double yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
        }
        // Author: http://csharphelper.com/blog/2014/07/determine-whether-a-point-is-inside-a-polygon-in-c/
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
        // Author: http://csharphelper.com/blog/2014/07/determine-whether-a-point-is-inside-a-polygon-in-c/
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
        // Author: http://csharphelper.com/blog/2014/07/determine-whether-a-point-is-inside-a-polygon-in-c/
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
        // Author: http://csharphelper.com/blog/2014/07/determine-whether-a-point-is-inside-a-polygon-in-c/
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
                    bool inters = Intersect(new Vector(p1), new Vector(p2), new Vector(gs.PortalInfos[i]), new Vector(gs.PortalInfos[item.Key]));
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
        public static bool FindIntersection(PortalInfo s1, PortalInfo e1, PortalInfo s2, PortalInfo e2) {
            Border b1 = new Border();
            b1.addValue(s1.Pos);
            b1.addValue(e1.Pos);

            Border b2 = new Border();
            b2.addValue(s2.Pos);
            b2.addValue(e2.Pos);

            //if (!b1.overlaps(b2)) return false; // not even overlaps -> no collision possible!

            return Intersect(new Vector(s1), new Vector(e1), new Vector(s2), new Vector(e2));
        }
        public static bool Intersect(PointD p, PointD p2, PointD q, PointD q2) {
            return FindIntersection(new Vector(p), new Vector(p2), new Vector(q), new Vector(q2)) != null;
        }
        public static bool Intersect(Vector p, Vector p2, Vector q, Vector q2) {
            return FindIntersection(p, p2, q, q2) != null;
        }
        public static Vector FindIntersection(PointD p, PointD p2, PointD q, PointD q2) {
            return FindIntersection(new Vector(p), new Vector(p2), new Vector(q), new Vector(q2));
        }
            // Author: https://www.codeproject.com/Tips/862988/Find-the-Intersection-Point-of-Two-Line-Segments
        public static Vector FindIntersection(Vector p, Vector p2, Vector q, Vector q2) {
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
                        return p;

                // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
                // then the two lines are collinear but disjoint.
                // No need to implement this expression, as it follows from the expression above.
                return null;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (rxs.IsZero() && !qpxr.IsZero())
                return null;

            // t = (q - p) x s / (r x s)
            var t = (q - p).Cross(s) / rxs;

            // u = (q - p) x r / (r x s)

            var u = (q - p).Cross(r) / rxs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
            // the two line segments meet at the point p + t r = q + u s.
            if (!rxs.IsZero() && (0 <= t && t <= 1) && (0 <= u && u <= 1)) {
                // We can calculate the intersection point using either t or u.
                intersection = p + t * r;

                if (intersection.Equals(p)) return null;
                if (intersection.Equals(p2)) return null;
                if (intersection.Equals(q)) return null;
                if (intersection.Equals(q2)) return null;

                // An intersection was found.
                return intersection;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
            return null;
        }

    }

    // https://www.codeproject.com/Tips/862988/Find-the-Intersection-Point-of-Two-Line-Segments
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
        public PointD toPointD() {
            return new PointD(X, Y);
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
            if (lonX < this.LonXMin) return true;
            if (latY > this.LatYMax) return true;
            if (latY < this.LatYMin) return true;
            return false;
        }

        public bool overlaps(Border b) {
            if (!this.isOutside(new PointD(b.LonXMin, b.LatYMin))) return true;
            if (!this.isOutside(new PointD(b.LonXMax, b.LatYMin))) return true;
            if (!this.isOutside(new PointD(b.LonXMin, b.LatYMax))) return true;
            if (!this.isOutside(new PointD(b.LonXMax, b.LatYMax))) return true;
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
}
