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

namespace Android_Application.Activities
{
    [Activity(Label = "welcomeElderly")]
    public class welcomeElderly : Activity
    {
        /// <summary>
        /// What goes into this: 
        ///     the id of the user
        /// 
        /// </summary>
        /// <param name="savedInstanceState"></param>
        int currentUserId;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                //Gets parameters from previous activity
                base.OnCreate(savedInstanceState);
                currentUserId = Intent.GetIntExtra("currentUserId", 20);
                bool sentBack = Intent.GetBooleanExtra("sentBack", false);

                //If appointment just booked, then go to list of appointments
                if (sentBack)
                {
                    click2();
                }

                //Sets up view
                RequestWindowFeature(WindowFeatures.NoTitle);
                SetContentView(Resource.Layout.loginWelcomeElderly);

                //Sets up items in view
                TextView nameOfUser = FindViewById<TextView>(Resource.Id.welcomeElderlyName);
                Button listOfAppointments = FindViewById<Button>(Resource.Id.welcomeElderlyListOfAppointments);
                Button bookAnAppointment = FindViewById<Button>(Resource.Id.welcomeElderlyBookAnAppointment);


                //Populates name of user
                user example = getUserDetails(currentUserId, false);
                nameOfUser.Text = example.firstName + " " + example.surname;

                //Sets up event handlers
                listOfAppointments.Click += ListOfAppointments_Click;
                bookAnAppointment.Click += BookAnAppointment_Click;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
        }

        private void BookAnAppointment_Click(object sender, EventArgs e)
        {
            //Starts the book appointment activity
            var newActivity = new Intent(this, typeof(bookNewAppointment));
            newActivity.PutExtra("currentUserId", currentUserId);
            StartActivity(newActivity);
        }

        private void ListOfAppointments_Click(object sender, EventArgs e)
        {
            //Starts the list of appointments activity
            var newActivity = new Intent(this, typeof(listOfAppointments));
            newActivity.PutExtra("currentUserId", currentUserId);
            newActivity.PutExtra("currentUserHelper", false);
            StartActivity(newActivity);
        }
        private void click2()
        {
            //Starts list of appointment activity
            Intent newActivity = new Intent(this, typeof(listOfAppointments));
            newActivity.AddFlags(ActivityFlags.ClearTop);
            newActivity.PutExtra("currentUserId", currentUserId);
            newActivity.PutExtra("currentUserHelper", false);
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