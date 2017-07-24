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
    [Activity(Label = "RatingsActivity")]
    public class RatingsActivity : Activity
    {
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Gets parameters from the previous activity
            int appointmentId = Intent.GetIntExtra("appointmentId", 0);
            int ratingId = Intent.GetIntExtra("ratingId", 0);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.ratingsScreen);

            //Gets views of the screen
            TextView nameOfHelper = FindViewById<TextView>(Resource.Id.ratingNameOfHelper);
            RatingBar ratingBar = FindViewById<RatingBar>(Resource.Id.ratingBar);
            Button submitRating = FindViewById<Button>(Resource.Id.submitRating);
            
            //Get's existing rating if one exists
            if (ratingId != 0)
            {
                //POPULATE RATING
                ratingBar.Progress = rating(ratingId);
            }
            else
            {
                ratingBar.Progress = 0;
            }

            //POPULATE THE NAME
            try
            {
                user x = getUserDetails(getAppointmentDetails(appointmentId).helperId, true);
                nameOfHelper.Text = x.firstName + " " + x.surname;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }

            //Get event handler
            submitRating.Click += (sender, e) => SubmitRating_Click(sender, e, appointmentId, ratingId);
        }

        private void SubmitRating_Click(object sender, EventArgs e, int appointmentId, int ratingId)
        {
            //Make call to the WebAPI, POSTing the next rating
            try
            {
                RatingBar ratingBar = FindViewById<RatingBar>(Resource.Id.ratingBar);
                rating newRating = new Types.rating();
                newRating.starRating = ratingBar.Progress;
                newRating.helperId = getAppointmentDetails(appointmentId).helperId;
                //uses the ratingId bit as a place to keep appointmentId if it is a new rating
                if (ratingId == 0)
                    newRating.id = appointmentId;
                else
                    newRating.id = ratingId;
                string data = JsonConvert.SerializeObject(newRating);
                string url = String.Empty;
                if (ratingId == 0)
                    url = "http://178.62.87.28:600/api/ratings/ar"; // add if it a new rating
                else
                    url = "http://178.62.87.28:600/api/ratings/ud"; // update if it is an existing rating
                var client = new RestClient(url);
                var request = new RestRequest();
                if (ratingId == 0)
                    request.Method = Method.POST;
                else
                    request.Method = Method.PATCH;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                request.AddParameter("application/json", data, ParameterType.RequestBody);
                var response = client.Execute(request);
                string result = response.Content;
                Toast.MakeText(BaseContext, "Rating submitted.", ToastLength.Short).Show();

            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
        }

        private int rating(int id)
        {
            //Make call to the WebAPI, and gets the integer value of the rating from the rating object.
            try
            {
                string url = "http://178.62.87.28:600/api/ratings/go" + Convert.ToString(id);
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.GET;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                var response = client.Execute(request);
                string result = response.Content;
                rating toReturn = JsonConvert.DeserializeObject<rating>(result);
                return toReturn.starRating;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
                return 0;
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
        private appointment getAppointmentDetails(int id)
        {
            //Make call to the WebAPI, and parse JSON input into a 'appointment' object
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