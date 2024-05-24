using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellType { Appear, Wave }
public class Spell : MonoBehaviour
{
    [SerializeField] private SpellType _type;
    [SerializeField] private float _collisionDelay;
    [SerializeField] private float _duration;
    [SerializeField] private Ease _ease;

    private Unit _unit;
    private Collider _collider;
    private ParticleSystem _particles;

    private void Start()
    {
        _unit = GetComponentInParent<Unit>();
        _collider = GetComponent<Collider>();
        _particles = GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_unit.IsEnemy && !other.CompareTag("FriendlyUnit")) return;
        if (!_unit.IsEnemy && !other.CompareTag("EnemyUnit")) return;

        var target = other.GetComponent<Unit>();
        _unit.DealDamageToUnit(target);
    }

    public void Cast()
    {
        _particles.Play();

        Util.Delay(_collisionDelay, () =>
        {
            switch (_type)
            {
                case SpellType.Appear: Appear(); break;
                case SpellType.Wave: Wave(); break;
            }
        });
    }

    private void Appear()
    {
        _collider.enabled = true;
        Util.Delay(_duration, () => _collider.enabled = false);
    }
    private void Wave()
    {
        transform.localScale = new Vector3(1, 1, 0f);
        _collider.enabled = true;

        transform.DOScaleZ(1, _duration).SetEase(_ease).onComplete =
            () => _collider.enabled = false;
    }
}
