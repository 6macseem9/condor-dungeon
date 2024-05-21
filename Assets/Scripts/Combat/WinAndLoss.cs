using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinAndLoss : MonoBehaviour
{

    public static WinAndLoss Instance;

    [SerializeField] private GameObject _raycastBlock;
    [SerializeField] private MainMenu _menu;
    [Space(7)]
    [SerializeField] private Image _cleared;
    [Space(7)]
    [SerializeField] private CanvasGroup _youDiedGroup;
    [SerializeField] private Image _skull;
    [SerializeField] private Image _topSword;
    [SerializeField] private Image _sideSword;
    [SerializeField] private Image _you;
    [SerializeField] private Image _died;
    [SerializeField] private TextMeshProUGUI _diedText;

    private RectTransform _youDiedTransform;

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
        _youDiedTransform = _youDiedGroup.GetComponent<RectTransform>();
    }

    public void YouDied()
    {
        _raycastBlock.SetActive(true);
        _youDiedGroup.alpha = 1;

        _skull.DOFade(1, 0.7f).SetEase(Ease.Flash, 15, 1);

        Util.Delay(1f, () =>
        {
            _topSword.rectTransform.DOAnchorPos(new Vector2(0,105),0.5f).SetEase(Ease.OutBack,1);

            _youDiedTransform.DOAnchorPos(new Vector2(-30, 0), 0.3f).SetEase(Ease.OutBack,10).SetDelay(0.2f);

            Util.Delay(0.2f, () =>
            {
                _you.enabled = true;
                _you.transform.DOShakePosition(0.2f, 5, 30);
            });
        });

        Util.Delay(1.5f, () =>
        {
            _sideSword.rectTransform.DOAnchorPos(new Vector2(-95, 6), 0.5f).SetEase(Ease.OutBack, 1);

            _youDiedTransform.DOAnchorPos(new Vector2(0, 0), 0.3f).SetEase(Ease.OutBack, 10).SetDelay(0.2f);

            Util.Delay(0.2f, () =>
            {
                _died.enabled = true;
                _died.transform.DOShakePosition(0.2f, 5, 30);
            });
        });

        Util.Delay(2.1f, () =>
        {
            _raycastBlock.SetActive(false);
            _menu.OpenPauseMenu(canContinue: false);

            _diedText.text = "ON FLOOR " + MapController.Instance.CurrentFloor;
            _diedText.DOFade(1, 0.5f).SetDelay(1);
        });
    }

    public void DungeonCleared()
    {
        _cleared.enabled = true;

        _menu.OpenPauseMenu(canContinue: false);
    }

    public void ResetUI()
    {
        _youDiedGroup.alpha = 0;
        _skull.DOFade(0, 0);
        _topSword.rectTransform.anchoredPosition = new Vector2(0, 432);
        _sideSword.rectTransform.anchoredPosition = new Vector2(-468, 6);
        _you.enabled = false;
        _died.enabled = false;
        _diedText.DOFade(0, 0);

        _cleared.enabled = false;
    }
}
