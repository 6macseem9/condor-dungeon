using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamagingItem : Item
{
    [Space(10)]
    public int Damage;

    public override void Activate()
    {
        GlobalEffects.Instance.ExplosionParticles.Play();

        var enemies = FindObjectsOfType<Unit>().Where(x => x.IsEnemy).ToList();
        enemies.ForEach(x => x.TakeDamage(Damage));
    }
}
