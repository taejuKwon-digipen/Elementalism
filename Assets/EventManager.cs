using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] public GameObject Button; // 버튼 프리팹
    public Canvas canvas;

    // 버튼 텍스트 목록
    private List<string> buttonTexts = new List<string>
    {
        "Effect 1",
        "Effect 2",
        "Effect 3",
        "Effect 4",
        "Effect 5"
    };

    // 선택된 텍스트를 저장할 리스트
    private List<string> selectedTexts = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        // 버튼 텍스트를 랜덤하게 선택하여 두 개의 버튼 생성
        CreateButton(new Vector2(0, -310));
        CreateButton(new Vector2(0, -450));
    }

    void CreateButton(Vector2 position)
    {
        // 랜덤 텍스트 선택
        string randomText;
        do
        {
            randomText = buttonTexts[Random.Range(0, buttonTexts.Count)];
        } while (selectedTexts.Contains(randomText)); // 이미 선택된 텍스트가 있는지 확인

        // 버튼 생성
        GameObject ButtonObj = Instantiate(Button, canvas.transform);
        Button button = ButtonObj.GetComponent<Button>();

        // 버튼 텍스트 설정
        TextMeshProUGUI buttonText = ButtonObj.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = randomText;

        // 버튼 위치 설정
        RectTransform rectTransform = ButtonObj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position; // 지정된 위치로 설정

        // 버튼 클릭 이벤트 추가
        button.onClick.AddListener(() => OnButtonClick(randomText));

        // 선택된 텍스트를 리스트에 추가
        selectedTexts.Add(randomText);
    }

    void OnButtonClick(string buttonText)
    {
        // 버튼 텍스트에 따라 다른 효과 실행
        switch (buttonText)
        {
            case "Effect 1":
                Debug.Log("Effect 1 activated!");
                // Effect 1 관련 코드
                GameManager.Instance.Player_HP -= 20;

                SceneManager.LoadScene("Map2");
                break;
            case "Effect 2":
                Debug.Log("Effect 2 activated!");
                // Effect 2 관련 코드

                SceneManager.LoadScene("Map2");
                break;
            case "Effect 3":
                Debug.Log("Effect 3 activated!");
                // Effect 3 관련 코드
                
                SceneManager.LoadScene("Map2");
                break;
            case "Effect 4":
                Debug.Log("Effect 4 activated!");
                // Effect 4 관련 코드

                SceneManager.LoadScene("Map2");
                break;
            case "Effect 5":
                Debug.Log("Effect 5 activated!");
                // Effect 5 관련 코드

                SceneManager.LoadScene("Map2");
                break;
            default:
                Debug.Log("No effect associated with this button.");
                break;
        }
    }
}