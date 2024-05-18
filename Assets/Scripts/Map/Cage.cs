using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Cage : MonoBehaviour
{
    [SerializeField] private Transform _cageObject;
    [SerializeField] private Transform _shadow;

    private Tooltip _tooltip;
    private Unit _unit;
    private Collider _collider;
    private NavMeshObstacle _obstacle;

    void Start()
    {
        _tooltip = GetComponent<Tooltip>();
        _collider = GetComponent<Collider>();   
        _obstacle = GetComponent<NavMeshObstacle>();

        Util.Delay(0.01f, () =>
        {
            var db = UnitSelectionManager.Instance.UnitDB;
            var unit = db[Random.Range(0, db.Count)];

            _unit = Instantiate(unit, transform.position, Quaternion.Euler(0, 180, 0));
            _unit.transform.parent = transform;
            Util.Delay(0.01f, () => 
            {
                _unit.EnableCollider(false);
                _obstacle.carving = true;
            } );

            _tooltip.Title = $"<color=#{unit.Class.ClassColor.ToHexString()}>" + _unit.Class.ClassName.ToUpper();
            _tooltip.Description = _unit.Class.ClassDescription + "\n\n<color=#a3c0e6>} CLICK TO UNLOCK WITH {1";
        });

        
    }

    private void OnMouseDown()
    {
        if(Resources.Instance.RemoveKeys(1))
        {
            Unlock();
        }
    }

    private void Unlock()
    {
        _obstacle.carving = false;
        _collider.enabled = false;
        _cageObject.DOMoveY(5, 2f).SetEase(Ease.OutCirc);
        _shadow.DOScale(1.8f,2f).SetEase(Ease.OutCirc);

        _unit.transform.parent = null;
        Util.Delay(0.05f,()=>_unit.EnableCollider(true));
        UnitSelectionManager.Instance.AddUnit(_unit);
    }
}
