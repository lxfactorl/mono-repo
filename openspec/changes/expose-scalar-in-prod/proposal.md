# Expose Scalar API Documentation in Production

## Linked Issue
Relates to #77

## Summary
Scalar API documentation is currently restricted to development. It needs to be exposed in production to facilitate discovery and live documentation.

## Problem Description
Currently, Scalar API documentation is likely restricted to development/staging environments. It should be exposed in production to facilitate discovery and live documentation of services.

## Proposed Solution
Update the startup configuration in backend services to ensure `app.MapScalarApiReference()` (or equivalent registration) is executed regardless of the environment, or specifically enabled for production.

## Rationale
- **Live Documentation**: Provides a source of truth for the API schema that exactly matches the production deployment.
- **Integration Support**: Helps internal and external developers verify API behavior in the production environment.
- **Improved Observability**: Facilitates quick manual smoke tests and discovery without needing local environment setup.
