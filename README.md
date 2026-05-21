# dotnet-react-blog-app
A Simple Full-stack Blog App | .NET Core Web API | MS SQL | React Vite | Docker-Compose

### Revert SQLiteDB:
```bash
rm BlogStore.db
dotnet ef migrations remove
dotnet ef migrations add InitialCreate --output-dir Data/Migrations
dotnet ef database update
```