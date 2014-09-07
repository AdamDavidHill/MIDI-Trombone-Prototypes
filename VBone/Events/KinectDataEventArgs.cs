using KinectData;
using System;
using System.Collections.Generic;

namespace VBone.Events
{
    public class KinectDataEventArgs : EventArgs
    {
        public KinectPacket86 Data { get; private set; }

        private KinectDataEventArgs() { }

        public KinectDataEventArgs(KinectPacket86 data)
        {
            this.Data = data;
        }
    }
}
