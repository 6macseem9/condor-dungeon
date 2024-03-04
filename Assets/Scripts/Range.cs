using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class Range : MonoBehaviour
{
    public string tagToCheck;

    public delegate void RangeEvent(Unit unit);
    public event RangeEvent OnEnter;
    public event RangeEvent OnExit;

    private Collider _collider;

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(tagToCheck)) return;
        OnEnter?.Invoke(other.GetComponent<Unit>());
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag(tagToCheck)) return;

        OnExit?.Invoke(other.GetComponent<Unit>());
    }

    public void ReTrigger()
    {
        _collider.enabled = false;
        _collider.enabled = true;
    }
}
