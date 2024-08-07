using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class FloorBattle
{
    [HideInInspector] public string Name = "Floor";

#if UNITY_EDITOR
    [NamedArrayAttribute("")]
#endif
    public Unit[] Enemies;
    public Vector2Int EnemyLevels;

#if UNITY_EDITOR
    [NamedArrayAttribute("Danger")]
#endif
    public int[] Battles;

    public int SkipChance;
    public float SpawnInterval;
}


public class BattleController : MonoBehaviour
{
    public static BattleController Instance;

    [SerializeField] private FloorBattle[] _floorBattles;
    [Space(5)]
    [SerializeField] private Vector3[] _spawnPositions;
    [Space(5)]
    [SerializeField] private CanvasGroup _startButtonGroup;
    [SerializeField] private RectTransform _startButton;
    [SerializeField] private RectTransform _skull;
    [SerializeField] private RectTransform _sword;
    [Space(5)]
    [SerializeField] private BattleIntroAndResults _battleIntro;

    public int CurrentBattle { get; set; }
    public bool InCombat { get; private set; }
    public int MaxFloors { get { return _floorBattles.Length; } }
    private FloorBattle _floor { get { return _floorBattles[MapController.Instance.CurrentFloor-1]; } }


    private Spawner[] _spawners;
    private int _enemyCount;
    private Tweener _loop;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else Instance = this;
    }
    private void OnValidate()
    {
        for(int i =0;i<_floorBattles.Length;i++) 
        {
            _floorBattles[i].Name = $"Floor {i+1}";
        }
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
        var spawner = _spawners.RandomChoice();

        var instance = Instantiate(unit, spawner.SpawnPoint, Quaternion.Euler(0,180,0));
        instance.Spawn();

        Util.DelayOneFrame(() => 
        {
            int levels = Random.Range(_floor.EnemyLevels.x-1, _floor.EnemyLevels.y);
            instance.GetComponent<Enemy>().LevelUp(levels);
            instance.FullHeal();
        });
        
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
        //UnitSelectionManager.Instance.StopAllUnits();

        foreach (var spawner in _spawners)
            spawner.ShowIcon(false);

        var danger = _floor.Battles[CurrentBattle];
        var enemies = ChooseEnemiesForBattle(danger).OrderBy(x=>Random.value).ToList();
        _enemyCount = enemies.Count;

        var interval = _floor.SpawnInterval;
        var skip = _floor.SkipChance;

        var step = -1;
        var skips = 0;
        _loop = Util.Repeat(interval, -1, () => { });
        _loop.onStepComplete = () =>
        {
            if(step!=0 && skips<2 && Random.Range(1,101) <= skip)
            {
                skips++;
                return;
            }
            step++;
            skips = 0;
            RandomSpawn(enemies[step]);

            if(step == enemies.Count-1)
            {
                ShowSpawners(false);
                _loop.Kill();
            }
        };
    }
    public List<Unit> ChooseEnemiesForBattle(int danger)
    {
        List<Unit> enemies = new List<Unit>();
        int remainingDanger = danger;

        while (remainingDanger > 0)
        {
            Unit enemy = GetRandomValidMonster(remainingDanger);
            enemies.Add(enemy);
            remainingDanger -= (enemy.Class as EnemyClass).DangerLevel;
        }

        return enemies;
    }

    private Unit GetRandomValidMonster(int dangerLevel)
    {
        List<Unit> validMonsters = _floor.Enemies.Where(x => (x.Class as EnemyClass).DangerLevel <= dangerLevel).ToList();
        return validMonsters.RandomChoice();
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
        CurrentBattle++;

        UnitSelectionManager.Instance.FullHeal();
        UnitSelectionManager.Instance.ReturnUnitsToPositions();

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
        var danger = _floor.Battles[CurrentBattle-1];
        var tenPercent = (int)(danger * 0.1f);
        var gold = Random.Range(danger - tenPercent, danger + tenPercent);
        var keys = Random.Range(1, 101) <= 10 ? Random.Range(1, 101) <= 30 ? 2 : 1 : 0;

        return (gold, keys);
    }

    public void ResetEverything()
    {
        HideStartBattleButton();
        MapController.Instance.SetCanMove(true);
        UnitSelectionManager.Instance.PauseUnitControl(false);
        InCombat = false;

        ShowSpawners(false);
        _loop.Kill();

        CurrentBattle = 0;
    }
    #endregion
}
