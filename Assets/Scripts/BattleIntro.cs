using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleIntro : MonoBehaviour
{
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
    private TextMeshProUGUI _goldText;
    private TextMeshProUGUI _darkGoldText;
    private TextMeshProUGUI _keysText;
    private TextMeshProUGUI _darkKeysText;


    void Start()
    { 
        _battleText = _text.sprite;
        _battleSword = _topSword.sprite;

        var texts = _resutls.GetComponentsInChildren<TextMeshProUGUI>();
        _darkGoldText = texts[0];
        _darkKeysText = texts[1];
        _goldText = texts[2];
        _keysText = texts[3];

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

        var gold = BattleController.Instance.GetBattleReward().Item1;
        var keys = BattleController.Instance.GetBattleReward().Item2;

        _goldText.text = gold.ToString();
        _darkGoldText.text = gold.ToString();
        _keysText.text = keys==0 ? "" : keys.ToString();
        _darkKeysText.text = keys == 0 ? "" : keys.ToString();
        _keysImage.enabled = keys != 0;

        AnimateSwordClash(_topSword.rectTransform, 1);
        AnimateSwordClash(_botSword.rectTransform, -1).
        onComplete= () =>
        {
            _swordGroup.gameObject.SetActive(false);
            _swordGroup.DOAnchorPosY(0, 0);

            MoveRewardText(_goldText, new Vector2(202.5f, 148f),()=>Resources.Instance.AddGold(gold));
            if (keys != 0) Util.Delay(0.5f,()=> MoveRewardText(_keysText, new Vector2(225.8f, 134.8f), () => Resources.Instance.AddKeys(keys)));

            Util.Delay(2, () =>
            {
                Util.Delay(0.3f, OnRegainControl);

                _goldText.enabled = false;
                _keysText.enabled = false;

                _resutls.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack)
                .onComplete = () =>
                {
                    _resutls.gameObject.SetActive(false);
                    _goldText.enabled = true;
                    _keysText.enabled = true;
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

            _text.DOFade(1, 0);
            _resutls.gameObject.SetActive(true);
        };

        return _swordGroup.DOAnchorPosY(750, 1f).SetEase(Ease.InBack).SetDelay(3.3f);


    }

    private void MoveRewardText(TextMeshProUGUI text,Vector2 pos, TweenCallback onComplete)
    {
        var startPos = text.rectTransform.anchoredPosition;
        text.transform.DOScale(1.2f, 0.3f).SetLoops(2, LoopType.Yoyo).SetDelay(0.25f).SetEase(Ease.OutCirc).onComplete = () =>
                text.rectTransform.DOAnchorPos(pos, 0.5f).SetDelay(0.25f).SetEase(Ease.OutCubic).onComplete = () =>
                {
                    text.enabled = false;
                    text.rectTransform.DOAnchorPos(startPos, 0);
                    onComplete.Invoke();
                };
    }
}
