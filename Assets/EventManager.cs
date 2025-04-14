using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class EventManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button Select_Button_1;
    [SerializeField] private Button Select_Button_2;
    [SerializeField] private Button Select_Button_3;
    // Start is called before the first frame update
    void Start()
    {
        Select_Button_1.onClick.AddListener(SelectButton);
        Select_Button_2.onClick.AddListener(SelectButton);
        Select_Button_3.onClick.AddListener(SelectButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SelectButton()
    {
        SceneManager.LoadScene("Map2");
    }    
}
