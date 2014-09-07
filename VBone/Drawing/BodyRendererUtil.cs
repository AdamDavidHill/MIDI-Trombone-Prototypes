using KinectData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VBone.Drawing
{
    public class BodyRendererUtil
    {
        private static Color blueColour = Color.FromArgb(128, 0, 100, 255);
        private static Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
        private static Brush handOpenBrush = new SolidColorBrush(blueColour);
        private static Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));
        private static Brush trackedJointBrush = handOpenBrush;
        private static Brush inferredJointBrush = Brushes.Red;
        private static Pen trackedBonePen = new Pen(handOpenBrush, 3);
        private static Pen inferredBonePen = new Pen(Brushes.Gray, 1);
        private const double HandSize = 30;
        private const double JointThickness = 5;
        private const double ClipBoundsThickness = 10;

        public static void DrawBonesAndJoints(IReadOnlyDictionary<JointType86, Joint86> joints, IDictionary<JointType86, Point> jointPoints, DrawingContext drawingContext)
        {
            // Draw the bones

            // Torso
            DrawBone(joints, jointPoints, JointType86.Head, JointType86.Neck, drawingContext);
            DrawBone(joints, jointPoints, JointType86.Neck, JointType86.SpineShoulder, drawingContext);
            DrawBone(joints, jointPoints, JointType86.SpineShoulder, JointType86.SpineMid, drawingContext);
            //DrawBone(joints, jointPoints, JointType86.SpineMid, JointType86.SpineBase, drawingContext);
            //DrawBone(joints, jointPoints, JointType86.SpineShoulder, JointType86.ShoulderRight, drawingContext);
            //DrawBone(joints, jointPoints, JointType86.SpineShoulder, JointType86.ShoulderLeft, drawingContext);
            //DrawBone(joints, jointPoints, JointType86.SpineBase, JointType86.HipRight, drawingContext);
            //DrawBone(joints, jointPoints, JointType86.SpineBase, JointType86.HipLeft, drawingContext);

            // Right Arm    
            DrawBone(joints, jointPoints, JointType86.ShoulderRight, JointType86.ElbowRight, drawingContext);
            DrawBone(joints, jointPoints, JointType86.ElbowRight, JointType86.WristRight, drawingContext);
            DrawBone(joints, jointPoints, JointType86.WristRight, JointType86.HandRight, drawingContext);
            DrawBone(joints, jointPoints, JointType86.HandRight, JointType86.HandTipRight, drawingContext);
            DrawBone(joints, jointPoints, JointType86.WristRight, JointType86.ThumbRight, drawingContext);

            // Left Arm
            DrawBone(joints, jointPoints, JointType86.ShoulderLeft, JointType86.ElbowLeft, drawingContext);
            DrawBone(joints, jointPoints, JointType86.ElbowLeft, JointType86.WristLeft, drawingContext);
            DrawBone(joints, jointPoints, JointType86.WristLeft, JointType86.HandLeft, drawingContext);
            DrawBone(joints, jointPoints, JointType86.HandLeft, JointType86.HandTipLeft, drawingContext);
            DrawBone(joints, jointPoints, JointType86.WristLeft, JointType86.ThumbLeft, drawingContext);

            List<JointType86> usefulJoints = new List<JointType86> { JointType86.ElbowLeft, JointType86.ElbowRight, JointType86.HandTipLeft, JointType86.HandTipRight, JointType86.Neck, JointType86.ShoulderLeft, JointType86.ShoulderRight, JointType86.SpineMid, JointType86.ThumbLeft, JointType86.ThumbRight, JointType86.WristLeft, JointType86.WristRight };

            // Draw the joints
            foreach (JointType86 jointType in joints.Keys.Where(i => usefulJoints.Contains(i)))
            {
                Brush drawBrush = null;
                TrackingState86 trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState86.Tracked)
                {
                    drawBrush = trackedJointBrush;
                }
                else if (trackingState == TrackingState86.Inferred)
                {
                    drawBrush = inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            }
        }

        private static void DrawBone(IReadOnlyDictionary<JointType86, Joint86> joints, IDictionary<JointType86, Point> jointPoints, JointType86 jointType0, JointType86 jointType1, DrawingContext drawingContext)
        {
            Joint86 joint0 = joints[jointType0];
            Joint86 joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState86.NotTracked || joint1.TrackingState == TrackingState86.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == TrackingState86.Inferred &&
                joint1.TrackingState == TrackingState86.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = inferredBonePen;
            if ((joint0.TrackingState == TrackingState86.Tracked) && (joint1.TrackingState == TrackingState86.Tracked))
            {
                drawPen = trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }

        /// <summary>
        /// Draws a hand symbol if the hand is tracked: red circle = closed, green circle = opened; blue circle = lasso
        /// </summary>
        /// <param name="handState">state of the hand</param>
        /// <param name="handPosition">position of the hand</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        public static void DrawHand(bool handOpen, Point handPosition, DrawingContext drawingContext)
        {
            drawingContext.DrawEllipse(handOpen ? handOpenBrush : handClosedBrush, null, handPosition, HandSize, HandSize);
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping body data
        /// </summary>
        /// <param name="body">body to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        public static void DrawClippedEdges(bool topClipped, bool bottomClipped, bool leftClipped, bool rightClipped, DrawingContext drawingContext, int kinectWidth, int kinectHeight)
        {
            if (topClipped)
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, kinectWidth, ClipBoundsThickness));
            }

            //if (bottomClipped)
            //{
            //    drawingContext.DrawRectangle(
            //        Brushes.Red,
            //        null,
            //        new Rect(0, kinectHeight - ClipBoundsThickness, kinectWidth, ClipBoundsThickness));
            //}

            if (leftClipped)
            {
                drawingContext.DrawRectangle(
                    handClosedBrush,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, kinectHeight));
            }

            if (rightClipped)
            {
                drawingContext.DrawRectangle(
                    handClosedBrush,
                    null,
                    new Rect(kinectWidth - ClipBoundsThickness, 0, ClipBoundsThickness, kinectHeight));
            }
        }
    }
}
