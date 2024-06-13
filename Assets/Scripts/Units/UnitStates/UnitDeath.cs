using DG.Tweening;
using Unity.VisualScripting;
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

        _collider.enabled = false;
        _attackRange.SetActive(false);
        _detectRange.SetActive(false);
        _healthbar.FadeOut();

        if(_unit.IsEnemy)
        {
            _nav.enabled = false;
            _collider.enabled = false;
            _uiElements.SetActive(false);
            _actionMarker.transform.DOKill();

            Util.Delay(0.05f, () => WaitForAnimFinish());
            return;
        }

        UnitSelectionManager.Instance.UnitDied();
    }
    private void WaitForAnimFinish()
    {
        var animDuration = _animator.GetCurrentAnimatorStateInfo(0).length;
        _transform.DOMoveY(-2, 2).SetEase(Ease.InExpo).SetDelay(animDuration).onComplete = () =>
        {
            BattleController.Instance.DecreaseEnemyCount();
            MonoBehaviour.Destroy(_unit.gameObject);
        };
    }
    public override void FixedUpdate()
    {
        
    }
}
