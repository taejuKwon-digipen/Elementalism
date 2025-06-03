using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GoogleSheetLoader : MonoBehaviour
{
    public static GoogleSheetLoader Instance { get; private set; }

    // 구글 시트 CSV 주소 (예시)
    [Header("구글 시트 CSV 주소")]
    public string languageSheetUrl;
    public string cardSheetUrl;
    public string dbSheetUrl;
    public string eventSheetUrl; // 이벤트 시트 URL

    // 언어별 텍스트 저장 (Key: string, Value: string)
    private Dictionary<string, string> localizedTexts = new Dictionary<string, string>();
    private int langColumn = 1; // 1: Korean, 2: English, 3: Japanese (기본값: Korean)

    public enum Language { Korean = 1, English = 2, Japanese = 3 }
    public Language CurrentLanguage = Language.English;

    public System.Action OnSheetLoaded; // 데이터 로드 완료 콜백
    public bool IsLoaded = false;

    // 카드 데이터 및 DB 데이터 저장용
    public class CardData
    {
        public int CardID;
        public string Name_KR;
        public string Desc_KR;
        public string Name_EN;
        public string Desc_EN;
        public string Name_JP;
        public string Desc_JP;
    }
    public Dictionary<int, CardData> cardDatas = new();
    public Dictionary<string, string> dbValues = new();

    // 이벤트 데이터 저장용
    public class EventData
    {
        public int id;
        public string content;
        public string choice1_text;
        public string choice1_effect;
        public string choice1_result;
        public string choice2_text;
        public string choice2_effect;
        public string choice2_result;
    }
    public Dictionary<int, EventData> eventDatas = new();

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
        StartCoroutine(LoadAllSheets());
    }

    private IEnumerator LoadAllSheets()
    {
        yield return StartCoroutine(LoadLanguageSheet());
        yield return StartCoroutine(LoadCardSheet());
        yield return StartCoroutine(LoadDBSheet());
        yield return StartCoroutine(LoadEventSheet());
        IsLoaded = true;
        OnSheetLoaded?.Invoke();
    }

    private IEnumerator LoadLanguageSheet()
    {
        Debug.Log("[GoogleSheetLoader] 언어 시트 데이터 요청 시작: " + languageSheetUrl);
        UnityWebRequest www = UnityWebRequest.Get(languageSheetUrl);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("[GoogleSheetLoader] 언어 시트 데이터 다운로드 성공");
            ParseCSV(www.downloadHandler.text, langColumn);
        }
        else
        {
            Debug.LogError($"[GoogleSheetLoader] 언어 시트 로드 실패: {www.error}\nURL: {languageSheetUrl}");
        }
    }

    private IEnumerator LoadCardSheet()
    {
        Debug.Log("[GoogleSheetLoader] 카드 시트 데이터 요청 시작: " + cardSheetUrl);
        UnityWebRequest www = UnityWebRequest.Get(cardSheetUrl);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("[GoogleSheetLoader] 카드 시트 데이터 다운로드 성공");
            ParseCardCSV(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"[GoogleSheetLoader] 카드 시트 로드 실패: {www.error}\nURL: {cardSheetUrl}");
        }
    }

    private IEnumerator LoadDBSheet()
    {
        Debug.Log("[GoogleSheetLoader] DB 시트 데이터 요청 시작: " + dbSheetUrl);
        UnityWebRequest www = UnityWebRequest.Get(dbSheetUrl);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("[GoogleSheetLoader] DB 시트 데이터 다운로드 성공");
            ParseDBCSV(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"[GoogleSheetLoader] DB 시트 로드 실패: {www.error}\nURL: {dbSheetUrl}");
        }
    }

    private void ParseCardCSV(string csv)
    {
        cardDatas.Clear();
        var lines = ReadCsvLines(csv);
        bool isFirstLine = true;
        foreach (var line in lines)
        {
            if (isFirstLine) { isFirstLine = false; continue; }
            var columns = CsvHelper.ParseLine(line);
            if (columns.Count > 6)
            {
                CardData card = new CardData();
                int.TryParse(columns[0].Trim(), out card.CardID);
                card.Name_KR = columns[1].Trim();
                card.Desc_KR = columns[2].Trim();
                card.Name_EN = columns[3].Trim();
                card.Desc_EN = columns[4].Trim();
                card.Name_JP = columns[5].Trim();
                card.Desc_JP = columns[6].Trim();
                cardDatas[card.CardID] = card;
                Debug.Log($"[GoogleSheetLoader][Card] ID={card.CardID}, KR={card.Name_KR}, EN={card.Name_EN}, JP={card.Name_JP}");
            }
        }
    }

    private void ParseDBCSV(string csv)
    {
        dbValues.Clear();
        var lines = ReadCsvLines(csv);
        bool isFirstLine = true;
        foreach (var line in lines)
        {
            if (isFirstLine) { isFirstLine = false; continue; }
            var columns = CsvHelper.ParseLine(line);
            if (columns.Count > 1)
            {
                string key = columns[0].Trim();
                string value = columns[1].Trim();
                dbValues[key] = value;
                Debug.Log($"[GoogleSheetLoader][DB] {key} = {value}");
            }
        }
    }

    // 셀 내부 줄바꿈까지 지원하는 CSV 줄 분리 함수
    private static List<string> ReadCsvLines(string csv)
    {
        var lines = new List<string>();
        var sb = new System.Text.StringBuilder();
        bool inQuotes = false;
        for (int i = 0; i < csv.Length; i++)
        {
            char c = csv[i];
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            if (c == '\n' && !inQuotes)
            {
                lines.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }
        if (sb.Length > 0)
            lines.Add(sb.ToString());
        return lines;
    }

    private void ParseCSV(string csv, int langColumn)
    {
        localizedTexts.Clear();
        var lines = ReadCsvLines(csv);
        bool isFirstLine = true;
        foreach (var line in lines)
        {
            if (isFirstLine) { isFirstLine = false; continue; } // 첫 줄(헤더) 무시

            var columns = CsvHelper.ParseLine(line);
            if (columns.Count > langColumn && !string.IsNullOrWhiteSpace(columns[0]))
            {
                string key = columns[0].Trim();
                string value = columns[langColumn].Trim();
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
            StartCoroutine(LoadAllSheets());
        }
    }

    public List<string> GetAllKeys()
    {
        return new List<string>(localizedTexts.Keys);
    }

    private IEnumerator LoadEventSheet()
    {
        Debug.Log("[GoogleSheetLoader] 이벤트 시트 데이터 요청 시작: " + eventSheetUrl);
        UnityWebRequest www = UnityWebRequest.Get(eventSheetUrl);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("[GoogleSheetLoader] 이벤트 시트 데이터 다운로드 성공");
            ParseEventCSV(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"[GoogleSheetLoader] 이벤트 시트 로드 실패: {www.error}\nURL: {eventSheetUrl}");
        }
    }

    private void ParseEventCSV(string csv)
    {
        eventDatas.Clear();
        var lines = ReadCsvLines(csv);
        bool isFirstLine = true;
        foreach (var line in lines)
        {
            if (isFirstLine) { isFirstLine = false; continue; }
            var columns = CsvHelper.ParseLine(line);
            if (columns.Count > 5)
            {
                EventData data = new EventData();
                int.TryParse(columns[0].Trim(), out data.id);
                data.content = columns[1].Trim();
                data.choice1_text = columns[2].Trim();
                data.choice1_effect = columns[3].Trim();
                data.choice2_text = columns[4].Trim();
                data.choice2_effect = columns[5].Trim();
                eventDatas[data.id] = data;
            }
        }
    }
} 