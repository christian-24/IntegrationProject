# IntegrationProject

A lightweight integration system for importing product, inventory, and pricing data from external CSV sources into a local SQL Server database.
Data is fetched via HTTP, parsed with CsvHelper, optionally filtered, and stored using Dapper.
Includes API endpoints for triggering imports and retrieving product details by SKU.
Built with .NET 7, ASP.NET Core, and tested via GitHub Actions.