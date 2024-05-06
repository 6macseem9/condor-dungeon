using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]
public class NexusWindow : MonoBehaviour
{
    [SerializeField] private StructureUnit _nexus;
    [SerializeField] private int _maxUnits;
    [SerializeField] private Unit[] _units;
    [SerializeField] private Vector3[] _spawnPositions;
    [SerializeField] private Image _healthbar;
    [SerializeField] private Tooltip _healthTooltip;
    [SerializeField] private TextMeshProUGUI _availableText;

    private CanvasGroup _canvGroup;
    private UnitSpawner[] _spawners;
    private int _posIndex = -1;

    private void Start()
    {
        _spawners = GetComponentsInChildren<UnitSpawner>();
        _canvGroup = GetComponent<CanvasGroup>();

        for(int i =0;i<_spawners.Length;i++)
        {
            _spawners[i].SetUnit(_units[i]);
        }

        Show(false);
        _availableText.text = _maxUnits + "/" + _maxUnits;
    }
    private void Update()
    {
        if (_canvGroup.alpha == 0) return;

        _healthbar.rectTransform.DOScaleX((_nexus.Healthbar.Percent / 100) * 3, 0);
        _healthTooltip.Description = _healthTooltip.Description.Substring(0,16) + _nexus.HP + "/" + _nexus.Stats.MaxHP;

        _availableText.text = AvailableUnits() + "/" + _maxUnits;
        if (AvailableUnits() == 0) _availableText.text = $"<color=#db4b3d>{_availableText.text}</color>";
    }
    public void Spawn(Unit unit)
    {
        _availableText.rectTransform.DOComplete();
        _availableText.rectTransform.DOShakeAnchorPos(0.3f, 4, 20);

        if (AvailableUnits() == 0) return;

        var instance = Instantiate(unit, GetNextPosition(), Quaternion.identity);
        instance.Spawn();
    }

    private int AvailableUnits()
    {
        return _maxUnits - UnitSelectionManager.Instance.AllUnits.Count;
    }

    public void Show(bool show)
    {
        _canvGroup.alpha = show ? 1 : 0;
        _canvGroup.blocksRaycasts = show;
        _canvGroup.interactable = show;

        if (show)
        {
            _canvGroup.transform.DOScale(1, 0.3f).SetEase(Ease.OutElastic, 1);
        }
        if (!show && _canvGroup.transform.localScale != Vector3.zero)
        {
            _canvGroup.transform.DOScale(0, 0f);
        }
    }

    private Vector3 GetNextPosition()
    {
        _posIndex++;
        if(_posIndex >= _spawnPositions.Length) _posIndex = 0;

        Vector3 random = new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));

        return _nexus.transform.position + _spawnPositions[_posIndex] + random;
    }
}
