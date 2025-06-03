using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XmlHandler.Model;
using XmlHandler.Util;
using XmlHandler.ViewModel;

namespace XmlHandler.View;

public partial class MainPage : Window
{
    private Point _startPoint;

    public MainPage(MainViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }

    private void datagrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        (this.DataContext as MainViewModel)?.OnSelectedCellChanged();
    }

    private void DataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _startPoint = e.GetPosition(null);
    }

    private void DataGrid_MouseMove(object sender, MouseEventArgs e)
    {
        Point mousePos = e.GetPosition(null);
        Vector diff = _startPoint - mousePos;

        if (e.LeftButton == MouseButtonState.Pressed &&
            (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
             Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
        {
            // Get the dragged row
            var dataGrid = sender as DataGrid;
            var row = UIHelper.FindAncestor<DataGridRow>((DependencyObject)e.OriginalSource);
            if (row == null) return;

            var user = (User)row.Item;
            DragDrop.DoDragDrop(row, user, DragDropEffects.Move);
        }
    }

    private void DataGrid_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(typeof(User)))
        {
            var target = UIHelper.GetDataGridRowItem<User>(e.OriginalSource);

            if (e.Data.GetData(typeof(User)) is not User droppedData || target == null || droppedData == target)
                return;

            var users = (ObservableCollection<User>)datagrid.ItemsSource;
            int oldIndex = users.IndexOf(droppedData);
            int newIndex = users.IndexOf(target);

            if (oldIndex != newIndex)
            {
                users.Move(oldIndex, newIndex);
            }
        }
    }
}