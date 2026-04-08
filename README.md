# WorkPoint Backend

REST API powering the WorkPoint enterprise dashboard. Built with .NET 8, Dapper, and SQL Server.

## Architecture

```
Controllers (HTTP layer)
    |
Services (business logic, injected via DI)
    |
IDataContextDapper (data access interface)
    |
DataContextDapper → Dapper → SQL Server stored procedures
```

**Key patterns:**
- Service layer with constructor-injected `IDataContextDapper` for testability
- Stored procedures for all database operations (`WorkPointSchema.*`)
- JWT Bearer authentication with PBKDF2 password hashing and refresh token rotation
- Rate limiting on auth endpoints (5 req/min per IP)

## API Endpoints

### Auth (`/auth`) — Public

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/auth/Register` | Register new user |
| POST | `/auth/Login` | Authenticate, returns JWT + refresh token |
| POST | `/auth/Refresh` | Exchange refresh token for new access token |
| PUT | `/auth/ResetPassword` | Change password (authenticated) |
| GET | `/auth/RefreshToken` | Generate new access token (authenticated) |

### Users (`/usercomplete`) — Authenticated

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/usercomplete/GetUsers/{userId}/{isActive}` | List users with optional filters |
| GET | `/usercomplete/GetUsersWithPagination/{page}/{limit}?query=&sort=` | Paginated user list |
| PUT | `/usercomplete/UpsertUser` | Create or update user |
| DELETE | `/usercomplete/DeleteUser/{userId}` | Delete user |

### Company (`/company`) — Authenticated

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/company/GetCompanyInfo` | Company overview stats |
| GET | `/company/GetMetrics/{year}/{status}` | Yearly metrics with monthly breakdown |
| GET | `/company/GetBudget/{year}` | Budget data by year |

### Posts (`/post`) — Authenticated

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/post/Posts/{postId}/{userId}/{searchParam}` | List posts with filters |
| GET | `/post/MyPosts` | Posts by current user |
| PUT | `/post/UpsertPost` | Create or update post |
| DELETE | `/post/Post/{postId}` | Delete post (owner only) |

### Salary & Departments — Authenticated

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/usersalary/GetUsersSalary` | All salary records |
| GET | `/usersalary/GetDepartmentsInfo/{department?}?query=&sort=` | Department stats |
| DELETE | `/usersalary/DeleteUserSalary/{userId}` | Delete salary record |
| GET | `/userjobinfo/GetUsersInDepartments/{dept}/{page}/{limit}?query=` | Users in department (paginated) |
| GET | `/userjobinfo/GetUsersJobInfo` | All job info records |
| DELETE | `/userjobinfo/DeleteUserJobInfo/{userId}` | Delete job info record |

### Health — Public

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/health` | JSON health status with SQL Server check |
| GET | `/ping` | Liveness probe (returns `"pong"`) |

## Authentication Flow

1. `POST /auth/Register` — creates user with PBKDF2-hashed password
2. `POST /auth/Login` — validates credentials, returns JWT access token (expires 1h) + refresh token (expires 7d)
3. Client sends `Authorization: Bearer <token>` on all subsequent requests
4. `POST /auth/Refresh` — exchange expired access token for a new one using the refresh token
5. Refresh tokens are single-use (revoked on use, new one issued)

## Database

- **SQL Server** with schema `WorkPointSchema`
- Tables: `Users`, `UserSalary`, `UserJobInfo`, `Posts`, `Auth`, `RefreshTokens`
- All reads/writes go through stored procedures (`spUsers_Get`, `spUser_Upsert`, `spPost_Get`, etc.)
- Dapper for lightweight ORM with parameterized queries

## Testing

42 tests (xUnit + Moq + FluentAssertions):

```bash
dotnet test DotnetAPI.Tests/
```

- **Unit tests:** Services (UserService, CompanyService, PostService, SalaryService), Controllers (CompanyController, UserCompleteController), Helpers (DataParserHelper)
- **Integration tests:** HTTP pipeline via `WebApplicationFactory<Program>` — health endpoints, auth middleware (401 on protected routes)

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Setup

```bash
git clone https://github.com/egallardop13/WorkPoint-backend.git
cd WorkPoint-backend
dotnet restore
```

Create `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=WorkPoint;Trusted_Connection=True;"
  },
  "AppSettings": {
    "TokenKey": "your-64-byte-minimum-secret-key",
    "PasswordKey": "your-password-salt-key"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

```bash
dotnet run
```

API available at `http://localhost:5000`.

## Deployment

Deployed on **Azure App Service** (B1 tier):

- Always On enabled for cold start mitigation
- `web.config` pre-warms `/ping` on restart
- Health check path: `/ping`
- Required environment variables: `ConnectionStrings__DefaultConnection`, `AppSettings__TokenKey`, `AppSettings__PasswordKey`

## Contact

[egallardodev@gmail.com](mailto:egallardodev@gmail.com)
