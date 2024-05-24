using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Class", menuName = "Enemy Class", order = 0)]
public class EnemyClass : Class
{
    [Space(7)]
    public List<Stats> LevelUpBonuses;
}
