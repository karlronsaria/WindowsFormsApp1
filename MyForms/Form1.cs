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
            this.MaximumSize = new System.Drawing.Size(
                Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height
            );
        }

        private async Task
        ChangeSearchResultsAsync(
                CancellationToken myCancellationToken
            )
        {
            string text = searchBox1.Text;

            if (text.Length == 0)
                return;

            SearchResultLayout documentResultsPanel = null;
            SearchResultLayout tagResultsPanel = null;

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

                    await Task.Run(() => documentResultsPanel.AddSearchResult(
                        text: item,
                        onDoubleClick: DocumentButton_DoubleClickAsync
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

                    await Task.Run(() => tagResultsPanel.AddSearchResult(
                        text: item,
                        onDoubleClick: TagButton_DoubleClickAsync
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

