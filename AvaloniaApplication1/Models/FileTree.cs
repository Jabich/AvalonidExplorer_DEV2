using Avalonia.Threading;
using AvaloniaApplication1.Helper;
using ReactiveUI;
using System;
using System.IO;
using System.Linq;
using NLog;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AvaloniaApplication1.Models
{
    public class FileTree : ReactiveObject
    {

        private string _rootFolderPath = "/home/orpo/Desktop/1/2";
        private string _startWatcherPath = "/home/orpo/Desktop/";

        //private string _rootFolderPath = "C:\\1\\2";
        //private string _startWatcherPath = "C:\\";
        private static FileTreeNodeModel _openedFolder;
        private static FileSystemWatcher _watcher;

        public FileTreeNodeModel OpenedFolder { get => _openedFolder; set => this.RaiseAndSetIfChanged(ref _openedFolder, value); }

        public FileTree()
        {
            _openedFolder = new FileTreeNodeModel(_rootFolderPath, Directory.Exists(_rootFolderPath));
            StartWatch();
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
        }
        private void DeliteIdentialFile(FileTreeNodeModel parent)
        {
            var duplicates = parent.Children.GroupBy(x => x)
                                           .Where(g => g.Count() > 1)
                                           .Select(g => g.Key)
                                           .ToList();

            foreach (var duplicate in duplicates)
            {
                parent.Children.Remove(duplicate);
            }
        }
        private void CreatedFile(object sender, FileSystemEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (e.FullPath.StartsWith(_rootFolderPath))
                {
                    if (e.FullPath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) >=
                            _rootFolderPath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) &&
                            e.FullPath.StartsWith(_rootFolderPath))
                    {
                        string pathParentFolder = Path.GetDirectoryName(e.FullPath);
                        var parent = SearchFile(pathParentFolder);
                        if (parent != null)
                        {          
                            try
                            {
                                foreach (var item in parent.Children.ToList())
                                {
                                    if (item.Path == e.FullPath)
                                        return;
                                }
                                var addFile = new FileTreeNodeModel(e.FullPath, Directory.Exists(e.FullPath), parent);
                                addFile.IsChecked = addFile.Parent.IsChecked == false || addFile.Parent == null ? false : true;
                                addFile.Parent.HasChildren = !addFile.IsDirectory ? true : false;
                                parent.Children.Add(addFile);
                                DeliteIdentialFile(parent);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            });
        }
        private void DeleteFile(object sender, FileSystemEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (e.FullPath.StartsWith(_rootFolderPath) || _rootFolderPath.StartsWith(e.FullPath))
                {
                    if (OpenedFolder.Path.StartsWith(e.FullPath) && (e.FullPath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) <=
                        _rootFolderPath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)) &&
                        e.FullPath.Length <= _rootFolderPath.Length)
                    {
                        ClearOpenFolderForm(OpenedFolder);
                        return;
                    }

                    else if (OpenedFolder.Path.StartsWith(e.FullPath) || (OpenedFolder.Path.StartsWith(e.FullPath) || e.FullPath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) >
                    OpenedFolder.Path.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)))
                    {
                        try
                        {
                            var parent = SearchFile(e.FullPath);
                            foreach (var file in parent.Parent.Children.ToList())
                            {
                                if (file.Path == e.FullPath)
                                {
                                    parent.Parent.Children.Remove(file);
                                }
                                if (file.Path == e.FullPath && OpenedFolder.Path.StartsWith(e.FullPath))
                                    OpenedFolder = parent.Parent;
                            }
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        var parentFolder = SearchFile(Path.GetDirectoryName(e.FullPath));
                        try
                        {
                            foreach (var children in parentFolder.Children.ToList())
                            {
                                if (children.Path == e.FullPath)
                                {
                                    parentFolder.Children.Remove(children);
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
                //if (!e.FullPath.StartsWith(/*_rootFolderPath */"C:\\Users"))
                if (e.FullPath.StartsWith(_rootFolderPath) || _rootFolderPath.StartsWith(e.OldFullPath))
                {
                    bool a = OpenedFolder.Path.StartsWith(e.OldFullPath);
                    bool b = (e.OldFullPath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) <=
                        _rootFolderPath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar));
                    bool c = e.OldFullPath.Length <= _rootFolderPath.Length;



                    if (OpenedFolder.Path.StartsWith(e.OldFullPath) && (e.FullPath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) <=
                        _rootFolderPath.Count(c => c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)) &&
                        e.OldFullPath.Length <= _rootFolderPath.Length)
                    {
                        //ChangePathToTheRootFolder(OpenedFolder, e.FullPath);
                        //ChangePathChildren(OpenedFolder);
                        ClearOpenFolderForm(OpenedFolder);
                    }
                    else
                    {
                        try
                        {
                            var changedFile = SearchFile(e.OldFullPath);
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
                            StartWatch();
                        }
                    }
                }
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

        private FileTreeNodeModel SearchFile(string searchedFilePath)
        {
            {
                if (OpenedFolder.Path == searchedFilePath)
                    return OpenedFolder;
                else if (searchedFilePath.StartsWith(OpenedFolder.Path))
                    return SearchChildren(searchedFilePath, OpenedFolder);
                else if (OpenedFolder.Path.StartsWith(searchedFilePath))
                    return SearchTreeParent(searchedFilePath, OpenedFolder);
                else
                    return SearchChildren(searchedFilePath, SearchTreeParent(searchedFilePath, OpenedFolder));
            }
        }
        private FileTreeNodeModel SearchTreeParent(string searchedFilePath, FileTreeNodeModel openedFolder)
        {
            return searchedFilePath.StartsWith(openedFolder.Path) ? openedFolder : SearchTreeParent(searchedFilePath, openedFolder.Parent);
        }
        private FileTreeNodeModel SearchChildren(string searchedFilePath, FileTreeNodeModel rootFolder)
        {
            var maxMatchFile1 = rootFolder.Children.ToList().Where(x => searchedFilePath.StartsWith(x.Path)).OrderByDescending(x => x.Path.Length).FirstOrDefault();
            try
            {
                return maxMatchFile1.Path == searchedFilePath ? maxMatchFile1 : SearchChildren(searchedFilePath, maxMatchFile1);
            }
            catch
            {
                return null;
            }
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
