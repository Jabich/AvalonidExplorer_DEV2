using Avalonia.Controls;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System.IO;
using System.Linq;

namespace AvaloniaApplication1.Models
{
    public class FileTree : ReactiveObject
    {
        private string _rootFolder = "D:\\";
        private static FileTreeNodeModel _openedFolder;
        private FileSystemWatcher _watcher;

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
            //if (!File.Exists(e.FullPath) && !Directory.Exists(e.FullPath))
            foreach (var item in parent.Children)
            {
                if (item.Path == e.FullPath) 
                    return;
            }
            parent.Children.Add(new FileTreeNodeModel(e.FullPath, Directory.Exists(e.FullPath), parent));

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
        }
        private void DeleteFile(object sender, FileSystemEventArgs e)
        {

        }
        private void RenamedFile(object sender, RenamedEventArgs e)
        {

        }
        private void ChangedFile(object sender, FileSystemEventArgs e)
        {

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

        private FileTreeNodeModel SearchFile(string searchedFilePath, FileTreeNodeModel openFolder)
        {
            if (openFolder.Path == searchedFilePath) { return openFolder; }
            else if (openFolder.Path.Length < searchedFilePath.Length) { return GoUp(searchedFilePath, openFolder); }
            else { return GoDown(searchedFilePath, openFolder); }
        }
        private FileTreeNodeModel GoUp(string searchedFilePath, FileTreeNodeModel openedFolder)
        {
            //var children = openedFolder.Children.OrderByDescending(openedFolder => searchedFile.IndexOf(openedFolder.Path)).FirstOrDefault();
            //var children = openedFolder.Children.FirstOrDefault(file => file.Path.IndexOf(searchedFile) == 0);

            //var maxMatch = openedFolder.Children.OrderByDescending(openedFolder =>)
            var maxMatchFile = openedFolder.Children.OrderByDescending(f => f.Path.Split(Path.DirectorySeparatorChar).Intersect(searchedFilePath.Split(Path.DirectorySeparatorChar)).Count()).FirstOrDefault();
            return maxMatchFile.Path == searchedFilePath ? maxMatchFile : GoUp(searchedFilePath, maxMatchFile);
        }
        private FileTreeNodeModel GoDown(string searchedFile, FileTreeNodeModel openedFolder)
        {
            return openedFolder.Parent.Path == searchedFile ? openedFolder : GoDown(searchedFile, openedFolder.Parent);
        }

    }
}
