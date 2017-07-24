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
using Android_Application.Backend;

namespace Android_Application.Activities
{
    [Activity(Label = "bookAppointment")]
    public class bookAppointment : ListActivity
    {
        // Define global variables which can be accessed within the class
        int helperId;
        int elderlyId;
        string[] toDisplayArray;
        List<DateTime> existingWeeksDateTime = new List<DateTime>();
        List<DateTime> originalList;
        List<week> existingWeeksWeek;
        timetable ttOfHelper;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                //Get parameters from previous screen
                base.OnCreate(savedInstanceState);
                helperId = Intent.GetIntExtra("helperId", 8);
                elderlyId = Intent.GetIntExtra("elderlyId", 20);
                
                //Setup view
                RequestWindowFeature(WindowFeatures.NoTitle);

                //Get timetable of helper
                ttOfHelper = new timetable();
                ttOfHelper = getTimetableOfHelper(helperId);

                //Populates the screen with available weeks
                List<string> toDisplay = new List<string>();
                existingWeeksWeek = new List<week>();
                for (int i = 0; i < ttOfHelper.weeks.Count(); i++)
                {
                    if (ttOfHelper.weeks[i].weekBeginning < DateTime.Now.AddDays(-7) && ttOfHelper.weeks[i].weekBeginning < DateTime.Now.AddDays(28)) // Do nothing if before now or after 6 weeks time
                    { }
                    else
                    {
                        //Since the weekId is necessary to be known, first we add all the custom weeks
                        existingWeeksDateTime.Add(ttOfHelper.weeks[i].weekBeginning);
                        existingWeeksWeek.Add(ttOfHelper.weeks[i].week);
                    }
                }
                originalList = new List<DateTime>(existingWeeksDateTime); // creates a list
                if (existingWeeksDateTime.Count() < 4) // only next four weeks
                {
                    DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday); // gets the week beginning date of current week
                    for (int i = 0; i < 4; i++)
                    {
                        if (existingWeeksDateTime.Contains(dt)) // check if the list already contains the date
                        {
                            //CHECK IF THERE IS ANY FREE TIME
                            int index = originalList.IndexOf(dt);
                            bool freeTime = checkIfAnyTime(existingWeeksWeek[index]); // if no time, then don't show it.
                            if (!freeTime)
                            {
                                existingWeeksDateTime.RemoveAt(index);
                            }
                            else
                            { }
                        }
                        else // not in custom list
                        {
                            bool freeTime = checkIfAnyTime(ttOfHelper.defaultWeek); // check if there would be free time
                            if (!freeTime)
                            { }
                            else
                            { existingWeeksDateTime.Add(dt); } // If possible to get appointments, then add it to the list to be displayed
                        }

                        dt = dt.AddDays(7);
                    }
                }
                existingWeeksDateTime = SortAscending(existingWeeksDateTime); // Sort the list of dateTimes
                for (int i = 0; i < existingWeeksDateTime.Count(); i++)
                {
                    toDisplay.Add(existingWeeksDateTime[i].ToString("MMMM dd, yyyy")); // Set the display format
                }

                toDisplayArray = new string[toDisplay.Count()]; // Array to display
                for (int j = 0; j < toDisplayArray.Length; j++)
                {
                    toDisplayArray[j] = toDisplay[j]; 
                }
                ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, toDisplayArray);
                
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
        }
        protected bool checkIfAnyTime(week week)
        {
            // Checks if there is any time in a week
            if(!checkDay(week.monday) && !checkDay(week.tuesday) && !checkDay(week.wednesday) && !checkDay(week.thursday) && !checkDay(week.friday) && !checkDay(week.saturday) && !checkDay(week.saturday) && !checkDay(week.sunday))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// Checks if there is any time in a day
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        protected bool checkDay(day day)
        {
            for (int i = 0; i < day.timesFree.Length; i++)
            {
                if (day.timesFree[i])
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Sorts a list of DateTimes in an ascending order - from earliest to latest.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        static List<DateTime> SortAscending(List<DateTime> list)
        {
            list.Sort((a, b) => a.CompareTo(b));
            return list;
        }

        //When an item is clicked
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            //Firstly, attempts to find the weekId of the item clicked.
            base.OnListItemClick(l, v, position, id);
            string dateTimeOfSelectedItem = existingWeeksDateTime[position].ToString("dd/MM/yyyy");
            DateTime dateTime = existingWeeksDateTime[position];
            int weekId;
            if(originalList.Contains(dateTime)) // see if custom week
            {
                int index = originalList.IndexOf(dateTime);
                weekId = existingWeeksWeek[index].id;
            }
            else
            {
                weekId = ttOfHelper.defaultWeek.id;
            }

            //starts new activity to select day
            var newActivity = new Intent(this, typeof(bookAppointmentDays));
            newActivity.PutExtra("weekId", weekId);
            newActivity.PutExtra("weekBeginning", dateTimeOfSelectedItem);
            newActivity.PutExtra("helperId", helperId);
            newActivity.PutExtra("elderlyId", elderlyId);
            StartActivity(newActivity);
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