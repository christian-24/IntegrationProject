using System.Reflection;
using CsvHelper.Configuration;
using IntegrationProject.Consts;
using IntegrationProject.DbHelpers;
using IntegrationProject.Enums;
using IntegrationProject.Extensions;
using IntegrationProject.Helpers;
using IntegrationProject.Mappers;
using IntegrationProject.Models;
using IntegrationProject.Results;
using static IntegrationProject.Consts.FileConsts;
using static IntegrationProject.Data.DapperQueries;

namespace IntegrationProject.Services
{
    public class IntegrationService
    {
        private readonly DataService         _dataService;
        private readonly IntegrationDbHelper _integrationDbHelper;
        private readonly FileHelper          _fileHelper;

        public IntegrationService(DataService dataService, IntegrationDbHelper integrationDbHelper, FileHelper fileHelper)
        {
            _dataService         = dataService;
            _integrationDbHelper = integrationDbHelper;
            _fileHelper          = fileHelper;
        }

        internal async Task<List<ImportResult>> ImportDataToLocalDb()
        {
            var results = new List<ImportResult>();

            results.Add(await ImportProducts());
            results.Add(await ImportInventory());
            results.Add(await ImportPrices());

            return results;
        }

        /// <summary>
        /// Imports product data from the external API.
        /// 
        /// This process includes:
        /// - Downloading the product CSV file from a configured URL,
        /// - Saving the file locally,
        /// - Parsing the CSV using a custom class map,
        /// - Filtering out:
        ///     • Products that are wires (IsWire = true),
        ///     • Products that do not ship within 24 hours (Shipping field parsed to int ≤ 24),
        /// - Inserting the filtered data into the local database using Dapper.
        /// 
        /// Assumes:
        /// - The CSV uses semicolon (;) as a delimiter,
        /// - The file contains a header row,
        /// - Empty lines should be skipped during parsing.
        /// </summary>
        /// <returns>An <see cref="ImportResult"/> containing the step name, status, and result message.</returns>

        private async Task<ImportResult> ImportProducts()
        {
            return await ImportStep<Product, ProductsMapper>(
                stepName            : nameof(ImportProducts),
                url                 : RequestConsts.ProductsRequest,
                delimeterInFile     : ";",
                isWithHeadline      : true,
                shouldSkipEmptyLine : true,
                filePath            : _fileHelper.GetLocalPathToSaveFile(FileNames.ProductFileName),
                filter              : list => list
                    .Where(p =>
                        p.Shipping.IsStringAsHoursLessOrEqualExcepted(24) && !p.IsWire)
                    .ToList(),
                sql                 : InsertQueries.InsertProductsSql
            );
        }

        /// <summary>
        /// Imports inventory data from the external API.
        /// Downloads a CSV file, saves it locally, filters only inventory items that ship within ≤ 24h, 
        /// parses the CSV and inserts valid entries into the database.
        /// 
        /// The filter ensures:
        /// - Only rows with valid shipping time (≤ 24h) are persisted.
        /// 
        /// Empty lines and malformed shipping fields are skipped during parsing.
        /// </summary>
        /// <returns>Import result including step name, status and detailed message.</returns>
        private async Task<ImportResult> ImportInventory()
        {
            return await ImportStep<Inventory, InventoryMapper>(
                stepName            : nameof(ImportInventory),
                url                 : RequestConsts.InventoryRequest,
                delimeterInFile     : ",",
                isWithHeadline      : true,
                shouldSkipEmptyLine : false,
                filePath            : _fileHelper.GetLocalPathToSaveFile(FileNames.InventoryFileName),
                filter              : list => list.Where(i => i.IsShippingIn24Hours).ToList(),
                sql                 : InsertQueries.InsertInventorySql
            );
        }

        /// <summary>
        /// Imports pricing data from the external API.
        /// Downloads a CSV file (which may not have headers), saves it locally,
        /// parses pricing data including base price, discounted price, and logistic unit price, 
        /// and inserts it into the database without filtering.
        /// 
        /// Assumes:
        /// - The file contains no header row.
        /// - Empty lines are skipped, and all numeric conversions are safely handled.
        /// 
        /// If no prices are found, the operation returns a 'NoData' status.
        /// </summary>
        /// <returns>Import result including step name, status and detailed message.</returns>
        private async Task<ImportResult> ImportPrices()
        {
            return await ImportStep<Prices, PricesMapper>(
                stepName            : nameof(ImportPrices),
                url                 : RequestConsts.PricesRequest,
                delimeterInFile     : ",",
                isWithHeadline      : false,
                shouldSkipEmptyLine : false,
                filePath            : _fileHelper.GetLocalPathToSaveFile(FileNames.PricesFileName),
                filter              : null,
                sql                 : InsertQueries.InsertPricesSql
            );
        }

        private async Task<ImportResult> ImportStep<TModel, TMap>(string stepName, string url, string delimeterInFile, bool isWithHeadline, bool shouldSkipEmptyLine, string filePath, Func<List<TModel>, List<TModel>>? filter, string sql)
            where TMap : ClassMap<TModel>, new()
        {
            try
            {
                var response = await _dataService.GetRawCsv(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new ImportResult
                    {
                        StepName = stepName,
                        Status   = ImportStatus.ApiError,
                        Message  = $"[{stepName}] Failed to download CSV. HTTP {(int)response.StatusCode} {response.ReasonPhrase}"
                    };
                }

                await _fileHelper.SaveResponseToFileAsync(response, filePath);

                using var reader = _fileHelper.ReadDataFromFilePath(filePath);
                var data         = ParserHelper.ParseCsv<TModel, TMap>(reader, delimeterInFile, isWithHeadline, shouldSkipEmptyLine);

                if (filter is not null)
                {
                    data = filter(data);
                }

                if (!data.Any())
                {
                    return new ImportResult
                    {
                        StepName = stepName,
                        Status   = ImportStatus.NoData,
                        Message  = $"[{stepName}] No records found after filtering."
                    };
                }

                try
                {
                    await _integrationDbHelper.InsertBatchAsync(sql, data);
                }
                catch (Exception dbEx)
                {
                    return new ImportResult
                    {
                        StepName = stepName,
                        Status   = ImportStatus.DbError,
                        Message  = $"[{stepName}] Failed to insert into database: {dbEx.Message}"
                    };
                }

                return new ImportResult
                {
                    StepName = stepName,
                    Status   = ImportStatus.Success,
                    Message  = $"[{stepName}] Successfully imported {data.Count} records."
                };
            }
            catch (Exception ex)
            {
                return new ImportResult
                {
                    StepName = stepName,
                    Status   = ImportStatus.UnknownError,
                    Message  = $"[{stepName}] Unexpected error: {ex.Message}"
                };
            }
        }
    }
}
