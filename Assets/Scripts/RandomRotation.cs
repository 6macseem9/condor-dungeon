using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    [SerializeField] private Vector3 _min;
    [SerializeField] private Vector3 _max;

    void Start()
    {
        Vector3 rotation = new Vector3(
            Random.Range(_min.x,_max.x),
            Random.Range(_min.y, _max.y),
            Random.Range(_min.z, _max.z)
            );

        transform.DORotate(rotation, 0);
    }
}
