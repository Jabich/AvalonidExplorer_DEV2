using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models
{
    public class MainModel: ReactiveObject
    {
        private string _pathRootFolder = "C:\\1\\2";
        public static FileTree _fileTree;
        public static Watcher _watcher;
        public static Logger Logger = new Logger();
        
        public FileTree FileTree
        {
            get => _fileTree;
            set => this.RaiseAndSetIfChanged(ref _fileTree, value);
        }

        public MainModel()
        {
            _fileTree = new FileTree(_pathRootFolder, true);
            _watcher = new Watcher(_pathRootFolder, _fileTree, this);
            Logger.logger.Debug("asdasdasdasd");
        }

        public void GoToFolder(FileTree selectedFile)
        {
            if (selectedFile != null && Directory.Exists(selectedFile.Path))
            {
                FileTree = selectedFile;
            }
        }

        public void GoBackFolder()
        {
            if (_fileTree != null && _fileTree.Parent != null)
            {
                FileTree = FileTree.Parent;
            }
        }

        public FileTree SearchFile(string searchedFilePath)
        {
            {
                if (FileTree.Path == searchedFilePath)
                    return FileTree;
                else if (searchedFilePath.StartsWith(FileTree.Path))
                    return SearchChildren(searchedFilePath, FileTree);
                else if (FileTree.Path.StartsWith(searchedFilePath))
                    return SearchTreeParent(searchedFilePath, FileTree);
                else
                    return SearchChildren(searchedFilePath, SearchTreeParent(searchedFilePath, FileTree));
            }
        }

        public FileTree SearchTreeParent(string searchedFilePath, FileTree openedFolder)
        {
            return searchedFilePath.StartsWith(openedFolder.Path)
                ? openedFolder
                : SearchTreeParent(searchedFilePath, openedFolder.Parent);
        }

        public FileTree SearchChildren(string searchedFilePath, FileTree rootFolder)
        {
            var maxMatchFile1 = rootFolder.Children.ToList().Where(x => searchedFilePath.StartsWith(x.Path))
                .OrderByDescending(x => x.Path.Length).FirstOrDefault();
            try
            {
                return maxMatchFile1.Path == searchedFilePath
                    ? maxMatchFile1
                    : SearchChildren(searchedFilePath, maxMatchFile1);
            }
            catch (Exception ex)
            {
                Logger.logger.Info("22222");
                return null;
            }
        }

        public void ChangePathChildren(FileTree changedFile)
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
        public void ClearOpenFolderForm()
        {
            try
            {
                FileTree.Path = "Исходный путь отсутствует!";
                foreach (var child in FileTree.Children.ToList())
                {
                    FileTree.Children.Remove(child);
                }

                FileTree.Parent = null;
            }
            catch (Exception ex)
            {
            }
        }
        public void TEST(FileTree fileTree)
        {
            FileTree = fileTree.Parent;
        }
    }
}
