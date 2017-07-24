using System;

namespace Android_Application.Types
{
    public class appointment : Object // Inherits from the object to get the compare function
    {
        private int _id;
        private int _helperId;
        private int _elderlyId;
        private DateTime _dateAndTime;
        private DateTime _dateCreated;
        private int _ratingId;
        private int _reviewId;
        public int id {
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
        public int elderlyId
        {
            get
            {
                return _elderlyId;
            }
            set
            {
                _elderlyId = value;
            }
        }
        public DateTime dateAndTime
        {
            get
            {
                return _dateAndTime;
            }
            set
            {
                _dateAndTime = value;
            }
        }
        public DateTime dateCreated
        {
            get
            {
                return _dateCreated;
            }
            set
            {
                _dateCreated = value;
            }
        }
        public int ratingId
        {
            get
            {
                return _ratingId;
            }
            set
            {
                _ratingId = value;
            }
        }
        public int reviewId
        {
            get
            {
                return _reviewId;
            }
            set
            {
                _reviewId = value;
            }
        }

        public static bool Equals(appointment a, appointment b) // Check if two appointments are the same
        {
            if (a.id != b.id)
                return false;
            if (a.helperId != b.helperId)
                return false;
            if (a.elderlyId != b.elderlyId)
                return false;
            if (DateTime.Compare(a.dateAndTime, b.dateAndTime) != 0)
                return false;
            if (DateTime.Compare(a.dateCreated, b.dateCreated) != 0)
                return false;
            if (a.ratingId != b.ratingId)
                return false;
            if (a.reviewId != b.reviewId)
                return false;
            return true;
               
       
        }
    }
}