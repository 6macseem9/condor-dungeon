using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[DefaultExecutionOrder(1)]
public class Unit : MonoBehaviour
{
    [field: SerializeField] public Stats Stats { get; private set; }

    protected NavMeshAgent _nav;
    protected Animator _animator;
    protected Collider _collider;
    protected UnitVisuals _visuals;

    private StateMachine _stateMachine;
    private UnitIdle _idleState;
    private UnitMove _moveState;
    private UnitAttack _attackState;
    private UnitChase _chaseState;
    private UnitDeath _deathState;

    public Range AttackRange { get; private set; }
    public Range DetectRange { get; private set; }

    public Healthbar Healthbar { get; protected set; }
    public AbilityController AbilityController { get; private set; }
    public Unit AttackTarget { get; set; }
    public bool Selected { get; private set; }
    public float HP { get; protected set; }
    public bool IsDying { get; protected set; }
    public bool HoldPosition { get; set; }

    public Action TargetLost { get; set; }
    public Action OnAttack { get; set; }

    //TEMP
    public string CurState { get { return _stateMachine==null? "structure" : _stateMachine.CurrentStateName; } }

    protected virtual void Start()
    {
        _collider = GetComponent<Collider>();
        _visuals = GetComponent<UnitVisuals>();
        _animator = GetComponentInChildren<Animator>();
        Healthbar = GetComponentInChildren<Healthbar>();   
        AbilityController = GetComponentInChildren<AbilityController>();

        _nav = GetComponent<NavMeshAgent>();
        _nav.updateRotation = false;

        Stats = Instantiate(Stats);

        var ranges = GetComponentsInChildren<Range>();
        AttackRange = ranges[0];
        DetectRange = ranges[1];
        AttackRange.OnEnter += EnemyInAttackRange;
        AttackRange.OnExit += EnemyLeftAttackRange;
        DetectRange.OnEnter += EnemyDetected;
        DetectRange.OnExit += EnemyLost;

        HP = Stats.MaxHP;
        _nav.speed = Stats.MoveSpeed;
        _animator.SetFloat("AttackSpeed", Stats.AttackSpeed);
        Util.Repeat(1, -1, () => Heal(Stats.Regen));

        SetUpStateMachine();

        if (this.IsEnemy()) return;
        UnitSelectionManager.Instance.AddUnit(this);
    }
    private void SetUpStateMachine()
    {
        _stateMachine = new StateMachine();

        _idleState = new UnitIdle(this, _animator, _nav,_visuals.ActionMarker);
        _moveState = new UnitMove(this, _animator, _nav, _visuals.ActionMarker);
        _attackState = new UnitAttack(this, _animator, _nav,_visuals.ActionMarker);
        _chaseState = new UnitChase(this, _animator, _nav, _visuals.ActionMarker, AttackRange);
        _deathState = new UnitDeath(this,_animator, _nav, _visuals.ActionMarker, _collider,_visuals.UiElements,AttackRange.gameObject,DetectRange.gameObject,Healthbar);

        _stateMachine.AddTransition(_idleState,_moveState, () => _nav.velocity != Vector3.zero && _nav.remainingDistance > 0.69f);
        _stateMachine.AddTransition(_moveState, _idleState, () => _moveState.Completed);
        _stateMachine.AddTransition(_attackState, _idleState, () => AttackTarget==null);
        _stateMachine.AddTransition(_chaseState, _idleState, () => AttackTarget == null);
        _stateMachine.AddNode(_deathState);

        _stateMachine.TransitionTo(_idleState);
    }

    protected virtual void Update()
    {
        _stateMachine.Update();

        if (AttackTarget!=null && AttackTarget.IsDying)
        {
            TargetLost?.Invoke();

            AttackTarget = null;
            DetectRange.ReTrigger();
            AttackRange.ReTrigger();
        }
    }
    protected virtual void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }

    public void MoveTo(Vector3 position)
    {
        _nav.SetDestination(position);
        _visuals.BounceMarker();

        AttackTarget = null;

        _stateMachine.TransitionTo(_moveState);
    }
    public virtual void Select(bool selected = true)
    {
        Selected = selected;
        _visuals.ShowUiElements(selected);
        _visuals.BounceSelect();
    }

    public void Chase(Unit unit, bool command = false)
    {
        if (AttackTarget == unit && command) return;
        AttackTarget = unit;

        AttackRange.ReTrigger();
        if (HoldPosition && !command) return;

        _visuals.TargetMarker();
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
            TargetLost?.Invoke();
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

        OnAttack?.Invoke();
        DealDamage();
    }
    public virtual void DealDamage()
    {

    }

    public virtual void DealDamageToUnit(Unit unit)
    {
        unit.TakeDamage(this);
    }

    public virtual void TakeDamage(Unit sender)
    {
        if (IsDying) return;

        var damage = Stats.CalculateMitigatedDamage(sender.Stats);

        HP -= damage;
        _visuals.DamageImpact(_animator.transform);
        _visuals.Flash();
        _visuals.ShakeHealthbar();

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
            AttackRange.ReTrigger();
        }
    }

    public void Heal(float amount)
    {
        if (IsDying) return;

        HP += amount;
        if(HP>Stats.MaxHP) { HP = Stats.MaxHP; }
    }

    public bool ToggleMode()
    {
        HoldPosition = !HoldPosition;

        return HoldPosition;
    }

    public float GetAttackPerSecond()
    {
        if (_animator == null) return 0;

        var clips = _animator.runtimeAnimatorController.animationClips;
        var clip = clips.First((x) => x.name.Contains("attack"));

        return 1 / clip.length * Stats.AttackSpeed;
    }

    public void Spawn()
    {
        transform.DOScale(0, 0);

        transform.DOScale(1,0.5f).SetEase(Ease.OutElastic,1);
    }

    public void Kill()
    {
        HP = 0;
        IsDying = true;
        _stateMachine.TransitionTo(_deathState);
    }

    public float DistanceTo(Unit unit)
    {
        if (unit == null) return 0;
        return _nav.DistanceTo(unit.transform.position);
    }
    public float DistanceTo(Vector3 position)
    {
        return _nav.DistanceTo(position);
    }
}
