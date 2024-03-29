using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBlock : MonoBehaviour
{
    [SerializeField] private int _healCost = 50;
    [Space(10)]
    [Header("Single select")]
    [SerializeField] private CanvasGroup _singleGroup;
    [SerializeField] private Animator _portrait;
    [SerializeField] private Sprite _multiPortrait;
    [SerializeField] private Image _mode;
    [SerializeField] private Sprite _holdMode;
    private Sprite _chaseMode;
    [SerializeField] private Image _healthbarImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _classText;
    [SerializeField] private Image _abilityImage;
    private Tooltip _abilityTooltip;
    [SerializeField] private Transform _statsParent;
    [SerializeField] private Tooltip _sellTooltip;
    [SerializeField] private Button _healButton;
    [Space(10)]
    
    [Header("Mutiselect")]
    [SerializeField] private CanvasGroup _multiGroup;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Transform _multiParent;
    [SerializeField] private StatBlockListItem _listItemPrefab;

    private Unit _unit;
    private TextMeshProUGUI[] _statTexts;
    private ObjectPool<StatBlockListItem> _pool;

    private Tweener _modeShake;

    void Start()
    {
        _statTexts = _statsParent.GetComponentsInChildren<TextMeshProUGUI>();
        _chaseMode = _mode.sprite;
        _abilityTooltip = _abilityImage.GetComponent<Tooltip>();

        var buttons = GetComponentsInChildren<Button>();
        foreach ( var button in buttons )
        {
            if (button.CompareTag("Ignore")) continue;
            button.onClick.AddListener(()=>ButtonPress(button.transform));
        }

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
        {
            _healthbarImage.rectTransform.DOScaleX(_unit.Healthbar.Percent / 100, 0);
            _healButton.interactable = _unit.HP != _unit.Stats.MaxHP && !_unit.IsDying;
        }

    }
    private void ButtonPress(Transform button)
    {
        button.DOKill();
        button.DOScale(0.8f, 0);
        button.DOScale(1f, 0.2f);
    }
    public void ToggleMode()
    {
        _mode.sprite = _unit.ToggleMode() ? _holdMode : _chaseMode;
        _modeShake.Complete();
        _modeShake = _mode.rectTransform.DOShakeAnchorPos(0.3f,1,30);
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
        _hideAfterSell.Kill();

        ShowGroup(_singleGroup, showSingle);
        ShowGroup(_multiGroup, showMulti);
    }

    private void ShowGroup(CanvasGroup group,bool show)
    {
        group.transform.SetAsLastSibling();

        group.blocksRaycasts = show;
        group.interactable = show;

        if (show)
        {
            group.alpha = 1;

            if (group==_singleGroup) group.transform.DOScale(0, 0f);
            else group.transform.DOScale(2f, 0f);

            group.transform.DOScale(3, 0.3f).SetEase(Ease.OutElastic, 1);
        }
        if(!show && group.transform.localScale != Vector3.zero)
        {
            group.transform.DOScale(0, 0f);
            group.alpha = 0;
        }
        
    }

    private void SingleSelect(Unit unit)
    {
        Show(true, false);

        _unit = unit;
        _mode.gameObject.SetActive(!unit.IsEnemy());
        _mode.sprite = _unit.HoldPosition ? _holdMode : _chaseMode;

        var abil = _unit.AbilityController.Ability;
        _abilityImage.sprite = abil.Icon;
        _abilityTooltip.SetInfo(abil.Name, abil.Description);

        _portrait.ReplaceClip(unit.Stats.Turnaround);
        _sellTooltip.Description = _sellTooltip.Description.Replace("cost", $"{(int)(_unit.Stats.Cost * 0.75f)}");

        _nameText.text = unit.gameObject.name.ToUpper();
        _classText.text = unit.Stats.ClassName;
        _classText.color = unit.Stats.ClassColor;

        var value = unit.Stats.GetArray();

        for (int i = 0; i < value.Length; i++)
        {
            if(i==2)
            {
                var val = unit.GetAttackPerSecond();
                _statTexts[i].text = _statTexts[i].text.Substring(0, 2) + (val == 0 ? "-" : val.ToString("0.00"));
                continue;
            }
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

    private Tweener _hideAfterSell;
    public void SellSelected()
    {
        UnitSelectionManager.Instance.AddGold((int)(_unit.Stats.Cost * 0.75f));
        _unit.Kill();
        _hideAfterSell= Util.Delay(0.5f, () => ShowGroup(_singleGroup, false));
    }

    public void HealSelected()
    {
        if (_unit.HP == _unit.Stats.MaxHP) return;
        if (!UnitSelectionManager.Instance.RemoveGold(_healCost)) return;
        
        _unit.Heal(9999);
    }
}
