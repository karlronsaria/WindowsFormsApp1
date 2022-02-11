using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MyForms
{
    // link: https://social.msdn.microsoft.com/Forums/windows/en-US/19be830d-12ff-4a03-9893-0733ca67bd85/how-do-i-prevent-the-designer-from-trying-to-design-my-partial-component?forum=winformsdesigner
    // retrieved: 2022_02_07
    [System.ComponentModel.DesignerCategory("")]
    public class MasterPane : FlowLayoutPanel
    {
        public const string TOSTRING_SPACING = "    ";

        public enum SublayoutType : int
        {
            Documents,
            Tags,
            Dates,
        }

        internal class LayoutDictionary :
            Dictionary<SublayoutType, Layout>
        {
            protected new void Clear()
            {
                base.Clear();
            }
        }

        private readonly LayoutDictionary _sublayouts;

        public MasterPane(): base()
        {
            _sublayouts = new LayoutDictionary();
        }

        protected new ControlCollection Controls
        {
            get => base.Controls;
        }

        internal LayoutDictionary Layouts
        {
            get => _sublayouts;
        }

        // // TODO
        // // OLD (2022_01_26)
        // public EventHandler LayoutChanged { get; set; } = delegate { };
        public EventHandler LayoutChanged { get; set; } = new EventHandler(delegate { return; });

        public SearchResultLayoutWithEndButton Tags
        {
            get
            {
                var key = SublayoutType.Tags;

                if (!Layouts.ContainsKey(key))
                    AddInOrder<SearchResultLayoutWithEndButton>(key, LayoutChanged);

                return Layouts[key] as SearchResultLayoutWithEndButton;
            }
        }

        public DateLayoutWithEndButton Dates
        {
            get
            {
                var key = SublayoutType.Dates;

                if (!Layouts.ContainsKey(key))
                    AddInOrder<DateLayoutWithEndButton>(key, LayoutChanged);

                return Layouts[key] as DateLayoutWithEndButton;
            }
        }

        public void Clear()
        {
            var myMethod = new Func<MasterPane, bool>(pane =>
            {
                pane.Controls.Clear();
                pane.Layouts.Clear();
                pane.LayoutChanged?.Invoke(pane, new EventArgs());
                return true;
            });

            MyForms.Forms.InvokeIfHandled(
                this,
                s => myMethod.Invoke(s as MasterPane),
                IsHandleCreated
            );
        }

        public bool? Remove(SublayoutType key)
        {
            var myMethod = new Func<MasterPane, bool>(pane =>
            {
                pane.Controls.Remove(Layouts[key]);
                bool success = pane.Layouts.Remove(key);
                pane.LayoutChanged.Invoke(pane, new EventArgs());
                return success;
            });

            return (bool?)MyForms.Forms.InvokeIfHandled(
                this,
                s => myMethod.Invoke(s as MasterPane),
                IsHandleCreated
            );
        }

        private void ReorderLayouts()
        {
            int i = 0;
            var nonLayoutControls = new List<Control>();

            while (i < Controls.Count && !(Controls[i] is Layout))
                nonLayoutControls.Add(Controls[i++]);

            Controls.Clear();

            foreach (Control control in nonLayoutControls)
                Controls.Add(control);

            foreach (var key in (SublayoutType[])Enum.GetValues(typeof(SublayoutType)))
                if (Layouts.ContainsKey(key))
                    Controls.Add(Layouts[key]);
        }

        public bool? AddInOrder<LayoutT>(
                SublayoutType key,
                SearchResult mySearchResult,
                Layout.RemoveOn removeWhen = MyForms.Layout.RemoveOn.NONE,
                EventHandler onContentChange = null
            ) where LayoutT : Layout, new()
        {
            return AddInOrder<LayoutT>(key, mySearchResult, null, removeWhen, onContentChange);
        }

        public bool? AddInOrder<LayoutT>(
                SublayoutType key,
                SearchResult mySearchResult,
                string labelText,
                Layout.RemoveOn removeWhen = MyForms.Layout.RemoveOn.NONE,
                EventHandler onContentChange = null
            ) where LayoutT : Layout, new()
        {
            var myMethod = new Func<MasterPane, bool>(pane =>
            {
                labelText = labelText ?? $"{key}:";
                pane.AddInOrder(key, new LayoutT() { LabelText = labelText }, onContentChange);
                bool success = pane.Layouts[key].Add(mySearchResult, removeWhen) ?? false;
                return success;
            });

            return (bool?)MyForms.Forms.InvokeIfHandled(
                this,
                s => myMethod.Invoke(s as MasterPane),
                IsHandleCreated
            );
        }

        public bool? AddInOrder<LayoutT>(
                SublayoutType key,
                EventHandler onContentChange = null
            ) where LayoutT : Layout, new()
        {
            var myMethod = new Func<MasterPane, bool>(pane =>
                pane.AddInOrder(
                    key,
                    new LayoutT() { LabelText = $"{key}:" },
                    onContentChange
                ) ?? false
            );

            return (bool?)MyForms.Forms.InvokeIfHandled(
                this,
                s => myMethod.Invoke(s as MasterPane),
                IsHandleCreated
            );
        }

        protected virtual void ProcessWhenItemRemoved(SublayoutType key)
        {
            Layouts.TryGetValue(key, out var layout);

            if (layout?.FlowPanelEmpty() ?? false)
                Remove(key);
        }

        public bool? AddInOrder(SublayoutType key, Layout value, EventHandler onContentChange = null)
        {
            if (onContentChange == null)
                onContentChange = LayoutChanged;

            var myMethod = new Func<MasterPane, bool>(pane =>
            {
                if (pane.Layouts.ContainsKey(key))
                    return false;

                value.ItemAdded += onContentChange;
                value.ItemRemoved += onContentChange;
                pane.Layouts.Add(key, value);
                pane.ReorderLayouts();

                if (key == SublayoutType.Documents)
                    pane.Layouts[key].ItemRemoved += (sender, e) => ProcessWhenItemRemoved(key);

                pane.LayoutChanged?.Invoke(pane, new EventArgs());
                return true;
            });

            return (bool?)MyForms.Forms.InvokeIfHandled(
                this,
                s => myMethod.Invoke(s as MasterPane),
                IsHandleCreated
            );
        }

        public bool Contains(SublayoutType key)
        {
            return Layouts.ContainsKey(key);
        }

        public IEnumerable<string> GetValues(SublayoutType key)
        {
            Layouts.TryGetValue(key, out Layout subpanel);
            return subpanel?.Values;
        }

        public bool Any()
        {
            return Layouts.Count > 0;
        }

        public override string ToString()
        {
            string text = "";

            foreach (var key in (MasterPane.SublayoutType[])Enum.GetValues(typeof(MasterPane.SublayoutType)))
            {
                if (Layouts.ContainsKey(key))
                {
                    text += $"{key}:\n\n";

                    foreach (string value in GetValues(key))
                    {
                        text += $"{TOSTRING_SPACING}{value}\n";
                    }

                    text += "\n";
                }
            }

            return text;
        }
    }
}
