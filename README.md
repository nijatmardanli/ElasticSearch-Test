# User Management API

This API provides endpoints for managing user data using Elasticsearch as the underlying data store. It allows operations such as retrieving user information, adding, updating, and deleting users. Logging is performed using Serilog and is integrated with Elasticsearch for centralized logging.

## Endpoints

### Get User by ID

Retrieve user information by their ID.

- **HTTP Method:** GET
- **Route:** `/api/users/{id:int}`
- **Parameters:**
  - `id` (integer): The ID of the user to retrieve.
- **Returns:** User information corresponding to the provided ID.

### Get User List

Retrieve a list of users with pagination support.

- **HTTP Method:** GET
- **Route:** `/api/users`
- **Query Parameters:**
  - `paginationRequest` (PaginationRequest): Object containing pagination parameters.
- **Returns:** List of users based on pagination settings.

### Search User

Search for users based on specified criteria such as first name, last name, and age.

- **HTTP Method:** GET
- **Route:** `/api/users/Search`
- **Query Parameters:**
  - `userSearchRequest` (UserSearchRequestDto): Object containing search criteria.
- **Returns:** List of users matching the search criteria.
- **Elasticsearch Query Translation to SQL Server:**
  - `FirstName = request.FirstName`
  - `LastName LIKE 'request.LastName%'`
  - `Age >= request.Age`

### Add User

Add a new user to the system.

- **HTTP Method:** POST
- **Route:** `/api/users`
- **Request Body:** UserAddDto: Data for adding a new user.
- **Returns:** Confirmation of the user being added.

### Update User

Update existing user information.

- **HTTP Method:** PUT
- **Route:** `/api/users`
- **Request Body:** UserUpdateDto: Data for updating an existing user.
- **Returns:** Confirmation of the user being updated.

### Delete User

Delete a user from the system.

- **HTTP Method:** DELETE
- **Route:** `/api/users`
- **Request Header:** `id` (integer): The ID of the user to delete.
- **Returns:** Confirmation of the user being deleted.

## Usage

To use these endpoints, send HTTP requests to the specified routes with the appropriate parameters and request bodies where required.