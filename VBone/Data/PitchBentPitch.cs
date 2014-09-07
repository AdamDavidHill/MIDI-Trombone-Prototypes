using MidiDotNet;
using System;
using System.Linq;
using VBone.Logic;

namespace VBone.Data
{
    public class PitchBentPitch
    {
        public const int MidiPitchBendMaxValue = 16384;
        public const int MidiPitchBendCenterValue = 8192;
        private TromboneNote note;
        private double percentThroughPositionRange;

        public PitchBentPitch(TromboneNote note, double percentThroughPositionRange = .5)
        {
            this.note = note;
            this.percentThroughPositionRange = percentThroughPositionRange;
        }

        /// <summary>
        /// Gets a value indicating the Pitch which should be sent to the output, only to be used in combination with the amount of pitch bend
        /// </summary>
        public Pitch MidiOutputPitch
        {
            get
            {
                return TromboneLogic.GetPitch(Position.Fourth, note.Harmonic);
            }
        }

        /// <summary>
        /// Gets a Midi data value indicating pitch bend value to adjust the MidiPitch by, from 0 to 16383, centre value 8192
        /// </summary>
        public int MidiPitchBendValue
        {
            get
            {
                const double ZoneSize = PitchBentPitch.MidiPitchBendMaxValue / 12.0;
                double centeredPositionPitchBendValue = PitchBentPitch.PitchBendValueFromPosition(this.note.Position);
                double rawValue = (int)(centeredPositionPitchBendValue + ZoneSize - this.percentThroughPositionRange * ZoneSize * 2);

                return (int)Math.Max(Math.Min(rawValue, PitchBentPitch.MidiPitchBendMaxValue - 1), 0);
            }
        }

        /// <summary>
        /// Returns the pitch bend value for a given position
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public static int PitchBendValueFromPosition(Position position)
        {
            int highestZeroBasedPosition = (int)Enum.GetValues(typeof(Position)).Cast<Position>().Max();
            int zeroBasedPosition = (int)position;
            int invertedZeroBasedPosition = highestZeroBasedPosition - zeroBasedPosition;

            return invertedZeroBasedPosition * (PitchBentPitch.MidiPitchBendMaxValue / highestZeroBasedPosition);

            // Explicit version:
            //switch (position)
            //{
            //    case Position.First:
            //        return 16383;
            //    case Position.Second:
            //        return 13653;
            //    case Position.Third:
            //        return 10922;
            //    case Position.Fourth:
            //        return 8192;
            //    case Position.Fifth:
            //        return 5461;
            //    case Position.Sixth:
            //        return 2731;
            //    case Position.Seventh:
            //        return 0;
            //    default:
            //        return 8192;
            //}
        }

        /// <summary>
        /// Gets a value indicating the number of semitones, positive or negative from 0 to 3, to pitch bend the Midi note pitch
        /// </summary>
        private int PositionAsSemitoneOffsetFromCentre
        {
            get
            {
                return ((int)note.Position + 1) - 4;
            }
        }
    }
}
