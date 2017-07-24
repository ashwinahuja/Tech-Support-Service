using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Net.Http;
using System.Threading.Tasks;
using RestSharp;
using Android_Application.Types;
using System.IO;
using Android_Application.Backend;
using SQLite;
using SQLitePCL;


namespace Android_Application.Activities
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        //Database info
        string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "dbUser.db3");
        SQLiteConnection db;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Setup view
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.login);

            //setup view parts
            Button loginLoginAsHelper = FindViewById<Button>(Resource.Id.loginLoginAsHelper);
            Button loginLoginAsElderly = FindViewById<Button>(Resource.Id.loginLoginAsElderly);
            EditText loginEmailAddress = FindViewById<EditText>(Resource.Id.loginEmailAddress);
            EditText loginPassword = FindViewById<EditText>(Resource.Id.loginPassword);

            //setup event handlers
            loginLoginAsHelper.Click += LoginLoginAsHelper_Click;
            loginLoginAsElderly.Click += LoginLoginAsElderly_Click;

            //Check if there are any existing logins and passwords.
            //If there are, then try and login with them.
            db = new SQLiteConnection(dbPath);
            try
            {
                var table = db.Table<SQLLiteLogin>();
                foreach(var item in table)
                {
                    user x = login(item.emailAddress, item.password, item.helper).Result;
                    //Try and login and go onto correct activities if login is successful
                    if (x.id != 0)
                    {
                        if (!item.helper)
                        { 
                            var newActivity = new Intent(this, typeof(welcomeElderly));
                            newActivity.PutExtra("currentUserId", x.id);
                            StartActivity(newActivity);
                        }
                        else
                        {
                            var newActivity = new Intent(this, typeof(welcomeHelper));
                            newActivity.PutExtra("currentUserId", x.id);
                            StartActivity(newActivity);
                        }
                    }
                }
            }
            catch
            {
                //If no table already, create one.
                db.CreateTable<SQLLiteLogin>();
            }
        }

        private void LoginLoginAsElderly_Click(object sender, EventArgs e)
        {
            //If login
            try
            {
                //Access correct views
                EditText eA = FindViewById<EditText>(Resource.Id.loginEmailAddress);
                EditText p = FindViewById<EditText>(Resource.Id.loginPassword);

                //Checks not empty
                if (checkIfEmpty(eA, p))
                    return;

                //Try and login
                user x = login(eA.Text, p.Text, false).Result;

                bool successfulLogin;
                if (x.id == 0)
                    successfulLogin = false;
                else
                    successfulLogin = true;
                if (successfulLogin)
                {
                    //Dialog box saying successful login
                    Dialogs newDialog = new Dialogs("Login Success", "You've successfully logged in", this);
                    SQLLiteLogin c = new SQLLiteLogin();
                    c.emailAddress = eA.Text;
                    c.password = p.Text;
                    c.helper = false;
                    db.Insert(c);
                    //Inserts the login into the database for a successful auto login next time
                    //Starts the correct activity
                    var newActivity = new Intent(this, typeof(welcomeElderly));
                    newActivity.PutExtra("currentUserId", x.id);
                    StartActivity(newActivity);
                    
                }
                else
                {
                    Dialogs newDialog = new Dialogs("Login Failed", "You've got your email address or password wrong.", this);
                }
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
        }

        private void LoginLoginAsHelper_Click(object sender, EventArgs e)
        {
            // Effectively the same as for helper.
            try
            {
                EditText eA = FindViewById<EditText>(Resource.Id.loginEmailAddress);
                EditText p = FindViewById<EditText>(Resource.Id.loginPassword);
                if (checkIfEmpty(eA, p))
                    return;
                user x = login(eA.Text, p.Text, true).Result;
                bool successfulLogin;
                if (x.id == 0)
                    successfulLogin = false;
                else
                    successfulLogin = true;
                if (successfulLogin)
                {
                    Dialogs newDialog = new Dialogs("Login Success", "You've successfully logged in", this);
                    
                    SQLLiteLogin c = new SQLLiteLogin();
                    c.emailAddress = eA.Text;
                    c.password = p.Text;
                    c.helper = true;
                    db.Insert(c);
                    var newActivity = new Intent(this, typeof(welcomeHelper));
                    newActivity.PutExtra("currentUserId", x.id);
                    StartActivity(newActivity);
                }
                else
                {
                    Dialogs newDialog = new Dialogs("Login Failed", "You've got your email address or password wrong", this);
                }
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
        }

        private bool checkIfEmpty(EditText eA, EditText p)
        {
            //Check if things are valid
            if (String.IsNullOrEmpty(eA.Text) || String.IsNullOrEmpty(p.Text))
            {
                //Dialog created
                AlertDialog.Builder a = new AlertDialog.Builder(this);
                a.SetTitle("Login Failed");
                a.SetMessage("You have not filled in all required information.");
                Dialog dialog = a.Create();
                dialog.Show();
                return true;
            }
            else
                return false;
            
        }

        private async Task<user> login(string emailAddress, string password, bool helper)
        {
            //Try and login with provided information
            try
            {
                userToBeVerified user = new userToBeVerified(); // create usertobeverified object
                user.emailAddress = emailAddress;
                user.password = MD5Hash.MD5HashReturn(password); // MD5 hash password
                user.helper = helper;

                //Serialized object, POSTS to WebAPI and deserialises input into a user object
                String data = JsonConvert.SerializeObject(user);
                var client = new RestClient("http://178.62.87.28:600/api/ac/users/vf");
                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                request.AddParameter("application/json", data, ParameterType.RequestBody);
                var response = client.Execute(request);
                string result = response.Content;
                user userreturned = JsonConvert.DeserializeObject<user>(result);
                return userreturned;
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