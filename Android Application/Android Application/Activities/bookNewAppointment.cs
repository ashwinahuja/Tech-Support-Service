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
    [Activity(Label = "bookNewAppointment")]
    public class bookNewAppointment : ListActivity
    {
        //Global variables used throughout the code
        public user[] listOfPeopleNearby;
        int currentUserId;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                //Gets parameters from the previous activity.
                base.OnCreate(savedInstanceState);
                currentUserId = Intent.GetIntExtra("currentUserId", 20);

                //Gets list of nearby people
                listOfPeopleNearby = getListOfNearbyUsers(currentUserId);
                int x = 0;
                try
                {
                    if (listOfPeopleNearby.Length < 100)
                        x = listOfPeopleNearby.Length;
                    else
                        x = 100;
                }
                //limits the view to 100 people
                catch
                {
                    Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                    StartActivity(typeof(MainActivity));
                }
                string[] thingsToShow = new string[x];
                for (int i = 0; i < x; i++)
                {
                    //For each person to display, show name and their average rating (to the nearest tenth)
                    string temp = listOfPeopleNearby[i].firstName + " " + listOfPeopleNearby[i].surname;
                    double averageRating = getAverageRating(listOfPeopleNearby[i].id);
                    if (!Double.IsNaN(averageRating))
                        temp += " (" + Convert.ToString(Math.Round(averageRating, 1)) + "/10)";
                    thingsToShow[i] = temp;
                }
                RequestWindowFeature(WindowFeatures.NoTitle);
                ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, thingsToShow);
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
        }
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            //If item is clicked

            base.OnListItemClick(l, v, position, id);
            user selected = listOfPeopleNearby[position];

            //Start activity of seeing more details about that person
            var newActivity = new Intent(this, typeof(helperDetails));
            newActivity.PutExtra("currentUserId", currentUserId);
            newActivity.PutExtra("helperId", selected.id);
            StartActivity(newActivity);
        }

        
        protected user[] getListOfNearbyUsers(int id)
        {
            //Make call to the WebAPI, and parse JSON input into a list of the 'user' object
            try
            {
                string url = "http://178.62.87.28:600/api/ach/gl" + Convert.ToString(id);
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.GET;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                var response = client.Execute(request);
                string result = response.Content;
                user[] toReturn = JsonConvert.DeserializeObject<user[]>(result);
                return toReturn;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
                user[] toReturn = new user[1];
                return toReturn;
            }
        }

        protected double getAverageRating(int id)
        {
            //Make call to the WebAPI, getting a list of the ratings. Then gets the average rating from this.
            try
            {
                string url = "http://178.62.87.28:600/api/ratings/ga" + Convert.ToString(id);
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.GET;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                var response = client.Execute(request);
                string result = response.Content;
                try
                {
                    rating[] toReturn = JsonConvert.DeserializeObject<rating[]>(result);
                    double x = 0;
                    for (int i = 0; i < toReturn.Length; i++)
                    {
                        x += toReturn[i].starRating;
                    }
                    x = x / toReturn.Length;
                    return x;
                }
                catch
                {
                    return 11;
                }
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
                return 0;
            }
            
        }
        private appointment[] returnListOfAppointments(int id, bool helper)
        {
            ////Make call to the WebAPI, and parse JSON input into a list of 'appointments' object
            try
            {
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
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
                appointment[] a = new appointment[1];
                return a;
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