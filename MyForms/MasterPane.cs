using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MyForms
{
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
            Dictionary<SublayoutType, ILayout>
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

        public EventHandler LayoutChanged { get; set; } = delegate { };

        public SearchResultLayoutWithEndButton Tags
        {
            get
            {
                var key = SublayoutType.Tags;

                if (!Layouts.ContainsKey(key))
                    AddInOrder<SearchResultLayoutWithEndButton>(key);

                return Layouts[key] as SearchResultLayoutWithEndButton;
            }
        }

        public DateLayoutWithEndButton Dates
        {
            get
            {
                var key = SublayoutType.Dates;

                if (!Layouts.ContainsKey(key))
                    AddInOrder<DateLayoutWithEndButton>(key);

                return Layouts[key] as DateLayoutWithEndButton;
            }
        }

        public void Clear()
        {
            var myMethod = new Func<MasterPane, bool>(pane =>
            {
                pane.Controls.Clear();
                pane.Layouts.Clear();
                pane.LayoutChanged.Invoke(pane, new EventArgs());
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
            Controls.Clear();

            foreach (var key in (SublayoutType[])Enum.GetValues(typeof(SublayoutType)))
                if (Layouts.ContainsKey(key))
                    Controls.Add(Layouts[key]);
        }

        public bool? AddInOrder<LayoutT>(
                SublayoutType key,
                SearchResult mySearchResult,
                ILayout.RemoveOn removeWhen = ILayout.RemoveOn.NONE
            ) where LayoutT : ILayout, new()
        {
            return AddInOrder<LayoutT>(key, mySearchResult, null, removeWhen);
        }

        public bool? AddInOrder<LayoutT>(
                SublayoutType key,
                SearchResult mySearchResult,
                string labelText,
                ILayout.RemoveOn removeWhen = ILayout.RemoveOn.NONE
            ) where LayoutT : ILayout, new()
        {
            var myMethod = new Func<MasterPane, bool>(pane =>
            {
                labelText = labelText ?? $"{key}:";
                pane.AddInOrder(key, new LayoutT() { LabelText = labelText });
                bool success = pane.Layouts[key].Add(mySearchResult, removeWhen) ?? false;
                // pane.LayoutChanged.Invoke(pane, new EventArgs());
                return success;
            });

            return (bool?)MyForms.Forms.InvokeIfHandled(
                this,
                s => myMethod.Invoke(s as MasterPane),
                IsHandleCreated
            );
        }

        public bool? AddInOrder<LayoutT>(
                SublayoutType key
            ) where LayoutT : ILayout, new()
        {
            var myMethod = new Func<MasterPane, bool>(pane =>
                pane.AddInOrder(key, new LayoutT() { LabelText = $"{key}:" }) ?? false
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

        public bool? AddInOrder(SublayoutType key, ILayout value)
        {
            var myMethod = new Func<MasterPane, bool>(pane =>
            {
                if (pane.Layouts.ContainsKey(key))
                    return false;

                pane.Layouts.Add(key, value);
                pane.ReorderLayouts();

                if (key == SublayoutType.Documents)
                    pane.Layouts[key].ItemRemoved += (sender, e) => ProcessWhenItemRemoved(key);

                pane.LayoutChanged.Invoke(pane, new EventArgs());
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
            Layouts.TryGetValue(key, out ILayout subpanel);
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
