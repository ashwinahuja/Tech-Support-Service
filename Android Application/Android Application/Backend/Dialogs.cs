using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Android_Application.Backend
{
    class Dialogs
    {
        internal Dialogs(string title, string content, Context y)
        {
            AlertDialog.Builder a = new AlertDialog.Builder(y);
            a.SetTitle(title);
            a.SetMessage(content);
            Dialog dialog = a.Create();
            dialog.Show();
        }
    }
}