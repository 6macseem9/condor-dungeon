using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StatBlockListItem : MonoBehaviour
{
    private TextMeshProUGUI[] _texts;

    public string ClassName { get { return _texts[0].text; } }
    public Color ClassColor { get { return _texts[0].color; } }
    public int Count { get; private set; }

    private void Awake()
    {
        _texts = GetComponentsInChildren<TextMeshProUGUI>();
        Count = 1;
    }
    public void SetInfo(string name, Color color)
    {
        _texts[0].text = name;
        _texts[0].color = color;
        ResetCount();
    }
    public void Increment()
    {
        Count++;
        _texts[1].text = "x" + Count;
    }
    public void ResetCount()
    {
        Count = 1;
        _texts[1].text = "x" + Count;
    }
    public void  SelectClass()
    {
        UnitSelectionManager.Instance.SelectClass(ClassName);
    }
}
