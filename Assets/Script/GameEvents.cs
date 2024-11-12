using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    // Start is called before the first frame update
    public static Action CheckIfShapeCanBePlaced;

    public static Action MoveShapeToStartPosition;

    // 퍼즐 재생성 위해서 변수 추가
    public static Action RequestNewShapes;

    public static Action SetShapeInactive;
}
