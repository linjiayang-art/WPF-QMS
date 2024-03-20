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

namespace SicoreQMS.Views
{
    /// <summary>
    /// ProdProcessUpdateView.xaml 的交互逻辑
    /// </summary>
    public partial class ProdProcessUpdateView : UserControl
    {
        public ProdProcessUpdateView()
        {
            InitializeComponent();
        }

        private void comProductName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                comProductName.IsDropDownOpen = true;
            }
          
        }

        private void comProductName_TextInput(object sender, TextCompositionEventArgs e)
        {
            comProductName.IsDropDownOpen = true;
        }


    }
}
