using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VBone
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

        public static void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext)
        {
            // Draw the bones

            // Torso
            DrawBone(joints, jointPoints, JointType.Head, JointType.Neck, drawingContext);
            DrawBone(joints, jointPoints, JointType.Neck, JointType.SpineShoulder, drawingContext);
            DrawBone(joints, jointPoints, JointType.SpineShoulder, JointType.SpineMid, drawingContext);
            //DrawBone(joints, jointPoints, JointType.SpineMid, JointType.SpineBase, drawingContext);
            //DrawBone(joints, jointPoints, JointType.SpineShoulder, JointType.ShoulderRight, drawingContext);
            //DrawBone(joints, jointPoints, JointType.SpineShoulder, JointType.ShoulderLeft, drawingContext);
            //DrawBone(joints, jointPoints, JointType.SpineBase, JointType.HipRight, drawingContext);
            //DrawBone(joints, jointPoints, JointType.SpineBase, JointType.HipLeft, drawingContext);

            // Right Arm    
            DrawBone(joints, jointPoints, JointType.ShoulderRight, JointType.ElbowRight, drawingContext);
            DrawBone(joints, jointPoints, JointType.ElbowRight, JointType.WristRight, drawingContext);
            DrawBone(joints, jointPoints, JointType.WristRight, JointType.HandRight, drawingContext);
            DrawBone(joints, jointPoints, JointType.HandRight, JointType.HandTipRight, drawingContext);
            DrawBone(joints, jointPoints, JointType.WristRight, JointType.ThumbRight, drawingContext);

            // Left Arm
            DrawBone(joints, jointPoints, JointType.ShoulderLeft, JointType.ElbowLeft, drawingContext);
            DrawBone(joints, jointPoints, JointType.ElbowLeft, JointType.WristLeft, drawingContext);
            DrawBone(joints, jointPoints, JointType.WristLeft, JointType.HandLeft, drawingContext);
            DrawBone(joints, jointPoints, JointType.HandLeft, JointType.HandTipLeft, drawingContext);
            DrawBone(joints, jointPoints, JointType.WristLeft, JointType.ThumbLeft, drawingContext);

            List<JointType> usefulJoints = new List<JointType> { JointType.ElbowLeft, JointType.ElbowRight, JointType.HandTipLeft, JointType.HandTipRight, JointType.Neck, JointType.ShoulderLeft, JointType.ShoulderRight, JointType.SpineMid, JointType.ThumbLeft, JointType.ThumbRight, JointType.WristLeft, JointType.WristRight };

            // Draw the joints
            foreach (JointType jointType in joints.Keys.Where(i => usefulJoints.Contains(i)))
            {
                Brush drawBrush = null;
                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = trackedJointBrush;
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            }
        }

        private static void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == TrackingState.Inferred &&
                joint1.TrackingState == TrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = inferredBonePen;
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
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
        public static void DrawHand(HandState handState, Point handPosition, DrawingContext drawingContext)
        {
            switch (handState)
            {
                case HandState.Closed:
                    drawingContext.DrawEllipse(handClosedBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Open:
                    drawingContext.DrawEllipse(handOpenBrush, null, handPosition, HandSize, HandSize);
                    break;

                case HandState.Lasso:
                    drawingContext.DrawEllipse(handLassoBrush, null, handPosition, HandSize, HandSize);
                    break;
            }
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping body data
        /// </summary>
        /// <param name="body">body to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        public static void DrawClippedEdges(Body body, DrawingContext drawingContext, int kinectWidth, int kinectHeight)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, kinectHeight - ClipBoundsThickness, kinectWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, kinectWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, kinectHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(kinectWidth - ClipBoundsThickness, 0, ClipBoundsThickness, kinectHeight));
            }
        }
    }
}
