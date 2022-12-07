using DeseasePredictWPF.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DeseasePredictWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow(OptionsVM opt)
        {
            DataContext = opt;
            InitializeComponent();
        }

        private void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            int x;
            if (!Int32.TryParse(tb.Text, out x))
                tb.Text = "0";
            tb.SelectionStart = tb.Text.Length;
        }
        private void textboxDouble_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string s = tb.Text.Replace('.', ',');
            double x;
            if (!Double.TryParse(s, out x) || x > 1)
                tb.Text = "0";
            tb.SelectionStart = tb.Text.Length;
        }
    }
}
