using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rot : MonoBehaviour {
    public Transform origin;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        Vector3 res = origin.InverseTransformPoint(this.transform.position);
        res = new Vector3(-res.x, res.y, res.z);
        Debug.Log(res);
    }
}
