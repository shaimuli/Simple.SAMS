namespace Simple.SAMS.CompetitionEngine
{
    public class PositioningItem
    {
        public int Index { get; set; }
        public string Code { get; set; }

        public override string ToString()
        {
            return Code;
        }
    }
}