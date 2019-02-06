using RosSharp.RosBridgeClient;
using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

// <Summary>
// @author: Enrique Avina 2019
// This file acts as the bridge between ROS communication and pedestrian behavior.
// This is achieved through Callbacks from ROS. Complile with /doc to search for XML documentation.
// </Summary>
public class PedestrianController_EditorOnly : MonoBehaviour
{
    public enum PoseStates
    {
        NotReady,
        Initial,
        Ready,
        Waiting,
    }

    [Tooltip("The prefab for the pedestrian")]
    public GameObject pedestrianPrefab;
    [Tooltip("Your ROS uri with the port 9090 (used by websocket)")]
    private string serverURL = "ws://192.168.0.19:9090";

    [Tooltip("The speed modifier for your pedestrian animation")]
    public float speed_multiplier = 0.5f;
    [Tooltip("The base speed of your pedestrian animation")]
    public float base_speed = 1.6f;

    private PoseStates poseState = PoseStates.NotReady;     // Keeps track of the state of the incoming poses
    private bool rosReady = false;                          // ROS Connectivity callback indicator
    private bool firstSignal = true;                        // used for determining if pose status is being recieved for the first time
    private RosConnector rosConnector;                      // RosConnector has a RosSocket which handles all the networking between Linux and Hololens systems
    private List<Pedestrian> pedestrians;                   // A List containing every Pedestrian object which includes the GameObject and all of its data
    private bool pedestriansCreated = false;                // Keeps track if the pedestrians have been created
    private int numPedestrians = 0;                         // Keeps track of the number of pedestrians (currently only beign set once)

    public void Start()
    {
        pedestrians = new List<Pedestrian>();
        rosConnector = GetComponent<RosConnector>();
        serverURL = rosConnector.RosBridgeServerUrl;
        RosConnector.ConnectToRos(RosConnector.Protocols.WebSocketSharp, serverURL, OnConnected);
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

    // <Summary>
    // GameObjects of the pedestrian prefab are instantiated here and given the values loaded at the 
    // HandlePosesUpdated callback function
    // </Summary>
    private void CreatePedestrians()
    {
        for (int i = 0; i < numPedestrians; i++)
        {
            pedestrians[i].obj = Instantiate(pedestrianPrefab);
            pedestrians[i].obj.transform.localScale = new Vector3(pedestrians[i].radius, pedestrians[i].radius, pedestrians[i].radius);
            //pedestrians[i].obj.transform.Find("Disk").transform.localScale = new Vector3(pedestrians[i].radius, 0.2f, pedestrians[i].radius);
            pedestrians[i].obj.transform.position = pedestrians[i].pose.position;   // this position is relative to the origin
            pedestrians[i].obj.transform.rotation = pedestrians[i].pose.rotation;

            if (pedestrians[i].agentType == AgentType.PEDESTRIAN)
            {
                pedestrians[i].m_Animator = pedestrians[i].obj.GetComponent<Animator>();
                pedestrians[i].m_Animator.speed = base_speed + pedestrians[i].speed * speed_multiplier;     // make the animation speed relative to the speed of the pedestrian
            }
        }

        pedestriansCreated = true;
        Debug.Log(numPedestrians + " pedestrians created!");
    }

    // <Summary>
    // UpdatePedestrians will give positional data to the pedestrian prefabs. Future implementations
    // will include directional arrows and visual indicators for their respective goals.
    // </Summary>
    private void UpdatePedestrians()
    {
        if (pedestrians != null && pedestrians.Count > 0)   // may or may not need this check
        {
            for (int i = 0; i < numPedestrians; i++)
            {
                pedestrians[i].obj.transform.position = pedestrians[i].pose.position;
                pedestrians[i].obj.transform.rotation = pedestrians[i].pose.rotation;
            }
        }
    }

    // <summary> 
    // HandlePosesUpdated is a callback function from the ROS Subscriber that handles incoming messages.
    // When a message is initially recieved, positional data about the pedestrians are updated, but the GameObjects
    // are not updated, because you cannot make UI calls within a thread other than the main thread (Update)
    // </summary>
    private void HandlePosesUpdated(SyntheticPeds message)
    {
        if (rosReady)     // don't start creating peds until mapping is finished and ROS connected
        {
            if (firstSignal)
            {
                firstSignal = false;
                numPedestrians = message.poses.Count;

                for (int i = 0; i < numPedestrians; i++)    // Create new pedestrians and fill in data that only needs to be read once
                {
                    pedestrians.Add(new Pedestrian(message.ids[i], message.radii[i]));
                }

                Debug.Log("First pose aquired");
            }
            else 
            {
                for (int i = 0; i < numPedestrians; i++)    // update values of all pedestrians
                {
                    pedestrians[i].id = message.ids[i];
                    pedestrians[i].pose = message.poses[i];
                    pedestrians[i].velocity = message.velocities[i];
                    pedestrians[i].goalPosition = message.goal_positions[i];
                    pedestrians[i].prefSpeed = pedestrians[i].prefSpeed;
                }

                if (poseState == PoseStates.NotReady)   // We do not want to create pedestrians in CreatePedestrians() until their vals have been initialized
                {
                    poseState = PoseStates.Initial;
                }
                else if (pedestriansCreated)
                {
                    poseState = PoseStates.Ready;
                }
            }
        }
    }

    // <summary>
    // subscribes to indicated topic and expects a message of type SyntheticPedestrian
    // </summary>
    private void OnConnected(object sender, EventArgs e)
    {
        rosConnector.RosSocket.Subscribe<SyntheticPeds>("/ped_sim/synthetic_pedestrians", HandlePosesUpdated);
        rosReady = true;
        Debug.Log("Subscribed to /ped_sim/synthetic_pedestrians");
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Closed");
    }
}