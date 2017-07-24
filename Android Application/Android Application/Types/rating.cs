using System;
namespace Android_Application.Types
{
    public class rating
    {
        private int _id;
        private int _starRating;
        private int _helperId;

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
        public int starRating
        {
            get
            {
                return _starRating;
            }
            set
            {
                _starRating = value;
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
    }
}