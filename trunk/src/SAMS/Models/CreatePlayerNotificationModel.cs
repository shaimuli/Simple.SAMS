using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAMS.Models
{
    public class CreatePlayerNotificationModel
    {
        public string Header { get; set; }
        public string CompetitionName { get; set; }
        public string PlayerName { get; set; }
    }
}