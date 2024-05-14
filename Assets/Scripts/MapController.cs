using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class MapController : MonoBehaviour
{
    public static MapController Instance;

    [SerializeField] private GameObject _raycastBlock;
    [SerializeField] private RectTransform _cover;
    [SerializeField] private Transform _player;
    [SerializeField] private Image _cantMove;
    private MapCell _currentPlayerCell;
    private List<Image> _playerDirections = new List<Image>();

    [Space(5)]
    [SerializeField] private int RoomLimit;
    [Space(5)]
    [SerializeField] private MapTile _startingTile;
    [Space(5)]
    [SerializeField] private List<MapTile> _tiles;
    [Space(5)]
    [SerializeField] private List<Room> _rooms;

    private Dictionary<(int, int), MapCell> _allCells = new Dictionary<(int, int), MapCell>();

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
        _playerDirections = _player.GetComponentsInChildren<Image>().ToList();
        _playerDirections.RemoveAt(0);

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
        Util.Delay(0.1f,()=>GenerateMap());
    }

    private void SetupMap()
    {
        var startCell = _allCells[(3, 3)];
        startCell.Options = new List<MapTile>() { _startingTile };
        startCell.MarkAsStartingCell();

        _currentPlayerCell = startCell;

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

    public void GenerateMap()
    {
        for (int j = 0; j < RoomLimit+1; j++)
        {
            var cell = GetLowestEntropyCell();
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
        }

        CloseOpenPassages();
    }

    public List<MapCell> GetNeighbors((int, int) pos)
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

        int rand = Random.Range(0, minCells.Count);
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
        foreach (var cell in _allCells.Values)
        {
            cell.ResetCell(options: _tiles);
        }

        SetupMap();
    }

    private void CloseOpenPassages()
    {
        var openCells = _allCells.Values.Where((x) => x.IsCollapsed && !x.IsClosed);

        foreach (var cell in openCells)
        {
            var neighbors = GetNeighbors(cell.Position);

            List<int> neededPassages = new List<int>();
            for (int i = 0; i < neighbors.Count; i++)
            {
                if (neighbors[i] is not null && neighbors[i].HasPassageFor(i)) neededPassages.Add(i);
            }

            var tile = _tiles.Find((x) =>
                x.Passages.SequenceEqual(neededPassages)
            );

            cell.SetTile(tile);
        }

        FillRooms();
        UpdatePlayerDirections(withReset: true);
    }

    private void FillRooms()
    {
        var cells = _allCells.Values.Where((x) => x.IsCollapsed && x.Position != (3, 3)).OrderBy(x => Random.value).ToList();

        for (int i = 0; i < cells.Count; i++)
        {
            if (i < cells.Count / 2)
            {
                if (i < 2)
                    cells[i].SetRoom(_rooms[1]/*Cages*/);
                else
                    cells[i].SetRoom(_rooms[Random.Range(2, 5)]);
            }
            else
                cells[i].SetRoom(_rooms[0]/*Battle*/);
        }
    }

    public void MovePlayerToCell(MapCell cell)
    {
        if (!cell.IsCollapsed) return;
        if (!_currentPlayerCell.HasPassageToCell(cell)) return;

        _raycastBlock.SetActive(true);

        _playerDirections.ForEach(x => x.DOFade(0, 0.1f));

        Util.Delay(0.3f, () =>
        {
            if (_currentPlayerCell.Room is not null) _currentPlayerCell.Room.Exit();
            _currentPlayerCell = cell;
            if (_currentPlayerCell.Room is not null) _currentPlayerCell.Room.Enter();
        });

        _cover.DOAnchorPos(new Vector2(0, -1484), 0.9f).onComplete = ()=> _cover.DOAnchorPos(new Vector2(0, 1484), 0);
        _player.DOMove(cell.transform.position, 1).onComplete = () =>
        {
            _raycastBlock.SetActive(false);
            UpdatePlayerDirections();
        };
    }

    public void SetCanMove(bool canMove)
    {
        if(!canMove)
        {
            _cantMove.enabled = true;
            _cantMove.DOFade(0, 0);
            _cantMove.DOFade(1, 0.7f).SetEase(Ease.Flash,15,1);
        }
        else
        {
            _cantMove.DOFade(0, 0.7f).SetEase(Ease.Flash, 15, 1)
                .onComplete = ()=> _cantMove.enabled = false;
        }
    }

    public void ClearCurrentRoom()
    {
        _currentPlayerCell.SetRoom(null);
    }

    private void UpdatePlayerDirections(bool withReset = false)
    {
        if(withReset) _playerDirections.ForEach(x => x.DOFade(0, 0f));

        foreach (var pass in _currentPlayerCell.Tile.Passages)
        {
            _playerDirections[pass].DOFade(1, 0.2f);
        }
    }
}
