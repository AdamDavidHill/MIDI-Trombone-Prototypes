using KinectData;
using System;
using System.Linq;
using System.Diagnostics;
using System.Windows.Media;
using VBone.Events;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Threading;
using VBone.Drawing;

namespace VBone.Comms
{
    public class KinectAnalyser
    {
        private Process process;
        private Dispatcher dispacher;

        public KinectAnalyser(Dispatcher dispacher)
        {
            this.dispacher = dispacher;
            this.DrawingGroup = new DrawingGroup();
        }

        public event EventHandler MusicalDataUpdatedEvent = delegate { };

        public DrawingGroup DrawingGroup { get; set; }

        public double HarmonicHeight { get; set; }

        public double SlideDistance { get; set; }


        public void Init()
        {
            this.process = new Process
            {
                StartInfo =
                    new ProcessStartInfo
                    {
                        FileName = @"N:\Code\ComAdamDavidHill\Windows\VBone\KinectRelay\bin\Debug\KinectRelay.exe",
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    }
            };

            this.process.OutputDataReceived += new DataReceivedEventHandler(this.DataReceivedHandler);
            this.process.Start();
            this.process.BeginOutputReadLine();
        }

        public void Cleanup()
        {
            try
            {
                this.process.Close();
                this.process.Kill();
            }
            catch
            {
            }
        }

        private void DataReceivedHandler(object sender, DataReceivedEventArgs args)
        {
            var kinectPacket = this.ExtractData(args.Data);

            if (kinectPacket == null)
            {
                return;
            }

            this.dispacher.BeginInvoke((Action)(() => this.AnalyseData(kinectPacket)), null);
        }

        private KinectPacket86 ExtractData(string s)
        {
            try
            {
                return String.IsNullOrEmpty(s) ? null : new KinectPacket86(s);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void AnalyseData(KinectPacket86 data)
        {
            var jointsDictionary = data.Joints.ToDictionary(t => t.JointType, t => t);
            this.InterpretMusicalGestures(data, jointsDictionary);
            this.Draw(data, jointsDictionary, new Rect(0.0, 0.0, data.MiscInfo.KinectWidth, data.MiscInfo.KinectHeight));
        }

        private void InterpretMusicalGestures(KinectPacket86 data, Dictionary<JointType86, Joint86> joints)
        {
            if (!joints.ContainsKey(JointType86.HandLeft) || !joints.ContainsKey(JointType86.HandRight) || !joints.ContainsKey(JointType86.Head))
            {
                return;
            }

            // TODO: Alternative mapping for Harmonic Input?
            this.HarmonicHeight = joints[JointType86.HandLeft].Y;
            this.SlideDistance = this.Distance(joints[JointType86.HandRight], joints[JointType86.Head]);
            this.MusicalDataUpdatedEvent(this, new EventArgs());
        }

        /// <summary>
        /// Calculates the distance in metres between two joints
        /// </summary>
        /// <param name="a">Joint a</param>
        /// <param name="b">Joint b</param>
        /// <returns>The distance in metres</returns>
        private double Distance(Joint86 a, Joint86 b)
        {
            float x = a.X - b.X;
            float y = a.Y - b.Y;
            float z = a.Z - b.Z;

            return Math.Sqrt(x * x + y * y + z * z);
        }

        private void Draw(KinectPacket86 data, Dictionary<JointType86, Joint86> joints, Rect drawingArea)
        {
            using (var dc = this.DrawingGroup.Open())
            {
                // Drawing invisible rectangle to ensure uniform scaling between frames
                dc.DrawRectangle(Brushes.Transparent, null, drawingArea);

                //Debug.WriteLine(slideDistance.ToString());
                BodyRendererUtil.DrawClippedEdges(data.MiscInfo.TopClipped, data.MiscInfo.BottomClipped, data.MiscInfo.LeftClipped, data.MiscInfo.RightClipped, dc, data.MiscInfo.KinectWidth, data.MiscInfo.KinectHeight);
                Dictionary<JointType86, Point> jointPoints = new Dictionary<JointType86, Point>();

                foreach (var jointType in joints.Keys)
                {
                    jointPoints[jointType] = new Point(joints[jointType].X2d, joints[jointType].Y2d);
                }

                BodyRendererUtil.DrawBonesAndJoints(joints, jointPoints, dc);
                BodyRendererUtil.DrawHand(data.MiscInfo.HandLeftOpen, jointPoints[JointType86.HandLeft], dc);
                BodyRendererUtil.DrawHand(data.MiscInfo.HandRightOpen, jointPoints[JointType86.HandRight], dc);

                if (jointPoints.ContainsKey(JointType86.Head))
                {
                    BodyRendererUtil.DrawHand(handOpen: true, handPosition: jointPoints[JointType86.Head], drawingContext: dc);
                }
            }

            this.DrawingGroup.ClipGeometry = new RectangleGeometry(drawingArea);
        }
    }
}
