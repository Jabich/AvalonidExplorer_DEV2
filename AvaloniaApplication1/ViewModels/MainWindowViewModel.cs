using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaApplication1.Helper;
using AvaloniaApplication1.Models;
using ReactiveUI;
using System;
using System.IO;

namespace AvaloniaApplication1.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region FIELDS
        private int _itemIndex = 0;
        private static IconConverter? s_iconConverter;
        private static MainModel _mainModel = new MainModel();
        private static FileTree? _fileTree;
        #endregion

        #region PROPERTIES
        public int ItemIndex
        {
            get => _itemIndex;
            set => this.RaiseAndSetIfChanged(ref _itemIndex, value);
        }
        public string Extensions { get => "exe/ jpeg/ png"; }

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

        public FileTree FileTree
        {
            get => _mainModel.FileTree;
            set => this.RaiseAndSetIfChanged(ref _fileTree, value);
        }
        #endregion

        public MainWindowViewModel()
        {
            _fileTree = _mainModel.FileTree;
            _mainModel.PropertyChanged += OnMyPropertyChanged!;
        }
        private void OnMyPropertyChanged(object sender, EventArgs e)
        {
            FileTree = _mainModel.FileTree!;
        }
        #region COMMANDS
        public void GoToFolderCommand()
        {
            if (ItemIndex >= 0 && Directory.Exists(FileTree.Children?[ItemIndex].Path) && 
                FileTree.Children.Count != 0)
                _mainModel.GoToFolder(FileTree.Children[ItemIndex]);
        }
        public void GoBackFolderCommand()
        {
            if (_mainModel.FileTree.Parent != null)
                _mainModel.GoBackFolder();
        }
        public void CancelCommand(Window window)
        {
            window.Close();
        }
        public void OkCommand(Window window)
        {

        }
        public void UpCommand()
        {
            if (ItemIndex > 0)
                ItemIndex--;
            else if (ItemIndex <= 0)
                ItemIndex = FileTree.Children!.Count - 1;
        }
        public void DownCommand()
        {
            if (ItemIndex < FileTree.Children?.Count - 1)
                ItemIndex++;
            else if (ItemIndex == FileTree.Children?.Count - 1)
                ItemIndex = 0;
        }
        #endregion
    }
}
