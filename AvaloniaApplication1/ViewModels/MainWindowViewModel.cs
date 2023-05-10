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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace AvaloniaApplication1.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static string _pathRootFolder = "C:\\1\\2";
        private static FileTree _fileTree = new FileTree(_pathRootFolder, true);
        private static IconConverter? s_iconConverter;
        private int _listBoxItem = 1;
        private string _extensions;
        private IEnumerable<FileTree> _filtredItems = _fileTree.File.Children!;
        public int ListBoxItem
        {
            get => _listBoxItem;
            set => this.RaiseAndSetIfChanged(ref _listBoxItem, value);
        }

        public string Extensions
        {
            get => _extensions;
            set
            {
                this.RaiseAndSetIfChanged(ref _extensions, value);
                if (Extensions != null)
                {
                    if (Extensions.Length != 0)
                    {
                        var extensions = Extensions.Split('/');
                        FilteredItems = FileTree.File.Children!.Where(file =>
                        {
                            if (Directory.Exists(file.Path))
                                return true;
                            string fileExtensions = Path.GetExtension(file.Name).TrimStart('.');
                            return extensions.Contains(fileExtensions);
                        });
                    }
                    else
                    {
                        FilteredItems = FileTree.File.Children;
                    }
                }
                else
                {
                    FilteredItems = FileTree.File.Children;
                    var extensions = Extensions.Split('/');
                    FilteredItems = FileTree.File.Children!.Where(file =>
                    {
                        if (Directory.Exists(file.Path))
                            return true;
                        string fileExtensions = Path.GetExtension(file.Name).TrimStart('.');
                        return extensions.Contains(fileExtensions);
                    });
                }
                

            }
        }

        public IEnumerable<FileTree> FilteredItems
        {
            get
            {
                //return FileTree.File.Children.Where(file => file.Name.Contains(".vsdx"));
                return _filtredItems;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _filtredItems, value);
            }
        }


        private MainModel _mainLogicModel;
        public static MainWindowViewModel mainModel;
        public MainWindowViewModel()
        {
            mainModel = this;
            _mainLogicModel = new MainModel(_fileTree);

        }
        public FileTree FileTree
        {
            get => _fileTree;
            set
            {
                this.RaiseAndSetIfChanged(ref _fileTree, value);
                _mainLogicModel.FileTree = value;
                FilteredItems = FileTree.File.Children;
                Extensions = Extensions;
            }
        }



        #region PROPERTIES

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
                WindowAssistant.FocusedListBox(MainWindow.listBoxExplorer);
            }
        }
        public void GoBackFolderCommand()
        {
            if (FileTree.Parent != null)
            {
                FileTree = FileTree.Parent;
                WindowAssistant.FocusedListBox(MainWindow.listBoxExplorer);
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
