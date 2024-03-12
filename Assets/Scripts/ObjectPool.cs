using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T>
{
    public List<T> List { get; private set; }
    private Func<T, bool> _predicate;
    private Func<T> _create;
    private Action<T> _enable;
    private Action<T> _disable;

    public ObjectPool(Func<T, bool> predicate, Func<T> create, Action<T> enable, Action<T> disable)
    {
        List = new List<T>();
        _predicate = predicate;
        _create = create;
        _enable = enable;
        _disable = disable;
    }

    public void AddDefault(T[] objs)
    {
        List.AddRange(objs);
    }

    public T GetObject()
    {
        foreach (T obj in List)
        {
            if (_predicate(obj))
            {
                _enable(obj);
                return obj;
            }
        }
        
        var newObj = _create();
        List.Add(newObj);
        return newObj;
    }

    public void DisableAll()
    {
        foreach (T obj in List)
        {
            _disable(obj);
        }
    }
}
