# Blogging Engine API

A robust, single-user blogging engine implemented in .NET 7, following SOLID principles and featuring both REST and GraphQL APIs.

## Tech Stack
- **Framework**: .NET 7 (ASP.NET Core)
- **Database**: PostgreSQL 15
- **ORM**: Entity Framework Core
- **API**: 
  - REST (Swagger/OpenAPI)
  - GraphQL (HotChocolate)
- **Infrastructure**: Docker & Docker Compose
- **Other**: JWT Authentication, DotNetEnv (Environment management), BCrypt.Net (Password hashing)

## Features
- **Articles**: Full CRUD operations for blog posts.
- **Comments**: Users can post comments on articles.
- **Voting**: Reddit-style voting (+1/-1) on comments, unique per IP address.
- **Real-time**: GraphQL Subscriptions for live updates on comments and votes.
- **Security**: JWT-based authentication for administrative actions.
- **Documentation**: Comprehensive Swagger UI and GraphQL schema docs.

## Prerequisites
- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [Docker & Docker Compose](https://docs.docker.com/get-docker/)

## Getting Started

### 1. Environment Configuration
Create a `.env` file in the root directory (one is already provided in the setup) with the following content:
```env
DefaultConnection="Host=db;Database=blogdb;Username=yh;Password=root"
JWT_KEY=ThisIsAVerySecretKeyForMyBloggingEngine2026
JWT_ISSUER=BlogAPI
JWT_AUDIENCE=BlogApp
```

### 2. Running with Docker Compose
The easiest way to get the API and Database running:
```bash
docker-compose up --build
```
The API will be available at `http://localhost:5000`.

### 3. Running Locally (Development)
Ensure you have a PostgreSQL instance running as specified in the `.env`.
```bash
dotnet restore
dotnet build
cd BlogAPI
dotnet run
```

## API Documentation
- **REST API**: [http://localhost:5000/swagger](http://localhost:5000/swagger)
- **GraphQL IDE**: [http://localhost:5000/graphql](http://localhost:5000/graphql)

## Architecture Notes
- **SOLID Principles**: The project uses a Service Layer abstraction to decouple business logic from API controllers and resolvers.
- **Global Exception Handling**: Custom middleware handles all unhandled errors and returns structured `ProblemDetails` responses.
- **Structured Logging**: `ILogger` is integrated throughout the application for audit trails and debugging.
- **Dependency Injection**: Fully utilized for service registration and management.

## Testing
To run the unit tests:
```bash
dotnet test
```
