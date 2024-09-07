using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

//흠냐릥 여기서 카드 데이터베이스에 어떻게 보내야 할지 모르겠네요 
//좀 더 찾아보고 수정함
//https://minyoung529.tistory.com/78 이거보고 다시 만들어야할듯 일단 잠와요

public class GoogleSheetLoader : MonoBehaviour
{
    string DataSheet;
    public Text displayText;
    const string googleSheetURL = "https://docs.google.com/spreadsheets/d/1ZySJiU_PCwQQ7e3l9tEEGYl9v9jpqNMdCZAlhuB5q9g/edit?usp=sharing";

    IEnumerator Start()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(googleSheetURL))
        {
            yield return www.SendWebRequest();
            if(www.isDone)
            {
                DataSheet = www.downloadHandler.text;
                Console.Write(DataSheet);
            }

            //DisplayText();
        }
    }

    void DisplayText()
    {
       // conole 에 표시할 수 있게 후에 제작
       
    }
}