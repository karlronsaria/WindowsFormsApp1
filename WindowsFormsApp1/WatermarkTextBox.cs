using System.Windows.Forms;
using System.Drawing;

namespace external
{
    /// <link>
    ///   <url>
    ///   https://stackoverflow.com/questions/2487104/how-do-i-implement-a-textbox-that-displays-type-here/11575975
    ///   https://stackoverflow.com/users/837726/joel
    ///   </url>
    ///   <retrievedata>
    ///   2021_12_18
    ///   </retrievedata>
    /// </link>
    /// <summary>
    /// A textbox that supports a watermak hint.
    /// </summary>
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

            GotFocus += (source, e) =>
            {
                RemoveWatermak();
            };

            LostFocus += (source, e) =>
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

    /*
    // link: https://www.codeproject.com/Articles/27849/WaterMark-TextBox-For-Desktop-Applications-Using-C
    // retrieved: 2021_12_18
    class WaterMarkTextBox : TextBox
    {
        private Font oldFont = null;
        private Boolean waterMarkTextEnabled = false;

        #region Attributes 
            private Color _waterMarkColor = Color.Gray;
            public Color WaterMarkColor
            {
                get { return _waterMarkColor; }
                set {
                    _waterMarkColor = value; Invalidate();
                    // thanks to Bernhard Elbl for Invalidate()
                }
            }

            private string _waterMarkText = "Water Mark";
            public string WaterMarkText
            {
                get { return _waterMarkText; }
                set { _waterMarkText = value; Invalidate(); }
            }
        #endregion

        //Default constructor
        public WaterMarkTextBox()
        {
            JoinEvents(true);
        }

        //Override OnCreateControl ... thanks to  "lpgray .. codeproject guy"
        protected override void OnCreateControl() 
        { 
            base.OnCreateControl();
            WaterMark_Toggle(null, null); 
        }
        
        //Override OnPaint
        protected override void OnPaint(PaintEventArgs args)
        {
            // Use the same font that was defined in base class
            System.Drawing.Font drawFont = new System.Drawing.Font(Font.FontFamily,
                Font.Size, Font.Style, Font.Unit);
            //Create new brush with gray color or 
            SolidBrush drawBrush = new SolidBrush(WaterMarkColor);//use Water mark color
            //Draw Text or WaterMark
            args.Graphics.DrawString((waterMarkTextEnabled ? WaterMarkText : Text),
                drawFont, drawBrush, new PointF(0.0F, 0.0F));
            base.OnPaint(args);
        }

        private void JoinEvents(Boolean join)
        {
            if (join)
            {
                this.TextChanged += new System.EventHandler(this.WaterMark_Toggle);
                this.LostFocus += new System.EventHandler(this.WaterMark_Toggle);
                this.FontChanged += new System.EventHandler(this.WaterMark_FontChanged);
                //No one of the above events will start immeddiatlly 
                //TextBox control still in constructing, so,
                //Font object (for example) couldn't be catched from within
                //WaterMark_Toggle
                //So, call WaterMark_Toggel through OnCreateControl after TextBox
                //is totally created
                //No doupt, it will be only one time call
                
                //Old solution uses Timer.Tick event to check Create property
            }
        }

        private void WaterMark_Toggle(object sender, EventArgs args )
        {
            if (this.Text.Length <= 0)
                EnableWaterMark();
            else
                DisbaleWaterMark();
        }

        private void EnableWaterMark()
        {
            //Save current font until returning the UserPaint style to false (NOTE:
            //It is a try and error advice)
            oldFont = new System.Drawing.Font(Font.FontFamily, Font.Size, Font.Style,
               Font.Unit);
            //Enable OnPaint event handler
            this.SetStyle(ControlStyles.UserPaint, true);
            this.waterMarkTextEnabled = true;
            //Triger OnPaint immediatly
            Refresh();
        }

        private void DisbaleWaterMark()
        {
            //Disbale OnPaint event handler
            this.waterMarkTextEnabled = false;
            this.SetStyle(ControlStyles.UserPaint, false);
            //Return back oldFont if existed
            if(oldFont != null)
                this.Font = new System.Drawing.Font(oldFont.FontFamily, oldFont.Size,
                    oldFont.Style, oldFont.Unit);
        }

        private void WaterMark_FontChanged(object sender, EventArgs args)
        {
            if (waterMarkTextEnabled)
            {
                oldFont = new System.Drawing.Font(Font.FontFamily,Font.Size,Font.Style,
                    Font.Unit);
                Refresh();
            }
        }
    }
    */
}
