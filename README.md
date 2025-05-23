# Clinic Management System API

A comprehensive clinic management system built with ASP.NET Core 8 Web API using Clean Architecture principles.

## Project Structure

The solution is organized into four main projects:

1. **ClinicManagementAPI.Domain**
   - Contains all entities, enums, and domain interfaces
   - No dependencies on other projects

2. **ClinicManagementAPI.Application**
   - Contains business logic and interfaces
   - Depends on Domain project
   - Contains DTOs, interfaces, and application services

3. **ClinicManagementAPI.Infrastructure**
   - Contains implementations of interfaces
   - Depends on Application and Domain projects
   - Contains database context, repositories, and external service implementations

4. **ClinicManagementAPI.API**
   - Contains controllers and API endpoints
   - Depends on all other projects
   - Contains API configurations and middleware

## Features

- User authentication and authorization using JWT
- Role-based access control (Admin, Doctor, Patient)
- CRUD operations for Doctors, Patients, Appointments, and Diagnoses
- Appointment management system
- Diagnosis and prescription management
- Notification system
- Swagger documentation
- Global error handling
- Logging
- Unit testing

## Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 or VS Code

## Getting Started

1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Run the following commands:
   ```bash
   dotnet restore
   dotnet ef database update
   dotnet run
   ```
4. Access the Swagger UI at `/swagger`

## API Documentation

The API documentation is available through Swagger UI when running the application. Detailed endpoint information and request/response examples are provided.

## Testing

Run the tests using:
```bash
dotnet test
```

## License

This project is licensed under the MIT License. 