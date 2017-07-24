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

using Newtonsoft.Json;
using RestSharp;
using Android_Application.Types;

namespace Android_Application.Activities
{
    [Activity(Label = "SeeAllReviewOfHelper")]
    public class SeeAllReviewOfHelper : ListActivity
    {
        //Global variables
        int helperId;
        review[] listOfReviews;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //get parameters from previous activity
            base.OnCreate(savedInstanceState);
            helperId = Intent.GetIntExtra("helperId", 8);

            
            RequestWindowFeature(WindowFeatures.NoTitle);

            //populate screen
            listOfReviews = getListOfReviews(helperId);
            try
            {
                string[] toDisplay = new string[listOfReviews.Length];
                for (int i = 0; i < listOfReviews.Length; i++)
                {
                    toDisplay[i] = listOfReviews[i].comment;
                }
                ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, toDisplay);
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
            // Create your application here

        }
        private review[] getListOfReviews(int id)
        {
            //Make call to the WebAPI, and parse JSON input into an array of 'review' object
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
                review[] toReturn = JsonConvert.DeserializeObject<review[]>(result);
                return toReturn;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
                review[] a = new review[1];
                return a;
            }
        }
    }
}