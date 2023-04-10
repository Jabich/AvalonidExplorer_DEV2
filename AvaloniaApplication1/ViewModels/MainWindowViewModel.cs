using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AvaloniaApplication1.Helper;
using AvaloniaApplication1.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System;
using System.IO;

namespace AvaloniaApplication1.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly string rootFolder = "D:\\";
        private static IconConverter? s_iconConverter;
        private static FileTreeNodeModel? _fileTree;

        public FileTreeNodeModel? FileTree
        {
            get => _fileTree;
            set => this.RaiseAndSetIfChanged(ref _fileTree, value);
        }
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

        public MainWindowViewModel()
        {
            FileTree = new FileTreeNodeModel(rootFolder, Directory.Exists(rootFolder));
        }

        #region COMMANDS
        public void GoToFolder(FileTreeNodeModel selectedFile)
        {
            if (selectedFile != null && Directory.Exists(selectedFile.Path))
            {
                FileTree = selectedFile;
            }
        }
        public void GoBackFolder()
        {
            if (FileTree != null && FileTree.Parent != null)
            {
                FileTree = FileTree.Parent;
            }
        }
        public void CancelCommand(Window window)
        {
            window.Close();
        }
        public void OkCommand(Window window)
        {
            window.Close();
        }

        #endregion

        #region METHODS
        private static FileTreeNodeModel CheckParents(FileTreeNodeModel fileTree)
        {
            return Directory.Exists(fileTree.Parent.Path) ? fileTree.Parent : CheckParents(fileTree.Parent);
        }
        public static void TEST(FileTreeNodeModel parent)
        {
            //FileTree = parent;
        }
        #endregion
    }
}
