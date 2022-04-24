using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrewNetwork {

    public class CrewNetDebug {

        public static void Log(object msg) {
#if UNITY
            UnityEngine.Debug.Log(msg);
#else
            Console.WriteLine(msg);
#endif
        }

        public static void LogError(object msg) {
#if UNITY
            UnityEngine.Debug.LogError(msg);
#else
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ResetColor();
#endif
        }

    }

}
