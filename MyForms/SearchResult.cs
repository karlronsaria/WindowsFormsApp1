using System;
using System.Windows.Forms;

namespace MyForms
{
    public class SearchResult : LayoutItem
    {
        public const int TOOL_TIP_AUTOMATIC_DELAY = 500;

        private string _toolTipText;
        private EventHandler _onMouseHover;

        public SearchResult(): base()
        {
            AutoSize = true;
            Cursor = Cursors.Arrow;
        }

        public SearchResult(
                string text,
                EventHandler onClick,
                EventHandler onDoubleClick
            ): base()
        {
            Text = text;
            Click += onClick;
            DoubleClick += onDoubleClick;
        }

        public override string ToolTip
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

