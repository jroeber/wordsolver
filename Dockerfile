FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

COPY *.sln ./
COPY WordSolver.Core/*.csproj ./WordSolver.Core/
COPY WordSolver.CLI/*.csproj ./WordSolver.CLI/
COPY WordSolver.WebAPI/*.csproj ./WordSolver.WebAPI/
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2.6-alpine3.9
WORKDIR /app
# TODO un-hardcode wordlist.txt
COPY --from=build-env /app/wordlist.txt . 
COPY --from=build-env /app/WordSolver.WebAPI/out .
ENTRYPOINT ["dotnet", "WordSolver.WebAPI.dll"]