using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MyForms
{
    public class MasterPane : FlowLayoutPanel
    {
        public enum SublayoutType : int
        {
            Documents,
            Tags,
            Dates,
        }

        internal class LayoutDictionary :
            Dictionary<SublayoutType, SearchResultLayout>
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

        internal LayoutDictionary
        Layouts
        {
            get => _sublayouts;
        }

        public EventHandler LayoutChanged { get; set; } = delegate { };

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

        public bool Remove(SublayoutType key)
        {
            var myMethod = new Func<MasterPane, bool>(pane =>
            {
                pane.Controls.Remove(Layouts[key]);
                pane.LayoutChanged.Invoke(pane, new EventArgs());
                return true;
            });

            MyForms.Forms.InvokeIfHandled(
                this,
                s => myMethod.Invoke(s as MasterPane),
                IsHandleCreated
            );

            return Layouts.Remove(key);
        }

        public bool? Add<LayoutT>(
                SublayoutType key,
                SearchResult mySearchResult,
                SearchResultLayout.RemoveOn removeWhen = SearchResultLayout.RemoveOn.NONE
            ) where LayoutT : SearchResultLayout, new()
        {
            return Add<LayoutT>(key, mySearchResult, null, removeWhen);
        }

        public bool? Add<LayoutT>(
                SublayoutType key,
                SearchResult mySearchResult,
                string labelText,
                SearchResultLayout.RemoveOn removeWhen = SearchResultLayout.RemoveOn.NONE
            ) where LayoutT : SearchResultLayout, new()
        {
            var myMethod = new Func<MasterPane, bool>(pane =>
            {
                labelText = labelText ?? key.ToString();
                pane.Add(key, new LayoutT() { LabelText = $"{labelText}:" });
                bool success = pane.Layouts[key].Add(mySearchResult, removeWhen) ?? false;
                pane.LayoutChanged.Invoke(pane, new EventArgs());
                return success;
            });

            return (bool?)MyForms.Forms.InvokeIfHandled(
                this,
                s => myMethod.Invoke(s as MasterPane),
                IsHandleCreated
            );
        }

        public bool? Add(SublayoutType key, SearchResultLayout value)
        {
            var myMethod = new Func<MasterPane, bool>(pane =>
            {
                if (pane.Layouts.ContainsKey(key))
                    return false;

                pane.Layouts.Add(key, value);
                pane.Controls.Add(value);
                // pane.LayoutChanged.Invoke(pane, new EventArgs());
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
            Layouts.TryGetValue(key, out SearchResultLayout subpanel);
            return subpanel?.Values;
        }

        public bool Any()
        {
            return Layouts.Count > 0;
        }
    }
}
