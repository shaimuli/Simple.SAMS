using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Simple;
using Simple.SAMS.Contracts.Competitions;

namespace SAMS.Models
{
    public class MatchPlayerViewModel
    {
        public MatchPlayerViewModel(MatchPlayer matchPlayer)
        {
            
            Id = matchPlayer.Id;
            LocalFirstName = matchPlayer.LocalFirstName;
            LocalLastName = matchPlayer.LocalLastName;
            EnglishFirstName = matchPlayer.EnglishFirstName;
            EnglishLastName = matchPlayer.EnglishLastName;
        }

        public int? Rank { get; set; }
        public string LocalFirstName { get; set; }
        public string LocalLastName { get; set; }
        public string EnglishFirstName { get; set; }
        public string EnglishLastName { get; set; }

        public int Id { get; set; }

        public string FullName
        {
            get { return LocalLastName.NotNullOrEmpty() ? LocalFirstName + " " + LocalLastName : LocalFirstName; }
        }
    }
}