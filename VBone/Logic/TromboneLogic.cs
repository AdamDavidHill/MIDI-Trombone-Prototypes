using MidiDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VBone.Data;

namespace VBone.Logic
{
    public static class TromboneLogic
    {
        public static Pitch GetPitch(Position position, Harmonic harmonic)
        {
            switch (harmonic)
            {
                case Harmonic.Fundamental:
                    return (Pitch)((int)Pitch.ASharp1 - (int)position);
                case Harmonic.Second:
                    return (Pitch)((int)Pitch.ASharp2 - (int)position);
                case Harmonic.Third:
                    return (Pitch)((int)Pitch.F3 - (int)position);
                case Harmonic.Fourth:
                    return (Pitch)((int)Pitch.ASharp3 - (int)position);
                case Harmonic.Fifth:
                    return (Pitch)((int)Pitch.D4 - (int)position);
                case Harmonic.Sixth:
                    return (Pitch)((int)Pitch.F4 - (int)position);
                case Harmonic.Seventh:
                    return (Pitch)((int)Pitch.GSharp4 - (int)position);
                case Harmonic.Eighth:
                    return (Pitch)((int)Pitch.ASharp4 - (int)position);
                default:
                    return Pitch.C4;
            }
        }
    }
}
