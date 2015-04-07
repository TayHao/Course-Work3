using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message
{
    [Serializable]
    public enum STATUS { LOGIN, GET_TABLE, ADD_STUDENT, ADD_ADMIN, ADD_INSTRUCTOR, GET_GROUPS, GET_PROJECT_BY_STUDENT, GET_EVENTS_BY_PROJECT }
    [Serializable]
    public class MSG
    {
        public STATUS stat;
    }
}
