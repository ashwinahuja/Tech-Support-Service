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

using RestSharp;
using Newtonsoft.Json;
using Android_Application.Types;
using Android_Application.Backend;
using System.Globalization;

namespace Android_Application.Activities
{
    [Activity(Label = "bookAppointmentTimes")]
    public class bookAppointmentTimes : ListActivity
    {
        //Global variables which are used throughout the program
        int helperId;
        int elderlyId;
        DateTime dt;
        day day;
        int dayId;
        List<string> times = new List<string>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                //Gets all the parameters which are passed to the activity
                base.OnCreate(savedInstanceState);
                helperId = Intent.GetIntExtra("helperId", 8);
                elderlyId = Intent.GetIntExtra("elderlyId", 20);
                dayId = Intent.GetIntExtra("dayId", 1);

                //Populates the listView
                /*
                 * For each time, check if it free and check if it is in the past.
                 * */
                day = getDay(dayId);
                dt = DateTime.Parse(Intent.GetStringExtra("dateTime"));

                for (int i = 0; i < day.timesFree.Count(); i++)
                {
                    if (day.timesFree[i])
                    {
                        DateTime dt2 = new DateTime();
                        dt2 = dt.AddHours(2 * i);
                        if (DateTime.Now < dt2)
                        {
                            times.Add((2 * i).ToString("00") + ":00 to " + Convert.ToString(2 * (i + 1)) + ":00");
                        }
                    }
                    else
                    { }
                }
                string[] timesForList = times.ToArray();
                if (times.Count() == 0)
                {
                    //If no available times
                    Toast.MakeText(BaseContext,"No times available on this day - Please try a different day",ToastLength.Long).Show();
                    Finish();

                }
                RequestWindowFeature(WindowFeatures.NoTitle);
                ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, timesForList);
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
        }
        protected day getDay (int id)
        {
            //Make call to the WebAPI, and parse JSON input into a 'day' object
            try
            {
                string url = "http://178.62.87.28:600/api/tt/gd" + Convert.ToString(id);
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.GET;

                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                var response = client.Execute(request);
                string result = response.Content;
                day toReturn = JsonConvert.DeserializeObject<day>(result);
                return toReturn;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
                return new day();
            }
        }
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            //When an item is clicked, find the time which was clicked
            base.OnListItemClick(l, v, position, id);
            string a = times[position];
            string b = a.Split(' ')[0];
            DateTime dateTimeToSend = dt.AddHours(DateTime.ParseExact(b,"HH:mm", CultureInfo.InvariantCulture).Hour);

            //Start activity to confirm details of an appointment and allow them to confirm it.
            var newActivity = new Intent(this, typeof(bookAppointmentDetailsConfirm));
            newActivity.PutExtra("dayId", dayId);
            newActivity.PutExtra("helperId", helperId);
            newActivity.PutExtra("elderlyId", elderlyId);
            string x = dateTimeToSend.ToString("dd/MM/yyyy HH:mm");
            newActivity.PutExtra("dateTime", x);
            StartActivity(newActivity);

            //When returned, pass back to previous screen
            Finish();
        }
    }
}