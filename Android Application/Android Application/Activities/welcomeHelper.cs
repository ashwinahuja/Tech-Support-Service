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
using SQLite;
using Android_Application.Types;

namespace Android_Application.Activities
{
    [Activity(Label = "welcomeHelper")]
    public class welcomeHelper : Activity
    {
        int currentUserId;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                //Sets up the view
                base.OnCreate(savedInstanceState);
                currentUserId = Intent.GetIntExtra("currentUserId", 20);
                RequestWindowFeature(WindowFeatures.NoTitle);
                SetContentView(Resource.Layout.loginWelcomeHelpers);

                //Sets up things in the view
                TextView WelcomeHelperName = FindViewById<TextView>(Resource.Id.welcomeHelperName);
                Button listOfAppointments = FindViewById<Button>(Resource.Id.welcomeHelperListOfAppointments);
                Button setTimetable = FindViewById<Button>(Resource.Id.welcomeHelperSetTimetable);

                //populates the name
                user example = getUserDetails(currentUserId, true);
                WelcomeHelperName.Text = example.firstName + " " + example.surname;

                //Sets up event handlers
                listOfAppointments.Click += ListOfAppointments_Click;
                setTimetable.Click += SetTimetable_Click;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
        }

        private void SetTimetable_Click(object sender, EventArgs e)
        {
            //Goes into set timetable activity
            var newActivity = new Intent(this, typeof(setTimetableWeeks));
            newActivity.PutExtra("helperId", currentUserId);
            StartActivity(newActivity);
        }

        private void ListOfAppointments_Click(object sender, EventArgs e)
        {
            //Goes into the list of appointments activity
            var newActivity = new Intent(this, typeof(listOfAppointments));
            newActivity.PutExtra("currentUserId", currentUserId);
            newActivity.PutExtra("currentUserHelper", true);
            StartActivity(newActivity);
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