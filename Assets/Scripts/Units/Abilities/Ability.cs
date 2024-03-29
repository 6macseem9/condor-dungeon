using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public string Name;
    [TextArea]public string Description;
    public Sprite Icon;

    protected Unit _unit;

    protected virtual void Start()
    {
        _unit = GetComponentInParent<Unit>();
    }
    protected virtual void Update()
    {
        
    }
}
