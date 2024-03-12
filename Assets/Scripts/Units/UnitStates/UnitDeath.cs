using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class UnitDeath : UnitState
{
    private Collider _collider;
    private GameObject _uiElements;
    private GameObject _attackRange;
    private GameObject _detectRange;
    private Healthbar _healthbar;
    private SpriteRenderer _actionMarker;

    public UnitDeath(Unit unit, Animator animator, NavMeshAgent nav, SpriteRenderer actionMarker, Collider collider, GameObject uiElements, GameObject attackRange, GameObject detectRange, Healthbar healthbar) : base(unit, animator, nav)
    {
        _collider = collider;
        _uiElements = uiElements;
        _attackRange = attackRange;
        _detectRange = detectRange;
        _healthbar = healthbar;
        _actionMarker = actionMarker;
    }

    public override void OnEnter()
    {
        _animator.Play("death");

        _nav.enabled = false;
        _collider.enabled = false;
        _uiElements.SetActive(false);
        _attackRange.SetActive(false);
        _detectRange.SetActive(false);
        _healthbar.FadeOut();
        _actionMarker.transform.DOKill();

        UnitSelectionManager.Instance.RemoveUnit(_unit);
        Util.Delay(0.05f, () => WaitForAnimFinish());
    }
    public override void Update()
    {

    }
    public override void FixedUpdate()
    {

    }
    public override void OnExit()
    {
        
    }

    private void WaitForAnimFinish()
    {
        var animDuration = _animator.GetCurrentAnimatorStateInfo(0).length;
        _transform.DOMoveY(-2, 2).SetEase(Ease.InExpo).SetDelay(animDuration).onComplete = () => MonoBehaviour.Destroy(_unit.gameObject);
    }
}
