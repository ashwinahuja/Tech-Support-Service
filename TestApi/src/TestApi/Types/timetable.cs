using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApi.Types
{
    public class names
    {
        private DateTime _weekBeginning;
        private week _week;
        public DateTime weekBeginning {
            get
            {
                return _weekBeginning;
            }
            set
            {
                _weekBeginning = value;
            }
        }
        public week week {
            get
            {
                return _week;
            }
            set
            {
                _week = value;
            }
        }

    }
    public class timetable
    {
        private int _id;
        private week _defaultWeek;
        private List<names> _weeks = new List<names>();
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
        public week defaultWeek {
            get
            {
                return _defaultWeek;
            }
            set
            {
                _defaultWeek = value;
            }
        }
        public List<names> weeks {
            get
            {
                return _weeks;
            }
            set
            {
                _weeks = value;
            }
        }


    }
}
