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
       // conole �� ǥ���� �� �ְ� �Ŀ� ����
       
    }
}