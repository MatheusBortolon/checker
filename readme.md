# Checker

A sample console application using dotnet 8 and healthchecks to exercise the coding with new version of framework.

## Getting Started

This project is able to read configs to configure the healthchecks and produces a single file to facilitate the final usage.

It uses a sightly modified version of WriteResponse for healthchecks available on [Microsoft Docs](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-8.0)

### Prerequisites

Requirements for the software
- [Dotnet 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### Usage

This software is a boilerplate that allows you to customize and make your own checks.

To make the single file is just publish the checker project with default dotnet tool

```
cd checker
dotnet publish
```

## Contributing

Feel free to fork and modify the project to fit your needs.

## License

This project is licensed under the [MIT](https://mit-license.org/)
