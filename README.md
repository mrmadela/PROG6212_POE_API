
# RaceDay — Event Management System

## 1. Project Overview

RaceDay is an event management application developed using ASP.NET Core Web API and ASP.NET Core MVC.

The application allows organisers to manage events, categories and participant results. Participants can browse available events, enrol in event categories, view their enrolled events and access their results.

The solution uses a separate API and MVC frontend. The MVC application communicates with the API through HTTP requests.

## 2. Main Features

### Organiser
- Register and log in.
- Create, view, update and delete events.
- Manage event categories.
- View event enrolments.
- Record participant results.
- Upload event banners using Azure Blob Storage.

### Participant
- Register and log in.
- Browse upcoming events.
- Search and filter events.
- View event details and categories.
- Enrol in event categories.
- View enrolled events.
- View personal results.
- View and update profile information.

### Security
- Session-based authentication.
- Organiser and Participant roles.
- Role-based access restrictions.
- Organiser event ownership checks.
- Password hashing.
- Input validation on supported endpoints.

## 3. Technologies Used

| Technology | Purpose |
|---|---|
| C# | Main programming language |
| .NET 8 | Application framework |
| ASP.NET Core Web API | Backend API |
| ASP.NET Core MVC | Frontend application |
| Entity Framework Core | Database access |
| SQL Server | Relational database |
| Bootstrap | Responsive interface |
| HTML, CSS and JavaScript | Frontend development |
| Swagger / OpenAPI | API documentation |
| Azure Blob Storage | Event banner storage |
| xUnit | Automated testing |
| Git and GitHub | Version control |
| Docker | Containerisation |
| GitHub Actions | Continuous integration |

## 4. Solution Structure

The solution contains four projects:

~~~text
PROG6212_POE_API/
│
├── RaceDay.Api/
│   ├── Controllers/
│   ├── Data/
│   ├── Models/
│   ├── Services/
│   └── Dockerfile
│
├── RaceDay.Api.Tests/
│
├── RaceDay.Contracts/
│
├── RaceDay.Mvc/
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── Views/
│   ├── wwwroot/
│   └── Dockerfile
│
├── .github/
│   └── workflows/
│       └── dotnet.yml
│
├── .dockerignore
├── docker-compose.yml
├── RaceDay.sln
└── README.md
~~~

## 5. Prerequisites

Install the following software before running the project:

- .NET 8 SDK.
- Visual Studio 2022 or a compatible code editor.
- SQL Server or SQL Server LocalDB for local development.
- SQL Server Management Studio (optional).
- Git.
- Docker Desktop (optional, for container deployment).

Azure Blob Storage is required for testing event banner uploads.

## 6. Clone the Repository

Open a terminal and run:

~~~bash
git clone https://github.com/mrmadela/PROG6212_POE_API.git
~~~

Navigate into the project folder:

~~~bash
cd PROG6212_POE_API
~~~

Restore the solution dependencies:

~~~bash
dotnet restore RaceDay.sln
~~~

## 7. Database Configuration

RaceDay uses SQL Server through Entity Framework Core.

Configure the API connection string in the API configuration.

The connection string key is:

~~~text
ConnectionStrings:DefaultConnection
~~~

For local SQL Server LocalDB development, an example connection string is:

~~~text
Server=(localdb)\api_guide;Database=RaceDayDB;Trusted_Connection=True;TrustServerCertificate=True;
~~~

Adjust the server and database names to match your SQL Server installation.

### Important database note

Before applying migrations, verify that the existing database schema matches the EF Core model and that the migration history is correct.

Do not run database migrations against an existing database until this has been checked. Follow the project's database migration instructions and back up existing data before making schema changes.

## 8. API Configuration

The API reads its database and Azure settings from configuration.

Relevant configuration keys:

~~~text
ConnectionStrings:DefaultConnection
AzureBlobStorage:ConnectionString
AzureBlobStorage:EventBannersContainer
AzureBlobStorage:ProfilePhotosContainer
~~~

Example container configuration:

~~~json
{
  "AzureBlobStorage": {
    "EventBannersContainer": "event-banners",
    "ProfilePhotosContainer": "profile-photos"
  }
}
~~~

### Configure secrets using User Secrets

From the solution root, initialise User Secrets if they have not already been configured:

~~~bash
dotnet user-secrets init --project RaceDay.Api
~~~

Set the database connection string:

~~~bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_DATABASE_CONNECTION_STRING" --project RaceDay.Api
~~~

Set the Azure Blob Storage connection string:

~~~bash
dotnet user-secrets set "AzureBlobStorage:ConnectionString" "YOUR_AZURE_CONNECTION_STRING" --project RaceDay.Api
~~~

Replace the example values with your own credentials.

Never commit connection strings, passwords or Azure access keys to GitHub.

## 9. MVC Configuration

