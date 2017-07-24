using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySql.Data.Common;
using MySql.Data.Types;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using TestApi.Backend;
using TestApi.Sql;
using TestApi.Types;
using System.Text;

namespace TestApi.Controllers
{

    [Route("api/app")]
    public class appointments : SQLStuffOnTopOfController
    {
       /// <summary>
       /// Responds to an SMS request - finds the nearest person who is available at the requested time.
       /// </summary>
       /// <param name="c">String of the contents received by the SMS server</param>
       /// <returns></returns>
       [HttpPostAttribute("sms{c}")]
       public bool createNewAppointmentsFromSMSRequest(string c)
       {
            //Parses the string into an SMS object
            string[] split = c.Split(',');
            sms a = new sms();
            a.From = split[0];
            a.Body = split[1];
            //Gets the date and time from the message
            DateTime fromTheMessage = new DateTime();
            bool foundTime = true;
            bool goneInto = false;
            if (a.Body.Contains("today") || a.Body.Contains("Today")) // If the message contains today in either capitalised or non capitalised format
            {
                goneInto = true;
                foundTime = a.Body.TryParseTime(DateTimeRoutines.DateTimeFormat.UK_DATE, out fromTheMessage); // Get the time
                DateTime newDt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, fromTheMessage.Hour, fromTheMessage.Minute, fromTheMessage.Second); // Create new object with today's date but the asked for time
                fromTheMessage = newDt; // Makes the from the message this new datetime
            }
            if (a.Body.Contains("tomorrow") || a.Body.Contains("Tomorrow"))
            {
                goneInto = true;
                foundTime = a.Body.TryParseTime(DateTimeRoutines.DateTimeFormat.UK_DATE, out fromTheMessage);
                DateTime newDt = new DateTime(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day, fromTheMessage.Hour, fromTheMessage.Minute, fromTheMessage.Second);
                fromTheMessage = newDt;
            }
            if (!goneInto)
            {
                fromTheMessage = getDateTimeTest(a.Body); // Otherwise, if no today or tomorrow, go into the main test of finding date and time.
            }
            if(fromTheMessage.Year == 1 || !foundTime || fromTheMessage < DateTime.Now) // Doesn't continue if appointment in the past, or datetime not found or no time in today
            {
                a.Body = "You failed to create an appointment as your message text was not of the right type or you asked to create an appointment in the past. Ensure the time is in the format of HH:MM."; // sends error message as SMS
                sendSMS(a.From, a.Body);
                return false;
            }
            else
            {
                accountsForHelper ah = new accountsForHelper();
                accountsForElderly ae = new accountsForElderly();
                List<user> allElderlyUsers = ah.getListOfUsersByDistanceFromElderlyPerson(ae.getInfoByPhoneNumber(a.From).id); // Gets list of nearby helpers based on the phone number
                bool foundAFreeUser = false;
                while(!foundAFreeUser) // While no appointment can be created
                {
                    foreach (user nextNearest in allElderlyUsers)
                    {
                        timetables ttt = new timetables();
                        timetable t = ttt.getTimetable(nextNearest.timetableId); // GetTimetable of the particular user
                        DateTime dt = fromTheMessage;
                        while(dt.DayOfWeek != DayOfWeek.Monday) { dt = dt.AddDays(-1); } // Gets the date to the date of the weekBeginning
                        int hour = fromTheMessage.Hour;
                        hour = hour / 2;
                        hour = hour * 2; // effectively rounds to the nearest two hour mark.
                        int nextHour = hour + 2;
                        string weekBeginning = dt.ToString("dd/MM/yyyy"); // Finds the stringValue of the weekBeginning
                        bool found = false;
                        week correctWeek = new week(); 
                        for (int i = 0; i < t.weeks.Count(); i++) // Tries to find if the week is in the custom weeks of the timetable or if it is in the default.
                        {
                            if (t.weeks[i].weekBeginning.ToString("dd/MM/yyyy") == weekBeginning) // This avoids issues in attempting to equate DateTimes.
                            {
                                found = true; 
                                correctWeek = t.weeks[i].week;
                            }
                        }
                        if(!found)
                        {
                            correctWeek = t.defaultWeek; // sets the week to focus on as the default week
                        }

                        day correctDay = new day();
                        //GOT THE CORRECT WEEK
                        switch(fromTheMessage.DayOfWeek) // depending on the day of the week, focuses on the correct day.
                        {
                            case DayOfWeek.Monday:
                                correctDay = correctWeek.monday;
                                break;
                            case DayOfWeek.Tuesday:
                                correctDay = correctWeek.tuesday;
                                break;
                            case DayOfWeek.Wednesday:
                                correctDay = correctWeek.wednesday;
                                break;
                            case DayOfWeek.Thursday:
                                correctDay = correctWeek.thursday;
                                break;
                            case DayOfWeek.Friday:
                                correctDay = correctWeek.friday;
                                break;
                            case DayOfWeek.Saturday:
                                correctDay = correctWeek.saturday;
                                break;
                            case DayOfWeek.Sunday:
                                correctDay = correctWeek.sunday;
                                break;
                            default:
                                correctDay = correctWeek.monday;
                                break;
                        }
                        int index = hour / 2; // WHich part of the timesFree array to check
                        if(correctDay.timesFree[index])
                        {
                            foundAFreeUser = true; // Found a user!! Populates an appointment and creates it.
                            appointment app = new appointment();
                            app.helperId = nextNearest.id;
                            app.elderlyId = ae.getInfoByPhoneNumber(a.From).id;
                            app.dateAndTime = fromTheMessage;
                            app.dateCreated = DateTime.Now;
                            user helper = ah.GetInfo(app.helperId); // Gets the helper details for the SMS message
                            createNewAppointmentsForAndroidApp(app);
                            sendSMS(a.From, "You have an appointment with " + helper.firstName + " " + helper.surname + " at " + app.dateAndTime.ToString("H:mm") + " on " + app.dateAndTime.ToString("MMMM dd, yyyy"));
                            foundAFreeUser = true;
                        }
                        if (foundAFreeUser)
                            break;
                    }
                    if (!foundAFreeUser)
                    {
                        sendSMS(a.From, "Nobody could be found."); // If gone through everyone and noone available, then sends a message saying noone could be found.                        return false;
                    }
                }
            }
            return true;
       }
        [HttpPut("sendSMS")]
        public bool sendSMS3([FromBody] sms s) // Since the sendSMS2 is async, it can't be called with a result of a boolean.
            //Therefore, this completes s, but returning a boolean conversion of the result.
        {
            if (sendSMS2(s).Result)
                return true;
            else
                return false;
        }
        private async Task<bool> sendSMS2( sms s)
        {
            try // In case of server issues, the catch is there.
            {
                string json = JsonConvert.SerializeObject(s);
                string url = "http://178.62.87.28:700/sendSms";
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                using (var httpClient = new HttpClient())
                {
                    var httpResponse = await httpClient.PostAsync(url, httpContent);

                    if (httpResponse.Content != null) // Should be a response of "SUCCESS" but we can ignore this really
                        //This is here incase we want to do more stuff in the future with the response.
                    {
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
        public async void sendSMS(string from, string body) // Effectively the same, but used within this solution, rather than by the API.
            // Therefore kept seperate to ensure that one doesn't break the other.
        {
            sms s = new sms();
            s.Body = body;
            s.From = from;
            string json = JsonConvert.SerializeObject(s);
            string url = "http://178.62.87.28:700/sendSms";
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.PostAsync(url, httpContent);

                if (httpResponse.Content != null)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                }
            }
        }

        /// <summary>
        /// Gets the datetime from a string, calling the DateTimeRoutines class.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
       [HttpGet("tdt{input}")]
       public DateTime getDateTimeTest(string input)
       {
            DateTime dt;
            input.TryParseDateOrTime(DateTimeRoutines.DateTimeFormat.USA_DATE, out dt);
            if (dt.Year == 1)
                input.TryParseDateOrTime(DateTimeRoutines.DateTimeFormat.UK_DATE, out dt);
            return dt;
       }
    


        /// <summary>
        /// Creates an appointment given the complete time and date and details of who the appointment is with
        /// Only occurs when the system knows the appointment is available.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="recurring">If a new column needs to be created, it is, then the entire process is repeated. However, the appointment shouldn't be readded to the database.</param>
        /// <returns></returns>
       [HttpPost("an")]
       public bool createNewAppointmentsForAndroidApp([FromBody] Types.appointment a, bool recurring = false)
       {
            
            if(!recurring)
            { 
                string test = "INSERT INTO appointments (helperId, elderlyId, dateAndTime, dateAndTimeCreated) VALUES (" + a.helperId + "," + a.elderlyId + ",'" + a.dateAndTime.ToString("yyyy-MM-dd H:mm:ss") + "','" + a.dateCreated.ToString("yyyy-MM-dd H:mm:ss") + "');";
                sqlCommand(false, test); // Insert the appointment into the SQL database.
            }
            string test2 = sqlCommand(true, "SELECT max(id) FROM appointments;", 1); // Gets the id of the appointment
            a.id = int.Parse(test2.Split('#')[0]);
            appointment b = getAppointmentInformation(a.id); // ensures it works with a complete appointment
            accountsForElderly ae = new accountsForElderly();
            accountsForHelper ah = new accountsForHelper();
            timetables ttt = new timetables();
            user helper = ah.GetInfo(a.helperId);
            user elderlyPerson = ae.GetInfo(a.elderlyId);

            timetable tt = ttt.getTimetable(helper.timetableId);
            //Check if the week already exists, if it doesn't then it needs to be added.
            //Get the week beginning of the appointment
            DateTime dt = a.dateAndTime;
            while (dt.DayOfWeek != DayOfWeek.Monday) { dt = dt.AddDays(-1); }
            string toCheck = dt.ToString("dd/MM/yyyy");

            bool found = false;
            week correctWeek = new week(); ;
            for (int i = 0; i < tt.weeks.Count(); i++)
            {
                if (tt.weeks[i].weekBeginning.ToString("dd/MM/yyyy") == toCheck)
                {
                    found = true;
                    correctWeek = tt.weeks[i].week;
                }
            }
            if(found) // We already have a custom week with this week
            {
                int weekId = correctWeek.id;
                day correctDay = new day();
                //GOT THE CORRECT WEEK, now let's get the correct day
                switch (a.dateAndTime.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        correctDay = correctWeek.monday;
                        break;
                    case DayOfWeek.Tuesday:
                        correctDay = correctWeek.tuesday;
                        break;
                    case DayOfWeek.Wednesday:
                        correctDay = correctWeek.wednesday;
                        break;
                    case DayOfWeek.Thursday:
                        correctDay = correctWeek.thursday;
                        break;
                    case DayOfWeek.Friday:
                        correctDay = correctWeek.friday;
                        break;
                    case DayOfWeek.Saturday:
                        correctDay = correctWeek.saturday;
                        break;
                    case DayOfWeek.Sunday:
                        correctDay = correctWeek.sunday;
                        break;
                    default:
                        correctDay = correctWeek.monday;
                        break;
                }
                int dayId = correctDay.id;
                int indexOfWhichThingToUpdate = a.dateAndTime.Hour / 2; // Which member of the timesFree array.
                day dayToMakeIt = correctDay; // Gets the correct day
                dayToMakeIt.timesFree[indexOfWhichThingToUpdate] = false; // changes the value.
                int newId = timetables.addDay(dayToMakeIt); // Doesn't update day, instead adds a new day
                correctDay = dayToMakeIt; // Changes the day in the week.
                correctDay.id = newId; // Changes the week id.
                switch (a.dateAndTime.DayOfWeek) // updates the week
                {
                    case DayOfWeek.Monday:
                        correctWeek.monday = correctDay;
                        break;
                    case DayOfWeek.Tuesday:
                        correctWeek.tuesday = correctDay;
                        break;
                    case DayOfWeek.Wednesday:
                        correctWeek.wednesday = correctDay;
                        break;
                    case DayOfWeek.Thursday:
                        correctWeek.thursday = correctDay;
                        break;
                    case DayOfWeek.Friday:
                        correctWeek.friday = correctDay;
                        break;
                    case DayOfWeek.Saturday:
                        correctWeek.saturday = correctDay;
                        break;
                    case DayOfWeek.Sunday:
                        correctWeek.sunday = correctDay;
                        break;
                    default:
                        correctWeek.monday = correctDay;
                        break;
                }
                timetables.updateWeek(correctWeek);
                return true;
                
            }
            else // must create a new column of the database and populate it.
            {
                sqlCommand(false, "ALTER TABLE timetables ADD " + toCheck.Replace('/','_') + " INT NULL;"); // creates a new column
                // toCheck is the weekBeginning eg 01/01/2016 - however the name of the columns are 01_01_2016 etc.
                string beenReturn = sqlCommand(true, "SELECT id, defaultWeek FROM timetables", 2); // get's all the columns of timetable
                string[] havingBeenSplit = beenReturn.Split('\n');
                for(int i = 0; i < havingBeenSplit.Length - 1; i++)
                {
                    string[] havingBeenSplitMore = havingBeenSplit[i].Split('#');
                    int idOfUser = int.Parse(havingBeenSplitMore[0]);
                    int defaultWeekIdOfUser = int.Parse(havingBeenSplitMore[1]);
                    week newWeek = timetables.getWeek2(defaultWeekIdOfUser); // get the default week from the database.
                    int newId = timetables.addWeek(newWeek); // Adds a copy of the default week to the database.
                    sqlCommand(false, "UPDATE timetables SET " + toCheck.Replace('/', '_') + " = " + newId + " WHERE id = " + idOfUser); // Updates the timetable
                }
                week weekToInsert = tt.defaultWeek;
                int id = timetables.addWeek(weekToInsert);
                sqlCommand(false, "UPDATE timetables SET " + toCheck.Replace('/','_') + " = " + id + " WHERE id = " + tt.id);
                return createNewAppointmentsForAndroidApp(a, true); // Reccurs on the same process but this time ensuring that the appointment is not readded to the database of appointments
            } 
        }

        /// <summary>
        /// Gets the appointment information
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
       [HttpGetAttribute("gi{id}")]
       public  Types.appointment getAppointmentInformation(int id)
       {
            string response = sqlCommand(true,"SELECT id, helperId, elderlyId, dateAndTime, dateAndTimeCreated, ratingId, reviewId FROM appointments WHERE id = '" + Convert.ToString(id) + "'", 7);
            appointment appointmentToReturn = new appointment();
            string[] responseSplit = response.Split('#');
            appointmentToReturn.id = Convert.ToInt32(responseSplit[0]);
            appointmentToReturn.helperId = Convert.ToInt32(responseSplit[1]);
            appointmentToReturn.elderlyId = Convert.ToInt32(responseSplit[2]);
            appointmentToReturn.dateAndTime = DateTime.Parse(responseSplit[3]);
            appointmentToReturn.dateCreated = DateTime.Parse(responseSplit[4]);
            int t1 = new int();
            int t2 = new int();
            int.TryParse(responseSplit[5], out t1) ;
            int.TryParse(responseSplit[6], out t2);
            appointmentToReturn.ratingId = t1;
            appointmentToReturn.reviewId = t2;
            return appointmentToReturn;
       }
        
        /// <summary>
        /// Deletes appointment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
       [HttpDeleteAttribute("re{id}")]
       public bool removeAppointment(int id)
       {
            sqlCommand(false, "DELETE FROM appointments WHERE id = " + Convert.ToString(id));
            return true;
       }

        /// <summary>
        /// Gets all the appointments of an elderly person
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
       [HttpGetAttribute("gAE{id}")]
       public List<appointment> appointmentsByElderly(int id)
       {
            string response = sqlCommand(true, "SELECT id, helperId, elderlyId, dateAndTime, dateAndTimeCreated, ratingId, reviewId FROM appointments WHERE elderlyId = '" + Convert.ToString(id) + "'", 7);
            string[] lines = response.Split('\n');
            List<appointment> listToReturn = new List<appointment>();
            for (int i = 0; i < lines.Length - 1; i++)
            {
                string line = lines[i];
                string[] responseSplit = line.Split('#');
                appointment appointmentToReturn = new appointment();
                appointmentToReturn.id = Convert.ToInt32(responseSplit[0]);
                appointmentToReturn.helperId = Convert.ToInt32(responseSplit[1]);
                appointmentToReturn.elderlyId = Convert.ToInt32(responseSplit[2]);
                appointmentToReturn.dateAndTime = DateTime.Parse(responseSplit[3]);
                appointmentToReturn.dateCreated = DateTime.Parse(responseSplit[4]);
                appointmentToReturn.ratingId = Convert.ToInt32(responseSplit[5]);
                appointmentToReturn.reviewId = Convert.ToInt32(responseSplit[6]);
                listToReturn.Add(appointmentToReturn);
            }
            return listToReturn;
       }

        /// <summary>
        /// Gets all the appointments of a helper
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
       [HttpGetAttribute("gAH{id}")]
       public List<appointment> appointmentsByHelper(int id)
       {
            string response = sqlCommand(true, "SELECT id, helperId, elderlyId, dateAndTime, dateAndTimeCreated, ratingId, reviewId FROM appointments WHERE helperId = '" + Convert.ToString(id) + "'", 7);
            string[] lines = response.Split('\n');
            List<appointment> listToReturn = new List<appointment>();
            for (int i = 0; i < lines.Length - 1; i++)
            {
                string line = lines[i];
                string[] responseSplit = line.Split('#');
                appointment appointmentToReturn = new appointment();
                appointmentToReturn.id = Convert.ToInt32(responseSplit[0]);
                appointmentToReturn.helperId = Convert.ToInt32(responseSplit[1]);
                appointmentToReturn.elderlyId = Convert.ToInt32(responseSplit[2]);
                appointmentToReturn.dateAndTime = DateTime.Parse(responseSplit[3]);
                appointmentToReturn.dateCreated = DateTime.Parse(responseSplit[4]);
                appointmentToReturn.ratingId = Convert.ToInt32(responseSplit[5]);
                appointmentToReturn.reviewId = Convert.ToInt32(responseSplit[6]);
                listToReturn.Add(appointmentToReturn);
            }
            
            return listToReturn;
       }

        
       
    }
}