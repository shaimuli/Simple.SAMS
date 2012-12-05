using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.CompetitionEngine
{
    public class RandomPositionGenerator
    {
        private readonly List<int> m_positions;
        private readonly Random m_randomizer = new Random();
        public RandomPositionGenerator(int[] positions)
        {
            m_positions = new List<int>(positions);
        }
        public bool CanTake()
        {
            return m_positions.Count > 0;
        }

        public int Take()
        {
            if (m_positions.Count == 0)
            {
            throw new InvalidOperationException("No positions left");
            }
            var randomIndex = m_randomizer.Next(0, m_positions.Count);
            var position = m_positions[randomIndex];
            m_positions.RemoveAt(randomIndex);
            return position;
        }

    }
}
