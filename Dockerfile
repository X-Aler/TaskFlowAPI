FROM mcr.microsoft.com/dotnet/sdk:10.0 as build

WORKDIR /src

COPY ["AspLearn/AspLearn.csproj", "AspLearn/"]
RUN dotnet restore "AspLearn/AspLearn.csproj"

COPY . .

WORKDIR "/src/AspLearn"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AspLearn.dll"]
