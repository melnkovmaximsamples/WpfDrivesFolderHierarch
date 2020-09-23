using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.Mvvm;
using TreeView.Models;

namespace TreeView.ViewModels
{
    public class MainViewModel: BindableBase
    {
        public ObservableCollection<DirectoryNode> Roots { get; }
        private List<string> Drives { get; set; }
        public int DrivesCount => Drives.Count;

        public MainViewModel()
        {
            Roots = new ObservableCollection<DirectoryNode>();
            Drives = GetDriveNames().ToList();
        }

        public AsyncCommand EditCommand => new AsyncCommand(async () =>
        {
            foreach (var drivePath in Drives)
            {
                Roots.Add(await GetChildren(drivePath));
            }
        });

        private async Task<DirectoryNode> GetChildren(string path)
        {
            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);
            
            var node = new DirectoryNode()
            {
                Name = path
            }
        }

        private AsyncCommand<DirectoryNode> _expandedCommand;
        public AsyncCommand<DirectoryNode> ExpandedCommand => _expandedCommand ??= new AsyncCommand<DirectoryNode>(async (item) =>
        {
            await InsertChildrenToDirectoryNodeAsync(item.Directories);
        });

        private async Task InsertChildrenToDirectoryNodeAsync(IEnumerable<DirectoryNode> parentDirectories)
        {
            foreach (var DirectoryNode in parentDirectories)
            {
                if (DirectoryNode.Directories.Count > 0) continue;

                var childDirectories = GetChildrenDirectoriesAsync(DirectoryNode.Name);
                
                await foreach (var child in childDirectories)
                {
                    DirectoryNode.Directories.Add(child);
                }
            }
        }

        private async IAsyncEnumerable<DirectoryNode> GetChildrenDirectoriesAsync(string parentDirPath)
        {
            var childrenDirPaths = await Task.Run(() =>
            {
                Thread.Sleep(10);
                try
                {
                    return Directory.GetDirectories(parentDirPath, string.Empty, SearchOption.TopDirectoryOnly);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Trace.WriteLine(ex.Message);
                }

                return new string[] { };
            });

            foreach (var dir in childrenDirPaths)
            {
                var fullPath = Path.Combine(parentDirPath, dir);
                var directoryNode = new DirectoryNode()
                {
                    Name = dir,
                    LastModifiedDt = Directory.GetLastWriteTime(fullPath),
                    Owner = "Maks"

                };
                directoryNode.Files.Add(new FileNode(){Name = "LOL"});
                yield return directoryNode;
            }
        }

        private IEnumerable<string> GetDriveNames()
        {
            var result = DriveInfo.GetDrives()
                .Select(d => d.Name)
                .ToArray();

            return result;
        }
    }
}
