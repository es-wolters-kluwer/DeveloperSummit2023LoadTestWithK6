Remove-Item ./app -Recurse -Force
dotnet restore ./sample/WkeSampleApp/WkeSampleApp.csproj
dotnet publish ./sample/WkeSampleApp/WkeSampleApp.csproj -c Release -o ./app/publish
Copy-Item ./DockerTools/Sample.DockerFile ./app 
Docker build -f ./app/wkeSampleLoad.DockerFile -t wke/sampleloadapp .