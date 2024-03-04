using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;

[DefaultExecutionOrder(1)]
public class Unit : MonoBehaviour
{
    [field: SerializeField] public int MaxHP { get; private set; }
    [field: SerializeField] public bool HoldPosition { get; private set; }
    [SerializeField] private int _damage;

    [SerializeField] private GameObject _uiElements;

    private SpriteRenderer _selectionRing;
    private SpriteRenderer _actionMarker;
    private Range _attackRange;
    private Range _detectRange;
    private Healthbar _healthbar;

    private NavMeshAgent _nav;
    private Animator _animator;
    private Collider _collider;

    private StateMachine _stateMachine;
    private UnitIdle _idleState;
    private UnitMove _moveState;
    private UnitAttack _attackState;
    private UnitChase _chaseState;
    private UnitDeath _deathState;

    public Unit AttackTarget { get; set; }

    private Tweener _selectBounce;

    public bool Selected { get; private set; }
    public int HP { get; private set; }
    public bool IsDying { get; private set; }

    //TEMP
    public string CurState { get { return _stateMachine.CurrentStateName; } } 

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _animator = GetComponentInChildren<Animator>();
        _healthbar = GetComponentInChildren<Healthbar>();   

        _nav = GetComponent<NavMeshAgent>();
        _nav.updateRotation = false;

        var sprites = GetComponentsInChildren<SpriteRenderer>();
        _selectionRing = sprites[1];
        _actionMarker = sprites[2];

        var ranges = GetComponentsInChildren<Range>();
        _attackRange = ranges[0];
        _detectRange = ranges[1];
        _attackRange.OnEnter += EnemyInAttackRange;
        _attackRange.OnExit += EnemyLeftAttackRange;
        _detectRange.OnEnter += EnemyDetected;
        _detectRange.OnExit += EnemyLost;

        HP = MaxHP;

        _actionMarker.transform.DORotate(new Vector3(90, 0, 360), 5, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        _uiElements.SetActive(Selected);

        SetUpStateMachine();

        UnitSelectionManager.Instance.AddUnit(this);
    }
    private void SetUpStateMachine()
    {
        _stateMachine = new StateMachine();

        _idleState = new UnitIdle(this, _animator, _nav);
        _moveState = new UnitMove(this, _animator, _nav, _actionMarker);
        _attackState = new UnitAttack(this, _animator, _nav);
        _chaseState = new UnitChase(this, _animator, _nav, _actionMarker);
        _deathState = new UnitDeath(this,_animator, _nav, _actionMarker.transform, _collider,_uiElements,_attackRange.gameObject,_detectRange.gameObject,_healthbar);

        _stateMachine.AddTransition(_idleState,_moveState, () => _nav.velocity != Vector3.zero && _nav.remainingDistance > 0.69f);
        _stateMachine.AddTransition(_moveState, _idleState, () => _moveState.Completed);
        _stateMachine.AddTransition(_attackState, _idleState, () => AttackTarget==null);
        _stateMachine.AddTransition(_chaseState, _idleState, () => AttackTarget == null);
        _stateMachine.AddNode(_deathState);

        _stateMachine.TransitionTo(_idleState);
    }

    private void Update()
    {
        _stateMachine.Update();

        if (AttackTarget!=null && AttackTarget.IsDying)
        {
            AttackTarget = null;
            _detectRange.ReTrigger();
            _attackRange.ReTrigger();
        }
    }
    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }

    private void BounceMarker()
    {
        _actionMarker.enabled = true;

        _actionMarker.transform.DOScale(1f, 0);
        _actionMarker.transform.DOScale(0.6f, 0.5f).SetEase(Ease.OutElastic);
    }

    public void MoveTo(Vector3 position)
    {
        _nav.SetDestination(position);
        BounceMarker();

        AttackTarget = null;

        _stateMachine.TransitionTo(_moveState);
    }
    public void Select(bool selected = true)
    {
        Selected = selected;
        _uiElements.SetActive(Selected);

        _selectBounce.Kill();
        _selectBounce = _selectionRing.transform.DOScale(1.2f, 0.1f);
        _selectBounce.onComplete = ()=> _selectBounce=_selectionRing.transform.DOScale(1f, 0.5f).SetEase(Ease.OutElastic);
    }
    public void Chase(Unit unit, bool command = false)
    {
        if (AttackTarget == unit && command) return;
        AttackTarget = unit;

        _attackRange.ReTrigger();
        if (HoldPosition && !command) return;

        //BounceMarker();
        _stateMachine.TransitionTo(_chaseState);
    }

    private void EnemyDetected(Unit unit)
    {
        if (AttackTarget == null)
        {
            Chase(unit);
        }
    }
    private void EnemyLost(Unit unit)
    {
        if (AttackTarget == unit)
        {
            AttackTarget = null;
        }
    }
    private void EnemyInAttackRange(Unit unit)
    {
        if (_stateMachine.CurrentStateName == nameof(UnitMove)) return;
        if (AttackTarget != unit) return;

        _stateMachine.TransitionTo(_attackState);
    }
    private void EnemyLeftAttackRange(Unit unit)
    {
        if (_stateMachine.CurrentStateName == nameof(UnitMove)) return;
        if (AttackTarget != unit) return;

        if (HoldPosition) _stateMachine.TransitionTo(_idleState); 
        else Chase(unit);
    }

    public void DealDamageToTarget()
    {
        if (AttackTarget == null) return;

        AttackTarget.TakeDamage(_damage,this);
    }
    public void TakeDamage(int amount, Unit sender)
    {
        if (IsDying) return;

        HP -= amount;
        if (HP <= 0)
        {
            HP = 0;
            IsDying = true;
            _stateMachine.TransitionTo(_deathState);
            return;
        }

        if (_stateMachine.CurrentStateName==nameof(UnitIdle) && AttackTarget == null)
        {
            Chase(sender);
            _attackRange.ReTrigger();
        }
    }
}
