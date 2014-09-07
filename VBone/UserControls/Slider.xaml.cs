using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VBone.UserControls
{
    /// <summary>
    /// Interaction logic for Slider.xaml
    /// </summary>
    public partial class Slider : UserControl
    {
        public static readonly DependencyProperty SliderPercentageProperty = DependencyProperty.Register("SliderPercentage", typeof(double), typeof(Slider), new PropertyMetadata(1.0));
        public static readonly DependencyProperty TabXProperty = DependencyProperty.Register("TabX", typeof(int), typeof(Slider));
        public static readonly DependencyProperty TabYProperty = DependencyProperty.Register("TabY", typeof(int), typeof(Slider));
        public static readonly DependencyProperty TabWidthProperty = DependencyProperty.Register("TabWidth", typeof(double), typeof(Slider), new PropertyMetadata(200.0));
        public static readonly DependencyProperty TabHeightProperty = DependencyProperty.Register("TabHeight", typeof(double), typeof(Slider), new PropertyMetadata(50.0));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(Slider), new PropertyMetadata(Orientation.Vertical));
        public static readonly DependencyProperty TabThicknessProperty = DependencyProperty.Register("TabThickness", typeof(double), typeof(Slider), new PropertyMetadata(50.0));
        public static readonly DependencyProperty TabBrushProperty = DependencyProperty.Register("TabBrush", typeof(SolidColorBrush), typeof(Slider), new PropertyMetadata(new SolidColorBrush(new Color { A = 136, R = 68, G = 136, B = 255 })));
        public static readonly DependencyProperty BackgroundBrushProperty = DependencyProperty.Register("BackgroundBrush", typeof(SolidColorBrush), typeof(Slider), new PropertyMetadata(new SolidColorBrush(new Color { A = 50, R = 68, G = 136, B = 255 })));

        private bool isMouseDown;

        public Slider()
        {
            InitializeComponent();
            this.canvas.SizeChanged += (s, e) => this.UpdateOrientation();
        }

        public event EventHandler DataChanged = delegate { };

        public double SliderPercentage
        {
            get { return (double)this.GetValue(SliderPercentageProperty); }
            set { this.SetValue(SliderPercentageProperty, value); }
        }

        public int TabX
        {
            get { return (int)this.GetValue(TabXProperty); }
            set { this.SetValue(TabXProperty, value); }
        }

        public int TabY
        {
            get { return (int)this.GetValue(TabYProperty); }
            set { this.SetValue(TabYProperty, value); }
        }

        public double TabWidth
        {
            get { return (double)this.GetValue(TabWidthProperty); }
            set { this.SetValue(TabWidthProperty, value); }
        }

        public double TabHeight
        {
            get { return (double)this.GetValue(TabHeightProperty); }
            set { this.SetValue(TabHeightProperty, value); }
        }

        public double TabThickness
        {
            get { return (double)this.GetValue(TabThicknessProperty); }
            set { this.SetValue(TabThicknessProperty, value); }
        }

        public Orientation Orientation
        {
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        public SolidColorBrush TabBrush
        {
            get { return (SolidColorBrush)this.GetValue(TabBrushProperty); }
            set { this.SetValue(TabBrushProperty, value); }
        }

        public SolidColorBrush BackgroundBrush
        {
            get { return (SolidColorBrush)this.GetValue(BackgroundBrushProperty); }
            set { this.SetValue(BackgroundBrushProperty, value); }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "SliderPercentage")
            {
                this.UpdateSliderPercentage((double)e.NewValue);
            }

            if (e.Property.Name == "Orientation")
            {
                this.UpdateOrientation();
            }

            base.OnPropertyChanged(e);
        }

        private void UpdateSliderPercentage(double value)
        {
            if (this.Orientation == Orientation.Vertical)
            {
                this.TabX = (int)(this.canvas.ActualWidth / 2.0 - this.tab.ActualWidth / 2.0);
                this.TabY = (int)(this.canvas.ActualHeight * value - this.tab.ActualHeight / 2.0);
            }
            else
            {
                this.TabX = (int)(this.canvas.ActualWidth * value - this.tab.ActualWidth / 2.0);
                this.TabY = (int)(this.canvas.ActualHeight / 2.0 - this.tab.ActualHeight / 2.0);
            }
        }

        private void UpdateOrientation()
        {
            this.TabWidth = this.Orientation == Orientation.Vertical ? this.canvas.ActualWidth : this.TabThickness;
            this.TabHeight = this.Orientation == Orientation.Vertical ? this.TabThickness : this.canvas.ActualHeight;
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsEnabled)
            {
                this.UpdateSliderPosition(e.GetPosition(this.canvas));
                this.DataChanged(this, new EventArgs());
                this.isMouseDown = true;
            }
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isMouseDown && this.IsEnabled)
            {
                this.UpdateSliderPosition(e.GetPosition(this.canvas));
                this.DataChanged(this, new EventArgs());
            }
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsEnabled)
            {
                this.isMouseDown = false;
            }
        }

        private void UpdateSliderPosition(Point position)
        {
            this.SliderPercentage = this.Orientation == Orientation.Vertical ? position.Y / this.canvas.ActualHeight : position.X / this.canvas.ActualWidth;
        }
    }
}
