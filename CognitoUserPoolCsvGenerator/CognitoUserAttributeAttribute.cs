using System;

namespace CognitoUserPoolCsvGenerator
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CognitoUserAttributeAttribute : Attribute
    {
        public string Name { get; set; }

        public CognitoUserAttributeAttribute(string name)
        {
            Name = name;
        }
    }
}
