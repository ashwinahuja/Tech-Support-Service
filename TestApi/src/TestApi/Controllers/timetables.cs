using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySql.Data.Common;
using MySql.Data.Types;
using TestApi.Backend;
using TestApi.Sql;
using System.Runtime.Serialization;
using TestApi.Types;
using System.Globalization;

namespace TestApi.Controllers
{
    /// <summary>
    /// Things it needs to do - add a new week to the timetable or update. These can effectively be classed as the same thing
    ///                         retrieve an existing timetable
    ///                         create a new timetable and return the timetableid, set the defaultweek to 1
    /// </summary>
    [Route("api/tt")]
    public class timetables : SQLStuffOnTopOfController
    {
        ///
        public int createTimetable()
        {
            sqlCommand(false, "INSERT INTO timetables (defaultWeek) VALUES (1)");
            return int.Parse(sqlCommand(true, "SELECT max(id) FROM timetables", 1).Split('#')[0]);
        }
        /// <summary>
        /// Add a timetable to the system
        /// </summary>
        /// <param name="t"></param>
        /// <param name="doIt"></param>
        [HttpPost("adt")]
        public void addTimetable([FromBody] timetable t, bool doIt = true)
        {
            //First Deal with Default Week
            t.defaultWeek.monday.id = addDay(t.defaultWeek.monday); // add each day to the database and get their ids
            t.defaultWeek.tuesday.id = addDay(t.defaultWeek.tuesday);
            t.defaultWeek.wednesday.id = addDay(t.defaultWeek.wednesday);
            t.defaultWeek.thursday.id = addDay(t.defaultWeek.thursday);
            t.defaultWeek.friday.id = addDay(t.defaultWeek.friday);
            t.defaultWeek.saturday.id = addDay(t.defaultWeek.saturday);
            t.defaultWeek.sunday.id = addDay(t.defaultWeek.sunday);

            t.defaultWeek.id = addWeek(t.defaultWeek); // add the week
            sqlCommand(false, "INSERT INTO timetables (defaultWeek) VALUES (" + t.defaultWeek.id + ");");
            int oldId = t.id;
            t.id = int.Parse(sqlCommand(true, "SELECT max(id) FROM timetables", 1).Split('#')[0]);
            
            //Then Each Week
            for (int i = 0; i < t.weeks.Count(); i++)
            {
                string nameOfColumn = t.weeks[i].weekBeginning.ToString("dd/MM/yyyy");
                nameOfColumn = nameOfColumn.Replace('/', '_'); // Gets the name of the column
                t.weeks[i].week.monday.id = addDay(t.weeks[i].week.monday);
                t.weeks[i].week.tuesday.id = addDay(t.weeks[i].week.tuesday);
                t.weeks[i].week.wednesday.id = addDay(t.weeks[i].week.wednesday);
                t.weeks[i].week.thursday.id = addDay(t.weeks[i].week.thursday);
                t.weeks[i].week.friday.id = addDay(t.weeks[i].week.friday);
                t.weeks[i].week.saturday.id = addDay(t.weeks[i].week.saturday);
                t.weeks[i].week.sunday.id = addDay(t.weeks[i].week.sunday);
                t.weeks[i].week.id = addWeek(t.weeks[i].week);
                try
                {
                    sqlCommand(false,"UPDATE timetables SET " + nameOfColumn + "=" + t.weeks[i].week.id + " WHERE id = " + t.id + ";");
                    //This only doesn't throw an exception if the column already exists. If not, the catch is used to create a new column
                }
                catch
                {
                    sqlCommand(false, "ALTER TABLE timetables ADD " + nameOfColumn + " INT NULL;"); // Add column
                    string beenReturn = sqlCommand(true, "SELECT id, defaultWeek FROM timetables", 2);
                    string[] havingBeenSplit = beenReturn.Split('\n');
                    for (int k = 0; k < havingBeenSplit.Length - 1; k++) // Similar to appointments, this effectively creates a copy of default week for each entry and populates the column
                    {
                        string[] havingBeenSplitMore = havingBeenSplit[k].Split('#');
                        int idOfUser = int.Parse(havingBeenSplitMore[0]);
                        int defaultWeekIdOfUser = int.Parse(havingBeenSplitMore[1]);
                        week newWeek = timetables.getWeek2(defaultWeekIdOfUser);
                        int newId = timetables.addWeek(newWeek);
                        sqlCommand(false, "UPDATE timetables SET " + nameOfColumn + " = " + newId + " WHERE id = " + idOfUser);
                    }
                    sqlCommand(false, "UPDATE timetables SET " + nameOfColumn + "=" + t.weeks[i].week.id + " WHERE id = " + t.id + ";");
                }
            }
            if (doIt) // If set as an update
            {
                sqlCommand(false, "UPDATE accountsForHelpers SET timetableId = " + t.id + " WHERE timetableId = " + oldId + ";");
            }
        }
        /// <summary>
        /// Get a timetable
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        [HttpGetAttribute("gt{i}")]
        public  Types.timetable getTimetable(int i)
        {
            string columnNames = sqlCommand(true, "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'timetables'", 1); // Gets all the column names
            string[] split = columnNames.Split('\n');
            for(int k = 0; k < split.Length - 1; k++)
            {
                split[k] = split[k].Substring(0, split[k].Length - 1); // Remove spaces between the column names
            }
            int lengthOfInputExpected = split.Length - 1;
            string input = sqlCommand(true, "SELECT * FROM timetables WHERE id = '" + i + "';",lengthOfInputExpected);
            string[] inputSplit = input.Split('#');
            TestApi.Types.timetable toReturn = new Types.timetable();
            toReturn.id = int.Parse(inputSplit[0]);
            toReturn.defaultWeek = getWeek(int.Parse(inputSplit[1])); // calls the get week on the defaultWeek 
            for(int j = 2; j < inputSplit.Length - 1; j++) // For each other week
            {
                Types.week week = new Types.week();
                Types.names name = new Types.names();
                if (String.IsNullOrEmpty(inputSplit[j]))
                { } // Incase it is empty
                else
                {
                    split[j] = split[j].Replace('_', '/');
                    name.weekBeginning = DateTime.ParseExact(split[j], "dd/MM/yyyy", CultureInfo.InvariantCulture); // Parse the column as a datebeginning date
                    
                    name.week = getWeek(int.Parse(inputSplit[j]));
                    toReturn.weeks.Add(name); // See the timetable object to understand what names is.
                }
            }
            return toReturn;
        }
        public static void updateWeek(week input)
        {
            string a = "UPDATE week SET mondayId = " + input.monday.id + ", tuesdayId = " + input.tuesday.id + ", wednesdayId = " + input.wednesday.id + ", thursdayId = " + input.thursday.id + ", fridayId = " + input.friday.id + ", saturdayId = " + input.saturday.id + ", sundayId = " + input.sunday.id + " WHERE id = " + input.id + ";";
            sqlCommand(false, a);
        }

       
        public static int addDay(Types.day input)
        {
            // converts a timesFree into an integer array.
            int[] timesFree2 = new int[12];
            for (int k = 0; k < input.timesFree.Length; k++)
            {
                if (input.timesFree[k])
                    timesFree2[k] = 1;
                else
                    timesFree2[k] = 0;
            }
            // Creates the SQL command to be executed
            string a = "INSERT INTO day (a02, a24, a46, a68, a810,a1012,a1214,a1416,a1618,a1820,a2022,a2224) VALUES(";
            for (int i = 0; i < timesFree2.Length-1; i++)
            {
                a += Convert.ToString(timesFree2[i]) + ",";
            }
            a += Convert.ToString(timesFree2[timesFree2.Length - 1]) + ");";
            sqlCommand(false, a);
            return int.Parse(sqlCommand(true, "SELECT max(id) FROM day", 1).Split('#')[0]); // gets the id of the day just added.
        }


