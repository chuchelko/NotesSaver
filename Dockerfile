FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
LABEL name="Notes Saver"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MVCNotesSaver.csproj", "./"]
RUN dotnet restore "MVCNotesSaver.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "MVCNotesSaver.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MVCNotesSaver.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MVCNotesSaver.dll"]
