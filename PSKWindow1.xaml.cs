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
using System.Windows.Shapes;

namespace shvFT991A
{
    /// <summary>
    /// PSKWindow1.xaml の相互作用ロジック
    /// </summary>
    public partial class PSKWindow1 : Window
    {
        public PSKWindow1()
        {
            InitializeComponent();
        }

        private void ButtonReturn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.Current.MainWindow.Show();
        }
    }
}
