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
        // �����̽��ٸ� ������ �� ������ �̵�
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("���� ����! ������ ���ư��ϴ�.");
            SceneManager.LoadScene("Map2"); 
        }
    }
}
