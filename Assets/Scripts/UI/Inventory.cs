using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class ItemChance
{
    [HideInInspector] public string Name;
    public Item Item;
    public int Probability;
}


public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [SerializeField] public List<ItemChance> PossibleItems;

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
    private void OnValidate()
    {
        foreach (var item in PossibleItems)
        {
            item.Name = item.Item is null ? "Item" : $"{item.Item.Name}  -  {item.Probability}%";
        }
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

    public void ClearInventory()
    {
        _pool.List.ForEach(x => { if (x.gameObject.activeSelf) x.Discard(); });
    }
}
