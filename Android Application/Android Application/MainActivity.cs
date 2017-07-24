using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android_Application.Activities; // Allows us to call the other activities
using System.IO;
using RestSharp;
using SQLite;
using Android_Application.Types;
namespace Android_Application
{
    [Activity(Label = "Tech Support", MainLauncher = true, Icon = "@drawable/icon")] // Defines the name and icon of the app
    public class MainActivity : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            RequestWindowFeature(WindowFeatures.NoTitle); 
            SetContentView(Resource.Layout.loginOrRegister); // Selects the correct layout
            
            // Get our button from the layout resource,
            // and attach an event to it
            Button loginOrRegisterLogin = FindViewById<Button>(Resource.Id.loginOrRegisterLogin);
            loginOrRegisterLogin.Click += LoginOrRegisterLogin_Click;
            

            Button loginOrRegisterRegister = FindViewById<Button>(Resource.Id.loginOrRegisterRegister);
            loginOrRegisterRegister.Click += loginOrRegisterRegister_Click;

        }

        private void LoginOrRegisterLogin_Click(object sender, EventArgs e) //When you click login button
        {
            StartActivity(typeof(Activities.LoginActivity)); // Starts login
        }

        private void loginOrRegisterRegister_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(Android_Application.Activities.RegisterActivity)); // Starts register
        }
    }
}

