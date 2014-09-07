using MidiDotNet;
using System.ComponentModel;
using VBone.Data;
using VBone.State;

namespace VBone.Logic
{
    public class TromboneNote : INotifyPropertyChanged
    {
        private Position position;
        private Harmonic harmonic;

        public TromboneNote(Position position, Harmonic harmonic)
        {
            this.Position = position;
            this.Harmonic = harmonic;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Position Position
        {
            get { return position; }
            set
            {
                position = value;
                this.OnPropertyChanged("Position");
                this.OnPropertyChanged("Pitch");
                this.OnPropertyChanged("NoteName");
            }
        }

        public Harmonic Harmonic
        {
            get { return harmonic; }
            set
            {
                harmonic = value;
                this.OnPropertyChanged("Harmonic");
                this.OnPropertyChanged("Pitch");
                this.OnPropertyChanged("NoteName");
            }
        }

        public Pitch Pitch
        {
            get
            {
                return TromboneLogic.GetPitch(this.Position, this.Harmonic);
            }
            set
            {

            }
        }

        public string NoteName
        {
            get
            {
                return SharpsOrFlatsState.PreferSharps ? this.Pitch.NotePreferringSharps().ToString() : this.Pitch.NotePreferringFlats().ToString();
            }
        }

        public override string ToString()
        {
            return this.NoteName;
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
