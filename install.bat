cd MVC_Products
dotnet tool install --global dotnet-ef
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.2
dotnet ef database update Revert_Product
dotnet ef database update Product