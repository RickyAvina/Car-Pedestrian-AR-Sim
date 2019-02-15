using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using HoloToolkit.Unity.UX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;

// <Summary>
// Manages the persisent anchors from the origin cube and also manages placing processes
// </Summary>
public class AncManager : MonoBehaviour {

    internal GameObject Origin;   // the prefab representing your origin 
    private string originAnchorName = "Origin";
    private WorldAnchorStore anchorStore;
    //private bool placing = false;
    //private TapToPlace tapToPlace;  // tapToPlace should handle destroying and reclaiming anchors

	// Use this for initialization
	void Start () {
        Origin = gameObject;
        //tapToPlace = GetComponent<TapToPlace>();
        //tapToPlace.IsBeingPlaced = false;
        //originAnchorName = tapToPlace.originAnchorName; // set the name of the anchor origin in TapToPlace
        Hide();
        WorldAnchorStore.GetAsync(AnchorStoreReady);
	}

    // <Summary>
    // Ready is called when mapping has finished to determine if you want to set the origin or not
    public void Ready()
    {
        Show();    // show the originCube

        string[] ids = anchorStore.GetAllIds();
        bool anchorFound = false;

        Debug.Log("IDs: " + ids.Length);

        for (int i = 0; i < ids.Length; i++)
        {
            Debug.Log($"ID: {ids[i]}");

            if (ids[i] == originAnchorName)
            {
                anchorStore.Load(ids[i], Origin);  // load the originCube anchor
                Debug.Log("Found the anchor: " + originAnchorName + "!");
                anchorFound = true;
                break;
            }
        }

        if (!anchorFound)
        {
            Debug.Log("No anchor found");
            Origin.transform.position = Camera.main.transform.position;
            //placing = true;
            //tapToPlace.IsBeingPlaced = true;    // explicitly setting IsBeingPlaced to true is only needed if 
        }
    }

    void AnchorStoreReady(WorldAnchorStore store)
    {
        Debug.Log("AnchorStoreReady");
        anchorStore = store;
    }

    public void Reset()
    {
        string[] ids = anchorStore.GetAllIds();
        for (int i = 0; i < ids.Length; i++)
        {
            anchorStore.Delete(ids[i]);
        }
        //placing = true;
    }

    public void Hide()
    {
        Origin.GetComponent<MeshRenderer>().enabled = false;    // hide the origin
        Origin.GetComponent<BoxCollider>().enabled = false;     // disable the collider
        Origin.GetComponent<BoundingBoxRig>().Deactivate();
        Origin.GetComponent<BoundingBoxRig>().appBarInstance.enabled = false;   // hide app bar
        Origin.GetComponent<HandDraggable>().enabled = false;

        foreach (Transform child in transform)
        {
            child.GetComponent<MeshRenderer>().enabled = false; // hide all of the axis
        }
    }

    public void Show()
    {
        Origin.GetComponent<MeshRenderer>().enabled = true;    // hide the origin
        Origin.GetComponent<BoxCollider>().enabled = true;     // disable the collider
        Origin.GetComponent<BoundingBoxRig>().appBarInstance.enabled = false;   // show app bar. Has been proven to work with false here too
        Origin.GetComponent<BoundingBoxRig>().Activate();
        Origin.GetComponent<HandDraggable>().enabled = true;


        foreach (Transform child in transform)
        {
            child.GetComponent<MeshRenderer>().enabled = true; // hide all of the axis
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
