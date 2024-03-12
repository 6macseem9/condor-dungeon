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

    public float Radius { get { return _collider == null ? 0 : _collider.radius; } }

    private SphereCollider _collider;

    private void Start()
    {
        _collider = GetComponent<SphereCollider>();
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
