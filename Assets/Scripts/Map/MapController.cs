using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MapController : MonoBehaviour
{
    public static MapController Instance;

    [SerializeField] private int _roomLimitStart;
    [SerializeField] private int _roomLimitIncrease;
    private int _roomLimit;
    [Space(5)]
    [SerializeField] private MapTile _startingTile;
    [Space(5)]
    [SerializeField] private List<MapTile> _tiles;
    [Space(5)]
    [SerializeField] private List<Room> _rooms;

    [Space(10)]

    [SerializeField] private GameObject _raycastBlock;
    [SerializeField] private RectTransform _cover;
    [SerializeField] private Transform _player;
    [SerializeField] private Image _cantMove;
    [Space(5)]
    [SerializeField] private Image _fillableSkull;
    [SerializeField] private Image _fillableCables;
    [SerializeField] private Image _frame;
    [SerializeField] private TextMeshProUGUI _battlesLeft;
    [SerializeField] private TextMeshProUGUI _battlesTotal;
    [SerializeField] private Button _descendButton;
    [SerializeField] private TextMeshProUGUI _floorNumber;

    public int CurrentFloor { get { return int.Parse(_floorNumber.text); } }

    private int _battleCount;
    private MapCell _currentPlayerCell;
    private List<Image> _playerDirections = new List<Image>();

    private Dictionary<(int, int), MapCell> _allCells = new Dictionary<(int, int), MapCell>();

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
        foreach(var tile in _tiles) 
        {
            tile.Name = tile.Sprite is null? "Tile" : $"{tile.Sprite.name}  -  {tile.Probability}%";
        }
        foreach (var room in _rooms)
        {
            room.Name = room.Icon is null ? "Room" : room.Icon.name;
        }
    }

    private void Start()
    {
        _playerDirections = _player.GetComponentsInChildren<Image>().ToList();
        _playerDirections.RemoveAt(0);

        _descendButton.AddPressAnimation();

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

        //SetupMap();
        //Util.Delay(0.1f,()=>GenerateMap());
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
        for (int j = 0; j < _roomLimit+1; j++)
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

        _player.DOMove(_allCells[(3, 3)].transform.position, 0);

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
        UpdateBatlleCount();
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

        _cover.DOAnchorPos(new Vector2(-910, -1555), 0.9f).onComplete = ()=> _cover.DOAnchorPos(new Vector2(-910, 1555), 0);
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
    public void RemoveCurrentRoomIcon()
    {
        _currentPlayerCell.ResetIcon();
    }

    private void UpdatePlayerDirections(bool withReset = false)
    {
        if(withReset) _playerDirections.ForEach(x => x.DOFade(0, 0f));

        foreach (var pass in _currentPlayerCell.Tile.Passages)
        {
            _playerDirections[pass].DOFade(1, 0.2f);
        }
    }

    public void UpdateBatlleCount(int add = -1)
    {
        _battleCount = add==-1? 0 : _battleCount+add;
        _battlesLeft.text = _battleCount.ToString();

        float total = _roomLimit / 2;
        _battlesTotal.text = total.ToString();

        float percent = _battleCount / total;
        _fillableSkull.DOFillAmount(percent,1f).SetEase(Ease.OutCirc);
        _fillableCables.DOFillAmount(percent, 1f).SetEase(Ease.OutCirc);

        _descendButton.gameObject.SetActive(_battleCount == total);
        var col = _battleCount == total ? new Color(163f / 255f, 192f / 255f, 230f / 255f) : new Color(98f / 255f, 117f / 255f, 186f / 255f);
        _frame.color = col;
        _battlesLeft.color = col;
    }

    public void Descend(bool reset = false)
    {
        _floorNumber.text = reset ? "1" : $"{int.Parse(_floorNumber.text)+1}";
        BattleController.Instance.CurrentBattle = 0;

        if (CurrentFloor == BattleController.Instance.MaxFloors+1)
        {
            WinAndLoss.Instance.DungeonCleared();
            return;
        }

        _roomLimit = reset ? _roomLimitStart : _roomLimit + _roomLimitIncrease;

        ResetMap();
        GenerateMap();
    }
}
