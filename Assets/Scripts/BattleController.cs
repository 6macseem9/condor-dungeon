using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [SerializeField] private BattleSequence _battleSequence;
    [SerializeField] private Vector3[] _spawnPositions;
    
    private Spawner[] _spawners;
    private int _currentBattle = 0;

    private void Start()
    {
        _spawners = GetComponentsInChildren<Spawner>();

        RandomizeSpawnPositions();
    }

    private void RandomSpawn(Unit[] units)
    {
        var unit = units[Random.Range(0, units.Length)];
        var spawner = _spawners[Random.Range(0, _spawners.Length)];

        var instance = Instantiate(unit, spawner.SpawnPoint, Quaternion.Euler(0,180,0));
        instance.Spawn();
    }

    private void RandomizeSpawnPositions()
    {
        List<int> used = new List<int>();
        int rand;

        foreach (var spawner in _spawners)
        {
            do
            {
                rand = Random.Range(0, _spawnPositions.Length);
            } while (used.Contains(rand));

            spawner.transform.localPosition = _spawnPositions[rand];
            spawner.ShowIcon(true);
            used.Add(rand);
        }
    }
    public void StartBattle()
    {
        //UnitSelectionManager.Instance.PauseInput();

        foreach (var spawner in _spawners)
            spawner.ShowIcon(false);

        var battle = _battleSequence.Sequence[_currentBattle];

        var step = 0;
        var skips = 0;
        var loop = Util.Repeat(battle.Interval, -1, () => { });
        loop.onStepComplete = () =>
        {
            if(step!=0 && skips<2 && Random.Range(1,101) <= battle.SkipChance)
            {
                skips++;
                Debug.Log("skip " + skips);
                return;
            }
            step++;
            skips = 0;
            Debug.Log(step);
            RandomSpawn(battle.Enemies);

            if(step == battle.Amount) 
                loop.Kill();
        };
    }
}
