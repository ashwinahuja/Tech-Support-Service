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
using Android_Application.Types;
using Newtonsoft.Json;

namespace Android_Application.Activities
{
    [Activity(Label = "listOfAppointments")]
    public class listOfAppointments : ListActivity
    {
        //Global variables
        appointment[] listOfAppointments2;
        int currentUserId;
        bool currentUserHelper;
       
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Gets parameters from previous activity
            base.OnCreate(savedInstanceState);
            currentUserId = Intent.GetIntExtra("currentUserId", 20);
            currentUserHelper = Intent.GetBooleanExtra("currentUserHelper", false);
            listOfAppointments2 = returnListOfAppointments(currentUserId, currentUserHelper);

            //Populates the list
            try
            {
                //Sorts the list into order of time
                Array.Sort(listOfAppointments2, delegate (appointment x, appointment y) { return x.dateAndTime.CompareTo(y.dateAndTime); });
                List<appointment> newList = listOfAppointments2.OfType<appointment>().ToList();
                List<appointment> secondList = new List<appointment>();
                
                //Get list of all appointments which are old and get them at the beginning
                while (newList.Count() > 0 && newList[0].dateAndTime < DateTime.Now)
                {
                    secondList.Insert(0, newList[0]);
                    newList.RemoveAt(0);
                }

                List<appointment> thirdList = new List<appointment>();
                thirdList = newList.Concat(secondList).ToList();

                //Therefore, order is now... nearest to latest in future, then just gone backwards in history
                listOfAppointments2 = thirdList.ToArray();
                string[] thingsToShowOnListView = new string[listOfAppointments2.Length];
                for (int i = 0; i < listOfAppointments2.Length; i++)
                {
                    //Gets the exact details of what to show on the view about the appointment
                    string name = String.Empty;
                    user x = new user();
                    if (currentUserHelper)
                        x = getUserDetails(listOfAppointments2[i].elderlyId, false);
                    else
                        x = getUserDetails(listOfAppointments2[i].helperId, true);
                    name = x.firstName + " " + x.surname;
                    thingsToShowOnListView[i] = "With " + name + " at " + listOfAppointments2[i].dateAndTime.ToString("H:mm") + " on " + listOfAppointments2[i].dateAndTime.ToString("MMMM dd, yyyy");

                }
                RequestWindowFeature(WindowFeatures.NoTitle);
                ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, thingsToShowOnListView);

            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                
                StartActivity(typeof(MainActivity));
            }
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            //If clicked, it starts the appointments details activity
            base.OnListItemClick(l, v, position, id);
            appointment appointmentSelected = listOfAppointments2[position];
            var activityToStart = new Intent(this, typeof(AppointmentsActivity));
            activityToStart.PutExtra("appointmentId", appointmentSelected.id);
            activityToStart.PutExtra("currentUserId", currentUserId);
            activityToStart.PutExtra("currentUserHelper", currentUserHelper);
            StartActivity(activityToStart);

        }

        
        private appointment[] returnListOfAppointments(int id, bool helper)
        {
            //Make call to the WebAPI, and parse JSON input into an array of 'appointment' object
            string url = "http://178.62.87.28:600/api/app/g";
            if (helper)
                url += "AH" + Convert.ToString(id);
            else
                url += "AE" + Convert.ToString(id);
            var client = new RestClient(url);
            var request = new RestRequest();
            request.Method = Method.GET;
            request.AddHeader("Accept", "application/json");
            request.Parameters.Clear();
            var response = client.Execute(request);
            string result = response.Content;
            appointment[] toReturn = JsonConvert.DeserializeObject<appointment[]>(result);
            return toReturn;
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
        private appointment getAppointmentDetails(int id)
        {
            //Make call to the WebAPI, and parse JSON input into an 'appointment' object
            try
            {
                string url = "http://178.62.87.28:600/api/app/gi" + Convert.ToString(id);
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.GET;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                var response = client.Execute(request);
                string result = response.Content;
                appointment toReturn = JsonConvert.DeserializeObject<appointment>(result);
                return toReturn;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
                return new appointment();
            }
        }
    }
}