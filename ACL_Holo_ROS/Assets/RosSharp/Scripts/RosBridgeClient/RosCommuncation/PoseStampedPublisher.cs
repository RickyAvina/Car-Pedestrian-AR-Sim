/*
© Siemens AG, 2017-2018
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class PoseStampedPublisher : Publisher<Messages.Geometry.PoseStamped>
    {
        public Transform PublishedTransform;
        public Transform Origin;
        private Vector3 relPos;

        public string FrameId = "unity_world";

        private Messages.Geometry.PoseStamped message;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
            relPos = Vector3.zero;
        }

        private void Update()
        {
            UpdateMessage();
        }

        private void InitializeMessage()
        {
            message = new Messages.Geometry.PoseStamped
            {
                header = new Messages.Standard.Header()
                {
                    frame_id = FrameId
                }
            };
        }

        private void UpdateMessage()
        {
            message.header.Update();
            Vector3 res = Origin.InverseTransformPoint(PublishedTransform.position);
            res = new Vector3(res.x/12.0f, res.z/12.0f, res.y/12.0f);

            //Vector3 res = new Vector3(-Origin.InverseTransformPoint(PublishedTransform.position).x, Origin.InverseTransformPoint(PublishedTransform.position).y, Origin.InverseTransformPoint(PublishedTransform.position).z);
            //Debug.Log(res);

            message.pose.position = GetGeometryPoint(res);  // changed from Unity2Ros() we are only changing x and z axis because of our coordinate frame
            message.pose.orientation = GetGeometryQuaternion(rotationTransformation());
            //message.pose.orientation = GetGeometryQuaternion(PublishedTransform.rotation.Unity2Ros());

            Publish(message);
        }

        private Quaternion rotationTransformation()
        {
            //Vector3 currEuler = new Vector3(PublishedTransform.transform.rotation.x, -PublishedTransform.transform.rotation.z, -PublishedTransform.transform.rotation.y);
            Quaternion relative = Origin.rotation * PublishedTransform.rotation;
            relative = new Quaternion(-relative.x, relative.z, -relative.y, relative.w);
            //Quaternion rotationAmount = Quaternion.Euler(0, 0, 90);

            //return Quaternion.Euler(currEuler);
            return relative; // switching z and y worked. x is pos
        }

        private Messages.Geometry.Point GetGeometryPoint(Vector3 position)
        {
            Messages.Geometry.Point geometryPoint = new Messages.Geometry.Point();
            geometryPoint.x = position.x;
            geometryPoint.y = position.y;
            geometryPoint.z = position.z;
            return geometryPoint;
        }

        private Messages.Geometry.Quaternion GetGeometryQuaternion(Quaternion quaternion)
        {
            Messages.Geometry.Quaternion geometryQuaternion = new Messages.Geometry.Quaternion();
            geometryQuaternion.x = quaternion.x;
            geometryQuaternion.y = quaternion.y;
            geometryQuaternion.z = quaternion.z;
            geometryQuaternion.w = quaternion.w;
            return geometryQuaternion;
        }
    }
}
