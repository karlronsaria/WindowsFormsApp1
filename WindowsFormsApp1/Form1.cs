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
    public interface IDataContext
    {
        IEnumerable<string> GetTagsMatchingSubstring(string substring);

        IEnumerable<string> GetTagsMatchingPattern(string pattern);

        IEnumerable<string> GetNamesMatchingSubstring(string substring);

        IEnumerable<string> GetNamesMatchingPattern(string pattern);

        IEnumerable<string> GetNamesMatchingTag(string tag);

        IEnumerable<string> GetTagsMatchingName(string tag);

        void SetTags(IEnumerable<string> names, IEnumerable<string> tags);
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
        private readonly IDataContext _database;

        public Form1(IDataContext myDatabase)
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

        private Control SearchResultsPanel
        {
            get
            {
                return tableLayoutPanel1 as Control;
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

        private static void AddLayoutButton(Control parent, string text, EventHandler buttonClick)
        {
            Button btn = new Button()
            {
                Text = $"{text}",
                AutoSize = true,
            };

            btn.Click += buttonClick;
            parent.Controls.Add(btn);
        }

        private static Control LoadListSublayout(Control parent, string labelText)
        {
            parent.Controls.Add(
                new Panel()
                {
                    Height = 10
                }
            );

            parent.Controls.Add(
                new Label()
                {
                    Text = $"{labelText}",
                    // Text = "It's all I have to bring today, this and my heart beside, this and my heart and all the fields, and all the meadows wide. Be sure you count, should I forget, someone the sum could tell, this and my heart and all the bees, which in the clover dwell.",
                    // Dock = DockStyle.Fill,
                    Anchor = AnchorStyles.Left | AnchorStyles.Right,
                    AutoSize = true,
                }
            );

            FlowLayoutPanel myFlowLayoutPanel = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                // Dock = DockStyle.Fill,
            };

            parent.Controls.Add(myFlowLayoutPanel);
            return myFlowLayoutPanel;
        }

        private void AddListLayout(Control parent, string label, IEnumerable<string> list, EventHandler buttonClick)
        {
            Control myFlowLayoutPanel = LoadListSublayout(parent, label);

            foreach (string item in list)
            {
                AddLayoutButton(myFlowLayoutPanel, item, buttonClick);
            }
        }

        private void SetSampleListLayout(string str)
        {
            int numPanels = 5;

            for (int i = 1; i <= numPanels; i++)
            {
                string label = $"Panel {i}:";
                string c = "";

                var someList = from b in Enumerable.Range(0, 37)
                               where (c = String.Join(
                                    "",
                                    from a in Enumerable.Range(1, b)
                                    select "A"
                                )).Count() > 0
                               select $"foobar {b}: {c}";

                AddListLayout(SearchResultsPanel, label, someList, (s, e) => { });
            }
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

