using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class CircleRenderer : MonoBehaviour
{
    public int Steps;
    public float Radius;
    public Range _range;

    private LineRenderer _lineRenderer;
    private Vector3 _rotation;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        DrawCircle(Steps, _range.Radius + 0.3f);

        float width = _lineRenderer.startWidth;
        _lineRenderer.material.mainTextureScale = new Vector2(1f / width, 1.0f);

        _rotation = new Vector3(90, 0, 0);
    }
    private void Update()
    {

        transform.rotation = Quaternion.Euler(_rotation);
    }

    void DrawCircle(int steps, float radius)
    {
        _lineRenderer.positionCount = steps;

        for (int currentStep = 0; currentStep < steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep / (steps - 1);

            float currentRadian = circumferenceProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = radius * xScaled;
            float y = radius * yScaled;
            float z = 0;

            Vector3 currentPosition = new Vector3(x, y, z);

            _lineRenderer.SetPosition(currentStep, currentPosition);
        }
    }
}
