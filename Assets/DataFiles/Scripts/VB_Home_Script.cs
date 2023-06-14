using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.Networking;
using System;

public class VB_Home_Script : MonoBehaviour
{
    VirtualButtonBehaviour vrb;
    private string defaultToken = "89C1672AC82243398BAD6B6A3AB5610F";
    private string defaultUri = "https://shared-CVUR4ADOSF15BKW8PLTRVHTZK9D6YIL9.octoeverywhere.com/api/printer";


    // Start is called before the first frame update
    void Start()
    {
        vrb = GetComponent<VirtualButtonBehaviour>();
        vrb.RegisterOnButtonPressed(OnButtonPressed);
        vrb.RegisterOnButtonReleased(OnButtonReleased);
    }

    public void OnTrackableStateChange(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
           newStatus == TrackableBehaviour.Status.TRACKED ||
           newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log("Image target detected");
        }
        else
        {
            Debug.Log("No Image Target");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        Debug.Log("Home Button Pressed");
        StartCoroutine(SendHomeRequest());
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        Debug.Log("Home Button Released");
    }

    public IEnumerator SendHomeRequest()
    {
        string homeRequestUri = defaultUri + "/printhead";
        string homeRequestData = "{\"command\": \"home\", \"axes\": [\"x\", \"y\", \"z\"]}";

        UnityWebRequest homeRequest = new UnityWebRequest(homeRequestUri, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(homeRequestData);
        homeRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        homeRequest.downloadHandler = new DownloadHandlerBuffer();
        homeRequest.SetRequestHeader("Content-Type", "application/json");
        homeRequest.SetRequestHeader("Authorization", "Bearer " + defaultToken);

        yield return homeRequest.SendWebRequest();

        if (homeRequest.result == UnityWebRequest.Result.ConnectionError || homeRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error While Sending Home Request: " + homeRequest.error);
            Debug.LogError("Result: " + homeRequest.result);
        }
        else
        {
            Debug.Log("Home Request Sent Successfully");
        }
    }
}
