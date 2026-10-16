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
