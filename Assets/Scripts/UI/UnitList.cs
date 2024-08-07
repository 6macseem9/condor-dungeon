using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitList : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private UnitListItem _listItemPrefab;

    private ObjectPool<UnitListItem> _pool;
    private void Start()
    {
        UnitSelectionManager.Instance.UnitAddedOrRemoved += UpdateList;

        _pool = new ObjectPool<UnitListItem>(
            (x) => !x.gameObject.activeSelf,
            () => Instantiate(_listItemPrefab, transform),
            (x) => x.gameObject.SetActive(true),
            (x) => x.gameObject.SetActive(false)
        );

        var items = GetComponentsInChildren<UnitListItem>();
        foreach (var item in items) item.gameObject.SetActive(false);
        _pool.AddDefault(items);

        GetComponentInChildren<Button>().AddPressAnimation();
    }

    public void UpdateList(List<Unit> units, Unit addedUnit)
    {
        _countText.text = units.Count + "/" + 25;
        _pool.DisableAll();

        foreach (Unit unit in units)
        {
            //if (_pool.List.Exists((x) => x.ClassName == unit.Class.ClassName))
            //{
            //    var text = _pool.List.Find((x) => x.ClassName == unit.Class.ClassName);

            //    if (text.gameObject.activeSelf)
            //    {
            //        text.Increment();
            //        continue;
            //    }
            //}

            var item = _pool.GetObject();
            item.SetInfo(unit);
        }
    }
}
