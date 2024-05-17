using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitAnimator : MonoBehaviour
{
    private Unit _unit;

    private void Start()
    {
        _unit = GetComponentInParent<Unit>();
    }

    public void DealDamage()
    {
        _unit.DealDamageToTarget();
    }
}
