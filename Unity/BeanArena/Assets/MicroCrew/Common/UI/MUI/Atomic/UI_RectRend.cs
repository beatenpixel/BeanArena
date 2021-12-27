using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_RectRend : UI_Rend {

    public override Type GetPoolObjectType() {
        return typeof(UI_RectRend);
    }

}