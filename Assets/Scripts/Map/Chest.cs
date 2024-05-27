using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;




public class Chest : MonoBehaviour
{
    [SerializeField] private Transform _cap;

    private bool _looted;


    private void OnMouseDown()
    {
        if (_looted) return;

        transform.DOScale(1.5f, 0);
        transform.DOScale(1, 0.3f).SetEase(Ease.OutCirc);
        
        if (!Inventory.Instance.HasSpace)
        {
            CursorController.Instance.NotEnoughResource(NotEnough.Inventory);
            return;
            
        }

        _looted = true;
        BattleIntroAndResults.Instance.PopOutReward(item: GetRandomItem());
        _cap.DOLocalRotate(new Vector3(120, 0, 0), 0.5f).SetEase(Ease.OutBack);
        MapController.Instance.RemoveCurrentRoomIcon();
    }

    private Item GetRandomItem()
    {
        int totalWeight = Inventory.Instance.PossibleItems.Sum(option => option.Probability);
        int randomNumber = Random.Range(1, totalWeight + 1);

        int cumulativeWeight = 0;
        foreach (var entry in Inventory.Instance.PossibleItems)
        {
            cumulativeWeight += entry.Probability;
            if (randomNumber <= cumulativeWeight)
            {
                return entry.Item;
            }
        }
        return null;
    }
}
