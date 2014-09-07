using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace KinectData
{
    //public class KinectPacket
    //{
    //    private const string HeaderString = "KinectPacket:";
    //    private const string Delimiter = ",";

    //    public KinectPacket(double headX, double headY, double headZ, double handRightX, double handRightY, double handRightZ, double handLeftX, double handLeftY, double handLeftZ)
    //    {
    //        this.HeadX = headX;
    //        this.HeadY = headY;
    //        this.HeadZ = headZ;
    //        this.HandRightX = handRightX;
    //        this.HandRightY = handRightY;
    //        this.HandRightZ = handRightZ;
    //        this.HandLeftX = handLeftX;
    //        this.HandLeftY = handLeftY;
    //        this.HandLeftZ = handLeftZ;
    //    }

    //    public KinectPacket(string text)
    //    {
    //        string rawData = KinectPacket.TrimHeader(text);
    //        var data = rawData.Split(new[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries).ToList();

    //        if (data.Count != 9)
    //        {
    //            throw new ArgumentException("Invalid number of data elements");
    //        }

    //        this.HeadX = double.Parse(data[0]);
    //        this.HeadY = double.Parse(data[1]);
    //        this.HeadZ = double.Parse(data[2]);
    //        this.HandRightX = double.Parse(data[3]);
    //        this.HandRightY = double.Parse(data[4]);
    //        this.HandRightZ = double.Parse(data[5]);
    //        this.HandLeftX = double.Parse(data[6]);
    //        this.HandLeftY = double.Parse(data[7]);
    //        this.HandLeftZ = double.Parse(data[8]);
    //    }

    //    public double HeadX { get; set; }
    //    public double HeadY { get; set; }
    //    public double HeadZ { get; set; }
    //    public double HandRightX { get; set; }
    //    public double HandRightY { get; set; }
    //    public double HandRightZ { get; set; }
    //    public double HandLeftX { get; set; }
    //    public double HandLeftY { get; set; }
    //    public double HandLeftZ { get; set; }

    //    public override string ToString()
    //    {
    //        var sb = new StringBuilder();
    //        sb.Append(HeaderString);
    //        sb.Append(this.HeadX.ToString());
    //        sb.Append(Delimiter);
    //        sb.Append(this.HeadY.ToString());
    //        sb.Append(Delimiter);
    //        sb.Append(this.HeadZ.ToString());
    //        sb.Append(Delimiter);
    //        sb.Append(this.HandRightX.ToString());
    //        sb.Append(Delimiter);
    //        sb.Append(this.HandRightY.ToString());
    //        sb.Append(Delimiter);
    //        sb.Append(this.HandRightZ.ToString());
    //        sb.Append(Delimiter);
    //        sb.Append(this.HandLeftX.ToString());
    //        sb.Append(Delimiter);
    //        sb.Append(this.HandLeftY.ToString());
    //        sb.Append(Delimiter);
    //        sb.Append(this.HandLeftZ.ToString());

    //        return sb.ToString();
    //    }

    //    private static string TrimHeader(string text)
    //    {
    //        if (text.Length > HeaderString.Length)
    //        {
    //            string header = text.Substring(0, HeaderString.Length);

    //            if (header.Equals(HeaderString))
    //            {
    //                return header.Substring(HeaderString.Length);
    //            }
    //        }

    //        throw new ArgumentException("Invalid data");
    //    }
    //}
}