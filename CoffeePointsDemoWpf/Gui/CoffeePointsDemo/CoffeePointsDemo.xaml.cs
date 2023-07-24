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

namespace CoffeePointsDemoWpf
{
    /// <summary>
    /// Interaction logic for CoffeePointsDemoFrm.xaml
    /// </summary>
    public partial class CoffeePointsDemoFrm : Window
    {
        public CoffeePointsDemoFrm(CoffeePointsDemoViewModel dataContext)
        {
            DataContext = dataContext;
            InitializeComponent();
        }
    }
}
