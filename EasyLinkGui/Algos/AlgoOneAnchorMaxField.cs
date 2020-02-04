using EasyLinkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasyLinkLib.geohelper;

namespace EasyLinkGui.Algos {
    class OneAnchorMaxField : AlgoDummy {
        internal override GameState getBestGame(GameState gs) {
            if (gs.Global.AnchorsPortals.Count != 1) return gs;
            List<PointD> allPoints = new List<PointD>();
            Dictionary<PointD, bool> anchors = new Dictionary<PointD, bool>();
            if (gs.Global != null) {
                foreach (PortalInfo item in gs.Global.AnchorsPortals) {
                    anchors[item.Pos] = true;
                }
            }
            foreach (PortalInfo pInfo in gs.PortalInfos) {
                if (!anchors.ContainsKey(pInfo.Pos)) {
                    allPoints.Add(pInfo.Pos);
                }
            }

            SortedList<double, PointD> anglePoints = new SortedList<double, PointD>();
            PointD anchor = anchors.Keys.ToArray()[0];

            foreach (PointD p in allPoints) {
                if (p.Equals(anchor)) continue;
                //gs.addLink(gs.Global.PortalMappingPointD[p], gs.Global.PortalMappingPointD[anchor]);
            }

            foreach (PointD p in allPoints) {
                if (p.Equals(anchor)) continue;
                double angl = geohelper.GetAngle(p, anchor);
                anglePoints.Add(angl, p);
            }
            double biggestAngleKey = 0;
            double biggestAngleDiff = 0;
            for (int i = 0; i < anglePoints.Count; i++) {
                double anglediff = anglePoints.Keys[i] - anglePoints.Keys[(i + 1) % anglePoints.Count];
                anglediff = Math.Abs(anglediff);
                while (anglediff > 180) anglediff -= 360;
                anglediff = Math.Abs(anglediff);
                //if (anglediff < 0) anglediff += 2 * Math.PI;
                if (anglediff > biggestAngleDiff) {
                    biggestAngleDiff = anglediff;
                    biggestAngleKey = anglePoints.Keys[(i + 1) % anglePoints.Count];
                }
            }
            int startInd = anglePoints.IndexOfKey(biggestAngleKey);
            for (int i = 0; i < anglePoints.Count * 2; i++) {
                double key = anglePoints.Keys[(i + startInd) % anglePoints.Count];
                PointD p = anglePoints[key];
                gs.addLink(gs.Global.PortalMappingPointD[p], gs.Global.PortalMappingPointD[anchor]);
                for (int j = 0; j < i; j++) {
                    double jkey = anglePoints.Keys[(startInd + j) % anglePoints.Count];
                    double angleDiff = key - jkey;
                    if (angleDiff < 0) angleDiff += 360;
                    if (angleDiff > 180) continue;
                    gs.addLink(gs.Global.PortalMappingPointD[p], gs.Global.PortalMappingPointD[anglePoints[jkey]]);
                }
            }
            return gs;
        }
    }
}
