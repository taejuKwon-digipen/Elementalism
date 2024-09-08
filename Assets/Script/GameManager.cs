using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

   private static GameManager _instance;

    public bool IsGameOver;
    public GameObject GameOverUI;
    private bool _LoadComplete = false;
    private float _LoadProgress = 0f; //로딩화면 만들때

    void Init()
    {

    }

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update

    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        // Destroy / Dontdestroy 적극 활용
    }
}
