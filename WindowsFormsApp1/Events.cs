using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private void openDirectoryStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                treeView1_Load(dialog.SelectedPath);
            }
        }

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
            ClearPreviewPane();

            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }

            var indices = listView1.SelectedIndices;
            var lastIndex = indices[indices.Count - 1];
            var modelItem = _items[lastIndex];

            var fullName = modelItem.FullName;
            var extension = modelItem.Extension;
            bool isDirectory = modelItem.Attributes.HasFlag(FileAttributes.Directory);

            if (!isDirectory)
            {
                switch (extension)
                {
                    case ".pdf":
                        SetPreviewPane(NewPdfPreview(fullName));
                        break;
                    default:
                        SetPreviewPane(NewPlainTextPreview(fullName));
                        break;
                }
            }
        }

        private void previewPane_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(
                e.Graphics,
                splitContainer1.Panel2.ClientRectangle,
                Color.Purple, 1, ButtonBorderStyle.Solid, // left
                Color.Purple, 1, ButtonBorderStyle.Solid, // top
                Color.Purple, 1, ButtonBorderStyle.Solid, // right
                Color.Purple, 1, ButtonBorderStyle.Solid  // bottom
            );
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string text = textBox1.Text;
            TagsLayoutPanel.Controls.Clear();

            if (text.Length == 0)
            {
                return;
            }

            var names = _database.GetNamesMatchingSubstring(text);

            if (names.Any())
            {
                AddListLayout("Documents:", names, DocumentButton_Click);
            }

            if (_database.GetTagsMatchingSubstring(text) is IEnumerable<string> tags && tags.Any())
            {
                AddListLayout("Tags:", tags, TagButton_Click);
            }
        }

        private void TagButton_Click(object sender, EventArgs e)
        {
            string text = (sender as Button).Text;
            TagsLayoutPanel.Controls.Clear();
            AddListLayout(
                $"Documents with the tag '{text}':",
                _database.GetNamesMatchingTag(text),
                DocumentButton_Click
            );
        }

        private void DocumentButton_Click(object sender, EventArgs e)
        {
            string text = (sender as Button).Text;
            TagsLayoutPanel.Controls.Clear();
            AddListLayout(
                $"Tags for the document '{text}':",
                _database.GetTagsMatchingName(text),
                TagButton_Click
            );
        }
    }
}
