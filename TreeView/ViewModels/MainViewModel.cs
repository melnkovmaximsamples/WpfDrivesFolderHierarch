using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using TreeView.Models;

namespace TreeView.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<Node> Root { get; set; }
        public ObservableCollection<string> Drives { get; set; }
        public int DrivesCount => Drives.Count;

        public MainViewModel()
        {
            Root = new ObservableCollection<Node>();
            var driveNames = GetDriveNames();
            Drives = new ObservableCollection<string>(driveNames);
        }

        private AsyncCommand<string> _selectDisk;
        public AsyncCommand<string> SelectDisk => _selectDisk  ??= new AsyncCommand<string>(async (path) =>
        {
            Root.Clear();
            var dirs = Directory.GetDirectories(path);

            foreach (var dir in dirs)
            {
                Root.Add(new Node() { Name = dir });
            }

            await InsertChildrenToNodeAsync(Root);
        }, canExecuteMethod: (path) => !SelectDisk.IsExecuting && !ExpandedCommand.IsExecuting);

        private AsyncCommand<Node> _expandedCommand;
        public AsyncCommand<Node> ExpandedCommand => _expandedCommand ??= new AsyncCommand<Node>(async (item) =>
        {
            await InsertChildrenToNodeAsync(item.Nodes);
        });

        private async Task InsertChildrenToNodeAsync(IEnumerable<Node> parentNodes)
        {
            foreach (var node in parentNodes)
            {
                if (node.Nodes.Count > 0) continue;

                var childNodes = GetChildrenNodesAsync(node.Name);

                await foreach (var child in childNodes)
                {
                    node.Nodes.Add(child);
                }
            }
        }

        private async IAsyncEnumerable<Node> GetChildrenNodesAsync(string parentDirPath)
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
                yield return new Node() { Name = dir };
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
