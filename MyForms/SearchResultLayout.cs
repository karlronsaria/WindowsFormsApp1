﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MyForms
{
    public class SearchResultLayoutWithEndButton : SearchResultLayout
    {
        public const string NEW_ITEM_TEXT = " + ";

        public SearchResultLayoutWithEndButton() : base() { }

        public SearchResultLayoutWithEndButton(
                int spacingHeight
            ) : base(spacingHeight) { }

        public SearchResultLayoutWithEndButton(
                Control parent,
                string labelText,
                int spacingHeight = DEFAULT_SPACING_HEIGHT
            ) : base(parent, labelText, spacingHeight) { }

        protected SearchResult NewItemButton { get; } = new SearchResult()
        {
            Text = NEW_ITEM_TEXT,
            BackColor = System.Drawing.Color.DimGray,
        };

        protected override int LastItemIndex
        {
            get
            {
                if (!FlowPanel.Controls.Contains(NewItemButton))
                    return -1;

                return FlowPanel.Controls.GetChildIndex(NewItemButton) - 1;
            }
        }

        protected override void AddFlowPanel()
        {
            base.AddFlowPanel();
            Add(NewItemButton);
        }

        protected override void AddToSubcontrols(Control parent, Control child)
        {
            parent.Controls.Remove(NewItemButton);
            parent.Controls.Add(child);
            parent.Controls.Add(NewItemButton);
        }
    }

    public class SearchResultLayout : System.Windows.Forms.FlowLayoutPanel
    {
        public const int DEFAULT_SPACING_HEIGHT = 10;
        public const FlowDirection DEFAULT_FLOW_DIRECTION = FlowDirection.LeftToRight;
        public const DockStyle DEFAULT_DOCKSTYLE = DockStyle.None;
        public const bool DEFAULT_AUTOSIZE_PREFERENCE = true;
        public const bool DEFAULT_WRAP_CONTENTS_PREFERENCE = true;

        private readonly Control _parent = null;
        private Panel _spacing;
        private Label _label;
        private FlowLayoutPanel _flowPanel;

        private void Initialize()
        {
            FlowDirection = FlowDirection.TopDown;
            WrapContents = false;
            AutoSize = true;
        }

        public SearchResultLayout()
        {
            Initialize();
            SpacingHeight = DEFAULT_SPACING_HEIGHT;
        }

        public SearchResultLayout(
                int spacingHeight
            ) : base()
        {
            Initialize();
            SpacingHeight = spacingHeight;
        }

        public SearchResultLayout(
                Control parent,
                string labelText,
                int spacingHeight = DEFAULT_SPACING_HEIGHT
            ) : base()
        {
            Initialize();
            SpacingHeight = spacingHeight;
            LabelText = labelText;
            Parent = parent;
        }

        public new Control Parent
        {
            get
            {
                return _parent;
            }

            set
            {
                if (_parent == null)
                    value.Invoke((MethodInvoker)delegate
                    {
                        value.Controls.Add(this);
                    });
            }
        }

        private new ControlCollection Controls { get => base.Controls; }

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

        protected virtual int LastItemIndex { get => FlowPanel.Controls.Count - 1; }

        public IEnumerable<string> Values
        {
            get
            {
                var list = new List<string>();

                for (int i = 0; i <= LastItemIndex; i++)
                    list.Add(FlowPanel.Controls[i].Text);

                return list;
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

        protected virtual void AddFlowPanel()
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

        protected virtual void AddToSubcontrols(Control parent, Control child)
        {
            parent.Controls.Add(child);
        }

        public bool? Add(SearchResult mySearchResult)
        {
            if (!HasInstance())
                Build();

            var myMethod = new Func<Control, bool>(s =>
            {
                if (s.Controls.ContainsKey(mySearchResult.Name))
                    return false;

                AddToSubcontrols(s, mySearchResult);
                return true;
            });

            return (bool?)MyForms.Forms.InvokeIfHandled(
                _flowPanel,
                s => myMethod.Invoke(s),
                true
            );
        }

        public bool? Remove(string text)
        {
            if (!HasInstance())
                Build();

            var myMethod = new Func<Control, bool>(s =>
            {
                if (!s.Controls.ContainsKey(text))
                    return false;

                s.Controls.RemoveByKey(text);
                return true;
            });

            return (bool?)MyForms.Forms.InvokeIfHandled(
                _flowPanel,
                s => myMethod.Invoke(s),
                true
            );
        }

        public void Clear()
        {
            if (!HasInstance())
                Build();

            Controls.Clear();
        }
    }
}
