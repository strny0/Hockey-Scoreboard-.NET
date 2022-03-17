using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HockeyScoreboardWpfControlLibrary
{
    /// <summary>
    /// Interaction logic for UpDownDouble.xaml
    /// </summary>
    public partial class UpDownDouble : UserControl
    {
        public event EventHandler ValueChanged;

        public event EventHandler PropertyChanged;

        public UpDownDouble()
        {
            InitializeComponent();

            TextBoxValue.SetBinding(TextBox.TextProperty, new Binding("Value")
            {
                ElementName = "root_UpDownDouble",
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(UpDownDouble)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(UpDownDouble)).AddValueChanged(this, ValueChanged);
            DependencyPropertyDescriptor.FromProperty(DecimalsProperty, typeof(UpDownDouble)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(MinimumProperty, typeof(UpDownDouble)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(MaximumProperty, typeof(UpDownDouble)).AddValueChanged(this, PropertyChanged);

            PropertyChanged += (x, y) => PropertyChangedMethod();
        }

        private void PropertyChangedMethod()
        {
            // just in case
        }

        #region Dependency properties

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set
            {
                if (value < Minimum)
                {
                    value = Minimum;
                }
                if (value > Maximum)
                {
                    value = Maximum;
                }
                value = (double)Math.Round(value, Decimals);
                SetValue(ValueProperty, value);
                ValueChanged(this, new EventArgs());
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(UpDownDouble), new PropertyMetadata(0f));

        public double Step
        {
            get { return (double)GetValue(StepProperty); }
            set
            {
                SetValue(StepProperty, value);
            }
        }

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(double), typeof(UpDownDouble), new PropertyMetadata(0.1f));

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set
            {
                if (value > Maximum)
                    Maximum = value;
                SetValue(MinimumProperty, value);
            }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(UpDownDouble), new PropertyMetadata(double.MinValue));

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set
            {
                if (value < Minimum)
                    Minimum = value;
                SetValue(MaximumProperty, value);
            }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(UpDownDouble), new PropertyMetadata(double.MaxValue));

        public int Decimals
        {
            get { return (int)GetValue(DecimalsProperty); }
            set { SetValue(DecimalsProperty, value); }
        }

        public static readonly DependencyProperty DecimalsProperty =
            DependencyProperty.Register("Decimals", typeof(int), typeof(UpDownDouble), new PropertyMetadata(2));

        #endregion Dependency properties

        private void RbuttonUp_Click(object sender, RoutedEventArgs e)
        {
            Value += Step;
        }

        private void RbuttonDown_Click(object sender, RoutedEventArgs e)
        {
            Value -= Step;
        }

        private void TextBoxValue_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                Value += Step;
            }
            else if (e.Delta < 0)
            {
                Value -= Step;
            }
        }

        private void TextBoxValue_Loaded(object sender, RoutedEventArgs e)
        {
            try { ValueChanged(this, new EventArgs()); }
            catch { }
        }
    }
}