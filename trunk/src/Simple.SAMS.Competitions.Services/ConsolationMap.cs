using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Competitions.Services
{
    public class ConsolationMap
    {
        private Dictionary<int, Tuple<int, int>> m_p32qmap;
        private Dictionary<int, int> m_p16map;
        private Dictionary<int, int> m_p32map;
        private Dictionary<int, int> m_p64map;
        public ConsolationMap()
        {
            CreateMap16();
            CreateMap32();
            CreateMap64();
            CreateQMap32();
        }

        public Tuple<int, int> GetNextPosition(int playersCount, int position)
        {
            return m_p32qmap[position];
        }

        private void CreateQMap32()
        {
            m_p32qmap = new Dictionary<int, Tuple<int, int>>();
            m_p32qmap[0] = new Tuple<int, int>(8,0);
            m_p32qmap[1] = new Tuple<int, int>(9,0);
            m_p32qmap[2] = new Tuple<int, int>(10,0);
            m_p32qmap[3] = new Tuple<int, int>(11,0);
            m_p32qmap[4] = new Tuple<int, int>(12,1);
            m_p32qmap[5] = new Tuple<int, int>(13,1);
            m_p32qmap[6] = new Tuple<int, int>(14,1);
            m_p32qmap[7] = new Tuple<int, int>(15,1);
            m_p32qmap[8] = new Tuple<int, int>(16,0);
            m_p32qmap[9] = new Tuple<int, int>(16,1);
            m_p32qmap[10] = new Tuple<int, int>(17,0);
            m_p32qmap[11] = new Tuple<int, int>(17,1);
            m_p32qmap[12] = new Tuple<int, int>(18,0);
            m_p32qmap[13] = new Tuple<int, int>(18,1);
            m_p32qmap[14] = new Tuple<int, int>(19,0);
            m_p32qmap[15] = new Tuple<int, int>(19,1);

            m_p32qmap[16] = new Tuple<int, int>(20,0);
            m_p32qmap[17] = new Tuple<int, int>(21,0);
            m_p32qmap[18] = new Tuple<int, int>(22,1);
            m_p32qmap[19] = new Tuple<int, int>(23,1);
            
            m_p32qmap[20] = new Tuple<int, int>(24,0);
            m_p32qmap[21] = new Tuple<int, int>(24,1);
            m_p32qmap[22] = new Tuple<int, int>(25,0);
            m_p32qmap[23] = new Tuple<int, int>(25,1);
            
            m_p32qmap[24] = new Tuple<int, int>(26,0);
            m_p32qmap[25] = new Tuple<int, int>(26,1);

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
