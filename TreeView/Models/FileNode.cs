using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm;

namespace TreeView.Models
{
    public class FileNode : BindableBase
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public int SizeBytes { get; set; }
        public string ParentAllocated { get; set; }
        public string Owner { get; set; }
        public string LastModified => LastModifiedDt.ToString("dd/MM/yyyy");

        public DateTime LastModifiedDt;
    }
}
