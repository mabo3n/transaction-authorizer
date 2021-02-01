FROM mcr.microsoft.com/dotnet/sdk:5.0-focal as build
WORKDIR /app
COPY . .
RUN dotnet publish src/Authorizer.csproj -c Release -o ./publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0 as release
WORKDIR /app
COPY --from=build /app/publish/ .
EXPOSE 80
ENTRYPOINT ["dotnet", "Authorizer.dll"]
