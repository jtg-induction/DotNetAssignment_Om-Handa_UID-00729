# DotNet Assignment- Restaurant Portal

A RESTful API Application to manage uesrs, restaurant, menu, orders. Built using .NET Framework 4.8 and Entity Framework 6.

## Features
- User registration and login
- JWT based authentication
- User profile updation
- User account deactivation
- JWT based authorisation
- Three roles: User, Admin and Owners
- Entity Framework for Database management
- Unit test cases for all services, controllers, repositories

## Tech Stack
- .Net Framework 4.8
- Entity Framework 6
- JWT
- Owin
- SQL Server
- NUnit
- Moq

## Folder Structure

```
DotNetAssignment_Om-Handa_UID-00729/
├── .gitattributes
├── .gitignore
├── DotNet Assignment.Tests/
│   ├── DotNet Assignment.Tests.csproj
│   ├── Properties/
│   │   └── AssemblyInfo.cs
│   ├── UnitTest1.cs
│   ├── app.config
│   └── packages.config
├── DotNet Assignment.slnx
├── DotNet Assignment/
│   ├── App_Start/
│   │   ├── UnityConfig.cs
│   │   └── WebApiConfig.cs
│   ├── Constants/
│   │   ├── ExceptionMessages.cs
│   │   ├── Regex.cs
│   │   └── SuccessMessages.cs
│   ├── Controllers/
│   │   ├── .gitkeep
│   │   ├── AddressController.cs
│   │   ├── AuthController.cs
│   │   └── UserController.cs
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── DotNet Assignment.csproj
│   ├── Global.asax
│   ├── Global.asax.cs
│   ├── Handlers/
│   │   ├── GlobalExceptionHandler.cs
│   │   └── ModelStateHandler.cs
│   ├── Migrations/
│   ├── Models/
│   │   ├── DTO/
│   │   │   ├── .gitkeep
│   │   │   ├── AddressDto.cs
│   │   │   ├── ApiResponseDto.cs
│   │   │   ├── ChangePasswordDto.cs
│   │   │   ├── JWTResponseDto.cs
│   │   │   ├── LoginRequestDto.cs
│   │   │   ├── LogoutRequestDto.cs
│   │   │   ├── RefreshTokenDto.cs
│   │   │   ├── SignupRequestDto.cs
│   │   │   └── UpdateUserDto.cs
│   │   ├── Entities/
│   │   │   ├── MenuItem.cs
│   │   │   ├── Order.cs
│   │   │   ├── OrderedItem.cs
│   │   │   ├── RefreshToken.cs
│   │   │   ├── Restaurant.cs
│   │   │   ├── RestaurantOwner.cs
│   │   │   ├── User.cs
│   │   │   └── UserAddress.cs
│   │   └── Enums/
│   │       ├── OrderStatus.cs
│   │       └── UserRoles.cs
│   ├── Properties/
│   │   └── AssemblyInfo.cs
│   ├── Repository/
│   │   ├── .gitkeep
│   │   ├── Address/
│   │   │   ├── AddressRepository.cs
│   │   │   └── IAddressRepository.cs
│   │   ├── RefreshTokens/
│   │   │   ├── IRefreshTokenRepository.cs
│   │   │   └── RefreshTokenRepository.cs
│   │   └── Users/
│   │       ├── IUserRepository.cs
│   │       └── UserRepository.cs
│   ├── Services/
│   │   ├── .gitkeep
│   │   ├── Address/
│   │   │   ├── AddressService.cs
│   │   │   └── IAddressService.cs
│   │   ├── Auth/
│   │   │   ├── AuthService.cs
│   │   │   └── IAuthService.cs
│   │   ├── JWT/
│   │   │   ├── IJWTService.cs
│   │   │   └── JWTService.cs
│   │   └── Users/
│   │       ├── IUserService.cs
│   │       └── UserService.cs
│   ├── Startup.cs
│   ├── Utils/
│   │   └── Hasher.cs
│   ├── Web.Debug.config
│   ├── Web.Release.config
│   ├── Web.config
│   └── packages.config
└── README.md

```

## Database

The application uses SQL Server with Entity Framework 6 Code First

Main Entities Include

- User
- UserAddress
- Restaurant
- MenuItem
- Order
- OrderItems
- RefreshToken
- Restaurant Owners

## API Endpoints

| Method | Endpoint                  |
|--------|---------------------------|
| POST   | /api/auth/signup          |
| POST   | /api/auth/login           |
| POST   | /api/auth/logout          |
| POST   | /api/auth/refresh         |
| POST   | /api/address/add          |
| PATCH  | /api/address/{id}         |
| PATCH  | /api/user/update          |
| PATCH  | /api/user/deactivate      |
| PATCH  | /api/user/change-password |

## Setup

### Prerequisites

- Visual Studio
- .NET Framework 4.8
- SQL Server

### Clone the repository

```
git clone https://github.com/jtg-induction/DotNetAssignment_Om-Handa_UID-00729.git
```
### Configure Database

Add a file - ConnectionString.config and use the following template

```
<connectionStrings>
	<add name="DatabaseName"
		connectionString=..."
	    providerName="System.Data.SqlClient"/>
</connectionStrings>
```

### Run Migrations

Open Package Manager Console and run following command

```
Update-Database
```

### Configure JWT security keys

Add a file - Secrets.config and use the following template

```
<appSettings>
	<add key="JwtKey" value="..." />
	<add key="JwtIssuer" value="..." />
</appSettings>
```

### Run the Application

Open visual studio and run the application

## Built By

Om Handa UID00729