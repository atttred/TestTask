# Incident Management API

This is a simple API for managing incidents, accounts, and contacts using **ASP.NET Core** with **EF Core** and **SQLite**.

## Database Structure

- **Incident → Account**: One incident can have many accounts.
- **Account → Contact**: One account can have many contacts.
- **Incident**:
  - Primary key: auto-generated, unique `Name` (string).
- **Account**:
  - Unique `Name` (string).
  - Linked to an `Incident`.
- **Contact**:
  - Unique `Email` (string).
  - Linked to an `Account`.

## Features

- Create incidents, accounts, and contacts.
- **Validations**:
  - Account cannot exist without a contact.
  - Incident cannot be created without an account.
- If the contact exists (checked by email), it updates the contact and links it to the account if not linked.
- Otherwise:
  - Creates a new contact.
  - Links it to the account.
  - Associates the account with the incident.

## Tools

- **Framework**: ASP.NET Core
- **Database**: SQLite
- **ORM**: Entity Framework Core (Code First)
