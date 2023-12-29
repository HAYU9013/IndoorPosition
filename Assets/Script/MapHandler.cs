using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapHandler : MonoBehaviour
{

    public bool isCreating = true; // creating mode or user mode


    MapHandler mapHandler;
    GetMacHandler getMacHandler;

    // warp the data to send to api
    [System.Serializable]
    public class MacData
    {
        
        public List<string> macs;
    }

 
    // draw the pin point on the map
    public GameObject mapArea;
    public GameObject locationPrefab; // the location pin prefab
    public float leftMost = -2f, rightMost = 2f; // the map position
    public float downMost = -2f, upMost = 2f;
    public float gridAmount = 5f; // how many point to show 
    public Vector3 standardScale;
    // the pin point list
    public List<GameObject> locationList = new List<GameObject>();

    // whether the pin point list have been collect the data
    [SerializeField]
    public List<bool> haveLocationData = new List<bool>();

    public int maxLocationNumber = 0;

    private string url = "https://a358-125-227-30-92.ngrok.io" + "/get_position";


    public int target = 9; // the location to show out
    public float updateTime = 0.5f; // how soon to get current position
    float updateTimeDelta ;

    OverallManager overallManager;
    // Start is called before the first frame update
    void Start()
    {
        overallManager = GameObject.Find("OverallManager").GetComponent<OverallManager>();
        mapHandler = GameObject.Find("MapArea").GetComponent<MapHandler>();
        getMacHandler = GameObject.Find("GetMac").GetComponent<GetMacHandler>();
        mapArea = GameObject.Find("MapArea");

        updateTimeDelta = updateTime;
        gridAmount--; // original will add one more
        initLocation();

    }

    // Update is called once per frame
    void Update()
    {
        if (overallManager.baseUrl.Length > 5 && overallManager.baseUrl + "/get_position" != url)
        {
            url = overallManager.baseUrl + "/get_position";
            Debug.LogWarning("change getPos url " + url);
        }

        if (updateTimeDelta < 0)
        {
            getWifiMac();
            updateLocationVisiable();
            updateTimeDelta = updateTime;
        }
        else
        {
            updateTimeDelta -= Time.deltaTime;
        }

    }

    public void setTarget(int t)
    {
        target = t;
    }

    void updateLocationVisiable()
    {

        for (int i = 0; i < locationList.Count; i++)
        {
            if (!locationList[i]) continue;

            Color NotHereColor = Color.white;
            Color IsHereColor = Color.white;
            Color BlackColor = Color.black;
            Color NoColor = Color.black;
            NotHereColor.a = 0.1f; // 0~1
            NoColor.a = 0f;

            if (isCreating) // 還沒有資料的設定黑色 有資料的設定彩色
            {
                locationList[i].transform.localScale = standardScale;
                if (haveLocationData[i])
                {
                    locationList[i].GetComponent<SpriteRenderer>().color = IsHereColor;
                }
                else
                {
                    
                    locationList[i].GetComponent<SpriteRenderer>().color = BlackColor;
                }
            }
            else // 正確點顯示白色 不是的顯示半透明
            {
                locationList[i].transform.localScale = standardScale * 0.3f;
                if (haveLocationData[i] == false)
                {
                    locationList[i].GetComponent<SpriteRenderer>().color = NoColor;
                }
                else if (locationList[i].name == "location_" + target.ToString())
                {
                    locationList[i].transform.localScale = standardScale;
                    locationList[i].GetComponent<SpriteRenderer>().color = IsHereColor;
                    
                    
                }
                else
                {
                   
                    locationList[i].GetComponent<SpriteRenderer>().color = NotHereColor;
                    
                }
            }
            

        }
    }


    void getWifiMac() 
    {
        
        MacData macData = new MacData();
        macData.macs = getMacHandler.ExecuteCommand();
        
        string jsonData = JsonUtility.ToJson(macData);
        Debug.Log(jsonData);

        overallManager.postData(jsonData, url);
        // send api;

        

    }

    


    // create pin point on the location and draw it out;
    void initLocation()
    {
        float gridLength = (rightMost - leftMost) / gridAmount;
        for (float i = leftMost; i <= rightMost; i += gridLength)
        {
            for(float j = downMost; j <= upMost; j += gridLength)
            {
                Vector3 tmpPos = new Vector3(i, j+1.4f, 0);
                GameObject newObject = Instantiate(locationPrefab, tmpPos, Quaternion.identity);
                newObject.transform.SetParent(mapArea.transform);
                standardScale = newObject.transform.localScale * 5 / gridAmount;
                newObject.transform.localScale *= 5 / gridAmount; // change scale according to the gridAmount

                newObject.name = "location_" + maxLocationNumber.ToString(); // 可能會出 bug
                
                maxLocationNumber++;
                locationList.Add(newObject);
                haveLocationData.Add(false);
            }
            
            
        }
    }




    // save load reset the haveLocationData list
    public void SaveHaveData()
    {
        print("save data");
        string saveData = "";
        for(int i = 0; i < haveLocationData.Count; i++)
        {
            if (haveLocationData[i]) saveData += "1";
            else saveData += "0";

        }
        print("Save Data: " + saveData);
        PlayerPrefs.SetString("haveLocationData", saveData);
        PlayerPrefs.Save();
    }
    public void LoadHaveData()
    {
        print("load data");
        string saveData = PlayerPrefs.GetString("haveLocationData");
        print("Load Data: " + saveData);
        for (int i = 0; i < haveLocationData.Count; i++)
        {
            if (saveData[i] == '1') haveLocationData[i] = true;
            else haveLocationData[i] = false;

        }

        
    }

    public void ResetHaveData()
    {
        print("reset data");
        for (int i = 0; i < haveLocationData.Count; i++)
        {
            haveLocationData[i] = false;
        }
        SaveHaveData();
        LoadHaveData();

    }
    

}
