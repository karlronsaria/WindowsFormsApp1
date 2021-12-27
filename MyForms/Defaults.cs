namespace MyForms
{
    class SystemTextBox : System.Windows.Forms.TextBox
    {
        public string WatermarkText { get; set; }
    }

    // class MyTextBox : SystemTextBox { }
    class MyTextBox : external.WatermarkTextBox { }
}