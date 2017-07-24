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
using TestApi.Types;
namespace TestApi.Controllers
{

    [Route("api/[Controller]")]
    public class ratings : SQLStuffOnTopOfController
    {
        /// <summary>
        /// Get all ratings of a helper
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
       [HttpGetAttribute("ga{id}")]
       public List<rating> getAllRatingsForUser(int id)
       {
            string listOfAllRatings = sqlCommand(true, "SELECT id, starRating, helperId FROM ratings WHERE helperId = " + Convert.ToString(id),3);
            string[] splitListOfAllRatings = listOfAllRatings.Split('\n');
            List<rating> ratingsToReturn = new List<rating>();
            for (int i = 0; i < splitListOfAllRatings.Length - 1; i++) // Parses the ratings into a list of ratings.
            {
                rating a = new rating();
                string[] b = splitListOfAllRatings[i].Split('#');
                a.id = Convert.ToInt32(b[0]);
                a.starRating = Convert.ToInt32(b[1]);
                a.helperId = Convert.ToInt32(b[2]);
                ratingsToReturn.Add(a);
            }
            return ratingsToReturn;
       }

        /// <summary>
        /// Get one specific rating based on the ratingId
        /// </summary>
        /// <param name="ratingId"></param>
        /// <returns></returns>
       [HttpGetAttribute("go{ratingId}")]
       public rating getOneSpecificRating(int ratingId)
       {
            string oneRating = sqlCommand(true, "SELECT r.id, r.starRating,r.helperId FROM ratings r WHERE r.id = " + Convert.ToString(ratingId),3);
            rating toReturn = new rating();
            string[] split = oneRating.Split('#');
            toReturn.id = Convert.ToInt32(split[0]);
            toReturn.starRating = Convert.ToInt32(split[1]);
            toReturn.helperId = Convert.ToInt32(split[2]);
            return toReturn;
       }

        /// <summary>
        /// Add a rating
        /// Appointment id is stored in the position of the ratingId - which isn't known yet.
        /// </summary>
        /// <param name="newRating"></param>
        /// <returns></returns>
       [HttpPost("ar")]
       public bool addARating([FromBody] rating newRating)
       {
            int appointmentId = newRating.id;
            int rating = newRating.starRating;
            int helperId = newRating.helperId;
            try
            {
                sqlCommand(false, "INSERT INTO ratings (starRating, helperId) VALUES('" + rating + "','" + helperId + "');");
                sqlCommand(false, "UPDATE appointments SET ratingId = (SELECT max(id) FROM ratings) WHERE id = '" + appointmentId + "'");
                return true;
            }
            catch
            {
                return false;
            }
       }

        /// <summary>
        /// Update rating
        /// </summary>
        /// <param name="newRating"></param>
        /// <returns></returns>
       [HttpPatchAttribute("ud")]
       public bool updateUserRating([FromBody] rating newRating)
       {
            int ratingId = newRating.id;
            int rating = newRating.starRating;

            try
            {
                sqlCommand(false, "UPDATE ratings SET starRating = '" + Convert.ToString(rating) + "' WHERE id = '" + ratingId + "';");
                return true;
            }
            catch
            {
                return false;
            }
       }

        /// <summary>
        /// Delete rating
        /// </summary>
        /// <param name="ratingId"></param>
        /// <returns></returns>
       [HttpDeleteAttribute("rm{ratingId}")]
       public bool deleteUserRating(int ratingId)
       {
            if (Convert.ToInt32(sqlCommand(true, "SELECT count(1) FROM ratings WHERE id = '" + ratingId + "'", 1).Split('#')[0]) >= 1)
            {
                try
                {
                    sqlCommand(false, "DELETE FROM ratings WHERE id = '" + Convert.ToString(ratingId) + "'", 1);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
       }
    }
}