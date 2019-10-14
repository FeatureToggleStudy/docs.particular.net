using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Transport;

class TransactionContext
{
    class MyMessageHandler : IHandleMessages<MyMessage>
    {
        #region TransactionContext

        public async Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            var transportTransaction = context.Extensions.Get<TransportTransaction>();
            var connection = transportTransaction.Get<SqlConnection>();
            var transaction = transportTransaction.Get<SqlTransaction>();

            using (var command = new SqlCommand("update...", connection, transaction))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        #endregion
    }

    class MyMessage : IMessage
    {
    }
}