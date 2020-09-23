using System;
using DevExpress.Mvvm;
using System.Collections.ObjectModel;
using System.Data;
using System.DirectoryServices;
using System.IO;
using System.Linq;

namespace TreeView.Models
{
    public class DirectoryNode: BindableBase
    {
        public ObservableCollection<DirectoryNode> Directories { get; }
        public ObservableCollection<FileNode> Files { get; }
        public string Name { get; set; }
        public int SizeMb => 1;
        public int SizeBytes => Directories.Sum(dir => dir.SizeBytes) + Files.Sum(file => file.SizeBytes);
        public int FoldersCount => Directories.Sum(dir => dir.FoldersCount);
        public int FilesCount => Directories.Sum(dir => dir.FilesCount) + Files.Count;

        public string ParentAllocated { get; set; }
        public string Owner { get; set; }
        public string LastModified => LastModifiedDt.ToString("dd/MM/yyyy");

        public DateTime LastModifiedDt;
        public DirectoryNode()
        {
            Directories = new ObservableCollection<DirectoryNode>();
            Files = new ObservableCollection<FileNode>();
        }
    }
}
