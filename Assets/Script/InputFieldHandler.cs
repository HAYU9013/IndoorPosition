using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldHandler : MonoBehaviour
{
    public OverallManager overallManager;
    public GameObject mapArea;
    // Start is called before the first frame update
    void Start()
    {
        mapArea = GameObject.Find("MapArea");
        overallManager = GameObject.Find("OverallManager").GetComponent<OverallManager>();
    }

    // Update is called once per frame
    void Update()
    {

        var input = gameObject.GetComponent<InputField>();
        input.onEndEdit.AddListener(SubmitName);

    }

    private void SubmitName(string arg0)
    {
        if(arg0.Length > 5)
        {
            Debug.LogWarning("change url");
            overallManager.baseUrl = arg0;
            // Debug.Log(arg0);
            // mapArea.SendMessage("setTarget", int.Parse(arg0));
        }

    }
}
