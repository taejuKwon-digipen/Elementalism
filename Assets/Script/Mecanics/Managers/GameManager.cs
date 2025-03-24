using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int Player_HP;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Player.inst != null)
        {
            Player_HP = Player.inst.HP;
        }
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
