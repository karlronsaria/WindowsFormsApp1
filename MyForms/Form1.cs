using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using System.Linq;

namespace MyForms
{
    public partial class Form1 : Form
    {
        public enum LayoutType : int
        {
            Search,
            Select,
        }

        public string Directory { get; set; }
        public string MostRecentJsonFile { get; set; }

        internal class LayoutDictionary :
            Dictionary<LayoutType, MasterPane> { }

        private readonly IDataContext _database;
        private readonly PreviewPane _myPreviewPane;
        private readonly TreeViewPane _myTreeViewPane;
        private readonly ListViewPane _myListViewPane;
        private readonly LayoutDictionary _mainLayouts;
        private CancellationTokenSource _searchBoxChanged;

        private void InitializeMyComponent()
        {
            this.searchBox1.TextChanged
                += new System.EventHandler(this.SearchBox_TextChangedAsync);

            this.selectValueLayoutPanel1.LayoutChanged
                += new System.EventHandler(this.SelectValuePane_LayoutChanged);
        }

        public Form1(IDataContext myDatabase, string startingDirectory)
        {
            InitializeComponent();
            InitializeMyComponent();

            Directory = startingDirectory;
            _database = myDatabase;
            _myPreviewPane = new PreviewPane(splitContainer2.Panel2);
            _myTreeViewPane = new TreeViewPane(treeView1, startingDirectory);
            _myListViewPane = new ListViewPane(listView1, startingDirectory);
            _mainLayouts = new LayoutDictionary
            {
                [LayoutType.Search] = searchResultLayoutPanel1,
                [LayoutType.Select] = selectValueLayoutPanel1
            };

            MainPanels[LayoutType.Select].Clear();
        }

        internal PreviewPane MyPreviewPane
        {
            get => _myPreviewPane;
        }

        internal TreeViewPane MyTreeViewPane
        {
            get => _myTreeViewPane;
        }

        internal ListViewPane MyListViewPane
        {
            get => _myListViewPane;
        }

        internal LayoutDictionary
        MainPanels
        {
            get => _mainLayouts;
        }

        internal CancellationTokenSource SearchBoxChanged
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
            MainPanels[LayoutType.Search].Controls.Clear();

            var myFlowLayoutPanel = new SearchResultLayout(
                parent: MainPanels[LayoutType.Search],
                labelText: $"Documents with the tag \"{text}\":"
            );

            try
            {
                foreach (var item in _database.GetNamesMatchingTag(text))
                {
                    myCancellationToken.ThrowIfCancellationRequested();
                    var mySearchResult = new SearchResult() { Text = item };
                    mySearchResult.Click += DocumentSearchResult_ClickAsync;
                    mySearchResult.DoubleClick += DocumentSearchResult_DoubleClickAsync;
                    await Task.Run(() => myFlowLayoutPanel.Add(mySearchResult));
                }
            }
            catch (OperationCanceledException) { }
        }

