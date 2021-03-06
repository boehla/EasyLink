﻿using EasyLinkLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyLinkGui.Algos {
    class AlgoDummy {

        public Dictionary<string, object> Parameter { get; set; }
        internal bool calcing = false;
       public GameState LastHandled { get; set; }
       public GameState Best { get; set; }

        public event Action<GameState> OnNewBestGameState;
        public event Action<GameState> OnCalculationFinish;

        private GameState startGame = null;

        public AlgoDummy() {
            Parameter = new Dictionary<string, object>();
            init();
        }

        public void startCalc(GameState gs) {
            startGame = gs.DeepClone();

            Thread thread = new Thread(() => startCalcMulti(startGame));
            thread.IsBackground = true;
            thread.Start();

        }
        private void startCalcMulti(GameState gs) {
            GameState best = null;
            try {
                best = getBestGame(gs);
            } finally {
                calcing = false;
                OnCalculationFinish(best);
            }
        }

        public void newBestGame(GameState gs) {
            OnNewBestGameState(gs);
            this.Best = gs;
        }
        internal virtual void init() {

        }
        internal virtual GameState getBestGame(GameState gs) {
            return null;
        }

        public void cancel() {
            this.calcing = true;
        }

        public object Settings { get; set; }
    }
}
