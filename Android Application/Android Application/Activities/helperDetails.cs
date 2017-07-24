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
using Android_Application.Backend;
namespace Android_Application.Activities
{
    [Activity(Label = "helperDetails")]
    public class helperDetails : Activity
    {
        int currentUserId;
        int helperId;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Gets parameters from the previous activity
            currentUserId = Intent.GetIntExtra("currentUserId", 20);
            helperId = Intent.GetIntExtra("helperId", 2);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.helperDetails);
            
            //Sets up views
            user helper = getUserDetails(helperId, true);
            TextView name = FindViewById<TextView>(Resource.Id.helperDetailsName);
            TextView firstLine = FindViewById<TextView>(Resource.Id.helperDetailsFirstLine);
            TextView secondLine = FindViewById<TextView>(Resource.Id.helperDetailsSecondLine);
            TextView city = FindViewById<TextView>(Resource.Id.helperDetailsCity);
            TextView postalCode = FindViewById<TextView>(Resource.Id.helperDetailsPostcode);
            TextView country = FindViewById<TextView>(Resource.Id.helperDetailsCountry);
            TextView telephone = FindViewById<TextView>(Resource.Id.helperDetailsContactNumber);
            Button seeReviews = FindViewById<Button>(Resource.Id.helperDetailsSeeReviews);
            Button bookAppointment = FindViewById<Button>(Resource.Id.helperDetailsBookAppointment);

            //Populates them
            try
            {
                name.Text = helper.firstName + " " + helper.surname;
                firstLine.Text = helper.firstLineOfAddress;
                if (!String.IsNullOrEmpty(helper.secondLineOfAddress))
                    secondLine.Text = helper.secondLineOfAddress;
                else
                    secondLine.Visibility = ViewStates.Gone;
                city.Text = helper.city;
                country.Text = helper.country;
                postalCode.Text = helper.postalCode;
                telephone.Text = helper.telephoneNumber;
                if (reviewForUser(helper.id))
                    seeReviews.Enabled = true;
                else
                    seeReviews.Enabled = false;

                bookAppointment.Click += BookAppointment_Click;
                seeReviews.Click += SeeReviews_Click;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
            

        }

        private void SeeReviews_Click(object sender, EventArgs e)
        {
            //If see review is clicked, goes into activity to see reviews
            var newActivity = new Intent(this, typeof(Android_Application.Activities.SeeAllReviewOfHelper));
            newActivity.PutExtra("helperId", helperId);
            StartActivity(newActivity);
            this.Recreate();
        }

        private void BookAppointment_Click(object sender, EventArgs e)
        {
            //Goes into the activity to book an appointment with this helper
            var newActivity = new Intent(this, typeof(bookAppointment));
            newActivity.PutExtra("helperId", helperId);
            newActivity.PutExtra("elderlyId", currentUserId);
            StartActivity(newActivity);
        }

        private bool reviewForUser(int id)
        {
            //Make call to the WebAPI, and parse JSON input into a 'review' object. Then it returns a true if any of them is a 'real' review (not a null) or false if there are no reviews
            //Therefore, if there are no reviews, the button is not enabled.
            try
            {
                string url = "http://178.62.87.28:600/api/reviews/ga" + Convert.ToString(id);
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.GET;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                var response = client.Execute(request);
                string result = response.Content;
                if (result == "[]")
                    return false;
                else
                    return true;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
                return false;
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