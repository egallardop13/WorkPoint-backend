# WorkPoint Backend 

This repository contains the backend implementation for the WorkPoint application, enabling secure data management, API endpoints, and enterprise analytics. The backend is built using **.NET** and connects to a **SQL Express** database for robust and scalable operations.

## Features

- **RESTful API**: Provides endpoints for managing budgets, employee activities, and other enterprise data.
- **Authentication & Authorization**: Secure user authentication and role-based access control.
- **Data Management**: CRUD operations for employees, budgets, and departments.
- **Logging and Monitoring**: Built-in logging for monitoring application activity.

## Technology Stack

- **.NET**: Core framework for building APIs.
- **SQL Express**: Relational database for data persistence.
- **Azure**: Backend hosting for production deployments.

## Prerequisites

To set up and run the backend locally, ensure you have the following:

- **.NET SDK**: [Download here](https://dotnet.microsoft.com/download)
- **SQL Express**: [Download here](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- **Postman** or similar API testing tools (optional).

## Installation & Setup

Follow these steps to set up the backend locally:

1. **Clone the repository**
   ```bash
   git clone https://github.com/egallardop13/WorkPoint-backend.git
   cd DotnetApi
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Create an `appsettings.json` file**
   If the file is not included in the repository, create it in the root directory with the following structure:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=YOUR_DATABASE_NAME;Trusted_Connection=True;"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft": "Warning",
         "Microsoft.Hosting.Lifetime": "Information"
       }
     },
     "AllowedHosts": "*"
   }
   ```

4. **Update the database connection**
   Replace `YOUR_SERVER_NAME` and `YOUR_DATABASE_NAME` in the `appsettings.json` file with your local SQL Express instance details.

5. **Start the backend server**
   ```bash
   dotnet run
   ```

6. **Test the API**
   Once the server is running, access the API at `http://localhost:5000`. Use Postman or similar tools to test the endpoints.

## Contact

For any inquiries or support, reach out to [egallardodev@gmail.com](mailto:egallardodev@gmail.com).
