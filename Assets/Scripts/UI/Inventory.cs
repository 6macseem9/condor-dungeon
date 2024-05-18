using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private ObjectPool<ItemUI> _pool;

    public bool HasSpace { get {  return _pool.List.Exists(x => x.gameObject.activeSelf == false); } }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else Instance = this;
    }
    private void Start()
    {
        _pool = new ObjectPool<ItemUI>(
            (x) => !x.gameObject.activeSelf,
            () => new ItemUI(),
            (x) => x.gameObject.SetActive(true),
            (x) => x.gameObject.SetActive(false)
        );

        var items = GetComponentsInChildren<ItemUI>();
        foreach (var item in items) item.gameObject.SetActive(false);
        _pool.AddDefault(items);
    }

    public void AddItem(Item item)
    {
        if (HasSpace)
        {
            var obj = _pool.GetObject();
            obj.SetItem(item);
        }
        else
        {
            CursorController.Instance.NotEnoughResource(NotEnough.Inventory);
        }
    }
}
