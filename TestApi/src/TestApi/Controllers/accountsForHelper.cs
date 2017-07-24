using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestApi.Sql;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.IO;
using TestApi.Types;
namespace TestApi.Controllers
{
    /// <summary>
    /// AccountForHelper: All the functions for communicating with the helper database
    /// </summary>
    [Route("api/ach")]
    public class accountsForHelper : SQLStuffOnTopOfController
    {
        /// <summary>
        /// 
        /// Test function
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public string GetHelloWorld()
        {
            return "Hello World!";
        }
        /// <summary>
        /// Get a list of users
        /// </summary>
        /// <returns>
        /// List of all the users in the file of users
        /// Should soon be deprecated as this is definitely not safe and should not be used in a final project.
        /// </returns>
        internal List<user> Get()
        {

            string listOfUsers = sqlCommand(true, "SELECT * FROM accountsForHelpers", 13);
            List<user> listToReturn = new List<user>();
            string[] splitList = listOfUsers.Split('\n');
            //string c = splitList[0];

            for (int i = 0; i < splitList.Length - 1; i++)
            {
                user a = new user();
                string[] completelySplitStrings = splitList[i].Split('#');
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
                int l = 0;
                int.TryParse(completelySplitStrings[12], out l);
                a.timetableId = l;
                listToReturn.Add(a);
            }
            return listToReturn;
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
            string response = sqlCommand(true, "SELECT * FROM accountsForHelpers WHERE id=" + Convert.ToString(id) + ";", 13);
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
            a.timetableId = int.Parse(completelySplitStrings[12]);
            a.helper = true;

            return a;
        }

        /// <summary>
        /// Add a new user to the database if and only if there is no user with the same email address and / or telephone number
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [HttpPost("users/ad")]
        public bool AddNewUser([FromBody] user a)
        {
            int response = int.Parse(sqlCommand(true, "SELECT count(1) FROM  accountsForHelpers WHERE emailAddress = '" + a.emailAddress + "'").Split('#')[0]);
            int response2 = int.Parse(sqlCommand(true, "SELECT count(1) FROM accountsForHelpers WHERE telephoneNumber = '" + a.telephoneNumber + "'").Split('#')[0]);
            
            if (response == 0 && response2 == 0)
            {
                timetables t = new timetables(); // Gets a new timetable and associates it with the user
                timetable defaultTimetable = t.getTimetable(1); // The default timetable is timetable 1
                t.addTimetable(defaultTimetable, false); // Insert the new timetable
                int timetableId = int.Parse(sqlCommand(true, "SELECT max(id) FROM timetables", 1).Split('#')[0]); // and get the new timetablesId
                sqlCommand(false, "INSERT INTO accountsForHelpers (username, emailAddress, password, firstname, surname, firstlineOfAddress, secondlineOfAddress, telephoneNumber, postalCode, city, country, timetableId) VALUES ('" + a.username + "','" + a.emailAddress + "','" + a.password + "','" + a.firstName + "','" + a.surname + "','" + a.firstLineOfAddress + "','" + a.secondLineOfAddress + "','" + a.telephoneNumber + "','" + a.postalCode + "','" + a.city + "','" + a.country + "'," + timetableId + ");");
                return true;
            }
            else
                return false; // Returns a false if thre user has not been added
        }
        
        /// <summary>
        /// Remove the user based on the id of the user
        /// </summary>
        /// <param name="id">Integer id in the database</param>
        /// <returns></returns>
        [HttpDelete("users/rm{id}")]
        public bool Delete(int id)
        {
            try {
                sqlCommand(false, "DELETE FROM accountsForHelpers WHERE id=" + Convert.ToString(id) + ";");
                return true;
            }
            catch (Exception) { //If it doesn't exist
                return false;
            }

        }

        /// <summary>
        /// Update the user to a new user
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [HttpPut("users/up")]
        public bool Update([FromBody] user a)
        {
            int response = int.Parse(sqlCommand(true, "SELECT count(1) FROM  accountsForHelpers WHERE emailAddress = '" + a.emailAddress + "'").Split('#')[0]);
            if (response != 0)
                return false;

            sqlCommand(false, String.Format("UPDATE accountsForHelpers SET username='{0}',emailAddress='{1}',password='{2}',firstname='{3}',surname='{4}',firstlineOfAddress='{5}',secondlineOfAddress='{6}',telephoneNumber='{7}',postalCode='{8}',city='{9}',country='{10}' WHERE id={11}", a.username, a.emailAddress, a.password, a.firstName, a.surname, a.firstLineOfAddress, a.secondLineOfAddress, a.telephoneNumber, a.postalCode, a.city, a.country, a.id));
            return true;

        }


        /// <summary>
        /// Verify the user - returns an empty user if doesn't exist.
        /// Otherwise returns the contents of the new user.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        [HttpPost("users/vf")]
        public user verifyUser([FromBody] userToBeVerified b)
        {
            if (int.Parse(sqlCommand(true, "SELECT count(1) FROM accountsForHelpers WHERE emailAddress='" + b.emailAddress + "' AND password='" + b.password + "'", 1).Split('#')[0]) > 0)
            {
                string response = sqlCommand(true, "SELECT * FROM accountsForHelpers WHERE emailAddress='" + b.emailAddress + "';", 12);
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
                a.helper = true;
                return a;
            }
            else
                return new user();
        }

