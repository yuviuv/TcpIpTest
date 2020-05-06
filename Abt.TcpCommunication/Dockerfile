FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Abt.TcpCommunication.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Abt.TcpCommunication.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Abt.TcpCommunication.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "Abt.TcpCommunication.dll"]

#EXPOSE 6001