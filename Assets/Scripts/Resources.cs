using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Resources : MonoBehaviour
{
    public static Resources Instance;

    [SerializeField] private int _startingGold;

    [SerializeField] private RectTransform _gold;
    [SerializeField] private RectTransform _keys;

    private TextMeshProUGUI _goldText;
    private TextMeshProUGUI _keysText;

    private Vector2 _goldPos;
    private Vector2 _keysPos;

    public int Gold { get; private set; }
    public int Keys{ get; private set; }

    private Tweener _textTween;
    private int _visualGold;

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
        _goldText = _gold.GetComponentInChildren<TextMeshProUGUI>();
        _keysText = _keys.GetComponentInChildren<TextMeshProUGUI>();

        _goldPos = _gold.anchoredPosition;
        _keysPos = _keys.anchoredPosition;

        Gold = _startingGold;
        SetVisualGold(Gold);
    }

    public void AddGold(int amount)
    {
        Gold += amount;

        UpdateTexts();
        ShakeTransform(_gold, _goldPos);
    }
    public bool RemoveGold(int amount)
    {
        if (amount > Gold)
        {
            CursorController.Instance.NotEnoughResource(NotEnough.Gold);
            return false;
        }

        Gold -= amount;

        UpdateTexts();
        ShakeTransform(_gold, _goldPos);

        return true;
    }
    public void AddKeys(int amount)
    {
        Keys += amount;
        
        UpdateTexts();
        ShakeTransform(_keys, _keysPos);
    }
    public bool RemoveKeys(int amount)
    {
        if (amount > Keys)
        {
            CursorController.Instance.NotEnoughResource(NotEnough.Keys);
            return false;
        }

        Keys -= amount;

        UpdateTexts();
        ShakeTransform(_keys,_keysPos);

        return true;
    }

    private void UpdateTexts()
    {
        _keysText.text = (Keys < 10 ? "0" : "") + Keys;

        if (Gold==_visualGold) return;

        if (_textTween is not null && _textTween.IsPlaying())
        {
            _textTween.Kill();
        }

        int diff = Gold - _visualGold;
        _textTween = Util.Repeat(0.05f, 10, () =>
            {
                SetVisualGold(_visualGold + diff/10);
            });

        _textTween.onComplete = () =>
        {
            if (_visualGold != Gold)
                SetVisualGold(Gold);
        };

    }

    private void SetVisualGold(int gold)
    {
        _visualGold = gold;
        _goldText.text = $"<font=\"6pxBG2\"><mark=#222533> {_visualGold}</mark></font>";

    }

    private void ShakeTransform(RectTransform transform, Vector2 defPos)
    {
        transform.anchoredPosition = defPos;
        transform.DOShakeAnchorPos(0.3f, 1, 30).onComplete = () => { transform.anchoredPosition = defPos; };
    }

    public void ResetResources()
    {
        Gold = _startingGold;
        Keys = 0;
        UpdateTexts();
    }
}
