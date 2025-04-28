using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GoogleSheetLoader : MonoBehaviour
{
    public static GoogleSheetLoader Instance { get; private set; }

    // 구글 시트 CSV 주소 (예시)
    [Header("구글 시트 CSV 주소")]
    public string sheetUrl = "https://docs.google.com/spreadsheets/d/1X1JrZBPwUKbbxhx_HtsJhCrLJG801lTCF4C6bTe9nJQ/gviz/tq?tqx=out:csv";

    // 언어별 텍스트 저장 (Key: string, Value: string)
    private Dictionary<string, string> localizedTexts = new Dictionary<string, string>();
    private int langColumn = 1; // 1: Korean, 2: English, 3: Japanese (기본값: Korean)

    public enum Language { Korean = 1, English = 2, Japanese = 3 }
    public Language CurrentLanguage = Language.English;

    public System.Action OnSheetLoaded; // 데이터 로드 완료 콜백
    public bool IsLoaded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않게
            Debug.Log("[GoogleSheetLoader] Instance 등록됨: " + GetInstanceID());
        }
        else
        {
            Debug.Log("[GoogleSheetLoader] 중복 생성 감지, 파괴: " + GetInstanceID());
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadSheetForLanguage(CurrentLanguage);
    }

    public void LoadSheetForLanguage(Language lang)
    {
        langColumn = (int)lang;
        StartCoroutine(LoadSheet());
    }

    private IEnumerator LoadSheet()
    {
        Debug.Log("[GoogleSheetLoader] 구글 시트 데이터 요청 시작: " + sheetUrl);
        UnityWebRequest www = UnityWebRequest.Get(sheetUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("[GoogleSheetLoader] 구글 시트 데이터 다운로드 성공");
            ParseCSV(www.downloadHandler.text, langColumn);
            Debug.Log($"구글 시트 로드 성공 (언어: {CurrentLanguage})");
            IsLoaded = true;
            yield return null; // 혹시라도 프레임을 넘기고 싶으면 추가
            OnSheetLoaded?.Invoke(); // 데이터 로드 완료 시 콜백 호출
        }
        else
        {
            Debug.LogError($"[GoogleSheetLoader] 구글 시트 로드 실패: {www.error}\nURL: {sheetUrl}");
            if (!string.IsNullOrEmpty(www.downloadHandler.text))
            {
                Debug.LogError($"[GoogleSheetLoader] 응답 내용: {www.downloadHandler.text}");
            }
        }
    }

    private void ParseCSV(string csv, int langColumn)
    {
        localizedTexts.Clear();
        var lines = csv.Split('\n');
        bool isFirstLine = true;
        foreach (var line in lines)
        {
            if (isFirstLine) { isFirstLine = false; continue; } // 첫 줄(헤더) 무시

            var columns = line.Split(',');
            if (columns.Length > langColumn && !string.IsNullOrWhiteSpace(columns[0]))
            {
                string key = columns[0].Trim().Trim('"');
                string value = columns[langColumn].Trim().Trim('"');
                localizedTexts[key] = value;
                Debug.Log($"[GoogleSheetLoader] {key} = {value}");
            }
        }
    }

    public string GetText(string key)
    {
        if (localizedTexts.TryGetValue(key, out var value))
        {
            return value;
        }
        else
        {
            Debug.LogWarning($"[GoogleSheetLoader] Key '{key}' not found in localizedTexts!");
            return key;
        }
    }

    // 언어 변경 시 호출
    public void ChangeLanguage(Language lang)
    {
        if (CurrentLanguage != lang)
        {
            CurrentLanguage = lang;
            LoadSheetForLanguage(lang);
        }
    }

    public List<string> GetAllKeys()
    {
        return new List<string>(localizedTexts.Keys);
    }
} 