The MVC application needs the API's base URL.

Configuration key:

~~~text
ApiSettings:BaseUrl
~~~

For local development, the API URL is configured as:

~~~text
https://localhost:7111/
~~~

The MVC application is configured to run at:

~~~text
https://localhost:7154/
~~~

If your launch settings use different ports, update the URLs accordingly.

## 10. Running the Applications Locally

The API and MVC applications must both be running.

### Terminal 1 — Start the API

From the solution root:

~~~bash
dotnet run --project RaceDay.Api
~~~

Open Swagger:

http://localhost:7111/swagger

### Terminal 2 — Start the MVC application

From the solution root:

~~~bash
dotnet run --project RaceDay.Mvc
~~~

Open the application:

https://localhost:7154/

The MVC application communicates with the API using the configured API base URL.

## 11. API Documentation

Swagger provides an interactive interface for viewing and testing available API endpoints.

When the API is running locally, open:

http://localhost:7111/swagger

The API includes authentication, event management, categories, enrolments, results and user profile operations.

## 12. Azure Blob Storage

RaceDay uses Azure Blob Storage for event banner uploads.

Create an Azure Storage Account and configure the connection string using User Secrets or an environment variable.

The API configuration uses:

~~~text
AzureBlobStorage:ConnectionString
AzureBlobStorage:EventBannersContainer
AzureBlobStorage:ProfilePhotosContainer
~~~

The configured container names are:

- event-banners
- profile-photos

The connection string must remain private and must not be committed to source control.

## 13. Docker Instructions

Docker support is provided through separate API and MVC Dockerfiles and a Docker Compose configuration.

Prerequisites:
- Docker Desktop installed.
- Docker Desktop running.
- A SQL Server instance accessible from the containers.
- Azure credentials if testing banner uploads.

Create a local `.env` file based on `.env.example`.

Configure:

~~~dotenv
RACEDAY_DB_CONNECTION=YOUR_DATABASE_CONNECTION_STRING
AZURE_BLOB_CONNECTION_STRING=YOUR_AZURE_CONNECTION_STRING
~~~

Build the containers:

~~~bash
docker compose build
~~~

Start the applications:

~~~bash
docker compose up -d
~~~

Check running containers:

~~~bash
docker compose ps
~~~

View application logs:

~~~bash
docker compose logs -f
~~~

Access the applications:

- MVC: http://localhost:7154/
- API Swagger: http://localhost:7111/swagger

Stop the containers:

~~~bash
docker compose down
~~~

Note: SQL Server LocalDB cannot be accessed directly from a Linux container. Use a separately accessible SQL Server instance and configure its network access and connection string accordingly.

## 14. Automated Testing

The solution includes an xUnit test project.

Run all tests from the solution root:

~~~bash
dotnet test RaceDay.sln
~~~

To build the solution:

~~~bash
dotnet build RaceDay.sln
~~~

The GitHub Actions workflow also restores, builds and tests the solution when changes are pushed to the main branch or a pull request is opened against it.

## 15. GitHub Actions

The workflow is located at:

~~~text
.github/workflows/dotnet.yml
~~~

It performs these steps:

1. Checks out the repository.
2. Sets up the .NET 8 SDK.
3. Restores NuGet dependencies.
4. Builds the complete solution.
5. Runs automated tests.

The workflow fails if the build or tests fail.

## 16. Screenshots

Add application screenshots to:

~~~text
docs/screenshots/
~~~

Suggested screenshots:

- RaceDay home page.
- Registration page.
- Login page.
- Organiser dashboard.
- Event management.
- Category management.
- Participant event browsing.
- Event details and enrolment.
- Participant results.
- Profile management.
- Swagger API documentation.

Insert the screenshots below when they are available.

## 17. Repository Information

**Project:** RaceDay Event Management System

**GitHub Repository:**

https://github.com/mrmadela/PROG6212_POE_API

**Solution:** RaceDay.sln

**Framework:** .NET 8

## 18. Security Notes

- Do not commit passwords or connection strings.
- Keep Azure Storage credentials private.
- Use User Secrets for local development.
- Use environment variables or a secure secrets manager for deployment.
- Do not expose private storage containers publicly.
- Validate uploaded files and restrict their size and permitted formats.
- Review authentication and authorisation before production deployment.

## 19. Project Status

This README describes the intended setup and features of the RaceDay solution. Docker deployment and CI should be considered verified only after their build and execution checks have passed.

Further testing and deployment verification form part of the final project stages.




















<!-- start of code -->
# RaceDay API — PROG6212 PoE Part 2

## Project Description

RaceDay is a RESTful API for managing running, walking and cycling events.
It supports user registration, login, profiles, events, categories,
participant enrolments and race results.

## Technologies Used

- C#
- ASP.NET Core Web API
- .NET 8
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI
- xUnit
- GitHub Actions

