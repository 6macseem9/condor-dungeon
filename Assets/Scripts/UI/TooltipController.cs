using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipController : MonoBehaviour
{
    public static TooltipController Instance;

    [SerializeField] private RectTransform _BG;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _desription;

    private RectTransform _rect;
    private CanvasGroup _group;

    public RectTransform Rect { get { return _rect; } }
    public float Width { get { return _BG.rect.width; } }
    public float Height { get { return _BG.rect.height; } }
    public bool Enabled { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else Instance = this;

        _rect = GetComponent<RectTransform>();
        _group = GetComponent<CanvasGroup>();
    }

    public void Enable()
    {
        Enabled = true;
    }

    public void SetTooltip(string title, string text)
    {
        _group.alpha = 1;
        _title.text = title;
        _desription.text = text;
    }
    public void HideTooltip()
    {
        Enabled=false;
        _group.alpha = 0;
    }
}
