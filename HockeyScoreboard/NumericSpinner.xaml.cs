using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HockeyScoreboard.Controls
{
    /// <summary>
    /// Interaction logic for NumericSpinner.xaml
    /// </summary>
    public partial class NumericSpinner : UserControl
    {
        #region Fields

        public event EventHandler PropertyChanged;
        public event EventHandler ValueChanged;
        #endregion

        public NumericSpinner()
        {
            InitializeComponent();

            tb_main.SetBinding(TextBox.TextProperty, new Binding("Value")
            {
                ElementName = "root_numeric_spinner",
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(NumericSpinner)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(NumericSpinner)).AddValueChanged(this, ValueChanged);
            DependencyPropertyDescriptor.FromProperty(DecimalsProperty, typeof(NumericSpinner)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(MinimumProperty, typeof(NumericSpinner)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(MaximumProperty, typeof(NumericSpinner)).AddValueChanged(this, PropertyChanged);


            PropertyChanged += (x, y) => validate();
        }



        #region ValueProperty

        public readonly static DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(decimal),
            typeof(NumericSpinner),
            new PropertyMetadata(new decimal(0)));

        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set
            {
                if (value < Minimum)
                    value = Minimum;
                if (value > Maximum)
                    value = Maximum;
                SetValue(ValueProperty, value);
                try { ValueChanged(this, new EventArgs()); }
                catch { }
            }
        }


        #endregion

        #region StepProperty

        public readonly static DependencyProperty StepProperty = DependencyProperty.Register(
            "Step",
            typeof(decimal),
            typeof(NumericSpinner),
            new PropertyMetadata(new decimal(0.1)));

        public decimal Step
        {
            get { return (decimal)GetValue(StepProperty); }
            set
            {
                SetValue(StepProperty, value);
            }
        }

        #endregion

        #region DecimalsProperty

        public readonly static DependencyProperty DecimalsProperty = DependencyProperty.Register(
            "Decimals",
            typeof(int),
            typeof(NumericSpinner),
            new PropertyMetadata(2));

        public int Decimals
        {
            get { return (int)GetValue(DecimalsProperty); }
            set
            {
                SetValue(DecimalsProperty, value);
            }
        }

        #endregion

        #region MinimumProperty

        public readonly static DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum",
            typeof(decimal),
            typeof(NumericSpinner),
            new PropertyMetadata(decimal.MinValue));

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

        #endregion

        #region MaximumPropertya

        public readonly static DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum",
            typeof(decimal),
            typeof(NumericSpinner),
            new PropertyMetadata(decimal.MaxValue));

        public decimal Maximum
        {
            get { return (decimal)GetValue(MaximumProperty); }
            set
            {
                if (value < Minimum)
                    value = Minimum;
                SetValue(MaximumProperty, value);
            }
        }

        #endregion

        /// <summary>
        /// Revalidate the object, whenever a value is changed...
        /// </summary>
        private void validate()
        {
            // Logically, This is not needed at all... as it's handled within other properties...
            if (Minimum > Maximum) Minimum = Maximum;
            if (Maximum < Minimum) Maximum = Minimum;
            if (Value < Minimum) Value = Minimum;
            if (Value > Maximum) Value = Maximum;

            Value = decimal.Round(Value, Decimals);
        }

        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            Value += Step;
        }




        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            Value -= Step;
        }



        private void tb_main_Loaded(object sender, RoutedEventArgs e)
        {
            ValueChanged(this, new EventArgs());
        }

        private void tb_main_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                Value += Step;

            else if (e.Delta < 0)
                Value -= Step;
        }
    }
}
