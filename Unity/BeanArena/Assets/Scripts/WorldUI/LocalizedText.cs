using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour {

    public TextMeshPro text;
    public int maxLength = -1;

    public void Localize(string str) {
        if (maxLength == -1 || str.Length <= maxLength) {
            text.text = str;
        } else {
            text.text = str.Substring(0, maxLength) + ".";            
        }
    }

}
