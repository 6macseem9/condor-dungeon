using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    public Ability Ability { get; private set; }

    void Start()
    {
        Ability = GetComponentInChildren<Ability>();
    }

    void Update()
    {
        
    }
}
