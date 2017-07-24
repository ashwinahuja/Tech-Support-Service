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
    [Activity(Label = "reviewActivity")]
    public class reviewActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Get parameters from previous activity
            int appointmentId = Intent.GetIntExtra("appointmentId", 0);
            int reviewId = Intent.GetIntExtra("reviewId", 0);

            //Sets up view
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.reviewScreen);
            
            //Sets up items in view
            TextView nameOfHelper = FindViewById<TextView>(Resource.Id.reviewNameOfHelper);
            EditText reviewComment = FindViewById<EditText>(Resource.Id.reviewComment);
            Button submitReview = FindViewById<Button>(Resource.Id.submitReview);

            if (reviewId != 0)
            {
                //POPULATE RATING
                reviewComment.Text = review(reviewId);
            }
            else
            {
                reviewComment.Text = String.Empty;
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

            submitReview.Click += (sender, e) => SubmitReview_Click(sender, e, appointmentId, reviewId);
        }

        private void SubmitReview_Click(object sender, EventArgs e, int appointmentId, int ratingId)
        {
            //Submits the review
            /*
             * If existing review, it is updated - otherwise it is added with the appointment id also passed to the WebAPI to be placed in the database.
             * 
             */
            try
            {
                EditText reviewComment = FindViewById<EditText>(Resource.Id.reviewComment);
                review newReview = new Types.review();
                newReview.comment = reviewComment.Text;
                newReview.helperId = getAppointmentDetails(appointmentId).helperId;
                if (ratingId == 0)
                    newReview.id = appointmentId;
                else
                    newReview.id = ratingId;
                string data = JsonConvert.SerializeObject(newReview);
                string url = String.Empty;
                if (ratingId == 0)
                    url = "http://178.62.87.28:600/api/reviews/ar";
                else
                    url = "http://178.62.87.28:600/api/reviews/ud";
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
                Toast.MakeText(BaseContext, "Review submitted.", ToastLength.Short).Show();

            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
        }

        private string review(int id)
        {
            //Make call to the WebAPI, and returns the review (as a string) after it being deserialised
            try
            {
                string url = "http://178.62.87.28:600/api/reviews/go" + Convert.ToString(id);
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.GET;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                var response = client.Execute(request);
                string result = response.Content;
                review toReturn = JsonConvert.DeserializeObject<review>(result);
                return toReturn.comment;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
                return String.Empty;
            }
        }
        private user getUserDetails(int id, bool helper)
        {
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