using Avalonia.Threading;
using ReactiveUI;
using System;
using System.IO;
using System.Linq;

namespace AvaloniaApplication1.Models
{
    public class Watcher : ReactiveObject
    {
        public FileSystemWatcher? _watcher;
        private static string? _rootFolderPath;
        private MainModel _mainModel;
        public MainModel MainModel
        {
            get => _mainModel;
            set => this.RaiseAndSetIfChanged(ref _mainModel, value);
        }
        public Watcher(string rootFolderPath, MainModel mainModel)
        {
            _rootFolderPath = rootFolderPath;
            _mainModel = mainModel;
            StartWatch();
        }
        private void StartWatch()
        {
            _watcher = new FileSystemWatcher()
            {
                Path = _rootFolderPath!,
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
        /// <summary>
        /// Метод обработчик FSW
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreatedFile(object sender, FileSystemEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    var parent = MainModel.SearchFile(Path.GetDirectoryName(e.FullPath)!);
                    if (parent.Children.Where(x => x.Path == e.FullPath).FirstOrDefault() != null) return;
                    var addFile = new FileTree(e.FullPath, Directory.Exists(e.FullPath), parent);
                    addFile.IsChecked = addFile.Parent != null && addFile.Parent.IsChecked != false;
                    addFile.HasChildren = addFile.IsDirectory && addFile.Children.Count != 0 || addFile.Children != null;
                    addFile.Parent!.HasChildren = true;
                    parent.Children.Add(addFile);
                }
                catch (Exception ex)
                {
                    Program.logger.Error($"Ошибка добавления файла. {ex.Message}");
                }
            });
        }
        /// <summary>
        /// Метод обработчик FSW
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteFile(object sender, FileSystemEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    var file = MainModel.SearchFile(e.FullPath);
                    if(file == null) return;
                    file.Parent!.Children.Remove(file);
                    if (e.FullPath == MainModel.FileTree.Path)
                        MainModel.GoBackFolder();
                }
                catch (Exception ex)
                {
                    Program.logger.Error($"Ошибка удаления файла. {ex.Message}");
                }
            });
        }
        /// <summary>
        /// Метод обработчик FSW
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenamedFile(object sender, RenamedEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    var changedFile = MainModel.SearchFile(e.OldFullPath);
                    var duplicat = changedFile.Parent!.Children.Where(x => x.Path == e.FullPath).FirstOrDefault();
                    changedFile.Parent.Children.Remove(duplicat!);
                    changedFile.Path = e.FullPath;
                    changedFile.Name = Path.GetFileName(e.FullPath);
                    if (Directory.Exists(changedFile.Path))
                        MainModel.ChangePathChildren(changedFile);
                }
                catch (Exception ex)
                {
                    StartWatch();
                    Program.logger.Error($"Ошибка переименования файла. {ex.Message}");
                }
            });
        }
    }
}
