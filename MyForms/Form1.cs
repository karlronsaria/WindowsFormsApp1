using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace MyForms
{
    public partial class Form1 : Form
    {
        private readonly IDataContext _database;
        private readonly PreviewPane _myPreviewPane;
        private readonly TreeViewPane _myTreeViewPane;
        private readonly ListViewPane _myListViewPane;
        private readonly Dictionary<string, Control> _mainPanels;
        private readonly Dictionary<string, Dictionary<string, SearchResultLayout>> _subpanels;
        private CancellationTokenSource _searchBoxChanged;

        public Form1(IDataContext myDatabase, string startingDirectory)
        {
            InitializeComponent();

            _database = myDatabase;
            _myPreviewPane = new PreviewPane(splitContainer2.Panel2);
            _myTreeViewPane = new TreeViewPane(treeView1, startingDirectory);
            _myListViewPane = new ListViewPane(listView1, startingDirectory);
            _mainPanels = new Dictionary<string, Control>();
            _subpanels = new Dictionary<string, Dictionary<string, SearchResultLayout>>();

            _mainPanels["Search"] = searchResultLayoutPanel1;
            _mainPanels["Set"] = setValueLayoutPanel1;

            _subpanels["Search"] = new Dictionary<string, SearchResultLayout>();
            _subpanels["Set"] = new Dictionary<string, SearchResultLayout>();
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

        public Dictionary<string, Control>
        MainPanels
        {
            get => _mainPanels;
        }

        public Dictionary<string, Dictionary<string, SearchResultLayout>>
        Subpanels
        {
            get => _subpanels;
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
        ShowMatchingTagResults(
                CancellationToken myCancellationToken,
                string text
            )
        {
            MainPanels["Search"].Controls.Clear();

            var myFlowLayoutPanel = new SearchResultLayout(
                parent: MainPanels["Search"],
                labelText: $"Documents with the tag \"{text}\":"
            );

            try
            {
                foreach (var item in _database.GetNamesMatchingTag(text))
                {
                    myCancellationToken.ThrowIfCancellationRequested();

                    var mySearchResult = new SearchResult()
                    {
                        Text = item,
                    };

                    mySearchResult.Click += DocumentSearchResult_ClickAsync;
                    mySearchResult.DoubleClick += DocumentSearchResult_DoubleClickAsync;

                    await Task.Run(() => myFlowLayoutPanel.Add(mySearchResult));
                }
            }
            catch (OperationCanceledException) { }
        }

        private async Task
        ShowMatchingDocumentResults(
                CancellationToken myCancellationToken,
                string text
            )
        {
            MainPanels["Search"].Controls.Clear();

            var myFlowLayoutPanel = new SearchResultLayout(
                parent: MainPanels["Search"],
                labelText: $"Tags for the document \"{text}\":"
            );

            try
            {
                foreach (var item in _database.GetTagsMatchingName(text))
                {
                    myCancellationToken.ThrowIfCancellationRequested();

                    var mySearchResult = new SearchResult()
                    {
                        Text = item,
                    };

                    mySearchResult.Click += TagSearchResult_ClickAsync;
                    mySearchResult.DoubleClick += TagSearchResult_DoubleClickAsync;

                    await Task.Run(() => myFlowLayoutPanel.Add(mySearchResult));
                }
            }
            catch (OperationCanceledException) { }
        }

        private async Task
        AddSearchResult(
                SearchResult mySearchResult,
                string mainPanelKey,
                string subpanelKey,
                string labelText
            )
        {
            if (!Subpanels[mainPanelKey].ContainsKey(subpanelKey))
                Subpanels[mainPanelKey][subpanelKey] = new SearchResultLayout(
                    parent: MainPanels[mainPanelKey],
                    labelText: labelText
                );

            await Task.Run(() => Subpanels[mainPanelKey][subpanelKey].Add(mySearchResult));
        }

        private async Task
        AddSetValueButton(
                CancellationToken myCancellationToken,
                string subpanelKey,
                string labelText,
                string buttonText
            )
        {
            var mySearchResult = new SearchResult()
            {
                Text = buttonText,
            };

            await AddSearchResult(
                mySearchResult: mySearchResult,
                mainPanelKey: "Set",
                subpanelKey: subpanelKey,
                labelText: labelText
            );
        }

        private async Task
        ChangeResultsAsync(
                CancellationToken myCancellationToken,
                string mainPanelKey
            )
        {
            MainPanels[mainPanelKey].Controls.Clear();
            string text = searchBox1.Text;

            if (text.Length == 0)
                return;

            Subpanels[mainPanelKey].Remove("Documents");
            Subpanels[mainPanelKey].Remove("Tags");

            try
            {
                foreach (string item in _database.GetNamesMatchingSubstring(text))
                {
                    myCancellationToken.ThrowIfCancellationRequested();

                    var mySearchResult = new SearchResult()
                    {
                        Text = item,
                    };

                    mySearchResult.Click += DocumentSearchResult_ClickAsync;
                    mySearchResult.DoubleClick += DocumentSearchResult_DoubleClickAsync;

                    await AddSearchResult(
                        mySearchResult: mySearchResult,
                        mainPanelKey: mainPanelKey,
                        subpanelKey: "Documents",
                        labelText: "Documents:"
                    );
                }

                foreach (string item in _database.GetTagsMatchingSubstring(text))
                {
                    myCancellationToken.ThrowIfCancellationRequested();

                    var mySearchResult = new SearchResult()
                    {
                        Text = item,
                    };

                    mySearchResult.Click += TagSearchResult_ClickAsync;
                    mySearchResult.DoubleClick += TagSearchResult_DoubleClickAsync;

                    await AddSearchResult(
                        mySearchResult: mySearchResult,
                        mainPanelKey: mainPanelKey,
                        subpanelKey: "Tags",
                        labelText: "Tags:"
                    );
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

