using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Simple.SAMS.Utilities
{
    public class DrawGeneratorInfo
    {
        public void Read(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            var pairs = lines.Select(l => l.Split('=')).Select(p => new KeyValuePair<string, string>(p[0], p[1]));
            
            var map = GetPropertiesMap();
            foreach (var pair in pairs)
            {
                PropertyInfo property;
                if (map.TryGetValue(pair.Key, out property))
                {
                    property.SetValue(this, pair.Value);
                }
            }
        }

        private Dictionary<string,PropertyInfo> GetPropertiesMap()
        {
            var properties = this.GetType().GetProperties();
            var pMap = new Dictionary<string, PropertyInfo>();
            foreach (var property in properties)
            {
                pMap[property.Name] = property;
            }
            return pMap;
        }

        public void Write(string fileName)
        {
            var lines = new List<string>();
            var properties = this.GetType().GetProperties();
            foreach (var property in properties)
            {
                lines.Add(property.Name + "=" + property.GetValue(this));
            }
            
            File.WriteAllLines(fileName, lines.ToArray());
        }

        public string DrawTitle { get; set; }
        public string DrawType { get; set; }
        public string DrawOtherInfo { get; set; }
        public string TargetFormat { get; set; }
        public string PaperSize { get; set; }
        public string DocumentAuthor { get; set; }
        public string DrawParticipantsCount { get; set; }
        public string DrawScriptID { get; set; }
        public string DrawScriptName { get; set; }
        public string DrawScriptInfo { get; set; }
        public string MinParticipants { get; set; }
        public string MaxParticipants { get; set; }
        public string PageOrientation { get; set; }
        public string PageMarginLeft { get; set; }
        public string PageMarginTop { get; set; }
        public string PageMarginRight { get; set; }
        public string PageMarginBottom { get; set; }
        public string DrawFontName { get; set; }
        public string DrawLineWidth { get; set; }
        public string DrawInfoLineWidth { get; set; }
        public string DrawBracketSpacing { get; set; }
        public string DrawDoubleSized1stBracket { get; set; }
        public string DrawReversedLastBracket { get; set; }
        public string DrawRepeatFinal { get; set; }
        public string DrawFullDELines { get; set; }
        public string DrawMergeStages { get; set; }
        public string DrawShowSeed { get; set; }
        public string DrawSeedChars { get; set; }
        public string DrawShowSeedBold { get; set; }
        public string DrawSeededLinesBold { get; set; }
        public string DataRandomizeSeeded { get; set; }
        public string DataRandomizeNonSeeded { get; set; }
        public string DrawShowMatchID { get; set; }
        public string DrawDetailedResults { get; set; }
        public string Draw3rdPlace { get; set; }
        public string DrawGroupsStandings { get; set; }
        public string DrawWinnerLabel { get; set; }
        public string DrawTopHalfFinalistLabel { get; set; }
        public string DataByeInfoText { get; set; }
        public string DataHomeAndAway { get; set; }
        public string DrawFontPercent { get; set; }
        public string DataParticipantsFile { get; set; }
        public string DataScheduleFile { get; set; }
        public string DataParticipantsFields { get; set; }
        public string DataScheduleFields { get; set; }
    }
}
