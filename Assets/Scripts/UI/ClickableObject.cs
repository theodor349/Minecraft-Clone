using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ClickableObject : Selectable, IPointerClickHandler, ISubmitHandler
{
    [Serializable]
    public class ButtonClickedEvent : UnityEvent { }

    [FormerlySerializedAs("LeftClick")]
    [SerializeField]
    private ButtonClickedEvent m_LeftClick = new ButtonClickedEvent();

    [FormerlySerializedAs("RightClick")]
    [SerializeField]
    private ButtonClickedEvent m_RightClick = new ButtonClickedEvent();

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            LeftPress();
        else if (eventData.button == PointerEventData.InputButton.Right)
            RightPress();
    }

    private void LeftPress()
    {
        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onClick", this);
        m_LeftClick.Invoke();
    }

    private void RightPress()
    {
        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onClick", this);
        m_RightClick.Invoke();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        throw new NotImplementedException();
    }
}