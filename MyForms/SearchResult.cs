using System;
using System.Windows.Forms;

namespace MyForms
{
    public class SearchResult : System.Windows.Forms.TextBox
    {
        public SearchResult()
        {
            base.ReadOnly = true;
            AutoSize = true;
            Cursor = Cursors.Arrow;
        }

        public SearchResult(
                string text,
                EventHandler onClick,
                EventHandler onDoubleClick
            )
        {
            Text = text;
            Click += onClick;
            DoubleClick += onDoubleClick;
        }

        public new bool ReadOnly { get => base.ReadOnly; }

        public new string Name { get => base.Name; }

        public new string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = value;
                base.Name = value;
                Size = TextRenderer.MeasureText(base.Text, Font);
            }
        }
    }
}

