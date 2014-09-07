using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using VBone.Comms;
using VBone.Logic;
using VBone.UserControls;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using VBone.Data;
using System.Timers;

namespace VBone
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const double HeadRadius = .11;
        private const double SlideLength = .586; // (According to Wick)
        private const double DistanceMouthToHandleInFirstPosition = .13; // Measured myself (Besson model)
        private const double MinSlideDistance = HeadRadius + DistanceMouthToHandleInFirstPosition;//.3;
        private const double MaxSlideDistance = MinSlideDistance + SlideLength;//.7;
        private const double MinHarmonicHeightDistance = -.3; // Arbitrary point in space, within arm's reach
        private const double MaxHarmonicHeightDistance = .4; // Arbitrary point in space, within arm's reach
        private Harmonic lastKinectHarmonic;
        private int currentVelocity = 127;
        private bool isMouseDown;
        private bool isKinectNoteSent;
        private bool kinectMode = true;
        private ObservableCollection<TromboneNote> tromboneNotes = new ObservableCollection<TromboneNote>();
        private DrawingImage imageSource;
        private KinectAnalyser kinectAnalyser;
        private Position currentPosition;
        private Harmonic currentHarmonic;
        private double slidePercentage;
        private double harmonicPercentage;

        public MainWindow()
        {
            this.kinectAnalyser = new KinectAnalyser(this.Dispatcher);
            this.kinectAnalyser.MusicalDataUpdatedEvent += this.OnMusicalDataUpdatedEvent;
            this.imageSource = new DrawingImage(this.kinectAnalyser.DrawingGroup);
            this.Closing += (s, e) => { this.Cleanup(); };
            this.MouseUp += (s, e) => this.isMouseDown = false;
            this.Loaded += (s, e) =>
                {
                    MidiDevice.OpenPorts();
                    this.kinectAnalyser.Init();

                    foreach (var i in MatrixDataGenerator.Generate())
                    {
                        this.TromboneNotes.Add(i);
                    }

                    this.OnPropertyChanged("TromboneNotes");
                };

            this.InitializeComponent();

            this.slider.DataChanged += (s, e) =>
            {
                this.currentVelocity = (int)(this.slider.SliderPercentage * 127);
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                return this.imageSource;
            }
        }

        public ObservableCollection<TromboneNote> TromboneNotes
        {
            get
            {
                return tromboneNotes;
            }

            set
            {
                tromboneNotes = value;
                this.OnPropertyChanged("TromboneNotes");
            }
        }

        public bool KinectMode
        {
            get
            {
                return this.kinectMode;
            }

            set
            {
                this.kinectMode = value;
                this.OnPropertyChanged("KinectMode");
            }
        }

        public Position CurrentPosition
        {
            get
            {
                return this.currentPosition;
            }

            set
            {
                this.currentPosition = value;
                this.OnPropertyChanged("CurrentPosition");
            }
        }

        public Harmonic CurrentHarmonic
        {
            get
            {
                return this.currentHarmonic;
            }

            set
            {
                this.currentHarmonic = value;
                this.OnPropertyChanged("CurrentHarmonic");
            }
        }

        public double SlidePercentage
        {
            get
            {
                return this.slidePercentage;
            }

            set
            {
                this.slidePercentage = value;
                this.gauge1.SliderPercentage = 1.0 - value;
                this.OnPropertyChanged("SlidePercentage");
            }
        }

        public double HarmonicPercentage
        {
            get
            {
                return this.harmonicPercentage;
            }

            set
            {
                this.harmonicPercentage = value;
                this.gauge2.SliderPercentage = 1.0 - value;
                this.OnPropertyChanged("HarmonicPercentage");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void OnMusicalDataUpdatedEvent(object sender, EventArgs e)
        {
            this.OnPropertyChanged("ImageSource");
            this.KinectNoteEvent(this.kinectAnalyser.SlideDistance, this.kinectAnalyser.HarmonicHeight);
        }

        private void NoteCell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var noteCell = sender as NoteCell;
            var tromboneNote = noteCell.DataContext as TromboneNote;
            var relativeLocation = e.GetPosition(noteCell);
            double percentThroughPosition = relativeLocation.X / noteCell.ActualWidth;
            MidiDevice.SendTromboneNoteOn(tromboneNote, percentThroughPosition: percentThroughPosition, velocity: currentVelocity);
            this.isMouseDown = true;
        }

        private void NoteCell_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isMouseDown)
            {
                var noteCell = sender as NoteCell;
                var tromboneNote = noteCell.DataContext as TromboneNote;
                var relativeLocation = e.GetPosition(noteCell);
                double percentThroughPosition = relativeLocation.X / noteCell.ActualWidth;
                MidiDevice.SendTromboneNoteChanged(tromboneNote, percentThroughPosition: percentThroughPosition);
            }
        }

        private void NoteCell_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var noteCell = sender as NoteCell;
            var tromboneNote = noteCell.DataContext as TromboneNote;
            MidiDevice.SendTromboneNoteOff(tromboneNote);
            this.isMouseDown = false;
        }

        private void KinectModeButtonClick(object sender, RoutedEventArgs e)
        {
            this.KinectMode = !this.KinectMode;
        }

        private void Cleanup()
        {
            MidiDevice.ClosePort();
            this.kinectAnalyser.Cleanup();
        }

        private void KinectNoteEvent(double slideDistance, double harmonicDistance)
        {
            double normalisedSlideDistance = slideDistance - MinSlideDistance;
            double slideAsPercentage = normalisedSlideDistance / SlideLength;

            //Debug.WriteLine("Distance: " + normalisedSlideDistance.ToString("#.##") + " - Percent: " + slideAsPercentage.ToString("#.##"));

            double inverseSlideAsPercentage = Math.Max(0, Math.Min(1, 1.0 - slideAsPercentage));
            double harmonicHeightAsPercentage = Math.Max(0, Math.Min(1, (harmonicDistance - MinHarmonicHeightDistance) / (MaxHarmonicHeightDistance - MinHarmonicHeightDistance))); // TODO: Invert?

            if (!this.isKinectNoteSent)
            {
                // ... Send initial note ...
                this.KinectNoteOn(inverseSlideAsPercentage, harmonicHeightAsPercentage);
                this.isKinectNoteSent = true;
            }
            else
            {
                if (this.IsHarmonicChanged(harmonicHeightAsPercentage))
                {
                    this.KinectNoteOn(inverseSlideAsPercentage, harmonicHeightAsPercentage);
                }
                else
                {
                    this.KinectMouseMove(inverseSlideAsPercentage, harmonicHeightAsPercentage);
                }
            }
        }

        private bool IsHarmonicChanged(double harmonicHeightAsPercentage)
        {
            return this.HarmonicFromPercentage(harmonicHeightAsPercentage) != this.lastKinectHarmonic;
        }

        private void KinectNoteOn(double slideAsPercentage, double harmonicHeightAsPercentage)
        {
            this.SlidePercentage = slideAsPercentage;
            this.HarmonicPercentage = harmonicHeightAsPercentage;
            this.CurrentPosition = this.PositionFromPercentage(slideAsPercentage);
            this.CurrentHarmonic = this.HarmonicFromPercentage(harmonicHeightAsPercentage);
            var tromboneNote = new TromboneNote(this.CurrentPosition, this.CurrentHarmonic);
            this.lastKinectHarmonic = this.CurrentHarmonic;
            MidiDevice.SendTromboneNoteOnAbsolutePitchBend(tromboneNote, percentPitchBend: slideAsPercentage, velocity: currentVelocity);
        }

        private void KinectMouseMove(double slideAsPercentage, double harmonicHeightAsPercentage)
        {
            if (this.isKinectNoteSent)
            {
                this.SlidePercentage = slideAsPercentage;
                this.HarmonicPercentage = harmonicHeightAsPercentage;
                this.CurrentPosition = this.PositionFromPercentage(slideAsPercentage);
                this.CurrentHarmonic = this.HarmonicFromPercentage(harmonicHeightAsPercentage);
                var tromboneNote = new TromboneNote(this.CurrentPosition, this.CurrentHarmonic);
                this.lastKinectHarmonic = this.CurrentHarmonic;
                MidiDevice.SendTromboneNoteChangedAbsolutePitchBend(tromboneNote, percentPitchBend: slideAsPercentage);
            }
        }

        //private void KinectNoteOff()
        //{
        //    var noteCell = sender as NoteCell;
        //    var tromboneNote = noteCell.DataContext as TromboneNote;
        //    MidiDevice.SendTromboneNoteOff(tromboneNote);
        //    this.isKinectNoteSent = false;
        //}

        private Position PositionFromPercentage(double percentage)
        {
            int positionCount = Enum.GetValues(typeof(Position)).Cast<int>().Count();

            return (Position)(Math.Round((positionCount - 1) * percentage));
        }

        private Harmonic HarmonicFromPercentage(double percentage)
        {
            int count = Enum.GetValues(typeof(Harmonic)).Cast<Harmonic>().Count();
            int countLessFundamental = count - 1;

            return (Harmonic)(Math.Truncate(countLessFundamental * percentage) + 1);
        }
    }
}