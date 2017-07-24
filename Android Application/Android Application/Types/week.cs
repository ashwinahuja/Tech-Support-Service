using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Android_Application.Types
{
    public class week
    {
        private int _id;
        private day _monday;
        private day _tuesday;
        private day _wednesday;
        private day _thursday;
        private day _friday;
        private day _saturday;
        private day _sunday;
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
        public day monday {
            get
            {
                return _monday;
            }
            set
            {
                _monday = value;
            }
        }
        public day tuesday {
            get
            {
                return _tuesday;
            }
            set
            {
                _tuesday = value;
            }
        }
        public day wednesday {
            get
            {
                return _wednesday;
            }
            set
            {
                _wednesday = value;
            }
        }
        public day thursday {
            get
            {
                return _thursday;
            }
            set
            {
                _thursday = value;
            }
        }
        public day friday {
            get
            {
                return _friday;
            }
            set
            {
                _friday = value;
            }
        }
        public day saturday {
            get
            {
                return _saturday;
            }
            set
            {
                _saturday = value;
            }
        }
        public day sunday {
            get
            {
                return _sunday;
            }
            set
            {
                _sunday = value;
            }
        }
    }
}
