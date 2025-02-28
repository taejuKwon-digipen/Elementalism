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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("���� ����! ������ ���ư��ϴ�.");

            if (MapManager.Instance != null)
            {
                Debug.Log("Main ������ MapManager�� ��� ����!");
            }
            else
            {
                Debug.Log("Main ������ MapManager�� �����!");
            }

            SceneManager.LoadScene("Map2");

            // �� ���� �� MapManager ���� Ȯ��
            if (MapManager.Instance != null)
            {
                Debug.Log("�� ���� �Ŀ��� MapManager�� ������!");
            }
        }
    }


}
