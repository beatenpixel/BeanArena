using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemButton : UIButtonBase {

    public ItemButtonState buttonState { get; private set; }

    public IconDrawer iconDrawer;

    [SerializeField] private ItemButtonConfig config;
    public override UIButtonConfig baseConfig => config;

    private Vector2 pointerPressPos;
    private bool isDragging;
    private float pressStartTime;

    public event Action<UIEventType, ItemButton, object> OnEvent;
    private object onClickArg;

    public GD_Item currentItem { get; private set; }

    private Orientation dragOrientation = Orientation.Right;
    private InventoryGroupDrawer inventoryGroupDrawer;

    public void Init_ItemButton(InventoryGroupDrawer groupDrawer) {
        inventoryGroupDrawer = groupDrawer;
    }

    private void Update() {
        if(isPressed) {
            Vector2 newPointerPos = Input.mousePosition;
            Vector2 dd = (newPointerPos - pointerPressPos);
            
            if((dragOrientation == Orientation.Right && dd.x > Screen.width * 0.05f)
                ||(dragOrientation == Orientation.Left && dd.x < Screen.width * -0.05f)
                || (Time.time > pressStartTime + 0.5f && dd.magnitude < Screen.width * 0.05f)) {
                StartDrag();
            }
        }

        if(isDragging) {
            subRectT.position = Vector2.Lerp(subRectT.position, Input.mousePosition, Time.deltaTime * 15f);

            if(Input.GetMouseButtonUp(0)) {
                StopDrag();
            }
        }
    }

    public void SetState(ItemButtonState state) {
        buttonState = state;
    }

    public void SetDragDirection(Orientation _dragOrientation) {
        dragOrientation = _dragOrientation;
    }

    public void SetItem(GD_Item itemData, SO_ItemInfo itemInfo) {
        currentItem = itemData;

        iconDrawer.DrawItem(itemData, itemInfo);
    }

    public void AlignToWorldFrame(WUI_EquipmentSlotFrame frame) {
        rectT.SetParent(frame.rectT);
        rectT.localScale = Vector3.one;
        subRectT.localScale = Vector3.one;

        rectT.SetAnchor(Vector2.zero, Vector2.one);
        rectT.SetOffset(Vector2.zero, Vector2.zero);

        SetDragDirection(Orientation.Left);
    }

    public void AlignToItemList() {
        rectT.SetParent(inventoryGroupDrawer.itemsButtonsRootT);
        rectT.localScale = Vector3.one;
        subRectT.localScale = Vector3.one;

        SetDragDirection(Orientation.Right);
    }

    private void StartDrag() {
        if (isDragging)
            return;

        subRectT.SetParent(UIWindowManager.inst.uiCanvas.canvasT);
        subRectT.SetAsLastSibling();
        subRectT.localScale = Vector3.one;

        if(buttonState == ItemButtonState.InHeroSlot) {
            subRectT.position = Input.mousePosition;
        }

        isDragging = true;

        OnEvent?.Invoke(UIEventType.DragStart, this, onClickArg);
    }

    private void StopDrag() {
        subRectT.SetParent(rectT);
        subRectT.anchoredPosition = startAnchoredPosition;
        isDragging = false;

        OnEvent?.Invoke(UIEventType.DragEnd, this, onClickArg);
    }

    public void SetArg(object arg) {
        onClickArg = arg;
    }

    protected override void OnBecomePressed(PointerEventData eventData) {
        //base.OnBecomePressed(eventData);

        pressStartTime = Time.time;
        pointerPressPos = eventData.position;
        isDragging = false;

        //subRectT.DOKill(true);
        //subRectT.DOScale(startScale * config.pressScale, config.pressDuration).SetUpdate(true).SetEase(Ease.OutBack);
    }

    protected override void OnBecomeUnpressed(PointerEventData eventData) {
        //base.OnBecomeUnpressed(eventData);

        //subRectT.DOKill(true);
        //subRectT.DOScale(startScale, config.pressDuration).SetUpdate(true).SetEase(Ease.OutBack);
    }

    protected override void OnClick(PointerEventData eventData) {
        base.OnClick(eventData);

        OnEvent?.Invoke(UIEventType.Click, this, onClickArg);
        MSound.Play("click", SoundConfig.randVolumePitch01);

        subRectT.DOKill(true);
        subRectT.DOPunchScale(startScale * config.punchScale, 0.2f, 12).SetUpdate(true);
    }

    public enum ItemButtonState {
        None,
        InInventory,
        InHeroSlot
    }

}

[System.Serializable]
public class ItemButtonConfig : UIButtonConfig {
    public float pressScale = 0.9f;
    public float pressDuration;
    public float punchScale = 0.1f;
}
