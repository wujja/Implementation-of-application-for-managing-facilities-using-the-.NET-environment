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
using MahApps.Metro.Controls;

namespace Apkainz
{
    /// <summary>
    /// Logika interakcji dla klasy ConfirmOperation.xaml
    /// </summary>
    /// 
    public partial class ConfirmOperation : MetroWindow
    {
        public bool wantToDelete;
        public ConfirmOperation()
        {
            InitializeComponent();
        }


        private void Yes_Button_Click(object sender, RoutedEventArgs e)
        {
            wantToDelete = true;
            Close();
        }

        private void No_Button_Click(object sender, RoutedEventArgs e)
        {
            wantToDelete = false;
            Close();
        }
    }

}
