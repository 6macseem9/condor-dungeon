using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class EnemyDanger
{
    public Unit Unit;
    public int DangerLevel;
}

public class BattleController : MonoBehaviour
{
    public static BattleController Instance;

    [SerializeField] private EnemyDanger[] _enemies;
    [SerializeField] private int[] _battlesDanger;
    [Space(5)]
    [SerializeField] private Vector3[] _spawnPositions;
    [Space(5)]
    [SerializeField] private CanvasGroup _startButtonGroup;
    [SerializeField] private RectTransform _startButton;
    [SerializeField] private RectTransform _skull;
    [SerializeField] private RectTransform _sword;
    [Space(5)]
    [SerializeField] private BattleIntroAndRewards _battleIntro;
    
    public bool InCombat { get; private set; }

    private Spawner[] _spawners;
    private int _currentBattle = 0;

    private int _enemyCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else Instance = this;
    }
    private void Start()
    {
        _spawners = GetComponentsInChildren<Spawner>();

        ShowSpawners(false);
        _startButton.GetComponentInChildren<Button>().AddPressAnimation();
        _sword.GetComponentInChildren<Button>().AddPressAnimation();
    }

    private void RandomSpawn(Unit unit)
    {
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
    private void ShowSpawners(bool show)
    {
        foreach (Spawner spawner in _spawners)
        {
            spawner.gameObject.SetActive(show);
            if (show == false) spawner.ShowIcon(true);
        }
    }
    public void InitializeBattle()
    {
        _battleIntro.Intro();

        RandomizeSpawnPositions();
        _startButtonGroup.gameObject.SetActive(true);
        _startButtonGroup.blocksRaycasts = true;
        _startButtonGroup.interactable = true;

        ShowSpawners(true);
    }
    public void StartBattle()
    {
        InCombat = true;
        StartButtonAnimation();
        UnitSelectionManager.Instance.PauseUnitControl(true);
        UnitSelectionManager.Instance.StopAllUnits();

        foreach (var spawner in _spawners)
            spawner.ShowIcon(false);

        var danger = _battlesDanger[_currentBattle];
        var enemies = ChooseEnemiesForBattle(100).OrderBy(x=>Random.value).ToList();
        _enemyCount = enemies.Count;

        var interval = 1;
        var skip = 50;

        var step = -1;
        var skips = 0;
        var loop = Util.Repeat(interval, -1, () => { });
        loop.onStepComplete = () =>
        {
            if(step!=0 && skips<2 && Random.Range(1,101) <= skip)
            {
                skips++;
                Debug.Log("skip " + skips);
                return;
            }
            step++;
            skips = 0;
            Debug.Log(step);
            RandomSpawn(enemies[step]);

            if(step == enemies.Count-1)
            {
                ShowSpawners(false);
                loop.Kill();
            }
        };
    }
    public List<Unit> ChooseEnemiesForBattle(int danger)
    {
        List<Unit> enemies = new List<Unit>();
        int remainingDanger = danger;

        while (remainingDanger > 0)
        {
            EnemyDanger enemy = GetRandomValidMonster(remainingDanger);
            enemies.Add(enemy.Unit);
            remainingDanger -= enemy.DangerLevel;
        }

        return enemies;
    }

    private EnemyDanger GetRandomValidMonster(int dangerLevel)
    {
        List<EnemyDanger> validMonsters = _enemies.Where(x => x.DangerLevel <= dangerLevel).ToList();
        return validMonsters[Random.Range(0, validMonsters.Count)];
    }

    #region Victory and UI
    public void DecreaseEnemyCount()
    {
        _enemyCount--;
        if(_enemyCount==0)
        {
            Victory();
        }
    }

    private void Victory()
    {
        InCombat = false;
        MapController.Instance.ClearCurrentRoom();
        //_currentBattle++;

        foreach (var unit in UnitSelectionManager.Instance.AllUnits)
        {
            unit.FullHeal();
        }

        _battleIntro.Victory(OnRegainControl: ()=>
        {
            MapController.Instance.SetCanMove(true);
            UnitSelectionManager.Instance.PauseUnitControl(false);
            HideStartBattleButton();
            MapController.Instance.UpdateBatlleCount(add: 1);
        });

        
    }

    private void StartButtonAnimation()
    {
        _sword.DOAnchorPosY(38.25F, 0.5f).SetEase(Ease.InBack).onComplete=()=>
        {
            _skull.DOAnchorPosY(3.3f, 0.3f).SetEase(Ease.OutBack,0.8f);
            _startButton.DOAnchorPosY(-24f, 0.3f).SetEase(Ease.OutCirc);
            _sword.DOAnchorPosY(17.35f, 0.3f).SetEase(Ease.OutBack);
        };
    }

    public void HideStartBattleButton()
    {
        _startButtonGroup.DOFade(0, 0.7f).SetEase(Ease.Flash, 15, 1)
                .onComplete = () =>
                {
                    _startButtonGroup.gameObject.SetActive(false);
                    _startButtonGroup.DOFade(1, 0);
                    _skull.DOAnchorPosY(24.33f, 0);
                    _startButton.DOAnchorPosY(0f, 0);
                    _sword.DOAnchorPosY(59.25f, 0);
                };
    }

    public (int,int) GetBattleReward()
    {
        var gold = Random.Range(50, 151);
        var keys = Random.Range(1, 101) <= 10 ? Random.Range(1, 101) <= 30 ? 2 : 1 : 0;

        return (gold, keys);
    }
    #endregion
}
