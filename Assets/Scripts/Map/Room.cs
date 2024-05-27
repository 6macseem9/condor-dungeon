using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Battle, Cages, Chest, Gold, Key, Empty }

[Serializable]
public class Room
{
    [HideInInspector] public string Name = "Room";
    public Sprite Icon;
    public RoomType Type;

    [Space(5)]
    public GameObject Prefab;

    public GameObject RoomObject { get; set; }

    public Room InitiateRoom()
    {
        Room room = new Room();
        room.Icon = Icon;
        room.Type = Type;
        room.RoomObject = null;

        if (Prefab == null) return room;
        room.RoomObject = GameObject.Instantiate(Prefab);
        room.RoomObject.SetActive(false);

        return room;
    }

    public void Enter()
    {
        if(Type == RoomType.Battle)
        {
            MapController.Instance.SetCanMove(false);
            BattleController.Instance.InitializeBattle();
        }
        if(RoomObject != null) RoomObject.SetActive(true);
    }

    public void Exit()
    {
        if (RoomObject != null) RoomObject.SetActive(false);
    }

    public void DestroyObject()
    {
        if (RoomObject == null) return;   
        GameObject.Destroy( RoomObject );
    }
}
