using System;
using System.Windows.Forms;
using System.Threading;

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

        public static object
        InvokeIfHandled(
                Control sender,
                InvokeHandler myMethod,
                bool isHandled
            )
        {
            if (isHandled)
                return sender.Invoke(myMethod, sender);

            return myMethod.DynamicInvoke(sender);
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

        public static Control GetActiveControl(ContainerControl parent)
        {
            if (parent.ActiveControl is ContainerControl controlBox)
                return GetActiveControl(controlBox);

            return parent.ActiveControl;
        }

        public static bool IsTextWritable(Control myControl)
        {
            if (!(myControl is TextBox textBox))
                return false;

            return !textBox.ReadOnly;
        }
    }
}

