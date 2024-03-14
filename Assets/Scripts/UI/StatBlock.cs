using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

public class StatBlock : MonoBehaviour
{
    [Header("Single select")]
    [SerializeField] private CanvasGroup _singleGroup;
    [SerializeField] private Image _portrait;
    [SerializeField] private Sprite _multiPortrait;
    [SerializeField] private Image _mode;
    [SerializeField] private Sprite _holdMode;
    private Sprite _chaseMode;
    [SerializeField] private Image _healthbarImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _classText;
    [SerializeField] private Transform _statsParent;
    [Space(10)]

    [Header("Mutiselect")]
    [SerializeField] private CanvasGroup _multiGroup;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Transform _multiParent;
    [SerializeField] private StatBlockListItem _listItemPrefab;

    private Unit _unit;
    private TextMeshProUGUI[] _statTexts;
    private ObjectPool<StatBlockListItem> _pool;

    void Start()
    {
        _statTexts = _statsParent.GetComponentsInChildren<TextMeshProUGUI>();
        _chaseMode = _mode.sprite;

        _pool = new ObjectPool<StatBlockListItem>(
            (x) => !x.gameObject.activeSelf,
            () => Instantiate(_listItemPrefab, _multiParent),
            (x) => x.gameObject.SetActive(true),
            (x) => x.gameObject.SetActive(false)
        );

        var items = _multiParent.GetComponentsInChildren<StatBlockListItem>();
        foreach ( var item in items ) { item.gameObject.SetActive(false); }
        _pool.AddDefault(items);

        Show(false, false);
    }

    void Update()
    {
        if (_singleGroup.alpha > 0 && _unit != null && _unit.Healthbar != null)
            _healthbarImage.rectTransform.DOScaleX(_unit.Healthbar.Percent / 100, 0);
    }
    public void ToggleMode()
    {
        _mode.sprite = _unit.ToggleMode() ? _holdMode : _chaseMode;
    }
    public void SetStats(List<Unit> units)
    {
        if (units == null)
        {
            Show(false, false);
            return;
        }

        if (units.Count == 1) SingleSelect(units[0]);
        else MultiSelect(units);
    }
    private void Show(bool showSingle, bool showMulti)
    {
        _singleGroup.alpha = showSingle ? 1 : 0;
        _singleGroup.blocksRaycasts = showSingle;
        _singleGroup.interactable = showSingle;

        _multiGroup.alpha = showMulti ? 1 : 0;
        _multiGroup.blocksRaycasts = showMulti;
        _multiGroup.interactable = showMulti;
    }

    private void SingleSelect(Unit unit)
    {
        Show(true, false);

        _unit = unit;
        _mode.sprite = _unit.HoldPosition ? _holdMode : _chaseMode;
        _portrait.sprite = unit.Stats.Portrait;

        _nameText.text = unit.gameObject.name.ToUpper();
        _classText.text = unit.Stats.ClassName;
        _classText.color = unit.Stats.ClassColor;

        var value = unit.Stats.GetArray();

        for (int i = 0; i < value.Length; i++)
        {
            _statTexts[i].text = _statTexts[i].text.Substring(0, 2) + (value[i] == 0 ? "-" : value[i]);
        }
    }

    private void MultiSelect(List<Unit> units)
    {
        Show(false, true);
        _countText.text = "x" + units.Count + " units";
        _unit = null;

        _pool.DisableAll();

        foreach (Unit unit in units)
        {
            if (_pool.List.Exists((x) => x.ClassName == unit.Stats.ClassName))
            {
                var text = _pool.List.Find((x) => x.ClassName == unit.Stats.ClassName);

                if (text.gameObject.activeSelf) 
                {
                    text.Increment();
                    continue;
                }
            }

            var item = _pool.GetObject();
            item.SetInfo(unit.Stats.ClassName, unit.Stats.ClassColor);
        }
    }
}
