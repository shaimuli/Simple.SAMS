using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.SAMS.Contracts
{
    public interface IEntity
    {
        int Id { get; set; }
        DateTime Created { get; set; }
        DateTime Updated { get; set; }
        EntityItemStatus ItemStatus { get; set; }

    }
}
