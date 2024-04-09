using DG.Tweening;
using System;
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
    public event RangeEvent NoOneDetected;
    public Action OnRetrigger;

    public float Radius { get { return _collider == null ? 0 : _collider.radius; } }

    private SphereCollider _collider;
    private Tweener _timer;

    private void Start()
    {
        _collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(tagToCheck)) return;

        _timer.Kill();
        OnEnter?.Invoke(other.GetComponent<Unit>());
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag(tagToCheck)) return;

        OnExit?.Invoke(other.GetComponent<Unit>());
    }

    public void ReTrigger()
    {
        OnRetrigger?.Invoke();
        _timer.Kill();
        _timer = Util.Delay(0.1f, () => NoOneDetected?.Invoke(null));

        _collider.enabled = false;
        _collider.enabled = true;
    }
}
