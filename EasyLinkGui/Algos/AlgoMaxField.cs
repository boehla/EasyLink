using EasyLinkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasyLinkLib.geohelper;

namespace EasyLinkGui.Algos {
    class AlgoMaxField : AlgoDummy {
        internal override GameState getBestGame(GameState gs) {

            Random r = new Random(0);
            List<PointD> allPoints = new List<PointD>();
            Dictionary<PointD, int> index = new Dictionary<PointD, int>();

            for(int i = 0; i < gs.PortalInfos.Count; i++) {
                PortalInfo pInfo = gs.PortalInfos[i];
                    allPoints.Add(pInfo.Pos);
                index[pInfo.Pos] = i;
                
            }

            List<PointD> hull = ConvexHull.MakeConvexHull(allPoints);

            for (int i = 0; i < hull.Count; i++) {
                gs.addLink(index[hull[(i - 1 + hull.Count) % hull.Count]], index[hull[i]]);
            }
            while(hull.Count > 3) {
                int mid = r.Next(0, hull.Count);
                int ind1 = (mid - 1 + hull.Count) % hull.Count;
                int ind2 = (mid + 1 + hull.Count) % hull.Count;
                gs.addLink(index[hull[ind1]], index[hull[ind2]]);
                hull.RemoveAt(mid);
            }



            return gs;
        }
    }
}
