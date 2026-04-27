# AWS API Gateway + Lambda Blue/Green Deployment Example

## Building and Deploying

### Build the .NET Lambda function

```bash
cd src/Ecommerce/CreateOrder
dotnet publish -o bin/publish -c Release --framework "net10.0" /p:GenerateRuntimeConfigurationFiles=true --runtime linux-arm64 --self-contained false
cd bin/publish
zip -r ../CreateOrder.zip .
cd -
```

