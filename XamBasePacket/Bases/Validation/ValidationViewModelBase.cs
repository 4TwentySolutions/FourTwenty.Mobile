using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace XamBasePacket.Bases.Validation
{
    public abstract class ValidationViewModelBase : ViewModelBase
    {
        public readonly Dictionary<string, List<string>> Errors = new Dictionary<string, List<string>>();

        protected virtual bool IsModelValid(bool displayErrors = false)
        {

            bool result = true;
            try
            {
                var properties = this.GetType().GetProperties()
                .Where(prop => prop.IsDefined(typeof(ValidationAttribute), false));
                ClearErrors();
                foreach (PropertyInfo info in properties)
                {
                    var attributes = (info.GetCustomAttributes(typeof(ValidationAttribute), false) as ValidationAttribute[]);
                    if (attributes != null)
                        ValidateAttributes(info, attributes, ref result);
                }
                if (!result && displayErrors)
                    DisplayValidationErrors();
            }
            catch (Exception ex)
            {
                // ignored
            }
            return result;
        }

        private void ValidateAttributes(PropertyInfo property, ValidationAttribute[] attributes, ref bool result, object childItem = null)
        {
            if (attributes == null)
                return;
            if (attributes.Any(x => x.GetType() == typeof(ValidableClassAttribute)))
            {
                var propertyValue = property.GetValue(this, null);

                var childProps = propertyValue?.GetType()?.GetProperties()?.Where(prop => prop.IsDefined(typeof(ValidationAttribute), false))?.ToList();
                if (childProps != null && childProps.Any())
                {
                    foreach (PropertyInfo prop in childProps)
                    {
                        var childAttrs = (prop.GetCustomAttributes(typeof(ValidationAttribute), false) as ValidationAttribute[]);
                        ValidateAttributes(prop, childAttrs, ref result, propertyValue);
                    }
                }
            }

            foreach (ValidationAttribute attribute in attributes)
            {
                var val = property.GetValue(childItem ?? this, null);
                attribute.Validate(val);
                if (!attribute.IsValid)
                {
                    result = attribute.IsValid;
                    if (Errors.ContainsKey(property.Name))
                    {
                        if (Errors[property.Name] == null)
                        {
                            if (attribute.ErrorResourceType != null && !string.IsNullOrEmpty(attribute.ErrorResourceName) && !string.IsNullOrEmpty(attribute.ErrorResourceType.FullName))
                            {
                                ResourceManager resourceManager = new ResourceManager(attribute.ErrorResourceType.FullName, attribute.ErrorResourceType.GetTypeInfo().Assembly);
                                var res = resourceManager.GetString(attribute.ErrorResourceName, Thread.CurrentThread.CurrentUICulture);
                                Errors[property.Name] = new List<string>() { res };
                            }
                            else
                            {

                                Errors[property.Name] = new List<string>() { attribute.ErrorMessage };
                            }
                        }
                        else
                        {
                            if (attribute.ErrorResourceType != null &&
                                !string.IsNullOrEmpty(attribute.ErrorResourceName) &&
                                !string.IsNullOrEmpty(attribute.ErrorResourceType.FullName))
                            {
                                ResourceManager resourceManager = new ResourceManager(attribute.ErrorResourceType.FullName, attribute.ErrorResourceType.GetTypeInfo().Assembly);
                                var res = resourceManager.GetString(attribute.ErrorResourceName, Thread.CurrentThread.CurrentUICulture);
                                Errors[property.Name].Add(string.Format(res));
                            }
                            else
                            {

                                Errors[property.Name].Add(string.Format(attribute.ErrorMessage));
                            }

                        }
                    }
                    else
                    {
                        if (attribute.ErrorResourceType != null && !string.IsNullOrEmpty(attribute.ErrorResourceName) && !string.IsNullOrEmpty(attribute.ErrorResourceType.FullName))
                        {
                            ResourceManager resourceManager = new ResourceManager(attribute.ErrorResourceType.FullName, attribute.ErrorResourceType.GetTypeInfo().Assembly);
                            var res = resourceManager.GetString(attribute.ErrorResourceName, Thread.CurrentThread.CurrentUICulture);
                            Errors.Add(property.Name, new List<string>() { res });
                        }
                        else
                        {
                            Errors.Add(property.Name, new List<string>() { attribute.ErrorMessage });
                        }

                    }
                }

            }
        }




        protected virtual void DisplayFirstValidationError()
        {
            if (Errors.Any())
            {
                ErrorText = Errors.First().Value?.FirstOrDefault();
            }
        }
        protected virtual void DisplayValidationErrors()
        {
            if (Errors.Any())
            {
                ErrorText = string.Join("\n", Errors.SelectMany(x => x.Value).Select(x => x));
            }
        }

        protected override void ClearErrors()
        {
            Errors.Clear();
            base.ClearErrors();
        }
    }
}
