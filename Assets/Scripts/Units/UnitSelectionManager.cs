using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class UnitPosition
{
    public Unit Unit;
    public Vector3 Position;
}

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance;

    [SerializeField] private List<UnitPosition> _startingUnits;
    [SerializeField] private float _unitSpread=1;

    [Space(7)]
    [SerializeField] private StatBlock _statBlock;
    [SerializeField] private GroupFormation _formation;

    [Space(7)]
#if UNITY_EDITOR
    [NamedArrayAttribute("")]
#endif
    public List<Unit> UnitDB;

    public List<Unit> AllUnits {get; private set;}
    private List<Unit> _selectedUnits = new List<Unit>();

    private Camera _camera;
    public bool SingleUnitSelected { get { return _selectedUnits.Count == 1; } }

    private bool _canControlUnits = true;
    private int _deadUnits;

    public delegate void AllUnitsEvent(List<Unit> allUnits, Unit addedUnit);
    public event AllUnitsEvent UnitAddedOrRemoved;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else Instance = this;

        AllUnits = new List<Unit>();
    }

    private void Start()
    {
        _camera = Camera.main;

        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
    }

    private void Update()
    {
        //if (_selectedUnits.Count == 1) UIDebug.Instance.Show("state:", _selectedUnits[0].CurState,"yellow");

        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Unit")))
                MultiSelect(hit.collider.GetComponent<Unit>());
            else
            {
                DeselectAll();

                _statBlock.SetStats(null);
            }
        }

        if (_canControlUnits && Input.GetMouseButtonDown(1) && _selectedUnits.Count > 0)
        {
            //if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Unit"))
            //    && hit.collider.CompareTag("EnemyUnit"))
            //{
            //    foreach (var unit in _selectedUnits)
            //    {
            //        unit.Chase(hit.collider.GetComponent<Unit>());
            //    }
            //    return;
            //}
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                MoveGroup(_selectedUnits, hit.point);
            }
            
        }
    }
    private void MoveGroup(List<Unit> units,Vector3 center)
    {
        if (_formation.CurrentFormation==Formation.NoFormation)
        {
            Vector3 oldCenter = GetCenter(units);

            foreach (var unit in units)
            {
                if (unit.IsEnemy) continue;
                var offset = unit.transform.position - oldCenter;

                unit.MoveTo(center + offset);
            }
        }
        else
        {
            Vector2Int size = Vector2Int.zero;
            switch (_formation.CurrentFormation)
            {
                case Formation.Square: size = OptimalGridSize(units.Count); break;
                case Formation.ThickRow: size = new Vector2Int(Mathf.CeilToInt(units.Count / 2f), 2); break;
                case Formation.Column: size = new Vector2Int(1, units.Count); break;
                case Formation.Row: size = new Vector2Int(units.Count, 1); break;
            }

            var offset = _formation.CurrentFormation == Formation.Square && units.Count == 3 ? 0.5f : 0;
            var positions = SquareFormation(center, size, _unitSpread, offset);

            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].IsEnemy) continue;
                units[i].MoveTo(positions[i]);
            }
        }
    }

    private void MultiSelect(Unit unit)
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            DeselectAll();
            SelectClass(unit.Class.ClassName);
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(unit.Selected)
            {
                Deselect(unit);
                return;
            }
        }
        else DeselectAll();

        Select(unit);
    }
    public void Select(Unit unit)
    {
        if (_selectedUnits.Contains(unit)) return;

        if (unit.IsEnemy) DeselectAll();
        else DeselectEnemy();

        _selectedUnits.Add(unit);
        unit.Select(true);
        _statBlock.SetStats(_selectedUnits);
    }

    public void Deselect(Unit unit)
    {
        unit.Select(false);
        _selectedUnits.Remove(unit);

        _statBlock.SetStats(_selectedUnits.Count==0 ? null : _selectedUnits);
    }
    public void DeselectAll()
    {
        foreach (var unit in _selectedUnits)
        {
            if (unit == null) continue;
            unit.Select(false);
        }
        _selectedUnits.Clear();
    }
    public void DeselectEnemy()
    {
        if (_selectedUnits.Count == 0) return;
        var enemy = _selectedUnits.FirstOrDefault((x) => x.IsEnemy);
        if (enemy == null) return;

        enemy.Select(false);
        _selectedUnits.Remove(enemy);
    }

    public List<Vector3> SquareFormation(Vector3 center, Vector2 size, float _spread, float nthOffset = 0)
    {
        var positions = new List<Vector3>();
        var middleOffset = new Vector3(size.x * 0.5f, 0, size.y * 0.5f);

        for (var x = 0; x < size.x; x++)
        {
            for (var z = 0; z < size.y; z++)
            {
                var pos = new Vector3(x + (z % 2 == 0 ? 0 : nthOffset), 0, z);

                pos -= middleOffset;
                pos += new Vector3(0.5f, 0, 0.5f);
                
                pos *= _spread;

                pos = center + pos;
                positions.Add(pos);
            }
        }
        return positions;
    }
    private Vector2Int OptimalGridSize(int n)
    {
        int bestDiff = n;
        int bestI = 1, bestJ = n;

        for (int i = 1; i <= n; i++)
        {
            int j = (int)Math.Ceiling((double)n / i);
            int diff = Math.Abs(i - j);
            if (diff <= bestDiff)
            {
                bestDiff = diff;
                bestI = i;
                bestJ = j;
            }
        }

        return new Vector2Int(bestI, bestJ);
    }

    public void AddUnit(Unit unit)
    {
        AllUnits.Add(unit);
        UnitAddedOrRemoved?.Invoke(AllUnits, unit);
    }
    public void SetSelectedUnitsMode(bool hold)
    {
        if (_selectedUnits.Count <= 1) return;

        foreach (var unit in _selectedUnits)
        {
            unit.HoldPosition = hold;
        }
    }

    public void SelectClass(string name)
    {
        DeselectAll();
        foreach (var unit in AllUnits)
        {
            if (unit.Class.ClassName == name)
                Select(unit);
        }
    }

    public void PauseUnitControl(bool pause)
    {
        _canControlUnits = !pause;
    }
    public void StopAllUnits()
    {
        foreach(var unit in AllUnits)
        {
            if (unit.IsMoving)
                unit.MoveTo(unit.transform.position);
        }
    }

    public void SpawnStartUnits()
    {
        _deadUnits = 0;

        AllUnits.AddRange(FindObjectsOfType<Unit>().Where(x=>x.IsEnemy && !x.IsDying));
        foreach(var unit in AllUnits)
        {
            if(unit == null) continue;
            Destroy(unit.gameObject);
        }
        AllUnits.Clear();
        UnitAddedOrRemoved?.Invoke(AllUnits, null);

        foreach (var entry in _startingUnits)
        {
            var unit = Instantiate(entry.Unit);
            unit.transform.position = entry.Position;
            Util.DelayOneFrame(()=>unit.SetDestination(entry.Position));
            AddUnit(unit);
        }
    }

    public void UnitDied()
    {
        _deadUnits++;
        if(_deadUnits==AllUnits.Count)
        {
            Util.Delay(2,()=> WinAndLoss.Instance.YouDied());
        }
    }

    public void FullHeal()
    {
        _deadUnits = 0;
        AllUnits.ForEach(unit => unit.FullHeal());
    }
    public void UpdateSelectedUnitStats()
    {
        if (_selectedUnits.Count != 1) return;

        _statBlock.SetStats(_selectedUnits);
    }

    public void ReturnUnitsToPositions()
    {
        AllUnits.ForEach(x => x.MoveTo(x.AssignedPosition));
    }

    public void SortUnits()
    {
        AllUnits = AllUnits.OrderBy(x => x.Class.ClassName).ToList();
        UnitAddedOrRemoved(AllUnits,null);
    }

    private Vector3 GetCenter(List<Unit> list)
    {
        var totalX = 0f;
        var totalY = 0f;
        var totalZ = 0f;
        foreach (var obj in list)
        {
            totalX += obj.transform.position.x;
            totalY += obj.transform.position.y;
            totalZ += obj.transform.position.z;
        }
        return new Vector3(totalX, totalY, totalZ) / list.Count;
    }
    //private void OnDrawGizmos()
    //{
    //    var size = OptimalGridSize(3);

    //    var positions = SquareFormation(transform.position, transform.rotation, size, 1.5f,0.5f);

    //    Color[] colors = { Color.red, Color.yellow, Color.green, Color.white };

    //    int i = 0;
    //    foreach (var pos in positions)
    //    {
    //        Gizmos.color = colors[i];
    //        Gizmos.DrawWireSphere(pos, 0.5f);
    //        i++;
    //    }
    //}
}