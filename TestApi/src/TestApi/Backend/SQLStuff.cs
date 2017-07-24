using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySql.Data.Common;
using MySql.Data.Types;

namespace TestApi.Sql
{
    public class SQLStuffOnTopOfController : Controller
    {
        public const bool server = false; // on the test machine, this is set to false to set the url to point to the server. Otherwise, for speed, it should point to localhost

        
        /// <summary>
        /// Returns a command - follows the following process:
        ///     (1) Creates a new connection with the db.
        ///     (2) Completes the SQL query
        ///     (3) Returns the value got from the MySQL server
        /// </summary>
        /// <param name="isResultExpected">true if a result is expected, false if not</param>
        /// <param name="command">String value of the query</param>
        /// <param name="numberExpectedPerRow">Defaults at one, but otherwise knows how to split the response</param>
        /// <returns></returns>
        public static string sqlCommand(bool isResultExpected, string command, int numberExpectedPerRow = 1)
        {
            MySqlConnection dbConnection = setupConnection(); // Setup Connection and return MySqlConnection object
            
            MySqlCommand commandForSql = new MySqlCommand(command, dbConnection);
            if(isResultExpected)
            {
                MySqlDataReader a = commandForSql.ExecuteReader();
                string toReturn = String.Empty;
                while(a.Read())
                {
                    for (int i = 0; i < numberExpectedPerRow; i++)
                    {
                        string ab = a.GetDataTypeName(i); //In case we are expecting a DateTime - deal with this
                        if (ab == "DATETIME")
                        {
                            toReturn += a.GetDateTime(i).ToString() + "#"; // Gets the DateTime as a DateTimeObject which can definitely be parsed by a DateTime.Parse()
                        }
                        else
                            toReturn += a.GetValue(i) + "#";
                    }
                    //# is used to seperate items in the same line
                    toReturn += "\n";
                    //NewLine ('\n') is used to seperate different lines
                }
                a.Close(); //For stability, the DataReader is closed
                closeConnection(dbConnection); // Connection is closed
                return toReturn;
            }
            else
            {
                commandForSql.ExecuteScalar(); // just execute the command
            }
            closeConnection(dbConnection); // Close the connection
            return String.Empty; // if you haven't already returned the string, then return an empty string
        }
        
        public static void closeConnection(MySqlConnection a)
        {
            a.Close();
        }
        
        /// <summary>
        /// Setup connection
        ///     (1) Defines the correct string for the connection
        ///     (2) Sets up the connection and returns the connection object.
        /// </summary>
        public static MySqlConnection setupConnection()
        {
            string connectionString = String.Empty;
            if(server)
                connectionString = "server=localhost;user id=#######; password=########;port=3306;database=ComputingProject;SslMode=None;Convert Zero DateTime=True;Allow Zero DateTime=True";
            else
                connectionString = "server=178.62.87.28;user id=#########;password=######;port=3306;database=ComputingProject;SslMode=None;Convert Zero DateTime=True;Allow Zero DateTime=True";
            MySqlConnection dbConnection = new MySqlConnection(connectionString);
            dbConnection.Open();
            return dbConnection;
        }
    }
}
