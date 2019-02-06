using RosSharp.RosBridgeClient.Messages.Geometry;
using System;
using UnityEngine;

public class Pedestrian {

    public GameObject obj;

    public Policies policy;
    public AgentType agentType;
    public Pose2D pose;
    public Point velocity;
    public Point goalPosition;
    public float radius;
    public float prefSpeed;
    public int id;

    public Animator m_Animator;
    public float speed
    {
        get
        {
            return (float)Math.Sqrt(Math.Pow(velocity.x, 2) + Math.Pow(velocity.y, 2) + Math.Pow(velocity.z, 2));
        }
    }
    /*
    public int policy;
    public int type;
    */

    public Pedestrian(int id)
    {
        this.id = id;
    }

    public Pedestrian(int id, float radius, Policies policy=Policies.RVO, AgentType agentType=AgentType.PEDESTRIAN)
    {
        this.id = id;
        this.radius = radius;        
    }

    public Pedestrian(int id, Pose2D pose, Point velocity, Point goalPosition, float radius, float prefSpeed, Policies policy=Policies.RVO, AgentType agentType=AgentType.PEDESTRIAN)
    {
        this.id = id;
        this.pose = pose;
        this.velocity = velocity;
        this.goalPosition = goalPosition;
        this.radius = radius;
        this.prefSpeed = prefSpeed;
        this.policy = policy;
        this.agentType = agentType;
    }
}
