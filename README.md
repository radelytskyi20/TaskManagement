# TaskManagement

You are tasked with developing a .NET backend service that manages a simple task management
system with user authentication. The system should allow users to create, update, delete, and
query tasks associated with their accounts. This assignment will evaluate your skills in .NET
backend development, SQL database management, API design, and user management.

# Project Setup

## 1. Clone the repository

Open your terminal/command prompt and run the following command to clone the repository:
```bash
git clone <repository-url>
```

## 2. Move to the project directory

```bash
cd <project-directory>
```

## 3. Run Docker Compose

Make sure Docker is installed and running on your machine. Then execute the following command to run containers:
```bash
docker-compose up
```
## 4. Apply Database Migrations

Open the Package Manager Console in Visual Studio and run:
```bash
Update-Database
```

Startup project: **TaskManagement.Api**
Default project: **Infrastructure\\TaskManagement.Persistance**

## 5. Application is ready

You can access the API at:
```bash
http://localhost:5003
```

You can also use use SwaggerUI:
```bash
http://localhost:5003/swagger
```
