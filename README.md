# AWS API Gateway + Lambda Blue/Green Deployment Example

A reference implementation of a blue/green deployment strategy using **AWS API Gateway stages** and **Lambda function aliases**, built as a companion to a conference talk for [AWS Midwest Community Day 2026](https://www.midwestcommunityday.com).

## Overview

Blue/green deployment is a release strategy that reduces risk by running two identical production environments — **blue** (current) and **green** (new) — and shifting traffic between them only after the new version has been validated.

This project demonstrates how to implement that pattern on AWS using API Gateway and Lambda.

### How It Works

**Lambda versioning and aliases** are the core mechanism. Each Lambda function has three aliases:

| Alias      | Role                                                               |
|------------|--------------------------------------------------------------------|
| `green`    | The latest deployed version — used for validation before promotion |
| `blue`     | The current production version — live traffic is served from here  |
| `previous` | The last production version — used for quick rollback              |

**API Gateway stages** map directly to these aliases. The `green` stage invokes the `green` alias; the `blue` stage invokes the `blue` alias. Promotion is a control-plane operation: updating which Lambda version an alias points to, with no redeployment required.

### Project Structure

| Path                                         | Description                                                                       |
|----------------------------------------------|-----------------------------------------------------------------------------------|
| `src/Ecommerce/`                             | .NET Lambda functions implementing a simple e-commerce orders API                 |
| `src/Ecommerce/Ecommerce.Library.Api.Tests/` | Integration test suite that validates the API end-to-end                          |
| `iac/`                                       | Terraform infrastructure — API Gateway, Lambda functions, DynamoDB, ACM, Route53  |
| `schemas/`                                   | JSON schemas for API Gateway request/response validation                          |
| `postman/`                                   | Postman collection and environments for manual testing                            |
| `.github/workflows/`                         | GitHub Actions CI/CD pipelines                                                    |

### API

The API is a simple CRUD orders service with four Lambda functions:

- `POST /v1/order` — Create order
- `GET /v1/order/{customerId}/{orderId}` — Get order
- `PUT /v1/order` — Update order
- `DELETE /v1/order` — Delete order

All endpoints are protected by a custom Lambda authorizer.

### CI/CD Pipelines

Organizations and teams have different standards for CI/CD pipelines. This project has two GitHub Actions workflows that represent the key concepts of this blue/green deployment strategy. Teams will need to adapt these to fit their own standards and requirements.

#### Key Concepts

- IAC tool updates green, not blue.
- Tests run against green before promotion.
- Promote green to blue only after tests pass.
- Promotion is a control-plane operation, not a data-plane operation.

#### [build-deploy.yml](.github/workflows/build-deploy.yml) — Build & Deploy

Builds the .NET Lambda packages and applies Terraform. This deploys new Lambda versions (pointed to by the `green` alias) and updates API Gateway configuration, but does **not** affect the `blue` stage or its Lambda aliases. Blue traffic is untouched until explicit promotion.

> **Note:** This workflow builds and deploys the authorizer Lambda function. Generally, you would want to do this in a separate workflow — API Gateway cannot use stage variables for authorizer Lambda functions, so deploying it updates both stages immediately with no opportunity to test before promotion.

> **Note:** This workflow does not have a manual approval step before applying Terraform. In real-world applications, Terraform changes should be reviewed before being applied.

#### [deploy-and-promote.yml](.github/workflows/deploy-and-promote.yml) — Deploy & Promote

Orchestrates the full promotion pipeline:

1. Deploys the API Gateway `green` stage
2. Runs the integration test suite against `green` — if any test fails, the pipeline stops here
3. Deploys the API Gateway `blue` stage
4. Promotes each Lambda function's aliases: `blue` → version `green` points to, `previous` → version `blue` pointed to
5. Runs the integration test suite again against `blue` to confirm the promotion succeeded

> **Note:** Running tests against `blue` after promotion is not strictly required. If they fail at this point, it would indicate a critical failure of the deployment strategy itself. It is included as a safety net to surface any issues with the blue stage post-promotion.

## Notes

- The Lambda functions in this project are written in .NET, but the concepts are applicable to any language supported by Lambda.

- [src/Ecommerce/Ecommerce.Library.Api.Tests](src/Ecommerce/Ecommerce.Library.Api.Tests) is a regular .NET unit test project. This represents _any_ test suite that excercises and validates the functionality of the API.
    - I kept it the same framework as the API project for simplicity.
    - The test project deliberately has its own copies of the request/response models and JSON schemas, to ensure the tests are independent of the API implementation.
    - Instead of / in addition to automated tests, you could do manual testing of the green stage, with something like a [Manual Workflow Approval](https://github.com/marketplace/actions/manual-workflow-approval) as a gate before promotion.

- [The custom authorizer Lambda function](/src/Ecommerce/Ecommerce.Authorizer) is a naive implementation of an API Gateway custom authorizer.
    - It is not intended for production use.
    - It is included because this API Gateway is exposed to the public internet, and the authorizer offers some protection against unauthorized access.
