using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectBox : MonoBehaviour
{
    private Camera _cam;
    private Canvas _canvas;
    private CanvasGroup _group;
    private RectTransform _rectTransform;

    private Rect _selectionBox;

    private Vector2 _startPos;
    private Vector2 _endPos;

    private bool _started;
    private Tweener _delay;

    private void Start()
    {
        _cam = Camera.main;
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _group = GetComponentInParent<CanvasGroup>();
    }

    private void Update()
    {
        if (_group.alpha == 0) return;
        if (!_started && EventSystem.current.IsPointerOverGameObject()) return;
        // When Clicked
        if (Input.GetMouseButtonDown(0))
        {
            _startPos = Input.mousePosition;

            _delay.Kill();
            _delay = Util.Delay(0.15f, ()=>_started = true);

            _selectionBox = new Rect();
        }

        // When Dragging
        if (Input.GetMouseButton(0))
        {
            _endPos = Input.mousePosition;

        }

        // When Releasing
        if (Input.GetMouseButtonUp(0))
        {
            _started = false;
            _startPos = Input.mousePosition;
            _endPos = Input.mousePosition;
            _delay.Kill();

            Draw();
        }

        if ((_endPos - _startPos).magnitude < 150 && !_started) return;

        if (_rectTransform.rect.width > 5 || _rectTransform.rect.height > 5)
        {
            SelectUnits();
        }

        Draw();
    }

    private void Draw()
    {
        _rectTransform.position = (_startPos + _endPos) / 2;

        var start = _startPos / _canvas.scaleFactor;
        var end = _endPos / _canvas.scaleFactor;
        Vector2 boxSize = new Vector2(Mathf.Abs(start.x - end.x), Mathf.Abs( start.y - end.y));
        _rectTransform.sizeDelta = boxSize;

        _selectionBox.xMin = Mathf.Min(Input.mousePosition.x, _startPos.x);
        _selectionBox.xMax = Mathf.Max(Input.mousePosition.x, _startPos.x);

        _selectionBox.yMin = Mathf.Min(Input.mousePosition.y, _startPos.y);
        _selectionBox.yMax = Mathf.Max(Input.mousePosition.y, _startPos.y);
    }

    private void SelectUnits()
    {
        // remove from all units so only unselected are looped through?
        // PERFORMANCE TERRORIST
        foreach (var unit in UnitSelectionManager.Instance.AllUnits)
        {
            if (UnitInBounds(unit))
            {
                if (!unit.Selected) UnitSelectionManager.Instance.Select(unit);
            }
            else
            {
                if (unit.Selected) UnitSelectionManager.Instance.Deselect(unit);
            }
        }
    }

    private bool UnitInBounds(Unit unit)
    {
        Vector3 unitHeight = new Vector3(0, 1.2f, 0);

        return _selectionBox.Contains(_cam.WorldToScreenPoint(unit.transform.position))
                || _selectionBox.Contains(_cam.WorldToScreenPoint(unit.transform.position + unitHeight));
    }
}