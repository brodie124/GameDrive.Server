# GameDrive.Server

### Migrations
This application handles EntityFramework Core migrations in a slightly atypical manner. 
Whilst EF Core would normally store migrations in a folder (usually called `Migrations`) inside the main project,
we have opted to store the migrations in a dedicated, separate project that exists within the solution.

#### Why has it been structured this way?
* This application supports multiple database providers - some of these may require specific migrations.
* Tests operate using an in-memory SQLite of the database. This is created using the additional available migrations.
* Finally, it makes it as clear as day where the migrations are stored.

#### How do I work with migrations in this way?
> You must be inside the `GameDrive.Server` project - not the root of the repository - to execute migration-related commands.

The chosen structure of the migrations means that you **cannot** simply use the `dotnet ef migrations ...` command to
execute migration-related operations because the tool no longer "knows" where to find the migrations folder.

To inform the tool where to find the migrations, append one of the following set of CLI args depending on which
provider you are creating a migration for.

| Provider | CLI Args                                                               |
|----------|------------------------------------------------------------------------|
| MySQL    | `--project ../GameDrive.Server.Migrations.MySQL -- --provider Mysql`   |
| SQLite   | `--project ../GameDrive.Server.Migrations.SQLite -- --provider Sqlite` |


For example, to create a new MySQL migration you would execute the following command:
`dotnet ef migrations add SomeMigration --project ../GameDrive.Server.Migrations.MySQL -- --provider Mysql`