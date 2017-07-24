using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestApi.Sql;
using TestApi.Backend;
using TestApi.Types;
namespace TestApi.Controllers


{
    /// <summary>
    /// Requests to be sent to .../api/ac...
    /// </summary>
    [Route("api/ac")]
    public class accounts : SQLStuffOnTopOfController
    {
        /// <summary>
        /// Useful test left - used by the app to test whether the API is working.
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public string GetHelloWorld()
        {
            return "Hello World!";
        }

        
        /// <summary>
        /// Add a new user to the database
        /// </summary>
        /// <param name="a"> Gets the user from the json of the object (FROM BODY)</param>
        /// <returns></returns>
        [HttpPost("users/ad")]
        public bool AddNewUser([FromBody] user a)
        {
            
            if (!a.helper) // Send it to the right command in either accountsForElderly or accountsFoHelper
            {
                accountsForElderly ae = new accountsForElderly();
                return ae.AddNewUser(a);
            }
            else if (a.helper)
            {
                accountsForHelper ah = new accountsForHelper();
                return ah.AddNewUser(a); // This is static so this can be done...
            }
            else
                return false;
        }
        /// <summary>
        /// VERIFY USER
        ///     Just sends the contents to the correct section - accounts for elderly or accounts for helpers.
        ///     Saves having to have a different login process.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [HttpPost("users/vf")]
        public user verifyUser([FromBody] userToBeVerified a)
        {
            if (!a.helper)
            {
                accountsForElderly ae = new accountsForElderly();
                return ae.verifyUser(a);
            }
            else if (a.helper)
            {
                accountsForHelper ah = new accountsForHelper();
                return ah.verifyUser(a);
            }
            else
                return new user();
        }
    }
}
