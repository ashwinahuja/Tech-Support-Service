using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySql.Data.Common;
using MySql.Data.Types;
using TestApi.Sql;
using TestApi.Types;
namespace TestApi.Controllers
{

    [Route("api/[Controller]")]
    public class reviews : SQLStuffOnTopOfController
    {
        /// <summary>
        /// Get all reviews of a user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGetAttribute("ga{id}")]
        public List<review> getAllReviewsOfAUser(int id)
        {
            string listOfAllReviews = sqlCommand(true, "SELECT id, review FROM reviews WHERE helperId = " + Convert.ToString(id), 2);
            string[] splitListOfAllReviews = listOfAllReviews.Split('\n');
            List<review> reviewsToReturn = new List<review>();
            for (int i = 0; i < splitListOfAllReviews.Length - 1; i++) // Parses the reviews into a list of review objects.
            {
                review a = new review();
                string[] b = splitListOfAllReviews[i].Split('#');
                a.id = Convert.ToInt32(b[0]);
                a.comment = b[1];
                a.helperId = id;
                reviewsToReturn.Add(a);
            }
            return reviewsToReturn;
        }

        /// <summary>
        /// Gets one specific review based on the reviewId
        /// </summary>
        /// <param name="appointmentId"></param>
        /// <returns></returns>
        [HttpGetAttribute("go{appointmentId}")]
        public review getOneSpecificReview(int appointmentId)
        {
            string oneReview = sqlCommand(true, "SELECT r.id, r.review, r.helperId FROM reviews r WHERE r.id = " + Convert.ToString(appointmentId), 3);
            review toReturn = new review();
            string[] split = oneReview.Split('#');
            toReturn.id = Convert.ToInt32(split[0]);
            toReturn.comment = split[1];
            toReturn.helperId = Convert.ToInt32(split[2]);

            return toReturn;
        }
        
        [HttpGet("geta{appointmentId}")]
        public review returnReviewBasedOnAppointmentId(int appointmentId)
        {
            string oneReview = sqlCommand(true, "SELECT r.id, r.review, r.helperId FROM reviews r JOIN appointments a ON a.reviewId = r.id", 3);
            review toReturn = new review();
            string[] split = oneReview.Split('#');
            toReturn.id = Convert.ToInt32(split[0]);
            toReturn.comment = split[1];
            toReturn.helperId = Convert.ToInt32(split[2]);
            return toReturn;
        }
        /// <summary>
        /// Add a review
        /// AppointmentId is stored in the reviewId location, which is not yet known.
        /// </summary>
        /// <param name="newReview"></param>
        /// <returns></returns>
        [HttpPost("ar")]
        public bool addAreview([FromBody] review newReview)
       {
            string review = newReview.comment;
            int appointmentId = newReview.id;
            int helperId = newReview.helperId;
            try
            {
                sqlCommand(false, "INSERT INTO reviews (review, helperId) VALUES('" + review + "','" + helperId + "');");
                sqlCommand(false, "UPDATE appointments SET reviewId = (SELECT max(id) FROM reviews) WHERE id = '" + appointmentId + "'");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Review is updated.
        /// </summary>
        /// <param name="newReview"></param>
        /// <returns></returns>
       [HttpPatchAttribute("ud")]
       public bool updateUserreview([FromBody] review newReview)
       {
            string review = newReview.comment;
            int reviewId = newReview.id;
            try
            {
                sqlCommand(false, "UPDATE reviews SET review = '" + Convert.ToString(review) + "' WHERE id = '" + reviewId + "';");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Review is removed.
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
       [HttpDeleteAttribute("rm{reviewId}")]
       public bool deleteUserreview(int reviewId)
       {
            if (Convert.ToInt32(sqlCommand(true, "SELECT count(1) FROM reviews WHERE id = '" + reviewId + "'", 1).Split('#')[0]) >= 1)
            {
                try
                {
                    sqlCommand(false, "DELETE FROM reviews WHERE id = '" + Convert.ToString(reviewId) + "'", 1);
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