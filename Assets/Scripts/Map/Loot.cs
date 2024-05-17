using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [SerializeField] private bool _gold;
    [SerializeField] private bool _keys;

    private bool _looted;

    private void OnMouseDown()
    {
        if (_looted) return;
        if (_gold && _keys) { Debug.LogWarning("GOLD AND KEYS IN ONE LOOT"); return; }

            _looted = true;
        transform.DOShakeRotation(0.5f,20,15);

        BattleIntroAndRewards.Instance.PopOutReward(_gold ? 100:0, _keys?1:0);
        MapController.Instance.RemoveCurrentRoomIcon();
    }
}
