using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyLinkLib {
    class Zobrist {
        static Random r = new Random(0);
        static Dictionary<string, List<long>> values = new Dictionary<string, List<long>>();

        static public long getValue(string key, int index) {
            if (!values.ContainsKey(key)) {
                values[key] = new List<long>();
            }
            List<long> curList = values[key];
            long rand = ((long)r.Next()) << 32;
            rand |= (uint)r.Next();
            while (curList.Count <= index + 1) curList.Add(rand);
            return curList[index];
        }
    }
}
