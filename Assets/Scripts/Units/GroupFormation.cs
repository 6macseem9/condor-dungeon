using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum Formation { Square, ThickColumn,ThickRow,Column,Row }

public class GroupFormation : MonoBehaviour
{
    public Formation CurrentFormation { get; private set; }

    private Button[] _buttons;

    private void Start()
    {
        _buttons = GetComponentsInChildren<Button>();
        foreach (Button button in _buttons) { button.AddPressAnimation(); }
    }

    public void SetFormation(int index)
    {
        CurrentFormation = (Formation)index;
        foreach (var button in _buttons)
        {
            button.interactable = true;
        }
    }
}
