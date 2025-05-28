using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Linq 사용을 위해 추가
// Easy Save 3의 핵심 클래스 ES3는 보통 전역적으로 접근 가능하므로 별도의 using 지시문이 필요 없을 수 있습니다.
// 문제가 발생하면 Easy Save 3 문서를 참조하여 정확한 사용법을 확인하세요.

[System.Serializable]
public struct CardSaveData // 카드 저장용 구조체
{
    public string cardName; // 또는 카드 ID (CardItem의 식별자)
    public bool isUnlocked;
}

public class SaveManager : MonoBehaviour
{
    [SerializeField] private CardItemSO cardDatabase; // 인스펙터에서 할당
    // GameManager는 싱글톤 인스턴스로 접근한다고 가정 (GameManager.Instance)

    private const string ChallengeLevelKey = "ChallengeLevel";
    private const string CardUnlockDataKey = "CardUnlockData";

    void Awake()
    {
        // cardDatabase가 할당되었는지 확인
        if (cardDatabase == null)
        {
            Debug.LogError("[SaveManager] CardItemSO (cardDatabase)가 할당되지 않았습니다! 인스펙터에서 할당해주세요.");
            // 예시: Resources.Load를 통한 로드 (실제 경로로 수정 필요)
            // cardDatabase = Resources.Load<CardItemSO>("ScriptableObjects/CardItemDatabase"); 
        }
        LoadGameData();
    }

    void OnApplicationQuit()
    {
        SaveGameData();
    }

    public void SaveGameData()
    {
        // 1. GameManager의 ChallengeLevel 저장
        if (GameManager.Instance != null)
        {
            // GameManager의 challengeLevel 필드가 public이거나, public 프로퍼티를 통해 접근 가능해야 합니다.
            ES3.Save<int>(ChallengeLevelKey, GameManager.Instance.ChallengeLevel);
            Debug.Log($"[SaveManager] ChallengeLevel 저장: {GameManager.Instance.ChallengeLevel}");
        }
        else
        {
            Debug.LogError("[SaveManager] GameManager 인스턴스를 찾을 수 없습니다!");
        }

        // 2. 카드 데이터 언락 상황 저장
        if (cardDatabase != null && cardDatabase.items != null)
        {
            List<CardSaveData> cardSaveList = new List<CardSaveData>();
            foreach (CardItem item in cardDatabase.items)
            {
                // CardItem에 CardName (또는 고유 ID)과 IsUnlocked 속성이 있다고 가정
                cardSaveList.Add(new CardSaveData { cardName = item.CardName, isUnlocked = item.IsUnlocked });
            }
            ES3.Save<List<CardSaveData>>(CardUnlockDataKey, cardSaveList);
            Debug.Log($"[SaveManager] {cardSaveList.Count}개의 카드 언락 정보 저장 완료.");
        }
        else
        {
            Debug.LogError("[SaveManager] CardDatabase 또는 그 안의 item 리스트가 유효하지 않아 카드 정보를 저장할 수 없습니다.");
        }
        Debug.Log("[SaveManager] 게임 데이터 저장 완료.");
    }

    public void LoadGameData()
    {
        // 1. GameManager의 ChallengeLevel 로드
        if (GameManager.Instance != null)
        {
            if (ES3.KeyExists(ChallengeLevelKey))
            {
                int loadedLevel = ES3.Load<int>(ChallengeLevelKey, 1); // 기본값 1
                GameManager.Instance.ChallengeLevel = loadedLevel;
                Debug.Log($"[SaveManager] ChallengeLevel 로드: {loadedLevel}");
            }
            else
            {
                GameManager.Instance.ChallengeLevel = 1; // 저장된 데이터가 없으면 기본값으로 설정
                 Debug.Log($"[SaveManager] 저장된 ChallengeLevel 없음. 기본값 1로 설정.");
            }
        }
        else
        {
            Debug.LogWarning("[SaveManager] GameManager 인스턴스를 찾을 수 없어 ChallengeLevel을 로드할 수 없습니다. (로드 시점이 너무 이를 수 있음)");
        }

        // 2. 카드 데이터 언락 상황 로드
        if (cardDatabase != null && cardDatabase.items != null)
        {
            if (ES3.KeyExists(CardUnlockDataKey))
            {
                List<CardSaveData> loadedCardData = ES3.Load<List<CardSaveData>>(CardUnlockDataKey, new List<CardSaveData>());
                if (loadedCardData != null) // ES3.Load는 defaultValue를 반환하므로 null 체크가 항상 필요하지 않을 수 있음
                {
                    foreach (CardSaveData savedData in loadedCardData)
                    {
                        // CardItemSO의 items 리스트에서 CardName (또는 ID)으로 해당 CardItem을 찾습니다.
                        CardItem cardInSO = cardDatabase.items.FirstOrDefault(item => item.CardName == savedData.cardName);
                        if (cardInSO != null)
                        {
                            cardInSO.IsUnlocked = savedData.isUnlocked;
                        }
                        else
                        {
                            Debug.LogWarning($"[SaveManager] CardItemSO에서 '{savedData.cardName}' 카드를 찾을 수 없습니다.");
                        }
                    }
                    Debug.Log($"[SaveManager] {loadedCardData.Count}개의 카드 언락 정보 로드 완료.");
                }
            }
            else
            {
                 Debug.Log($"[SaveManager] 저장된 카드 언락 정보 없음. CardItemSO의 기본값 사용.");
            }
        }
        else
        {
            Debug.LogWarning("[SaveManager] CardDatabase 또는 그 안의 item 리스트가 유효하지 않아 카드 정보를 로드할 수 없습니다.");
        }
        Debug.Log("[SaveManager] 게임 데이터 로드 완료.");
    }
} 