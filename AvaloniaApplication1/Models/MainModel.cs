using ReactiveUI;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models
{
    public class MainModel : ReactiveObject
    {
        private string _pathRootFolder = "C:\\1\\2";
        //private string _pathRootFolder = "/home/orpo/Desktop/1/2";
        public static FileTree? _fileTree;
        public static Watcher? _watcher;

        public static void TEST(FileTree fileTree)
        {
            fileTree.File.Children.RemoveAt(0);
        }
        public FileTree FileTree
        {
            get => _fileTree!;
            set => this.RaiseAndSetIfChanged(ref _fileTree, value);
        }
        public MainModel(FileTree fileTree)
        {
            _fileTree = fileTree;
            _watcher = new Watcher(_pathRootFolder, this, FileTree);
            Task.Run(() =>
            {
                CheckChangeRootPath();
            });
        }
        /// <summary>
        /// Поиск файла в дереве
        /// </summary>
        /// <param name="searchedFilePath"></param>
        /// <returns>Экземпляр типа FileTree (Файл)</returns>
        public FileTree SearchFile(string searchedFilePath)
        {
            if (FileTree.Path == searchedFilePath)
                return FileTree;
            else if (searchedFilePath.StartsWith(FileTree.Path))
                return SearchChildren(searchedFilePath, FileTree)!;
            else if (FileTree.Path.StartsWith(searchedFilePath))
                return SearchTreeParent(searchedFilePath, FileTree);
            else
                return SearchChildren(searchedFilePath, SearchTreeParent(searchedFilePath, FileTree))!;
        }
        /// <summary>
        /// Поиск родительского элемента в дереве
        /// </summary>
        /// <param name="searchedFilePath"></param>
        /// <param name="openedFolder"></param>
        /// <returns>Элемент типа FileTree (Файл)</returns>
        public FileTree SearchTreeParent(string searchedFilePath, FileTree openedFolder)
        {
            return searchedFilePath.StartsWith(openedFolder.Path)
                ? openedFolder
                : SearchTreeParent(searchedFilePath, openedFolder.Parent!);
        }   
        /// <summary>
        /// Поиск дочернего элементав дереве
        /// </summary>
        /// <param name="searchedFilePath"></param>
        /// <param name="rootFolder"></param>
        /// <returns>Элемент типа FileTree (Файл)</returns>
        public FileTree? SearchChildren(string searchedFilePath, FileTree rootFolder)
        {
            var maxMatchFile = rootFolder.Children.Where(x=> searchedFilePath.StartsWith(x.Path))
                                                  .FirstOrDefault()!;
            try
            {
                return maxMatchFile.Path == searchedFilePath
                    ? maxMatchFile
                    : SearchChildren(searchedFilePath, maxMatchFile);
            }
            catch (Exception ex)
            {
                Program.logger.Error($"Не удалось найти файл в дереве {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Изменение путей дочерних элементов при изменении переименовании родительской папки
        /// </summary>
        /// <param name="changedFile"></param>
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
        /// <summary>
        /// Очистка формы 
        /// </summary>
        public void ClearOpenFolderForm()
        {
            try
            {
                FileTree.Path = "Исходный путь отсутствует!";
                FileTree.Children.Clear();
                FileTree.Parent = null;
            }
            catch (Exception ex)
            {
                Program.logger.Error($"Ошибка отчиски формы. {ex}");
            }
        }
        /// <summary>
        /// Проверка изменения путей родительской папки и до неё
        /// </summary>
        private void CheckChangeRootPath()
        {
            while (true)
            {
                if (!Directory.Exists(_pathRootFolder))
                    ClearOpenFolderForm();
            }
        }
    }
}
