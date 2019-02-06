using System.Collections.Generic;
using UnityEngine;
using geometry_msgs = RosSharp.RosBridgeClient.Messages.Geometry;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Debug = UnityEngine.Debug;
using RosSharp.RosBridgeClient;
using System.Collections;
using System;

/* 
 * 
 * This file controls the behavior of all of the pedestrians from one single GameObject.
 * It has a callback from BasicROS to register when a new pose is available.
 * 
 * TODO:
 *  - Create velocity/acceleration based motion to interpolate points for smoother experience
 *  - Global Anchors
 */

public class PedestrianController_Simple : MonoBehaviour
{
    public enum PoseStates
    {
        NotReady,
        Initial,
        Ready,
        Waiting,
    }

    PoseStates poseState = PoseStates.NotReady;
    private bool rosReady = false;          // when ROS has been connected
    private bool firstSignal = true;    // used for determining if pose status is being recieved for the first time

    private RosConnector rosConnector;  // RosConnector has a RosSocket which handles all the networking between Linux and Hololens systems

    // variables
    private geometry_msgs.PoseArray poseArray;  // Keeps track of the poses being recieved
    //private int poseStatus = -2;    // -2 for initial pose in callback, -1 means first pose recieved, queued for main thread, 0 means no new pose, 1 means ready for main loop update

    private int numPedestrians = 0;  // Keeps track of the number of pedestrians (currently only beign set once)
    private List<GameObject> pedestrians = new List<GameObject>();  // keeps track of the pedestrian game objects
    public GameObject pedestrianPrefab; // the physical prefab being used (should be searched for in memory)

    /* Temporary variables */
    private geometry_msgs.Point tempPos;    // small optimization
    private GameObject tempPedestrian;
    private Vector3 originCubePos;

    public string serverURL = "ws://192.168.0.19:9090";

    public void Start()
    {
        rosConnector = GetComponent<RosConnector>();
        RosConnector.ConnectToRos(RosConnector.Protocols.WebSocketSharp, serverURL, OnConnected);
    }

    private void OnConnected(object sender, EventArgs e)
    {
        rosConnector.RosSocket.Subscribe<geometry_msgs.PoseArray>("/ped_sim/pedestrian_pos", HandlePosesUpdated);

        Debug.Log("Subscribed to /ped_sim/pedestrian_pos");
        rosReady = true;
    }

    void Update()
    {
        if (poseState == PoseStates.Initial)
        {
            CreatePedestrians();
            poseState = PoseStates.Waiting;
        }
        if (poseState == PoseStates.Ready)
        {
            poseState = PoseStates.Waiting;
            UpdatePedestrians();
        }
    }

    private void CreatePedestrians()
    {
        for (int i = 0; i < numPedestrians; i++)
        {
            tempPedestrian = Instantiate(pedestrianPrefab);

            tempPos = poseArray.poses[i].position;  // position returned by car, relative to its coordinate system (should be the same as our origin)
            tempPedestrian.transform.position = new Vector3(tempPos.x, 0, tempPos.y);    // localPosition should set position relative to parent
            pedestrians.Add(tempPedestrian);
        }
        Debug.Log(numPedestrians + " pedestrians created!");
    }

    private void UpdatePedestrians()
    {
        if (pedestrians != null && pedestrians.Count > 0)
        {
            for (int i = 0; i < numPedestrians; i++)
            {
                tempPos = poseArray.poses[i].position;  // In Unity y vector is pointing up, while in ROS, z is up.
                pedestrians[i].transform.position = new Vector3(originCubePos.x + tempPos.x, originCubePos.y, originCubePos.z + tempPos.y); // might need position interpolation in frames which do not have a new pose. This would mean switching to a velocity model instead.
                pedestrians[i].transform.localRotation = new Quaternion(0, poseArray.poses[i].orientation.z, 0, poseArray.poses[i].orientation.w); // In Unity z axis is switched with y and needs to be made negative
            }
        }
        else
        {
            // To fix this from happening, you must queue every event in the HandlePosesUpdated function to the main queue so poseStatus doesn't get changed midway through Update(), this is a ductape fix
            CreatePedestrians();    // Unfortunately Unity is not Thread Safe
        }
    }

    private void HandlePosesUpdated(geometry_msgs.PoseArray poses)
    {
        if (rosReady)
        {
            if (firstSignal)
            {
                Debug.Log("First pose aquired");
                poseState = PoseStates.Initial;
                numPedestrians = poses.Count;
                firstSignal = false;
            }
            else
            {
                poseState = PoseStates.Ready;
            }
            poseArray = poses;
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Closed");
    }
}