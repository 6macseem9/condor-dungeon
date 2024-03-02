using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIDebug : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text; 
    private List<DebugText> _debugs = new List<DebugText>();

    public static UIDebug Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    ///<summary>
    ///The following color names are supported: black, blue, green, orange, purple, red, white, and yellow.
    ///</summary>
    public void Show(string label, string value, string valueColor = "white", string labelColor="white")
    {
        DebugText debug = _debugs.Find(x => x.Label == label);

        if (debug==null) 
        {
            var text = Instantiate(_text,transform);
            var newDebug = new DebugText(label, labelColor, valueColor, text);
            newDebug.Update(value);
            _debugs.Add(newDebug);
            return;
        }

        debug.Update(value);
    }

    private class DebugText
    {
        public string Label;
        public string LabelColor;
        public string ValueColor;
        public TextMeshProUGUI Text;

        public DebugText(string label, string labelColor, string valueColor, TextMeshProUGUI text)
        {
            Label = label;
            LabelColor = labelColor;
            ValueColor = valueColor;
            Text = text;
        }

        public void Update(string value)
        {
            Text.text = $"<color={LabelColor}>{Label}</color> <color={ValueColor}>{value}</color>";
        }
    }
}
