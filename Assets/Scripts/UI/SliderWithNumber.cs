using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderWithNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private Slider _slider;

    private void Start()
    {
        _slider = GetComponent<Slider>();
        _text.text = _slider.value.ToString();
    }

    public void UpdateText()
    {
        _text.text = _slider.value.ToString();
    }
}
