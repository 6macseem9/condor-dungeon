using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour,IPointerClickHandler
{
    private TextMeshProUGUI _text;
    private Button _discard;
    private Tooltip _tooltip;
    private CanvasGroup _canvasGroup;

    private Item _item;

    private void Awake()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _tooltip = _text.GetComponent<Tooltip>();
        _discard = GetComponentInChildren<Button>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _discard.AddPressAnimation();
    }

    public void SetItem(Item item)
    {
        _canvasGroup.alpha = 0;

        _item = Instantiate(item);
        _item.Initiate();
        _text.text = $"<font=\"6pxBG SDF\"><mark=#36354d>{item.Name}</mark></font>";
        _text.color = item.Color;
        _tooltip.Title = $"<color=#{item.Color.ToHexString()}>" + item.Name;
        _tooltip.Description = item.Description+"\n\n<color=#a3c0e6>} CLICK TO USE ";

        _canvasGroup.DOFade(1, 0.2f).onComplete = () =>
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        };
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(_item.CombatOnly && !BattleController.Instance.InCombat)
        {
            CursorController.Instance.NotEnoughResource(NotEnough.Combat);
            return;
        }

        _item.Activate();
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        transform.DOShakeScale(0.3f,1.2f,30).onComplete= ()=> 
        {
            Discard();
        };
        

    }

    public void Discard()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        TooltipController.Instance.HideTooltip();

        _canvasGroup.DOFade(0, 0.2f).onComplete = () =>
        {
            gameObject.SetActive(false);
        };
    }
}
