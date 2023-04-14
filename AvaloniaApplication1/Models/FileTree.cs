using Avalonia.Controls;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System.IO;
using System.Linq;

namespace AvaloniaApplication1.Models
{
    public class FileTree : ReactiveObject
    {
        private string _rootFolder = "D:\\1\\rdfgdfg\\3";
        private static FileTreeNodeModel _openedFolder;
        private static FileSystemWatcher _watcher;

        public FileTreeNodeModel OpenedFolder { get => _openedFolder; set => this.RaiseAndSetIfChanged(ref _openedFolder, value); }

        public FileTree()
        {
            _openedFolder = new FileTreeNodeModel(_rootFolder, Directory.Exists(_rootFolder));
            _watcher = new FileSystemWatcher()
            {
                Path = _rootFolder,
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite |
                               NotifyFilters.FileName |
                               NotifyFilters.DirectoryName |
                               NotifyFilters.Size,
                InternalBufferSize = 60 * 1024,
            };
            _watcher.Created += CreatedFile;
            _watcher.Deleted += DeleteFile;
            _watcher.Renamed += RenamedFile;
            _watcher.Changed += ChangedFile;
        }

        private void CreatedFile(object sender, FileSystemEventArgs e)
        {
            string pathParentFolder = Path.GetDirectoryName(e.FullPath);
            var parent = SearchFile(pathParentFolder, OpenedFolder);
            foreach (var item in parent.Children)
            {
                if (item.Path == e.FullPath)
                    return;
            }
            parent.Children.Add(new FileTreeNodeModel(e.FullPath, Directory.Exists(e.FullPath), parent));
            #region Test
            //if (OpenedFolder.Path.Length > Path.GetDirectoryName(e.FullPath).Length)
            //{
            //    SearchParentAndAddFile(e.FullPath, OpenedFolder);
            //}
            //else if (OpenedFolder.Path.Length < Path.GetDirectoryName(e.FullPath).Length)
            //{
            //    SearchChildrenAndAddFile(e.FullPath, OpenedFolder);
            //}
            //else
            //{
            //    var addedFile = new FileTreeNodeModel(
            //     e.FullPath,
            //     File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory),
            //     OpenedFolder);
            //    OpenedFolder.Children.Add(addedFile);
            //}

            //if (Directory.Exists(e.FullPath) || File.Exists(e.FullPath))
            //{
            //    //var file = this;

            //    var node = new FileTreeNodeModel(
            //        e.FullPath,
            //        File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory),
            //        OpenedFolder);

            //    OpenedFolder.HasChildren = OpenedFolder.Parent != null ? true : false;

            //    if (OpenedFolder.Path == Path.GetDirectoryName(node.Path))
            //    {
            //        node.IsChecked = node.Parent.IsChecked == null ? false : node.Parent.IsChecked;
            //        OpenedFolder.Children!.Add(node);

            //    }
            //}
            #endregion
        }
        private void DeleteFile(object sender, FileSystemEventArgs e)
        {
            var parentFolder = SearchFile(Path.GetDirectoryName(e.FullPath), OpenedFolder) ?? OpenedFolder;
            try
            {
                foreach (var children in parentFolder.Children)
                {
                    if (children.Path == e.FullPath)
                    {
                        parentFolder.Children.Remove(children);
                    }
                }
            }
            catch
            {
                //if(parentFolder.Parent.Children != null && parentFolder.Parent.Children.Contains(parentFolder)) 
                //parentFolder.Parent.Children.Remove(parentFolder);
            }

        }
        private void RenamedFile(object sender, RenamedEventArgs e)
        {
            try
            {
                var changedFile = SearchFile(e.OldFullPath, OpenedFolder);
                if (changedFile.Parent.Path == _rootFolder || changedFile.Path == e.OldFullPath)
                {
                    changedFile.Path = e.FullPath;
                    changedFile.Name = Path.GetFileName(e.FullPath);
                    
                }
                else
                {
                    changedFile.Parent.Path = e.FullPath;
                    changedFile.Parent.Name = Path.GetFileName(e.FullPath);
                }
       


                ChangePathChildren(changedFile.Parent);
            }
            catch
            {

            }
        }
        private void ChangedFile(object sender, FileSystemEventArgs e)
        {
            //var changedFile = SearchFile(e.FullPath, OpenedFolder);
            //if (changedFile.Path == e.FullPath)
            //{
            //    var info = new FileInfo(e.FullPath);
            //    changedFile.Size = info.Length;
            //    changedFile.Modified = info.LastWriteTimeUtc;
            //}
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
        public static FileTreeNodeModel GetOpenedFolder()
        { 
            return _openedFolder;
        }
        public void ReturnToExistingFolder(FileTreeNodeModel file)
        {
            OpenedFolder = file;
        }


        //Старая реализация
        private void SearchParentAndAddFile(string addedFilePath, FileTreeNodeModel openedFolder = null)
        {
            string pathFolderToAddFile = Path.GetDirectoryName(addedFilePath);
            if (openedFolder.Path == pathFolderToAddFile)
            {
                var addedFile = new FileTreeNodeModel(
                  addedFilePath,
                  File.GetAttributes(addedFilePath).HasFlag(FileAttributes.Directory),
                  OpenedFolder);
                openedFolder.Children.Add(addedFile);
            }
            else
            {
                SearchParentAndAddFile(addedFilePath, openedFolder.Parent);
            }
        }
        //========================
        private void SearchChildrenAndAddFile(string addedFilePath, FileTreeNodeModel openedFolder = null)
        {
            string pathFolderToAddFile = Path.GetDirectoryName(addedFilePath);
            if (openedFolder.Path == pathFolderToAddFile/* && !File.Exists(addedFilePath) && !Directory.Exists(addedFilePath)*/)
            {
                foreach (var file in openedFolder.Children)
                {
                    if (file.Path == addedFilePath)
                    {
                        return;
                    }
                }
                var addedFile = new FileTreeNodeModel(
                  addedFilePath,
                  File.GetAttributes(addedFilePath).HasFlag(FileAttributes.Directory),
                  OpenedFolder);
                openedFolder.Children.Add(addedFile);
                return;
            }
            else if (openedFolder.Path.Length < addedFilePath.Length)
            {
                var sdf = openedFolder.Children.OrderByDescending(openedFolder => openedFolder.Path.
                                                Intersect(pathFolderToAddFile).
                                                Count()).
                                                FirstOrDefault();
                SearchChildrenAndAddFile(addedFilePath, sdf);
            }
        }
        //========================


        private FileTreeNodeModel SearchFile(string searchedFilePath, FileTreeNodeModel openFolder)
        {
            if (openFolder.Path == searchedFilePath) { return openFolder; }
            else if (openFolder.Path.Length < searchedFilePath.Length) { return GoUp(searchedFilePath, openFolder); }
            else { return GoDown(searchedFilePath, openFolder); }
        }
        private FileTreeNodeModel GoUp(string searchedFilePath, FileTreeNodeModel openedFolder)
        {
            var maxMatchFile1 = openedFolder.Children.Where(x => searchedFilePath.StartsWith(x.Path)).OrderByDescending(x => x.Path.Length).FirstOrDefault();
            //var maxMatchFile = openedFolder.Children.OrderByDescending(f => f.Path.Split(Path.DirectorySeparatorChar).Intersect(searchedFilePath.Split(Path.DirectorySeparatorChar)).Count()).FirstOrDefault();
            try
            {
                return maxMatchFile1.Path == searchedFilePath ? maxMatchFile1 : GoUp(searchedFilePath, maxMatchFile1);
            }
            catch
            {
                return null;
            }
        }
        private FileTreeNodeModel GoDown(string searchedFile, FileTreeNodeModel openedFolder)
        {
            return openedFolder.Parent.Path == searchedFile ? openedFolder : GoDown(searchedFile, openedFolder.Parent);
        }
        private void ChangePathChildren(FileTreeNodeModel changedFile)
        {
            foreach (var child in changedFile.Children)
            {
                child.Path = Path.Combine(changedFile.Path, child.Name);
                if (Directory.Exists(child.Path))
                {
                    ChangePathChildren(child);
                }

            }
        }

    }
}
