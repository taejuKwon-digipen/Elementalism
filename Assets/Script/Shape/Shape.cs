// �� ��ũ��Ʈ�� ���ӿ��� ���Ǵ� ���� ����(Shape)�� ������ �����մϴ�.
// ���콺 �Է°� �巡�� �̺�Ʈ�� ó���Ͽ� ���� ������ ����, �̵�, ��ġ�� �� �ְ� �մϴ�.
// ShapeData�� ������� ���� ������ ���� ��ġ�� �����մϴ�.

using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerDownHandler
{
    public GameObject squareShapeImage;      // ���� ������ ���� ��� �̹���
    public Vector3 shapeSelectedScale;       // ���õ� ������ ������
    public Vector2 offset = new Vector2(0f, 700f);  // �巡�� �� ��ġ ������ ������

    [HideInInspector]
    public ShapeData CurrentShapeData;       // ���� Shape�� ������

    private List<GameObject> _currentShape = new List<GameObject>();  // ���� Shape�� �����ϴ� ��ϵ��� ����Ʈ
    private Vector3 _shapeStartScale;        // �ʱ� ������ ��
    private RectTransform _transform;        // RectTransform ������Ʈ
    private bool _shapeDraggable = true;     // �巡�� ���� ����
    private bool _isDragging = false;        // ���� �巡�� ������ ����
    private Canvas _canvas;                  // �θ� ĵ����

    public void Awake()
    {
        _shapeStartScale = this.GetComponent<RectTransform>().localScale; // �ʱ� ������ ����
        _transform = this.GetComponent<RectTransform>();                  // RectTransform ��������
        _canvas = GetComponentInParent<Canvas>();                         // �θ� ĵ���� ��������
        _shapeDraggable = true;                                           // �巡�� ���� ����
    }

    void Start()
    {
        // �ʱ�ȭ�� �ʿ��� ������ ������ ���⿡ �ۼ�
    }
    void Update()
    {
        // ���� �巡�� ���� ���� ȸ��
        if (_isDragging)
        {
            // Q Ű�� ������ �������� ȸ��
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RotateShape(-90); // �������� 90�� ȸ��
            }
            // E Ű�� ������ ���������� ȸ��
            if (Input.GetKeyDown(KeyCode.E))
            {
                RotateShape(90);  // ���������� 90�� ȸ��
            }
        }
    }

    // ���ο� Shape ��û�� ó���ϴ� �޼���
    public void RequestNewShape(ShapeData shapeData)
    {
        CreateShape(shapeData); // ���ο� Shape ����
    }

    // Shape�� �����ϴ� �޼���
    public void CreateShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;  // ���� ShapeData ����
        var totalSquareNumber = GetNumberOfSquares(shapeData); // �ʿ��� ��� �� ���

        // �ʿ��� ��� ����ŭ ����Ʈ�� �߰�
        while (_currentShape.Count <= totalSquareNumber)
        {
            _currentShape.Add(Instantiate(squareShapeImage, transform) as GameObject);
        }

        // ��� �ʱ�ȭ
        foreach (var square in _currentShape)
        {
            square.gameObject.transform.localPosition = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRect = squareShapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x,
            squareRect.rect.height * squareRect.localScale.y);
        int currentIndexInList = 0;

        // ShapeData�� ������� ��ϵ��� ��ġ ����
        for (var row = 0; row < shapeData.rows; row++)
        {
            for (var column = 0; column < shapeData.columns; column++)
            {
                ElementType elementType = shapeData.board[row].colum[column];
                if (elementType != ElementType.None)
                {
                    _currentShape[currentIndexInList].SetActive(true);
                    _currentShape[currentIndexInList].GetComponent<RectTransform>().localPosition =
                        new Vector2(GetXPositionForShapeSquare(shapeData, column, moveDistance),
                        GetYPositionForShapeSquare(shapeData, row, moveDistance));

                    var shapeSquare = _currentShape[currentIndexInList].GetComponent<ShapeSquare>();
                    shapeSquare.SetElementType(elementType);

                    currentIndexInList++;
                    
                }
            }
        }

    }

    // Shape�� ��ϵ��� Y ��ġ�� ����ϴ� �޼���
    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        float shiftOnY = 0f;

        if (shapeData.rows > 1)
        {
            if (shapeData.rows % 2 != 0) // ���� ���� Ȧ���� ���
            {
                var middleSquareIndex = (shapeData.rows - 1) / 2;
                var multiplier = row - middleSquareIndex;
                shiftOnY = -moveDistance.y * multiplier;
            }
            else // ���� ���� ¦���� ���
            {
                var middleSquareIndex = shapeData.rows / 2;
                var multiplier = row - (middleSquareIndex - 0.5f);
                shiftOnY = -moveDistance.y * multiplier;
            }
        }
        return shiftOnY;
    }

    // Shape�� ��ϵ��� X ��ġ�� ����ϴ� �޼���
    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        float shiftOnX = 0f;

        if (shapeData.columns > 1)
        {
            if (shapeData.columns % 2 != 0) // ���� ���� Ȧ���� ���
            {
                var middleSquareIndex = (shapeData.columns - 1) / 2;
                var multiplier = column - middleSquareIndex;
                shiftOnX = moveDistance.x * multiplier;
            }
            else // ���� ���� ¦���� ���
            {
                var middleSquareIndex = shapeData.columns / 2;
                var multiplier = column - (middleSquareIndex - 0.5f);
                shiftOnX = moveDistance.x * multiplier;
            }
        }
        return shiftOnX;
    }

    // ShapeData���� Ȱ��ȭ�� ����� ���� ����ϴ� �޼���
    private int GetNumberOfSquares(ShapeData shapeData)
    {
        int number = 0;
        foreach (var rowData in shapeData.board)
        {
            foreach (var elementType in rowData.colum)
            {
                if (elementType != ElementType.None)
                    number++;
            }
        }
        return number;
    }

    // ���콺 Ŭ�� �̺�Ʈ ó�� (�ʿ� �� ����)
    public void OnPointerClick(PointerEventData eventData)
    {

    }

    // ���콺 ��ư�� ���� �� �̺�Ʈ ó�� (�ʿ� �� ����)
    public void OnPointerUp(PointerEventData eventData)
    {
        foreach (var square in _currentShape)
        {
            Block block = square.GetComponent<Block>();

            if (block != null && block.currentCollidedBlock != ElementType.None)
            {
                // �浹�� ���� Ÿ������ �̹��� ����
                //block.SetBlockImage(block.currentCollidedBlock);
                block.SetElementType(block.currentCollidedBlock);
            }
        }

        this.GetComponent<RectTransform>().localScale = _shapeStartScale; // �������� ������� ����
        GameEvents.CheckIfShapeCanBePlaced(); // Shape�� ��ġ�� �� �ִ��� üũ�ϴ� �̺�Ʈ ȣ��
    }

    // �巡�� ���� �� ȣ��Ǵ� �޼���
    public void OnBeginDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = shapeSelectedScale; // ���õ� ���·� ������ ����
        _isDragging = true; // �巡�� �� ���� ����
    }

    // �巡�� �߿� ȣ��Ǵ� �޼���
    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchorMin = new Vector2(0, 0);
        _transform.anchorMax = new Vector2(0, 0);
        _transform.pivot = new Vector2(0, 0);

        Vector2 pos;
        // ȭ�� ��ǥ�� ĵ������ ���� ��ǥ�� ��ȯ�Ͽ� ��ġ ����
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
            eventData.position, Camera.main, out pos);
        _transform.localPosition = pos + offset;
    }

    // �巡�� ���� �� ȣ��Ǵ� �޼���
    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = _shapeStartScale; // �������� ������� ����
        _isDragging = false; // �巡�� �� ���� ����
        GameEvents.CheckIfShapeCanBePlaced(); // Shape�� ��ġ�� �� �ִ��� üũ�ϴ� �̺�Ʈ ȣ��
    }
    
    // ����� ȸ����Ű�� �޼���
    private void RotateShape(float angle)
    {
        _transform.Rotate(0, 0, angle); // RectTransform�� �������� Z�� ȸ��
    }

    // ���콺 ��ư�� ������ �� �̺�Ʈ ó�� (�ʿ� �� ����)
    public void OnPointerDown(PointerEventData eventData)
    {

    }
}
