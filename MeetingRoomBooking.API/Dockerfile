# Dockerfile for MeetingRoomBooking.API
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only the API project file and restore first for layer caching
COPY ["MeetingRoomBooking.API/MeetingRoom.API.csproj", "MeetingRoomBooking.API/"]
RUN dotnet restore "MeetingRoomBooking.API/MeetingRoom.API.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/MeetingRoomBooking.API"
RUN dotnet build "MeetingRoom.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/MeetingRoomBooking.API"
RUN dotnet publish "MeetingRoom.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeetingRoom.API.dll"]
