# TaskManagement

You are tasked with developing a .NET backend service that manages a simple task management
system with user authentication. The system should allow users to create, update, delete, and
query tasks associated with their accounts. This assignment will evaluate your skills in .NET
backend development, SQL database management, API design, and user management.

## Project Setup

**1. Clone the repository**

Open your terminal/command prompt and run the following command to clone the repository:
```bash
git clone <repository-url>
```

**2. Move to the project directory** 
```bash
cd <project-directory>
```

**3. Run Docker Compose**

Make sure Docker is installed and running on your machine. Then execute the following command to run containers:
```bash
docker-compose up
```
**4. Apply Database Migrations**

Open the Package Manager Console in Visual Studio and run:
```bash
Update-Database
```

Startup project: **TaskManagement.Api**

Default project: **Infrastructure\\TaskManagement.Persistance**

**5. Application is ready**

You can access the API at:
```bash
http://localhost:5003
```

You can also use use SwaggerUI:
```bash
http://localhost:5003/swagger
```

## Api endpoints

### Health Check

Returns health status of the application.

- URL
    
    `/health`

- Method

    `GET`

- URL Params
    
    `None`

- Success Response

    **Code:** 200 OK
    
    Content: `{ "Api is online" }`

- Error Response

    **Code:** 500 INTERNAL SERVER ERROR
    
    **Content:** `{ "An error has occured while handling a request. All required information has been logged. Please contact an application administrator." }`

### User Registration

Registers a new user by creating a user account with a username, email, and password.

- **URL**

    `/users/register`

- **Method**

    `POST`

- **URL Params**

    `None`

- **Data Params**

    ```json
    {
      "username": "string",
      "email": "string",
      "password": "string"
    }
    ```

- **Success Response**

    **Code:** 201 CREATED
    
    Content: `None`

- **Error Response**

    **Code:** 400 BAD REQUEST
    
    **Content:** `[ "Error messages" ]`

    OR

    **Code:** 500 INTERNAL SERVER ERROR
    
    **Content:** `{ "An error has occured while handling a request. All required information has been logged. Please contact an application administrator." }`

### User Login

Authenticates a user and returns an access token.

- **URL**

    `/users/login`

- **Method**

    `POST`

- **URL Params**

    `None`

- **Data Params**

    ```json
    {
      "identifier": "string",
      "password": "string"
    }
    ```

- **Success Response**

    **Code:** 200 OK
    
    **Content:**
    ```json
    {
      "accessToken": "jwt_token_string"
    }
    ```
    The `accessToken` will be set in a cookie.

- **Error Response**

    **Code:** 401 UNAUTHORIZED
    
    **Content:** `[ "Error messages" ]`

    OR

    **Code:** 500 INTERNAL SERVER ERROR
    
    **Content:**
    `"An error has occurred while handling the request. All required information has been logged. Please contact an application administrator."`

- **Cookie**

    The response will include a cookie named `accessToken` with an expiration time defined in the system.

### Create Task

Creates a new task with a title, description, due date, status, and priority.

- **URL**

    `/tasks`

- **Method**

    `POST`

- **URL Params**

    `None`

- **Data Params**

    ```json
    {
      "title": "string",
      "description": "string",
      "dueDate": "date_time",
      "status": 0,
      "priority": 0
    }
    ```

- **Success Response**

    **Code:** 201 CREATED
    
    **Content:** `"task_id"`

- **Error Response**

    **Code:** 400 BAD REQUEST
    
    **Content:** `[ "Error messages" ]`

    OR

    **Code:** 500 INTERNAL SERVER ERROR
    
    **Content:** 
    `"An error has occurred while handling the request. All required information has been logged. Please contact an application administrator."`

### Get All Tasks

Retrieves a paginated list of tasks with optional filters for status, priority, and date range, and allows sorting.

- **URL**

    `/tasks`

- **Method**

    `GET`

