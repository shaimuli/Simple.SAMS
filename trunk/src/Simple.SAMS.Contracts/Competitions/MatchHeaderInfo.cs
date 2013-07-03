using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts.Competitions
{
    [DataContract(Namespace = Namespaces.Data)]
    public class MatchHeaderInfo
    {
        public string Date
        {
            get { return StartTime.HasValue ? StartTime.Value.ToShortDateString() : string.Empty; }
        }
        public string Time
        {
            get { return StartTime.HasValue ? StartTime.Value.ToShortTimeString() : string.Empty; }
        }

        [DataMember(EmitDefaultValue = false)]
        public SlotType? SlotType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? SlotPosition { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Player1Code { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string Player2Code { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int RoundRelativePosition { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public CompetitionSection Section { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int CompetitionId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Id { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public StartTimeType StartTimeType { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public MatchStatus Status { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public MatchWinner Winner { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public MatchResult? Result { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int Position { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public int Round { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool IsFinal { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public bool IsSemiFinal { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public SetScore[] SetScores { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public DateTime? StartTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public MatchPlayer Player1 { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public MatchPlayer Player2 { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public MatchPlayer Player3 { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public MatchPlayer Player4 { get; set; }

        public MatchPlayer[] Players
        {
            get { return new[] {Player1, Player2, Player3, Player4}; }
        }

    }
}
