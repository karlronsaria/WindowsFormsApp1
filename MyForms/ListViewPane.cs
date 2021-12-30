using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MyForms
{
    public class ListViewPane
    {
        private readonly ListView _myListView;
        private readonly List<FileSystemInfo> _listViewItems = new List<FileSystemInfo>();

        public ListViewPane(ListView myListView, string startingDirectory)
        {
            _myListView = myListView;

            Load(
                directoryPath: startingDirectory,
                isHandled: false
            );
        }

        public IList<FileSystemInfo> ActiveListItems
        {
            get => _listViewItems;
        }

        public void Load(string directoryPath, bool isHandled = true)
        {
            Load(new DirectoryInfo(directoryPath), isHandled);
        }

        public void Load(TreeNode newSelected, bool isHandled = true)
        {
            Load((DirectoryInfo)newSelected.Tag, isHandled);
        }

        public void Load(DirectoryInfo directory, bool isHandled = true)
        {
            MyForms.Forms.InvokeIfHandled(
                _myListView,
                s => (s as ListView).Items.Clear(),
                isHandled
            );

            ActiveListItems.Clear();
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item;

            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                try
                {
                    item = new ListViewItem(dir.Name, 0);

                    subItems = new ListViewItem.ListViewSubItem[]
                    {
                        new ListViewItem.ListViewSubItem(
                            owner: item,
                            text: dir.LastWriteTime.ToShortDateString()
                        )
                    };

                    item.SubItems.AddRange(subItems);

                    MyForms.Forms.InvokeIfHandled(
                        _myListView,
                        s => (s as ListView).Items.Add(item),
                        isHandled
                    );

                    ActiveListItems.Add(dir);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
            }

            foreach (FileInfo file in directory.GetFiles()) 
            {
                try
                {
                    item = new ListViewItem(file.Name, 1);

                    subItems = new ListViewItem.ListViewSubItem[]
                    {
                        new ListViewItem.ListViewSubItem(
                            owner: item,
                            text: file.LastWriteTime.ToShortDateString()
                        )
                    };

                    item.SubItems.AddRange(subItems);

                    MyForms.Forms.InvokeIfHandled(
                        _myListView,
                        s => (s as ListView).Items.Add(item),
                        isHandled
                    );

                    ActiveListItems.Add(file);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
            }

            MyForms.Forms.InvokeIfHandled(
                _myListView,
                s => (s as ListView).AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize),
                isHandled
            );
        }

        public FileSystemInfo GetLastSelectedItem()
        {
            if (_myListView.SelectedItems.Count == 0)
                return null;

            var indices = _myListView.SelectedIndices;
            var lastIndex = indices[indices.Count - 1];
            var modelItem = ActiveListItems[lastIndex];
            return modelItem;
        }
    }
}
