using UnityEngine;

namespace RosSharp.RosBridgeClient.Messages.Geometry
{
    public class Pose2D : Message
    {
        public const string RosMessageName = "geometry_msgs/Pose2D";
        public Standard.Header header;
        public float x;
        public float y;
        public float theta;

        public Pose2D()
        {
            header = new Standard.Header();
            x = 0;
            y = 0;
            theta = 0;
        }

        public UnityEngine.Vector3 position
        {
            get
            {
                return new UnityEngine.Vector3(x, 0, y);    // In Unity, y is up
            }
        }

        public UnityEngine.Quaternion rotation
        {
            get
            {
                //Debug.Log(theta*180/Mathf.PI);
                return UnityEngine.Quaternion.Euler(0, -theta * 180 / Mathf.PI + 90, 0);
                //return UnityEngine.Quaternion.Euler(0, -theta*180/Mathf.PI + 90, 0);    // z and y axis are swapped for unity and ROS
            }
        }
    }
}