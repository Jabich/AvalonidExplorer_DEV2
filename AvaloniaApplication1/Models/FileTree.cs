using Avalonia.Controls;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System.IO;

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
                NotifyFilter = NotifyFilters.LastWrite|
                               NotifyFilters.FileName|
                               NotifyFilters.DirectoryName|
                               NotifyFilters.Size,
            };
            _watcher.Created += CreatedFile;
            _watcher.Deleted += DeleteFile;
            _watcher.Renamed += RenamedFile;
            _watcher.Changed += ChangedFile;
        }
        private void CreatedFile(object sender, FileSystemEventArgs e)
        {
            
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
    }
}
