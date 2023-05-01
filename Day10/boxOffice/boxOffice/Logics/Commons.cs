using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.Windows;

namespace boxOffice.Logics
{
    public class Commons
    {
        public static readonly string myConnString = "Server=localhost;" +
                                               "port=3306;" +
                                               "Database=miniproject;" +
                                               "Uid=root;" +
                                               "Pwd=12345;";

        public static async Task<MessageDialogResult> ShowMessageAsync(string title, string message,
            MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            return await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(title, message, style, null);
        }
    }
}
