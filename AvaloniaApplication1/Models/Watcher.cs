﻿using Avalonia.Threading;
using AvaloniaApplication1.ViewModels;
using DynamicData.Experimental;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models
{
    public class Watcher: ReactiveObject
    {
        public FileSystemWatcher _watcher;
        private static string _rootFolderPath;
        private FileTree _fileTree;
        private MainModel _mainModel;
        public MainModel MainModel
        {
            get => _mainModel;
            set => this.RaiseAndSetIfChanged(ref _mainModel, value);
        }
        public FileTree FileTree
        {
            get => _fileTree;
            set => this.RaiseAndSetIfChanged(ref _fileTree, value);
        }
        public Watcher(string rootFolderPath, FileTree fileTree, MainModel mainModel)
        {
            _rootFolderPath = rootFolderPath;
            _fileTree = fileTree;
            _mainModel = mainModel;
            StartWatch();
        }
        private void StartWatch()
        {
            _watcher = new FileSystemWatcher()
            {
                Path = _rootFolderPath,
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

        private void CreatedFile(object sender, FileSystemEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (e.FullPath.StartsWith(_rootFolderPath))
                {
                    if (e.FullPath.Count(c =>
                            c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) >=
                        _rootFolderPath.Count(c =>
                            c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) &&
                        e.FullPath.StartsWith(_rootFolderPath))
                    {
                        string pathParentFolder = Path.GetDirectoryName(e.FullPath);
                        var parent = MainModel.SearchFile(pathParentFolder);
                        if (parent != null)
                        {
                            try
                            {
                                foreach (var item in parent.Children.ToList())
                                {
                                    if (item.Path == e.FullPath)
                                        return;
                                }

                                var addFile = new FileTree(e.FullPath, Directory.Exists(e.FullPath), parent);
                                addFile.IsChecked = addFile.Parent.IsChecked == false || addFile.Parent == null
                                    ? false
                                    : true;
                                addFile.HasChildren =
                                    addFile.IsDirectory && addFile.Children.Count != 0 || addFile.Children != null
                                        ? true
                                        : false;
                                addFile.Parent.HasChildren = true;
                                parent.Children.Add(addFile);
                            }
                            catch (Exception ex)
                            {
                                Program.logger.Error("awdwad");

                                //logger.logger.Info($"Файл {ex.Message} - {e.Name} уже был создан");
                                //logger.Info($"Файл {ex.Message} - {e.Name} уже был создан");
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
                    if (_fileTree.Path.StartsWith(e.FullPath) &&
                        (e.FullPath.Count(c =>
                             c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) <=
                         _rootFolderPath.Count(c =>
                             c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)) &&
                        e.FullPath.Length <= _rootFolderPath.Length)
                    {
                        _mainModel.ClearOpenFolderForm();
                        return;
                    }

                    else if (_fileTree.Path.StartsWith(e.FullPath) || (_fileTree.Path.StartsWith(e.FullPath) ||
                                                                          e.FullPath.Count(c =>
                                                                              c == Path.DirectorySeparatorChar ||
                                                                              c == Path.AltDirectorySeparatorChar) >
                                                                          _fileTree.Path.Count(c =>
                                                                              c == Path.DirectorySeparatorChar ||
                                                                              c == Path.AltDirectorySeparatorChar)))
                    {
                        try
                        {
                            var parent = MainModel.SearchFile(e.FullPath);
                            foreach (var file in parent.Parent.Children.ToList())
                            {
                                if (file.Path == e.FullPath)
                                {
                                    parent.Parent.Children.Remove(file);
                                    //MainModel.FileTree = new FileTree("C:\\Users\\ORPO\\Desktop\\AstraLinuxFoldr", true);
                                    //FileTree.Children.Clear();
                                    //MainModel.GoBackFolder();
                                    //MainModel.FileTree = parent.Parent.Parent;


                                    //MainModel.FileTree.Children.Clear();
                                    //FileTree = new FileTree("C:\\Users\\ORPO\\Desktop\\AstraLinuxFoldr", true);
                                }

                                if (file.Path == e.FullPath && _mainModel.FileTree.Path.StartsWith(e.FullPath))
                                {
                                    MainModel.GoBackFolder();
                                    //var gkjghj = MainWindowViewModel.MainModel.FileTree;
                                }
                            }
                            //var dfgdfg = MainWindowViewModel.MainModel.FileTree;
                        }
                        catch
                        {
                            //var dfgdfg = MainWindowViewModel.MainModel.FileTree;
                            //Logger.logger.Info("22222");
                            Program.logger.Error("awdwad");
                        }
                    }
                    else
                    {
                        var parentFolder = MainModel.SearchFile(Path.GetDirectoryName(e.FullPath));
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
                        catch (Exception ex)
                        {
                            //Logger.logger.Info("22222");
                            Program.logger.Error("awdwad");
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
                    if (_fileTree.Path.StartsWith(e.OldFullPath) &&
                        (e.FullPath.Count(c =>
                             c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar) <=
                         _rootFolderPath.Count(c =>
                             c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)) &&
                        e.OldFullPath.Length <= _rootFolderPath.Length)
                    {
                        _mainModel.ClearOpenFolderForm();
                    }
                    else
                    {
                        try
                        {
                            var changedFile = _mainModel.SearchFile(e.OldFullPath);
                            foreach (var file in changedFile.Parent.Children.ToList())
                            {
                                if (file.Name == Path.GetFileName(e.FullPath))
                                {
                                    changedFile.Parent.Children.Remove(file);
                                }
                            }

                            if (changedFile != null)
                            {
                                changedFile.Path = e.FullPath;
                                changedFile.Name = Path.GetFileName(e.FullPath);
                                _mainModel.ChangePathChildren(changedFile);
                            }

                            StartWatch();
                        }
                        catch (Exception ex)
                        {
                            Program.logger.Error("awdwad");
                            //Logger.logger.Info("22222");
                            StartWatch();
                        }
                    }
                }
            });
        }
    }
}
