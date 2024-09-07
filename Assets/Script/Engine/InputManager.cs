using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour // 키보드 같은거 설정
{
    public static InputManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private static InputManager _instance;
 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