        /// <summary>
        /// Return the list of helpers who are able to help (ordered by how near they are to the user)
        /// </summary>
        /// <param name="idOfElderlyPerson"></param>
        /// <returns></returns>
        [HttpGet("gl{idOfElderlyPerson}")]
        public List<user> getListOfUsersByDistanceFromElderlyPerson(int idOfElderlyPerson = 4)
        {
            accountsForHelper ah = new accountsForHelper();
            accountsForElderly ae = new accountsForElderly();
            List<user> listOfAllElderly = ah.Get();
            user elderlyPerson = ae.GetInfo(idOfElderlyPerson);
            string address = elderlyPerson.postalCode; // Entirely rely upon the postcode since this is the most reliable.
            //string address = elderlyPerson.firstLineOfAddress + ", " + elderlyPerson.secondLineOfAddress;
            List<user> sortedList = mergeSort(listOfAllElderly, address);
            return sortedList;

        }
        public static List<user> mergeSort(List<user> listOfAllElderly, string address)
        {
            if (listOfAllElderly.Count() == 1) // Base Case
                return listOfAllElderly;
            List<user> firstHalf = new List<user>(); // Split the entirety of the list into a first half and second half
            List<user> secondHalf = new List<user>();
            for (int i = 0; i < listOfAllElderly.Count() / 2; i++) // Effectively copy until half way (or just before halfway)
            {
                firstHalf.Add(listOfAllElderly[0]); 
                listOfAllElderly.RemoveAt(0);
            }
            for (int j = 0; j < listOfAllElderly.Count(); j++) // Take the rest
            {
                secondHalf.Add(listOfAllElderly[j]);
            }
            return merge(mergeSort(firstHalf, address), mergeSort(secondHalf, address), address); // Recursively mergesort
        }
        public static List<user> merge(List<user> listOne, List<user> ListTwo, string address)
        {

            List<user> sortedListToReturn = new List<user>(); // Setup the new merged (sorted list)
            while(listOne.Count() != 0 || ListTwo.Count() != 0) // While there are objects in both lists
            {
                if (listOne.Count() <= 0) // If one list is empty
                {
                    sortedListToReturn.Add(ListTwo[0]);
                    ListTwo.RemoveAt(0);
                }
                else if (ListTwo.Count() <= 0) // If other list is empty
                {
                    sortedListToReturn.Add(listOne[0]);
                    listOne.RemoveAt(0);
                }
                    
                else
                {
                    accountsForHelper ah = new accountsForHelper();
                    accountsForElderly ae = new accountsForElderly();
                    string AddressOne = listOne[0].postalCode;
                    string AddressTwo = ListTwo[0].postalCode;
                    if (listOne[0].distance == double.MaxValue) // If distance hasn't been already found (defaults to max value of double)
                        listOne[0].distance =  ah.getDistance(address, AddressOne);
                    double distanceOne = listOne[0].distance;
                    if (ListTwo[0].distance == double.MaxValue)
                        ListTwo[0].distance = ah.getDistance(address, AddressTwo);
                    double distanceTwo = ListTwo[0].distance;
                    if (distanceOne <= distanceTwo) // Adds something to list and removes that from the other list depending on which distance is shorter.
                    {
                        sortedListToReturn.Add(listOne[0]);
                        listOne.RemoveAt(0);
                    }
                    else
                    {
                        sortedListToReturn.Add(ListTwo[0]);
                        ListTwo.RemoveAt(0);
                    }
                }

            }
            return sortedListToReturn;
        }

        /*
        Gets the distance between two points.
         */
        
        [HttpGet("users/gd")]
        public double getDistance(string origin, string destination) 
        {

            double distance = double.MaxValue; // In case of a failure of the API (especially if postcode is invalid)
            string url = "http://maps.googleapis.com/maps/api/directions/json?origin=" + origin + "&destination=" + destination + "&sensor=false"; // Gets value of the Google API
            string requesturl = url;
            string content = fileGetContents(requesturl);
            JObject o = JObject.Parse(content); // Parses the object
            try
            {
                distance = (int)o.SelectToken("routes[0].legs[0].distance.value"); // Gets the value of the distance.
                return distance / 1000;
            }
            catch
            {
                return distance;
            }
        }
        /*
        Completes the actual HTTP Request
         */
        protected static string fileGetContents(string fileName)
        {
            string sContents = string.Empty;
            string me = string.Empty;
            try
            {
                if (fileName.ToLower().IndexOf("http:") > -1)
                {
                    HttpClient wc = new HttpClient();
                    byte[] response = wc.GetByteArrayAsync(fileName).Result;
                    sContents = System.Text.Encoding.ASCII.GetString(response);

                }
                else
                {
                    
                }
            }
            catch { }
            return sContents;
        }



    }
}
