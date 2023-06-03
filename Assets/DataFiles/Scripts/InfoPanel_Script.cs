using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class InfoPanel_Script : MonoBehaviour
{
    public TMP_Text canvastext;
    string token = "89C1672AC82243398BAD6B6A3AB5610F";
    string HTTPResponse = null;
    HTTPResponseData response;

    // Start is called before the first frame update
    void Start()
    {
        Coroutine OctoprintHTTPRequest = StartCoroutine(getRequest("https://shared-CVUR4ADOSF15BKW8PLTRVHTZK9D6YIL9.octoeverywhere.com/api/printer", token));
        
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
        //Debug.Log(HTTPResponse);
        HTTPResponseData response = JsonUtility.FromJson<HTTPResponseData>(HTTPResponse);

        bool getReadyProperty = response.someData.ready;
        Debug.Log("ready: " + getReadyProperty);

        canvastext.text = getReadyProperty.ToString();
    }

}

[System.Serializable]
public class ResponseData
{
    public bool ready;
    //public string state;
    //public string temperature;
    //public string bed;
    //public string tool0;
}
[System.Serializable]
public class HTTPResponseData
{
    public ResponseData someData;
}


