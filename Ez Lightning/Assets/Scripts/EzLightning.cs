using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EzLightning : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float updateInterval = .03f;
    [SerializeField] float scrollSpeed = 1f;
    [SerializeField] float flickerFactor = .1f;

    [Tooltip("New point is added for per n unit distance traveled")]
    [SerializeField] int addPointPerNUnit = 5;
    [Tooltip("Only three points are used without distance checking ('per n unit' is ignored)")]
    [SerializeField] bool simpleLigtning = true;
    [Tooltip("Creates two child objects as start and end")]
    [SerializeField] bool controlWithChild = true;

    public Vector3 startPosition = new Vector3(0f, 0f, 0f);
    public Vector3 endPosition = new Vector3(0f, 0f, 1f);

    Transform startPoint;
    Transform endPoint;

    void OnEnable()
    {
        Init();
        StartCoroutine(UpdateLightning());
    }

    void Init()
    {
        if (controlWithChild)
        {
            Transform _startPoint = new GameObject("StartPoint").transform;
            Transform _endPoint = new GameObject("EndPoint").transform;

            _startPoint.SetParent(transform);
            _endPoint.SetParent(transform);

            startPoint = _startPoint;
            endPoint = _endPoint;

            endPoint.position = endPosition;
        }
    }

    IEnumerator UpdateLightning()   
    {
        WaitForSeconds wfs = new WaitForSeconds(updateInterval);
        float value = 0f;
        while (true)
        {
            value += scrollSpeed *  updateInterval;
            SetTextureOffset(value);

            if (value >= 1f)
                value = 0f;

            UpdateStartAndEndPositions();
            CalculatePoints();
            yield return wfs;
        }
    }

    void SetTextureOffset(float value)
    {
        lineRenderer.material.mainTextureOffset = new Vector2(value, 0f);
    }

    void CalculatePoints()
    {
        int remainingPoints = GetRemainingPointsByDistance();
        Vector3[] points = new Vector3[remainingPoints + 2];
        points[0] = startPosition;
        points[remainingPoints + 1] = endPosition;

        Vector3 offset = (endPosition - startPosition) / (remainingPoints + 1);
        for (int i = 1; i <= remainingPoints; i++)
        {
            points[i] = points[i - 1] + offset + GetRandomOffset();
        }

        int pointsLen = points.Length;
        lineRenderer.positionCount = pointsLen;
        for (int i = 0; i < pointsLen; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }

    //if controlling with cilds
    void UpdateStartAndEndPositions()
    {
        if (startPoint == null || endPoint == null)
            return;

        startPosition = startPoint.position;
        endPosition = endPoint.position;
    }

    //The part of finding the remaining points according to the distance without start and end
    int GetRemainingPointsByDistance()
    {
        if (simpleLigtning)
            return 1;

        int remainingPoints = Mathf.CeilToInt(Vector3.Distance(startPosition, endPosition) / addPointPerNUnit);
        remainingPoints = Mathf.Clamp(remainingPoints, 1, remainingPoints);
        return remainingPoints;
    }

    Vector3 GetRandomOffset()
    {
        return new Vector3(Random.Range(-flickerFactor, flickerFactor), Random.Range(-flickerFactor, flickerFactor), Random.Range(-flickerFactor, flickerFactor));
    }

}
