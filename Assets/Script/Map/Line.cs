using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Line : MonoBehaviour
{
    public Vector3 start;
    public Vector3 end;
    public int segments = 20;
    private LineRenderer line;
    private Vector3 normal;
    private readonly List<Vector3> points = new();

    private void Awake() {
        line = GetComponent<LineRenderer>();
        line.positionCount = segments;
        line.SetPosition(0, start);
        line.SetPosition(segments - 1, end);
    }

    // Start is called before the first frame update
    void Start()
    {
        normal = Vector3.Cross(start, end);
        CreateBezierCurve();
    }

    private void CreateBezierCurve() {
        Vector3 firstPoint = Vector3.Lerp(start, end, 0.33f);
        Vector3 secondPoint = Vector3.Lerp(start, end, 0.66f);
        int direction = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;

        firstPoint += normal * (direction * 100);
        firstPoint.z = 0;
        direction *= -1;
        secondPoint += normal * (direction * 100);
        secondPoint.z = 0;
        points.Add(start);
        points.Add(firstPoint);
        points.Add(secondPoint);
        points.Add(end);

        for (int i = 0; i < segments; i += 1) {
            float t = i / (float)segments;
            line.SetPosition(i, CalculateBezierPoint(t));
        }
    }

    private Vector3 CalculateBezierPoint(float t) {
        Vector3 results = Vector3.zero;

        for (int i = 0; i < points.Count; i += 1)
            results += points[i] * BersteinPolynome(i, points.Count - 1, t);
        return results;
    }

    private float BersteinPolynome(int i, int n, float t) {
        return BinomialCoefficient(n, i) * math.pow(t, i) * math.pow(1 - t, n - i);
    }

    private float BinomialCoefficient(int n, int i) {
        return Factorial(n) / (Factorial(i) * Factorial(n - i));
    }

    private int Factorial(int number) {
        int result = 1;

        if (number == 0)
            return 1;
        for (int i = 1; i <= number; i += 1)
            result *= i;
        return result;
    }
}
