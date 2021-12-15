using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PdfiumViewer;
using System.IO;
using Application;

namespace WindowsFormsApp1
{
    public interface IRecordContext
    {
        ICollection<Record> Records { get; set; }
    }

    public partial class Form1 : Form
    {
        const string STARTING_DIRECTORY = @"C:\Users\karlr\OneDrive\__POOL";
        // const string STARTING_DIRECTORY = @"C:\";
        const string PLAIN_TEXT_FONT_FAMILY = "Consolas";
        const int PLAIN_TEXT_POINT = 10;

        private IList<FileSystemInfo> _items;
        private Control _panelControl;
        private DirectoryInfo _currentDirectory;
        private IRecordContext _database;

        public Form1(IRecordContext myDatabase)
        {
            InitializeComponent();
            treeView1_Load(STARTING_DIRECTORY);
            _database = myDatabase;
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
            myRenderer.Font = new Font(PLAIN_TEXT_FONT_FAMILY, PLAIN_TEXT_POINT);
            myRenderer.BackColor = Color.Black;
            myRenderer.ForeColor = Color.Violet;
            return myRenderer;
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

        private void treeView1_Load(string directoryPath)
        {
            treeView1.Nodes.Clear();
            TreeNode rootNode;
            DirectoryInfo info = new DirectoryInfo(directoryPath);

            if (info.Exists)
            {
                _currentDirectory = info;

                rootNode = new TreeNode
                {
                    Text = info.Name,
                    Tag = info
                };
                
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
        }

        private void listView1_Load(TreeNode newSelected)
        {
            listView1_Load((DirectoryInfo)newSelected.Tag);
        }

        private void listView1_Load(DirectoryInfo newDirectory)
        {
            listView1.Items.Clear();
            _currentDirectory = newDirectory;
            _items = new List<FileSystemInfo>();
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item;

            foreach (DirectoryInfo dir in _currentDirectory.GetDirectories())
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

                    _items.Add(dir);
                    item.SubItems.AddRange(subItems);
                    listView1.Items.Add(item);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
            }

            foreach (FileInfo file in _currentDirectory.GetFiles()) 
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

                    _items.Add(file);
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

        private Control TagsLayoutPanel
        {
            get
            {
                return flowLayoutPanel1 as Control;
            }
        }

        private Control PreviewPane
        {
            get
            {
                return splitContainer2.Panel2 as Control;
            }
        }

        private void ClearPreviewPane()
        {
            PreviewPane.Controls.Remove(_panelControl);
        }

        private void SetTagsLayout()
        {
            
        }

        private void SetPreviewPane(Control myControl)
        {
            _panelControl = myControl;
            PreviewPane.Controls.Add(_panelControl);
            _panelControl.Dock = DockStyle.Fill;
        }

        private void GoToSelectedDirectory()
        {
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }

            var indices = listView1.SelectedIndices;
            var lastIndex = indices[indices.Count - 1];
            var modelItem = _items[lastIndex];
            string fullName = modelItem.FullName;
            bool isDirectory = modelItem.Attributes.HasFlag(FileAttributes.Directory);

            if (isDirectory)
            {
                treeView1_Load(fullName);
                listView1_Load((DirectoryInfo)modelItem);
            }
        }
    }
}

