using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace EasyLinkProxy {
    [ServiceContract(Namespace = "EasyLinkProxy")]
    public interface IEasyLink {
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        string SetData(string data, string hash);

        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetData(string key);
    }

    public class EasyLinkService : IEasyLink {
        public static string Password = "";
        public static Lib.ValueBuffer Valbuffer = new Lib.ValueBuffer();

        public string SetData(string data, string hash) {
            string key = CreateMD5(data + Password).ToLower();
            if (key.Equals(hash.ToLower())){
                Lib.Logging.log("setdata.txt", string.Format("{0}: {1:n0}b", key, data.Length));
                Valbuffer.setVal(key, data, TimeSpan.FromHours(24));
            } else {
                Lib.Logging.log("setdata.txt", string.Format("Wrong hash! {0} != {1}", key, hash.ToLower()));
                return "";
            }            
            return key;
        }
        public string GetData(string key) {
            string ret = Lib.Converter.toString(Valbuffer.getVal(key));
            Lib.Logging.log("getdata.txt", string.Format("{0}: {1:n0}b", key, ret.Length));
            return ret; ;
        }

        public static string CreateMD5(string input) {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create()) {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++) {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
