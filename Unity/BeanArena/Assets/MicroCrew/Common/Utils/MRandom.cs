using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MRandom {

    public static int[] randomIndexesCache = new int[2000];

    public static T GetWithChance<T>(T[] items, int[] chances) {
        int ind = -1;

        for (int i = 0; i < items.Length; i++) {
            for (int x = 0; x < chances[i]; x++) {
                randomIndexesCache[++ind] = i;
            }
        }

        return items[randomIndexesCache[UnityEngine.Random.Range(0, ind)]];
    }

    public static T Get<T>(params RandomEntry<T>[] entries) {
        int ind = -1;

        for (int i = 0; i < entries.Length; i++) {
            for (int x = 0; x < entries[i].probability; x++) {
                randomIndexesCache[++ind] = i;
            }
        }

        return entries[randomIndexesCache[UnityEngine.Random.Range(0, ind)]].item;
    }

    public static T Get<T>(List<RandomEntry<T>> entries) {
        int ind = -1;

        for (int i = 0; i < entries.Count; i++) {
            for (int x = 0; x < entries[i].probability; x++) {
                randomIndexesCache[++ind] = i;
            }
        }

        return entries[randomIndexesCache[UnityEngine.Random.Range(0, ind)]].item;
    }

    public static T GetRandom<T>(this T[] arr) {
        return arr[UnityEngine.Random.Range(0, arr.Length)];
    }

    public static float Range(float min, float max) {
        return UnityEngine.Random.Range(min, max);
    }

    public static int Range(int min, int max) {
        return UnityEngine.Random.Range(min, max);
    }

    public static float SignedRange(float absMin, float absMax) {
        return Range(absMin, absMax) * Sign();
    }

    public static int Sign() {
        return (UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1);
    }

    public static bool GetBool() {
        return UnityEngine.Random.Range(0, 2) == 0;
    }

    public static Vector2 OnRectEdge(Vector2 size) {
        if(GetBool()) {
            return new Vector2(size.x * 0.5f * Sign(), Range(-size.y * 0.5f, size.y * 0.5f));
        } else {
            return new Vector2(Range(-size.x * 0.5f, size.x * 0.5f), size.y * 0.5f * Sign());
        }
    }
    public static Vector2 InsideRect(Vector2 size) {
        return new Vector2(MRandom.Range(-size.x, size.x), MRandom.Range(-size.y, size.y));
    }

    public static string RandomFirstName() {
        int sex = MRandom.Range(0, 2);
        if(sex == 0) {
            return randomMaleNames[MRandom.Range(0, randomMaleNames.Length)];
        } else {
            return randomFemaleNames[MRandom.Range(0, randomFemaleNames.Length)];
        }        
    }

    private static string[] randomMaleNames = new string[] {
        "James","John","Robert","Michael","William",
        "David","Richard","Charles","Joseph","Thomas",
        "Christopher","Daniel","Paul","Mark","Donald",
        "George","Kenneth","Steven","Edward","Brian",
        "Ronald","Anthony","Kevin","Jason","Matthew",
        "Gary","Timothy","Jose","Larry","Jeffrey",
        "Frank","Scott","Eric","Stephen","Andrew",
        "Raymond","Jack","Gregory","Joshua","Jerry",
        "Dennis","Walter","Patrick","Peter","Harold",
        "Douglas","Henry","Carl","Arthur","Ryan"
    };

    private static string[] randomFemaleNames = new string[] {
        "Mary", "Lisa", "Michelle", "Brenda",   "Stephanie",    "Alice",    "Katherine",    "Kathy",    "Andrea",   "Lois",
        "Patricia", "Nancy",    "Laura",    "Amy",  "Carolyn",  "Julie",    "Joan", "Theresa",  "Kathryn",  "Tina",
        "Linda",    "Karen",    "Sarah",    "Anna", "Christine",    "Heather",  "Ashley",   "Beverly",  "Louise",   "Phyllis",
        "Barbara",  "Betty",    "Kimberly", "Rebecca",  "Marie",    "Teresa",   "Judith",   "Denise",   "Sara", "Norma",
        "Elizabeth",    "Helen",    "Deborah",  "Virginia", "Janet",    "Doris",    "Rose", "Tammy",    "Anne", "Paula",
        "Jennifer", "Sandra",   "Jessica",  "Kathleen", "Catherine",    "Gloria",   "Janice",   "Irene",    "Jacqueline",   "Diana",
        "Maria",    "Donna",    "Shirley",  "Pamela",   "Frances",  "Evelyn",   "Kelly",    "Jane", "Wanda",    "Annie",
        "Susan",    "Carol",    "Cynthia",  "Martha",   "Ann",  "Jean", "Nicole",   "Lori", "Bonnie",   "Lillian",
        "Margaret", "Ruth", "Angela",   "Debra",    "Joyce",    "Cheryl",   "Judy", "Rachel",   "Julia",    "Emily",
        "Dorothy",  "Sharon",   "Melissa",  "Amanda",   "Diane",    "Mildred",  "Christina",    "Marilyn",  "Ruby", "Robin"
    };

}

public struct RandomEntry<T> {

    public int probability;
    public T item;

    public RandomEntry(T item, int probability) {
        this.item = item;
        this.probability = probability;
    }
}