using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Competitions.Data
{
    public interface IDataEntity
    {
        int Id { get; set; }
        DateTime Created { get; set; }
        DateTime Updated { get; set; }
        int RowStatus { get; set; }

    }
}
