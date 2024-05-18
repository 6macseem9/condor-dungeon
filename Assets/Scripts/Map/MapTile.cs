using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class MapTile
{
    public Sprite Sprite;
    public int Probability = 100;
    public int Angle { get; set; } = 0;
    [Space(10)]

    //      0
    //    3[ ]1
    //      2
    [SerializeField] private bool _north0;
    [SerializeField] private bool _east1;
    [SerializeField] private bool _south2;
    [SerializeField] private bool _west3;

    public List<int> Passages
    {
        get
        {
            var list = new List<int>();
            if (_north0) list.Add(0);
            if (_east1) list.Add(1);
            if (_south2) list.Add(2);
            if (_west3) list.Add(3);
            return list;
        }
    }

    public MapTile CreateRotatedCopy()
    {
        var tile = new MapTile();
        tile.Sprite = Sprite;
        tile.Probability = Probability;

        tile._north0 = _west3;
        tile._east1 = _north0;
        tile._south2 = _east1;
        tile._west3 = _south2;

        tile.Angle = Angle + 90;

        return tile;
    }
}
