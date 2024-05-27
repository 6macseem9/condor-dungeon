using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescendTransition : MonoBehaviour
{
    [SerializeField] private GameObject _raycastBlock;
    [SerializeField] private RectTransform _animation;
    [SerializeField] private RectTransform _borders;
    private RectTransform _rectTransform;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void StartTransition(bool reset = false)
    {
        _raycastBlock.SetActive(true);

        _rectTransform.DOAnchorPos(Vector2.zero, 2f).SetEase(Ease.OutBack)
            .onComplete = () => MapController.Instance.Descend(reset);

        _rectTransform.DOAnchorPos(new Vector2(0,-1460), 2f).SetEase(Ease.InBack).SetDelay(2.5f);
        
        _animation.DORotate(new Vector3(0, 0, 8), 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        _borders.DORotate(new Vector3(0, 0, -32), 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        Util.Delay(4.5f, () =>
        {
            _raycastBlock.SetActive(false);

            _animation.DOKill();
            _borders.DOKill();
            _animation.DORotate(new Vector3(0, 0, -8), 0);
            _borders.DORotate(new Vector3(-0, 0, -72), 0);
            _rectTransform.DOAnchorPos(new Vector2(0, 1460), 0);
        });
    }
}
