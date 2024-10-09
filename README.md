<h1 align="center">🚀 Mercora</h1>

[MicrosoftSQLServer]: https://img.shields.io/badge/Microsoft%20SQL%20Server-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white
[.Net]: https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white
[C#]: https://custom-icon-badges.demolab.com/badge/C%23-%23239120.svg?logo=cshrp&style=for-the-badge&logoColor=white
[JSON]: https://img.shields.io/badge/JSON-000?logo=json&style=for-the-badge&logoColor=white
[Docker]: https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white
[Redis]: https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white
[Stripe]: https://img.shields.io/badge/Stripe-5851DD?style=for-the-badge&logo=stripe&logoColor=white

![MicrosoftSQLServer]
![.Net]
![C#]
![JSON]
![Docker]
![Redis]
![Stripe]

<p align="center">
  <b>Mercora is a modern e-commerce platform built with ASP.NET Core API designed with a focus on Clean Architecture.</b>
</p>

## 🐳 Docker Compose

This project utilizes Docker Compose to define and run a multi-container application consisting of:

- Redis Caching: Provides caching capabilities to enhance performance.
- SQL Server Database: Serves as the data storage for the application.
- API: Hosts the application's backend logic and endpoints.

## 💎 Prerequisites

<ul>
  <li><a href="https://dotnet.microsoft.com/pt-br/">.NET 8</a></li>
  <li><a href="https://github.com">Git</a></li>
  <li><a href="https://docker.com">Docker</a></li>
</ul>

## 🎇 Setup and Configuration

**1- Clone the Repository**

```bash
git clone "https://github.com/ahmedeldamity/Mercora"
cd Mercora
```

**2- Build and Start Containers**

Use Docker Compose to build and start all containers defined in the `docker-compose.yml` file.

```bash
docker-compose up --build
```

This command will build the necessary images (if not already built) and start the containers as defined.

**3- Access the Services**

- API: Access the API at `http://localhost:8080`.
- SQL Server Database: Connect to the SQL Server using the connection string `Server=store_database;Database=StoreDatabase;User Id=sa;Password=PAssWord**;`.
- Redis Caching: Redis can be accessed at `redis://localhost:6379`.

## Persisting Data

Data for SQL Server and Redis is persisted using Docker volumes. Ensure that volumes are properly configured in docker-compose.yml to prevent data loss on container restarts.

## Database ERD

The Entity-Relationship Diagram (ERD) for the SQL Server database is included below. This diagram illustrates the structure of the database, including tables, relationships, and key constraints.

![image](https://github.com/user-attachments/assets/d7723c57-a4f1-47df-b98d-8afb7c467e11)

## ✔️ Health Check

**The application includes a unified health check endpoint to monitor the status of various system components:**

- SQL Server Database: Ensures the connection to the store database is healthy.
- Redis Caching: Verifies the availability and functionality of the Redis caching system.
- Hangfire Job Processing: Checks the status of Hangfire to ensure background jobs are being processed correctly.
- Mail Service: Monitors the mail service to confirm that emails can be sent.

## 🔄 API Versioning

**API versioning is supported to ensure backward compatibility as the API evolves. Clients can specify the version via the URL path, query string, or request headers.**

## ⏳ Rate Limiting

**The API uses multiple rate limiting strategies to manage traffic:**

- Fixed Window: Limits requests within a defined time window.
- Sliding Window: Allows a smoother request flow by checking limits within a moving time window.
- Concurrency Limit: Restricts the number of concurrent requests being processed at the same time.

## API Endpoints

**Health Check :**

| Method              | Endpoint                                | Description                                         |
|---------------------|-----------------------------------------|-----------------------------------------------------|
| <kbd> GET </kbd>    | <kbd> /_health </kbd>                   | Returns the health status of the system.            |

**Products:**

| Method              | Endpoint                                | Description                                         |
|---------------------|-----------------------------------------|-----------------------------------------------------|
| <kbd> GET </kbd>    | <kbd> /api/product </kbd>               | Retrieve a list of products.                        |
| <kbd> GET </kbd>    | <kbd> /api/product/{id} </kbd>          | Retrieve details of a specific product.             |
| <kbd> POST </kbd>   | <kbd> /api/product </kbd>               | Add a new product.                                  | 
| <kbd> PUT </kbd>    | <kbd> /api/product/{id} </kbd>          | Update product.                                     |
| <kbd> DELETE </kbd> | <kbd> /api/product/{id} </kbd>          | Delete a product.                                   |

**Brands :**

| Method              | Endpoint                                | Description                                          |
|---------------------|-----------------------------------------|------------------------------------------------------|
| <kbd> GET </kbd>    | <kbd> /api/brand </kbd>                 | Retrieve a list of brands.                           |
| <kbd> GET </kbd>    | <kbd> /api/brand/{id} </kbd>            | Retrieve details of a specific brand.                |
| <kbd> GET </kbd>    | <kbd> /api/brand/search </kbd>          | Retrieve details of a specific brand.                |
| <kbd> POST </kbd>   | <kbd> /api/brand </kbd>                 | Add a new brand                                      |
| <kbd> PUT </kbd>    | <kbd> /api/brand/{id} </kbd>            | Update brand.                                        |
| <kbd> DELETE </kbd> | <kbd> /api/brand/{id} </kbd>            | Delete a brand.                                      |
 
**Categories :**

| Method              | Endpoint                                | Description                                          |
|---------------------|-----------------------------------------|------------------------------------------------------|
| <kbd> GET </kbd>    | <kbd> /api/category </kbd>              | Retrieve a list of categories.                       |
| <kbd> GET </kbd>    | <kbd> /api/category/{id} </kbd>         | Retrieve details of a specific category.             |
| <kbd> GET </kbd>    | <kbd> /api/category/search </kbd>       | Retrieve details of a specific category.             |
| <kbd> POST </kbd>   | <kbd> /api/category </kbd>              | Add a new category                                   |
| <kbd> PUT </kbd>    | <kbd> /api/category/{id} </kbd>         | Update category.                                     |
| <kbd> DELETE </kbd> | <kbd> /api/category/{id} </kbd>         | Delete a category.                                   |
 
**Delivery Methods :**

| Method              | Endpoint                                | Description                                          |
|---------------------|-----------------------------------------|------------------------------------------------------|
| <kbd> GET </kbd>    | <kbd> /api/DeliveryMethod </kbd>        | Retrieve a list of Delivery Methods.                 |
| <kbd> GET </kbd>    | <kbd> /api/DeliveryMethod/{id} </kbd>   | Retrieve details of a specific Delivery Method.      |
| <kbd> POST </kbd>   | <kbd> /api/DeliveryMethod </kbd>        | Add a new Delivery Method                            | 
| <kbd> PUT </kbd>    | <kbd> /api/DeliveryMethod/{id} </kbd>   | Update Delivery Method.                              |
| <kbd> DELETE </kbd> | <kbd> /api/DeliveryMethod/{id} </kbd>   | Delete a Delivery Method.                            |

**Account :**

| Method              | Endpoint                                   | Description                                         |
|---------------------|--------------------------------------------|-----------------------------------------------------|
| <kbd> Post </kbd>   | <kbd> /api/v1.0/Account/register </kbd>    | Register a new user and receive a JWT token V-1.0   |
| <kbd> Post </kbd>   | <kbd> /api/v2.0/Account/register </kbd>    | Register a new user and receive a JWT token V-2.0   |
| <kbd> Post </kbd>   | <kbd> /api/v2.1/Account/register </kbd>    | Register a new user and receive a JWT token V-2.1   |
| <kbd> Post </kbd>   | <kbd> /api/v1.0/Account/login </kbd>       | Authenticate a user and receive a JWT token V-1.0   |
| <kbd> Post </kbd>   | <kbd> /api/v2.0/Account/login </kbd>       | Authenticate a user and receive a JWT token V-2.0   |
| <kbd> POST </kbd>   | <kbd> /api/v1/Account/google-login </kbd>  | Authenticate a user and receive a JWT token.        |
| <kbd> GET </kbd>    | <kbd> /api/v1/Account </kbd>               | Get Current User.                                   |
| <kbd> GET </kbd>    | <kbd> /api/v1/Account/refresh-token </kbd> | Get New Refresh Token.                              |
| <kbd> Post </kbd>   | <kbd> /api/v1/Account/revoke-token </kbd>  | Revoke Refresh Token.                               |

**Authentications :**

| Method              | Endpoint                                                  | Description                          |
|---------------------|-----------------------------------------------------------|--------------------------------------|
| <kbd> Post </kbd>   | <kbd> /api/v1/Auth/send-email-verification-code </kbd>    | Send Email Verification Code V-1.0   |
| <kbd> Post </kbd>   | <kbd> /api/v2/Auth/send-email-verification-code </kbd>    | Send Email Verification Code V-2.0   |
| <kbd> Post </kbd>   | <kbd> /api/v1/Auth/verify-register-code </kbd>            | Verify Register Code.                |
| <kbd> Post </kbd>   | <kbd> /api/v1/Auth/send-password-verification-code </kbd> | Send Password Reset Email V-1.0      |
| <kbd> Post </kbd>   | <kbd> /api/v2/Auth/send-password-verification-code </kbd> | Send Password Reset Email V-2.0      |
| <kbd> Post </kbd>   | <kbd> /api/v1/Auth/Verify-Reset-Code </kbd>               | Verify Reset Code.                   |
| <kbd> Post </kbd>   | <kbd> /api/v1/Auth/change-password </kbd>                 | Change Password.                     |

**Basket :**

| Method              | Endpoint                                                  | Description                          |
|---------------------|-----------------------------------------------------------|--------------------------------------|
| <kbd> Post </kbd>   | <kbd> /api/Basket </kbd>                                  | Create or Update Basket.             |
| <kbd> GET </kbd>    | <kbd> /api/Basket/{id} </kbd>                             | Retrieve the current user's basket.  |
| <kbd> DELETE </kbd> | <kbd> /api/Basket/{id} </kbd>                             | Delete Basket.                       |

**Payment :**

| Method              | Endpoint                                                  | Description                          |
|---------------------|-----------------------------------------------------------|--------------------------------------|
| <kbd> Post </kbd>   | <kbd> /api/Payment/{{basketId}} </kbd>                    | Create or Update Payment.            |

**Order :**

| Method              | Endpoint                                                  | Description                          |
|---------------------|-----------------------------------------------------------|--------------------------------------|
| <kbd> Post </kbd>   | <kbd> /api/Order </kbd>                                   | Create Order.                        |
| <kbd> GET </kbd>    | <kbd> /api/Order </kbd>                                   | Get User Orders.                     |
| <kbd> GET </kbd>    | <kbd> /api/Order/{{id}} </kbd>                            | Get Order By Id                      |


## ⚔ Stopping and Removing Containers

To stop and remove all running containers, use:

```bash
docker-compose down
```
