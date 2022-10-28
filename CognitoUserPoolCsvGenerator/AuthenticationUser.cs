using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Amazon.CognitoIdentityProvider.Model;

namespace CognitoUserPoolCsvGenerator
{
    public record AuthenticationUser
    {
        public int ASFSAUserID { get; set; }

        [CognitoUserAttribute("custom:aura_ci")]
        public int ID { get; set; }

        private string Username { get; set; }

        public string Password { get; set; }
        
        [CognitoUserAttribute("given_name")]
        public string FirstName { get; set; }
        
        [CognitoUserAttribute("family_name")]
        public string LastName { get; set; }

        [CognitoUserAttribute("email")]
        public string Email1 { get; set; }

        public string PreferredAddressLine1 { get; set; }
        public string PreferredAddressLine2 { get; set; }
        public string PreferredCity { get; set; }
        public string PreferredState { get; set; }
        public string PreferredZip { get; set; }
        public string PreferredCountry { get; set; }
        public string CompanyName { get; set; }
        public string Status { get; set; }
        public DateTime DuesPaidThru { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string ReturnURL { get; set; }
        public int LastUpdatedBy { get; set; }
        public string Title { get; set; }
        public string PhoneAreaCode { get; set; }
        public string Phone { get; set; }
        public string FaxAreaCode { get; set; }
        public string FaxPhone { get; set; }
        public string Chapter { get; set; }
        public string DirectorFoodService { get; set; }
        public string TrendSetter { get; set; }
        public int CompanyID { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public bool SNNUserAgreement { get; set; }
        public DateTime SNNUserAgreementDate { get; set; }
        public int ReferredBy { get; set; }

        public static IEnumerable<AttributeType> ToUserAttribute(AuthenticationUser user)
        {
            var properties = typeof(AuthenticationUser)
                .GetProperties()
                .Where(p => p.GetCustomAttribute<CognitoUserAttributeAttribute>() != null);

            return properties.Select(p => new AttributeType
            {
                Name = p.GetCustomAttribute<CognitoUserAttributeAttribute>()?.Name,
                Value = Convert.ToString(p.GetValue(user))
            });
        }

        /// <summary>
        /// Creates a unique user name for Cognito User
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// https://github.com/aurachicago/Aura.Auth new user creation follows this pattern of creating a unique 'user name' as a guid
        /// then using Attributes of the user to allow login by the 'email' attribute.
        /// </remarks>
        public static string CreateUniqueUserName() => Guid.NewGuid().ToString();
    }
}
