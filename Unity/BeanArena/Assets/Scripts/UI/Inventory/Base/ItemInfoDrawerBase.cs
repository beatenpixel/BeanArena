using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemInfoDrawerBase : MonoBehaviour {

    public GameObject panelRootGO;
    public ItemInfoPanel itemInfoPanel;

    public virtual void Init() {
        itemInfoPanel.Init();
    }

    public void Show(bool show) {
        panelRootGO.SetActive(show);
    }

    public virtual void DrawInfo(object item) {
        itemInfoPanel.DrawInfo((GD_Item)item);
    }

}
