using CoffeePointsDemo.Service;
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
        private CoffeePointsDemoViewModel _viewModel;
        private FormStateHolder _formStateHolder = new FormStateHolder();
        private FormStateHolder _formStateHolder2 = new FormStateHolder();

        public CoffeePointsDemoFrm(CoffeePointsDemoViewModel dataContext)
        {
            DataContext = dataContext;
            _viewModel = dataContext;

            _formStateHolder.CreateFormState(CoffeePointsDemoViewModel.SelectionMode.SelectNone.ToString()).AddAction(() =>
            {
                EditCvn.Visibility = Visibility.Hidden;
                NewBtn.Visibility = Visibility.Visible;
                SaveBtn.Visibility = Visibility.Hidden;
                DeleteBtn.Visibility = Visibility.Hidden;

            }).Parent.CreateFormState(CoffeePointsDemoViewModel.SelectionMode.SelectRegular.ToString()).AddAction(() =>
            {
                EditCvn.Visibility = Visibility.Visible;
                NewBtn.Visibility = Visibility.Visible;
                SaveBtn.Visibility = Visibility.Visible;
                DeleteBtn.Visibility = Visibility.Visible;
            });

            _formStateHolder2.CreateFormState(CoffeePointsDemoViewModel.SaveOrNotMode.CanSave.ToString()).AddAction(() =>
            {
                SaveBtn.Visibility = Visibility.Hidden;

            }).Parent.CreateFormState(CoffeePointsDemoViewModel.SaveOrNotMode.CannotSave.ToString()).AddAction(() =>
            {
                SaveBtn.Visibility = Visibility.Visible;
            });

            _viewModel.SelectionModeChanged += _viewModel_SelectionModeChanged1;

            InitializeComponent();
        }

        private void _viewModel_SelectionModeChanged1(CoffeePointsDemoViewModel.SelectionMode selectionMode)
        {
            _formStateHolder.SetFormState(selectionMode.ToString());
        }

        private void ItemGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_viewModel.NeedToStopItemSelectionChange())
            {
                e.Handled = true;
            }
        }
    }
}
