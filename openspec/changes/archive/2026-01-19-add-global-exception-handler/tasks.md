# Tasks

- [x] Research `IExceptionHandler` interface in .NET 8+
- [x] Define error response model with:
  - Standard properties: `statusCode`, `message` (always present)
  - Optional `exception` property (only included in Development environment with serialized exception details)
- [x] Implement `GlobalExceptionHandler` that always catches exceptions and returns structured responses
- [x] Add `IsDevelopment()` check to conditionally include exception details in response
- [x] Register the exception handler in the dependency injection container
- [x] Add support for mapping specific exception types to custom status codes and messages
- [x] Add unit/integration tests for:
  - Exception handling in Development (includes exception details)
  - Exception handling in Production (excludes exception details)
  - Custom exception mapping logic
- [x] Update `openspec/project.md` to document this as a mandatory standard for all services
