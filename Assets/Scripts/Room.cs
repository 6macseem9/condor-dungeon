using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Battle, Cages, Chest, Gold, Key, Empty }

[Serializable]
public class Room
{
    public Sprite Icon;
    public RoomType Type;

    [Space(5)]
    public GameObject Prefab;

    private GameObject _roomObject;

    public void InitiateRoom(MapCell cell)
    {
        if (Prefab == null) return;
        _roomObject = GameObject.Instantiate(Prefab);
        _roomObject.SetActive(false);
    }

    public void Enter()
    {
        if(Type == RoomType.Battle)
        {
            MapController.Instance.SetCanMove(false);
            BattleController.Instance.InitializeBattle();
        }
        if(_roomObject != null) _roomObject.SetActive(true);
    }

    public void Exit()
    {
        if (_roomObject != null) _roomObject.SetActive(false);
    }
}
