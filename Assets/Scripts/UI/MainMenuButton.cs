using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button _button;
    void Start()
    {
        _button = GetComponent<Button>();

        _button.onClick.AddListener(() =>
        {
            transform.DOScale(2.5f, 0);
            transform.DOScale(3.5f, 0.2f);
        });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_button.interactable) return;

        transform.DOScale(3,0);
        transform.DOScale(3.5f, 0.4f).SetEase(Ease.OutCirc);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_button.interactable) return;

        transform.DOScale(3.5f, 0);
        transform.DOScale(3, 0.4f);
    }
}
