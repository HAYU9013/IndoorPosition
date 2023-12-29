using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
public class GetMacHandler : MonoBehaviour
{
    public string command = "netsh wlan show networks mode=Bssid";
    public List<string> receive = new List<string>();

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PressButton()
    {
        print("getting wifi");
        ExecuteCommand();
    }

    public List<string> ExecuteCommand()
    {
        
        Process process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c " + command;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;

        receive.Clear();
        process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                // UnityEngine.Debug.Log("Command Output: " + e.Data);
                receive.Add(e.Data.ToString());
            }
        });
        
        process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit();
        process.Close();
        
        return DataProceed();
    }
    List<string> DataProceed()
    {
        List<string> retMacString = new List<string>();
        
        // print("ret size: " + retMacString.Count);
        for(int i = 0; i < receive.Count; i++)
        {
            // UnityEngine.Debug.Log(receive[i]);
            if (receive[i].Contains("BSSID 1"))
            {
                // 字符串包含搜索字符串

                // UnityEngine.Debug.Log(receive[i]);
                string[] macStrings = receive[i].Split(':');
                string mac = "";
                int len = macStrings.Length;
                mac = macStrings[len - 6].Trim() + ":" + macStrings[len - 5] + ":" + macStrings[len - 4] + ":" + macStrings[len - 3] + ":" + macStrings[len - 2] + ":" + macStrings[len - 1];
                
                // UnityEngine.Debug.Log(mac);
                string signal = receive[i + 1].Split(':')[1];
                signal = "" + signal[1] + signal[2];
                // UnityEngine.Debug.Log(signal);
                retMacString.Add(mac);
                retMacString.Add(signal);
            }
        }
        return retMacString;
    }
}
