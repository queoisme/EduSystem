FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY PRN232.EduSystem.Repositories/PRN232.EduSystem.Repositories.csproj PRN232.EduSystem.Repositories/
COPY PRN232.EduSystem.Services/PRN232.EduSystem.Services.csproj PRN232.EduSystem.Services/
COPY PRN232.EduSystem.API/PRN232.EduSystem.API.csproj PRN232.EduSystem.API/
RUN dotnet restore PRN232.EduSystem.API/PRN232.EduSystem.API.csproj

COPY . .
RUN dotnet publish PRN232.EduSystem.API/PRN232.EduSystem.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "PRN232.EduSystem.API.dll"]
