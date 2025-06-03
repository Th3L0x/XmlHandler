using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XmlHandler.Util;

internal class UIHelper
{
    public static T? FindAncestor<T>(DependencyObject current) where T : DependencyObject
    {
        while (current != null)
        {
            if (current is T t)
                return t;
            current = VisualTreeHelper.GetParent(current);
        }
        return null;
    }

    public static T? GetDataGridRowItem<T>(object source) where T : class
    {
        DependencyObject dep = (DependencyObject)source;

        while (dep != null && dep is not DataGridRow)
        {
            dep = VisualTreeHelper.GetParent(dep);
        }

        return (dep as DataGridRow)?.Item as T;
    }

}
