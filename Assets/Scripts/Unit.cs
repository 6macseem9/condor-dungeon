using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [SerializeField] private GameObject _uiElements;

    private SpriteRenderer _selectionRing;
    private SpriteRenderer _actionMarker;
    private NavMeshAgent _nav;
    private Animator _animator;

    private StateMachine _stateMachine;
    private UnitMove moveState;

    private Tweener _selectBounce;

    public bool Selected { get; private set; }

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

        var idleState = new UnitIdle(transform, _animator, _nav);
        moveState = new UnitMove(transform, _animator, _nav, _actionMarker);

        _stateMachine.AddTransition(idleState,moveState, () => _nav.velocity != Vector3.zero && _nav.remainingDistance > 1);
        _stateMachine.AddTransition(moveState, idleState, () => moveState.Completed);

        _stateMachine.TransitionTo(idleState);
    }

    private void Update()
    {
        _stateMachine.Update();

        _uiElements.SetActive(Selected);

        if (Selected) UIDebug.Instance.Show("State:", _stateMachine.CurrentStateName, "orange");
    }
    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }

    public void MoveTo(Vector3 position)
    {
        _nav.SetDestination(position);
        BounceMarker();

        _stateMachine.TransitionTo(moveState);
    }

    private void BounceMarker()
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
