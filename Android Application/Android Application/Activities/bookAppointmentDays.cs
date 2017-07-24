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
using Android_Application.Backend;
using Android_Application.Types;
using Newtonsoft.Json;

namespace Android_Application.Activities
{
    [Activity(Label = "bookAppointmentDays")]
    public class bookAppointmentDays : ListActivity
    {
        week weekToBeDisplayed;
        int helperId;
        int elderlyId;
        DateTime dtOfWeekBeginning;
        List<string> listOfDisplayed;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                weekToBeDisplayed = getWeek(Intent.GetIntExtra("weekId", 1));
                helperId = Intent.GetIntExtra("helperId", 8);
                elderlyId = Intent.GetIntExtra("elderlyId", 20);
                dtOfWeekBeginning = DateTime.Parse(Intent.GetStringExtra("weekBeginning"));
                listOfDisplayed = new List<string>();
                //MONDAY
                if (checkDay(weekToBeDisplayed.monday) && DateTime.Now <= dtOfWeekBeginning.AddDays(1))
                {
                    listOfDisplayed.Add(dtOfWeekBeginning.AddDays(0).DayOfWeek.ToString() + " " + dtOfWeekBeginning.AddDays(0).ToString("MMMM dd, yyyy"));
                }
                //TUESDAY
                if (checkDay(weekToBeDisplayed.tuesday) && DateTime.Now <= dtOfWeekBeginning.AddDays(2))
                {
                    listOfDisplayed.Add(dtOfWeekBeginning.AddDays(1).DayOfWeek.ToString() + " " + dtOfWeekBeginning.AddDays(1).ToString("MMMM dd, yyyy"));
                }
                //WEDNESDAY
                if (checkDay(weekToBeDisplayed.wednesday) && DateTime.Now <= dtOfWeekBeginning.AddDays(3))
                {
                    listOfDisplayed.Add(dtOfWeekBeginning.AddDays(2).DayOfWeek.ToString() + " " + dtOfWeekBeginning.AddDays(2).ToString("MMMM dd, yyyy"));
                }
                //THURSDAY
                if (checkDay(weekToBeDisplayed.thursday) && DateTime.Now <= dtOfWeekBeginning.AddDays(4))
                {
                    listOfDisplayed.Add(dtOfWeekBeginning.AddDays(3).DayOfWeek.ToString() + " " + dtOfWeekBeginning.AddDays(3).ToString("MMMM dd, yyyy"));
                }
                //FRIDAY
                if (checkDay(weekToBeDisplayed.friday) && DateTime.Now <= dtOfWeekBeginning.AddDays(5))
                {
                    listOfDisplayed.Add(dtOfWeekBeginning.AddDays(4).DayOfWeek.ToString() + " " + dtOfWeekBeginning.AddDays(4).ToString("MMMM dd, yyyy"));
                }
                //SATURDAY
                if (checkDay(weekToBeDisplayed.saturday) && DateTime.Now <= dtOfWeekBeginning.AddDays(6))
                {
                    listOfDisplayed.Add(dtOfWeekBeginning.AddDays(5).DayOfWeek.ToString() + " " + dtOfWeekBeginning.AddDays(5).ToString("MMMM dd, yyyy"));
                }
                //SUNDAY
                if (checkDay(weekToBeDisplayed.sunday) && DateTime.Now <= dtOfWeekBeginning.AddDays(7))
                {
                    listOfDisplayed.Add(dtOfWeekBeginning.AddDays(6).DayOfWeek.ToString() + " " + dtOfWeekBeginning.AddDays(6).ToString("MMMM dd, yyyy"));
                }

                string[] thingsToBeDisplayed = listOfDisplayed.ToArray();
                if (listOfDisplayed.Count() == 0)
                {
                    Toast.MakeText(BaseContext, "No times available on this week", ToastLength.Short).Show();
                    Finish();

                }
                RequestWindowFeature(WindowFeatures.NoTitle);
                ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, thingsToBeDisplayed);
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }

            // Create your application here
        }
        protected week getWeek(int id)
        {
            try
            {
                string url = "http://178.62.87.28:600/api/tt/gw" + Convert.ToString(id);
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.GET;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                var response = client.Execute(request);
                string result = response.Content;
                week toReturn = JsonConvert.DeserializeObject<week>(result);
                return toReturn;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
                return new week();
            }
        }
        protected bool checkDay(day day)
        {
            for (int i = 0; i < day.timesFree.Length; i++)
            {
                if (day.timesFree[i])
                    return true;
            }
            return false;
        }
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            base.OnListItemClick(l, v, position, id);
            //helperId
            //elderlyId
            //date
            //dayId
            int dayId = 1;
            int numberToAdvance = 0;
            string a = listOfDisplayed[position];
            if(a.Contains("Monday"))
            {
                dayId = weekToBeDisplayed.monday.id;
                numberToAdvance = 0;
            }
            else if (a.Contains("Tuesday"))
            {
                dayId = weekToBeDisplayed.tuesday.id;
                numberToAdvance = 1;
            }
            else if (a.Contains("Wednesday"))
            {
                dayId = weekToBeDisplayed.wednesday.id;
                numberToAdvance = 2;
            }
            else if (a.Contains("Thursday"))
            {
                dayId = weekToBeDisplayed.thursday.id;
                numberToAdvance = 3;
            }
            else if (a.Contains("Friday"))
            {
                dayId = weekToBeDisplayed.friday.id;
                numberToAdvance = 4;
            }
            else if (a.Contains("Saturday"))
            {
                dayId = weekToBeDisplayed.saturday.id;
                numberToAdvance = 5;
            }
            else if (a.Contains("Sunday"))
            {
                dayId = weekToBeDisplayed.sunday.id;
                numberToAdvance = 6;
            }
            DateTime dayToSend = dtOfWeekBeginning.AddDays(numberToAdvance);
            string dateTimeToSend = dayToSend.ToString("dd/MM/yyyy");
            var newActivity = new Intent(this, typeof(bookAppointmentTimes));
            newActivity.PutExtra("dayId", dayId);
            newActivity.PutExtra("helperId", helperId);
            newActivity.PutExtra("elderlyId", elderlyId);
            newActivity.PutExtra("dateTime", dateTimeToSend);
            
            StartActivity(newActivity);
        }

        
    }
}