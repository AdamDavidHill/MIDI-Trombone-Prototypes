using MidiDotNet;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VBone.Data;
using VBone.Logic;

namespace VBone.Comms
{
    public static class MidiDevice
    {
        private const string MidiPortName = "VBone";
        public static OutputDevice MidiOut = OutputDevice.InstalledDevices.Where(i => i.Name == MidiPortName).SingleOrDefault();
        public static InputDevice BreathControllerMidiIn = InputDevice.InstalledDevices.Where(i => i.Name.Contains("Breath Controller")).SingleOrDefault();
        private static Pitch lastPitch;
        private static TromboneNote lastTromboneNote;
        private static Dictionary<Pitch, bool> isNoteOn = new Dictionary<Pitch, bool>();

        public static void OpenPorts()
        {
            if (!MidiOut.IsOpen)
            {
                MidiOut.Open();
            }

            if (BreathControllerMidiIn != null && !BreathControllerMidiIn.IsOpen)
            {
                BreathControllerMidiIn.Open();
                BreathControllerMidiIn.ControlChange += BreathControllerMidiIn_ControlChange;
                BreathControllerMidiIn.StartReceiving(null);
            }
        }

        public static void ClosePort()
        {
            if (!MidiOut.IsOpen)
            {
                MidiOut.Close();
            }
        }

        public static void SendTromboneNoteOn(TromboneNote note, int velocity = 127, bool stopPrevious = true, bool overlapNoteOff = false, double percentThroughPosition = .5)
        {
            var pitchBendPitch = new PitchBentPitch(note, percentThroughPosition);

            if (velocity > 127)
            {
                velocity = 127;
            }

            if (stopPrevious && !overlapNoteOff && lastTromboneNote != null)
            {
                SendTromboneNoteOff(lastTromboneNote);
            }

            UpdatePitchBend(pitchBendPitch.MidiPitchBendValue);
            SendNoteOn(pitchBendPitch.MidiOutputPitch, velocity);

            if (stopPrevious && overlapNoteOff && lastTromboneNote != null)
            {
                SendTromboneNoteOff(lastTromboneNote);
            }

            lastTromboneNote = note;
        }

        static bool hasHadNoteOn;
        public static void SendTromboneNoteOnAbsolutePitchBend(TromboneNote note, int velocity = 127, bool stopPrevious = true, bool overlapNoteOff = false, double percentPitchBend = .5)
        {
            if (velocity > 127)
            {
                velocity = 127;
            }

            if (stopPrevious && !overlapNoteOff && lastTromboneNote != null)
            {
                if (hasHadNoteOn)
                {
                    SendTromboneNoteOff(lastTromboneNote);
                }
            }

            //int pitchBendValue = PitchBentPitch.MidiPitchBendCenterValue + (int)((double)(PitchBentPitch.MidiPitchBendCenterValue - 1) * percentPitchBend);
            int pitchBendValue = (int)((PitchBentPitch.MidiPitchBendMaxValue - 1) * percentPitchBend);
            if (hasHadNoteOn)
            {
                UpdatePitchBend(pitchBendValue);

            }
            else
            {
                hasHadNoteOn = true;
            }
            SendNoteOn(TromboneLogic.GetPitch(Position.Fourth, note.Harmonic), velocity);
            //Debug.WriteLine("Original Note: " + note.ToString());

            if (stopPrevious && overlapNoteOff && lastTromboneNote != null)
            {
                SendTromboneNoteOff(lastTromboneNote);
            }

            lastTromboneNote = note;
        }

        public static void SendTromboneNoteChanged(TromboneNote note, double percentThroughPosition = .5)
        {
            int value = new PitchBentPitch(note, percentThroughPosition).MidiPitchBendValue;
            UpdatePitchBend(value);


            if (lastTromboneNote != null && note.Harmonic != lastTromboneNote.Harmonic)
            {
                // Cross harmonic slur
                var cachedLastTromboneNote = lastTromboneNote;
                SendTromboneNoteOn(note, overlapNoteOff: true, percentThroughPosition: percentThroughPosition);
            }
        }

        public static void SendTromboneNoteChangedAbsolutePitchBend(TromboneNote note, double percentPitchBend = .5)
        {
            //int value = PitchBentPitch.MidiPitchBendCenterValue + (int)((double)(PitchBentPitch.MidiPitchBendCenterValue - 1) * percentPitchBend);
            int value = (int)((PitchBentPitch.MidiPitchBendMaxValue - 1) * percentPitchBend);
            UpdatePitchBend(value);
            //Debug.WriteLine("Pitch Bend: " + value.ToString() + " - " + percentPitchBend.ToString("#.##"));
            //Debug.WriteLine("Pitch Bend: " + value.ToString());

            if (lastTromboneNote != null && note.Harmonic != lastTromboneNote.Harmonic)
            {
                // Cross harmonic slur
                var cachedLastTromboneNote = lastTromboneNote;
                SendTromboneNoteOnAbsolutePitchBend(note, overlapNoteOff: true, percentPitchBend: percentPitchBend);
                //Debug.WriteLine("NoteOn: " + note.ToString());
            }
        }

        public static void SendTromboneNoteOff(TromboneNote note)
        {
            SendPitchBentPitchOff(new PitchBentPitch(note));
        }

        public static void SendPitchBentPitchOff(PitchBentPitch note)
        {
            SendNoteOff(note.MidiOutputPitch);
        }

        public static void SendNoteOn(Pitch pitch, int velocity = 127, bool stopPrevious = true, bool overlapNoteOff = false)
        {
            var cachedLastPitch = lastPitch;

            if (velocity > 127)
            {
                velocity = 127;
            }

            if (stopPrevious && !overlapNoteOff)
            {
                SendNoteOff(cachedLastPitch);
            }

            MidiOut.SendNoteOn(Channel.Channel1, pitch, velocity);
            lastPitch = pitch;
            isNoteOn[pitch] = true;

            if (stopPrevious && overlapNoteOff)
            {
                SendNoteOff(cachedLastPitch);
            }
        }

        public static void SendNoteOff(Pitch pitch)
        {
            MidiOut.SendNoteOff(Channel.Channel1, pitch, 0);
            isNoteOn[pitch] = false;
        }

        public static void UpdatePitchBend(int value)
        {
            MidiOut.SendPitchBend(Channel.Channel1, value);
        }

        private static void BreathControllerMidiIn_ControlChange(ControlChangeMessage msg)
        {
            if (msg.Control == Control.BreathControl)
            {
                // Send Expression or Breath control depending on configuration of VMI
                MidiOut.SendControlChange(Channel.Channel1, Control.BreathControl, msg.Value);

                // TODO: For Kinect-style interaction:
                // If new value - prev value > threshold
                // then noteon
                // else
                // update volume
            }
        }
    }
}
