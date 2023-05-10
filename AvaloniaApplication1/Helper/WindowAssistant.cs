using Avalonia.Controls;
using AvaloniaApplication1.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Helper
{
    public static class WindowAssistant
    {
        public static void FocusedListBox(ListBox listBox)
        {
            if (listBox.ItemCount > 0)
            {
                listBox.SelectedIndex = 0;
                listBox.ItemContainerGenerator.ContainerFromIndex(0).Focus();
            }
        }
    }
}
