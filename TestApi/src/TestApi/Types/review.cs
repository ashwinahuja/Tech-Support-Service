using System;
namespace TestApi.Controllers
{
    public class review
    {
        private int _id;
        private int _helperId;
        private string _comment;

        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        public int helperId
        {
            get
            {
                return _helperId;
            }
            set
            {
                _helperId = value;
            }
        }
        public string comment
        {
            get
            {
                return _comment;
            }
            set
            {
                _comment = value;
            }
        }
        
    }
}