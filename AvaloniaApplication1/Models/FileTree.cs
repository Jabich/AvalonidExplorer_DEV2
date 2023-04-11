using Avalonia.Controls;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System.IO;

namespace AvaloniaApplication1.Models
{
    public class FileTree : ReactiveObject
    { 

        private string _rootFolder = "D:\\";
        public static FileTreeNodeModel _openedFolder;
        public FileTreeNodeModel OpenedFolder { get => _openedFolder; set => this.RaiseAndSetIfChanged(ref _openedFolder, value); }

        public FileTree()
        {
            _openedFolder = new FileTreeNodeModel(_rootFolder, Directory.Exists(_rootFolder));
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
