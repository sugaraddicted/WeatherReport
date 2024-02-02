# syntax=docker/dockerfile:1

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
EXPOSE 8090
COPY . /source

WORKDIR /source/WeatherReport
ARG TARGETARCH

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

COPY --from=build /app .

USER $APP_UID
ENV ASPNETCORE_URLS http://*:8090/

ENTRYPOINT ["dotnet", "WeatherReport.dll"]