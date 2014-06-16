using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Web;

namespace GuildfordBoroughCouncil.Web
{
    public class UnhandledExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(context.Exception));
        }
    }
}
