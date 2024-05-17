using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    public string Title;
    [TextArea] public string Description;
    public float Delay = 0.3f;
    public bool Dynamic;
    public bool NotUI;

    private Tweener _timer;
    private bool _focused;

    public void SetInfo(string title, string description, float delay = 0)
    {
        Title = title;
        Description = description;
        Delay = delay;
    }

    private void Update()
    {
        if(Dynamic && _focused)
            TooltipController.Instance.SetTooltip(Title, Description);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipController.Instance.Enable();
        _timer = Util.Delay(Delay,()=> 
        { 
            _focused = true;
            TooltipController.Instance.SetTooltip(Title, Description); 
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _timer.Kill();
        _focused = false;
        TooltipController.Instance.HideTooltip();
    }

    private void OnMouseEnter()
    {
        if (NotUI) OnPointerEnter(null);
    }

    private void OnMouseExit()
    {
        if (NotUI) OnPointerExit(null);
    }
}
