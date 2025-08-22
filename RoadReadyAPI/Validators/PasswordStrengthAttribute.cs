using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RoadReadyAPI.Validators
{
    public class PasswordStrengthAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var password = value as string;
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            // Our custom logic without regex
            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            if (password.Length < 8 || !hasUpperCase || !hasLowerCase || !hasDigit || !hasSpecialChar)
            {
                ErrorMessage = "Password must be at least 8 characters long and contain an uppercase letter, a lowercase letter, a number, and a special character.";
                return false;
            }

            return true;
        }
    }
}