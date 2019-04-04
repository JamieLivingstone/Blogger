FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .

COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done

COPY tests/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p tests/${file%.*}/ && mv $file tests/${file%.*}/; done

RUN dotnet restore

# copy everything else and build app
COPY . .
WORKDIR /app/src/Blogger.WebApi

# Run Dotnet watch to hot compile changes
ENV DOTNET_USE_POLLING_FILE_WATCHER 1
ENTRYPOINT ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:5000"]