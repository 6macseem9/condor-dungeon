using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance;

    [SerializeField] private float _unitSpread=1;

    [Space(7)]
    [SerializeField] private StatBlock _statBlock;

    public List<Unit> AllUnits {get; private set;}
    private List<Unit> _selectedUnits = new List<Unit>();

    private Camera _camera;

    public bool SingleUnitSelected { get { return _selectedUnits.Count == 1; } }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else Instance = this;

        AllUnits = new List<Unit>();
    }

    void Start()
    {
        _camera = Camera.main;

        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
    }

    void Update()
    {
        //UIDebug.Instance.Show("Selected:", _selectedUnits.Count == 0 ? "null" : _selectedUnits[0].name, "yellow");
        
        UIDebug.Instance.Show("State:", _selectedUnits.Count == 0 ? "null" : _selectedUnits[0].CurState.Replace("Unit",""), "orange");

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

        if (Input.GetMouseButtonDown(1) && _selectedUnits.Count > 0)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Unit"))
                && hit.collider.CompareTag("EnemyUnit"))
            {
                foreach (var unit in _selectedUnits)
                {
                    unit.Chase(hit.collider.GetComponent<Unit>(),true);
                }
                return;
            }
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                MoveGroup(_selectedUnits, hit.point);
            }
            
        }
    }

    private void MoveGroup(List<Unit> units,Vector3 center)
    {
        var size = OptimalGridSize(units.Count);
        var rotation = Quaternion.LookRotation(center - units[0].transform.position, Vector3.up);

        var positions = SquareFormation(center, rotation, size, _unitSpread, units.Count==3? 0.5f: 0);

        for (int i = 0; i < units.Count; i++)
        {
            units[i].MoveTo(positions[i]);
        }
    }

    private void MultiSelect(Unit unit)
    {
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
            unit.Select(false);
        }
        _selectedUnits.Clear();
    }

    public List<Vector3> SquareFormation(Vector3 center, Quaternion rotation, Vector2 size, float _spread, float nthOffset = 0)
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

                pos = center + rotation * pos;
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
    }
    public void RemoveUnit(Unit unit)
    {
        AllUnits.Remove(unit);
        _selectedUnits.Remove(unit);
    }

    public void ToggleSelectedUnitMode()
    {
        if (_selectedUnits.Count <= 0) return;
        _selectedUnits[0].ToggleMode();
    }
    public void SetSelectedUnitsMode(bool hold)
    {
        if (_selectedUnits.Count <= 1) return;

        foreach (var unit in _selectedUnits)
        {
            unit.HoldPosition = hold;
        }
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

public static class Util
{
    public static Tweener Delay(float time, TweenCallback func, bool realTime = false)
    {
        float timer = 0;
        Tweener tween = DOTween.To(() => timer, x => timer = x, time, time).SetUpdate(realTime);
        tween.onComplete = func;
        return tween;
    }

    public static Tweener Repeat(float time, int times, TweenCallback func, bool realTime = false)
    {
        float timer = 0;
        Tweener tween = DOTween.To(() => timer, x => timer = x, time, time).SetUpdate(realTime);
        tween.SetLoops(times);
        tween.onStepComplete = func;
        return tween;
    }
}