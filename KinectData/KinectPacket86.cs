using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Kinect;

namespace KinectData
{
    public class KinectPacket86
    {
        private const string HeaderString = "KP:";
        private const string TypeDelimiter = "@";
        private const string InstanceDelimiter = "|";
        private MiscInfo86 miscInfo;
        private IEnumerable<Joint86> joints;

        public KinectPacket86(MiscInfo86 miscInfo, IEnumerable<Joint86> joints)
        {
            this.MiscInfo = miscInfo;
            this.Joints = joints;
        }

        public KinectPacket86(string text)
        {
            string rawData = KinectPacket86.TrimHeader(text);
            var mixedData = rawData.Split(new[] { TypeDelimiter }, StringSplitOptions.RemoveEmptyEntries).ToList();
            this.miscInfo = new MiscInfo86(mixedData[0].Split(new[] { InstanceDelimiter }, StringSplitOptions.RemoveEmptyEntries).Single());
            var jointData = mixedData[1].Split(new[] { InstanceDelimiter }, StringSplitOptions.RemoveEmptyEntries).ToList();
            this.Joints = jointData.Select(i => new Joint86(i));
        }

        public MiscInfo86 MiscInfo { get { return this.miscInfo; } set { this.miscInfo = value; } }

        public IEnumerable<Joint86> Joints { get { return this.joints; } set { this.joints = value; } }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(HeaderString);
            sb.Append(this.miscInfo.ToString());
            sb.Append(TypeDelimiter);

            foreach (var joint in this.joints)
            {
                sb.Append(joint.ToString());
                sb.Append(InstanceDelimiter);
            }

            return sb.ToString();
        }

        private static string TrimHeader(string text)
        {
            if (text.Length > HeaderString.Length)
            {
                string header = text.Substring(0, HeaderString.Length);

                if (header.Equals(HeaderString))
                {
                    return text.Substring(HeaderString.Length);
                }
            }

            return "Invalid data - header not found";
        }
    }
}