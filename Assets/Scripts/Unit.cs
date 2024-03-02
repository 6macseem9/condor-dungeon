using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [SerializeField]private GameObject _uiElements;

    private SpriteRenderer _selectionRing;
    private SpriteRenderer _actionMarker;
    private NavMeshAgent _nav;
    private Animator _animator;
    private StateMachine _stateMachine;

    private Tweener _selectBounce;

    public bool Selected { get; private set; }
    private bool _startedPath;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _nav = GetComponent<NavMeshAgent>();
        _nav.updateRotation = false;

        var sprites = GetComponentsInChildren<SpriteRenderer>();
        _selectionRing = sprites[1];
        _actionMarker = sprites[2];

        _actionMarker.transform.DORotate(new Vector3(90, 0, 360), 5, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);

        SetUpStateMachine();

        UnitSelectionManager.Instance.AddUnit(this);
    }
    private void SetUpStateMachine()
    {
        _stateMachine = new StateMachine();

        var idleState = new UnitIdle(_animator, _nav);
        var moveState = new UnitMove(_animator, _nav, _actionMarker);

        _stateMachine.AddTransition(idleState, moveState, () => false);
        _stateMachine.AddTransition(moveState, idleState, () => moveState.Completed);

        _stateMachine.SetState(idleState);
    }

    private void Update()
    {
        _stateMachine.Update();

        _uiElements.SetActive(Selected);

        _actionMarker.transform.position = _nav.destination;

        if (_startedPath && _nav.velocity == Vector3.zero)
        {
            _actionMarker.enabled = false;
            _startedPath = false;
            _animator.SetBool("isMoving", false);

            _nav.avoidancePriority = 50;
            _nav.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        }
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();

        if (_nav.steeringTarget == transform.position) return;
        Vector3 lookAtPos = _nav.steeringTarget - transform.position;
        Quaternion newRotation = Quaternion.LookRotation(lookAtPos, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 0.1f);
    }


    public void MoveTo(Vector3 position)
    {
        _nav.SetDestination(position);
        
        MoveMarker();
        _animator.SetBool("isMoving", true);

        Util.Delay(0.02f, () => _startedPath = true);
        _nav.avoidancePriority = 49;
        _nav.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }

    private void MoveMarker()
    {
        _actionMarker.enabled = true;

        _actionMarker.transform.DOScale(1f, 0);
        _actionMarker.transform.DOScale(0.6f, 0.5f).SetEase(Ease.OutElastic);
    }
    
    public void Select(bool selected = true)
    {
        Selected = selected;
        
        _selectBounce.Kill();
        _selectBounce = _selectionRing.transform.DOScale(1.2f, 0.1f);
        _selectBounce.onComplete = ()=> _selectBounce=_selectionRing.transform.DOScale(1f, 0.5f).SetEase(Ease.OutElastic);
    }
}
