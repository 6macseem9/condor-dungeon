using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(0)]
public class UnitSpawner : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private Unit _unit;
    private Animator _portrait;
    private NexusWindow _nexus;
    private Tooltip _tooltip;

    private void Start()
    {
        _portrait = GetComponent<Animator>();
        _tooltip = GetComponent<Tooltip>();
        _nexus = GetComponentInParent<NexusWindow>();
    }

    public void SetUnit(Unit unit)
    {
        _unit = unit;
        _portrait.ReplaceClip(unit.Class.Turnaround);
        _tooltip.SetInfo($"<color=#{ColorUtility.ToHtmlStringRGB(unit.Class.ClassColor)}>{unit.Class.ClassName.ToUpper()}</color>",
            unit.Class.ClassDescription,0);
    }

    public void Spawn()
    {
        _portrait.transform.DOKill();
        _portrait.transform.DOScale(5f, 0);
        _portrait.transform.DOScale(3, 0.2f);
        _nexus.Spawn(_unit);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
