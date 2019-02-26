using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EasyLinkLib.geohelper;

// decode source: https://github.com/iitc-project/ingress-intel-total-conversion/blob/0fabfb68943fc833153d89454220fa638a8b9fb6/code/entity_decode.js
namespace EasyLinkLib {
    public enum IngressTeam { None, Enlightened, Resistance }
    public class IngressEntities {
    }
    public class CoreEntity {
        public string Guid { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreateDate { get; set; }
        public CoreEntity(JToken data) {
            if (data == null) return;
            Guid = Lib.Converter.toString(data[0]);
            Date = Lib.Converter.toDateTime(Lib.Converter.toLong(data[1]) / 1000, DateTime.MinValue, "1970");
            CreateDate = DateTime.UtcNow;
        }
        public double latlonConverter(int val) {
            return val / 1e6;
        }

        public IngressTeam getTeam(object val) {
            string strval = Lib.Converter.toString(val);
            switch (strval.ToUpper()) {
                case "ENLIGHTENED": 
                case "E": return IngressTeam.Enlightened;

                case "RESISTANCE":
                case "R":
                    return IngressTeam.Resistance;

                default: return IngressTeam.None;
            }
        }
        public static Color getTeamColor(IngressTeam team) {
            switch (team) {
                case IngressTeam.Enlightened: return Color.Green;
                case IngressTeam.Resistance: return Color.Blue;
                default: return Color.Gray;
            }
        }
    }
    /*
     *team:          a[1],
      latE6:         a[2],
      lngE6:         a[3]
    }
  };

  var SUMMARY_PORTAL_DATA_LENGTH = 14;
  function summaryPortalData(a) {
    return {
      level:         a[4],
      health:        a[5],
      resCount:      a[6],
      image:         a[7],
      title:         a[8],
      ornaments:     a[9],
      mission:       a[10],
      mission50plus: a[11],
      artifactBrief: parseArtifactBrief(a[12]),
      timestamp:     a[13]
      */
    public class PortalEntity : CoreEntity {
        public PointD Pos { get; }
        public IngressTeam Team { get; }
        public string Name { get; }

        public int Level { get; }
        public double Health { get; }
        public int ResCount { get; }
        public string Image { get; }
        public bool Mission { get; }

        public PortalEntity(JToken data) : base(data) {
            JToken details = data[2];
            if (!Lib.Converter.toString(details[0]).Equals("p")) throw new Exception("This is not a portal!");

            this.Team = getTeam(details[1]);
            this.Pos = new PointD(latlonConverter(Lib.Converter.toInt(details[3])), latlonConverter(Lib.Converter.toInt(details[2])));
            this.Level = Lib.Converter.toInt(details[4]);
            this.Health = Lib.Converter.toDouble(details[5]);
            this.ResCount = Lib.Converter.toInt(details[6]);
            this.Image = Lib.Converter.toString(details[7]);
            this.Name = Lib.Converter.toString(details[8]);

            this.Mission = Lib.Converter.toBool(details[10]);


        }
    }
    /*
team:   ent[2][1],
oGuid:  ent[2][2],
oLatE6: ent[2][3],
oLngE6: ent[2][4],
dGuid:  ent[2][5],
dLatE6: ent[2][6],
dLngE6: ent[2][7]
*/
    public class LinkEntity : CoreEntity {
        public IngressTeam Team { get; }

        public PointD OPos { get; }
        public String OGuid { get; }

        public PointD DPos { get; }
        public String DGuid { get; }

        public Color Color {
            get {
                return CoreEntity.getTeamColor(this.Team);
            }
        }

        public LinkEntity(JToken data) : base(data) {

    JToken details = data[2];
            if (!Lib.Converter.toString(details[0]).Equals("e")) throw new Exception("This is not a Link!");

            this.Team = getTeam(details[1]);

            this.OGuid = Lib.Converter.toString(details[2]);
            this.OPos = new PointD(latlonConverter(Lib.Converter.toInt(details[4])), latlonConverter(Lib.Converter.toInt(details[3])));

            this.DGuid = Lib.Converter.toString(details[5]);
            this.DPos = new PointD(latlonConverter(Lib.Converter.toInt(details[7])), latlonConverter(Lib.Converter.toInt(details[6])));
        }
        public LinkEntity(LinkEntityDataset val) : base(null) {
            this.Guid = val.Guid;
            this.Team = (IngressTeam)val.Team;
            this.CreateDate = val.LastRefresh;

            this.OGuid = val.OGuid;
            this.OPos = new PointD(val.OPosX, val.OPosY);

            this.DGuid = val.DGuid;
            this.DPos = new PointD(val.DPosX, val.DPosY);
        }

        public LinkEntityDataset parseToDataset() {
            LinkEntityDataset ret = new LinkEntityDataset();

            ret.Guid = this.Guid;
            ret.Team = (int)this.Team;
            ret.LastRefresh = this.CreateDate;

            ret.OGuid = this.OGuid;
            ret.OPosX = this.OPos.X;
            ret.OPosY = this.OPos.Y;

            ret.DGuid = this.DGuid;
            ret.DPosX = this.DPos.X;
            ret.DPosY = this.DPos.Y;

            return ret;
        }
    }
    /*
type: ent[2][0],
team: ent[2][1],
points: ent[2][2].map(function(arr) { return {guid: arr[0], latE6: arr[1], lngE6: arr[2] }; })
*/
    public class FieldEntity : CoreEntity {

        public FieldEntity(JToken data) : base(data) {

            JToken details = data[2];
            if (!Lib.Converter.toString(details[0]).Equals("r")) throw new Exception("This is not a Field!");
        }
    }


}
