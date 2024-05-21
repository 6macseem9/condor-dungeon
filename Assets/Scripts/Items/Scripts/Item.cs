using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 2)]
public class Item : ScriptableObject
{
    public string Name;
    public Color Color = Color.white;
    [TextArea] public string Description;
    public bool CombatOnly;
    public bool NoActivation;

    public virtual void Initiate()
    {

    }
    public virtual void Activate()
    {

    }
    public virtual void OnDiscard()
    {

    }
}
