# AWS API Gateway + Lambda Blue/Green Deployment Example

## Building and Deploying

### Build the .NET Lambda Functions Locally

```bash
./scripts/build-lambdas.sh
```

## Notes

* The Lambda functions in this project are written in .NET, but the concepts are applicable to any language supported by Lambda.

* [src/Ecommerce/Ecommerce.Library.Api.Tests](src/Ecommerce/Ecommerce.Library.Api.Tests) is a regular .NET unit test project. This represents _any_ test suite that excercises and validates the functionality of the API.
    * I kept it the same framework as the API project for simplicity.
    * Instead of / in addition to automated tests, you could do manual testing of the green stage, with something like a [Manual Workflow Approval](https://github.com/marketplace/actions/manual-workflow-approval) as a gate before promotion.

* [The custom authorizer Lambda function](/src/Ecommerce/Ecommerce.Authorizer) is a naive implementation of an API Gateway custom authorizer.
    * It is not intended for production use.
    * It is included because this API Gateway is exposed to the public internet, and the authorize it offers some protection against unauthorized access.

### Notes on the CI/CD Pipelines

* Organizations and teams have different standards for CI/CD pipelines. This project has two GitHub Actions workflows that represent the key concepts of this blue/green deployment strategy. Teams will need to adapt these concepts to fit their standards and requirements.

#### Key Concepts

* IAC tool updates green, not blue.
* Tests are against green before promotion.
* Promote green to blue after successful tests.
* Promotion is a control plane operation, not a data plane operation.

#### CI/CD Pipeline 1: [build-deploy.yml](.github/workflows/build-deploy.yml)

This pipeline builds the Lambda functions and deploys the infrastructure using Terraform. Doing this will not affect the blue stage, because:

* The blue stage will not be deployed until promotion.
* The blue stage points to blue Lambda aliases, which will not be updated until promotion.

##### Important Notes

* This workflow builds and deploys the authorizer Lambda function.
    * Generally, you would want to do this in a separate workflow. API gateway cannot use stage variables for authorizer Lambda functions. Deploying it updates it on both blue and green stages immediately, with no opportunity to test before promotion.

* This workflow does not have tests or a manual approval step before applying the Terraform. In real-world applications, Terraform changes should be reviewed before being applied.

#### CI/CD Pipeline 2: [deploy-and-promote.yml](.github/workflows/deploy-and-promote.yml)

This pipeline:

1. Deploys the green stage and runs tests against it.
2. If the tests are successful, promotes the green stage to blue.
3. Promotes the green Lambda aliases to blue, which updates the blue stage to point to the new Lambda versions.
4. Runs the test suite again on the blue stage.

##### Is it necessary to run tests on the blue stage after promotion?

It is not strictly necessary to run the tests at this point. If the tests fail, that would indicate a critical failure of this deployment strategy. But it doesn't hurt, and it provides an alert if something is wrong with the blue stage after promotion.

