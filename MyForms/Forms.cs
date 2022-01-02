using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace MyForms
{
    public static class Forms
    {
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

        public static void
        InvokeIfHandled(
                Control sender,
                InvokeHandler myMethod,
                bool isHandled
            )
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
                };

                btn.DoubleClick += onDoubleClick;
                parent.Controls.Add(btn);
            });
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
            await AddListLayoutAsync(
                parent: parent,
                searchResults:
                    from item in list
                    select (
                        new SearchResult(
                            text: item,
                            onClick: (s, e) => { },
                            onDoubleClick: onDoubleClick
                        )
                    ),
                myCancellationToken: myCancellationToken,
                labelText: labelText
            );
        }

        public static async Task
        AddListLayoutAsync(
                Control parent,
                IEnumerable<SearchResult> searchResults,
                CancellationToken myCancellationToken,
                string labelText = SAMPLE_TEXT
            )
        {
            var myFlowLayoutPanel = new SearchResultLayout(parent, labelText);

            try
            {
                foreach (var item in searchResults)
                {
                    myCancellationToken.ThrowIfCancellationRequested();
                    await Task.Run(() => myFlowLayoutPanel.Add(item));
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}

