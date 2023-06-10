using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using JetBrains.Annotations;

public class InfoPanel_Script : MonoBehaviour
{
    public TMP_Text statusText;
    public TMP_Text stateText;
    public TMP_Text bedTempText;
    public TMP_Text toolTempText;
    private string defaultUri = "https://shared-CVUR4ADOSF15BKW8PLTRVHTZK9D6YIL9.octoeverywhere.com/api/printer";
    private string defaultToken = "89C1672AC82243398BAD6B6A3AB5610F";
    string token = "89C1672AC82243398BAD6B6A3AB5610F";
    string HTTPResponse = null;
    HTTPResponseData response;
    private float updateInterval = 3f; //time interval between update requests

    // Start is called before the first frame update
    void Start()
    {
        StartUpdateRequest();
        Coroutine OctoprintHTTPRequest = StartCoroutine(getRequest(defaultUri, defaultToken));
    }

    public void StartUpdateRequest()
    {
        StartCoroutine(UpdateRequest());
    }

    private IEnumerator UpdateRequest() 
    {
        while (true) 
        {
            yield return getRequest(defaultUri, defaultToken);
            yield return new WaitForSeconds(updateInterval);
        }
    }

    IEnumerator getRequest(string uri, string token)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        uwr.SetRequestHeader("Authorization", "Bearer " + token);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error While Sending: " + uwr.error);
            Debug.LogError("Result: " + uwr.result);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            Debug.Log("Response Code: " + uwr.responseCode);
        }
        //set response as HTTPResponse
        HTTPResponse = uwr.downloadHandler.text;
        Debug.Log("Recieved" + uwr.downloadHandler.text);
        Debug.Log("Received JSON: " + HTTPResponse);
        HTTPResponseData response = JsonUtility.FromJson<HTTPResponseData>(HTTPResponse);

        string stateTextValue = response.state.text;
        StateFlagsData stateFlags = response.state.flags;
        bool operationalFlag = stateFlags.operational;
        float bedTempCurrent = response.temperature.bed.actual;
        float toolTempCurrent = response.temperature.tool0.actual;

        statusText.text = "SD Card Status: " + operationalFlag.ToString();
        stateText.text = "Printer Status: " + stateTextValue;
        bedTempText.text = "Bed Temp: " + bedTempCurrent.ToString() + "°C";
        toolTempText.text = "Tool Temp: " + toolTempCurrent.ToString() + "°C";
    }

}

[System.Serializable]
public class SDData
{
    public bool ready;
}
[System.Serializable]
public class StateFlagsData
{
    public bool cancelling;
    public bool closedOrError;
    public bool error;
    public bool finishing;
    public bool operational;
    public bool paused;
    public bool pausing;
    public bool printing;
    public bool ready;
    public bool resuming;
    public bool sdReady;
}

[System.Serializable]
public class StateData
{
    public string error;
    public string text;
    public StateFlagsData flags;
}
[System.Serializable]
public class BedTemperatureData
{
    public float actual;
    public float offset;
    public float target;
}

[System.Serializable]
public class ToolheadTemperatureData
{
    public float actual;
    public float offset;
    public float target;
}
[System.Serializable]
public class TemperatureData
{
    public BedTemperatureData bed;
    public ToolheadTemperatureData tool0;
}

[System.Serializable]
public class HTTPResponseData
{
    public SDData sd;
    public StateData state;
    public TemperatureData temperature;
}


