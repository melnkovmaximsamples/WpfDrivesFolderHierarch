using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using TreeView.Models;

namespace TreeView.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<DirectoryNode> Roots { get; }
        public ObservableCollection<string> Drives { get; }
        public int DrivesCount => Drives.Count;

        public MainViewModel()
        {
            Roots = new ObservableCollection<DirectoryNode>();
            var driveNames = GetDriveNames();
            Drives = new ObservableCollection<string>(driveNames);
        }

        public AsyncCommand AfterLoadCommand => new AsyncCommand(async () =>
        {
            foreach (var drivePath in Drives)
            {
                var dirs = Directory.GetDirectories(drivePath);
                var root = new DirectoryNode()
                {
                    Name = drivePath
                };
                Roots.Add(root);

                foreach (var dir in dirs)
                {
                    root.Directories.Add(new DirectoryNode() { Name = dir });
                }

                await InsertChildrenToNodeAsync(root.Directories);
            }
        });

        private AsyncCommand<DirectoryNode> _expandedCommand;

        public AsyncCommand<DirectoryNode> ExpandedCommand => _expandedCommand ??=
            new AsyncCommand<DirectoryNode>(async (item) =>
            {
                await InsertChildrenToNodeAsync(item.Directories);
            });

        private async Task InsertChildrenToNodeAsync(IEnumerable<DirectoryNode> parentNodes)
        {
            foreach (var DirectoryNode in parentNodes)
            {
                if (DirectoryNode.Directories.Count > 0) continue;

                var childNodes = GetChildrenNodesAsync(DirectoryNode.Name);

                await foreach (var child in childNodes)
                {
                    DirectoryNode.Directories.Add(child);
                }
            }
        }

        private async IAsyncEnumerable<DirectoryNode> GetChildrenNodesAsync(string parentDirPath)
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
                yield return new DirectoryNode() {Name = dir};
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
