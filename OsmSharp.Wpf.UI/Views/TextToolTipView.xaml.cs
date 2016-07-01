namespace OsmSharp.Wpf.UI.Views
{
    /// <summary>
    /// Логика взаимодействия для TextToolTipView.xaml
    /// </summary>
    public partial class TextToolTipView
    {
        public TextToolTipView()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return TextBlock.Text; }
            set { TextBlock.Text = value; }
        }
    }
}
