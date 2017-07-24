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

namespace Android_Application.Types
{
    public class TwilioRegistration
    {
        public string Number { get; set; }
        public string friendlyName { get; set; }
    }
}