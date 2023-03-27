#Build application in temporal image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
#Copy source files
COPY ./sample/WkeSampleApp .
#Restore
RUN dotnet restore WkeSampleApp.csproj
#Build and publish
RUN dotnet publish WkeSampleApp.csproj -c Release -o out --no-restore

#Build app image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build app/out/. .
ENTRYPOINT ["dotnet", "WkeSampleApp.dll"]