        [HttpPostAttribute("ud")]
        public bool updateDay([FromBody] Types.day input)
        {
            //Updates the day
            int[] timesFree2 = new int[12];
            for (int k = 0; k < input.timesFree.Length; k++)
            {
                if(input.timesFree[k])
                    timesFree2[k] = 1;
                else
                    timesFree2[k] = 0;
            }
            string a = "UPDATE day SET a02 = " + timesFree2[0] + ", a24 = " + timesFree2[1] + ", a46 = " + timesFree2[2] + ", a68 = " + timesFree2[3] + ", a810 = " + timesFree2[4] + ",a1012=" + timesFree2[5] + ",a1214=" + timesFree2[6] + ",a1416=" + timesFree2[7] + ",a1618=" + timesFree2[8] + ",a1820=" + timesFree2[9] + ",a2022=" + timesFree2[10] + ",a2224=" + timesFree2[11] + " WHERE id =" + input.id + ";";
            
            sqlCommand(false, a);
            return true;
        }

        public static int addWeek(Types.week input)
        {
            //Adds a week
            int[] days = new int[7];
            days[0] = input.monday.id;
            days[1] = input.tuesday.id;
            days[2] = input.wednesday.id;
            days[3] = input.thursday.id;
            days[4] = input.friday.id;
            days[5] = input.saturday.id;
            days[6] = input.sunday.id;
            string url = "INSERT INTO week (mondayId, tuesdayId, wednesdayId, thursdayId, fridayId, saturdayId, sundayId) VALUES (";
            for (int i = 0; i < 6; i++)
                url += Convert.ToString(days[i]) + ",";
            url += Convert.ToString(days[6]) + ")";
            sqlCommand(false, url);
            return int.Parse(sqlCommand(true, "SELECT max(id) FROM week", 1).Split('#')[0]); // returns the weekId of the thing just added
        }

