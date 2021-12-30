using System;
using System.Windows.Forms;

namespace MyForms
{
    public class SearchResultLayout : System.Windows.Forms.FlowLayoutPanel
    {
        public const int DEFAULT_SPACING_HEIGHT = 10;
        public const FlowDirection DEFAULT_FLOW_DIRECTION = FlowDirection.LeftToRight;
        public const DockStyle DEFAULT_DOCKSTYLE = DockStyle.None;
        public const bool DEFAULT_AUTOSIZE_PREFERENCE = true;
        public const bool DEFAULT_WRAP_CONTENTS_PREFERENCE = true;

        private Panel _spacing;
        private Label _label;
        private FlowLayoutPanel _flowPanel;

        public SearchResultLayout(
                int spacingHeight = DEFAULT_SPACING_HEIGHT
            ) : base()
        {
            SpacingHeight = spacingHeight;
        }

        public FlowLayoutPanel FlowPanel
        {
            get
            {
                if (!HasInstance())
                    Build();

                return _flowPanel;
            }
        }

        public int SpacingHeight { get; }

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

        private bool HasInstance()
        {
            return _label != null;
        }

        private void AddSpacing()
        {
            _spacing = new Panel()
            {
                Height = SpacingHeight,
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
                FlowDirection = DEFAULT_FLOW_DIRECTION,
                Dock = DEFAULT_DOCKSTYLE,
                AutoSize = DEFAULT_AUTOSIZE_PREFERENCE,
                WrapContents = DEFAULT_WRAP_CONTENTS_PREFERENCE,
            };

            this.Controls.Add(_flowPanel);
        }

        private void Build(string labelText = "")
        {
            AddSpacing();
            AddLabel(labelText);
            AddFlowPanel();
        }

        public void AddSearchResult(string text, EventHandler onDoubleClick)
        {
            if (!HasInstance())
                Build();

            var btn = new SearchResult()
            {
                Text = text,
                AutoSize = true,
            };

            btn.DoubleClick += onDoubleClick;

            MyForms.Forms.InvokeIfHandled(
                _flowPanel,
                s => s.Controls.Add(btn),
                true
            );
        }
    }
}
