using System;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace MyForms
{
    public partial class Form1 : Form
    {
        private readonly IDataContext _database;
        private readonly PreviewPane _myPreviewPane;
        private readonly TreeViewPane _myTreeViewPane;
        private readonly ListViewPane _myListViewPane;
        private CancellationTokenSource _searchBoxChanged;

        public Form1(IDataContext myDatabase, string startingDirectory)
        {
            _database = myDatabase;

            InitializeComponent();

            _myPreviewPane = new PreviewPane(splitContainer2.Panel2);
            _myTreeViewPane = new TreeViewPane(treeView1, startingDirectory);
            _myListViewPane = new ListViewPane(listView1, startingDirectory);
        }

        public Control SearchResultsPanel
        {
            get => searchResultLayoutPanel1;
        }

        public PreviewPane MyPreviewPane
        {
            get => _myPreviewPane;
        }

        public TreeViewPane MyTreeViewPane
        {
            get => _myTreeViewPane;
        }

        public ListViewPane MyListViewPane
        {
            get => _myListViewPane;
        }

        public CancellationTokenSource SearchBoxChanged
        {
            get => _searchBoxChanged;
            set => _searchBoxChanged = value;
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

        private async Task
        ChangeSearchResultsAsync(
                CancellationToken myCancellationToken
            )
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

        private async Task
        SetSelectedDirectoryTreeAsync()
        {
            var modelItem = MyListViewPane.GetLastSelectedItem();
            bool isDirectory = (bool)(modelItem?.Attributes.HasFlag(FileAttributes.Directory));

            if (isDirectory)
            {
                await Task.Run(() => MyTreeViewPane.Load((DirectoryInfo)modelItem));
                await Task.Run(() => MyListViewPane.Load((DirectoryInfo)modelItem));
            }
        }
    }
}

