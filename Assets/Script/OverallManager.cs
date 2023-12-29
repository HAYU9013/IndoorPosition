using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.NetworkInformation;
using System;
using UnityEngine.Networking;
using UnityEngine.Android;
using UnityEngine.UI;


public class OverallManager : MonoBehaviour
{
    MapHandler mapHandler;
    public string baseUrl;
    public int pressCnt = 0;
    public Text isCreating;
    [System.Serializable]
    public class ResponseData
    {
        public string message;
        public string position;
    }

    public GameObject buttonA, buttonB;
    
    public Text timer;
    public float CountDown = 0f;
    // Start is called before the first frame update
    void Start()
    {
        baseUrl = "https://a358-125-227-30-92.ngrok.io";
        mapHandler = GameObject.Find("MapArea").GetComponent<MapHandler>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CountDown >= 0)
        {
            CountDown -= Time.deltaTime;
        }
        
        
    }
    


    public void pressButtonTest() // testing button
    {
        
        pressCnt++;
        string printText = " hello world";
        // Debug.Log(pressCnt.ToString() + printText);
        
        mapHandler.isCreating = !mapHandler.isCreating;

        if (mapHandler.isCreating)
        {
            isCreating.text = "EDIT";
            buttonA.SetActive(true);
            buttonB.SetActive(true);
        }
        else
        {
            isCreating.text = "locate";
            buttonA.SetActive(false);
            buttonB.SetActive(false);
        }
      

    }

    public void postData(string jsonData, string url)
    {
        StartCoroutine(Upload(jsonData, url));   
    }
     IEnumerator Upload(string jsonData, string url)
    {
        // Debug.Log("Send: " + jsonData);
        jsonData = "@" + jsonData + "@";
        using (UnityWebRequest www = UnityWebRequest.Post(url, jsonData))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Debug.Log("Form upload complete!");
                if (url.Contains("get"))
                {
                    string responseText = www.downloadHandler.text; // 获取后端返回的数据
                    ResponseData responseData = JsonUtility.FromJson<ResponseData>(responseText);

                    // 获取position的值
                    string positionValue = responseData.position;
                    Debug.Log("Position Value: " + positionValue);
                    mapHandler.GetComponent<MapHandler>().setTarget(int.Parse(positionValue));
                }
                
                
            }
        }
    }


}
