using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Competitions.Services
{
    public class ConsolationMap
    {
        private Dictionary<int, int> m_p16map;
        private Dictionary<int, int> m_p32map;
        private Dictionary<int, int> m_p64map;
        public ConsolationMap()
        {
            CreateMap16();
            CreateMap32();
            CreateMap64();
        }

        public int Position(int playersCount, int position)
        {
            if (playersCount == 16)
            {
                return m_p16map[position];
            }
            else if (playersCount == 32)
            {
                return m_p32map[position];
            }
            else if (playersCount == 64)
            {
                return m_p64map[position];
            }
            throw new NotSupportedException("Players count {0} not supported.".ParseTemplate(playersCount));
        }

        private void CreateMap16()
        {
            m_p16map = new Dictionary<int, int>();
            m_p16map[8] = 11;
            m_p16map[9] = 10;
            m_p16map[10] = 9;
            m_p16map[11] = 8;
        }

        private void CreateMap32()
        {
            m_p32map = new Dictionary<int, int>();
            m_p32map[16] = 15;
            m_p32map[17] = 14;
            m_p32map[18] = 13;
            m_p32map[19] = 12;
            m_p32map[20] = 11;
            m_p32map[21] = 10;
            m_p32map[22] = 9;
            m_p32map[23] = 8;

            m_p32map[24] = 21;
            m_p32map[25] = 20;
            m_p32map[26] = 23;
            m_p32map[27] = 22;
        }        
        
        private void CreateMap64()
        {
            m_p32map = new Dictionary<int, int>();
            m_p32map[16] = 15;
            m_p32map[17] = 14;
            m_p32map[18] = 13;
            m_p32map[19] = 12;
            m_p32map[20] = 11;
            m_p32map[21] = 10;
            m_p32map[22] = 9;
            m_p32map[23] = 8;
            m_p32map[16] = 15;
            m_p32map[17] = 14;
            m_p32map[18] = 13;
            m_p32map[19] = 12;
            m_p32map[20] = 11;
            m_p32map[21] = 10;
            m_p32map[22] = 9;
            m_p32map[23] = 8;

            m_p32map[16] = 15;
            m_p32map[17] = 14;
            m_p32map[18] = 13;
            m_p32map[19] = 12;
            m_p32map[20] = 11;
            m_p32map[21] = 10;
            m_p32map[22] = 9;
            m_p32map[23] = 8;

            m_p32map[24] = 21;
            m_p32map[25] = 20;
            m_p32map[26] = 23;
            m_p32map[27] = 22;
        }
    }
}
