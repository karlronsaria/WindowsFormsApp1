using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MyForms
{
    public abstract class ISearchResult : System.Windows.Forms.TextBox
    {
        public ISearchResult() : base()
        {
            base.ReadOnly = true;
        }

        public abstract string ToolTip { get; set; }

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

    public abstract class ILayout : System.Windows.Forms.FlowLayoutPanel
    {
        public enum RemoveOn : int
        {
            NONE,
            CLICK,
            DOUBLE_CLICK,
            CONTROL_CLICK,
            SHIFT_CLICK,
        }

        public EventHandler ItemRemoved { get; set; } = delegate { };

        public abstract string LabelText { get; set; }

        public abstract int Count { get; }

        public abstract IEnumerable<string> Values { get; }

        public abstract bool FlowPanelEmpty();

        public abstract bool? Add(ISearchResult mySearchResult, RemoveOn removeWhen);

        public bool Any()
        {
            return Count > 0;
        }
    }

    public class DateLayoutWithEndButton : LayoutWithEndButton<SearchResult>
    {
        public static void HandleDateTextChange(TextBox textBox)
        {
            if (Application.DateText.TryGetDateString(textBox.Text, out string newText))
            {
                textBox.Text = newText;
                textBox.Select(textBox.TextLength, 0);
            }
        }

        public static bool HandleDateKeyPress(TextBox textBox, KeyPressEventArgs e)
        {
            if (Application.DateText.GetNextPartialDateString(
                    keyChar: e.KeyChar,
                    input: textBox.Text,
                    nextPartialDate: out string newText
                ))
            {
                textBox.Text = newText;
                textBox.Select(textBox.TextLength, 0);
                return e.Handled = true;
            }

            return e.Handled;
        }

        public static T ToDateTextBox<T>(T textBox)
            where T : TextBox, new()
        {
            textBox.KeyPress += (sender, e) => HandleDateKeyPress(textBox as TextBox, e);
            textBox.TextChanged += (sender, e) => HandleDateTextChange(textBox as TextBox);
            return textBox;
        }

        public DateLayoutWithEndButton() : base()
        {
            ToDateTextBox<TextBox>(NewItemTextBox);
        }

        public DateLayoutWithEndButton(
                int spacingHeight
            ) : base(spacingHeight)
        {
            ToDateTextBox<TextBox>(NewItemTextBox);
        }

        public DateLayoutWithEndButton(
                Control parent,
                string labelText,
                int spacingHeight = DEFAULT_SPACING_HEIGHT
            ) : base(parent, labelText, spacingHeight)
        {
            ToDateTextBox<TextBox>(NewItemTextBox);
        }
    }

    public class SearchResultLayout : Layout<SearchResult>
    {
        public SearchResultLayout() : base() { }

        public SearchResultLayout(
                int spacingHeight
            ) : base(spacingHeight) { }

        public SearchResultLayout(
                Control parent,
                string labelText,
                int spacingHeight = DEFAULT_SPACING_HEIGHT
            ) : base(parent, labelText, spacingHeight) { }
    }

    public class SearchResultLayoutWithEndButton : LayoutWithEndButton<SearchResult>
    {
        public SearchResultLayoutWithEndButton() : base() { }

        public SearchResultLayoutWithEndButton(
                int spacingHeight
            ) : base(spacingHeight) { }

        public SearchResultLayoutWithEndButton(
                Control parent,
                string labelText,
                int spacingHeight = DEFAULT_SPACING_HEIGHT
            ) : base(parent, labelText, spacingHeight) { }
    }

    public class LayoutWithEndButton<ButtonT> : Layout<ButtonT>
        where ButtonT : ISearchResult, new()
    {
        public const string NEW_ITEM_TEXT = " + ";

        public LayoutWithEndButton() : base()
        {
            BuildNewItemTextBox();
        }

        public LayoutWithEndButton(
                int spacingHeight
            ) : base(spacingHeight)
        {
            BuildNewItemTextBox();
        }

        public LayoutWithEndButton(
                Control parent,
                string labelText,
                int spacingHeight = DEFAULT_SPACING_HEIGHT
            ) : base(parent, labelText, spacingHeight)
        {
            BuildNewItemTextBox();
        }

        private readonly ButtonT _newItemButton = new ButtonT()
        {
            Text = NEW_ITEM_TEXT,
            BackColor = System.Drawing.Color.DimGray,
            ToolTip = "New Item",
        };

        public ButtonT NewItemButton
        {
            get
            {
                if (!HasInstance())
                    Build();

                return _newItemButton;
            }
        }

        protected override int LastItemIndex
        {
            get
            {
                return FlowPanel.Controls.Contains(NewItemButton)
                    ? FlowPanel.Controls.GetChildIndex(NewItemButton) - 1
                    : -1;
            }
        }

        private readonly TextBox _newItemTextBox = new TextBox();

        private void BuildNewItemTextBox()
        {
            NewItemTextBox.LostFocus += (lostFocusSender, lostFocusArgs) =>
            {
                Remove(NewItemTextBox);
                Add(NewItemButton);
            };

            NewItemTextBox.KeyDown += (keyDownSender, keyDownArgs) =>
            {
                switch (keyDownArgs.KeyCode)
                {
                    case Keys.Enter:
                        string text = NewItemTextBox.Text;
                        Remove(NewItemTextBox);

                        if (!String.IsNullOrWhiteSpace(text))
                            Add(
                                mySearchResult: new ButtonT() { Text = text, },
                                removeWhen: RemoveOn.CLICK
                            );

                        Add(NewItemButton);
                        break;
                    case Keys.Escape:
                        Remove(NewItemTextBox);
                        Add(NewItemButton);
                        break;
                }
            };
        }

        public TextBox NewItemTextBox
        {
            get
            {
                return _newItemTextBox;
            }
        }

        protected void ProcessNewItemButton(object sender, EventArgs e)
        {
            Remove(NewItemButton);
            // var myTextBox = new TextBox();
            NewItemTextBox.Text = "";
            Add(NewItemTextBox);
            NewItemTextBox.Focus();

        }

        protected override void AddFlowPanel()
        {
            base.AddFlowPanel();
            Add(NewItemButton);

            NewItemButton.Click += ProcessNewItemButton;
            NewItemButton.GotFocus += ProcessNewItemButton;
        }

        protected override void AddToSubcontrols(Control parent, Control child)
        {
            parent.Controls.Remove(NewItemButton);
            parent.Controls.Add(child);
            parent.Controls.Add(NewItemButton);
        }
    }

    public class Layout<ButtonT> : ILayout
        where ButtonT : ISearchResult, new()
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

        public Layout()
        {
            Initialize();
            SpacingHeight = DEFAULT_SPACING_HEIGHT;
        }

        public Layout(
                int spacingHeight
            ) : base()
        {
            Initialize();
            SpacingHeight = spacingHeight;
        }

        public Layout(
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

        public override string LabelText
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

        public override IEnumerable<string> Values
        {
            get
            {
                var list = new List<string>();

                for (int i = 0; i <= LastItemIndex; i++)
                    list.Add(FlowPanel.Controls[i].Text);

                return list;
            }
        }

        protected bool HasInstance()
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

        protected void Build(string labelText = "")
        {
            AddSpacing();
            AddLabel(labelText);
            AddFlowPanel();
        }

        protected virtual void AddToSubcontrols(Control parent, Control child)
        {
            parent.Controls.Add(child);
        }

        protected bool? Add(Control myControl)
        {
            var myMethod = new Func<Control, bool>(s =>
            {
                s.Controls.Add(myControl);
                return true;
            });

            return (bool?)MyForms.Forms.InvokeIfHandled(
                _flowPanel,
                s => myMethod.Invoke(s),
                IsHandleCreated
            );
        }

        protected bool? Remove(Control myControl)
        {
            var myMethod = new Func<Control, bool>(s =>
            {
                s.Controls.Remove(myControl);
                this.ItemRemoved.Invoke(s, new EventArgs());
                return true;
            });

            return (bool?)MyForms.Forms.InvokeIfHandled(
                _flowPanel,
                s => myMethod.Invoke(s),
                IsHandleCreated
            );
        }

        public override bool? Add(ISearchResult mySearchResult, RemoveOn removeWhen = RemoveOn.NONE)
        {
            if (!HasInstance())
                Build();

            var myMethod = new Func<Control, bool>(s =>
            {
                if (s.Controls.ContainsKey(mySearchResult.Name))
                    return false;

                void removalHandler(object sender, EventArgs args)
                {
                    Remove(mySearchResult);
                }

                switch (removeWhen)
                {
                    case RemoveOn.NONE:
                        break;
                    case RemoveOn.CLICK:
                        mySearchResult.Click += removalHandler;
                        break;
                    case RemoveOn.DOUBLE_CLICK:
                        mySearchResult.DoubleClick += removalHandler;
                        break;
                }

                AddToSubcontrols(s, mySearchResult);
                return true;
            });

            return (bool?)MyForms.Forms.InvokeIfHandled(
                _flowPanel,
                s => myMethod.Invoke(s),
                IsHandleCreated
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
                this.ItemRemoved.Invoke(s, new EventArgs());
                return true;
            });

            return (bool?)MyForms.Forms.InvokeIfHandled(
                _flowPanel,
                s => myMethod.Invoke(s),
                IsHandleCreated
            );
        }

        public void Clear()
        {
            if (!HasInstance())
                Build();

            var myMethod = new Func<Control, bool>(s =>
            {
                s.Controls.Clear();
                this.ItemRemoved.Invoke(s, new EventArgs());
                return true;
            });

            MyForms.Forms.InvokeIfHandled(
                _flowPanel,
                s => myMethod.Invoke(s),
                IsHandleCreated
            );
        }

        public override int Count
        {
            get => LastItemIndex + 1;
        }

        public override bool FlowPanelEmpty()
        {
            return FlowPanel.Controls.Count == 0;
        }
    }
}
