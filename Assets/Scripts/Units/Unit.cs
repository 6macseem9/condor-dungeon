using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(1)]
public class Unit : MonoBehaviour
{
    [field: SerializeField] public Class Class { get; private set; }
    public Stats Stats { get { return BonusStats==null? Class.Stats : Class.Stats + BonusStats; } }
    public Stats BonusStats { get; set; }
    public int Level { get; set; } = 1;
    public bool IsEnemy { get { return CompareTag("EnemyUnit"); } }
    public bool IsMoving { get { return _stateMachine.CurrentStateName=="UnitMove"; } }
    public int UpgradeCost { get { return LevelCost * Level + (int)(LevelCost * Level * 0.25f); } }
    public int LevelCost { get; set; } = 50;
    public Vector3 AssignedPosition { get; private set; }


    #region Components
    protected NavMeshAgent _nav;
    protected Animator _animator;
    protected Collider _collider;
    protected UnitVisuals _visuals;
    #endregion

    #region StateMachine & states
    private StateMachine _stateMachine;
    private UnitIdle _idleState;
    private UnitMove _moveState;
    private UnitAttack _attackState;
    private UnitChase _chaseState;
    private UnitDeath _unitDeathState;
    #endregion

    #region Combat Components
    public Range AttackRange { get; private set; }
    public Range DetectRange { get; private set; }
    public Healthbar Healthbar { get; protected set; }
    public AbilityController AbilityController { get; private set; }
    public Unit AttackTarget { get; set; }
    public bool Selected { get; private set; }
    public float HP { get; protected set; }
    public bool IsDying { get; protected set; }
    public bool HoldPosition { get; set; } = false;
    #endregion

    #region Actions
    public Action OnAttack { get; set; }
    public Action OnDeath { get; set; }
    public Action<bool> OnSelect { get; set; }
    public Action<int> OnLevelUp { get; set; }

    public Func<int,int> HandleDamage;
    #endregion

    protected virtual void Start()
    {
        _collider = GetComponent<Collider>();
        _visuals = GetComponent<UnitVisuals>();
        _animator = GetComponentInChildren<Animator>();
        Healthbar = GetComponentInChildren<Healthbar>();   
        AbilityController = GetComponentInChildren<AbilityController>();

        _nav = GetComponent<NavMeshAgent>();
        _nav.updateRotation = false;

        Class = Instantiate(Class);
        BonusStats = new Stats();

        #region Range init
        var ranges = GetComponentsInChildren<Range>();
        AttackRange = ranges[0];
        DetectRange = ranges[1];
        AttackRange.OnEnter += EnemyInAttackRange;
        AttackRange.OnExit += EnemyLeftAttackRange;
        DetectRange.OnEnter += EnemyDetected;
        DetectRange.OnExit += EnemyLost;
        #endregion

        HP = Stats.MaxHP;
        _animator.SetFloat("AttackSpeed", Stats.AttackSpeed);

        SetUpStateMachine();

        gameObject.name = NameGenerator.GetRandomName();

        AssignedPosition = transform.position;
    }
    private void SetUpStateMachine()
    {
        _stateMachine = new StateMachine();

        _idleState = new UnitIdle(this, _animator, _nav,_visuals.ActionMarker);
        _moveState = new UnitMove(this, _animator, _nav, _visuals.ActionMarker);
        _attackState = new UnitAttack(this, _animator, _nav,_visuals.ActionMarker);
        _chaseState = new UnitChase(this, _animator, _nav, _visuals.ActionMarker, AttackRange);
        _unitDeathState = new UnitDeath(this,_animator, _nav, _visuals.ActionMarker, _collider,_visuals.UiElements,AttackRange.gameObject,DetectRange.gameObject,Healthbar);

        _stateMachine.AddTransition(_idleState,_moveState, () => _nav.velocity != Vector3.zero && _nav.remainingDistance > 0.69f);
        _stateMachine.AddTransition(_moveState, _idleState, () => _moveState.Completed);
        _stateMachine.AddTransition(_attackState, _idleState, () => AttackTarget==null);
        _stateMachine.AddTransition(_chaseState, _idleState, () => AttackTarget == null);
        _stateMachine.AddNode(_unitDeathState);

        _stateMachine.TransitionTo(_idleState);
    }

