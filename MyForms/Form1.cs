using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PdfiumViewer;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace MyForms
{
    public partial class Form1 : Form
    {
        public const string PLAIN_TEXT_FONT_FAMILY = "Consolas";
        public const int PLAIN_TEXT_POINT = 10;

        private Control _panelControl;
        private DirectoryInfo _currentDirectory;
        private readonly List<FileSystemInfo> _listViewItems = new List<FileSystemInfo>();
        private readonly IDataContext _database;
        private readonly string _startingDirectory;

        public IList<FileSystemInfo> ActiveListItems
        {
            get { return _listViewItems; }
        }

        public Form1(IDataContext myDatabase, string startingDirectory)
        {
            _database = myDatabase;
            _startingDirectory = startingDirectory;
            InitializeComponent();
            TreeView1_Load(_startingDirectory);
        }

        private CancellationTokenSource _searchBoxChanged;

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

        private void TreeView1_Load(string directoryPath)
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

        private void ListView1_Load(TreeNode newSelected)
        {
            ListView1_Load((DirectoryInfo)newSelected.Tag);
        }

        private void ListView1_Load(DirectoryInfo newDirectory)
        {
            listView1.Items.Clear();
            ActiveListItems.Clear();
            _currentDirectory = newDirectory;
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

                    item.SubItems.AddRange(subItems);
                    listView1.Items.Add(item);
                    ActiveListItems.Add(dir);
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

                    item.SubItems.AddRange(subItems);
                    listView1.Items.Add(item);
                    ActiveListItems.Add(file);
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
                return searchResultLayoutPanel1 as Control;
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

        private async Task ChangeSearchResultsAsync(CancellationToken myCancellationToken)
        {
            string text = searchBox1.Text;

            if (text.Length == 0)
                return;

            Control documentResultsPanel = null;
            Control tagResultsPanel = null;

            try
            {
                foreach (string item in _database.GetNamesMatchingSubstring(text))
                {
                    if (documentResultsPanel == null)
                        documentResultsPanel = Forms.LoadListSublayout(
                            parent: SearchResultsPanel,
                            labelText: "Documents:"
                        );

                    myCancellationToken.ThrowIfCancellationRequested();

                    await Task.Run(() => Forms.AddSearchResult(
                        parent: documentResultsPanel,
                        onDoubleClick: DocumentButton_DoubleClickAsync,
                        text: item
                    ));
                }

                foreach (string item in _database.GetTagsMatchingSubstring(text))
                {
                    if (tagResultsPanel == null)
                        tagResultsPanel = Forms.LoadListSublayout(
                            parent: SearchResultsPanel,
                            labelText: "Tags:"
                        );

                    myCancellationToken.ThrowIfCancellationRequested();

                    await Task.Run(() => Forms.AddSearchResult(
                        parent: tagResultsPanel,
                        onDoubleClick: TagButton_DoubleClickAsync,
                        text: item
                    ));
                }
            }
            catch (OperationCanceledException) { }
        }

        private void SetPreviewPane(Control myControl)
        {
            _panelControl = myControl;
            PreviewPane.Controls.Add(_panelControl);
            _panelControl.Dock = DockStyle.Fill;
        }

        private void SetSelectedDirectoryTree()
        {
            if (listView1.SelectedItems.Count == 0)
                return;

            var indices = listView1.SelectedIndices;
            var lastIndex = indices[indices.Count - 1];
            var modelItem = ActiveListItems[lastIndex];
            string fullName = modelItem.FullName;
            bool isDirectory = modelItem.Attributes.HasFlag(FileAttributes.Directory);

            if (isDirectory)
            {
                TreeView1_Load(fullName);
                ListView1_Load((DirectoryInfo)modelItem);
            }
        }
    }
}

