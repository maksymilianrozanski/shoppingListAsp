FROM mcr.microsoft.com/dotnet/sdk:5.0
ENV PATH $PATH:/root/.dotnet/tools
RUN dotnet tool install -g dotnet-ef --version 5.0.2
COPY . /app
WORKDIR /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
EXPOSE 80/tcp
EXPOSE 5000/tcp
RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh
