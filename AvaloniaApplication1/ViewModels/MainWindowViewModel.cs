using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaApplication1.Helper;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Views;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Prism.Events;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.IO;
using System.Runtime.CompilerServices;

namespace AvaloniaApplication1.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly IEventAggregator _eventAggregator;
        private static IconConverter? s_iconConverter;
        private static MainModel _mainModel;
        private static FileTree _fileTree;
        public MainModel MainModel { get { return _mainModel; } }
        public FileTree FileTree
        {
            get => _fileTree;
            set => this.RaiseAndSetIfChanged(ref _fileTree, value);
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindowViewModel(/*IEventAggregator eventAggregator*/)
        {
            //_eventAggregator = eventAggregator;
            _mainModel = new MainModel();
            //_fileTree = new FileTree("C:\\1\\2", true);
            _fileTree = new FileTree("C:\\Users\\ORPO\\Desktop\\Kexi-master", true);
            _mainModel.PropertyChanged += OnMyPropertyChanged;
        }

        private void OnMyPropertyChanged(object sender, EventArgs e)
        {
            FileTree = _mainModel.FileTree;
        }
        #region COMMANDS
        public void GoToFolderCommand(FileTree selectedFile)
        {
            //FileTree = new FileTree("C:\\Users\\ORPO\\Desktop\\Kexi-master", true);
            FileTree = new FileTree("C:\\1\\2", true);
            //FileTree = selectedFile;



            //if (selectedFile != null && Directory.Exists(selectedFile.Path))
            //{
            //    _mainModel.GoToFolder(selectedFile);
            //    //if (MainWindow.listBoxExplorer.ItemCount > 0)
            //    //{
            //    //    MainWindow.listBoxExplorer.SelectedIndex = 0;
            //    //    MainWindow.listBoxExplorer.ItemContainerGenerator.ContainerFromIndex(0).Focus();
            //    //}
            //}
        }
        public void GoBackFolderCommand()
        {
            if (FileTree != null && FileTree.Parent != null)
            {
                //FileTree = FileTree.Parent;
                _mainModel.GoBackFolder();
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
        public void TEST(FileTree fileTree)
        {
            //fileTree.Children.Clear();
            fileTree = new FileTree("C:\\Users\\ORPO\\Desktop\\Kexi-master", true);
        }
        #endregion

        private void UpdateFileTree()
        {

        }
    }
}
