using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyLinkLib {
    public class LinkLookupTbl {
        byte[] data;
        int bitcount = 0;

        string filename = "";
        FileStream fs;

        public LinkLookupTbl(GameState gs) {

            int tmp = gs.PortalData.Count;
            while (tmp > 0) {
                tmp = tmp >> 1;
                bitcount++;
            }
            if (bitcount * 4 >= 32) {
                filename = "linklookup.bin";
                //if (File.Exists(filename)) File.Delete(filename);
                //fs = new FileStream(filename, FileMode.OpenOrCreate);
                return;
            } else {
                data = new byte[((1 << (bitcount * 4)) + 7) / 8];
            }

            if (data != null) {
                for (int p1 = 0; p1 < gs.PortalInfos.Count; p1++) {
                    for (int p2 = 0; p2 < gs.PortalInfos.Count; p2++) {
                        if (p1 == p2) continue;
                        for (int p3 = 0; p3 < gs.PortalInfos.Count; p3++) {
                            if (p2 == p3 || p1 == p3) continue;
                            for (int p4 = 0; p4 < gs.PortalInfos.Count; p4++) {
                                if (p3 == p4 || p2 == p4 || p1 == p4) continue;

                                long ind = ((long)p1 << (bitcount * 3)) | ((long)p2 << (bitcount * 2)) | ((long)p3 << (bitcount * 1)) | ((long)p4 << (bitcount * 0));
                                int byteInd = (int)(ind >> 3);
                                int bitInd = (int)(ind & 7);

                                if (geohelper.FindIntersection(gs.PortalInfos[p1], gs.PortalInfos[p2], gs.PortalInfos[p3], gs.PortalInfos[p4])) {
                                    data[byteInd] |= (byte)(1 << bitInd);
                                }
                            }
                        }
                    }
                }
            } else {
                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = 8;
                long total = 0;
                Parallel.For(0, gs.PortalInfos.Count, po,
                   p1 => {
                       for (int p2 = 0; p2 < gs.PortalInfos.Count; p2++) {
                           if (p2 == p1) continue;

                           long tmpLength = gs.PortalInfos.Count * gs.PortalInfos.Count;
                           byte[] block = new byte[(tmpLength + 7) / 8];
                           long blockInd = block.Length * (p1 * gs.PortalInfos.Count + p2);
                           for (int p3 = 0; p3 < gs.PortalInfos.Count; p3++) {
                               if (p3 == p1 || p3 == p2) continue;
                               for (int p4 = 0; p4 < gs.PortalInfos.Count; p4++) {
                                   if (p4 == p3) continue;
                                   if (p4 == p1 || p4 == p2) continue;
                                   total++;

                                   long bitId = p3 * gs.PortalInfos.Count + p4;
                                   int byteInd = (int)(bitId >> 3);
                                   int bitInd = (int)(bitId & 7);

                                   if (geohelper.FindIntersection(gs.PortalInfos[p1], gs.PortalInfos[p2], gs.PortalInfos[p3], gs.PortalInfos[p4])) {
                                       block[byteInd] |= (byte)(1 << bitInd);
                                   }
                                   /*
                                   long ind = ((long)p3 << (bitcount * 1)) | ((long)p4 << (bitcount * 0));
                                   int byteInd = (int)(ind >> 3);
                                   int bitInd = (int)(ind & 7);

                                   if (geohelper.FindIntersection(gs.PortalInfos[p1], gs.PortalInfos[p2], gs.PortalInfos[p3], gs.PortalInfos[p4])) {
                                       block[byteInd] |= (byte)(1 << bitInd);
                                   }*/
                               }
                           }
                           lock (fs) {
                               fs.Position = blockInd;
                               fs.Write(block, 0, block.Length);
                           }
                       }
                   });

                fs.Close();
                fs = new FileStream(filename, FileMode.Open);
            }
        }

        public bool crossLink(GameState gs, int p1id, int p2id) {
            if(fs != null) {
                if (p2id < p1id) {
                    int sw = p1id;
                    p1id = p2id;
                    p2id = sw;
                }

                long tmpLength = gs.PortalInfos.Count * gs.PortalInfos.Count;
                byte[] block = new byte[(tmpLength + 7) / 8];
                long blockInd = block.Length * (p1id * gs.PortalInfos.Count + p2id);

                lock (fs) {
                    fs.Position = blockInd;
                    fs.Read(block, 0, block.Length);
                }
                int byteInd = 0;
                int bitInd = 0;
                while(byteInd < block.Length) {
                    byte b = block[byteInd];
                    bitInd = 0;
                    while(b > 0) {
                        bool interSec = (b & 1) == 1;
                        b = (byte)(b >> 1);
                        
                        if (interSec) {
                            int linkInd = byteInd * 8 + bitInd;
                            int p3 = linkInd / gs.PortalData.Count;
                            int p4 = linkInd % gs.PortalData.Count;
                            if (gs.PortalData[p3].SideLinks.ContainsKey(p4)) {
                                bool recheck = geohelper.crossLink(gs, p1id, p2id);
                                if (!recheck) {

                                }
                                return true;
                            }
                        }
                        bitInd++;
                    }
                    byteInd++;
                }
            } else if(data != null) {
                for (int i = 0; i < gs.PortalInfos.Count; i++) {
                    if (i == p1id || i == p2id) continue;
                    foreach (KeyValuePair<int, bool> item in gs.PortalData[i].SideLinks) {
                        //if (!item.Value) continue;
                        //Point p = LineIntersection.FindIntersection(new Line(gs.PortalInfos[p1id], gs.PortalInfos[p2id]), new Line(gs.PortalInfos[i], gs.PortalInfos[item.Key]), 0.00000000000);
                        long ind = ((long)p2id << (bitcount * 3)) | ((long)p1id << (bitcount * 2)) | ((long)i << (bitcount * 1)) | ((long)item.Key << (bitcount * 0));
                        int byteInd = (int)(ind >> 3);
                        int bitInd = (int)(ind & 7);

                        //bool inters = FindIntersection(gs.PortalInfos[p1id], gs.PortalInfos[p2id], gs.PortalInfos[i], gs.PortalInfos[item.Key]);
                        //if (!p.Equals(default(Point))) return true;
                        if (((data[byteInd] >> bitInd) & 1) == 1) return true;
                    }

                }
            } else {
                return geohelper.crossLink(gs, p1id, p2id);
            }


            return false;
        }
    }
}
