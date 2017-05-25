﻿using System.ComponentModel;
using System.Web.Mvc;

namespace uCommunity.Core.ModelBinders
{
    public interface ICustomModelBinder
    {
        object BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor);
    }
}