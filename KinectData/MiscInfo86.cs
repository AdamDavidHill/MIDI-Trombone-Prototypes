using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectData
{
    public class MiscInfo86
    {
        private const string Delimiter = ",";

        public MiscInfo86(int kinectWidth, int kinectHeight, bool topClipped, bool bottomClipped, bool leftClipped, bool rightClipped, bool handLeftOpen, bool handRightOpen)
        {
            this.KinectWidth = kinectWidth;
            this.KinectHeight = kinectHeight;
            this.TopClipped = topClipped;
            this.BottomClipped = bottomClipped;
            this.LeftClipped = leftClipped;
            this.RightClipped = rightClipped;
            this.HandLeftOpen = handLeftOpen;
            this.HandRightOpen = handRightOpen;
        }

        public MiscInfo86(string serialisedJoint86)
        {
            var data = serialisedJoint86.Split(new[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries);
            this.KinectWidth = int.Parse(data[0]);
            this.KinectHeight = int.Parse(data[1]);
            this.TopClipped = bool.Parse(data[2]);
            this.BottomClipped = bool.Parse(data[3]);
            this.LeftClipped = bool.Parse(data[4]);
            this.RightClipped = bool.Parse(data[5]);
            this.HandLeftOpen = bool.Parse(data[6]);
            this.HandRightOpen = bool.Parse(data[7]);
        }

        public int KinectWidth { get; set; }
        public int KinectHeight { get; set; }
        public bool TopClipped { get; set; }
        public bool BottomClipped { get; set; }
        public bool LeftClipped { get; set; }
        public bool RightClipped { get; set; }
        public bool HandLeftOpen { get; set; }
        public bool HandRightOpen { get; set; }

        public override string ToString()
        {
            return this.KinectWidth.ToString()
                + Delimiter
                + this.KinectHeight.ToString()
                + Delimiter
                + this.TopClipped.ToString()
                + Delimiter
                + this.BottomClipped.ToString()
                + Delimiter
                + this.LeftClipped.ToString()
                + Delimiter
                + this.RightClipped.ToString()
                + Delimiter
                + this.HandLeftOpen.ToString()
                + Delimiter
                + this.HandRightOpen.ToString();
        }
    }
}
