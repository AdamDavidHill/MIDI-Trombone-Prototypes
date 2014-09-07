using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VBone.Data;
using VBone.Logic;

namespace VBone.Tests
{
    [TestClass]
    public class PitchBendPitchTests
    {
        [TestMethod]
        public void PitchBendPitch_BFlat2ndHarm_IsMaxPitchBendValue()
        {
            var note = new TromboneNote(Position.First, Harmonic.Second);
            var pitch = new PitchBentPitch(note, percentThroughPositionRange: 0);

            Assert.AreEqual(PitchBentPitch.MidiPitchBendMaxValue - 1, pitch.MidiPitchBendValue);
        }

        [TestMethod]
        public void PitchBendPitch_E2ndHarm_IsLowValue()
        {
            var note = new TromboneNote(Position.Seventh, Harmonic.Second);
            var pitch = new PitchBentPitch(note, percentThroughPositionRange: 0);

            Assert.AreNotEqual(0, pitch.MidiPitchBendValue);
        }
    }
}
