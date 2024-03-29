using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class UnitVisuals : MonoBehaviour
{
    [SerializeField] private Material _flashMaterial;
    [field: SerializeField] public GameObject UiElements { get; private set; }
    public SpriteRenderer ActionMarker { get; private set; }
    private SpriteRenderer _selectionRing;
    private Transform _healthbar;
    private LineRenderer _rangeCircle;

    private MeshRenderer[] _meshes;
    private Material _defMaterial;

    private Tweener _selectBounce;
    private Tweener _markerSpin;
    private Tweener _markerSpinLock;
    private Tweener _markerBounce;
    private Tweener _shake;
    private Tweener _healthbarShake;

    private void Start()
    {
        _meshes = GetComponentsInChildren<MeshRenderer>();
        _defMaterial = _meshes[0].material;

        var sprites = GetComponentsInChildren<SpriteRenderer>();
        _selectionRing = sprites[1];
        ActionMarker = sprites[2];

        _healthbar = GetComponentInChildren<Healthbar>().transform;
        _rangeCircle = GetComponentInChildren<LineRenderer>();
        _markerSpinLock = ActionMarker.transform.DORotate(new Vector3(90, 0, 45), 0).SetLoops(-1);
        _markerSpinLock.Pause();
        _markerSpin = ActionMarker.transform.DORotate(new Vector3(90, 0, 360), 5, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        ShowUiElements(false);
    }
    private void Update()
    {
        //if(_rangeCircle.enabled && !UnitSelectionManager.Instance.SingleUnitSelected)
        //{
        //    _rangeCircle.enabled = false;
        //}
    }

    public void ShowUiElements(bool show)
    {
        UiElements.SetActive(show);

        //ShowRange(show);
    }
    public void BounceMarker()
    {
        ActionMarker.enabled = true;
        _markerBounce.Kill();
        _markerSpinLock.Pause();
        _markerSpin.Play();

        ActionMarker.transform.DOScale(1f, 0);
        ActionMarker.transform.DOScale(0.6f, 0.5f).SetEase(Ease.OutElastic);

        
    }
    public void TargetMarker()
    {
        _markerSpin.Pause();
        _markerSpinLock.Play();
        ActionMarker.enabled = true;

        _markerBounce.Kill();
        ActionMarker.transform.DOScale(0.6f, 0);
        _markerBounce = ActionMarker.transform.DOScale(1f, 0.3f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
    }
    public void BounceSelect()
    {
        _selectBounce.Kill();
        _selectBounce = _selectionRing.transform.DOScale(1.2f, 0.1f);
        _selectBounce.onComplete = () => _selectBounce = _selectionRing.transform.DOScale(1f, 0.5f).SetEase(Ease.OutElastic);
    }
    public void DamageImpact(Transform transform)
    {
        _shake.Complete();
        _shake = transform.DOShakeRotation(0.2f, 30, 20);
    }
    public void ShakeHealthbar()
    {
        _healthbarShake.Complete();
        _healthbarShake = _healthbar.DOShakePosition(0.2f, 0.25f, 50);
    }
    public void ShowRange(bool show)
    {
        if (show && UnitSelectionManager.Instance.SingleUnitSelected)
        {
            _rangeCircle.enabled = true;
            return;
        }
        
        _rangeCircle.enabled= false;
    }
    public void Flash()
    {
        ApplyMaterials(_flashMaterial);

        Util.Delay(0.1f, () =>
        {
            ApplyMaterials(_defMaterial);
        });
    }
    private void ApplyMaterials(Material mat)
    {
        foreach (var mesh in _meshes)
        {
            if (mesh.CompareTag("Ignore")) continue;
            mesh.material = mat;
        }
    }
}
