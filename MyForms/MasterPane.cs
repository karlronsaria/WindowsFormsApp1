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

        public void Clear()
        {
            Controls.Clear();
            Layouts.Clear();
        }

        public bool Remove(SublayoutType key)
        {
            Controls.Remove(Layouts[key]);
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
            var myMethod = new Func<Control, bool>(s =>
            {
                var pane = s as MasterPane;
                labelText = labelText ?? key.ToString();

                pane.Add(key, new LayoutT() { LabelText = $"{labelText}:", });
                return pane.Layouts[key].Add(mySearchResult, removeWhen) ?? false;
            });

            return (bool?)MyForms.Forms.InvokeIfHandled(
                this,
                s => myMethod.Invoke(s),
                IsHandleCreated
            );
        }

        public bool? Add(SublayoutType key, SearchResultLayout value)
        {
            var myMethod = new Func<Control, bool>(s =>
            {
                var pane = s as MasterPane;

                if (pane.Layouts.ContainsKey(key))
                    return false;

                pane.Layouts.Add(key, value);
                pane.Controls.Add(value);
                return true;
            });

            return (bool?)MyForms.Forms.InvokeIfHandled(
                this,
                s => myMethod.Invoke(s),
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
    }
}
