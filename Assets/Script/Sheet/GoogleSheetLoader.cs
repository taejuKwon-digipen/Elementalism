using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

//��Đ� ���⼭ ī�� �����ͺ��̽��� ��� ������ ���� �𸣰ڳ׿� 
//�� �� ã�ƺ��� ������
//https://minyoung529.tistory.com/78 �̰ź��� �ٽ� �������ҵ� �ϴ� ��Ϳ�

public class GoogleSheetLoader : MonoBehaviour
{
  /*  public static GoogleSheetLoader Instance
    {
        get
        {
            return _instance;
        }
    }

    private static GoogleSheetLoader _instance;*/

    public static string GetTSVAddress(string address, string range, long sheetID)
    {
        return $"{address}/export?format=tsv&range={range}&gid={sheetID}";
    }
}

public class ReadSheet : MonoBehaviour
{
    public static ReadSheet Instance
    {
        get
        {
            return _instance;
        }
    }
    private static ReadSheet _instance;

    public readonly string ADDRESS = "https://docs.google.com/spreadsheets/d/1ZySJiU_PCwQQ7e3l9tEEGYl9v9jpqNMdCZAlhuB5q9g";
    public readonly string RANGE = "A2:E17";
    public readonly long SHEET_ID = 0;

    private void Start()
    {
        StartCoroutine(LoadData());
    }

    private IEnumerator LoadData()
    {
        UnityWebRequest www = UnityWebRequest.Get(GoogleSheetLoader.GetTSVAddress(ADDRESS, RANGE, SHEET_ID));
        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);
    }
}

