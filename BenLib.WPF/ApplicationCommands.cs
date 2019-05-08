using BenLib.Framework;
using System.Windows.Controls;
using System.Windows.Input;
using Clipboard = System.Windows.Forms.Clipboard;
using Threading = BenLib.Standard.Threading;

namespace BenLib.WPF
{
    public static class ApplicationCommands
    {
        public static ICommand Copy => new CommandHandler(sender => { if (sender is TextBox tbsender) Threading.MultipleAttempts(() => Clipboard.SetText(tbsender.SelectedText), throwEx: false); });

        public static ICommand Cut => new CommandHandler(sender =>
        {
            if (sender is TextBox tbsender)
            {
                Threading.MultipleAttempts(() => Clipboard.SetText(tbsender.SelectedText), throwEx: false);

                int sst = tbsender.SelectionStart;
                tbsender.Text = tbsender.Text.Remove(sst, tbsender.SelectionLength);
                tbsender.SelectionStart = sst;
            }
        });

        public static ICommand Paste => new CommandHandler(sender => {  if (sender is TextBox tbsender) tbsender.Paste(); });
    }
}
