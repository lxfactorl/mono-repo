# Implement Mandatory Global Exception Handling Middleware

## Linked Issue
Relates to #76

## Summary
Implement a global exception handling middleware to prevent exposing sensitive exception details in production while maintaining detailed error reporting in development environments.

## Problem Description
Currently, the REST calls expose all exception details directly to the client. This is a security risk in production environments and lacks a standardized way to handle and report errors.

## Proposed Solution
Implement a global exception handling middleware that:
1. **Catches all unhandled exceptions** and reports a generic `500 Internal Server Error` in production environments.
2. **Exposes detailed exception information** (message, stack trace) in non-production (debug/development) environments.
3. **Provides customization options** to change the HTTP status code and error message based on the exception type.
4. **Leverages existing Microsoft packages** for standard implementation (e.g., `Microsoft.AspNetCore.Diagnostics.IExceptionHandler` in .NET 8+).

## MANDATORY STANDARD
This solution is now documented in `openspec/project.md` as a **mandatory part for any new service**. Every service MUST implement this pattern to ensure security and consistency across the monorepo.

## Rationale
- **Security**: Prevents leaking internal system details, database schemas, or logic through exception messages in production.
- **Consistency**: Ensures all API errors follow a standard format and include appropriate HTTP status codes.
- **Developer Experience**: Maintains detailed error reporting for development and debugging.

## Technical Details
Recommended to use the standard .NET 8+ `IExceptionHandler` interface for a modern and idiomatic implementation.
