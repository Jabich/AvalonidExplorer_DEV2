using Avalonia.Controls;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System.IO;
using System.Linq;
using RxFileSystemWatcher;
using System;
using AvaloniaApplication1.Helper;
using Avalonia.Threading;
using JetBrains.Annotations;
using System.Collections.ObjectModel;

namespace AvaloniaApplication1.Models
{
    public class FileTree : ReactiveObject
    {

        //private string _rootFolderPath = "/home/orpo/Desktop/1";
        //private string _startWatcherPath = "/home/orpo/Desktop/1";

        private string _rootFolderPath = "C:\\";
        private string _startWatcherPath = "C:\\";
        private static FileTreeNodeModel _openedFolder;
        private static FileTreeNodeModel _rootFolder;
        private static FileSystemWatcher _watcher;

        public FileTreeNodeModel OpenedFolder { get => _openedFolder; set => this.RaiseAndSetIfChanged(ref _openedFolder, value); }

        public FileTree()
        {
            _openedFolder = new FileTreeNodeModel(_rootFolderPath, Directory.Exists(_rootFolderPath));
            _rootFolder = _openedFolder;
            StartWatch();
        }
        public static void CheckGCWatcher()
        {
            WeakReference weakReference = new WeakReference(_watcher);
            var aawd = weakReference.IsAlive;
            var buferSizeCheck = _watcher.InternalBufferSize;
        }

