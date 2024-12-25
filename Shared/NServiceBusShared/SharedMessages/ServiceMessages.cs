﻿namespace SharedMessages
{
    //____________________________UserManagement Events____________________________//
    public class SignupCompleted : IEvent
    {
        public SignupCompleted(Guid leadId, string name, string email,
            string password, Guid addressId, string street, string city,
            string postalCode)
        {
            UserId = leadId;
            Name = name;
            Email = email;
            Password = password;
            AddressId = addressId;
            Street = street;
            City = city;
            PostalCode = postalCode;
        }

        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid AddressId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }

    public class UserValidated : IEvent
    {
        public Guid UserId { get; set; }
    }

    public class UserValidationFailed : IEvent
    {
        public Guid UserId { get; set; }
        public string Reason { get; set; }
    }

    //____________________________UserManagement Commands____________________________//

    public class ValidateUser : ICommand
    {
        public string Email { get; set; }
    }

    public class SignupCommand : ICommand
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }
    //____________________________PaymentService Events____________________________//
    public class PaymentProcessed : IEvent
    {
        public Guid PaymentId { get; set; }
    }

    public class PaymentFailed : IEvent
    {
        public Guid PaymentId { get; set; }
        public string Reason { get; set; }
    }

    //____________________________PaymentService Commands____________________________//

    public class ProcessPayment : ICommand
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
    }


    //____________________________OrderService Events____________________________//


    //____________________________OrderService Commands____________________________//

}
