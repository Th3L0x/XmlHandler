using Microsoft.Win32;

namespace XmlHandler.Util;

internal class OpenFileHelper
{
    public static string? OpenFile()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Open File",
            Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*"
        };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
