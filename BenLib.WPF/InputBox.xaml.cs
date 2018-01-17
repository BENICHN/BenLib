using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace BenLib.WPF
{
    /// <summary>
    /// Logique d'interaction pour InputBox.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        #region Champs & Propriétés

        /// <summary>
        /// Résutat de l'instance actuelle de <see cref="InputBox"/>
        /// </summary>
        private InputBoxDialogResult m_result = InputBoxDialogResult.Cancel;

        /// <summary>
        /// Type de contenu de la <see cref='InputBox'/>.
        /// </summary>
        public ContentTypes ContentType { get => TypedTextBox.GetContentType(tb); set => TypedTextBox.SetContentType(tb, value); }

        /// <summary>
        /// Contenu de la <see cref='InputBox'/>.
        /// </summary>
        public string Text => tb.Text;

        #endregion

        #region Constructeur

        public InputBox(string Text = "", string Caption = "", ContentTypes ContentType = ContentTypes.Text, Brush BottomBrush = null)
        {
            InitializeComponent();
            Title = Caption;
            lb.Content = Text;
            this.ContentType = ContentType;
            BottomBorder.Background = BottomBrush ?? new SolidColorBrush(Color.FromRgb(240, 240, 240));
            Icon = new System.Drawing.Bitmap(16, 16).ToSource();
            tb.Focus();
        }

        #endregion

        #region Méthodes

        public new InputBoxDialogResult ShowDialog()
        {
            base.ShowDialog();
            return m_result;
        }

        public static InputBoxResult Show(string Text = "", string Caption = "", ContentTypes ContentType = ContentTypes.Text, Brush BottomBrush = null)
        {
            InputBox box = new InputBox(Text, Caption, ContentType, BottomBrush);
            return new InputBoxResult(box.ShowDialog(), box.Text);
        }

        #endregion

        #region Events

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            m_result = InputBoxDialogResult.OK;
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    m_result = InputBoxDialogResult.OK;
                    Close();
                    break;
                case Key.Escape:
                    Close();
                    break;
            }
        }

        #endregion
    }

    /// <summary>
    /// Contient le résultat d'une <see cref="InputBox"/>.
    /// </summary>
    public class InputBoxResult
    {
        public InputBoxResult(InputBoxDialogResult Result, string Text)
        {
            this.Result = Result;
            this.Text = Text;
        }

        public InputBoxDialogResult Result { get; set; }
        public string Text { get; set; }
    }

    public enum InputBoxDialogResult { OK, Cancel }
}
