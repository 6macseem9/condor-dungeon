using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapCell : MonoBehaviour, IPointerClickHandler
{
    public List<MapTile> Options { get; set; } = new List<MapTile>();
    public (int,int) Position { get; set; }
    public MapTile Tile { get; set; }
    public Room Room { get; set; }

    private int _closedPassages;
    private int _connectedThreshold = 1;
    public bool IsCollapsed { get; private set; }
    public bool IsConnected { get { return _closedPassages >= _connectedThreshold; } }
    public int Entropy { get { return Options.Count; } }
    public bool IsValid { get { return Entropy > 0 && IsConnected; } }
    public bool IsClosed { get { return Tile is null ? false : _closedPassages >= Tile.Passages.Count; } }

    private Image _tileImage;
    private Image _iconImage;
    private Sprite _emptyTileSprite;

    void Start()
    {
        var images = GetComponentsInChildren<Image>();
        _tileImage = images[0];
        _iconImage = images[1];

        _emptyTileSprite = _tileImage.sprite;
    }

    public void SetTile(MapTile tile)
    {
        Tile = tile;

        _tileImage.sprite = tile.Sprite;
        _tileImage.transform.DORotate(new Vector3(0, 0, -tile.Angle),0);
        IsCollapsed = true;
        Options.Clear();
    }

    public void UpdateOptions(int direction,bool passage)
    {
        if(passage) _closedPassages++;
        if (IsCollapsed) return;

        int neededDirection = (direction + 2) % 4;

        var list = new List<MapTile>();
        foreach (MapTile tile in Options) 
        { 
            if (passage && tile.Passages.Contains(neededDirection)) list.Add(tile);
            if (!passage && !tile.Passages.Contains(neededDirection)) list.Add(tile);
        }

        Options = list;
    }

    public void Collapse()
    {
        int totalWeight = Options.Sum(option => option.Probability);
        int randomNumber = Random.Range(1, totalWeight + 1); 

        int cumulativeWeight = 0;
        foreach (var option in Options)
        {
            cumulativeWeight += option.Probability;
            if (randomNumber <= cumulativeWeight)
            {
                SetTile(option);
                break;
            }
        }
    }
    public void ResetCell(List<MapTile> options)
    {
        if (Room != null)
        {
            Room.DestroyObject();
            SetRoom(null);
        }

        _tileImage.sprite = _emptyTileSprite;
        _iconImage.sprite = _emptyTileSprite;
        _tileImage.transform.rotation = Quaternion.identity;
        IsCollapsed = false;
        _closedPassages = 0;
        Options = options;
        Tile = null;
    }

    public void ResetIcon()
    {
        _iconImage.sprite = _emptyTileSprite;
    }

    public void MarkAsStartingCell()
    {
        _connectedThreshold = 0;
    }

    public bool HasPassageFor(int neighborPassage)
    {
        if (Tile is null) return false;

        int passage = (neighborPassage + 2) % 4;
        return Tile.Passages.Contains(passage);
    }

    public void SetRoom(Room room)
    {
        _iconImage.sprite = room is null ? _emptyTileSprite : room.Icon;
        if (room is null) { Room = null; return; }

        Room = room.InitiateRoom();
        if(Room.RoomObject != null) Room.RoomObject.name = Position.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MapController.Instance.MovePlayerToCell(this);
    }

    public bool HasPassageToCell(MapCell cell)
    {
        var neighbours = MapController.Instance.GetNeighbors(Position);
        if (!neighbours.Contains(cell)) return false;

        return cell.HasPassageFor(neighbours.IndexOf(cell));
    }

    public bool IsCellNear(MapCell cell)
    {
        var x = Mathf.Abs(Position.Item1 - cell.Position.Item1);
        var y = Mathf.Abs(Position.Item2 - cell.Position.Item2);

        return x + y == 1;
    }
}
