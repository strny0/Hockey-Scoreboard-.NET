using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HockeyScoreboardWpfControlLibrary
{
    /// <summary>
    /// Interaction logic for UpDownFloat.xaml
    /// </summary>
    public partial class UpDownFloat : UserControl
    {
        public event EventHandler ValueChanged;

        public event EventHandler PropertyChanged;

        public UpDownFloat()
        {
            InitializeComponent();

            TextBoxValue.SetBinding(TextBox.TextProperty, new Binding("Value")
            {
                ElementName = "root_UpDownFloat",
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(UpDownFloat)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(UpDownFloat)).AddValueChanged(this, ValueChanged);
            DependencyPropertyDescriptor.FromProperty(DecimalsProperty, typeof(UpDownFloat)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(MinimumProperty, typeof(UpDownFloat)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(MaximumProperty, typeof(UpDownFloat)).AddValueChanged(this, PropertyChanged);

            PropertyChanged += (x, y) => PropertyChangedMethod();
        }

        private void PropertyChangedMethod()
        {
            // just in case
        }

        #region Dependency properties

        public float Value
        {
            get { return (float)GetValue(ValueProperty); }
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
                value = (float)Math.Round(value, Decimals);
                SetValue(ValueProperty, value);
                ValueChanged(this, new EventArgs());
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(float), typeof(UpDownFloat), new PropertyMetadata(0f));

        public float Step
        {
            get { return (float)GetValue(StepProperty); }
            set
            {
                SetValue(StepProperty, value);
            }
        }

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(float), typeof(UpDownFloat), new PropertyMetadata(0.1f));

        public float Minimum
        {
            get { return (float)GetValue(MinimumProperty); }
            set
            {
                if (value > Maximum)
                    Maximum = value;
                SetValue(MinimumProperty, value);
            }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(float), typeof(UpDownFloat), new PropertyMetadata(float.MinValue));

        public float Maximum
        {
            get { return (float)GetValue(MaximumProperty); }
            set
            {
                if (value < Minimum)
                    Minimum = value;
                SetValue(MaximumProperty, value);
            }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(float), typeof(UpDownFloat), new PropertyMetadata(float.MaxValue));

        public int Decimals
        {
            get { return (int)GetValue(DecimalsProperty); }
            set { SetValue(DecimalsProperty, value); }
        }

        public static readonly DependencyProperty DecimalsProperty =
            DependencyProperty.Register("Decimals", typeof(int), typeof(UpDownFloat), new PropertyMetadata(2));

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