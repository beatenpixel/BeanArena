using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewNetwork {
    public static class CrewNetExt {

        public static string GetHEXString(this byte[] data) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++) {
                sb.Append(data[i].ToString("X2")).Append("_");
            }
            return sb.ToString();
        }

        public static string ToPrettyString<T>(this IList<T> list, string mainLabel, bool useNewLines, bool showRowInd = true, Func<T,string> toStringFunc = null) {
            StringBuilder sb = new StringBuilder(mainLabel + "\n");
            int counter = 0;
            foreach (var item in list) {
                sb.Append(showRowInd ? (counter++)+" " : "");
                sb.Append((toStringFunc==null)?item.ToString():toStringFunc(item));
                sb.Append((useNewLines ? "\n" : ", "));
            }
            return sb.ToString();
        }

    }
}
