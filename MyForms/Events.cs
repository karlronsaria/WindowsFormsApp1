using System;
using System.Windows.Forms;
using System.IO;

namespace MyForms
{
    public partial class Form1 : Form
    {
        private void OpenDirectoryStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
                MyTreeViewPane.Load(dialog.SelectedPath);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.OemQuestion:
                    if (e.Modifiers == Keys.Control)
                        searchBox1.Focus();

                    break;
            }
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

        private async void ListView1_KeyDown(object sender, KeyEventArgs e)
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

        private async void ListView1_DoubleClick(object sender, System.EventArgs e)
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
            SearchResultsPanel.Controls.Clear();
            await ChangeSearchResultsAsync(SearchBoxChanged.Token);
        }

        private async void TagButton_DoubleClickAsync(object sender, EventArgs e)
        {
            SearchBoxChanged = Forms.NewCancellationSource(SearchBoxChanged);
            SearchResultsPanel.Controls.Clear();
            string text = (sender as Forms.SearchResult)?.Text;

            await Forms.AddListLayoutAsync(
                parent: SearchResultsPanel,
                list: _database.GetNamesMatchingTag(text),
                onDoubleClick: DocumentButton_DoubleClickAsync,
                myCancellationToken: SearchBoxChanged.Token,
                labelText: $"Documents with the tag \"{text}\":"
            );
        }

        private async void DocumentButton_DoubleClickAsync(object sender, EventArgs e)
        {
            SearchBoxChanged = Forms.NewCancellationSource(SearchBoxChanged);
            SearchResultsPanel.Controls.Clear();
            string text = (sender as Forms.SearchResult)?.Text;

            await Forms.AddListLayoutAsync(
                parent: SearchResultsPanel,
                list: _database.GetTagsMatchingName(text),
                onDoubleClick: TagButton_DoubleClickAsync,
                myCancellationToken: SearchBoxChanged.Token,
                labelText: $"Tags for the document \"{text}\":"
            );
        }
    }
}
