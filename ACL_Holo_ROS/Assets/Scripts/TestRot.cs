using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRot : MonoBehaviour {
    public Transform Origin;
    private Vector3 relPos;
	// Use this for initialization
	void Start () {
        Debug.Log("YEE");

    }

    // Update is called once per frame
    void Update () {
        Debug.Log("YEE");
        relPos = Origin.InverseTransformPoint(this.transform.position);
        Debug.Log("relPos: " + relPos);
    }
}
