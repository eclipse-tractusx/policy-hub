# Policy-Hub

This repository contains the backend code for the Policy-Hub written in C#.

For **information about the policy hub**, please refer to the documentation, especially the context and scope section in the [architecture documentation](./docs/architecture).

For **installation** details, please refer to the [README.md](./charts/policy-hub/README.md) of the provided helm chart.

To see the latest open API specs you can have a look at the [API Hub](https://eclipse-tractusx.github.io/api-hub/policy-hub/).

## How to build and run

Install the [.NET 9.0 SDK](https://www.microsoft.com/net/download).

Run the following command from the CLI:

```console
dotnet build src
```

Make sure the necessary config is added to the settings of the service you want to run.
Run the following command from the CLI in the directory of the service you want to run:

```console
dotnet run
```

## Known Issues and Limitations

See [Known Knowns](/docs/admin/known-issues-and-limitations.md).

## Notice for Docker image

This application provides container images for demonstration purposes.

See Docker notice files for more information:

* [policy-hub-service](./docker/notice-policy-hub-service.md)
* [policy-hub-migrations](./docker/notice-policy-hub-migrations.md)

## License

Distributed under the Apache 2.0 License.
See [LICENSE](./LICENSE) for more information
