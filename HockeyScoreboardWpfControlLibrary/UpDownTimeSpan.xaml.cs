using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HockeyScoreboardWpfControlLibrary
{
    /// <summary>
    /// Interaction logic for UpDownTimeSpan.xaml
    /// </summary>
    public partial class UpDownTimeSpan : UserControl
    {
        public event EventHandler ValueChanged;

        public event EventHandler PropertyChanged;

        public UpDownTimeSpan()
        {
            InitializeComponent();

            TextBoxValue.SetBinding(TextBox.TextProperty, new Binding("Value")
            {
                ElementName = "root_UpDownTimeSpan",
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(UpDownTimeSpan)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(UpDownTimeSpan)).AddValueChanged(this, ValueChanged);
            DependencyPropertyDescriptor.FromProperty(MinimumProperty, typeof(UpDownTimeSpan)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(MaximumProperty, typeof(UpDownTimeSpan)).AddValueChanged(this, PropertyChanged);

            PropertyChanged += (x, y) => PropertyChangedMethod();
        }

        private void PropertyChangedMethod()
        {
            // just in case
        }

        #region Dependency properties

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
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
                SetValue(ValueProperty, value);
                ValueChanged(this, new EventArgs());
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(UpDownTimeSpan), new PropertyMetadata(0));

        public int Step
        {
            get { return (int)GetValue(StepProperty); }
            set
            {
                SetValue(StepProperty, value);
            }
        }

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(int), typeof(UpDownTimeSpan), new PropertyMetadata(1));

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set
            {
                if (value > Maximum)
                    Maximum = value;
                SetValue(MinimumProperty, value);
            }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(decimal), typeof(UpDownTimeSpan), new PropertyMetadata(int.MinValue));

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set
            {
                if (value < Minimum)
                    Minimum = value;
                SetValue(MaximumProperty, value);
            }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(UpDownTimeSpan), new PropertyMetadata(int.MaxValue));

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