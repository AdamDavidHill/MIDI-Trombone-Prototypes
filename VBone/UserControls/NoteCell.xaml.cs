using MidiDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VBone.UserControls
{
    /// <summary>
    /// Interaction logic for NoteCell.xaml
    /// </summary>
    public partial class NoteCell : UserControl
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(int), typeof(NoteCell));
        public static readonly DependencyProperty NoteNameProperty = DependencyProperty.Register("NoteName", typeof(string), typeof(NoteCell));
        public static readonly DependencyProperty PitchProperty = DependencyProperty.Register("Pitch", typeof(Pitch), typeof(NoteCell));

        public NoteCell()
        {
            InitializeComponent();
        }

        public int Position
        {
            get { return (int)this.GetValue(PositionProperty); }
            set { this.SetValue(PositionProperty, value); }
        }

        public string NoteName
        {
            get { return (string)this.GetValue(NoteNameProperty); }
            set { this.SetValue(NoteNameProperty, value); }
        }

        public Pitch Pitch
        {
            get { return (Pitch)this.GetValue(PitchProperty); }
            set { this.SetValue(PitchProperty, value); }
        }
    }
}
