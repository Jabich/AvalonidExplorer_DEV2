using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace AvaloniaApplication1.Views
{
    public partial class MainWindow : Window
    {
        public static ListBox? listBoxExplorer;
        public MainWindow()
        {
            InitializeComponent();
            Activated += FocusedListBoxItem!;
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        private void FocusedListBoxItem(object sender, EventArgs e)
        {
            var filesListBox = this.FindControl<ListBox>("FilesListBox");
            listBoxExplorer = filesListBox;
            if(filesListBox.ItemCount >0)
            {
                filesListBox.SelectedIndex = 0;
                filesListBox.ItemContainerGenerator.ContainerFromIndex(0).Focus();
            }
        }
    }
}
