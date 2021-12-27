using System;
using System.Windows.Forms;

namespace MyForms
{
    internal class SearchResultLayout : System.Windows.Forms.FlowLayoutPanel
    {
        public const int DEFAULT_SPACING_HEIGHT = 10;
        public const FlowDirection DEFAULT_FLOW_DIRECTION = FlowDirection.LeftToRight;
        public const DockStyle DEFAULT_DOCKSTYLE = DockStyle.None;
        public const bool DEFAULT_AUTOSIZE_PREFERENCE = true;
        public const bool DEFAULT_WRAP_CONTENTS_PREFERENCE = true;

        private Control _spacing;
        private Control _label;
        private Control _flowPanel;

        private int _spacingHeight;
        private FlowDirection _flowDirection;
        private DockStyle _dockStyle;
        private bool _autosize;
        private bool _wrapContents;

        public SearchResultLayout(
                int spacingHeight = DEFAULT_SPACING_HEIGHT,
                FlowDirection flowDirection = DEFAULT_FLOW_DIRECTION,
                DockStyle dockStyle = DEFAULT_DOCKSTYLE,
                bool autosize = DEFAULT_AUTOSIZE_PREFERENCE,
                bool wrapContents = DEFAULT_WRAP_CONTENTS_PREFERENCE
            ) : base()
        {
            _spacingHeight = spacingHeight;
            _flowDirection = flowDirection;
            _dockStyle = dockStyle;
            _autosize = autosize;
            _wrapContents = wrapContents;
        }

        public int SpacingHeight
        {
            get { return _spacingHeight; }
        }

        private bool HasInstance()
        {
            return _label != null;
        }

        private void AddSpacing()
        {
            _spacing = new Panel()
            {
                Height = _spacingHeight,
            };

            this.Controls.Add(_spacing);
        }

        private void AddLabel(string text)
        {
            _label = new Label()
            {
                Text = text,
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                AutoSize = true,
            };

            this.Controls.Add(_label);
        }

        private void AddFlowPanel()
        {
            _flowPanel = new FlowLayoutPanel()
            {
                FlowDirection = _flowDirection,
                Dock = _dockStyle,
                AutoSize = _autosize,
                WrapContents = _wrapContents,
            };

            this.Controls.Add(_flowPanel);
        }

        private void Build(string labelText = "")
        {
            AddSpacing();
            AddLabel(labelText);
            AddFlowPanel();
        }

        public string LabelText
        {
            get
            {
                if (!HasInstance())
                {
                    Build();
                    return "";
                }

                return _label.Text;
            }

            set
            {
                if (!HasInstance())
                    Build(value);

                _label.Text = value;
            }
        }

        public void AddButton(string text, EventHandler buttonClick)
        {
            if (!HasInstance())
                Build();

            Button btn = new Button()
            {
                Text = text,
                AutoSize = true,
            };

            btn.Click += buttonClick;
            _flowPanel.Controls.Add(btn);
        }
    }
}
