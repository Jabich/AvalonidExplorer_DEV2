using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaApplication1.Helper;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Views;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.IO;

namespace AvaloniaApplication1.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private int _listBoxItem = 1;
        public int ListBoxItem
        {
            get => _listBoxItem;
            set => this.RaiseAndSetIfChanged(ref _listBoxItem, value);
        }






        private static IconConverter? s_iconConverter;
        private static MainModel _mainModel = new MainModel();
        private static FileTree _fileTree;
 
        public static MainModel MainModel { get => _mainModel; }

        public FileTree FileTree 
        { 
            get => MainModel.FileTree; 
            set => this.RaiseAndSetIfChanged(ref _fileTree,value);
        }
        #region PROPERTIES
        public string Extensions { get => ".exe/ .jpeg/ .png"; }

        public static IMultiValueConverter FileIconConverter
        {
            get
            {
                if (s_iconConverter is null)
                {
                    var assetLoader = AvaloniaLocator.Current.GetRequiredService<IAssetLoader>();

                    using (var fileStream = assetLoader.Open(new Uri("avares://AvaloniaApplication1/Assets/file.png")))
                    using (var folderStream = assetLoader.Open(new Uri("avares://AvaloniaApplication1/Assets/folder.png")))
                    using (var folderOpenStream = assetLoader.Open(new Uri("avares://AvaloniaApplication1/Assets/folder.png")))
                    {
                        s_iconConverter = new IconConverter(new Bitmap(fileStream), new Bitmap(folderStream), new Bitmap(folderOpenStream));
                    }
                }

                return s_iconConverter;
            }
        }
        #endregion
        public FileTree Test(FileTree awdwa)
        {
            MainModel.FileTree = awdwa;
            return MainModel.FileTree;
        }
        #region COMMANDS
        public void GoToFolderCommand(FileTree selectedFile)
        {
            if (selectedFile != null && Directory.Exists(selectedFile.Path))
            {
                MainModel.GoToFolder(selectedFile);
                if (MainWindow.listBoxExplorer.ItemCount > 0)
                {
                    MainWindow.listBoxExplorer.SelectedIndex = 0;
                    MainWindow.listBoxExplorer.ItemContainerGenerator.ContainerFromIndex(0).Focus();
                }
            }
        }
        public void GoBackFolderCommand()
        {
            if (MainModel.FileTree != null && MainModel.FileTree.Parent != null)
            {
                MainModel.GoBackFolder();
                if (MainWindow.listBoxExplorer.ItemCount > 0)
                {
                    MainWindow.listBoxExplorer.SelectedIndex = 0;
                    MainWindow.listBoxExplorer.ItemContainerGenerator.ContainerFromIndex(0).Focus();
                }
            }
        }
        public void CancelCommand(Window window)
        {
            window.Close();
        }
        public void OkCommand(Window window)
        {

        }
        #endregion
    }
}
