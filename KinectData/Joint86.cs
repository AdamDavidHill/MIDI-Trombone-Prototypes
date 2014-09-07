using Microsoft.Kinect;
using System;

namespace KinectData
{
    public class Joint86
    {
        private const string Delimiter = ",";

        public JointType86 JointType { get; set; }
        public TrackingState86 TrackingState { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float X2d { get; set; }
        public float Y2d { get; set; }

        public Joint86(JointType86 jointType, TrackingState86 trackingState, float x, float y, float z, float x2d, float y2d)
        {
            this.JointType = jointType;
            this.TrackingState = trackingState;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.X2d = x2d;
            this.Y2d = y2d;
        }

        public Joint86(string serialisedJoint86)
        {
            var data = serialisedJoint86.Split(new[] { Delimiter }, StringSplitOptions.None);
            this.JointType = (JointType86)int.Parse(data[0]);
            this.TrackingState = (TrackingState86)int.Parse(data[1]);
            this.X = float.Parse(this.Coalesce(data[2]));
            this.Y = float.Parse(this.Coalesce(data[3]));
            this.Z = float.Parse(this.Coalesce(data[4]));
            this.X2d = float.Parse(this.Coalesce(data[5]));
            this.Y2d = float.Parse(this.Coalesce(data[6]));
        }

        public override string ToString()
        {
            const string FormatTemplate = "#.##";

            return ((int)this.JointType).ToString()
                + Delimiter
                + ((int)this.TrackingState).ToString()
                + Delimiter
                + this.X.ToString(FormatTemplate)
                + Delimiter
                + this.Y.ToString(FormatTemplate)
                + Delimiter
                + this.Z.ToString(FormatTemplate)
                + Delimiter
                + this.X2d.ToString(FormatTemplate)
                + Delimiter
                + this.Y2d.ToString(FormatTemplate);
        }

        private string Coalesce(string s)
        {
            return string.IsNullOrEmpty(s) ? "0" : s;
        }
    }
}
