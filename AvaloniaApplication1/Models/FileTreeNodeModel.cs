using Avalonia.Controls.Shapes;
using Avalonia.Threading;
using AvaloniaApplication1.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Models
{
    public class FileTreeNodeModel : ReactiveObject
    {
        #region FIELDS
        private string _path;
        private string _name;
        private long? _size;
        private DateTimeOffset? _modified;
        private FileSystemWatcher? _watcher;
        private ObservableCollection<FileTreeNodeModel>? _children;
        private bool _hasChildren = true;
        private bool _isExpanded;
        private string _version;
        private string? _hashSum;
        private bool _isChecked;
        #endregion

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                this.RaiseAndSetIfChanged(ref _isChecked, value);
                if (HasChildren)
                {
                    Task.Run(() =>
                    {
                        foreach (var child in Children)
                        {
                            child.IsChecked = value;
                        }
                    });
                }
            }
        }
        public string? HashSum
        {
            get => _hashSum;
            private set => this.RaiseAndSetIfChanged(ref _hashSum, value);
        }
        public string Version
        {
            get => _version;
            private set => this.RaiseAndSetIfChanged(ref _version, value);
        }
        public string Path
        {
            get => _path;
            set => this.RaiseAndSetIfChanged(ref _path, value);
        }
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        public long? Size
        {
            get => _size;
            set => this.RaiseAndSetIfChanged(ref _size, value);
        }
        public DateTimeOffset? Modified
        {
            get => _modified;
            set => this.RaiseAndSetIfChanged(ref _modified, value);
        }
        public bool HasChildren
        {
            get => _hasChildren;
            set => this.RaiseAndSetIfChanged(ref _hasChildren, value);
        }
        public bool IsExpanded
        {
            get => _isExpanded;
            set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
        }

        public bool IsDirectory { get; }
        public FileTreeNodeModel? Parent { get; set; }
        public ObservableCollection<FileTreeNodeModel> Children => _children ??= LoadChildren();
        public string ModifiedToString => IsDirectory ? Directory.GetCreationTime(_path).ToString() : Modified == null ? "-" : $"{Modified.Value}";

        public FileTreeNodeModel(string path, bool isDirectory, FileTreeNodeModel? parent = null, bool isRoot = false)
        {
            _path = path;
            _name = isRoot ? path : System.IO.Path.GetFileName(Path);
            _isExpanded = isRoot;
            IsDirectory = isDirectory;
            HasChildren = isDirectory;
            _isChecked = false;
            Parent = parent;
            try
            {
                if (!IsDirectory)
                {
                    _version = string.IsNullOrEmpty(FileVersionInfo.GetVersionInfo(Path).ProductVersion!)
                        ? "-"
                        : FileVersionInfo.GetVersionInfo(Path).ProductVersion!;

                    var info = new FileInfo(path);
                    Size = info.Length;
                    Modified = info.LastWriteTimeUtc;
                }
                else
                {
                    _version = "-";
                }
            }
            catch
            {

            }

        }

        private ObservableCollection<FileTreeNodeModel> LoadChildren()
        {
            if (!IsDirectory)
            {
                return null;
            }
            var options = new EnumerationOptions { IgnoreInaccessible = true };
            var result = new ObservableCollection<FileTreeNodeModel>();

            foreach (var d in Directory.EnumerateDirectories(Path, "*", options))
            {
                result.Add(new FileTreeNodeModel(d, true, this));
            }

            foreach (var f in Directory.EnumerateFiles(Path, "*", options))
            {
                result.Add(new FileTreeNodeModel(f, false, this));
            }

            _watcher = new FileSystemWatcher
            {
                Path = Path,
                NotifyFilter = NotifyFilters.FileName |
                               NotifyFilters.Size |
                               NotifyFilters.LastWrite |
                               NotifyFilters.DirectoryName,
                //IncludeSubdirectories = true,
                //EnableRaisingEvents = true,
            };

            _watcher.Changed += OnChanged;
            _watcher.Created += OnCreated;
            _watcher.Deleted += OnDeleted;
            _watcher.Renamed += OnRenamed;

            if (result.Count == 0)
                HasChildren = false;

            return result;
        }

        public static Comparison<FileTreeNodeModel?> SortAscending<T>(Func<FileTreeNodeModel, T> selector)
        {
            return (x, y) =>
            {
                switch (x)
                {
                    case null when y is null:
                        return 0;
                    case null:
                        return -1;
                    default:
                        if (y is null)
                            return 1;
                        break;
                }
                return x.IsDirectory == y.IsDirectory
                        ? Comparer<T>.Default.Compare(selector(x), selector(y))
                        : x.IsDirectory ? -1 : 1;
            };
        }

        public static Comparison<FileTreeNodeModel?> SortDescending<T>(Func<FileTreeNodeModel, T> selector)
        {
            return (x, y) =>
            {
                switch (x)
                {
                    case null when y is null:
                        return 0;
                    case null:
                        return 1;
                    default:
                        if (y is null)
                            return -1;
                        break;
                }
                return x.IsDirectory == y.IsDirectory
                        ? Comparer<T>.Default.Compare(selector(y), selector(x))
                        : x.IsDirectory ? -1 : 1;
            };
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            //var a = File.Exists(e.FullPath);


            if (e.ChangeType == WatcherChangeTypes.Changed && Directory.Exists(e.FullPath))
            {
                Dispatcher.UIThread.Post(() =>
                {
                    foreach (var child in _children!)
                    {
                        if (child.Path == e.FullPath)
                        {
                            if (!child.IsDirectory)
                            {
                                var info = new FileInfo(e.FullPath);

                                child.Size = info.Length;
                                child.Modified = info.LastWriteTimeUtc;
                            }
                            break;
                        }
                    }
                });
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var correctPath = System.IO.Path.Combine(Path, e.Name);
                if (Directory.Exists(correctPath) || File.Exists(correctPath))
                {
                    //var file = this;

                    var node = new FileTreeNodeModel(
                        correctPath,
                        File.GetAttributes(correctPath).HasFlag(FileAttributes.Directory),
                        this);

                    HasChildren = Parent != null ? true : false;

                    if (_path == System.IO.Path.GetDirectoryName(node.Path))
                    {
                        node.IsChecked = node.Parent.IsChecked == null ? false : node.Parent.IsChecked;
                        _children!.Add(node);

                    }
                }
            });
        }
        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            var openedFolder = FileTree.GetOpenedFolder();

            Dispatcher.UIThread.Post(() =>
            {
                for (var i = 0; i < _children!.Count; ++i)
                {
                    if (_children[i].Path == System.IO.Path.Combine(Path, e.Name))
                    {
                        _children.RemoveAt(i);
                        if (openedFolder.Path.Length >= System.IO.Path.Combine(Path, e.Name).Length)
                        {
                            MainWindowViewModel.OpenedFolder.ReturnToExistingFolder(this);
                        }
                        Debug.WriteLine($"Removed {e.FullPath}");
                        break;
                    }
                }
            });
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            //Dispatcher.UIThread.Post(() =>
            //{
                foreach (var child in _children!)
                {
                    if (child.Path == System.IO.Path.Combine(Path, e.OldName))
                    {
                        child.Path = System.IO.Path.Combine(Path, e.Name);
                        child.Name = e.Name ?? string.Empty;
                    }
                }
            //});
        }
    }
}