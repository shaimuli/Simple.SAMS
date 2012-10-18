using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAMS.Models
{
    public class CreateCompetitionModel
    {
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        
        public CompetitionTypeReference Type { get; set; }

        public IEnumerable<CompetitionTypeReference> AvailableTypes { get; set; }
    }
}