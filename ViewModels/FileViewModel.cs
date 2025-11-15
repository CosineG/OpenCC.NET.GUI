using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.Storage.Pickers.Provider;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using ModernWpf.Controls;
using OpenCC.NET.GUI.Enums;

namespace OpenCC.NET.GUI.ViewModels
{
    /// <summary>
    /// 转换所用的文件容器
    /// </summary>
    public class File : ObservableObject
    {
        private FileStatus _status;
        private string _outputFolder;

        public string Name { get; }
        public string Path { get; }

        public FileStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public string OutputFolder
        {
            get => _outputFolder;
            set => SetProperty(ref _outputFolder, value);
        }

        public File(string name, string path)
        {
            Status = FileStatus.Ready;
            Name = name;
            Path = path;
        }
    }

    public class FileViewModel : ObservableRecipient
    {
        private ObservableCollection<File> _files = new ObservableCollection<File>();
        private string _outputFolder;

        public string OutputFolder
        {
            get => _outputFolder;
            set => SetProperty(ref _outputFolder, value);
        }
        
        public ICommand AddFileCommand { get; }
        public ICommand RemoveFileCommand { get; }
        public ICommand ClearFileCommand { get; }
        public ICommand SelectOutputFolderCommand { get; }

        public FileViewModel()
        {
            AddFileCommand = new RelayCommand(AddFile);
            RemoveFileCommand = new RelayCommand<IList>(RemoveFile);
            ClearFileCommand = new RelayCommand<string>(ClearFile);
            SelectOutputFolderCommand = new RelayCommand(SelectOutputFolder);
            IsActive = true;
        }

        protected override void OnActivated()
        {
            // 收到请求后将列表中的文件作为消息发送
            Messenger.Register<FileViewModel, string, string>(this, "RequestFile", (_, m) =>
            {
                if (Files.Count != 0)
                {
                    Messenger.Send<ICollection<File>, string>(Files, "FileList");
                }
            });

            // 即使更新列表中文件当前的处理状态
            Messenger.Register<FileViewModel, File, string>(this, "FileStatusRunning",
                (_, m) => { m.Status = FileStatus.Running; });
            Messenger.Register<FileViewModel, File, string>(this, "FileStatusSuccess",
                (_, m) => { m.Status = FileStatus.Success; });
            Messenger.Register<FileViewModel, File, string>(this, "FileStatusFail",
                (_, m) => { m.Status = FileStatus.Fail; });
        }

        public ObservableCollection<File> Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }

        public void SelectOutputFolder()
        {
            var folderBrowserDialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog
            {
                Description = "请选择输出位置",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true
            };
            if (folderBrowserDialog.ShowDialog() == true)
            {
                OutputFolder = folderBrowserDialog.SelectedPath;
            }
        }

        public void AddFile()
        {
            if (string.IsNullOrEmpty(OutputFolder))
            {
                SelectOutputFolder();
                if (string.IsNullOrEmpty(OutputFolder))
                {
                    return;
                }
            }
            
            // 选择原文件
            var openFileDialog = new OpenFileDialog {Filter = "所有文件 (*.*)|*.*", Multiselect = true};
            if (openFileDialog.ShowDialog() == false) return;
            var dialogFilePaths = openFileDialog.FileNames;

            foreach (var filePath in dialogFilePaths)
            {
                if (Files.All(f => f.Path != filePath))
                {
                    var fileName = Path.GetFileName(filePath);
                    var file = new File(fileName, filePath) {OutputFolder = this.OutputFolder};
                    Files.Add(file);
                }
            }
        }

        public void RemoveFile(IList selectedFiles)
        {
            var selectedPaths = selectedFiles.Cast<File>().Select(f => f.Path).ToHashSet();
            for (var i = Files.Count - 1; i >= 0; i--)
            {
                if (selectedPaths.Contains(Files[i].Path))
                {
                    Files.RemoveAt(i);
                }
            }
        }

        public void ClearFile(string type)
        {
            switch (type)
            {
                case "All":
                    Files.Clear();
                    break;
                case "Success":
                    for (var i = Files.Count - 1; i >= 0; i--)
                    {
                        if (Files[i].Status == FileStatus.Success)
                        {
                            Files.RemoveAt(i);
                        }
                    }
                    break;
            }
        }
    }
}