    protected virtual void Update()
    {
        _stateMachine.Update();

        if (IsDying) return;
        if (AttackTarget!=null && AttackTarget.IsDying)
        {
            AttackTarget = null;
            DetectRange.ReTrigger();
            AttackRange.ReTrigger();
        }
    }
    protected virtual void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }

    #region Commands
    public void MoveTo(Vector3 position)
    {
        if (IsDying) return;

        _nav.SetDestination(position);
        _visuals.BounceMarker();

        AssignedPosition = position;
        AttackTarget = null;

        _stateMachine.TransitionTo(_moveState);
    }
    public virtual void Select(bool selected = true)
    {
        Selected = selected;
        _visuals.ShowUiElements(selected);
        _visuals.BounceSelect();

        OnSelect?.Invoke(selected);
    }

    public void Chase(Unit unit)
    {
        if (unit == null) return;
        AttackTarget = unit;

        AttackRange.ReTrigger();
        if (HoldPosition) return;

        _visuals.TargetMarker();
        _stateMachine.TransitionTo(_chaseState);
    }
    #endregion

    #region Detection
    private void EnemyDetected(Unit unit)
    {
        if (unit.IsDying) return;

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
        if (unit.IsDying) return;
        if (AttackTarget != unit) return;

        _stateMachine.TransitionTo(_attackState);
    }
    private void EnemyLeftAttackRange(Unit unit)
    {
        if (AttackTarget != unit) return;

        if (HoldPosition) _stateMachine.TransitionTo(_idleState); 
        else Chase(unit);
    }
    #endregion

    #region Damage / Heal
    public void DealDamageToTarget()
    {
        if (AttackTarget == null) return;

        OnAttack?.Invoke();
        DealDamage();
    }
    public virtual void DealDamage()
    {

    }

    public void DealDamageToUnit(Unit unit)
    {
        unit.TakeDamage(this);
    }

    public void TakeDamage(Unit sender)
    {
        if (IsDying) return;

        //OPTIMIZE
        var damage = Stats.CalculateMitigatedDamage(sender.Stats);
        if (HandleDamage is not null)
        {
            damage = HandleDamage(damage);
            if (damage == 0) return;
        }

        TakeDamage(damage);

        if (_stateMachine.CurrentStateName==nameof(UnitIdle) && AttackTarget == null)
        {
            Chase(sender);
            AttackRange.ReTrigger();
        }
    }

    public void TakeDamage(int trueDamage)
    {
        if (IsDying) return;

        HP -= trueDamage;
        _visuals.DamageImpact(_animator.transform);
        _visuals.Flash();
        _visuals.ShakeHealthbar();

        if (HP <= 0)
        {
            HP = 0;
            AttackTarget = null;
            IsDying = true;
            _stateMachine.TransitionTo(_unitDeathState);
            OnDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (IsDying) return;
        HP += amount;
        if(HP>Stats.MaxHP) { HP = Stats.MaxHP; }
    }

    public void HealPercent(int percent)
    {
        Heal(percent * Stats.MaxHP / 100);
    }

    public void FullHeal()
    {
        HP = Stats.MaxHP;

        if(IsDying)
        {
            IsDying = false;

            _collider.enabled =true;
            AttackRange.gameObject.SetActive(true);
            DetectRange.gameObject.SetActive(true);
            Healthbar.FadeOut(false);

            _stateMachine.TransitionTo(_idleState);
            _animator.Play("idle");
            Spawn();
        }
    }
    #endregion

    #region Misc

    public float GetAttackPerSecond()
    {
        if (_animator == null) return 0;

        var clips = _animator.runtimeAnimatorController.animationClips;
        var clip = clips.First((x) => x.name.Contains("attack"));

        return 1 / clip.length * Class.Stats.AttackSpeed;
    }

    public void Spawn()
    {
        transform.DOScale(0, 0);

        transform.DOScale(1,0.5f).SetEase(Ease.OutElastic,1);
    }

    public float DistanceTo(Unit unit)
    {
        if (unit == null) return 0;
        return _nav.DistanceTo(unit.transform.position);
    }

    public void UpgradeStat(int index)
    {
        Level++;
        OnLevelUp?.Invoke(Level);
        switch (index)
        {
            case 0: BonusStats.MaxHP += (int)(Class.Stats.MaxHP * 0.1f); if(!BattleController.Instance.InCombat) HP = Stats.MaxHP; break;
            case 1: BonusStats.AttackSpeed += /*Class.Stats.AttackSpeed * 0.1f*/0.04f; _animator.SetFloat("AttackSpeed", Stats.AttackSpeed); break;
            case 2: BonusStats.Strength += 1; break;
            case 3: BonusStats.Armor += 1; break;
            case 4: BonusStats.Intellect += 1; break;
            case 5: BonusStats.Resistance+= 1; break;
        }
    }

    public void EnableCollider(bool enable)
    {
        _collider.enabled = enable;
        _nav.enabled = enable;
    }

    public void UpdateAttackSpeed()
    {
        _animator.SetFloat("AttackSpeed", Stats.AttackSpeed);
    }

    public void SetDestination(Vector3 destination)
    {
        _nav.SetDestination(destination);
    }
    #endregion
}
