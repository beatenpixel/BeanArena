using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotAnEasterEgg {

    // this guy is kinda cool - A Friend Of Darkness

    private static string[] first50DiscordBois = new string[] {
        "beatenpixel",
        "Carl-bot",
        "A Friend Of Darkness",
        "Двемер-питсоед",
        "respawn",
        "rokdan32",
        "sankalp",
        "ASSASiN",
        "ATav2010",
        "Average discord enjoyer",
        "Beluga",
        "Oberst Krautf",
        "d1amondzz",
        "Dank boi",
        "Dasha",
        "Dob",
        "VIPER20V",
        "Eduardo_533",
        "fideos con manteca",
        "gerardo",
        "Groovy",
        "i exist!!",
        "i like cats",
        "III",
        "Kabomb",
        "KoRshUn",
        "MalchicizGdhki",
        "Thehazmatguy049",
        "Pet Russian",
        "PhoenixDourHmWk",
        "pico",
        "player one",
        "Polonexo",
        "Purpel",
        "RadioactiveCerberus950",
        "safe3370",
        "talia",
        "tantan",
        "tetoon",
        "the lonely person",
        "TheVigilante",
        "vinnyforce",
        "xdavidx",
        "YotheR",
        "Ysuba",
        "Zarie",
        "epitacio gamer",
        "АК-47",
        "Шапка",
        "BigBadAud999",
        "a strange symbols dude was here"
    };   

    public static void PrintFirst50DiscordBois() {
        string str = "";
        for (int i = 0; i < first50DiscordBois.Length; i++) {
            str += first50DiscordBois[i] + ", ";
        }
        Debug.Log(str);
    }

}
