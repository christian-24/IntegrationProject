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

        private async Task<ImportResult> ImportProducts()
        {
            return await ImportStep<Product, ProductsMapper>(
                nameof(ImportProducts),
                RequestConsts.ProductsRequest,
                ";",
                true,
                true,
                _fileHelper.GetLocalPathToSaveFile(FileNames.ProductFileName),
                list => list
                    .Where(p =>
                        p.Shipping.IsStringAsHoursLessOrEqualExcepted(24) &&
                        !(string.IsNullOrWhiteSpace(p.Name) && p.Name.ToLower().Contains("kabel ")))
                    .ToList(),
                InsertQueries.InsertProductsSql
            );
        }

        private async Task<ImportResult> ImportInventory()
        {
            return await ImportStep<Inventory, InventoryMapper>(
                nameof(ImportInventory),
                RequestConsts.InventoryRequest,
                ",",
                true,
                false,
                _fileHelper.GetLocalPathToSaveFile(FileNames.InventoryFileName),
                list => list.Where(i => i.IsShippingIn24Hours).ToList(),
                InsertQueries.InsertInventorySql
            );
        }

        private async Task<ImportResult> ImportPrices()
        {
            return await ImportStep<Prices, PricesMapper>(
                nameof(ImportPrices),
                RequestConsts.PricesRequest,
                ",",
                false,
                false,
                _fileHelper.GetLocalPathToSaveFile(FileNames.PricesFileName),
                null,
                InsertQueries.InsertPricesSql
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
