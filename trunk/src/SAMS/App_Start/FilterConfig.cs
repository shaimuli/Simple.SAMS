using System.Web;
using System.Web.Mvc;
using SAMS.Filters;

namespace SAMS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandleErrorAttribute());
        }
    }
}