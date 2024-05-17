using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleIntroAndRewards : MonoBehaviour
{
    public static BattleIntroAndRewards Instance;

    [SerializeField] private Image _text;
    [SerializeField] private Image _topSword;
    [SerializeField] private Image _botSword;
    [SerializeField] private RectTransform _swordGroup;
    [Space(5)]
    [SerializeField] private Sprite _victoryText;
    [SerializeField] private Sprite _victorySword;
    private Sprite _battleText;
    private Sprite _battleSword;

    [Space(5)]
    [SerializeField] private RectTransform _resutls;
    [SerializeField] private Image _keysImage;
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private TextMeshProUGUI _keysText;
    [SerializeField] private TextMeshProUGUI _darkGoldText;
    [SerializeField] private TextMeshProUGUI _darkKeysText;

    private Vector2 _goldPos;
    private Vector2 _keysPos;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else Instance = this;
    }
    void Start()
    { 
        _battleText = _text.sprite;
        _battleSword = _topSword.sprite;

        _goldPos = _goldText.rectTransform.anchoredPosition;
        _keysPos = _keysText.rectTransform.anchoredPosition;
    }


    public void Intro()
    {
        _text.sprite = _battleText;
        _topSword.sprite = _battleSword;
        _botSword.sprite = _battleSword;
        _swordGroup.gameObject.SetActive(true);

        Util.Delay(1.4f, () =>
        {
            AnimateSwordSpread(_topSword.rectTransform, 1);
            AnimateSwordSpread(_botSword.rectTransform, -1);

            _text.DOFade(0, 0.1f).SetDelay(0.4f);
        });

        
    }

    private void AnimateSwordSpread(RectTransform sword, int mod)
    {
        sword.DORotate(new Vector3(0, 0, 13), 0.5f).SetEase(Ease.OutBack).onComplete = () =>
        {
            sword.DORotate(new Vector3(0, 0, -270), 1f, RotateMode.FastBeyond360);
            sword.DOAnchorPosX(320*mod, 1.8f);
        };

    }
    public void Victory(TweenCallback OnRegainControl)
    {
        _text.sprite = _victoryText;
        _topSword.sprite = _victorySword;
        _botSword.sprite = _victorySword;

        var rewards = BattleController.Instance.GetBattleReward();
        var gold = rewards.Item1;
        var keys = rewards.Item2;

        _goldText.text = "#" + gold;
        _darkGoldText.text = gold.ToString();
        _keysText.text = _keysText.text.Substring(0,1) + keys;
        _darkKeysText.text = keys == 0 ? "" : keys.ToString();
        _keysImage.enabled = keys != 0;

        Util.Delay(1.8f, () =>
        {
            _text.DOFade(1, 0);
            _resutls.gameObject.SetActive(true);

            _goldText.enabled = true;
            _keysText.enabled = keys != 0;
            _goldText.rectTransform.DOAnchorPos(_goldPos, 0);
            _keysText.rectTransform.DOAnchorPos(_keysPos, 0);
        });
        AnimateSwordClash(_topSword.rectTransform, 1);
        AnimateSwordClash(_botSword.rectTransform, -1).
        onComplete= () =>
        {
            _swordGroup.gameObject.SetActive(false);
            _swordGroup.DOAnchorPosY(0, 0);

            RecieveGold(_goldPos, gold);
            if (keys != 0) Util.Delay(0.5f, () => RecieveKeys(_keysPos, keys));

            Util.Delay(2, () =>
            {
                Util.Delay(0.3f, OnRegainControl);

                _resutls.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
                .onComplete = () =>
                {
                    _resutls.gameObject.SetActive(false);
                    _resutls.transform.DOScale(new Vector3(3,3,3), 0);
                };
            });
            
        };
    }
    private Tweener AnimateSwordClash(RectTransform sword, int mod)
    {
        sword.DORotate(new Vector3(0, 0, 733), 1.8f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
        sword.DOAnchorPosX(95.8f * mod, 1.8f).SetEase(Ease.InQuad).onComplete = () =>
        {
            sword.DORotate(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.OutBack);
        };

        return _swordGroup.DOAnchorPosY(750, 1f).SetEase(Ease.InBack).SetDelay(3.3f);
    }


    public void RecieveGold(Vector2 startPos, int gold=0)
    {
        _goldText.rectTransform.DOAnchorPos(startPos, 0);
        _goldText.transform.DOScale(3.5f, 0.3f).SetLoops(2, LoopType.Yoyo).SetDelay(0.25f).SetEase(Ease.OutCirc).onComplete = () =>
                _goldText.rectTransform.DOAnchorPos(new Vector2(595, 444), 0.5f).SetDelay(0.25f).SetEase(Ease.OutCubic).onComplete = () =>
                {
                    _goldText.enabled = false;
                    Resources.Instance.AddGold(gold);
                };
    }

    public void RecieveKeys(Vector2 startPos, int keys = 0)
    {
        _keysText.rectTransform.DOAnchorPos(startPos, 0);
        _keysText.transform.DOScale(3.5f, 0.3f).SetLoops(2, LoopType.Yoyo).SetDelay(0.25f).SetEase(Ease.OutCirc).onComplete = () =>
                _keysText.rectTransform.DOAnchorPos(new Vector2(673, 409), 0.5f).SetDelay(0.25f).SetEase(Ease.OutCubic).onComplete = () =>
                {
                    _keysText.enabled = false;
                    Resources.Instance.AddKeys(keys);
                };
    }

    public void PopOutReward(int gold = 0, int keys = 0)
    {
        TextMeshProUGUI text = _goldText;
        if (keys != 0) text = _keysText;

        text.transform.DOMove(CursorController.Instance.transform.position, 0);
        text.transform.DOScale(0, 0);

        if (gold != 0) text.text = "#" + gold;
        if (keys != 0) text.text = _keysText.text.Substring(0, 1) + keys;

        text.enabled = true;
        text.transform.DOScale(3, 0.4f);
        text.transform.DOMove(CursorController.Instance.transform.position + new Vector3(0, 75, 0), 0.4f);

        Util.Delay(0.15f, () => 
        {
            if (gold!=0) RecieveGold(text.rectTransform.anchoredPosition, gold);
            if (keys != 0) RecieveKeys(text.rectTransform.anchoredPosition, keys);
        }
        );
    }
}
