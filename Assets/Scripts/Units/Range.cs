using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class Range : MonoBehaviour
{
    [SerializeField] private string _tagToCheck;

    public delegate void RangeEvent(Unit unit);
    public event RangeEvent OnEnter;
    public event RangeEvent OnExit;
    public Action OnRetrigger;

    public float Radius { get { return _collider == null ? 0 : _collider.radius; } }

    private SphereCollider _collider;

    private void Start()
    {
        _collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag(_tagToCheck)) return;

        OnEnter?.Invoke(other.GetComponent<Unit>());
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag(_tagToCheck)) return;

        OnExit?.Invoke(other.GetComponent<Unit>());
    }

    public void ReTrigger()
    {
        OnRetrigger?.Invoke();

        _collider.enabled = false;
        Util.Delay(0.01f,()=> _collider.enabled = true);
    }
}
