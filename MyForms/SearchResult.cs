using System;
using System.Windows.Forms;

namespace MyForms
{
    public class SearchResult : TextBox
    {
        public const int TOOL_TIP_AUTOMATIC_DELAY = 500;

        private string _toolTipText;
        private EventHandler _onMouseHover;

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

        public string ToolTip
        {
            get
            {
                return _toolTipText;
            }

            set
            {
                if (_onMouseHover != null)
                {
                    this.MouseHover -= _onMouseHover;
                    _onMouseHover = null;
                }

                if (String.IsNullOrEmpty(value))
                {
                    _toolTipText = null;
                    return;
                }

                _toolTipText = value;

                _onMouseHover = (sender, e) =>
                {
                    new ToolTip
                    {
                        AutomaticDelay = TOOL_TIP_AUTOMATIC_DELAY,
                    }
                    .SetToolTip(sender as Control, _toolTipText);
                };

                this.MouseHover += _onMouseHover;
            }
        }
    }
}

