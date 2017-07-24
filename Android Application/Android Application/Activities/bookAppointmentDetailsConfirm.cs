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
using Android_Application.Backend;
using Newtonsoft.Json;
using System.Globalization;

namespace Android_Application.Activities
{
    [Activity(Label = "bookAppointmentDetailsConfirm")]
    public class bookAppointmentDetailsConfirm : Activity
    {
        //Global variables used throughout the program
        int helperId;
        int elderlyId;
        int index;
        int dayId;
        DateTime dateTimeOfAppointment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                //Sets up the view
                base.OnCreate(savedInstanceState);
                RequestWindowFeature(WindowFeatures.NoTitle);
                SetContentView(Resource.Layout.appointmentDetails);

                //Gets the parameters passed when the activity was called
                helperId = Intent.GetIntExtra("helperId", 8);
                elderlyId = Intent.GetIntExtra("elderlyId", 20);
                dayId = Intent.GetIntExtra("dayId", 1);
                index = Intent.GetIntExtra("index", 1);
                dateTimeOfAppointment = DateTime.ParseExact(Intent.GetStringExtra("dateTime"), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                //Populates the elements of the view
                TextView nameOfHelper = FindViewById<TextView>(Resource.Id.nameOfHelper);
                TextView dateOfAppointment = FindViewById<TextView>(Resource.Id.dateOfAppointment);
                TextView timeOfAppointment = FindViewById<TextView>(Resource.Id.timeOfAppointment);
                Button submitButton = FindViewById<Button>(Resource.Id.confirmAppointment);
                submitButton.Click += SubmitButton_Click;
                user helper = getUserDetails(helperId, true);
                nameOfHelper.Text = helper.firstName + " " + helper.surname;
                dateOfAppointment.Text = dateTimeOfAppointment.ToString("MMMM dd, yyyy");
                timeOfAppointment.Text = dateTimeOfAppointment.ToString("HH:mm");
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            //Submit button is clicked - appointment is to be booked.
            try
            {
                appointment newAppointment = new appointment();
                newAppointment.dateAndTime = dateTimeOfAppointment;
                newAppointment.dateCreated = DateTime.Now;
                newAppointment.elderlyId = elderlyId;
                newAppointment.helperId = helperId;
                string url = String.Empty;
                string data = JsonConvert.SerializeObject(newAppointment);
                url = "http://178.62.87.28:600/api/app/an";
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                request.AddParameter("application/json", data, ParameterType.RequestBody);
                var response = client.Execute(request);
                string result = response.Content;

                //Goes into welcome elderly with a sentBack parameter - so it is passed to the listOfAppointments
                var newActivity = new Intent(this, typeof(welcomeElderly));
                newActivity.PutExtra("currentUserId", elderlyId);
                newActivity.PutExtra("sentBack", true);
                StartActivity(newActivity);
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
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