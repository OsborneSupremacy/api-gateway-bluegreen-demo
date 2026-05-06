# AWS API Gateway + Lambda Blue/Green Deployment Example

## Building and Deploying

### Build the .NET Lambda functions

```bash
cd scripts
./build-lambdas.sh
```

## Notes

* The Lambda functions in this project are built in .NET. That is my framework of choice, but the concepts in this project are applicable to any language supported by Lambda.

* [build-deploy.yml](.github/workflows/build-deploy.yml) builds and deploys the authorizer Lambda function.
    * Generally, you would want to do this in a separate workflow. API gateway cannot use stage variables for authorizer Lambda functions. Deploying it updates it on both blue and green stages immediately, with no opportunity to test before promotion.

* [src/Ecommerce/Ecommerce.Library.Api.Tests](src/Ecommerce/Ecommerce.Library.Api.Tests) is a regular .NET unit test project. This represents _any_ test suite that confirm the functionality of the API.
    * I kept it the same framework as the API project for simplicity.
    * Instead of or in addition to automated tests, you could do manual testing of the green stage, with something like a [Manual Workflow Approval](https://github.com/marketplace/actions/manual-workflow-approval) as a gate before promotion.

* [The custom authorizer Lambda function](/src/Ecommerce/Ecommerce.Authorizer) is a naive implementation of an API Gateway custom authorizer.
    * It is not intended for production use.
    * It is included because this API Gateway is exposed to the public internet, and the authorize it offers some protection against unauthorized access.

