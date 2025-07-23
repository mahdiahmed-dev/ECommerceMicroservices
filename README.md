# ECommerceMicroservices

A sample .NET 8 microservices-based e-commerce system demonstrating distributed services with different data stores and messaging.

# Architecture

- ASP.NET Core Web APIs using Microservices
- **RabbitMQ** for message communication
- **Databases:**
  - `ProductService` → PostgreSQL
  - `OrderService` → Microsoft SQL Server
  - `NotificationService` → MongoDB