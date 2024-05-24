using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [SerializeField] private Vector2Int _goldMinMax;
    [SerializeField] private bool _gold;
    [SerializeField] private bool _keys;
    [Space(7)]
    [SerializeField] bool _clearRoom;
    [SerializeField] private GameObject _objectToDisable;

    private bool _looted;

    private void OnMouseDown()
    {
        if (_looted) return;
        if (_gold && _keys) { Debug.LogWarning("GOLD AND KEYS IN ONE LOOT"); return; }

        _objectToDisable.SetActive(false);
        _looted = true;

        transform.DOScale(1.5f, 0);
        transform.DOScale(1, 0.3f).SetEase(Ease.OutCirc);
        //transform.DOShakeRotation(0.5f,20,15);

        var gold = Random.Range(_goldMinMax.x, _goldMinMax.y);

        BattleIntroAndResults.Instance.PopOutReward(_gold ? gold:0, _keys?1:0);
        if(_clearRoom) MapController.Instance.RemoveCurrentRoomIcon();
    }
}
