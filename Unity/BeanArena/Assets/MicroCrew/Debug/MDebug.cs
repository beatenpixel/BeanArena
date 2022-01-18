using IngameDebugConsole;
using MicroCrew.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Tayx.Graphy;
using UnityEngine;

public class MDebug : Singleton<MDebug> {

    public GraphyManager graphyManager;
    public DebugLogManager debugLogManager;

    public override void Init() {
        
    }

    protected override void Shutdown() {
        
    }

    [ConsoleMethod("fps", "Show GraphyConsole")]
    public static void CreateCubeAt() {
        inst.graphyManager.ToggleActive();
    }

}

public interface IMLogger {
    
}

public class MLog {

    private string prefix;

    public static void Log(object message, bool addCallerInfo = false, [CallerMemberName] string memberName = "") {
        if(addCallerInfo) {
            StackTrace trace = new StackTrace();
            StackFrame frame = trace.GetFrame(1); // 0 will be the inner-most method
            MethodBase method = frame.GetMethod();

            UnityEngine.Debug.Log($"[{method.DeclaringType}::{memberName}] " + message);
        } else {
            UnityEngine.Debug.Log(message);
        }        
    }

    public static void LogLine(object message = null, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0) {
        StackTrace trace = new StackTrace();
        StackFrame frame = trace.GetFrame(1); // 0 will be the inner-most method
        MethodBase method = frame.GetMethod();

        UnityEngine.Debug.Log($"[{method.DeclaringType}::{memberName} Line {sourceLineNumber}]" + message??"");
    }

    public MLog(string _prefix) {
        prefix = "[" + _prefix + "] "; 
    }

    public void Log(object message) {
        UnityEngine.Debug.Log(prefix + message);
    }

    public void LogError(object message) {
        UnityEngine.Debug.LogError(prefix + message);
    }

}

public class MLogFont {
    private string _prefix;

    private string _suffix;

    public static MLogFont Bold = new MLogFont("b");
    public static MLogFont Italic = new MLogFont("i");
    private MLogFont(string format) {
        _prefix = $"<{format}>";
        _suffix = $"</{format}>";
    }

    public static string operator %(string text, MLogFont textFormat) {
        return textFormat._prefix + text + textFormat._suffix;
    }
}

public class MLogColor {

    // Color Example

    public static MLogColor Red = new MLogColor(Color.red);
    public static MLogColor Yellow = new MLogColor(Color.yellow);
    public static MLogColor Green = new MLogColor(Color.green);
    public static MLogColor Blue = new MLogColor(Color.blue);
    public static MLogColor Cyan = new MLogColor(Color.cyan);
    public static MLogColor Magenta = new MLogColor(Color.magenta);

    // Hex Example

    public static MLogColor Orange = new MLogColor("#FFA500");
    public static MLogColor Olive = new MLogColor("#808000");
    public static MLogColor Purple = new MLogColor("#800080");
    public static MLogColor DarkRed = new MLogColor("#8B0000");
    public static MLogColor DarkGreen = new MLogColor("#006400");
    public static MLogColor DarkOrange = new MLogColor("#FF8C00");
    public static MLogColor Gold = new MLogColor("#FFD700");

    private readonly string _prefix;

    private const string Suffix = "</color>";

    // Convert Color to HtmlString
    private MLogColor(Color color) {
        _prefix = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>";
    }
    // Use Hex Color
    private MLogColor(string hexColor) {
        _prefix = $"<color={hexColor}>";
    }

    public static string operator %(string text, MLogColor color) {
        return color._prefix + text + Suffix;
    }
}

