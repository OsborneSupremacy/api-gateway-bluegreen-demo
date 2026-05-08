# AWS API Gateway + Lambda Blue/Green Deployment Example

A reference implementation of a blue/green deployment strategy using **AWS API Gateway stages** and **Lambda function aliases**, built as a companion to a conference talk for [AWS Midwest Community Day 2026](https://www.midwestcommunityday.com).

## Overview

Blue/green deployment is a release strategy that reduces risk by running two identical production environments — **blue** (current) and **green** (new) — and shifting traffic between them only after the new version has been validated.

This project demonstrates how to implement that pattern on AWS using API Gateway and Lambda.

## How It Works

**Lambda versioning and aliases** are the core mechanism. Each Lambda function has three aliases:

| Alias      | Role                                                               |
|------------|--------------------------------------------------------------------|
| `green`    | The latest deployed version — used for validation before promotion |
| `blue`     | The current production version — live traffic is served from here  |
| `previous` | The last production version — used for quick rollback              |

**API Gateway stages** map directly to these aliases. The `green` stage invokes the `green` alias; the `blue` stage invokes the `blue` alias. Promotion is a control-plane operation: updating which Lambda version an alias points to, with no redeployment required.

## Project Structure

| Path                                 | Description                                                                      |
|--------------------------------------|----------------------------------------------------------------------------------|
| `src/Ecommerce/`                     | .NET Lambda functions implementing a simple e-commerce orders API                |
| `test/Ecommerce.Api.Tests/`          | Integration test suite that validates the API end-to-end                         |
| `iac/`                               | Terraform infrastructure — API Gateway, Lambda functions, DynamoDB, ACM, Route53 |
| `schemas/`                           | JSON schemas for API Gateway request/response validation                         |
| `postman/`                           | Postman collection and environments for manual testing                           |
| `.github/workflows/`                 | GitHub Actions CI/CD pipelines                                                   |

## API

The API is a simple CRUD orders service with four Lambda functions:

- `POST /v1/order` — Create order
- `GET /v1/order/{customerId}/{orderId}` — Get order
- `PUT /v1/order` — Update order
- `DELETE /v1/order` — Delete order

All endpoints are protected by a custom Lambda authorizer.

## CI/CD Pipelines

This project uses one GitHub Actions workflow to demonstrate the full promotion pipeline. In a real-world application, you would likely want to break this into multiple workflows for better separation of concerns.

### Key Concepts

- IAC tool updates green, not blue.
- Tests run against green before promotion.
- Promote green to blue only after tests pass.
- Promotion is a control-plane operation, not a data-plane operation.

### Pipeline Flow

1. Builds the infrastructure using Terraform. This creates new Lambda versions (referenced by the `green` alias) and updates the API Gateway configuration
2. Deploys the API Gateway `green` stage
3. Runs the integration test suite against `green` — if any test fails, the pipeline stops here
4. Deploys the API Gateway `blue` stage
5. Promotes each Lambda function's aliases: `blue` → version `green` points to, `previous` → version `blue` pointed to
6. Runs the integration test suite again against `blue` to confirm the promotion succeeded

## Notes

- The Lambda functions in this project are written in .NET, but the concepts are applicable to any language supported by Lambda.

- [src/Ecommerce/Ecommerce.Library.Api.Tests](src/Ecommerce/Ecommerce.Library.Api.Tests) is a regular .NET unit test project. This represents _any_ test suite that excercises and validates the functionality of the API.
    - I kept it the same framework as the API project for simplicity.
    - The test project deliberately is in a different solution than the API project and has its own copies of the request/response models and JSON schemas to ensure the tests are independent of the API implementation.

- [The custom authorizer Lambda function](/src/Ecommerce/Ecommerce.Authorizer) is a naive implementation of an API Gateway custom authorizer.
    - It is not intended for production use.
    - It is included because this API Gateway is exposed to the public internet, and the authorizer offers some protection against unauthorized access.
