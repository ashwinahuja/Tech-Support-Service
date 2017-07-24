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

using Android_Application.Backend;
using RestSharp;
using System.Globalization;

namespace Android_Application.Activities
{
    [Activity(Label = "setTimetableWeeks")]
    public class setTimetableWeeks : ListActivity
    {
        int helperId;
        string[] listOfThingsToDisplay;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                //Gets parameters from the previous screen
                base.OnCreate(savedInstanceState);
                helperId = Intent.GetIntExtra("helperId", 8);
                listOfThingsToDisplay = new string[8];

                //Places the next 8 weeks in the screen and creates an array of them
                for (int i = 0; i < listOfThingsToDisplay.Length; i++)
                {
                    DateTime dt = DateTime.Now.AddDays(7 * i).StartOfWeek(DayOfWeek.Monday);
                    listOfThingsToDisplay[i] = dt.ToString("MMMM dd, yyyy");
                }

                //display them
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
            //Start the activity to choose which day's timetable to change, passing the week beginning date.
            base.OnListItemClick(l, v, position, id);
            DateTime toPass = DateTime.ParseExact(listOfThingsToDisplay[position], "MMMM dd, yyyy", CultureInfo.InvariantCulture);
            string toPassString = toPass.ToString("dd/MM/yyyy");
            var newActivity = new Intent(this, typeof(setTimetableDays));
            newActivity.PutExtra("helperId", helperId);
            newActivity.PutExtra("dateTime", toPassString);
            StartActivity(newActivity);
        }
    }
}