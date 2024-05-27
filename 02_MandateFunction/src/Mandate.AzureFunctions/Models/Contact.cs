// <copyright file="Contact.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function;

public class Contact
{
    public Contact(int id, string firstName, string lastName, string email, bool isActive)
    {
        this.Id = id;
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Email = email;
        this.IsActive = isActive;
    }

    public int Id { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string Email { get; }

    public bool IsActive { get; }

}
