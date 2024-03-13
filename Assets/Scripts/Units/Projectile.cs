using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 _startPosition;
    
    private Transform _target;
    private Action _onHit;

    private void FixedUpdate()
    {
        var destination = new Vector3(_target.position.x, transform.position.y, _target.position.z);
        transform.position = Vector3.MoveTowards(transform.position, destination, _speed);

        if (Vector3.Distance(transform.position, destination) < 0.3f)
        {
            gameObject.SetActive(false);
            _onHit?.Invoke();
            return;
        }

        Vector3 lookAtPos = destination - transform.position;
        Quaternion newRotation = Quaternion.LookRotation(lookAtPos, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 0.1f);
    }

    public void SetTarget(Transform target, Action onHit)
    {
        transform.localPosition = _startPosition;
        gameObject.SetActive(true);

        _target = target;
        _onHit = onHit;
    }
}
