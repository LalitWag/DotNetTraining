namespace Project4.MauiApps.Views
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public static class ValidationHelper
    {
        public static bool ValidateStudent(CommonLogic.Student student, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            var validationContext = new ValidationContext(student, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(student, validationContext, validationResults, validateAllProperties: true))
            {
                foreach (var validationResult in validationResults)
                {
                    errorMessages.Add(validationResult.ErrorMessage);
                }

                return false; // Validation failed
            }

            return true; // Validation passed
        }
    }
}