        private void StartWatch()
        {
            _watcher = new FileSystemWatcher()
            {
                Path = _startWatcherPath,
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite |
                               NotifyFilters.FileName |
                               NotifyFilters.DirectoryName |
                               NotifyFilters.Size,
            };
            _watcher.Created += CreatedFile;
            _watcher.Deleted += DeleteFile;
            _watcher.Renamed += RenamedFile;
            _watcher.Changed += ChangedFile;
        }
        private void DeliteIdentialFile(FileTreeNodeModel parent, string passAddedFile)
        {
            //parent.Children.Remove(parent.Children.Where(x => x.Path == passAddedFile).First());
            parent.Children.Distinct();
        }
        //object locker = new object();
        private void CreatedFile(object sender, FileSystemEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {

                if (true)
                {
                    if (e.FullPath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) >=
                        _rootFolder.Path.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) &&
                        e.FullPath.StartsWith(_rootFolderPath))
                    {
                        string pathParentFolder = Path.GetDirectoryName(e.FullPath)/*?.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)*/;
                        var parent = SearchFile(pathParentFolder, OpenedFolder, Operation.CreateFile);
                        if (parent != null)
                        {
                            foreach (var item in parent.Children)
                            {
                                if (item.Path == e.FullPath)
                                    return;
                            }
                            parent.Children.Add(new FileTreeNodeModel(e.FullPath, Directory.Exists(e.FullPath), parent));
                            //DeliteIdentialFile(parent, e.FullPath);
                        }
                    }
                }
            });
        }



        //private void CreatedFile(object sender, FileSystemEventArgs e)
        //{
        //    Dispatcher.UIThread.InvokeAsync(() =>
        //    {
        //        // Если путь начинается с "C:\Users", то выходим из метода
        //        if (e.FullPath.StartsWith("C:\\Users"))
        //            return;

        //        // Если количество разделителей пути файла больше или равно количеству разделителей корневой папки,
        //        // и путь начинается с корневой папки, то добавляем файл в дерево
        //        if (e.FullPath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) >=
        //            _rootFolder.Path.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) &&
        //            e.FullPath.StartsWith(_rootFolderPath))
        //        {
        //            string parentFolderPath = Path.GetDirectoryName(e.FullPath);
        //            var parentNode = SearchFile(parentFolderPath, OpenedFolder, Operation.CreateFile);
        //            if (parentNode != null)
        //            {
        //                lock (parentNode.Children)
        //                {
        //                    // Если файл уже есть в дереве, то выходим из метода
        //                    if (parentNode.Children.Any(c => c.Path == e.FullPath))
        //                        return;

        //                    parentNode.Children.Add(new FileTreeNodeModel(e.FullPath, Directory.Exists(e.FullPath), parentNode));
        //                }
        //            }
        //        }
        //    });
        //}




        private void DeleteFile(object sender, FileSystemEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (true)
                {
                    if (e.FullPath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) <
                    _rootFolder.Path.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar))
                    {
                        ClearOpenFolderForm(OpenedFolder);
                        return;
                    }
                    else
                    {
                        var parentFolder = SearchFile(Path.GetDirectoryName(e.FullPath), OpenedFolder, Operation.DeleteFile) ?? OpenedFolder;
                        try
                        {
                            foreach (var children in parentFolder.Children)
                            {
                                if (children.Path == e.FullPath)
                                {
                                    parentFolder.Children.Remove(children);
                                    OpenedFolder = parentFolder;
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            });

        }
        private void RenamedFile(object sender, RenamedEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (!e.FullPath.StartsWith("C:\\Users"))
                {
                    if (e.FullPath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) <
                _rootFolder.Path.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar))
                    {
                        ChangePathToTheRootFolder(_rootFolder, e.FullPath);
                        ChangePathChildren(_rootFolder);
                        ClearOpenFolderForm(OpenedFolder);
                    }
                    else
                    {
                        try
                        {
                            var changedFile = SearchFile(e.OldFullPath, OpenedFolder, Operation.ChangeNameFile);
                            if (changedFile != null)
                            {
                                changedFile.Path = e.FullPath;
                                changedFile.Name = Path.GetFileName(e.FullPath);
                                ChangePathChildren(changedFile);
                            }
                            StartWatch();
                        }
                        catch
                        {

                        }
                    }
                }
            });
        }
        private void ChangedFile(object sender, FileSystemEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {

            });
        }

        public void GoToFolder(FileTreeNodeModel selectedFile)
        {
            if (selectedFile != null && Directory.Exists(selectedFile.Path))
            {
                OpenedFolder = selectedFile;
            }
        }
        public void GoBackFolder()
        {
            if (OpenedFolder != null && OpenedFolder.Parent != null)
            {
                OpenedFolder = OpenedFolder.Parent;
            }
        }
        private FileTreeNodeModel SearchFile(string searchedFilePath, FileTreeNodeModel openFolder, Operation operation)
        {
            try
            {
                if (openFolder.Path == searchedFilePath) { return openFolder; }
                else if (openFolder.Path.Length < searchedFilePath.Length) { return GoUp(searchedFilePath, openFolder, operation); }
                else { return GoDown(searchedFilePath, openFolder, operation); }
            }
            catch
            {

            }
            return null;

        }
        private FileTreeNodeModel GoUp(string searchedFilePath, FileTreeNodeModel openedFolder, Operation operation)
        {
            var maxMatchFile1 = openedFolder.Children.Where(x => searchedFilePath.StartsWith(x.Path)).OrderByDescending(x => x.Path.Length).FirstOrDefault();
            try
            {
                return maxMatchFile1.Path == searchedFilePath ? maxMatchFile1 : GoUp(searchedFilePath, maxMatchFile1, operation);
            }
            catch
            {
                return null;
            }
        }
        private FileTreeNodeModel GoDown(string searchedFile, FileTreeNodeModel openedFolder, Operation operation)
        {
            if (openedFolder.Path == searchedFile && (operation == Operation.DeleteFile || operation == Operation.CreateFile))
                return openedFolder;
            else if (openedFolder.Path == searchedFile && operation == Operation.ChangeNameFile)
                return openedFolder;
            else
                return GoDown(searchedFile, openedFolder.Parent, operation);
        }
        private void ChangePathChildren(FileTreeNodeModel changedFile)
        {
            foreach (var child in changedFile.Children.ToList())
            {
                child.Path = Path.Combine(changedFile.Path, child.Name);
                if (Directory.Exists(child.Path))
                {
                    ChangePathChildren(child);
                }

            }
        }
        private void ChangePathToTheRootFolder(FileTreeNodeModel fileTreeParent, string changedFilePath)
        {
            int countSeparatorsChangedFilePath = changedFilePath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar);
            string newTreeParentPath = changedFilePath;
            char[] separators = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
            string[] partsParentPath = fileTreeParent.Path.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < partsParentPath.Length; i++)
            {
                if (i >= countSeparatorsChangedFilePath + 1)
                    newTreeParentPath = Path.Combine(newTreeParentPath, partsParentPath[i]);
            }
            fileTreeParent.Path = newTreeParentPath;
        }
        private void ClearOpenFolderForm(FileTreeNodeModel openedFolder)
        {
            try
            {
                openedFolder.Path = "Исходный путь отсутствует!";
                foreach (var child in openedFolder.Children.ToList())
                {
                    openedFolder.Children.Remove(child);
                }
                openedFolder.Parent = null;
            }
            catch (Exception ex)
            {

            }

        }
    }
}
