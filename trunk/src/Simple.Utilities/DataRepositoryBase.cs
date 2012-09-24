using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Utilities
{
    public class DataRepositoryBase<TDataContext>
        where TDataContext : DataContext, new()
    {
        
        protected virtual TDataContext CreateDataContext()
        {
            return new TDataContext();
        }


        protected virtual void UseDataContext(Action<TDataContext> action)
        {
            using (var dataContext = CreateDataContext())
            {
                
                action(dataContext);
            }
        }

    }
}
