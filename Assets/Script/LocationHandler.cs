using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class LocationHandler : MonoBehaviour
{
    MapHandler mapHandler;
    GetMacHandler getMacHandler;
    // public string jsonData;
    [System.Serializable]
    public class MacData
    {
        public int point;
        public List<string> macs;
    }

    [System.Serializable]
    public class MacListWarp
    {
        public List<MacData> MacList = new List<MacData>();
    }

    public MacListWarp macList;

    public int getWifiTime = 0;
    private float getWifiDuration = 2f;
    public float getWifiTick = 0;
    public bool getWifiDone = false;

    OverallManager overallManager;
    private string url = "https://a358-125-227-30-92.ngrok.io" + "/data_update";
    public int totalGetWifiTime = 10;

    // Start is called before the first frame update
    void Start()
    {
        mapHandler = GameObject.Find("MapArea").GetComponent<MapHandler>();
        getMacHandler = GameObject.Find("GetMac").GetComponent<GetMacHandler>();
        overallManager = GameObject.Find("OverallManager").GetComponent<OverallManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(overallManager.baseUrl.Length > 5 && overallManager.baseUrl + "/data_update" != url)
        {
            url = overallManager.baseUrl + "/data_update";
            Debug.LogWarning("change update url " + url);
        }
        
        getWifiTick -= Time.deltaTime;
        if (getWifiTime > 0 && getWifiTick < 0)
        {
            getWifiMac();
            getWifiTime--;
            getWifiTick = getWifiDuration;
            print("still have " + getWifiTime);
            


            if (getWifiTime == 0)
            {
                print("done");
                
            }
        }

        if (getWifiDone == true)
        {
            
   
            
            getWifiDone = false;
        }

    }

    private void OnMouseDown()
    { 
        
        macList.MacList.Clear();
        print("Mouse down");
        getWifiTime = totalGetWifiTime + 1; // 取得 10 次
        int num = int.Parse(gameObject.name.Split("_")[1]);
        mapHandler.haveLocationData[num] = true;

    }

    void getWifiMac()
    {
        
        int num = int.Parse(gameObject.name.Split("_")[1]);
        mapHandler.haveLocationData[num] = true;
        
        MacData macData = new MacData();
        
        macData.macs = getMacHandler.ExecuteCommand();
        macData.point = num;

        string jsonData = "" + JsonUtility.ToJson(macData);
        Debug.Log(jsonData);

        overallManager.postData(jsonData, url);

    }
    

    
}
