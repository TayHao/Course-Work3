using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudClient
{
    public class EventRow
    {
        public string ID { get; set; }
        public string Name{ get; set; }
        public string Deadline{ get; set; }
        public string Date{ get; set; }
        public string Points{ get; set; }
        public string Penalty{ get; set; }

        public EventRow(string id, string name, string deadline, string date, string points, string penalty)
        {
            ID = id;
            Name = name;
            Deadline = deadline;
            Date = date;
            Points = points;
            Penalty = penalty;
        }

        public EventRow()
        {
                
        }
    }
}
