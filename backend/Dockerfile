FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY src/src.sln ./
COPY src/Fundo.Domain/Fundo.Domain.csproj Fundo.Domain/
COPY src/Fundo.Application/Fundo.Application.csproj Fundo.Application/
COPY src/Fundo.Infrastructure/Fundo.Infrastructure.csproj Fundo.Infrastructure/
COPY src/Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj Fundo.Applications.WebApi/

RUN dotnet restore Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj

COPY src/ ./
RUN dotnet publish Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Fundo.Applications.WebApi.dll"]
