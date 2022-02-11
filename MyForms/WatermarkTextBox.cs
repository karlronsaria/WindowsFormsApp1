using System.Windows.Forms;
using System.Drawing;

namespace external
{
    /// <link>
    ///   <url>
    ///   https://stackoverflow.com/questions/2487104/how-do-i-implement-a-textbox-that-displays-type-here/11575975
    ///   https://stackoverflow.com/users/837726/joel
    ///   </url>
    ///   <retrievedate>
    ///   2021_12_18
    ///   </retrievedate>
    /// </link>
    /// <link>
    ///   <url>
    ///   https://social.msdn.microsoft.com/Forums/windows/en-US/19be830d-12ff-4a03-9893-0733ca67bd85/how-do-i-prevent-the-designer-from-trying-to-design-my-partial-component?forum=winformsdesigner
    ///   </url>
    ///   <retreivedate>
    ///   2022_02_07
    ///   </retreivedate>
    /// </link>
    /// <summary>
    /// A textbox that supports a watermak hint.
    /// </summary>
    [System.ComponentModel.DesignerCategory("")]
    public class WatermarkTextBox : TextBox
    {
        /// <summary>
        /// The text that will be presented as the watermak hint
        /// </summary>
        private string _watermarkText = "Type here";
        /// <summary>
        /// Gets or Sets the text that will be presented as the watermak hint
        /// </summary>
        public string WatermarkText
        {
            get { return _watermarkText; }
            set { _watermarkText = value; }
        }

        /// <summary>
        /// Whether watermark effect is enabled or not
        /// </summary>
        private bool _watermarkActive = true;
        /// <summary>
        /// Gets or Sets whether watermark effect is enabled or not
        /// </summary>
        public bool WatermarkActive
        {
            get { return _watermarkActive; }
            set { _watermarkActive = value; }
        }

        /// <summary>
        /// Create a new TextBox that supports watermak hint
        /// </summary>
        public WatermarkTextBox()
        {
            this._watermarkActive = true;
            this.Text = _watermarkText;
            this.ForeColor = Color.Gray;

            GotFocus += (sender, e) =>
            {
                RemoveWatermak();
            };

            LostFocus += (sender, e) =>
            {
                ApplyWatermark();
            };
        }

        /// <summary>
        /// Remove watermark from the textbox
        /// </summary>
        public void RemoveWatermak()
        {
            if (this._watermarkActive)
            {
                this._watermarkActive = false;
                this.Text = "";
                this.ForeColor = Color.Black;
            }
        }

        /// <summary>
        /// Applywatermak immediately
        /// </summary>
        public void ApplyWatermark()
        {
            if (!this._watermarkActive
                && string.IsNullOrEmpty(this.Text)
                || ForeColor == Color.Gray)
            {
                this._watermarkActive = true;
                this.Text = _watermarkText;
                this.ForeColor = Color.Gray;
            }
        }

        /// <summary>
        /// Apply watermak to the textbox. 
        /// </summary>
        /// <param name="newText">Text to apply</param>
        public void ApplyWatermark(string newText)
        {
            WatermarkText = newText;
            ApplyWatermark();
        }
        
        protected override void OnCreateControl() 
        { 
            base.OnCreateControl();
            ApplyWatermark();
        }
    }
}
