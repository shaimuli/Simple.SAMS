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
        public DateTime? EndTime { get; set; }
        public string MainRefereePhone { get; set; }
        public string MainReferee { get; set; }
        public string Site { get; set; }
        public string SitePhone { get; set; }
        public HttpPostedFile PlayersFile { get; set; }
    }
}