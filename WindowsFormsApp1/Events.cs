using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PdfiumViewer;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            listView1_Load(e.Node);
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                case Keys.Space:
                    if (treeView1.SelectedNode is TreeNode n)
                    {
                        listView1_Load(n);
                    }

                    // link: https://stackoverflow.com/questions/13952932/disable-beep-of-enter-and-escape-key-c-sharp
                    // retrieved: 2021_12_13
                    e.Handled = e.SuppressKeyPress = true;
                    break;
                case Keys.Up:
                    if (e.KeyData.HasFlag(Keys.Alt))
                    {
                        var parent = _currentDirectory.Parent;
                        treeView1_Load(parent.FullName);
                        listView1_Load(parent);
                    }
                    break;
            }
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    GoToSelectedDirectory();
                    break;
                case Keys.Up:
                    if (e.KeyData.HasFlag(Keys.Alt))
                    {
                        var parent = _currentDirectory.Parent;
                        treeView1_Load(parent.FullName);
                        listView1_Load(parent);
                    }
                    break;
            }
        }

        private void listView1_DoubleClick(object sender, System.EventArgs e)
        {
            GoToSelectedDirectory();
        }

        private void listView1_SelectedIndexChanged_UsingItems(object sender, System.EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }

            var indices = listView1.SelectedIndices;
            var lastIndex = indices[indices.Count - 1];
            var viewItem = listView1.Items[lastIndex];
            var modelItem = _items[lastIndex];

            var fullName = modelItem.FullName;
            var extension = modelItem.Extension;
            var type = viewItem.SubItems[1].Text;

            switch (type)
            {
                case "File":
                    switch (extension)
                    {
                        case ".pdf":
                            SetPreviewPane(NewPdfPreview(fullName));
                            break;
                        default:
                            SetPreviewPane(NewPlainTextPreview(fullName));
                            break;
                    }

                    break;
            }
        }
    }
}
