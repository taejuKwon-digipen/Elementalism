using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 스페이스바를 누르면 맵 씬으로 이동
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("게임 종료! 맵으로 돌아갑니다.");
            SceneManager.LoadScene("Map2"); 
        }
    }
}
