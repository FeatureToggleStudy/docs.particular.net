﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using NServiceBus.Persistence.Sql.ScriptBuilder;
using NServiceBus.Unicast.Subscriptions;
using NUnit.Framework;
#pragma warning disable 618

public class ScriptWriter
{
    [Test]
    public void Write()
    {
        var directory = Path.Combine(TestContext.CurrentContext.TestDirectory, "../../../");
        foreach (var variant in Enum.GetValues(typeof(BuildSqlDialect)).Cast<BuildSqlDialect>())
        {
            Write(directory, variant, "TimeoutCreate", TimeoutScriptBuilder.BuildCreateScript(variant));
            Write(directory, variant, "TimeoutDrop", TimeoutScriptBuilder.BuildDropScript(variant));

            Write(directory, variant, "OutboxCreate", OutboxScriptBuilder.BuildCreateScript(variant));
            Write(directory, variant, "OutboxDrop", OutboxScriptBuilder.BuildDropScript(variant));

            Write(directory, variant, "SubscriptionCreate", SubscriptionScriptBuilder.BuildCreateScript(variant));
            Write(directory, variant, "SubscriptionDrop", SubscriptionScriptBuilder.BuildDropScript(variant));

            var sagaDefinition = new SagaDefinition(
                tableSuffix: "OrderSaga",
                name: "OrderSaga",
                correlationProperty: new CorrelationProperty(
                    name: "OrderNumber",
                    type: CorrelationPropertyType.Int),
                transitionalCorrelationProperty: new CorrelationProperty(
                    name: "OrderId",
                    type: CorrelationPropertyType.Guid));
            Write(directory, variant, "SagaCreate", SagaScriptBuilder.BuildCreateScript(sagaDefinition, variant));
            Write(directory, variant, "SagaDrop", SagaScriptBuilder.BuildDropScript(sagaDefinition, variant));
        }

        var dialects = new SqlDialect[]
        {
            new SqlDialect.MsSqlServer(),
            new SqlDialect.MySql(),
            new SqlDialect.Oracle()
        };

        foreach (var dialect in dialects)
        {
            var timeoutCommands = TimeoutCommandBuilder.Build(
                sqlDialect: dialect,
                tablePrefix: "EndpointName");
            Write(directory, dialect, "TimeoutAdd", timeoutCommands.Add);
            Write(directory, dialect, "TimeoutNext", timeoutCommands.Next);
            Write(directory, dialect, "TimeoutRange", timeoutCommands.Range);
            Write(directory, dialect, "TimeoutRemoveById", timeoutCommands.RemoveById);
            Write(directory, dialect, "TimeoutRemoveBySagaId", timeoutCommands.RemoveBySagaId);
            Write(directory, dialect, "TimeoutPeek", timeoutCommands.Peek);

            var outboxCommands = OutboxCommandBuilder.Build(
                tablePrefix: "EndpointName",
                sqlDialect: dialect);
            Write(directory, dialect, "OutboxCleanup", outboxCommands.Cleanup);
            Write(directory, dialect, "OutboxGet", outboxCommands.Get);
            Write(directory, dialect, "OutboxSetAsDispatched", outboxCommands.SetAsDispatched);
            Write(directory, dialect, "OutboxStore", outboxCommands.Store);

            var subscriptionCommands = SubscriptionCommandBuilder.Build(
                sqlDialect: dialect,
                tablePrefix: "EndpointName");
            Write(directory, dialect, "SubscriptionSubscribe", subscriptionCommands.Subscribe);
            Write(directory, dialect, "SubscriptionUnsubscribe", subscriptionCommands.Unsubscribe);
            Write(directory, dialect, "SubscriptionGetSubscribers", subscriptionCommands.GetSubscribers(new List<MessageType>
            {
                new MessageType("MessageTypeName", new Version())
            }));

            var sagaCommandBuilder = new SagaCommandBuilder(dialect);
            Write(directory, dialect, "SagaComplete", sagaCommandBuilder.BuildCompleteCommand("EndpointName_SagaName"));
            Write(directory, dialect, "SagadGetByProperty", sagaCommandBuilder.BuildGetByPropertyCommand("PropertyName", "EndpointName_SagaName"));
            Write(directory, dialect, "SagaGetBySagaId", sagaCommandBuilder.BuildGetBySagaIdCommand("EndpointName_SagaName"));
            Write(directory, dialect, "SagaSave", sagaCommandBuilder.BuildSaveCommand("CorrelationProperty", "TransitionalCorrelationProperty", "EndpointName_SagaName"));
            Write(directory, dialect, "SagaUpdate", sagaCommandBuilder.BuildUpdateCommand("TransitionalCorrelationProperty", "EndpointName_SagaName"));

            // since we don't have doco on oracle saga finders
            if (!(dialect is SqlDialect.Oracle))
            {
                Write(directory, dialect, "SagaSelect", sagaCommandBuilder.BuildSelectFromCommand("EndpointName_SagaName"));
            }
        }
    }

    #region CreationScriptSaga

    public class OrderSaga :
        SqlSaga<OrderSaga.OrderSagaData>
    {
        public class OrderSagaData :
            ContainSagaData
        {
            public int OrderNumber { get; set; }
            public Guid OrderId { get; set; }
        }

        protected override string CorrelationPropertyName => nameof(OrderSagaData.OrderNumber);

        protected override string TransitionalCorrelationPropertyName => nameof(OrderSagaData.OrderId);

        #endregion

        protected override void ConfigureMapping(IMessagePropertyMapper mapper)
        {
        }

    }

    static void Write(string testDirectory, BuildSqlDialect variant, string suffix, string script)
    {
        Write(testDirectory, suffix, script, variant.ToString());
    }

    static void Write(string testDirectory, SqlDialect dialect, string suffix, string script)
    {
        Write(testDirectory, suffix, script, dialect.ToString());
    }

    static void Write(string testDirectory, string suffix, string script, string variantAsString)
    {
        var path = Path.Combine(testDirectory, $"{variantAsString}_{suffix}.sql");
        File.Delete(path);
        using (var writer = File.CreateText(path))
        {
            Trace.WriteLine($"{variantAsString}_{suffix}Sql");
            writer.WriteLine($@"startcode	{variantAsString}_{suffix}Sql".Replace("\t", " "));
            writer.WriteLine(script);
            writer.WriteLine("endcode");
        }
    }
}