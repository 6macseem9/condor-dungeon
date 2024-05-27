using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private CanvasGroup _hud;
    [SerializeField] private DescendTransition _transition;
    [SerializeField] private CameraController _camera;
    [SerializeField] private Button _continueButton;
    [Space(7)]
    [SerializeField] private Material _material;

    private CanvasGroup _canvGroup;

    void Start()
    {
        _canvGroup = GetComponent<CanvasGroup>();

        _camera.transform.DORotate(new Vector3(0, -360, 0), 10, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame()
    {
        _transition.StartTransition( reset: true);
        Util.Delay(2f, () =>
        {
            _canvGroup.ShowCompletely(false);

            _camera.transform.DOKill();
            _camera.transform.DORotate(Vector3.zero, 0);
            _camera.ResetCamera();

            _hud.ShowCompletely(true);

            WinAndLoss.Instance.ResetUI();
            BattleController.Instance.ResetEverything();
            UnitSelectionManager.Instance.SpawnStartUnits();
            BattleIntroAndResults.Instance.ReturnToDefaultPositions();
            Inventory.Instance.ClearInventory();
            Resources.Instance.ResetResources();
        });
    }

    public void OpenPauseMenu(bool canContinue = true)
    {
        _background.enabled = true;

        _continueButton.interactable = canContinue;
        _continueButton.onClick.RemoveAllListeners();
        _continueButton.onClick.AddListener(ClosePauseMenu);

        _canvGroup.interactable = true;
        _canvGroup.blocksRaycasts = true;
        _canvGroup.DOFade(1, 1).SetEase(Ease.OutCirc);
    }

    private void ClosePauseMenu()
    {
        _canvGroup.interactable = false;

        _canvGroup.DOFade(0, 1).SetEase(Ease.OutCirc).onComplete = () =>
        {
            _canvGroup.blocksRaycasts = false;
        };
    }

    public void SetBGSpeed(float value)
    {
        _material.SetFloat("_Speed", value);
    }
    public void SetBGdistX(float value)
    {
        var vec = new Vector2(value, _material.GetVector("_Wackiness").y);
        _material.SetVector("_Wackiness",vec);
    }
    public void SetBGdistY(float value)
    {
        var vec = new Vector2(_material.GetVector("_Wackiness").x, value);
        _material.SetVector("_Wackiness", vec);
    }
}
