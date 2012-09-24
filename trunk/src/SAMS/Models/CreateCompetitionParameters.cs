using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAMS.Models
{
    public class CreateCompetitionParameters
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public DateTime StartTime { get; set; }
        public HttpPostedFile PlayersFile { get; set; }
    }
}