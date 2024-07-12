// <copyright file="MandateAuthorizationFilterAttribute.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using System.Net;
    using Kpmg.Constellation.Security.Claims;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class MandateAuthorizationFilterAttribute : ActionFilterAttribute
    {
        public MandateAuthorizationFilterAttribute()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            var descriptor = (ControllerActionDescriptor)context.ActionDescriptor;

            if (!user.IsSystemAccount() && IsManagementActionCall(descriptor))
            {
                context.Result = new ObjectResult($"You are not allowed to execute the {descriptor.ControllerName} => {descriptor.ActionName} endPoint")
                {
                    StatusCode = (int)HttpStatusCode.Forbidden,
                };
            }

            base.OnActionExecuting(context);
        }

        private static bool IsManagementActionCall(ControllerActionDescriptor descriptor)
        {
            return descriptor.ControllerName == "Mandate" &&
                     (descriptor.AttributeRouteInfo?.Template == "api/mandate/technical" ||
                     descriptor.AttributeRouteInfo?.Template == "api/mandate/recovery-form-io" ||
                     descriptor.AttributeRouteInfo?.Template == "api/mandate/recovery" ||
                     descriptor.AttributeRouteInfo?.Template == "api/mandate/refresh-mandates-statuses");
        }
    }
}
