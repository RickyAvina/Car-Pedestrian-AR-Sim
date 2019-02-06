using System.Collections.Generic;

namespace RosSharp.RosBridgeClient.Messages.Geometry
{
    internal class PoseArray: Message
    {
        public const string RosMessageName = "geometry_msgs/PoseArray";
        public Standard.Header header;
        public List<Pose> poses;

        public int Count
        {
            get
            {
                return poses.Count;
            }
        }

        public PoseArray()
        {
            header = new Standard.Header();
            poses = new List<Pose>();
        }
    }
}