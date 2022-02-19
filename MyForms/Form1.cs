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
    [System.ComponentModel.DesignerCategory("Form")]
    public partial class Form1 : Form
    {
        public enum LayoutType : int
        {
            Search,
            Select,
        }

        public string Directory { get; set; }

        public string MostRecentJsonFile { get; set; }

        public string StatusLine
        {
            get => statusBar1.Text;

            set
            {
                var myMethod = new Func<TextBox, bool>(c =>
                {
                    c.Text = value;
                    return true;
                });

                MyForms.Forms.InvokeIfHandled(
                    statusBar1,
                    s => myMethod.Invoke(s as TextBox),
                    IsHandleCreated
                );
            }
        }

        internal class LayoutDictionary :
            Dictionary<LayoutType, MasterPane> { }

        private readonly IDataConnector _database;
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

        public Form1(IDataConnector myDatabase, string startingDirectory)
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
                foreach (var item in _database.GetNamesMatchingDate(
                        date: text,
                        format: Formats.DATE_FORMAT,
                        pattern: Formats.DATE_PATTERN_NONCAPTURE
                    ))
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

            var label = MyForms.Layout.NewLabel;
            label.Text = $"Document: {text}";

            MainPanels[LayoutType.Search].Controls.Add(MyForms.Layout.NewSpacing);
            MainPanels[LayoutType.Search].Controls.Add(label);
            MainPanels[LayoutType.Search].Controls.Add(MyForms.Layout.NewSpacing);

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

                foreach (var item in _database.GetDatesMatchingName(
                        name: text, format: Formats.DATE_FORMAT
                    ))
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
                LayoutType mainPanelKey,
                string text,
                HashSet<Searches.Mode> modes
            )
        {
            MainPanels[mainPanelKey].Clear();

            if (!modes.Contains(Searches.Mode.All) && text.Length == 0)
                return;

            GetCollection collectionHandler;

            try
            {
                if (modes.Contains(Searches.Mode.Document))
                {
                    if (modes.Contains(Searches.Mode.All))
                        collectionHandler = str => _database.GetNames();
                    else if (modes.Contains(Searches.Mode.Regex))
                        collectionHandler = str => _database.GetNamesMatchingPattern(str);
                    else
                        collectionHandler = str => _database.GetNamesMatchingSubstring(
                            substring: str,
                            exact: modes.Contains(Searches.Mode.Exact)
                        );

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

                if (modes.Contains(Searches.Mode.Tag))
                {
                    if (modes.Contains(Searches.Mode.All))
                        collectionHandler = str => _database.GetTags();
                    else if (modes.Contains(Searches.Mode.Regex))
                        collectionHandler = str => _database.GetTagsMatchingPattern(str);
                    else
                        collectionHandler = str => _database.GetTagsMatchingSubstring(
                            substring: str,
                            exact: modes.Contains(Searches.Mode.Exact)
                        );

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

                if (modes.Contains(Searches.Mode.Date))
                {
                    if (modes.Contains(Searches.Mode.All))
                        foreach (string item in _database.GetDates())
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
                    else
                        foreach (string item in _database.GetNamesMatchingDate(
                                date: text,
                                format: Formats.DATE_FORMAT,
                                pattern: Formats.DATE_PATTERN_NONCAPTURE
                            ))
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
                _database.AddTags(documents, tags);

            var dates = mainPanel.GetValues(MasterPane.SublayoutType.Dates);

            if (dates != null)
                _database.AddDates(documents, dates, Formats.DATE_FORMAT);
        }
    }
}
