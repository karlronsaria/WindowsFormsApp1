using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace MyForms
{
    public partial class Form1 : Form
    {
        public enum LayoutType : int
        {
            Search,
            Select,
        }

        public enum SublayoutType : int
        {
            Documents,
            Tags,
            Dates,
        }

        internal class LayoutDictionary :
            Dictionary<LayoutType, Control> { }

        internal class SublayoutDictionary :
            Dictionary<LayoutType, Dictionary<SublayoutType, SearchResultLayout>> { }

        private readonly IDataContext _database;
        private readonly PreviewPane _myPreviewPane;
        private readonly TreeViewPane _myTreeViewPane;
        private readonly ListViewPane _myListViewPane;
        private readonly LayoutDictionary _mainLayouts;
        private readonly SublayoutDictionary _sublayouts;
        private CancellationTokenSource _searchBoxChanged;

        public Form1(IDataContext myDatabase, string startingDirectory)
        {
            InitializeComponent();

            _database = myDatabase;
            _myPreviewPane = new PreviewPane(splitContainer2.Panel2);
            _myTreeViewPane = new TreeViewPane(treeView1, startingDirectory);
            _myListViewPane = new ListViewPane(listView1, startingDirectory);
            _mainLayouts = new LayoutDictionary();
            _sublayouts = new SublayoutDictionary();

            _mainLayouts[LayoutType.Search] = searchResultLayoutPanel1;
            _mainLayouts[LayoutType.Select] = selectValueLayoutPanel1;

            _sublayouts[LayoutType.Search] = new Dictionary<SublayoutType, SearchResultLayout>();
            _sublayouts[LayoutType.Select] = new Dictionary<SublayoutType, SearchResultLayout>();
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

        internal SublayoutDictionary
        Subpanels
        {
            get => _sublayouts;
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
            MainPanels[LayoutType.Search].Controls.Clear();

            var myFlowLayoutPanel = new SearchResultLayout(
                parent: MainPanels[LayoutType.Search],
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
                LayoutType mainPanelKey,
                SublayoutType subpanelKey,
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
        AddSelectValueButton(
                CancellationToken myCancellationToken,
                SublayoutType subpanelKey,
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
                mainPanelKey: LayoutType.Select,
                subpanelKey: subpanelKey,
                labelText: labelText
            );
        }

        private async Task
        ChangeResultsAsync(
                CancellationToken myCancellationToken,
                LayoutType mainPanelKey
            )
        {
            MainPanels[mainPanelKey].Controls.Clear();
            string text = searchBox1.Text;

            if (text.Length == 0)
                return;

            Subpanels[mainPanelKey].Remove(SublayoutType.Documents);
            Subpanels[mainPanelKey].Remove(SublayoutType.Tags);

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
                        subpanelKey: SublayoutType.Documents,
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
                        subpanelKey: SublayoutType.Tags,
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

