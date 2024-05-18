using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitListItem : MonoBehaviour
{
    private TextMeshProUGUI[] _texts;
    private Button _button;

    public string ClassName { get; private set; }
    public Color ClassColor { get { return _texts[0].color; } }

    private Unit _unit;
    private UnitSelectionManager _selectionManager;

    private void Awake()
    {
        _texts = GetComponentsInChildren<TextMeshProUGUI>();

        _button = GetComponentInChildren<Button>();
        _button.AddPressAnimation();
    }
    private void Start()
    {
        _selectionManager = UnitSelectionManager.Instance;
    }
    public void SetInfo(Unit unit)
    {
        _unit = unit;
        _unit.OnLevelUp += UpdateLevel;
        _unit.OnSelect += ShowSelected;

        ClassName = unit.Class.ClassName;
        _texts[0].text = FormatText(ClassName);
        _texts[0].color = unit.Class.ClassColor;
        _texts[1].text = FormatText(_unit.Level.ToString());
    }

    private string FormatText(string text)
    {
        return $"<font=\"6pxBG SDF\"><mark=#36354d>{text}</mark></font>";
    }
    public void  SelectClass()
    {
        if (_unit is null) return;

        if(Input.GetKey(KeyCode.LeftControl))
        {
            _selectionManager.SelectClass(ClassName);
            return;
        }

        if(!Input.GetKey(KeyCode.LeftShift))_selectionManager.DeselectAll();
        _selectionManager.Select(_unit);
    }

    private void ShowSelected(bool show)
    {
        _texts[0].color = show? new Color(0.172549f,1,0) :_unit.Class.ClassColor;
    }

    private void UpdateLevel(int level)
    {
        _texts[1].text = FormatText(level.ToString());
    }
}
