﻿FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TicTacToeBank/TicTacToeBank.csproj", "TicTacToeBank/"]
COPY ["TicTacToeBank.Core/TicTacToeBank.Core.csproj", "TicTacToeBank.Core/"]
COPY ["TicTacToeBank.DatabaseAccess/TicTacToeBank.DatabaseAccess.csproj", "TicTacToeBank.DatabaseAccess/"]
COPY ["TicTacToeBank.Application/TicTacToeBank.Application.csproj", "TicTacToeBank.Application/"]
RUN dotnet restore "TicTacToeBank/TicTacToeBank.csproj"
COPY . .
WORKDIR "/src/TicTacToeBank"
RUN dotnet build "./TicTacToeBank.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TicTacToeBank.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicTacToeBank.dll"]