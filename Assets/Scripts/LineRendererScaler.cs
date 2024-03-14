using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererScaler : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        float width = _lineRenderer.startWidth;
        _lineRenderer.material.mainTextureScale = new Vector2(1f / width, 1.0f);
    }
}
