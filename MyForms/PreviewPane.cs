using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PdfiumViewer;

namespace MyForms
{
    public class PreviewPane
    {
        public const string PLAIN_TEXT_FONT_FAMILY = "Consolas";
        public const int PLAIN_TEXT_POINT = 10;

        private readonly Control _parentPanel;
        private Control _currentPanel;

        public PreviewPane(Control parent)
        {
            _parentPanel = parent;
        }

        public Control CurrentPanel { get { return _currentPanel; } }

        public Control ParentPanel { get { return _parentPanel; } }

        public static Font MyFont => new Font(PLAIN_TEXT_FONT_FAMILY, PLAIN_TEXT_POINT);

        public static Control NewPdfPreview(string filePath)
        {
            var myRenderer = new PdfRenderer();
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            myRenderer.Load(PdfDocument.Load(new MemoryStream(bytes)));
            return myRenderer;
        }

        public static Control NewPlainTextPreview(string filePath)
        {
            var myRenderer = new RichTextBox();
            myRenderer.LoadFile(filePath, RichTextBoxStreamType.PlainText);
            myRenderer.Font = MyFont;
            myRenderer.BackColor = Color.Black;
            myRenderer.ForeColor = Color.Violet;
            return myRenderer;
        }

        public void ClearPreviewPane()
        {
            _parentPanel.Controls.Remove(_currentPanel);
        }

        public void SetPreviewPane(Control myControl)
        {
            _currentPanel = myControl;
            _parentPanel.Controls.Add(_currentPanel);
            _currentPanel.Dock = DockStyle.Fill;
        }
    }
}
