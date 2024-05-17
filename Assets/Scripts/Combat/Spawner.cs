using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Vector3 SpawnPoint { get {  return transform.position - new Vector3(0,0.7f,0.5f); } }

    private SpriteRenderer _arrow;
    private SpriteRenderer _icon;

    void Start()
    {
        _icon = GetComponentsInChildren<SpriteRenderer>()[0];
        _arrow = GetComponentsInChildren<SpriteRenderer>()[1];

        _arrow.transform.DOLocalMoveZ(-0.9f,0.3f).SetLoops(-1,LoopType.Yoyo);
    }

    
    public void ShowIcon(bool show)
    {
        _icon.DOFade(show ? 1 : 0, 0.2f);
        _arrow.DOFade(show ? 1 : 0.5f, 0.2f);
    }
}
