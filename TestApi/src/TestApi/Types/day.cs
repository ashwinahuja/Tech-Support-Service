using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApi.Types
{
    public class day
    {
        private int _id;
        private bool[] _timesFree = new bool[12];
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
        public bool[] timesFree
        {
            get
            {
                return _timesFree;
            }
            set
            {
                _timesFree = value;
            }
        }
        
    }
}
