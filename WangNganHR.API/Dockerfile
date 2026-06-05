FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["WangNganHR.API/WangNganHR.API.csproj", "WangNganHR.API/"]
COPY ["WangNganHR.Shared/WangNganHR.Shared.csproj", "WangNganHR.Shared/"]
RUN dotnet restore "WangNganHR.API/WangNganHR.API.csproj"
COPY . .
WORKDIR "/src/WangNganHR.API"
RUN dotnet publish "WangNganHR.API.csproj" -c Release -o /app/out

FROM base AS final
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "WangNganHR.API.dll"]