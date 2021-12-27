using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SwitchPanel : UI_Element {

    public UI_Image background;
    public UI_Button[] buttons;

	public UI_SwitchPanel(string _name, RectTransform _rootT, int count) : base(_name, UIElementType.SwitchPanel) {
        background = CreateImage("background", null).Sprite("square").Color(MAssets.colors["switch_panel"]);
        background.rootRend.SetParent(_rootT);
        background.rootRend.AnchorMin(0, 0).AnchorMax(1f, 0).OffsetMin(0f, 0).OffsetMax(0, 200);

        buttons = new UI_Button[count];
        for (int i = 0; i < count; i++) {
            UI_Button button = new UI_Button("button" + i, background.rootRend.GetRectT(), new UI_Button.UIData_Button() {
                defaultColor = MAssets.colors["button"]
            });

            button.SetClick((x) => {
                OnButtonClick((int)x);
            }, i);

            float start = i / (float)count;
            float end = (i + 1) / (float)count;
            button.rootRend.AnchorMin(start, 0f).AnchorMax(end, 1f).OffsetInset(10, 10);
            button.CreateText("#" + i, Color.white);
        }
    }

    private void OnButtonClick(int ind) {
        Debug.Log("Button " + ind + " is pressed");
    }

}
