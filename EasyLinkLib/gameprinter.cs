using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyLinkLib {
    public class GamePrinter {
        private GameState gs;
        float borderPerc = 0.1f; 
        private Border extBorder = null;

        public PointF MousePosition { get; set; }

        public GameState GameState {
            get { return gs; }
            set { gs = value; }
        }

        public GamePrinter(GameState gs) {
            this.gs = gs;
        }
        

        float PortalWidth = 15;

        public Bitmap printGameState(int height, int width) {
            Bitmap ret = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(ret);

            extBorder = new Border();

            for (int i = 0; i < gs.PortalInfos.Count; i++) {
                PortalInfo p = gs.PortalInfos[i];
                extBorder.addValue(p.Pos);
            }
            // Extend border
            extBorder.addValue(extBorder.LonXMin - extBorder.LonXDiff * borderPerc, extBorder.LatYMin - extBorder.LatYDiff * borderPerc);
            extBorder.addValue(extBorder.LonXMax + extBorder.LonXDiff * borderPerc, extBorder.LatYMax + extBorder.LatYDiff * borderPerc);

            foreach (Field f in gs.Fields) {
                List<System.Drawing.Point> points = new List<System.Drawing.Point>();
                foreach (int pid in f.NodesIds) {
                    float x1 = getXPer(gs.PortalInfos[pid]) * width;
                    float y1 = getYPer(gs.PortalInfos[pid]) * height;
                    points.Add(new System.Drawing.Point((int)x1, (int)y1));
                }
                g.FillPolygon(new SolidBrush(Color.FromArgb(30, Color.Blue)), points.ToArray());
            }

            Pen linkPen = new Pen(new SolidBrush(Color.Blue), 3);
            for (int i = 0; i < gs.PortalInfos.Count; i++) {
                Portal p = gs.PortalData[i];
                foreach (KeyValuePair<int, bool> target in p.SideLinks) {
                    if (!target.Value) continue; // insideLink, draw only outside.

                    float x1 = getXPer(gs.PortalInfos[i]) * width;
                    float y1 = getYPer(gs.PortalInfos[i]) * height;
                    float x2 = getXPer(gs.PortalInfos[target.Key]) * width;
                    float y2 = getYPer(gs.PortalInfos[target.Key]) * height;

                    float xm = (x1 + x2 * 9) / 10;
                    float ym = (y1 + y2 * 9) / 10;
                    linkPen.Color = Color.Blue;
                    g.DrawLine(linkPen, x1, y1, xm, ym);
                    linkPen.Color = Color.DarkBlue;
                    g.DrawLine(linkPen, xm, ym, x2, y2);
                }
            }


            for (int i = 0; i < gs.PortalData.Count; i++) {
                PortalInfo ni = gs.PortalInfos[i];
                Portal n = gs.PortalData[i];
                float x1 = getXPer(ni) * width - (PortalWidth / 2);
                float y1 = getYPer(ni) * height - (PortalWidth / 2);
                //g.FillEllipse(new SolidBrush(n.InTriangle ? Color.LightGray : Color.Black), x1, y1, PortalWidth, PortalWidth);
                g.DrawLine(new Pen(n.InTriangle ? Color.LightGray : Color.Black, 3), x1 - (PortalWidth / 2), y1, x1 + (PortalWidth / 2), y1);
                g.DrawLine(new Pen(n.InTriangle ? Color.LightGray : Color.Black, 3), x1, y1 - (PortalWidth / 2), x1, y1 + (PortalWidth / 2));

                if (gs.PortalInfos[i].ShowLabel) g.DrawString(ni.Name + " / " + ni.Pos.X + " / " + ni.Pos.Y, new Font("Tahoma", 12), Brushes.Black, x1, y1 + PortalWidth);
            }

            int fromPortal = -1;
            foreach (Link link in gs.getTotalLinkList()) {
                if(fromPortal != -1) {
                    int newPortal = link.P1;

                    float x1 = getXPer(gs.PortalInfos[fromPortal]) * width;
                    float y1 = getYPer(gs.PortalInfos[fromPortal]) * height;
                    float x2 = getXPer(gs.PortalInfos[newPortal]) * width;
                    float y2 = getYPer(gs.PortalInfos[newPortal]) * height;

                    g.DrawLine(new Pen(new SolidBrush(Color.Yellow), 1), x1, y1, x2, y2);
                }
                fromPortal = link.P1;
            }

            /*
            if(MousePosition != null) {
                float x1 = getXPer(MousePosition) * width - (PortalWidth / 2);
                float y1 = getYPer(ni) * height - (PortalWidth / 2);
                g.FillEllipse(Color.Green), x1, y1, PortalWidth, PortalWidth);
            }*/
            
            g.Dispose();

            return ret;
        }

        private float getYPer(PortalInfo ni) {
            return (float)(1 - ((ni.Pos.Y - extBorder.LatYMin) / extBorder.LatYDiff));
        }
        private float getXPer(PortalInfo ni) {
            return (float)((ni.Pos.X - extBorder.LonXMin) / extBorder.LonXDiff);
        }

        public PointF parseFromImage(PointF mv, float height, float width) {
            if (extBorder == null) return default(PointF);
            float retX = mv.X / width;
            float retY = mv.Y / height;

            retX = (float)(((retX) * extBorder.LonXDiff) + extBorder.LonXMin);
            retY = (float)(((1 - (retY)) * extBorder.LatYDiff) + extBorder.LatYMin);

            return new PointF(retX, retY);
        }
    }
}
