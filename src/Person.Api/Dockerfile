#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Person.Api/Person.Api.csproj", "Person.Api/"]
COPY ["src/Person.BusinessLogic/Person.BusinessLogic.csproj", "Person.BusinessLogic/"]
RUN dotnet restore "src/Person.Api/Person.Api.csproj"
COPY . .
WORKDIR "/src/src/Person.Api"
RUN dotnet build "Person.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Person.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Person.Api.dll"]