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

using Android_Application.Types;
using RestSharp;
using Newtonsoft.Json;
using System.Globalization;
using Android_Application.Backend;

namespace Android_Application.Activities
{
    [Activity(Label = "setTimetableTimes")]
    public class setTimetableTimes : Activity
    {
        timetable t;
        DateTime dt;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                //Get things from previous activity
                base.OnCreate(savedInstanceState);
                t = getTimetableOfHelper(Intent.GetIntExtra("helperId", 8));
                dt = DateTime.ParseExact(Intent.GetStringExtra("dateTime"), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                //Setup view
                RequestWindowFeature(WindowFeatures.NoTitle);
                SetContentView(Resource.Layout.setTimetableTimes);

                //Setup items in the views
                Switch a02 = FindViewById<Switch>(Resource.Id.a02);
                Switch a24 = FindViewById<Switch>(Resource.Id.a24);
                Switch a46 = FindViewById<Switch>(Resource.Id.a46);
                Switch a68 = FindViewById<Switch>(Resource.Id.a68);
                Switch a810 = FindViewById<Switch>(Resource.Id.a810);
                Switch a1012 = FindViewById<Switch>(Resource.Id.a1012);
                Switch a1214 = FindViewById<Switch>(Resource.Id.a1214);
                Switch a1416 = FindViewById<Switch>(Resource.Id.a1416);
                Switch a1618 = FindViewById<Switch>(Resource.Id.a1618);
                Switch a1820 = FindViewById<Switch>(Resource.Id.a1820);
                Switch a2022 = FindViewById<Switch>(Resource.Id.a2022);
                Switch a2224 = FindViewById<Switch>(Resource.Id.a2224);


                getCorrectStatusOfButtons(); // Literally, get correct status of buttons
                Button submitButton = FindViewById<Button>(Resource.Id.setTimetableSubmit);

                //Event handlers
                submitButton.Click += SubmitButton_Click;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
        }
        private void getCorrectStatusOfButtons()
        {
            //Setup items
            Switch a02 = FindViewById<Switch>(Resource.Id.a02);
            Switch a24 = FindViewById<Switch>(Resource.Id.a24);
            Switch a46 = FindViewById<Switch>(Resource.Id.a46);
            Switch a68 = FindViewById<Switch>(Resource.Id.a68);
            Switch a810 = FindViewById<Switch>(Resource.Id.a810);
            Switch a1012 = FindViewById<Switch>(Resource.Id.a1012);
            Switch a1214 = FindViewById<Switch>(Resource.Id.a1214);
            Switch a1416 = FindViewById<Switch>(Resource.Id.a1416);
            Switch a1618 = FindViewById<Switch>(Resource.Id.a1618);
            Switch a1820 = FindViewById<Switch>(Resource.Id.a1820);
            Switch a2022 = FindViewById<Switch>(Resource.Id.a2022);
            Switch a2224 = FindViewById<Switch>(Resource.Id.a2224);
            day day = new day();
            bool found = false;

            //Try to find which day of the timetable is relevant
            for (int i = 0; i < t.weeks.Count(); i++)
            {
                if (t.weeks[i].weekBeginning == dt.StartOfWeek(DayOfWeek.Monday)) // For each week in the timetable (custom weeks) check if it the one we are looking for
                {
                    found = true;
                    switch (dt.DayOfWeek) // find exactly which day it is
                    {
                        case DayOfWeek.Monday:
                            day = t.weeks[i].week.monday;
                            break;
                        case DayOfWeek.Tuesday:
                            day = t.weeks[i].week.tuesday;
                            break;
                        case DayOfWeek.Wednesday:
                            day = t.weeks[i].week.wednesday;
                            break;
                        case DayOfWeek.Thursday:
                            day = t.weeks[i].week.thursday;
                            break;
                        case DayOfWeek.Friday:
                            day = t.weeks[i].week.friday;
                            break;
                        case DayOfWeek.Saturday:
                            day = t.weeks[i].week.saturday;
                            break;
                        case DayOfWeek.Sunday:
                            day = t.weeks[i].week.sunday;
                            break;
                        default:
                            day = t.weeks[i].week.monday;
                            break;
                    }
                }
            }
            if(!found) // instead need to use the defaultweek as the correct week
            {
                switch (dt.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        day = t.defaultWeek.monday;
                        break;
                    case DayOfWeek.Tuesday:
                        day = t.defaultWeek.tuesday;
                        break;
                    case DayOfWeek.Wednesday:
                        day = t.defaultWeek.wednesday;
                        break;
                    case DayOfWeek.Thursday:
                        day = t.defaultWeek.thursday;
                        break;
                    case DayOfWeek.Friday:
                        day = t.defaultWeek.friday;
                        break;
                    case DayOfWeek.Saturday:
                        day = t.defaultWeek.saturday;
                        break;
                    case DayOfWeek.Sunday:
                        day = t.defaultWeek.sunday;
                        break;
                    default:
                        day = t.defaultWeek.monday;
                        break;
                }
            }

            //populate the switches based on the day we have found.

            a02.Checked = day.timesFree[0];
            a24.Checked = day.timesFree[1];
            a46.Checked = day.timesFree[2];
            a68.Checked = day.timesFree[3];
            a810.Checked = day.timesFree[4];
            a1012.Checked = day.timesFree[5];
            a1214.Checked = day.timesFree[6];
            a1416.Checked = day.timesFree[7];
            a1618.Checked = day.timesFree[8];
            a1820.Checked = day.timesFree[9];
            a2022.Checked = day.timesFree[10];
            a2224.Checked = day.timesFree[11];
            return;

        }
        private void SubmitButton_Click(object sender, EventArgs e)
        {
            //if submit button is pressed
            bool found = false;
            for (int i = 0; i < t.weeks.Count(); i++) // find which week we need to update or if it is the default week
            {
                if(t.weeks[i].weekBeginning == dt.StartOfWeek(DayOfWeek.Monday)) // if it this week
                {
                    //If yes
                    found = true;
                    switch (dt.DayOfWeek) // Find which day to update
                    {
                        case DayOfWeek.Monday:
                            t.weeks[i].week.monday = assembleDay(); // Change the day as required
                            break;
                        case DayOfWeek.Tuesday:
                            t.weeks[i].week.tuesday = assembleDay();
                            break;
                        case DayOfWeek.Wednesday:
                            t.weeks[i].week.wednesday = assembleDay();
                            break;
                        case DayOfWeek.Thursday:
                            t.weeks[i].week.thursday = assembleDay();
                            break;
                        case DayOfWeek.Friday:
                            t.weeks[i].week.friday = assembleDay();
                            break;
                        case DayOfWeek.Saturday:
                            t.weeks[i].week.saturday = assembleDay();
                            break;
                        case DayOfWeek.Sunday:
                            t.weeks[i].week.sunday = assembleDay();
                            break;
                        default:
                            t.weeks[i].week.monday = assembleDay();
                            break;
                    }
                }
            }
            if(!found) // nope, not found
            {
                names x = new names(); // create a new week in the views
                x.weekBeginning = dt.StartOfWeek(DayOfWeek.Monday); // populate the week beginning
                week week = t.defaultWeek; // now make a clone of the default week
                switch (dt.DayOfWeek) // then change one of the days as required.
                {
                    case DayOfWeek.Monday:
                        week.monday = assembleDay();
                        break;
                    case DayOfWeek.Tuesday:
                        week.tuesday = assembleDay();
                        break;
                    case DayOfWeek.Wednesday:
                        week.wednesday = assembleDay();
                        break;
                    case DayOfWeek.Thursday:
                        week.thursday = assembleDay();
                        break;
                    case DayOfWeek.Friday:
                        week.friday = assembleDay();
                        break;
                    case DayOfWeek.Saturday:
                        week.saturday = assembleDay();
                        break;
                    case DayOfWeek.Sunday:
                        week.sunday = assembleDay();
                        break;
                    default:
                        week.monday = assembleDay();
                        break;
                }
                x.week = week;
                t.weeks.Add(x); // add it to the existing timetable  
            }
            try
            {
                //Now add the timetable and therefore it is updated
                String data = JsonConvert.SerializeObject(t);
                var client = new RestClient("http://178.62.87.28:600/api/tt/adt");
                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                request.AddParameter("application/json", data, ParameterType.RequestBody);
                client.Execute(request);
                Toast.MakeText(BaseContext, "The day has been successfully updated.", ToastLength.Short).Show(); // Notify the user that the day has been updated


            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
        }
        private day assembleDay()
        {
            //Convert the position of the switches into a 'day' object based on their position
            Switch a02 = FindViewById<Switch>(Resource.Id.a02);
            Switch a24 = FindViewById<Switch>(Resource.Id.a24);
            Switch a46 = FindViewById<Switch>(Resource.Id.a46);
            Switch a68 = FindViewById<Switch>(Resource.Id.a68);
            Switch a810 = FindViewById<Switch>(Resource.Id.a810);
            Switch a1012 = FindViewById<Switch>(Resource.Id.a1012);
            Switch a1214 = FindViewById<Switch>(Resource.Id.a1214);
            Switch a1416 = FindViewById<Switch>(Resource.Id.a1416);
            Switch a1618 = FindViewById<Switch>(Resource.Id.a1618);
            Switch a1820 = FindViewById<Switch>(Resource.Id.a1820);
            Switch a2022 = FindViewById<Switch>(Resource.Id.a2022);
            Switch a2224 = FindViewById<Switch>(Resource.Id.a2224);
            day day = new day();
            day.timesFree[0] = a02.Checked;
            day.timesFree[1] = a24.Checked;
            day.timesFree[2] = a46.Checked;
            day.timesFree[3] = a68.Checked;
            day.timesFree[4] = a810.Checked;
            day.timesFree[5] = a1012.Checked;
            day.timesFree[6] = a1214.Checked;
            day.timesFree[7] = a1416.Checked;
            day.timesFree[8] = a1618.Checked;
            day.timesFree[9] = a1820.Checked;
            day.timesFree[10] = a2022.Checked;
            day.timesFree[11] = a2224.Checked;
            return day;
        }

        private timetable getTimetableOfHelper(int id)
        {
            //Make call to the WebAPI, and parse JSON input into a 'timetable' object
            try
            {
                int timetableId = getUserDetails(id, true).timetableId;
                string url = "http://178.62.87.28:600/api/tt/gt" + Convert.ToString(timetableId);
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.GET;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                var response = client.Execute(request);
                string result = response.Content;
                timetable toReturn = JsonConvert.DeserializeObject<timetable>(result);
                return toReturn;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
                return new timetable();
            }
        }
        private user getUserDetails(int id, bool helper)
        {
            //Make call to the WebAPI, and parse JSON input into a 'user' object
            try
            {
                string url = String.Empty;
                if (helper)
                    url = "http://178.62.87.28:600/api/ach/users/gi" + Convert.ToString(id);
                else
                    url = "http://178.62.87.28:600/api/ace/users/gi" + Convert.ToString(id);
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.GET;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                var response = client.Execute(request);
                string result = response.Content;
                user toReturn = JsonConvert.DeserializeObject<user>(result);
                return toReturn;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
                return new user();
            }
        }
    }
}