        [HttpGetAttribute("gw{i}")]
        public Types.week getWeek(int i)
        {
            //Defines SQL command, executes it and parses the response into a week object which can be returned
            string returned = sqlCommand(true, "SELECT id, mondayId, tuesdayId, wednesdayId, thursdayId, fridayId, saturdayId, sundayId FROM week WHERE id = '" + i + "'", 8);
            string[] returnedSplit = returned.Split('#');
            Types.week toReturn = new Types.week();
            toReturn.id = int.Parse(returnedSplit[0]);
            timetables ttt = new timetables();
            toReturn.monday = ttt.getDay(int.Parse(returnedSplit[1]));
            toReturn.tuesday = ttt.getDay(int.Parse(returnedSplit[2]));
            toReturn.wednesday = ttt.getDay(int.Parse(returnedSplit[3]));
            toReturn.thursday = ttt.getDay(int.Parse(returnedSplit[4]));
            toReturn.friday = ttt.getDay(int.Parse(returnedSplit[5]));
            toReturn.saturday = ttt.getDay(int.Parse(returnedSplit[6]));
            toReturn.sunday = ttt.getDay(int.Parse(returnedSplit[7]));
            return toReturn;
        }

        public static Types.week getWeek2(int i)
        {
            string returned = sqlCommand(true, "SELECT id, mondayId, tuesdayId, wednesdayId, thursdayId, fridayId, saturdayId, sundayId FROM week WHERE id = '" + i + "'", 8);
            string[] returnedSplit = returned.Split('#');
            Types.week toReturn = new Types.week();
            toReturn.id = int.Parse(returnedSplit[0]);
            timetables ttt = new timetables();
            toReturn.monday = ttt.getDay(int.Parse(returnedSplit[1]));
            toReturn.tuesday = ttt.getDay(int.Parse(returnedSplit[2]));
            toReturn.wednesday = ttt.getDay(int.Parse(returnedSplit[3]));
            toReturn.thursday = ttt.getDay(int.Parse(returnedSplit[4]));
            toReturn.friday = ttt.getDay(int.Parse(returnedSplit[5]));
            toReturn.saturday = ttt.getDay(int.Parse(returnedSplit[6]));
            toReturn.sunday = ttt.getDay(int.Parse(returnedSplit[7]));
            return toReturn;
        }

        /// <summary>
        /// Gets the day
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        [HttpGetAttribute("gd{i}")]
        public Types.day getDay(int i)
        {
            //Defines the SQL command and parses response into a day object.
            Types.day toReturn = new Types.day();
            string[] responseSplit = sqlCommand(true, "SELECT id, a02, a24, a46, a68, a810, a1012, a1214, a1416, a1618, a1820, a2022, a2224 FROM day WHERE id = '" + i + "';", 13).Split('#');
            toReturn.id = int.Parse(responseSplit[0]);
            for (int j = 1; j < responseSplit.Length - 1; j++ )
            {
                if (responseSplit[j] == "0")
                    toReturn.timesFree[j - 1] = false;
                else if (responseSplit[j] == "1")
                    toReturn.timesFree[j - 1] = true;
                else
                    toReturn.timesFree[j - 1] = false;
            }
            return toReturn;
        }


    }

}