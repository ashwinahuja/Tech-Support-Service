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
    [Activity(Label = "AppointmentsActivity")]
    public class AppointmentsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Gets the parameters from previous screen
            int appointmentId = Intent.GetIntExtra("appointmentId", 16);
            int currentUser = Intent.GetIntExtra("currentUserId", 20);
            bool currentUserHelper = Intent.GetBooleanExtra("currentUserHelper", false);

            //Sets up screen
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.appointmentDetailsScreen);

            //Defines views in the screen
            TextView appointmentDetailsDate = FindViewById<TextView>(Resource.Id.appointmentDetailsDate);
            TextView appointmentDetailsTime = FindViewById<TextView>(Resource.Id.appointmentDetailsTime);
            TextView appointmentDetailsName = FindViewById<TextView>(Resource.Id.appointmentDetailsName);
            TextView appointmentDetailsContactNumber = FindViewById<TextView>(Resource.Id.appointmentDetailsContactNumber);
            TextView appointmentDetailsFirstLineOfAddress = FindViewById<TextView>(Resource.Id.appointmentDetailsFirstLineOfAddressOfAppointment);
            TextView appointmentDetailsSecondLineOfAddress = FindViewById<TextView>(Resource.Id.appointmentDetailsSecondLineOfAddressOfAppointment);
            TextView appointmentDetailsCityAddress = FindViewById<TextView>(Resource.Id.appointmentDetailsCityAddressOfAppointment);
            TextView appointmentDetailsPostalCode = FindViewById<TextView>(Resource.Id.appointmentDetailsPostalCodeOfAppointment);
            Button appointmentDetailsOfCancelAppointment = FindViewById<Button>(Resource.Id.cancelAppointment);
            Button appointmentDetailsSetRating = FindViewById<Button>(Resource.Id.setRating);
            Button appointmentDetailsSetReview = FindViewById<Button>(Resource.Id.setReview);

            //Populates the view
            try
            {
                appointment appointmentDetails = getAppointmentDetails(appointmentId);
                appointmentDetailsDate.Text = appointmentDetails.dateAndTime.ToString("MMMM dd, yyyy");
                appointmentDetailsTime.Text = appointmentDetails.dateAndTime.ToString("H:mm");
                user helper = new user();
                user elderly = new user();
                if (currentUserHelper)
                {
                    helper = getUserDetails(currentUser, true);
                    elderly = getUserDetails(appointmentDetails.elderlyId, false);
                    appointmentDetailsName.Text = elderly.firstName + " " + elderly.surname; // populates name and contact number views
                    appointmentDetailsContactNumber.Text = elderly.telephoneNumber;
                }
                else
                {
                    helper = getUserDetails(appointmentDetails.helperId, true);
                    elderly = getUserDetails(appointmentDetails.elderlyId, false);
                    appointmentDetailsName.Text = helper.firstName + " " + helper.surname;
                    appointmentDetailsContactNumber.Text = helper.telephoneNumber;
                }

                //Populates address views
            
                appointmentDetailsFirstLineOfAddress.Text = elderly.firstLineOfAddress;
                if (String.IsNullOrEmpty(elderly.secondLineOfAddress))
                    appointmentDetailsSecondLineOfAddress.Visibility = ViewStates.Gone;
                else
                {
                    appointmentDetailsSecondLineOfAddress.Text = elderly.secondLineOfAddress;
                }
                appointmentDetailsCityAddress.Text = elderly.city;
                appointmentDetailsPostalCode.Text = elderly.postalCode;

                //Enables cancel appointment if current
                if (appointmentDetails.dateAndTime > DateTime.Now)
                {
                    appointmentDetailsOfCancelAppointment.Enabled = true;
                    appointmentDetailsSetRating.Enabled = false;
                    appointmentDetailsSetReview.Enabled = false;
                }

                //Disables cancel appointment if old - also enables set review and setrating if the person is an elderly person 
                else
                {
                    appointmentDetailsOfCancelAppointment.Enabled = false;
                    if (!currentUserHelper)
                    {
                        appointmentDetailsSetRating.Enabled = true;
                        appointmentDetailsSetReview.Enabled = true;
                    }
                    else
                    {
                        appointmentDetailsSetRating.Enabled = false;
                        appointmentDetailsSetReview.Enabled = false;
                    }
                }

                //Sets up click events
                appointmentDetailsOfCancelAppointment.Click += (sender, e) => AppointmentDetailsOfCancelAppointment_Click(sender, e, appointmentId);
                appointmentDetailsSetRating.Click += (sender, e) => AppointmentDetailsSetRating_Click(sender, e, appointmentId, appointmentDetails.ratingId);
                appointmentDetailsSetReview.Click += (sender, e) => AppointmentDetailsSetReview_Click(sender, e, appointmentId, appointmentDetails.reviewId);
            }
            catch
            {
                //If internet is not working
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
            

        }

        private void AppointmentDetailsSetReview_Click(object sender, EventArgs e, int appointmentId, int reviewId)
        {
            //Goes into set review activity
            var activity = new Intent(this, typeof(Android_Application.Activities.reviewActivity));
            activity.PutExtra("appointmentId", appointmentId);
            activity.PutExtra("reviewId", reviewId);
            StartActivity(activity);
        }

        private void AppointmentDetailsSetRating_Click(object sender, EventArgs e, int appointmentId, int ratingId)
        {
            var activity = new Intent(this, typeof(Android_Application.Activities.RatingsActivity));
            activity.PutExtra("appointmentId", appointmentId);
            activity.PutExtra("ratingId", ratingId);
            StartActivity(activity);
            Finish();
            //throw new NotImplementedException();
        }

        private void AppointmentDetailsOfCancelAppointment_Click(object sender, EventArgs e, int id)
        {
            try
            {
                string url = "http://178.62.87.28:600/api/app/re" + Convert.ToString(id);
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.DELETE;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                client.Execute(request);
                Finish();
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
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