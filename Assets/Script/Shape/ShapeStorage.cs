// �� ��ũ��Ʈ�� ���ӿ��� ����� Shape(���� ����)���� �����ϰ� �����մϴ�.
// ShapeStorage Ŭ������ ShapeData�� Shape �ν��Ͻ����� ����Ʈ�� ������ ������,
// ���� ���� �� �� Shape�� ������ ShapeData�� �Ҵ��Ͽ� ���� ������ �����մϴ�.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData; // ���� ������ ShapeData�� ����Ʈ
    public List<Shape> shapeList;     // ���� ���� ��ġ�� Shape �ν��Ͻ����� ����Ʈ

    void Start()
    {
        // ������ ���۵� �� �� Shape�� ������ ShapeData�� �Ҵ��Ͽ� �����մϴ�.
        foreach (var shape in shapeList)
        {
            // shapeData ����Ʈ���� ������ �ε����� �����մϴ�.
            var shapeIndex = UnityEngine.Random.Range(0, shapeData.Count);
            // ���õ� ShapeData�� ����Ͽ� Shape�� �����մϴ�.
            shape.CreateShape(shapeData[shapeIndex]);
        }
    }
}
