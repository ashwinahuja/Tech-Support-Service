using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Android_Application.Types
{
    public class userToBeVerified
    {
        private string _emailAddress;
        private string _password;
        private bool _helper;
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

    }
}
