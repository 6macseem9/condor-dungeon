using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class Healthbar : MonoBehaviour
{
    private Unit _unit;

    private SpriteRenderer _fg;
    private SpriteRenderer _bg;

    public float Percent { get; private set; }

    private Camera _cam;

    private void Start()
    {
        _unit = GetComponentInParent<Unit>();
        _cam = Camera.main;

        var sprites = GetComponentsInChildren<SpriteRenderer>();
        _fg = sprites[0];
        _bg = sprites[1];
    }

    private void Update()
    {
        Quaternion lookRotation = _cam.transform.rotation;
        transform.rotation = lookRotation;

        Percent = _unit.HP * 100 / _unit.Stats.MaxHP;
        _fg.size = new Vector2(Percent/100f, _fg.size.y);
    }

    public void FadeOut(bool fade = true)
    {
        _bg.DOFade(fade ? 0 : 1, 0);
    }
}
