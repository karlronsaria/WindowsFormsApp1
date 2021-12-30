using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace MyForms
{
    public static class Forms
    {
        public class SearchResult : System.Windows.Forms.TextBox { }

        internal const string SAMPLE_TEXT =
            "It's all I have to bring today, " +
            "this and my heart beside, " +
            "this and my heart and all the fields, " +
            "and all the meadows wide. " +
            "Be sure you count, should I forget, " +
            "someone the sum could tell, " +
            "this and my heart and all the bees, " +
            "which in the clover dwell.";

        public delegate void InvokeHandler(Control sender);

        public static void InvokeIfHandled(Control sender, InvokeHandler myMethod, bool isHandled)
        {
            if (isHandled)
                sender.Invoke(myMethod, sender);
            else
                myMethod.Invoke(sender);
        }

        public static CancellationTokenSource
        NewCancellationSource(
                CancellationTokenSource mySource
            )
        {
            if (mySource == null)
            {
                mySource = new CancellationTokenSource();
            }
            else
            {
                mySource.Cancel();
                mySource.Dispose();
                mySource = new CancellationTokenSource();
            }

            return mySource;
        }

        public static void
        AddSearchResult(
                Control parent,
                EventHandler onDoubleClick,
                string text = SAMPLE_TEXT
            )
        {
            parent.Invoke((MethodInvoker)delegate
            {
                var btn = new SearchResult()
                {
                    Text = text,
                    AutoSize = true,
                    ReadOnly = true,
                };

                btn.Size = TextRenderer.MeasureText(btn.Text, btn.Font);
                btn.Cursor = Cursors.Arrow;
                btn.DoubleClick += onDoubleClick;
                parent.Controls.Add(btn);
            });
        }

        public static Control
        LoadListSublayout(
                Control parent,
                string labelText = SAMPLE_TEXT
            )
        {
            FlowLayoutPanel myFlowLayoutPanel = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
            };

            parent.Invoke((MethodInvoker)delegate
            {
                parent.Controls.Add(
                    new Panel()
                    {
                        Height = 10
                    }
                );

                parent.Controls.Add(
                    new Label()
                    {
                        Text = labelText,
                        Anchor = AnchorStyles.Left | AnchorStyles.Right,
                        AutoSize = true,
                    }
                );

                parent.Controls.Add(myFlowLayoutPanel);
            });

            return myFlowLayoutPanel;
        }

        public static async Task
        AddListLayoutAsync(
                Control parent,
                IEnumerable<string> list,
                EventHandler onDoubleClick,
                CancellationToken myCancellationToken,
                string labelText = SAMPLE_TEXT
            )
        {
            Control myFlowLayoutPanel = LoadListSublayout(parent, labelText);

            try
            {
                foreach (string item in list)
                {
                    myCancellationToken.ThrowIfCancellationRequested();

                    await Task.Run(() => AddSearchResult(
                        parent: myFlowLayoutPanel,
                        text: item,
                        onDoubleClick: onDoubleClick
                    ));
                }
            }
            catch (OperationCanceledException) { }
        }

        public static async Task
        SetSampleListLayoutAsync(
                Control parent,
                CancellationToken myCancellationToken,
                string sampleText = "foobar"
            )
        {
            int numPanels = 5;

            for (int i = 1; i <= numPanels; i++)
            {
                string label = $"Panel {i}:";
                string c = "";

                var someList = from b in Enumerable.Range(0, 37)
                               where (c = String.Join(
                                    "",
                                    from a in Enumerable.Range(1, b)
                                    select "A"
                                )).Count() > 0
                               select $"{sampleText} {b}: {c}";

                await AddListLayoutAsync(
                    parent: parent,
                    list: someList,
                    onDoubleClick: (s, e) => { },
                    myCancellationToken: myCancellationToken,
                    labelText: label
                );
            }
        }
    }
}

