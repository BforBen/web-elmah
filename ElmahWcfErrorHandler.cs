using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace GuildfordBoroughCouncil.Web
{
    /// <summary>
    /// Based on http://lbenotes.wordpress.com/2012/01/30/getting-elmah-to-work-with-wcf-services-hosted-in-iis/
    /// </summary>
    public class ElmahWcfErrorHandler : IErrorHandler
    {
        public bool HandleError(Exception error)
        {
            return false;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if (error == null)
                return;

            ///In case we run outside of IIS, 
            ///make sure aspNetCompatibilityEnabled="true" in web.config under system.serviceModel/serviceHostingEnvironment
            ///to be sure that HttpContext.Current is not null
            if (HttpContext.Current == null)
                return;

            Elmah.ErrorSignal.FromCurrentContext().Raise(error);
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceErrorBehaviorAttribute : Attribute, IServiceBehavior
    {
        Type errorHandlerType;

        public ServiceErrorBehaviorAttribute(Type errorHandlerType)
        {
            this.errorHandlerType = errorHandlerType;
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
           System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) { }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandler;
            errorHandler = (IErrorHandler)Activator.CreateInstance(errorHandlerType);
            foreach (ChannelDispatcherBase cdb in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher cd = cdb as ChannelDispatcher;
                cd.ErrorHandlers.Add(errorHandler);
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }
    }
}
