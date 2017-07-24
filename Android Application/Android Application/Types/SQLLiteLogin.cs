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

using SQLite;
using SQLitePCL;
namespace Android_Application.Types
{
    public class SQLLiteLogin
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string emailAddress { get; set; }
        public string password { get; set; }
        public bool helper { get; set; }
        public SQLLiteLogin()
        { }

    }
}