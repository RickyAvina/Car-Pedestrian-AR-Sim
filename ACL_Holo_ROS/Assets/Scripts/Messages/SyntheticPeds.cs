using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Messages.Geometry;
using RosSharp.RosBridgeClient.Messages.Standard;
using System.Collections.Generic;

class SyntheticPeds: Message
{ 
    public const string RosMessageName = "publish_peds/SyntheticPeds";
    public Header header;
    public List<Pose2D> poses;
    public List<Point> velocities;
    public List<Point> goal_positions;
    public List<float> radii;
    public List<float> pref_speeds;
    public List<int> ids;
    public List<Policies> policies;
    public List<AgentType> agentTypes;

    public SyntheticPeds()
    {
        header = new Header();
        poses = new List<Pose2D>();
        velocities = new List<Point>();
        goal_positions = new List<Point>();
        radii = new List<float>();
        pref_speeds = new List<float>();
        ids = new List<int>();
        policies = new List<Policies>();
        agentTypes = new List<AgentType>();
    }
}

public enum Policies
{
    STATIC,
    NONCOOPERATIVE,
    CADRL,
    GA3C_CADRL,
    RVO
}

public enum AgentType
{
    PEDESTRIAN,
    ROBOT,
    GOLF_CART
}