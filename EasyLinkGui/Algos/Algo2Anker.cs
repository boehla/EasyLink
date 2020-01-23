using EasyLinkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasyLinkLib.geohelper;

namespace EasyLinkGui.Algos {
    class Algo2Anker : AlgoDummy {

        internal override GameState getBestGame(GameState gs) {
            SortedList<double, PortalInfo> allPoints = new SortedList<double, PortalInfo>();

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

            Vector mid = new Vector(midX, midY);



            foreach (PortalInfo pInfo in gs.PortalInfos) {
                if (pInfo.Guid.Equals(gs.Global.AnchorsPortals[0].Guid)) continue;
                if (pInfo.Guid.Equals(gs.Global.AnchorsPortals[1].Guid)) continue;

                Vector p = new Vector(pInfo.Pos);

                Vector distVec = mid - p;

                double dist = Math.Abs(distVec.X) + Math.Abs(distVec.X);
                dist = distVec.X * distVec.X + distVec.Y * distVec.Y;
                if (allPoints.ContainsKey(dist)) continue;
                allPoints.Add(dist, pInfo);
            }

            GameState best = null;
            GameState newGame = gs.DeepClone();
            while (true) {
                

                

                int bestNextLinkScore = 0;
                PortalInfo bestNextLInk = null;
                foreach (KeyValuePair<double, PortalInfo> item in allPoints) {
                    if (!newGame.checkLink(item.Value.Guid, gs.Global.AnchorsPortals[0].Guid)) continue;
                    if (!newGame.checkLink(item.Value.Guid, gs.Global.AnchorsPortals[1].Guid)) continue;

                    GameState tmpGame = newGame.DeepClone();
                    tmpGame.linkToAllAnchors(item.Value);

                    int canLinkCount = 0;
                    foreach (KeyValuePair<double, PortalInfo> tmp in allPoints) {
                        if (tmp.Key < item.Key) continue;
                        if (!tmpGame.checkLink(tmp.Value.Guid, gs.Global.AnchorsPortals[0].Guid)) continue;
                        if (!tmpGame.checkLink(tmp.Value.Guid, gs.Global.AnchorsPortals[1].Guid)) continue;
                        canLinkCount++;
                    }
                    if(bestNextLInk == null || canLinkCount > bestNextLinkScore) {
                        bestNextLinkScore = canLinkCount;
                        bestNextLInk = item.Value;
                    }
                }
                if(bestNextLInk == null) {
                    this.newBestGame(newGame);
                    return newGame;
                }
                if(this.Best == null || newGame.getAPScore() > this.Best.getAPScore()) {
                    this.newBestGame(newGame);
                }
                //if (allPoints.ElementAt(0).Value.Guid.Equals("7d20a9e41685453ebcabd9d7edb5e086.16")) {
                newGame.linkToAllAnchors(bestNextLInk);
                /*
                if (allPoints.ElementAt(0).Value.Guid.Equals("b94d809debed4b48bd29ee80d1150370.16")) {
                    this.newBestGame(newGame);
                    return newGame;
                }

                if (best == null || best.getAPScore() < newGame.getAPScore()) {
                    best = newGame;
                    this.newBestGame(best);
                }
                this.LastHandled = newGame;

                allPoints.RemoveAt(0);
                */
            }


            return best;
        }
    }
}
