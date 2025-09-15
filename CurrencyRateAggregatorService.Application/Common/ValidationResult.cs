using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyRateAggregatorService.Application.Common
{
    public class ValidationResult<T>
    {
        public bool IsValid { get; }
        public string? ErrorMessage { get; }
        public T? Value { get; }

        private ValidationResult(bool isValid, T? value, string? errorMessage)
        {
            IsValid = isValid;
            Value = value;
            ErrorMessage = errorMessage;
        }

        public static ValidationResult<T> Success(T value) => new(true, value, null);
        public static ValidationResult<T> Fail(string message) => new(false, default, message);
    }
}
