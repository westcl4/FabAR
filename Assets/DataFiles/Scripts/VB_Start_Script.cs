using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.Networking;
using System;
using Unity.VisualScripting.Antlr3.Runtime;

public class VB_Start_Script : MonoBehaviour
{
    VirtualButtonBehaviour vrb;
    string token = "89C1672AC82243398BAD6B6A3AB5610F";

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
        Debug.Log("Button Pressed");
        Coroutine OctoprintHTTPRequest = StartCoroutine(getRequest("http://homer.local/api/server", token));

    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        Debug.Log("Button Released");
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
    }
}
