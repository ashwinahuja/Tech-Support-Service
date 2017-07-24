using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApi.Types
{
    public class user
    {
        private int _id;
        private string _username;
        private string _firstname;
        private string _surname;
        private string _password;
        private string _emailAddress;
        private string _firstLineOfAddress;
        private string _secondLineOfAddress;
        private string _telephoneNumber;
        private string _postalCode;
        private string _city;
        private string _country;
        private bool _helper = false;
        private int _timetableId;
        private double _distance = double.MaxValue;
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
        public string username {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }
        public string firstName {
            get
            {
                return _firstname;
            }
            set
            {
                _firstname = value;
            }
        }
        public string surname {
            get
            {
                return _surname;
            }
            set
            {
                _surname = value;
            }
        }
        public string password {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }
        public string emailAddress {
            get
            {
                return _emailAddress;
            }
            set
            {
                _emailAddress = value;
            }
        }
        public string firstLineOfAddress {
            get
            {
                return _firstLineOfAddress;
            }
            set
            {
                _firstLineOfAddress = value;
            }
        }
        public string secondLineOfAddress{
            get
            {
                return _secondLineOfAddress;
            }
            set
            {
                _secondLineOfAddress = value;
            }
        }
        public string telephoneNumber {
            get
            {
                return _telephoneNumber;
            }
            set
            {
                _telephoneNumber = value;
            }
        }
        public string postalCode{
            get
            {
                return _postalCode;
            }
            set
            {
                _postalCode = value;
            }
        }
        public string city {
            get
            {
                return _city;
            }
            set
            {
                _city = value;
            }
        }
        public string country {
            get
            {
                return _country;
            }
            set
            {
                _country = value;
            }
        }
        public bool helper {
            get
            {
                return _helper;
            }
            set
            {
                _helper = value;
            }
        }
        public int timetableId {
            get
            {
                return _timetableId;
            }
            set
            {
                _timetableId = value;
            }
        }

        public double distance {
            get
            {
                return _distance;
            }
            set
            {
                _distance = value;
            }
        }
    }
}
