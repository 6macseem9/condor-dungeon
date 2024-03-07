using DG.Tweening;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    private Unit _unit;

    private SpriteRenderer _fg;
    private SpriteRenderer _bg;

    private void Start()
    {
        _unit = GetComponentInParent<Unit>();

        var sprites = GetComponentsInChildren<SpriteRenderer>();
        _fg = sprites[0];
        _bg = sprites[1];
    }

    private void Update()
    {
        Quaternion lookRotation = Camera.main.transform.rotation;
        transform.rotation = lookRotation;

        float percent = _unit.HP * 100 / _unit.MaxHP;
        _fg.size = new Vector2(percent/100f, _fg.size.y);
    }

    public void FadeOut()
    {
        _bg.DOFade(0, 0);
    }
}
