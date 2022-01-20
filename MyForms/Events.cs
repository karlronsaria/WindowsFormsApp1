using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyForms
{
    public partial class Form1 : Form
    {
        private EventHandler _setValuesButton_onClick = delegate { };

        private void OpenDirectoryStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
                MyTreeViewPane.Load(dialog.SelectedPath);
        }

        private static Control GetActiveControl(ContainerControl parent)
        {
            if (parent.ActiveControl is ContainerControl controlBox)
                return GetActiveControl(controlBox);

            return parent.ActiveControl;
        }

        private static bool IsTextWritable(Control myControl)
        {
            if (!(myControl is TextBox textBox))
                return false;

            return !textBox.ReadOnly;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            var what = GetActiveControl(this);

            switch (e.KeyCode)
            {
                case Keys.N:
                    if (!IsTextWritable(what))
                        MainPanels[LayoutType.Select].Tags.NewItemButton.Focus();

                    break;
                case Keys.D:
                    if (!IsTextWritable(what))
                        MainPanels[LayoutType.Select].Dates.NewItemButton.Focus();

                    break;
                case Keys.OemQuestion:
                    if (e.Modifiers == Keys.Control)
                        searchBox1.Focus();

                    break;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            MyListViewPane.Load(e.Node);
        }

        private void TreeView1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                case Keys.Space:
                    if (treeView1.SelectedNode is TreeNode n)
                        MyListViewPane.Load(n);

                    // link: https://stackoverflow.com/questions/13952932/disable-beep-of-enter-and-escape-key-c-sharp
                    // retrieved: 2021_12_13
                    e.Handled = e.SuppressKeyPress = true;
                    break;
                case Keys.Up:
                    if (e.KeyData.HasFlag(Keys.Alt))
                    {
                        var parent = MyTreeViewPane.CurrentDirectory.Parent;
                        MyTreeViewPane.Load(parent.FullName);
                        MyListViewPane.Load(parent);
                    }

                    break;
            }
        }

        private async void ListView1_KeyDownAsync(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    await SetSelectedDirectoryTreeAsync();
                    break;
                case Keys.Up:
                    if (e.KeyData.HasFlag(Keys.Alt))
                    {
                        var parent = MyTreeViewPane.CurrentDirectory.Parent;
                        MyTreeViewPane.Load(parent.FullName);
                        MyListViewPane.Load(parent);
                    }

                    break;
            }
        }

        private async void ListView1_DoubleClickAsync(object sender, System.EventArgs e)
        {
            await SetSelectedDirectoryTreeAsync();
        }

        private void ListView1_SelectedIndexChanged_UsingItems(object sender, System.EventArgs e)
        {
            MyPreviewPane.ClearPreviewPane();
            var modelItem = MyListViewPane.GetLastSelectedItem();

            if (modelItem == null)
                return;

            bool isDirectory = modelItem.Attributes.HasFlag(FileAttributes.Directory);

            if (isDirectory)
                return;

            var fullName = modelItem.FullName;
            var extension = modelItem.Extension;

            switch (extension.ToUpper())
            {
                case ".PDF":
                    MyPreviewPane.SetPreviewPane(PreviewPane.NewPdfPreview(fullName));
                    break;
                default:
                    MyPreviewPane.SetPreviewPane(PreviewPane.NewPlainTextPreview(fullName));
                    break;
            }
        }

        private async void SearchBox_TextChangedAsync(object sender, EventArgs e)
        {
            SearchBoxChanged = Forms.NewCancellationSource(SearchBoxChanged);
            await ChangeResultsAsync(SearchBoxChanged.Token, LayoutType.Search);
        }

        private async void TagSearchResult_ClickAsync(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                SearchBoxChanged = Forms.NewCancellationSource(SearchBoxChanged);

                await ShowMatchingTagResults(
                    myCancellationToken: SearchBoxChanged.Token,
                    text: (sender as SearchResult)?.Text
                );
            }
        }

        private async void DocumentSearchResult_ClickAsync(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                SearchBoxChanged = Forms.NewCancellationSource(SearchBoxChanged);

                await ShowMatchingDocumentResults(
                    myCancellationToken: SearchBoxChanged.Token,
                    text: (sender as SearchResult)?.Text
                );
            }
        }

        private async void TagSearchResult_DoubleClickAsync(object sender, EventArgs e)
        {
            SearchBoxChanged = Forms.NewCancellationSource(SearchBoxChanged);

            await Task.Run(() =>
                MainPanels[LayoutType.Select].Tags.Add(
                    mySearchResult: new SearchResult()
                    {
                        Text = (sender as SearchResult).Text,
                    },
                    removeWhen: SearchResultLayout.RemoveOn.CLICK
                )
            );
        }

        private async void DocumentSearchResult_DoubleClickAsync(object sender, EventArgs e)
        {
            SearchBoxChanged = Forms.NewCancellationSource(SearchBoxChanged);

            await Task.Run(() =>
                MainPanels[LayoutType.Select]
                    .AddInOrder<SearchResultLayout>(
                        key: MasterPane.SublayoutType.Documents,
                        mySearchResult: new SearchResult()
                        {
                            Text = (sender as SearchResult).Text,
                        },
                        removeWhen: SearchResultLayout.RemoveOn.CLICK
                    )
            );
        }

        private async void TagSelectValue_ClickAsync(object sender, EventArgs e)
        {
            SearchBoxChanged = Forms.NewCancellationSource(SearchBoxChanged);
            await Task.Run(() => { });
        }

        private async void DocumentSelectValue_ClickAsync(object sender, EventArgs e)
        {
            SearchBoxChanged = Forms.NewCancellationSource(SearchBoxChanged);
            await Task.Run(() => { });
        }

        private async void TagSelectValue_DoubleClickAsync(object sender, EventArgs e)
        {
            SearchBoxChanged = Forms.NewCancellationSource(SearchBoxChanged);
            await Task.Run(() => { });
        }

        private async void DocumentSelectValue_DoubleClickAsync(object sender, EventArgs e)
        {
            SearchBoxChanged = Forms.NewCancellationSource(SearchBoxChanged);
            await Task.Run(() => { });
        }

        private void SetValuesButton1_Click(object sender, EventArgs e)
        {
            _setValuesButton_onClick(sender, e);
        }

        private void ProcessSetValuesAndClearPanel(object sender, EventArgs e)
        {
            SetValues();
            MainPanels[LayoutType.Select].Clear();
        }

        private void ProcessAddNewItemToPanel(object sender, EventArgs e)
        {
            MainPanels[LayoutType.Select].Tags.NewItemButton.Focus();
        }

        private void SelectValuePane_LayoutChanged(object sender, EventArgs e)
        {
            var selectPanel = MainPanels[LayoutType.Select];

            selectPanel.Layouts.TryGetValue(
                MasterPane.SublayoutType.Documents,
                out var documentsPanel
            );

            if (documentsPanel == null)
            {
                SetValuesButton1.Text = "New";
                _setValuesButton_onClick = ProcessAddNewItemToPanel;
                return;
            }

            if (!documentsPanel.Any())
            {
                SetValuesButton1.Text = "New";
                _setValuesButton_onClick = ProcessAddNewItemToPanel;
                return;
            }

            bool valuesAreSettable = selectPanel.Layouts.Count > 1 && documentsPanel.Count > 0;

            if (!valuesAreSettable)
            {
                SetValuesButton1.Text = "New";
                _setValuesButton_onClick = ProcessAddNewItemToPanel;
                return;
            }

            valuesAreSettable = false;

            foreach (
                var layouts in
                    from l in selectPanel.Layouts.Values
                    where l is SearchResultLayoutWithEndButton
                    select l
                )
                valuesAreSettable |= layouts.Count > 0;

            if (!valuesAreSettable)
            {
                SetValuesButton1.Text = "New";
                _setValuesButton_onClick = ProcessAddNewItemToPanel;
                return;
            }

            SetValuesButton1.Text = "Set";
            _setValuesButton_onClick = ProcessSetValuesAndClearPanel;
        }
    }
}
