## Database Migrations

### NPM command for generating models from database

####
Prerequisites: `Install-Package Microsoft.EntityFrameworkCore.Tools`

* `Scaffold-DbContext "server=127.0.0.1;port=5432;user id=;password=;database=;" Npgsql.EntityFrameworkCore.PostgreSQL -OutputDir Scaffolds`

### Migration commands

#### Add

* `Add-Migration -Context "AppDbContext" [name]`
* `dotnet ef migrations add [name] --context "AppDbContext"`

#### Remove

* `Remove-Migration -Context "AppDbContext" [name]`
* `dotnet ef migrations remove [name]`

#### Update

`$env:DB_CONNECTION_STRING=''`

* `Update-Database -Context "AppDbContext" [opt:name (0:removes all)]`    
* `dotnet ef database update [opt:name (0:removes all)] --context "AppDbContext"`
