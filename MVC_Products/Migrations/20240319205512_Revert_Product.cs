using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MVC_Product.Migrations
{
    /// <inheritdoc />
    public partial class Revert_Product : Migration
    {
        /// <inheritdoc/>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS \"Products\";");
        }
/// <inheritdoc/>

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO
                $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_database WHERE datname = 'ProductDatabase') THEN
                        CREATE DATABASE ProductDatabase;
                    END IF;
                END
                $$;

                CREATE TABLE ""Products"" (
                ""Id"" SERIAL PRIMARY KEY,
                ""Name"" TEXT NOT NULL,
                ""Price"" NUMERIC NOT NULL
                );
                GRANT SELECT, INSERT, UPDATE, DELETE ON ""Products"" TO tester;
            ");
            
        }
    }
}
