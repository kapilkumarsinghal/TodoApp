# Todo Tracker

A full-stack Todo application built with:

- ASP.NET Core Web API
- Angular frontend
- SQL Server database

## Prerequisites

- .NET 8 SDK
- Node.js 22+
- Docker Desktop (for the SQL Server container)

## Start SQL Server

```bash
docker compose up -d sqlserver
```

## Backend

From the `TodoApp.Api` folder:

```bash
dotnet restore
dotnet run
```

The API will be available at:

- http://localhost:5160
- https://localhost:7251

Swagger UI:

- http://localhost:5160/swagger

## Frontend

From the `todo-frontend` folder:

```bash
npm install
npm start
```

Then open:

- http://localhost:4200

## Database config

The default connection string uses a local SQL Server container:

```json
"Server=localhost,1433;Database=TodoAppDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
```

## Features

- Create tasks
- View all tasks
- Mark tasks complete or incomplete
- Delete tasks
- Sort by date and priority in the API response
