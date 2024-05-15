using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public static BattleController Instance;

    [SerializeField] private BattleSequence _battleSequence;
    [SerializeField] private Vector3[] _spawnPositions;
    [Space(5)]
    [SerializeField] private CanvasGroup _startButtonGroup;
    [SerializeField] private RectTransform _startButton;
    [SerializeField] private RectTransform _skull;
    [SerializeField] private RectTransform _sword;
    [Space(5)]
    [SerializeField] private BattleIntro _battleIntro;
    
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
        StartButtonAnimation();
        UnitSelectionManager.Instance.PauseUnitControl(true);
        UnitSelectionManager.Instance.StopAllUnits();

        foreach (var spawner in _spawners)
            spawner.ShowIcon(false);

        var battle = _battleSequence.Sequence[_currentBattle];
        _enemyCount = battle.Amount;

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
            {
                ShowSpawners(false);
                loop.Kill();
            }
        };
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

    private void ShowSpawners(bool show)
    {
        foreach (Spawner spawner in _spawners)
        {
            spawner.gameObject.SetActive(show);
            if(show==false) spawner.ShowIcon(true);
        }
    }

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
        MapController.Instance.ClearCurrentRoom();
        _battleIntro.Victory(OnRegainControl: ()=>
        {
            MapController.Instance.SetCanMove(true);
            UnitSelectionManager.Instance.PauseUnitControl(false);
            HideStartBattleButton();
            MapController.Instance.UpdateBatlleCount(add: 1);
        });

        foreach (var unit in UnitSelectionManager.Instance.AllUnits)
        {
            unit.FullHeal();
        }
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
}
