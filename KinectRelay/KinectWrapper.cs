using KinectData;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace KinectRelay
{
    public class KinectWrapper
    {
        private KinectSensor kinect;
        private BodyFrameReader bodyFrameReader;
        private Body[] bodies;
        private int kinectWidth;
        private int kinectHeight;
        private TimeSpan startTime;
        private DateTime nextStatusUpdate = DateTime.MinValue;
        private uint framesSinceUpdate = 0;
        private Stopwatch stopwatch = null;

        public KinectWrapper()
        {
            this.stopwatch = new Stopwatch();
        }

        public void Init()
        {
            this.kinect = KinectSensor.GetDefault();

            if (this.kinect == null)
            {
                return;
            }

            this.kinect.Open();
            this.kinectWidth = this.kinect.DepthFrameSource.FrameDescription.Width;
            this.kinectHeight = this.kinect.DepthFrameSource.FrameDescription.Height;
            this.bodyFrameReader = this.kinect.BodyFrameSource.OpenReader();

            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += BodyFrameReaderFrameArrived;
            }
        }

        public void Cleanup()
        {
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.kinect != null)
            {
                this.kinect.Close();
                this.kinect = null;
            }
        }

        private void BodyFrameReaderFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            if (this.startTime.Ticks == 0)
            {
                this.startTime = e.FrameReference.RelativeTime;
            }

            try
            {
                var frame = e.FrameReference.AcquireFrame();

                if (frame != null)
                {
                    using (frame)
                    {
                        this.framesSinceUpdate++;

                        if (DateTime.Now >= this.nextStatusUpdate)
                        {
                            double fps = 0.0;

                            if (this.stopwatch.IsRunning)
                            {
                                this.stopwatch.Stop();
                                fps = this.framesSinceUpdate / this.stopwatch.Elapsed.TotalSeconds;
                                this.stopwatch.Reset();
                            }

                            this.nextStatusUpdate = DateTime.Now + TimeSpan.FromSeconds(1);
                        }

                        if (!this.stopwatch.IsRunning)
                        {
                            this.framesSinceUpdate = 0;
                            this.stopwatch.Start();
                        }

                        if (this.bodies == null)
                        {
                            this.bodies = new Body[frame.BodyCount];
                        }

                        // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                        // As long as those body objects are not disposed and not set to null in the array,
                        // those body objects will be re-used.
                        frame.GetAndRefreshBodyData(this.bodies);

                        var body = this.bodies.Where(i => i.IsTracked).FirstOrDefault();
                        bool topClipped = body.ClippedEdges.HasFlag(FrameEdges.Top);
                        bool bottomClipped = body.ClippedEdges.HasFlag(FrameEdges.Bottom);
                        bool leftClipped = body.ClippedEdges.HasFlag(FrameEdges.Left);
                        bool rightClipped = body.ClippedEdges.HasFlag(FrameEdges.Right);
                        bool handLeftOpen = body.HandLeftState == HandState.Open;
                        bool handRightOpen = body.HandRightState == HandState.Open;
                        var miscInfo = new MiscInfo86(kinectWidth, kinectHeight, topClipped, bottomClipped, leftClipped, rightClipped, handLeftOpen, handRightOpen);
                        string output = new KinectPacket86(miscInfo, body.Joints.Select(i => this.ToJoint86(i.Value))).ToString();
                        Console.WriteLine(output);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private Joint86 ToJoint86(Joint joint)
        {
            var depthSpacePoint = this.kinect.CoordinateMapper.MapCameraPointToDepthSpace(joint.Position);

            return new Joint86((JointType86)(int)joint.JointType, (TrackingState86)(int)joint.TrackingState, joint.Position.X, joint.Position.Y, joint.Position.Z, depthSpacePoint.X, depthSpacePoint.Y);
        }

        /// <summary>
        /// Calculates the distance in metres between two joints
        /// </summary>
        /// <param name="a">Joint a</param>
        /// <param name="b">Joint b</param>
        /// <returns>The distance in metres</returns>
        private double Distance(Joint a, Joint b)
        {
            float x = a.Position.X - b.Position.X;
            float y = a.Position.Y - b.Position.Y;
            float z = a.Position.Z - b.Position.Z;

            return Math.Sqrt(x * x + y * y + z * z);
        }

    }
}