        private async Task
        ShowMatchingDateResults(
                CancellationToken myCancellationToken,
                string text
            )
        {
            MainPanels[LayoutType.Search].Controls.Clear();

            var myFlowLayoutPanel = new SearchResultLayout(
                parent: MainPanels[LayoutType.Search],
                labelText: $"Documents with the date \"{text}\":"
            );

            try
            {
                foreach (var item in _database.GetNamesMatchingDate(text))
                {
                    myCancellationToken.ThrowIfCancellationRequested();
                    var mySearchResult = new SearchResult() { Text = item };
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
            MainPanels[LayoutType.Search].Clear();

            var label = ILayout.NewLabel;
            label.Text = $"Document: {text}";

            MainPanels[LayoutType.Search].Controls.Add(label);
            MainPanels[LayoutType.Search].Controls.Add(ILayout.NewSpacing);

            try
            {
                foreach (var item in _database.GetTagsMatchingName(text))
                {
                    myCancellationToken.ThrowIfCancellationRequested();
                    var mySearchResult = new SearchResult() { Text = item };
                    mySearchResult.Click += TagSearchResult_ClickAsync;
                    mySearchResult.DoubleClick += TagSearchResult_DoubleClickAsync;

                    await Task.Run(() =>
                        MainPanels[LayoutType.Search]
                            .AddInOrder<SearchResultLayout>(
                                key: MasterPane.SublayoutType.Tags,
                                mySearchResult: mySearchResult
                            )
                    );
                }

                foreach (var item in _database.GetDatesMatchingName(text))
                {
                    myCancellationToken.ThrowIfCancellationRequested();
                    var mySearchResult = new SearchResult() { Text = item };
                    mySearchResult.Click += DateSearchResult_ClickAsync;
                    mySearchResult.DoubleClick += DateSearchResult_DoubleClickAsync;

                    await Task.Run(() =>
                        MainPanels[LayoutType.Search]
                            .AddInOrder<SearchResultLayout>(
                                key: MasterPane.SublayoutType.Dates,
                                mySearchResult: mySearchResult
                            )
                    );
                }
            }
            catch (OperationCanceledException) { }
        }

        public delegate System.Collections.Generic.IEnumerable<string>
        GetCollection(string str);

        private async Task
        ChangeResultsAsync(
                CancellationToken myCancellationToken,
                LayoutType mainPanelKey
            )
        {
            MainPanels[mainPanelKey].Clear();
            string text = searchBox1.Text;

            if (text.Length == 0)
            {
                statusBar1.Text = "";
                return;
            }

            Match modesCapture;
            bool hasModes;
            bool regex = false;
            bool exact = false;
            bool document = false;
            bool tag = false;
            bool date = false;

            modesCapture = Regex.Match(
                input: text,
                pattern: @"^\s*(?<modes>\w+(\s*,\s*\w+)*)\s*\:\s*(?<searchstr>.*)$"
            );

            hasModes = modesCapture.Success;

            if (hasModes)
            {
                statusBar1.Text = "Modes:";

                var modes = from mode in modesCapture.Groups["modes"].Value.Split(',')
                            select mode.Trim().ToLowerInvariant();

                text = modesCapture.Groups["searchstr"].Value.Trim();
                var actualModes = new HashSet<string>();

                foreach (var mode in modes)
                {
                    switch (mode)
                    {
                        case "r":
                        case "re":
                        case "regex":
                            regex = true;
                            actualModes.Add("regex");
                            break;
                        case "e":
                        case "ex":
                        case "exact":
                            exact = true;
                            actualModes.Add("exact");
                            break;
                        case "doc":
                        case "document":
                        case "documents":
                            document = true;
                            actualModes.Add("document");
                            break;
                        case "tag":
                        case "tags":
                            tag = true;
                            actualModes.Add("tag");
                            break;
                        case "date":
                        case "dates":
                            date = true;
                            actualModes.Add("date");
                            break;
                    }
                }

                foreach (var mode in actualModes)
                    statusBar1.AppendText($" <{mode}>");
            }

            if (!document && !tag && !date)
            {
                document = true;
                tag = true;
            }

            if (!exact)
            {
                Match exactCapture = Regex.Match(text, "(?<=^\\s*\")(?<exacttext>.*)(?=\"\\s*$)");

                if (exactCapture.Success)
                {
                    exact = true;
                    text = exactCapture.Groups["exacttext"].Value;

                    if (!hasModes)
                        statusBar1.Text = "Modes:";

                    statusBar1.AppendText(" <exact>");
                }
            }

            GetCollection collectionHandler;

            try
            {
                if (document)
                {
                    if (regex)
                        collectionHandler = str => _database.GetNamesMatchingPattern(str);
                    else
                        collectionHandler = str => _database.GetNamesMatchingSubstring(str, exact);

                    foreach (string item in collectionHandler(text))
                    {
                        myCancellationToken.ThrowIfCancellationRequested();
                        var mySearchResult = new SearchResult() { Text = item };
                        mySearchResult.Click += DocumentSearchResult_ClickAsync;
                        mySearchResult.DoubleClick += DocumentSearchResult_DoubleClickAsync;

                        await Task.Run(() =>
                            MainPanels[LayoutType.Search]
                                .AddInOrder<SearchResultLayout>(
                                    key: MasterPane.SublayoutType.Documents,
                                    mySearchResult: mySearchResult
                                )
                        );
                    }
                }

                if (tag)
                {
                    if (regex)
                        collectionHandler = str => _database.GetTagsMatchingPattern(str);
                    else
                        collectionHandler = str => _database.GetTagsMatchingSubstring(str, exact);

                    foreach (string item in collectionHandler(text))
                    {
                        myCancellationToken.ThrowIfCancellationRequested();
                        var mySearchResult = new SearchResult() { Text = item };
                        mySearchResult.Click += TagSearchResult_ClickAsync;
                        mySearchResult.DoubleClick += TagSearchResult_DoubleClickAsync;

                        await Task.Run(() =>
                            MainPanels[LayoutType.Search]
                                .AddInOrder<SearchResultLayout>(
                                    key: MasterPane.SublayoutType.Tags,
                                    mySearchResult: mySearchResult
                                )
                        );
                    }
                }

                if (date)
                {
                    foreach (string item in _database.GetNamesMatchingDate(text))
                    {
                        myCancellationToken.ThrowIfCancellationRequested();
                        var mySearchResult = new SearchResult() { Text = item };
                        mySearchResult.Click += DocumentSearchResult_ClickAsync;
                        mySearchResult.DoubleClick += DocumentSearchResult_DoubleClickAsync;

                        await Task.Run(() =>
                            MainPanels[LayoutType.Search]
                                .AddInOrder<SearchResultLayout>(
                                    key: MasterPane.SublayoutType.Documents,
                                    mySearchResult: mySearchResult
                                )
                        );
                    }
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

        private void
        SetValues()
        {
            var mainPanel = MainPanels[LayoutType.Select];
            var documents = mainPanel.GetValues(MasterPane.SublayoutType.Documents);

            if (documents == null)
                return;

            var tags = mainPanel.GetValues(MasterPane.SublayoutType.Tags);

            if (tags != null)
                _database.SetTags(documents, tags);

            var dates = mainPanel.GetValues(MasterPane.SublayoutType.Dates);

            if (dates != null)
                _database.SetDates(documents, dates, Application.DateText.DATE_FORMAT);
        }
    }
}

