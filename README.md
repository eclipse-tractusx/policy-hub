# Policy-Hub

This repository contains the backend code for the Policy-Hub written in C#.

## How to build and run

Install [the .NET 7.0 SDK](https://www.microsoft.com/net/download).

Run the following command from the CLI:

```console
dotnet build src
```

Make sure the necessary config is added to the settings of the service you want to run.
Run the following command from the CLI in the directory of the service you want to run:

```console
dotnet run
```

## Notice for Docker image

This application provides container images for demonstration purposes.

### DockerHub

* [https://hub.docker.com/r/tractusx/policy-hub-service](https://hub.docker.com/r/tractusx/policy-hub-service)
* [https://hub.docker.com/r/tractusx/policy-hub-migrations](https://hub.docker.com/r/tractusx/policy-hub-migrations)

### Base images

mcr.microsoft.com/dotnet/aspnet:7.0-alpine:

* Dockerfile: [mcr.microsoft.com/dotnet/aspnet:7.0-alpine](https://github.com/dotnet/dotnet-docker/blob/main/src/aspnet/7.0/alpine3.17/amd64/Dockerfile)
* GitHub project: [https://github.com/dotnet/dotnet-docker](https://github.com/dotnet/dotnet-docker)
* DockerHub: [https://hub.docker.com/_/microsoft-dotnet-aspnet](https://hub.docker.com/_/microsoft-dotnet-aspnet)

mcr.microsoft.com/dotnet/runtime:7.0-alpine:

* Dockerfile: [mcr.microsoft.com/dotnet/runtime:7.0-alpine](https://github.com/dotnet/dotnet-docker/blob/main/src/runtime/7.0/alpine3.17/amd64/Dockerfile)
* GitHub project: [https://github.com/dotnet/dotnet-docker](https://github.com/dotnet/dotnet-docker)
* DockerHub: [https://hub.docker.com/_/microsoft-dotnet-runtime](https://hub.docker.com/_/microsoft-dotnet-runtime)

## License

Distributed under the Apache 2.0 License.
See [LICENSE](./LICENSE) for more information