using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.ViewModels;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace AvaloniaApplication1.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Activated += WindowActivated;
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        private void WindowActivated(object sender, EventArgs e)
        {
            var filesListBox = this.FindControl<ListBox>("FilesListBox");
            ////filesListBox.Focus();
            //filesListBox.SelectedIndex = 0;
            ////KeyboardNavigation.SetTabNavigation(filesListBox, KeyboardNavigationMode.Continue);
            ////filesListBox.InvalidateVisual();
            //filesListBox.Focus();
            //if (filesListBox.SelectedItem != null)
            //{
            //    var dfg = filesListBox.SelectedItem as ListBoxItem;
            //    var fdgf = filesListBox.SelectedItem as ListBoxItem;
            //    var itemContainer = FilesListBox.ItemContainerGenerator.IndexFromContainer(filesListBox.SelectedIndex);
            //}
            ////((ListBoxItem)filesListBox.SelectedItem).Focus();
            //else
            //    filesListBox.Focus();


            filesListBox.Focus();
            filesListBox.SelectedIndex = 0;
            //((ListBoxItem)filesListBox.SelectedItem).Focus();
            var fdhgh = filesListBox.ItemContainerGenerator.ContainerFromIndex(0);
            fdhgh.Focus();

        }


        private void InputElement_OnLostFocus(object? sender, RoutedEventArgs e)
        {
            var filesListBox = this.FindControl<ListBox>("FilesListBox");
            filesListBox.Focus();
            filesListBox.SelectedIndex = 0;
            //((ListBoxItem)filesListBox.SelectedItem).Focus();
            var fdhgh = filesListBox.ItemContainerGenerator.ContainerFromIndex(0);
            fdhgh.Focus();
        }
    }
}
