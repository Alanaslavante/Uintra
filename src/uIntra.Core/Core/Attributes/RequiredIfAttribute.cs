﻿using System.ComponentModel.DataAnnotations;

namespace Uintra.Core.Attributes
{
    public class RequiredIfAttribute : RequiredAttribute
    {
        private string PropertyName { get; set; }
        private object DesiredValue { get; set; }

        public RequiredIfAttribute(string propertyName, object desiredValue)
        {
            PropertyName = propertyName;
            DesiredValue = desiredValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            var type = instance.GetType();
            var propertyValue = type.GetProperty(PropertyName).GetValue(instance, null);
            if (propertyValue.ToString() == DesiredValue.ToString())
            {
                var result = base.IsValid(value, context);
                return result;
            }
            return ValidationResult.Success;
        }
    }
}
