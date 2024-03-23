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
        _portrait.ReplaceClip(unit.Stats.Turnaround);
        _tooltip.SetInfo($"<color=#{ColorUtility.ToHtmlStringRGB(unit.Stats.ClassColor)}>{unit.Stats.ClassName.ToUpper()}</color>",
                         $"<color=#ffeb33>COST: {unit.Stats.Cost}g</color>\n"+ unit.Stats.ClassDescription,0);
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
