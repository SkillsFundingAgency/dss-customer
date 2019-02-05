using DFC.Common.Standard.Logging;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.ChangeFeed.SQLServer
{
    public class SQLServerProvider : ISQLServerProvider
    {
        private readonly ILoggerHelper _loggerHelper;
        private readonly IDbConnection _dbConnection;
        public SQLServerProvider(ILoggerHelper loggerHelper, IDbConnection dbConnection)
        {
            _loggerHelper = loggerHelper;
            _dbConnection = dbConnection;
        }

        public async Task<bool> UpsertResource(Document document, ILogger log)
        {
            try
            {
                _loggerHelper.LogMethodEnter(log);

                await Task.Run(() => Execute(document));

                _loggerHelper.LogMethodExit(log);
                return true;
            }
            catch (Exception ex)
            {
                _loggerHelper.LogException(log, Guid.NewGuid(), ex);
                return false;
            }
        }

        private void Execute(Document document)
        {
            _dbConnection.Open();

            var dbCommand = BuildCommand();

            dbCommand.Parameters.Add(BuildParameter(dbCommand, document));

            dbCommand.ExecuteNonQuery();

            _dbConnection.Close();
        }

        private IDbDataParameter BuildParameter(IDbCommand command, Document document)
        {
            IDbDataParameter dbParameter = command.CreateParameter();
            dbParameter.ParameterName = "@CustomerJson";            
            dbParameter.Direction = ParameterDirection.Input;
            dbParameter.Value = document.ToString();

            return dbParameter;
        }

        private IDbCommand BuildCommand()
        {
            IDbCommand result = _dbConnection.CreateCommand();
            result.CommandType = CommandType.StoredProcedure;
            result.CommandText = "UpsertCustomer";

            return result;
        }
    }
}
