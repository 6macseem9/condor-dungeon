using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBlock : MonoBehaviour
{
    [SerializeField] private Animator _portrait;
    [SerializeField] private Image _cover;
    [SerializeField] private Image _abilityImage;
    [SerializeField] private Image _healthbarImage;
    [SerializeField] private TextMeshProUGUI _healthbarText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _classText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _upgradeCost;
    private Tooltip _abilityTooltip;
    [SerializeField] private Transform _statsParent;
    [SerializeField] private Transform _bonusStatsParent;

    private Unit _unit;
    private TextMeshProUGUI[] _statTexts;
    private TextMeshProUGUI[] _bonusStatTexts;
    private Button[] _buttons;

    void Start()
    {
        _statTexts = _statsParent.GetComponentsInChildren<TextMeshProUGUI>();
        _bonusStatTexts = _bonusStatsParent.GetComponentsInChildren<TextMeshProUGUI>();

        _abilityTooltip = _abilityImage.GetComponent<Tooltip>();

        _buttons = GetComponentsInChildren<Button>();
        foreach ( var button in _buttons )
        {
            if (button.CompareTag("Ignore")) continue;
            button.AddPressAnimation();
        }

        Unselect("NO UNIT SELECTED");
    }

    void Update()
    {
        if (_unit != null && _unit.Healthbar != null)
        {
            _healthbarImage.rectTransform.DOScaleX(_unit.Healthbar.Percent / 100 *3, 0);
            _healthbarText.text = _unit.HP + "/" + _unit.Stats.MaxHP;
        }

    }
    public void SetStats(List<Unit> units)
    {
        if (units == null)
        {
            Unselect("NO UNIT SELECTED");
            return;
        }

        if (units.Count == 1) Select(units[0]);
        else Unselect("GROUP SELECTED");
    }

    private void Select(Unit unit)
    {
        _unit = unit;
        _cover.enabled = false;

        var abil = _unit.AbilityController.Ability;
        _abilityImage.sprite = abil.Icon;
        _abilityTooltip.SetInfo(abil.Name, abil.Description);

        _portrait.ReplaceClip(unit.Class.Turnaround);

        _levelText.text = unit.Level.ToString();
        _upgradeCost.text = "#" + unit.UpgradeCost;

        _nameText.text = unit.gameObject.name.ToUpper();
        _classText.text = unit.Class.ClassName;
        _classText.color = unit.Class.ClassColor;


        DisplayStats(_statTexts, unit.Class.Stats,true);
        DisplayStats(_bonusStatTexts, unit.BonusStats, false) ;

        if(_unit.IsEnemy) foreach (var button in _buttons) button.interactable = false;
    }

    private void Unselect(string message)
    {
        _unit = null;
        _cover.enabled = true;

        _nameText.text = "<color=#a3c0e6>" + message;
        _classText.text = "";
    }

    private void DisplayStats(TextMeshProUGUI[] texts, Stats stats, bool icons)
    {
        var value = stats.GetArray();

        for (int i = 0; i < value.Count; i++)
        {
            if (i == 1)
            {
                var val = icons? _unit.GetAttackPerSecond() : _unit.GetAttackPerSecond() * value[i];
                texts[i].text = (icons ? texts[i].text.Substring(0, 2) : "") 
                    + (val==0? val :  val.ToString("0.00"));
                continue;
            }

            if(icons)
            {
                _buttons[i].interactable = value[i] != 0;
            }
            texts[i].text = (icons? texts[i].text.Substring(0, 2) : "") + value[i];
        }
    }

    public void UpgradeStat(int index)
    {
        if (UnitSelectionManager.Instance.RemoveGold(_unit.UpgradeCost))
        {
            _upgradeCost.rectTransform.DOComplete();
            _upgradeCost.rectTransform.DOShakeAnchorPos(0.3f, 4, 20);

            _unit.UpgradeStat(index);
            Select(_unit);
        }
    }

    //private Tweener _hideAfterSell;
    //public void SellSelected()
    //{
    //    UnitSelectionManager.Instance.AddGold((int)(_unit.Stats.Cost * 0.75f));
    //    _unit.Kill();
    //    _hideAfterSell= Util.Delay(0.5f, () => ShowGroup(_singleGroup, false));
    //}

    //public void HealSelected()
    //{
    //    if (_unit.HP == _unit.Stats.MaxHP) return;
    //    if (!UnitSelectionManager.Instance.RemoveGold(_healCost)) return;
        
    //    _unit.Heal(9999);
    //}
}
