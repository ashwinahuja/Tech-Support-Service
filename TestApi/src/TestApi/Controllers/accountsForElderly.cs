using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestApi.Sql;
using TestApi.Types;
namespace TestApi.Controllers
{

    /// <summary>
    /// This is one of the paths of the accounts for elderly people
    /// Called through /api/ace
    /// </summary>
    [Route("api/ace")]
    public class accountsForElderly : SQLStuffOnTopOfController
    {
        /// <summary>
        /// Used as a test for the api if necessary.
        /// 
        /// </summary>
        /// <returns>"Hello World"</returns>
        [HttpGet("")]
        public string GetHelloWorld()
        {
            return "Hello World!";
        }

        /// <summary>
        /// Get user info by integer id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// User with all associated properties
        /// </returns>
        [HttpGet("users/gi{id}")]
        public user GetInfo(int id)
        {
            string response = sqlCommand(true, "SELECT * FROM accountsForElderly WHERE id=" + Convert.ToString(id)+";", 12);
            user a = new user();
            string[] completelySplitStrings = response.Split('#');
            a.id = int.Parse(completelySplitStrings[0]);
            a.username = completelySplitStrings[1];
            a.emailAddress = completelySplitStrings[2];
            a.password = completelySplitStrings[3];
            a.firstName = completelySplitStrings[4];
            a.surname = completelySplitStrings[5];
            a.firstLineOfAddress = completelySplitStrings[6];
            a.secondLineOfAddress = completelySplitStrings[7];
            a.telephoneNumber = completelySplitStrings[8];
            a.postalCode = completelySplitStrings[9];
            a.city = completelySplitStrings[10];
            a.country = completelySplitStrings[11];
            return a;
        }
        /// <summary>
        /// Returns the user of a particular phone number
        /// Only called internally by the appointment calling system when an SMS is sent
        /// </summary>
        /// <param name="phoneNumber">Correct user is sent or an empty user if phone number not found</param>
        /// <returns></returns>
        public user getInfoByPhoneNumber(string phoneNumber)
        {
            string response = sqlCommand(true, "SELECT * FROM accountsForElderly WHERE telephoneNumber=" + phoneNumber + ";", 12);
            user a = new user();
            try
            {
                string[] completelySplitStrings = response.Split('#');
                a.id = int.Parse(completelySplitStrings[0]);
                a.username = completelySplitStrings[1];
                a.emailAddress = completelySplitStrings[2];
                a.password = completelySplitStrings[3];
                a.firstName = completelySplitStrings[4];
                a.surname = completelySplitStrings[5];
                a.firstLineOfAddress = completelySplitStrings[6];
                a.secondLineOfAddress = completelySplitStrings[7];
                a.telephoneNumber = completelySplitStrings[8];
                a.postalCode = completelySplitStrings[9];
                a.city = completelySplitStrings[10];
                a.country = completelySplitStrings[11];
            }
            catch(Exception) {}
            return a;
        }

        /// <summary>
        /// Add new user to database
        /// </summary>
        /// <param name="a">New user to be added</param>
        /// <returns>Returns a boolean value as to whether the user has been successfully added,</returns>
        [HttpPost("users/ad")]
        public bool AddNewUser([FromBody] user a)
        {
            int response = int.Parse(sqlCommand(true, "SELECT count(1) FROM  accountsForElderly WHERE emailAddress = '" + a.emailAddress + "'").Split('#')[0]); // Checks if the email address already exists or not
            int response2 = int.Parse(sqlCommand(true, "SELECT count(1) FROM accountsForElderly WHERE telephoneNumber = '" + a.telephoneNumber + "'").Split('#')[0]); // Checks if the phone number already exists or not.
            //Console.WriteLine(b);
            if (response == 0 && response2 == 0)
            {
                sqlCommand(false, "INSERT INTO accountsForElderly (username, emailAddress, password, firstname, surname, firstlineOfAddress, secondlineOfAddress, telephoneNumber, postalCode, city, country) VALUES ('"+a.username + "','" + a.emailAddress + "','" + a.password + "','" + a.firstName + "','" + a.surname + "','" + a.firstLineOfAddress + "','" + a.secondLineOfAddress + "','" + a.telephoneNumber + "','" + a.postalCode + "','" + a.city + "','" + a.country + "');");
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Delete a user from the database based on their id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("users/rm{id}")]
        public bool Delete(int id)
        {
            try{
                sqlCommand(false, "DELETE FROM accountsForElderly WHERE id=" + Convert.ToString(id)+";");
                return true;
                        }            
            catch(Exception){
                return false;
            }
            
        }

        /// <summary>
        /// Update the user changing it to the contents of the user whose json is sent to the api.
        /// </summary>
        /// <param name="a">User to be updated</param>
        /// <returns>Boolean according to whether it was successful or not</returns>
        [HttpPut("users/up")]
        public bool Update([FromBody] user a)
        {
            int response = int.Parse(sqlCommand(true, "SELECT count(1) FROM  accountsForElderly WHERE id = " + a.id + ";").Split('#')[0]);
            if(response==0)
                return false; // returns a false if there is no id

            sqlCommand(false, String.Format("UPDATE accountsForElderly SET username='{0}',emailAddress='{1}',password='{2}',firstname='{3}',surname='{4}',firstlineOfAddress='{5}',secondlineOfAddress='{6}',telephoneNumber='{7}',postalCode='{8}',city='{9}',country='{10}' WHERE id={11}",a.username, a.emailAddress, a.password, a.firstName, a.surname, a.firstLineOfAddress, a.secondLineOfAddress,a.telephoneNumber, a.postalCode, a.city, a.country, a.id));
                return true;
        }
        
        /// <summary>
        /// Verify User
        ///     Returns a user
        ///         Empty if the login was unsuccessful
        ///         Otherwise populated with the contents of the user if it was successful
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        [HttpPost("users/vf")]
        public user verifyUser([FromBody] userToBeVerified b)
        {
            string output = ("SELECT count(1) FROM accountsForElderly WHERE emailAddress='" + b.emailAddress + "' AND password='" + b.password + "'").Split('#')[0];
            
            if (int.Parse(sqlCommand(true, "SELECT count(1) FROM accountsForElderly WHERE emailAddress='" + b.emailAddress + "' AND password='" + b.password + "'", 1).Split('#')[0]) > 0)
            {
                string response = sqlCommand(true, "SELECT * FROM accountsForElderly WHERE emailAddress='" + b.emailAddress + "';", 12);
                user a = new user();
                string[] completelySplitStrings = response.Split('#');
                a.id = int.Parse(completelySplitStrings[0]);
                a.username = completelySplitStrings[1];
                a.emailAddress = completelySplitStrings[2];
                a.password = completelySplitStrings[3];
                a.firstName = completelySplitStrings[4];
                a.surname = completelySplitStrings[5];
                a.firstLineOfAddress = completelySplitStrings[6];
                a.secondLineOfAddress = completelySplitStrings[7];
                a.telephoneNumber = completelySplitStrings[8];
                a.postalCode = completelySplitStrings[9];
                a.city = completelySplitStrings[10];
                a.country = completelySplitStrings[11];
                a.helper = false;
                return a;
            }
            else
                return new user() ;


        }
        
        
    }
}
