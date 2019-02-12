using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechController : MonoBehaviour, ISpeechHandler
{
    public TextMesh text;
    public EventHandler OnMappingFinished;  // public event notifying subscribers when mapping has finished
    public EventHandler OnOriginSet;        // public event notifying subscribers when the origin has been set
    //public TapToPlace tapToPlace;           // required to know if origin is currently being set
    public GameObject origin;

    void Start()
    {
        //tapToPlace.OnPlaced += OnPlaced;
        text.text = "Say 'ready' when\nyou are satisfied\n with the mapping";
    }

    //private void OnPlaced(object o, EventArgs e)
    //{
    //    text.text = "Click the box to re-configure your origin\n or say 'start' to\nstart the simulation";
    //}

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        switch (eventData.RecognizedText.ToLower())
        {
            case "ready":
                MappingFinished();
                break;
            case "start":
                OriginSet();
                break;
            default:
                break;
        }
    }

    void Update()
    {
        //if (tapToPlace == null)
        //{
        //    Debug.LogError("Set TapToPlace!");
        //    return;
        //}
    }

    private void AttachWorldAnchor()
    {
        Debug.Log("I reached Attach World Anchor! [Speech]");

        if (WorldAnchorManager.Instance != null)
        {
            // Add world anchor when object placement is done.
            WorldAnchorManager.Instance.AttachAnchor(origin);
            Debug.Log("[Speech] " +
                "Anchor Attached with anchor name: originAnchor");
        }
    }

    private void OriginSet()
    {
        //if (tapToPlace==null)
        //{
        //    Debug.LogError("Set TapToPlace!");
        //    return;
        //}

        //if (tapToPlace.IsBeingPlaced)
        //{
        //    text.text = "Place your origin first before starting";
        //    return;
        //}

        AttachWorldAnchor();
        OnOriginSet(this, null);
        text.text = "";
        // disable TapToPlace here
    }


    private void MappingFinished()
    {
        OnMappingFinished(this, null);
        SpatialMappingManager.Instance.DrawVisualMeshes = false;
        SpatialMappingManager.Instance.StopObserver();  // stop updating the mesh

        //if (tapToPlace == null)
        //{
        //    Debug.LogError("Set TapToPlace!");
        //    return;
        //}

        text.text = "Click the box to re-configure your origin\n or say 'start' to\nstart the simulation";

        // Make planes here
    }

    private void OnApplicationQuit()
    {
        //if (tapToPlace.OnPlaced != null)
        //{
        //    tapToPlace.OnPlaced -= OnPlaced;    // remove listener
        //}
    }
}
