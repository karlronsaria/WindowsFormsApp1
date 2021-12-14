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
        const string STARTING_DIRECTORY = @"C:\Users\karlr\OneDrive\__POOL";
        // const string STARTING_DIRECTORY = @"C:\";

        private IList<FileSystemInfo> _items;
        private Control _panelControl;

        public Form1()
        {
            InitializeComponent();
            PopulateTreeView(STARTING_DIRECTORY);
        }

        private Control NewPdfPreview(string filePath)
        {
            var myRenderer = new PdfRenderer();
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            myRenderer.Load(PdfDocument.Load(new MemoryStream(bytes)));
            return myRenderer;
        }

        private Control NewPlainTextPreview(string filePath)
        {
            var myRenderer = new RichTextBox();
            myRenderer.LoadFile(filePath, RichTextBoxStreamType.PlainText);
            return myRenderer;
        }

        private void openDirectoryStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                PopulateTreeView(dialog.SelectedPath);
            }
        }

        private void PopulateTreeView(string directoryPath)
        {
            treeView1.Nodes.Clear();
            TreeNode rootNode;
            DirectoryInfo info = new DirectoryInfo(directoryPath);

            if (info.Exists)
            {
                rootNode = new TreeNode
                {
                    Text = info.Name,
                    Tag = info
                };
                
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;

            foreach (DirectoryInfo subDir in subDirs)
            {
                try
                {
                    aNode = new TreeNode
                    {
                        Text = subDir.Name,
                        ImageIndex = 0,
                        SelectedImageIndex = 0,
                        Tag = subDir,
                        ImageKey = "folder"
                    };

                    subSubDirs = subDir.GetDirectories();

                    if (subSubDirs.Length != 0)
                    {
                        GetDirectories(subSubDirs, aNode);
                    }

                    nodeToAddTo.Nodes.Add(aNode);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // // No smaller than design time size
            // this.MinimumSize = new System.Drawing.Size(this.Width, this.Height);

            // No larger than screen size
            this.MaximumSize = new System.Drawing.Size(
                Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height
            );

            // this.AutoSize = true;
            // this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        private void listView1_Load(TreeNode newSelected)
        {
            listView1.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item;

            _items = new List<FileSystemInfo>();

            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                try
                {
                    _items.Add(dir);
                    item = new ListViewItem(dir.Name, 0);

                    subItems = new ListViewItem.ListViewSubItem[] {
                        new ListViewItem.ListViewSubItem(item, "Directory"),
                        new ListViewItem.ListViewSubItem(
                            item,
                            dir.LastAccessTime.ToShortDateString()
                        )
                    };

                    item.SubItems.AddRange(subItems);
                    listView1.Items.Add(item);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
            }

            foreach (FileInfo file in nodeDirInfo.GetFiles()) 
            {
                try
                {
                    _items.Add(file);
                    item = new ListViewItem(file.Name, 1);

                    subItems = new ListViewItem.ListViewSubItem[] {
                        new ListViewItem.ListViewSubItem(item, "File"),
                        new ListViewItem.ListViewSubItem(
                            item,
                            file.LastAccessTime.ToShortDateString()
                        )
                    };

                    item.SubItems.AddRange(subItems);
                    listView1.Items.Add(item);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
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
            }
        }

        private void SetPreviewPane(Control myControl)
        {
            splitContainer1.Panel2.Controls.Remove(_panelControl);
            _panelControl = myControl;
            splitContainer1.Panel2.Controls.Add(_panelControl);
            _panelControl.Dock = DockStyle.Fill;
        }

        private void listView1_SelectedIndexChanged_UsingItems(
            object sender, System.EventArgs e)
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

public class MySR : ToolStripSystemRenderer
{
    public MySR() { }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        base.OnRenderToolStripBorder(e);
        e.Graphics.FillRectangle(Brushes.White, e.ConnectedArea);
    }
}

