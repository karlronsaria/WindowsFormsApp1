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

        private void ImportToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                InitialDirectory = Directory,
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    _database.FromJson(dialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        caption: ex.GetType().Name,
                        text: ex.Message
                    );
                }
            }
        }

        private void ExportToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Application.File.NewDatedFilePath(
                filePath: MostRecentJsonFile,
                out _,
                out string newPath
            );

            var dialogResult = MessageBox.Show(
                text: newPath,
                caption: "Writing to file",
                buttons: MessageBoxButtons.OKCancel
            );

            if (dialogResult == DialogResult.OK)
                _database.ToJson(newPath);
        }

        private void ExportAsToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                InitialDirectory = Directory,
            };

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _database.ToJson(dialog.FileName);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            var what = Forms.GetActiveControl(this);

            switch (e.KeyCode)
            {
                case Keys.N:
                    if (!Forms.IsTextWritable(what))
                        MainPanels[LayoutType.Select].Tags.NewItemButton.Focus();

                    break;
                case Keys.D:
                    if (!Forms.IsTextWritable(what))
                        MainPanels[LayoutType.Select].Dates.NewItemButton.Focus();

                    break;
                case Keys.S:
                    if (!Forms.IsTextWritable(what) && SetValuesButton1.Text == "Set")
                        ProcessSetValuesAndClearPanel(this, new EventArgs());

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

            try
            {
                await ChangeResultsAsync(SearchBoxChanged.Token, LayoutType.Search);
            }
            catch (ArgumentException) { }
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.searchPanel1.Focus();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
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

        private async void DateSearchResult_ClickAsync(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                SearchBoxChanged = Forms.NewCancellationSource(SearchBoxChanged);

                await ShowMatchingDateResults(
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

            var myMethod = new Func<Form1, bool>(s =>
            {
                s.MainPanels[LayoutType.Select]
                    .AddInOrder<SearchResultLayoutWithEndButton>(
                        key: MasterPane.SublayoutType.Tags
                    );

                s.MainPanels[LayoutType.Select]
                    .Layouts[MasterPane.SublayoutType.Tags]
                    .Add(
                        mySearchResult: new SearchResult()
                        {
                            Text = (sender as SearchResult).Text,
                        },
                        removeWhen: ILayout.RemoveOn.CLICK
                    );

                s.SelectValuePane_LayoutChanged(this, new EventArgs());
                return true;
            });

            await Task.Run(() =>
            {
                _ = (bool?)MyForms.Forms.InvokeIfHandled(
                    this,
                    s => myMethod.Invoke(s as Form1),
                    IsHandleCreated
                );
            });
        }

        private async void DateSearchResult_DoubleClickAsync(object sender, EventArgs e)
        {
            SearchBoxChanged = Forms.NewCancellationSource(SearchBoxChanged);

            var myMethod = new Func<Form1, bool>(s =>
            {
                s.MainPanels[LayoutType.Select]
                    .AddInOrder<DateLayoutWithEndButton>(
                        key: MasterPane.SublayoutType.Dates
                    );

                s.MainPanels[LayoutType.Select]
                    .Layouts[MasterPane.SublayoutType.Dates]
                    .Add(
                        mySearchResult: new SearchResult()
                        {
                            Text = (sender as SearchResult).Text,
                        },
                        removeWhen: ILayout.RemoveOn.CLICK
                    );

                s.SelectValuePane_LayoutChanged(this, new EventArgs());
                return true;
            });

            await Task.Run(() =>
            {
                _ = (bool?)MyForms.Forms.InvokeIfHandled(
                    this,
                    s => myMethod.Invoke(s as Form1),
                    IsHandleCreated
                );
            });
        }

        private async void DocumentSearchResult_DoubleClickAsync(object sender, EventArgs e)
        {
            SearchBoxChanged = Forms.NewCancellationSource(SearchBoxChanged);

            var myMethod = new Func<Form1, bool>(s =>
            {
                s.MainPanels[LayoutType.Select]
                    .AddInOrder<SearchResultLayout>(
                        key: MasterPane.SublayoutType.Documents
                    );

                s.MainPanels[LayoutType.Select]
                    .Layouts[MasterPane.SublayoutType.Documents]
                    .Add(
                        mySearchResult: new SearchResult()
                        {
                            Text = (sender as SearchResult).Text,
                        },
                        removeWhen: ILayout.RemoveOn.CLICK
                    );

                s.SelectValuePane_LayoutChanged(this, new EventArgs());
                return true;
            });

            await Task.Run(() =>
            {
                _ = (bool?)MyForms.Forms.InvokeIfHandled(
                    this,
                    s => myMethod.Invoke(s as Form1),
                    IsHandleCreated
                );
            });
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
            // link: https://stackoverflow.com/questions/3845695/is-there-a-builtin-confirmation-dialog-in-windows-forms
            // retrieved: 2022_01_20

            var confirmResult = MessageBox.Show(
                text: MainPanels[LayoutType.Select].ToString(),
                caption: "Confirm setting values",
                buttons: MessageBoxButtons.YesNo
            );

            if (confirmResult == DialogResult.Yes)
            {
                SetValues();
                MainPanels[LayoutType.Select].Clear();
                return;
            }

            return;
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
                var layout in
                    from l in selectPanel.Layouts.Values
                    where l is SearchResultLayoutWithEndButton
                    select l
                )
                valuesAreSettable |= layout.Count > 0;

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
