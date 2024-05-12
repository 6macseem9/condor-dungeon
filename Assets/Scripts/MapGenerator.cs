using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int RoomLimit;
    [Space(5)]
    [SerializeField] private MapTile _startingTile;
    [Space(5)]
    [SerializeField] private List<MapTile> _tiles;
    [Space(5)]
    [SerializeField] private Sprite _battleSprite;
    [SerializeField] private Sprite _rewardSprite;

    private Dictionary<(int,int),MapCell> _allCells = new Dictionary<(int, int), MapCell>();


    private void Start()
    {
        _tiles.AddRange(GetRotatedTiles());

        var cells = GetComponentsInChildren<MapCell>();
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Options = _tiles;

            int x = i % 8;
            int y = i / 8;
            cells[i].Position = (x, y);
            _allCells.Add((x, y), cells[i]);
        }

        SetupMap();
    }

    private void SetupMap()
    {
        _allCells[(3, 3)].Options = new List<MapTile>() { _startingTile };
        _allCells[(3, 3)].MarkAsStartingCell();

        for (int i = 0; i < 8; i++)
        {
            _allCells[(i, 0)].UpdateOptions(2, false);
            _allCells[(i, 6)].UpdateOptions(0, false);

            if (i < 7)
            {
                _allCells[(0, i)].UpdateOptions(1, false);
                _allCells[(7, i)].UpdateOptions(3, false);
            }
        }
    }

    public async void GenerateMap()
    {
        for (int j = 0; j < RoomLimit; j++)
        {
            var cell =  GetLowestEntropyCell();
            if (cell is null)
            {
                ResetMap();
                GenerateMap();
                return;
            }

            cell.Collapse();

            var neighbours = GetNeighbors(cell.Position);
            for (int i = 0; i < neighbours.Count; i++)
            {
                if (neighbours[i] is null) continue;
                neighbours[i].UpdateOptions(i, cell.Tile.Passages.Contains(i));
            }

            await Task.Delay(200);
        }

        CloseOpenPassages();
    }

    private List<MapCell> GetNeighbors((int,int) pos)
    {
        List<MapCell> cells = new List<MapCell>
        {
            pos.Item2 != 0 ? _allCells[(pos.Item1, pos.Item2 - 1)] : null,
            pos.Item1 != 7 ? _allCells[(pos.Item1 + 1, pos.Item2)] : null,
            pos.Item2 != 6 ? _allCells[(pos.Item1, pos.Item2 + 1)] : null,
            pos.Item1 != 0 ? _allCells[(pos.Item1 - 1, pos.Item2)] : null
        };

        return cells;
    }

    private MapCell GetLowestEntropyCell()
    {
        int min = 99;
        List<MapCell> minCells = new List<MapCell>();

        foreach (var cell in _allCells.Values)
        {
            if (!cell.IsValid) continue;

            if (cell.Entropy < min)
            {
                min = cell.Entropy;

                minCells.Clear();
                minCells.Add(cell);
            }
            else if (cell.Entropy == min)
            {
                minCells.Add(cell);
            }
        }

        int rand = UnityEngine.Random.Range(0, minCells.Count);
        return minCells.Count == 0 ? null : minCells[rand];
    }

    private List<MapTile> GetRotatedTiles()
    {
        List<MapTile> rotatedTiles = new List<MapTile>();
        foreach (var tile in _tiles)
        {
            MapTile current = tile;
            for (int i = 0; i < 3; i++)
            {
                current = current.CreateRotatedCopy();
                rotatedTiles.Add(current);
            }
        }
        return rotatedTiles;
    }

    public void ResetMap()
    {
        foreach(var cell in _allCells.Values)
        {
            cell.ResetCell(options: _tiles);
        }

        SetupMap();
    }

    private async void CloseOpenPassages()
    {
        var openCells = _allCells.Values.Where((x)=>x.IsCollapsed && !x.IsClosed);

        foreach (var cell in openCells)
        {
            var neighbors = GetNeighbors(cell.Position);

            List<int> neededPassages = new List<int>();
            for (int i = 0;i < neighbors.Count;i++)
            {
                if (neighbors[i] is not null && neighbors[i].HasPassageFor(i)) neededPassages.Add(i);
            }

            var tile = _tiles.Find((x) =>
                x.Passages.SequenceEqual(neededPassages)
            );

            cell.SetTile(tile);

            await Task.Delay(200);
        }
        FillRooms();
    }

    private async void FillRooms()
    {
        var cells = _allCells.Values.Where((x)=> x.IsCollapsed && x.Position!=(3,3)).OrderBy(x=>Random.value).ToList();

        for(int i =0; i< cells.Count;i++)
        {
            if (i >= cells.Count/2) 
                cells[i].SetContent(_rewardSprite);
            else
                cells[i].SetContent(_battleSprite);
            await Task.Delay(200);
        }
    }
}
