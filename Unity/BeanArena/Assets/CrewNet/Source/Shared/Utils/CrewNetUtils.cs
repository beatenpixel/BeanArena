using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewNetwork {

    public static class CrewNetUtils {

        private static char[] randomChars;

        static CrewNetUtils() {
            randomChars = new char[35];
            for (int i = 0; i < 26; i++) {
                randomChars[i] = (char)('A' + i);
            }
            for (int i = 0; i < 9; i++) {
                randomChars[26 + i] = (char)('1' + i);
            }
        }

        public static string GeneratePrettyRoomUID(int length) {
            string str = "";
            Random random = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < length; i++) {
                if(i % 3 == 0 && i != length - 1 && i > 0) {
                    str += '-';
                }
                str += randomChars[random.Next(randomChars.Length)];
            }
            return str;
        }

    }

}
