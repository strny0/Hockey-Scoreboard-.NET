using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace HockeyScoreboardWpfControlLibrary
{
    /// <summary>
    /// Interaction logic for UpDownInt.xaml
    /// </summary>
    public partial class UpDownInt : UserControl
    {

        public event EventHandler ValueChanged;
        public event EventHandler PropertyChanged;

        public UpDownInt()
        {
            InitializeComponent();

            TextBoxValue.SetBinding(TextBox.TextProperty, new Binding("Value")
            {
                ElementName = "root_UpDownInt",
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

            DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(UpDownInt)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(ValueProperty, typeof(UpDownInt)).AddValueChanged(this, ValueChanged);
            DependencyPropertyDescriptor.FromProperty(MinimumProperty, typeof(UpDownInt)).AddValueChanged(this, PropertyChanged);
            DependencyPropertyDescriptor.FromProperty(MaximumProperty, typeof(UpDownInt)).AddValueChanged(this, PropertyChanged);

            PropertyChanged += (x, y) => PropertyChangedMethod();
        }

        private void PropertyChangedMethod()
        {
            if (Minimum > Maximum) Minimum = Maximum;
            if (Maximum < Minimum) Maximum = Minimum;
            if (Value < Minimum) Value = Minimum;
            if (Value > Maximum) Value = Maximum;
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
                try { ValueChanged(this, new EventArgs()); }
                catch { }
            }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(UpDownInt), new PropertyMetadata(0));

        public int Step
        {
            get { return (int)GetValue(StepProperty); }
            set
            {
                SetValue(StepProperty, value);
            }
        }

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(int), typeof(UpDownInt), new PropertyMetadata(1));

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set
            {
                if (value > Maximum)
                {
                    Minimum = value;
                }
                SetValue(MinimumProperty, value);
            }
        }

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(UpDownInt), new PropertyMetadata(int.MinValue));

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set
            {
                if (value < Minimum)
                {
                    Maximum = value;
                }

                SetValue(MaximumProperty, value);

            }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(UpDownInt), new PropertyMetadata(int.MaxValue));

        #endregion


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

        private void TextBoxValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            try { Value = int.Parse(TextBoxValue.Text); }
            catch { TextBoxValue.Text = Value.ToString(); }
        }
    }
}
