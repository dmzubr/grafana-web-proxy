FROM mcr.microsoft.com/dotnet/core/runtime:3.1

WORKDIR /app

COPY ./bin/Release/netcoreapp3.1/ubuntu.18.04-x64/publish ./

RUN rm -f /app/*.pfx && \
    rm -f /app/appsettings.json && \
    mv /app/appsettings_example.json /app/appsettings.json 

ENTRYPOINT ["dotnet", "GrafanaProxy.WebApi.dll"]