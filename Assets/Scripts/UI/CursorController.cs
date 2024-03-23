using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{
    private TooltipController _tooltip;
    private RectTransform _rect;
    private Canvas _canvas;
    [SerializeField] private Image _cursor;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _tooltip = GetComponentInChildren<TooltipController>();

        //Cursor.visible = false;
    }

    void Update()
    {
        _rect.anchoredPosition = Input.mousePosition;

        AdjustTooltipPosition();

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            ClickAnim();
        }
    }

    private void AdjustTooltipPosition()
    {
        if (!_tooltip.Enabled) return;
        Vector2 pos = _rect.anchoredPosition;

        if (_rect.anchoredPosition.x + 50 + _tooltip.Width > _canvas.pixelRect.width)
            pos = new Vector2(-343, pos.y);
        else
            pos = new Vector2(50, pos.y);

        if (_rect.anchoredPosition.y - _tooltip.Height < 0)
            pos = new Vector2(pos.x, _tooltip.Height - 33);
        else
            pos = new Vector2(pos.x, 3);

        _tooltip.Rect.anchoredPosition = pos;
    }

    private void ClickAnim()
    {
        //_cursor.rectTransform.DOKill();
        //_cursor.rectTransform.anchoredPosition += new Vector2(0, 3);
        //_cursor.rectTransform.DOAnchorPos(Vector2.zero, 0.2f);

        //_cursor.sprite = _click;
        //Util.Delay(0.2f, () => _cursor.sprite = _default);
    }
}
