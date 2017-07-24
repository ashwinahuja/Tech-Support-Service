using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android_Application.Backend;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RestSharp;
using Newtonsoft.Json;
using Android_Application.Types;
using System.Threading.Tasks;

namespace Android_Application.Activities
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                //Sets up view
                base.OnCreate(savedInstanceState);
                RequestWindowFeature(WindowFeatures.NoTitle);
                SetContentView(Resource.Layout.register);

                //Defines items in view
                EditText registerFirstName = FindViewById<EditText>(Resource.Id.registerFirstName);
                EditText registerSurname = FindViewById<EditText>(Resource.Id.registerSurname);
                EditText registerUsername = FindViewById<EditText>(Resource.Id.registerUsername);
                EditText registerEmailAddress = FindViewById<EditText>(Resource.Id.registerEmailAddress);
                EditText registerPassword = FindViewById<EditText>(Resource.Id.registerPassword);
                EditText registerFirstLine = FindViewById<EditText>(Resource.Id.registerFirstLine);
                EditText registerSecondLine = FindViewById<EditText>(Resource.Id.registerSecondLine);
                EditText registerCity = FindViewById<EditText>(Resource.Id.registerCity);
                EditText registerCountry = FindViewById<EditText>(Resource.Id.registerCountry);
                EditText registerPostalCode = FindViewById<EditText>(Resource.Id.registerPostalCode);
                EditText registerTelephoneNumber = FindViewById<EditText>(Resource.Id.registerTelephoneNumber);
                Button registerRegisterAsHelper = FindViewById<Button>(Resource.Id.registerRegisterAsHelper);
                Button registerRegisterAsElderly = FindViewById<Button>(Resource.Id.registerRegisterAsElderly);
                // Create your application here

                //Sets up event handlers
                registerRegisterAsElderly.Click += RegisterRegisterAsElderly_Click;
                registerRegisterAsHelper.Click += RegisterRegisterAsHelper_Click;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }

        }

        private void RegisterRegisterAsHelper_Click(object sender, EventArgs e)
        {
            if (!checkValid()) // Check that the contents of the screen are valid (things are filled in)
            {
                Dialogs newDialog = new Dialogs("Registration Unsuccessful", "Not all required fields have been filled or your passwords do not match.", this);
                return;
            }
            EditText registerFirstName = FindViewById<EditText>(Resource.Id.registerFirstName);
            EditText registerSurname = FindViewById<EditText>(Resource.Id.registerSurname);
            EditText registerUsername = FindViewById<EditText>(Resource.Id.registerUsername);
            EditText registerEmailAddress = FindViewById<EditText>(Resource.Id.registerEmailAddress);
            EditText registerPassword = FindViewById<EditText>(Resource.Id.registerPassword);
            EditText registerFirstLine = FindViewById<EditText>(Resource.Id.registerFirstLine);
            EditText registerSecondLine = FindViewById<EditText>(Resource.Id.registerSecondLine);
            EditText registerCity = FindViewById<EditText>(Resource.Id.registerCity);
            EditText registerCountry = FindViewById<EditText>(Resource.Id.registerCountry);
            EditText registerPostalCode = FindViewById<EditText>(Resource.Id.registerPostalCode);
            EditText registerTelephoneNumber = FindViewById<EditText>(Resource.Id.registerTelephoneNumber);
            Button registerRegisterAsHelper = FindViewById<Button>(Resource.Id.registerRegisterAsHelper);
            Button registerRegisterAsElderly = FindViewById<Button>(Resource.Id.registerRegisterAsElderly);

            //Creates a user object and populates it
            user user = new user();
            user.city = registerCity.Text;
            user.country = registerCountry.Text;
            user.emailAddress = registerEmailAddress.Text;
            user.firstLineOfAddress = registerFirstLine.Text;
            user.secondLineOfAddress = registerSecondLine.Text;
            user.surname = registerSurname.Text;
            user.password = MD5Hash.MD5HashReturn(registerPassword.Text); //Password is MD5 hashed
            string telephoneNumber = registerTelephoneNumber.Text;

            if (telephoneNumber[0] == '0')
            {
                telephoneNumber = "44" + telephoneNumber.Substring(1); // phone numbers are stored as 44...
            }

            user.telephoneNumber = telephoneNumber;
            user.username = registerUsername.Text;
            user.postalCode = registerPostalCode.Text;
            user.firstName = registerFirstName.Text;
            user.helper = true;
            if (register(user)) // try and register user
            {
                //SUCCESS
                string returnFromVerificationProcess = string.Empty;
                TwilioRegistration newUser = new TwilioRegistration();
                //try to get a phone verification
                newUser.Number = telephoneNumber;
                newUser.friendlyName = registerFirstName.Text + " " + registerSurname.Text;
                returnFromVerificationProcess = verifyNumber(newUser); // verify number
                if (returnFromVerificationProcess.Length > 10) // indicative of failure
                {
                    Dialogs newDialog2 = new Dialogs("Registration Unsuccessful", "Phone number was invalid or has already been used for another account", this); 
                    user addedUser = testVerify(user.emailAddress, registerPassword.Text, false).Result; // effectively used to get the id of the user which has been added
                    deleteUser(addedUser.id, false); // then delete it
                }
                else
                {
                    Dialogs newDialog = new Dialogs("Registration Successful", "You should now respond to the incoming call with the following code: " + returnFromVerificationProcess, this);
                }
            }
            else
            {
                Dialogs newDialog = new Dialogs("Registration Unsuccessful", "There is already an account with that email address or phone number", this);
            }
            return;
            //
            //

        }
        private bool checkValid()
        {
            //Check if any important fields are empty
            //AND if the passwords do not match
            EditText registerFirstName = FindViewById<EditText>(Resource.Id.registerFirstName);
            EditText registerSurname = FindViewById<EditText>(Resource.Id.registerSurname);
            EditText registerUsername = FindViewById<EditText>(Resource.Id.registerUsername);
            EditText registerEmailAddress = FindViewById<EditText>(Resource.Id.registerEmailAddress);
            EditText registerPassword = FindViewById<EditText>(Resource.Id.registerPassword);
            EditText registerConfirmPassword = FindViewById<EditText>(Resource.Id.registerConfirmPassword);
            EditText registerFirstLine = FindViewById<EditText>(Resource.Id.registerFirstLine);
            EditText registerSecondLine = FindViewById<EditText>(Resource.Id.registerSecondLine);
            EditText registerCity = FindViewById<EditText>(Resource.Id.registerCity);
            EditText registerCountry = FindViewById<EditText>(Resource.Id.registerCountry);
            EditText registerPostalCode = FindViewById<EditText>(Resource.Id.registerPostalCode);

            EditText registerTelephoneNumber = FindViewById<EditText>(Resource.Id.registerTelephoneNumber);
            Button registerRegisterAsHelper = FindViewById<Button>(Resource.Id.registerRegisterAsHelper);
            Button registerRegisterAsElderly = FindViewById<Button>(Resource.Id.registerRegisterAsElderly);
            if (String.IsNullOrEmpty(registerFirstName.Text))
                return false;
            if (String.IsNullOrEmpty(registerSurname.Text))
                return false;
            if (String.IsNullOrEmpty(registerUsername.Text))
                return false;
            if (String.IsNullOrEmpty(registerEmailAddress.Text))
                return false;
            if (String.IsNullOrEmpty(registerPassword.Text))
                return false;
            if (String.IsNullOrEmpty(registerConfirmPassword.Text))
                return false;
            if (String.IsNullOrEmpty(registerFirstLine.Text))
                return false;
            if (String.IsNullOrEmpty(registerPostalCode.Text))
                return false;
            if (String.IsNullOrEmpty(registerCity.Text))
                return false;
            if (String.IsNullOrEmpty(registerTelephoneNumber.Text))
                return false;
            if (registerPassword.Text != registerConfirmPassword.Text)
                return false;
            return true;
        }


        private void RegisterRegisterAsElderly_Click(object sender, EventArgs e)
        {
            //Effectively the same as the register as helper click
            if (!checkValid())
            {
                Dialogs newDialog = new Dialogs("Registration Unsuccessful", "Not all required fields have been filled.", this);
                return;
            }
            EditText registerFirstName = FindViewById<EditText>(Resource.Id.registerFirstName);
            EditText registerSurname = FindViewById<EditText>(Resource.Id.registerSurname);
            EditText registerUsername = FindViewById<EditText>(Resource.Id.registerUsername);
            EditText registerEmailAddress = FindViewById<EditText>(Resource.Id.registerEmailAddress);
            EditText registerPassword = FindViewById<EditText>(Resource.Id.registerPassword);
            EditText registerFirstLine = FindViewById<EditText>(Resource.Id.registerFirstLine);
            EditText registerSecondLine = FindViewById<EditText>(Resource.Id.registerSecondLine);
            EditText registerCity = FindViewById<EditText>(Resource.Id.registerCity);
            EditText registerCountry = FindViewById<EditText>(Resource.Id.registerCountry);
            EditText registerPostalCode = FindViewById<EditText>(Resource.Id.registerPostalCode);
            EditText registerTelephoneNumber = FindViewById<EditText>(Resource.Id.registerTelephoneNumber);
            Button registerRegisterAsHelper = FindViewById<Button>(Resource.Id.registerRegisterAsHelper);
            Button registerRegisterAsElderly = FindViewById<Button>(Resource.Id.registerRegisterAsElderly);

            user user = new user();
            user.city = registerCity.Text;
            user.country = registerCountry.Text;
            user.emailAddress = registerEmailAddress.Text;
            user.firstLineOfAddress = registerFirstLine.Text;
            user.secondLineOfAddress = registerSecondLine.Text;
            user.surname = registerSurname.Text;
            string telephoneNumber = registerTelephoneNumber.Text;
            if (telephoneNumber[0] == '0')
            {
                telephoneNumber = "44" + telephoneNumber.Substring(1);
            }
            user.telephoneNumber = telephoneNumber;
            user.username = registerUsername.Text;
            user.postalCode = registerPostalCode.Text;
            user.firstName = registerFirstName.Text;
            user.password = MD5Hash.MD5HashReturn(registerPassword.Text);
            user.helper = false;
            if (register(user))
            {
                string returnFromVerificationProcess = string.Empty;
                TwilioRegistration newUser = new TwilioRegistration();
                newUser.Number = telephoneNumber;
                newUser.friendlyName = registerFirstName.Text + " " + registerSurname.Text;
                returnFromVerificationProcess = verifyNumber(newUser);
                if (returnFromVerificationProcess.Length > 10)
                {
                    Dialogs newDialog2 = new Dialogs("Registration Unsuccessful", "There is already an account with that phone number", this);
                    user addedUser = testVerify(user.emailAddress, registerPassword.Text, false).Result;
                    deleteUser(addedUser.id, false);
                }
                else
                {
                    Dialogs newDialog = new Dialogs("Registration Successful", "You should now respond to the incoming call with the following code: " + returnFromVerificationProcess, this);
                }
            }
            else
            {
                Dialogs newDialog = new Dialogs("Registration Unsuccessful", "There is already an account with that email address or phone number", this);
            }
            return;
        }

        private void deleteUser(int id, bool helper)
        {
            //Make call to the WebAPI to delete the user based on it's user id
            //URL differs based on whether trying to delete a helper or an elderly person
            try
            {
                string url = String.Empty;
                if (helper)
                    url = "http://178.62.87.28:600/api/ach/users/rm" + Convert.ToString(id);
                else
                    url = "http://178.62.87.28:600/api/ace/users/rm" + Convert.ToString(id);
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.DELETE;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                var response = client.Execute(request);
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
            }
        }
        private async Task<user> testVerify(string emailAddress, string password, bool helper)
        {
            //Effectively tries to login and returns the user...
            try
            {
                userToBeVerified user = new userToBeVerified();
                user.emailAddress = emailAddress;
                user.password = MD5Hash.MD5HashReturn(password);
                //user.password = password;
                user.helper = helper;
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
        private bool register(user user)
        {
            //Makes a POST call to the WebAPI and therefore attempts to register the user.
            try
            {
                String data = JsonConvert.SerializeObject(user);
                var client = new RestClient("http://178.62.87.28:600/api/ac/users/ad");
                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                request.AddParameter("application/json", data, ParameterType.RequestBody);
                var response = client.Execute(request);
                string result = response.Content;
                return bool.Parse(result);
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
                return false;
            }
        }

        private string verifyNumber(TwilioRegistration user)
        {
            //Verifies the number by making a POST call to the SMS server
            try
            {
                String data = JsonConvert.SerializeObject(user);
                var client = new RestClient("http://178.62.87.28:700/verifyNumber");
                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                request.AddParameter("application/json", data, ParameterType.RequestBody);
                var response = client.Execute(request);
                string result = response.Content;
                return result;
            }
            catch
            {
                Toast.MakeText(BaseContext, "Ensure that you are connected to the internet", ToastLength.Short).Show();
                StartActivity(typeof(MainActivity));
                return String.Empty;
            }
        }
    }
}