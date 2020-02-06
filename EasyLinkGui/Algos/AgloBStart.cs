using EasyLinkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static EasyLinkLib.geohelper;

namespace EasyLinkGui.Algos {
    class AgloBStart : AlgoDummy {

        SharedCalcData shared = null;
        int nextthreadid = 0;
        GameState gs = null;
        internal override void init() {
            this.Settings = new CustSettings();
        }
        private CustSettings custSettings{
            get { return (CustSettings)this.Settings; }
            }
        internal override GameState getBestGame(GameState gs) {
            this.gs = gs;
            shared = new SharedCalcData();

            if(gs.Global.AnchorsPortals.Count == 2) {
                int p1 = gs.getIndexByGuid(gs.Global.AnchorsPortals[0].Guid);
                int p2 = gs.getIndexByGuid(gs.Global.AnchorsPortals[1].Guid);
                if (!gs.PortalData[p1].SideLinks.ContainsKey(p2)) {
                    gs.addLink(p1, p2);
                    gs = gs.DeepClone();
                }
            }



            shared.toDo.Add((float)gs.getSearchScore(), gs);

            Thread d = new Thread(CalcThread);
            d.IsBackground = true;
            d.Priority = ThreadPriority.BelowNormal;
            shared.RunningThreads++;
            d.Start();

            Thread.Sleep(3000);

            int countEmpty = 0;
            try {
                while (!calcing) {
                    lock (shared) {
                        if (shared.toDo.Count > 0) {
                            countEmpty = 0;
                        } else {
                            countEmpty++;
                            if (countEmpty > 5) return shared.bestGame;
                        }
                    }
                    Thread.Sleep(1000);
                }
            } catch (Exception ex) {
                Lib.Logging.logException("", ex);
            } finally {
            }

            return shared.bestGame;
        }

        public void CalcThread() {
            bool alreadyCountDown = false;
            int threadid = nextthreadid++;
            try {
                Lib.Logging.log("thread.txt", "Starting new Thread: " + threadid);
                int countEmpty = 0;
                while (!calcing) {
                    GameState curToDo = null;
                    lock (shared) {
                        int targetThreads = Math.Max(1, custSettings.TargetThreadCount);
                        while (targetThreads > shared.RunningThreads) {
                            Thread d = new Thread(CalcThread);
                            d.IsBackground = true;
                            d.Priority = ThreadPriority.BelowNormal;
                            shared.RunningThreads++;
                            d.Start();
                        }
                        if (shared.RunningThreads > targetThreads) {
                            alreadyCountDown = true;
                            shared.RunningThreads--;
                            return;
                        }
                        if (shared.toDo.Count > 0) {
                            curToDo = shared.toDo.ElementAt(0).Value;
                            shared.toDo.RemoveAt(0);
                            countEmpty = 0;
                        }
                    }
                    if (curToDo == null) {
                        Thread.Sleep(1000);
                        countEmpty++;
                        if (countEmpty > 5) return;
                        continue;
                    }
                    lock (shared) {
                        LastHandled = curToDo;
                    }
                    Lib.Performance.setWatch("GetAllPossible", true);
                    List<GameState> nextgs = curToDo.getAllPossible();
                    Lib.Performance.setWatch("GetAllPossible", false);


                    foreach (GameState item in nextgs) {
                        long hash = item.GetLongHashCode();
                        //float gamescore = (float)item.getGameScore();
                        //float searchscore = (float)item.getSearchScore();
                        calcSearchScore(item);
                        float gamescore = (float)item.getSearchScore();
                        float searchscore = gamescore;
                        lock (shared) {
                            if (shared.bestGame == null || gamescore > shared.bestVal) {
                                shared.bestVal = gamescore;
                                shared.bestGame = item;
                                shared.resultTime = DateTime.UtcNow;
                                newBestGame(shared.bestGame);
                            } else {
                                if (shared.allGamesViewed.ContainsKey(hash)) continue;
                            }
                            shared.toDo.Add(searchscore, item);

                            while (shared.toDo.Count > 10000) {
                                shared.toDo.RemoveAt(shared.toDo.Count - 1);
                            }

                            shared.allGamesViewed[hash] = true;
                        }
                    }
                }
            } catch (Exception ex) {
                Lib.Logging.logException("", ex);
            } finally {
                Lib.Logging.log("thread.txt", "Stop thread: " + threadid);
                lock (shared) {
                    if (!alreadyCountDown) shared.RunningThreads--;
                    if (shared.RunningThreads == 0) {
                        shared = new SharedCalcData();
                    }
                }
            }
        }

        private void calcSearchScore(GameState gs) {
            gs.CustSearchScore = gs.getAPScore() - gs.TotalWay * 5;
            //gs.CustSearchScore = gs.TotalArea;
        }

        class SharedCalcData {
            public float bestVal = 0;
            public GameState bestGame = null;
            public Dictionary<long, bool> allGamesViewed = new Dictionary<long, bool>();
            public SortedList<float, GameState> toDo = new SortedList<float, GameState>(new DuplicateKeyComparer<float>());

            public DateTime resultTime;

            public int RunningThreads = 0;
        }

        class CustSettings {
            public int TargetThreadCount { get; set; } = 1;
        }
    }
}
