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
    public int Count { get; private set; }

    private void Awake()
    {
        _texts = GetComponentsInChildren<TextMeshProUGUI>();
        Count = 1;

        _button = GetComponentInChildren<Button>();
        _button.AddPressAnimation();
    }
    public void SetInfo(string name, Color color)
    {
        ClassName = name;
        _texts[0].text = FormatText(name);
        _texts[0].color = color;
        ResetCount();
    }
    public void Increment()
    {
        Count++;
        _texts[1].text = FormatText("x" + Count);
    }
    public void ResetCount()
    {
        Count = 1;
        _texts[1].text = FormatText("x"+Count);
    }

    private string FormatText(string text)
    {
        return $"<font=\"6pxBG SDF\"><mark=#36354d>{text}</mark></font>";
    }
    public void  SelectClass()
    {
        UnitSelectionManager.Instance.SelectClass(ClassName);
    }
}
