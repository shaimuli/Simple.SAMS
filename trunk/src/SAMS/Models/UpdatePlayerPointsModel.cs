using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAMS.Models
{
    public class UpdatePlayerPointsModel
    {
        public int Id { get; set; }
        public int? Points { get; set; }
        public int? Position { get; set; }
    }
}