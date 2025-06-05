using System.Data.SqlClient;
using Dapper;

namespace IntegrationProject.DbHelpers
{
    public class IntegrationDbHelper
    {
        private readonly string _connectionString;

        public IntegrationDbHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task InsertBatchAsync<T>(string sql, List<T> dataList, int batchSize = 500)
        {
            if (dataList is null || !dataList.Any())
                return;

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            foreach (var batch in dataList.Chunk(batchSize))
            {
                await connection.ExecuteAsync(sql, batch, transaction);
            }

            await transaction.CommitAsync();
        }
    }
}
