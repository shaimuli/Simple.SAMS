using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Simple.SAMS.CompetitionEngine;

namespace CompetitionEngineTestClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }

        private readonly Random m_randomizer =new Random();
   
        private void quali_Click(object sender, EventArgs e)
        {
            var playersCount = int.Parse(PlayersCount.Text);
            var gPlayersCount = int.Parse(GPlayersCount.Text);
            var qualifyingCount = int.Parse(QCount.Text);
            var ps = Enumerable.Range(0, gPlayersCount).Select(i => new Player { Id = i + 1, Rank = i+1/* m_randomizer.Next(0, gPlayersCount)*/ });
            players.Items.Clear();
            players.Items.AddRange(ps.ToArray());
            var engine = new QualifyingPositioningEngine();
            var evPositions = engine.Evaluate(new QualifyingPositionParameters()
                                                {
                                                    PlayersCount = playersCount,
                                                    QualifyingCount = qualifyingCount,
                                                    Players = ps.ToArray()
                                                });
            positions.Items.Clear();
            var index = 1;
            foreach (var competitionPosition in evPositions)
            {

                positions.Items.Add(string.Format("{0}:{1}", index, competitionPosition.PlayerId));
                index++;
            }
        }

        private void FINALS_Click(object sender, EventArgs e)
        {
            var playersCount = int.Parse(FinalPlayersCount.Text);
            var gPlayersCount = int.Parse(FinalGPlayersCount.Text);
            var rankedCount = int.Parse(FinalRPlayersCount.Text);
            var ps = Enumerable.Range(0, gPlayersCount).Select(i => new Player { Id = i + 1, Rank = i + 1/* m_randomizer.Next(0, gPlayersCount) }*/});
            finalPlayers.Items.Clear();
            finalPlayers.Items.AddRange(ps.ToArray());
            var engine = new FinalPositioningEngine();
            var evPositions = engine.Evaluate(new FinalPositioningParameters()
            {
                PlayersCount = playersCount,
                RankedPlayersCount = rankedCount,
                Players = ps.ToArray()
            });
            finalPositions.Items.Clear();
            var index = 1;
            foreach (var competitionPosition in evPositions)
            {

                finalPositions.Items.Add(string.Format("{0}:{1}", index, competitionPosition.PlayerId));
                index++;
            }
        }
    }
}
