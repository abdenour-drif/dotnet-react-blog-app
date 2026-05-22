FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

COPY Blog.Api/Blog.Api.csproj ./

RUN dotnet restore

EXPOSE 5270

CMD ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:5270"]