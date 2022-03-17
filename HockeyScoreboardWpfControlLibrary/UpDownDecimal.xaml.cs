using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HockeyScoreboardWpfControlLibrary
{
    /// <summary>
    /// Interaction logic for UpDownDecimal.xaml
    /// </summary>
    public partial class UpDownDecimal : UserControl
    {
        public event EventHandler ValueChanged;

        public event EventHandler PropertyChanged;

        public UpDownDecimal()
        {
            InitializeComponent();

            TextBoxValue.SetBinding(TextBox.TextProperty, new Binding("Value")
            {
                ElementName = "root_UpDownDecimal",
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(UpDownDecimal)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(UpDownDecimal)).AddValueChanged(this, ValueChanged);
            DependencyPropertyDescriptor.FromProperty(DecimalsProperty, typeof(UpDownDecimal)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(MinimumProperty, typeof(UpDownDecimal)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(MaximumProperty, typeof(UpDownDecimal)).AddValueChanged(this, PropertyChanged);

            PropertyChanged += (x, y) => PropertyChangedMethod();
        }

        private void PropertyChangedMethod()
        {
            // just in case
        }

        #region Dependency properties

        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
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
                value = decimal.Round(value, Decimals);
                SetValue(ValueProperty, value);
                ValueChanged(this, new EventArgs());
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal), typeof(UpDownDecimal), new PropertyMetadata(new decimal(0)));

        public decimal Step
        {
            get { return (decimal)GetValue(StepProperty); }
            set
            {
                SetValue(StepProperty, value);
            }
        }

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(decimal), typeof(UpDownDecimal), new PropertyMetadata(new decimal(0.1)));

        public decimal Minimum
        {
            get { return (decimal)GetValue(MinimumProperty); }
            set
            {
                if (value > Maximum)
                    Maximum = value;
                SetValue(MinimumProperty, value);
            }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(decimal), typeof(UpDownDecimal), new PropertyMetadata(decimal.MinValue));

        public decimal Maximum
        {
            get { return (decimal)GetValue(MaximumProperty); }
            set
            {
                if (value < Minimum)
                    Minimum = value;
                SetValue(MaximumProperty, value);
            }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(decimal), typeof(UpDownDecimal), new PropertyMetadata(decimal.MaxValue));

        public int Decimals
        {
            get { return (int)GetValue(DecimalsProperty); }
            set { SetValue(DecimalsProperty, value); }
        }

        public static readonly DependencyProperty DecimalsProperty =
            DependencyProperty.Register("Decimals", typeof(int), typeof(UpDownDecimal), new PropertyMetadata(2));

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