- **URL Params**

    - `sort=[string]` (Optional)  
      Sorts the tasks by a specific field (e.g., "createdAt", "dueDate", "priority").

    - `status=[array of integers]` (Optional)  
      Filters tasks by status values (0: Pending, 1: InProgress, 2: Completed).

    - `priority=[array of integers]` (Optional)  
      Filters tasks by priority values (0: Low, 1: Medium, 2: High).

    - `start=[ISO Date]` (Optional)  
      Filters tasks starting from a specific date.

    - `end=[ISO Date]` (Optional)  
      Filters tasks up to a specific date.

    - `pageNumber=[integer]` (Optional, default: 1)  
      Specifies the page number for paginated results.

    - `pageSize=[integer]` (Optional, default: 10)  
      Specifies the number of tasks per page.

- **Data Params**

    `None`

- **Success Response**

    **Code:** 200 OK
    
    **Content:**
    ```json
    [
      {
        "id": 1,
        "title": "Task 1",
        "description": "Description for task 1",
        "dueDate": "2024-09-11T08:26:46.326Z",
        "status": 0,
        "priority": 1,
        "userId": 123
      },
      {
        "id": 2,
        "title": "Task 2",
        "description": "Description for task 2",
        "dueDate": "2024-09-12T08:26:46.326Z",
        "status": 1,
        "priority": 2,
        "userId": 123
      }
    ]
    ```

- **Error Response**

    **Code:** 500 INTERNAL SERVER ERROR
    
    **Content:** 
    `"An error has occurred while handling the request. All required information has been logged. Please contact an application administrator."`

### Get Task by ID

Retrieves details of a specific task by its unique ID.

- **URL**

    `/tasks/{id}`

- **Method**

    `GET`

- **URL Params**

    - `id=[GUID]` (Required)  
      The unique identifier of the task.

- **Data Params**

    `None`

- **Success Response**

    **Code:** 200 OK
    
    **Content:**
    ```json
    {
      "id": "b6a1e50d-5a8d-42c9-9227-3f9c7db40da4",
      "title": "Task Title",
      "description": "Task description",
      "dueDate": "2024-09-11T08:26:46.326Z",
      "status": 0,
      "priority": 1,
      "userId": 123
    }
    ```

- **Error Response**

    **Code:** 404 NOT FOUND
    
    **Content:** `None`

    OR

    **Code:** 403 FORBID
    
    **Content:** `None`

    OR

    **Code:** 500 INTERNAL SERVER ERROR
    
    **Content:** 
    `"An error has occurred while handling the request. All required information has been logged. Please contact an application administrator."`

### Update Task

Updates the details of an existing task identified by its unique ID.

- **URL**

    `/tasks/{id}`

- **Method**

    `PUT`

- **URL Params**

    - `id=[GUID]` (Required)  
      The unique identifier of the task to be updated.

- **Data Params**

    ```json
    {
      "title": "string",
      "description": "string",
      "dueDate": "date_time",
      "status": 0,
      "priority": 0
    }
    ```

- **Success Response**

    **Code:** 204 NO CONTENT
    
    Content: `None`

- **Error Response**

    **Code:** 404 NOT FOUND
    
    **Content:** `None`

    OR

    **Code:** 403 FORBID
    
    **Content:** `None`

    OR

    **Code:** 500 INTERNAL SERVER ERROR
    
    **Content:** 
    `"An error has occurred while handling the request. All required information has been logged. Please contact an application administrator."`

### Delete Task

Deletes an existing task identified by its unique ID.

- **URL**

    `/tasks/{id}`

- **Method**

    `DELETE`

- **URL Params**

    - `id=[GUID]` (Required)  
      The unique identifier of the task to be deleted.

- **Data Params**

    `None`

- **Success Response**

    **Code:** 204 NO CONTENT
    
    Content: `None`

- **Error Response**

    **Code:** 404 NOT FOUND
    
    **Content:** `None`

    OR

    **Code:** 403 FORBID
    
    **Content:** `None`

    OR

    **Code:** 500 INTERNAL SERVER ERROR
    
    **Content:** 
    `"An error has occurred while handling the request. All required information has been logged. Please contact an application administrator."`
