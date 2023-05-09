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
using System.Reactive.Linq;

namespace AvaloniaApplication1.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static string _pathRootFolder = "C:\\1\\2";
        private static FileTree _fileTree = new FileTree(_pathRootFolder, true);
        private static IconConverter? s_iconConverter;
        private int _listBoxItem = 1;
        public int ListBoxItem
        {
            get => _listBoxItem;
            set => this.RaiseAndSetIfChanged(ref _listBoxItem, value);
        }




        private MainModel _mainLogicModel;
        public MainWindowViewModel()
        {
            _mainLogicModel = new MainModel(FileTree);
        }
        public FileTree FileTree 
        { 
            get => _fileTree; 
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

        #region COMMANDS
        public void GoToFolderCommand(FileTree selectedFile)
        {
            if (selectedFile != null && Directory.Exists(selectedFile.Path))
            {
                FileTree = selectedFile;
                //_mainLogicModel.GoToFolder(selectedFile);
                //if (MainWindow.listBoxExplorer.ItemCount > 0)
                //{
                //    MainWindow.listBoxExplorer.SelectedIndex = 0;
                //    MainWindow.listBoxExplorer.ItemContainerGenerator.ContainerFromIndex(0).Focus();
                //}
            }
        }
        public void GoBackFolderCommand()
        {
            if (FileTree.Parent != null)
            {
                FileTree = FileTree.Parent; 
                //_mainLogicModel.GoBackFolder();
                //if (MainWindow.listBoxExplorer.ItemCount > 0)
                //{
                //    MainWindow.listBoxExplorer.SelectedIndex = 0;
                //    MainWindow.listBoxExplorer.ItemContainerGenerator.ContainerFromIndex(0).Focus();
                //}
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
