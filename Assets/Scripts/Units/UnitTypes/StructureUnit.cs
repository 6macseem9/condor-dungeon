using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class StructureUnit : Unit
{
    [SerializeField] private UnityEvent _onSelect;

    protected override void Start()
    {
        _collider = GetComponent<Collider>();
        _visuals = GetComponent<UnitVisuals>();
        _animator = GetComponentInChildren<Animator>();
        Healthbar = GetComponentInChildren<Healthbar>();

        HP = Stats.MaxHP;
        Util.Repeat(1, -1, () => Heal(Stats.Regen));
    }

    protected override void Update()
    {
        
    }

    protected override void FixedUpdate()
    {
        
    }

    public override void Select(bool selected = true)
    {
        if(selected)_onSelect.Invoke();

        _visuals.ShowUiElements(selected);
        _visuals.BounceSelect();
    }

    public override void TakeDamage(Unit sender)
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

            _collider.enabled = false;
            _visuals.UiElements.SetActive(false);
            Healthbar.FadeOut();

            transform.DOShakePosition(0.4f, 0.05f,30,90,false,false).SetLoops(-1,LoopType.Restart);
            transform.DOMoveY(-3, 3).SetEase(Ease.InExpo).SetDelay(0.5f).onComplete = () => 
            {
                transform.DOKill();
                Destroy(gameObject);
            };
            return;
        }
    }
}
