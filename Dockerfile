FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and restore
COPY ["src/AhmedOS.Domain/AhmedOS.Domain.csproj", "src/AhmedOS.Domain/"]
COPY ["src/AhmedOS.Application/AhmedOS.Application.csproj", "src/AhmedOS.Application/"]
COPY ["src/AhmedOS.Infrastructure/AhmedOS.Infrastructure.csproj", "src/AhmedOS.Infrastructure/"]
COPY ["src/AhmedOS.Web/AhmedOS.Web.csproj", "src/AhmedOS.Web/"]
RUN dotnet restore "src/AhmedOS.Web/AhmedOS.Web.csproj"

# Copy all source code and build
COPY src/ ./src/
WORKDIR "/src/src/AhmedOS.Web"
RUN dotnet publish "AhmedOS.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "AhmedOS.Web.dll"]
