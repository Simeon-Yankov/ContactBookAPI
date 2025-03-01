namespace ContactBookAPI.Domain.Constants;
public class DomainConstants
{
    public class Person
    {
        public const int MaxFullNameLength = 70;
    }

    public class Address
    {
        public const int MinAddressLength = 2;
        public const int MaxAddressLength = 256;
    }

    public class PhoneNumber
    {
        public const int MinPhoneNumberLength = 5;
        public const int MaxPhoneNumberLength = 20;
        public const string PhoneNumberRegularExpression = @"^\+\d+$";
    }
}
