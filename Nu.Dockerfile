FROM mcr.microsoft.com/dotnet/sdk:3.1 as build
WORKDIR /app
COPY . .
RUN dotnet publish src/Nu.csproj -c Release -o ./publish

FROM mcr.microsoft.com/dotnet/aspnet:3.1 as release
WORKDIR /app
COPY --from=build /app/publish/ .
EXPOSE 80
ENTRYPOINT ["dotnet", "Nu.dll"]
