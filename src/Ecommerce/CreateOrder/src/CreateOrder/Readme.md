# CreateOrder Lambda

This project implements an AWS API Gateway integration Lambda that creates ecommerce orders and persists them to DynamoDB.

## API request body

The function expects `APIGatewayProxyRequest.Body` JSON with this shape:

```json
{
  "customerId": "customer-123",
  "currency": "USD",
  "shippingAddress": "123 Main St, Seattle, WA",
  "items": [
    {
      "sku": "SKU-001",
      "name": "Wireless Mouse",
      "quantity": 2,
      "unitPrice": 25.50
    }
  ]
}
```

Returns:
- `201` with the created order payload
- `400` for invalid request payloads
- `500` when persistence fails

## Configuration

Set environment variable `ORDERS_TABLE_NAME` to the DynamoDB table used for inserts.

## Local workflows

Run tests from solution root (`src/Ecommerce`):

```bash
dotnet test CreateOrder/test/CreateOrder.Tests/CreateOrder.Tests.csproj
```

Deploy from Lambda project directory:

```bash
dotnet tool update -g Amazon.Lambda.Tools
cd CreateOrder/src/CreateOrder
dotnet lambda deploy-function
```
