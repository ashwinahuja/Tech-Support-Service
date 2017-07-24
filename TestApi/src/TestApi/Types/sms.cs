using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApi.Controllers
{
    public class sms
    {
        private string _From;
        private string _Body;
        public string From {
            get
            {
                return _From;
            }
            set
            {
                _From = value;
            }
        }
        public string Body {
            get
            {
                return _Body;
            }
            set
            {
                _Body = value;
            }
        }
    }
}
