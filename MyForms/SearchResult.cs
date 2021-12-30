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

        public bool ReadOnly { get => base.ReadOnly; }

        public string Text
        {
            get
            {
                return base.Text;
            }

            set
            {
                base.Text = value;
                Size = TextRenderer.MeasureText(base.Text, Font);
            }
        }
    }
}

