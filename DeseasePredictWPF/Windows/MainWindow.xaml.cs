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

namespace DeseasePredictWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new CCellularAutomatVM(this);
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