## User Roles

### Organiser
- Manage their own events.
- Create and manage categories for their events.
- View enrolments for their own events.
- Record results for their events.

### Participant
- Browse events and categories.
- Manage their own profile.
- Enrol in categories.
- View their own enrolments and results.
- Cancel their own enrolment when cancellation rules permit.

## Project Structure

- `RaceDay.Api` — API controllers, models, data access and configuration.
- `RaceDay.Contracts` — request and response DTOs.
- `RaceDay.Api.Tests` — automated tests.
- `.github/workflows` — continuous integration workflow.

## Database Setup

The API uses SQL Server and the `RaceDayDB` database.

Update `RaceDay.Api/appsettings.json` with the SQL Server instance
available on your machine. Do not commit passwords or other secrets.

The development connection used for this project is:

`(localdb)\api_guide`

Database name: `RaceDayDB`.

Ensure the database has the tables and relationships from the Part 1
database design before running the API.

## How to Run the API

1. Install the .NET 8 SDK and SQL Server LocalDB.
2. Clone this repository.
3. Open `RaceDay.sln` in Visual Studio, or use a terminal.
4. Check the connection string in `RaceDay.Api/appsettings.json`.
5. Run:

   `dotnet restore RaceDay.sln`

   `dotnet build RaceDay.sln`

   `dotnet test RaceDay.sln`

6. Start the API:

   `dotnet run --project RaceDay.Api`

7. Open the HTTPS Swagger URL shown in the terminal.

## Authentication

Register an account and log in using the authentication endpoints.
The API hashes passwords and stores the logged-in user's ID and role
in a session.

Keep the session cookie when testing protected endpoints.

## API Endpoints

### Authentication
- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/logout`

### Profile
- `GET /api/users/profile`
- `PUT /api/users/profile`

### Events
- `GET /api/events`
- `GET /api/events/{id}`
- `GET /api/events/mine`
- `POST /api/events`
- `PUT /api/events/{id}`
- `DELETE /api/events/{id}`

### Categories
- `GET /api/events/{eventId}/categories`
- `POST /api/events/{eventId}/categories`
- `PUT /api/categories/{id}`
- `DELETE /api/categories/{id}`

### Enrolments
- `POST /api/enrolments`
- `GET /api/enrolments/mine`
- `GET /api/enrolments/event/{eventId}`
- `DELETE /api/enrolments/{id}`

### Results
- `POST /api/results`
- `GET /api/results/mine`
- `GET /api/results/event/{eventId}`

## Unit Testing

Run the automated tests using:

`dotnet test RaceDay.sln`

Tests should cover password hashing, registration, login, unauthorised
access, role restrictions, event creation and enrolment.

## CI/CD

GitHub Actions runs restore, build and test checks on pushes and pull
requests. See the Actions tab for the latest workflow result.

### GitHub Actions Screenshot

Add the screenshot of the successful workflow here after it passes.

## Video Presentation

Unlisted YouTube video: REPLACE_WITH_YOUR_UNLISTED_VIDEO_URL

## Repository

https://github.com/mrmadela/PROG6212_POE_API.git
<!-- end of code -->



























# PROG6212_POE_API
Application Programming Interface

BUSINESS ROLES 

Authentication & Users 

Every user must have a unique email address across the system. 

A user's role (Organiser or Participant) is fixed at registration and determines which profile record is linked to that account. 

Each user account links to exactly one profile record — an Organiser account never has a Participant profile, and vice versa. 

Login credentials (email, password) live only on the users record — they are never duplicated on the organiser or participant profile. 

Organisers 5. Only Organisers may create, update, or delete events. 6. Only the Organiser who created an event (the "owner") may update it, delete it, or manage its categories — other Organisers cannot. 

Participants 7. Any logged-in user (either role) may browse events and view available categories. 8. A Participant may only view or update their own profile, enrolments, and results — never another participant's. 

Events 9. Every event must capture a name, description, date, location, distance, and type (Run, Walk, or Cycle). 10. An event cannot be deleted while it still has categories or enrolments linked to it. 

Categories 11. Every category belongs to exactly one event. 12. A category must be either age-based or distance-based — never both, and never neither. 13. A category cannot be deleted while participants are enrolled in it. 

Event Enrolments 14. A participant may enrol in a given category only once (no duplicate enrolments for the same participant + category). 15. Every enrolment records the participant, the event, and the category together — the event is stored directly, not just inferred through the category. 16. A participant may cancel their own enrolment, but not once a result has already been captured against it. 17. Only the Organiser who owns the event may view the full list of enrolments for it. 

Results 18. A result can only be captured for an enrolment that already exists. 19. Each enrolment can have at most one result. 20. Only the Organiser who owns the related event may capture or update a result. 21. A participant may only view their own results, never another participant's. 
