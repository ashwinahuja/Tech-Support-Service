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
using System.Globalization;

namespace Android_Application.Activities
{
    [Activity(Label = "setTimetableDays")]
    public class setTimetableDays : ListActivity
    {
        //Global variables
        int helperId;
        string[] listOfThingsToDisplay;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                //Get parameters from previous activity
                base.OnCreate(savedInstanceState);
                helperId = Intent.GetIntExtra("helperId", 8);
                DateTime d = DateTime.ParseExact(Intent.GetStringExtra("dateTime"), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                //gets a list of the days of the week to display on the screen
                listOfThingsToDisplay = new string[7];
                for (int i = 0; i < listOfThingsToDisplay.Length; i++)
                {
                    DateTime dt = d.AddDays(i);
                    listOfThingsToDisplay[i] = dt.ToString("MMMM dd, yyyy");
                }
                RequestWindowFeature(WindowFeatures.NoTitle);
                ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, listOfThingsToDisplay);
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
        }
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            //Starts the activity of setting the free time of the clicked day
            base.OnListItemClick(l, v, position, id);
            DateTime toPass = DateTime.ParseExact(listOfThingsToDisplay[position], "MMMM dd, yyyy", CultureInfo.InvariantCulture);
            string toPassString = toPass.ToString("dd/MM/yyyy");
            var newActivity = new Intent(this, typeof(setTimetableTimes));
            newActivity.PutExtra("helperId", helperId);
            newActivity.PutExtra("dateTime", toPassString);
            StartActivity(newActivity);
        }
    }
}