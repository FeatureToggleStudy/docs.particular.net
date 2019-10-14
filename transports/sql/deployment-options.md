---
title: Deployment options
reviewed: 2019-10-14
component: SqlTransport
tags:
- Transport
related:
 - nservicebus/operations
redirects:
 - nservicebus/sqlserver/multiple-databases
 - nservicebus/sqlserver/deployment-options
 - transports/sqlserver/deployment-options
---

When using SQL Server Transport, business data is likely to also be stored in the SQL Server. There are multiple possible ways in which business data and message data (queues) can be hosted in the SQL Server infrastructure. Regardless of the specific deployment option, as long as all the data resides on a single instance of SQL Server, NServiceBus can run in an *exactly-once* message processing mode. This means that, when a result of handling a message is updating the business data and sending out further messages, all three involved operations:
- consuming the incoming messages
- updating data
- sending outgoing messages
either succeed or fail as a single unit. As a consequence, there is no duplicate messages produced and no need to make the business logic *idempotent*. 

NServiceBus SQL and NHibernate persistence automatically detect SQL Server transport transaction context and use it for accessing [Saga](/nservicebus/sagas) data in the *exactly-once* processing mode. In order for the handler logic to also benefit from that mode, the data access code needs to use SQL Server transport transaction context.

In the `TransactionScope` [transaction mode](/transports/transactions) SQL Server transport ensures that there is a `TransactionScope` when the message handler is executed so that when a connection is open, it is automatically enlisted in an ongoing transaction.

In the `SendsAtomicWithReceive` mode the message handler has to retrieve the native transaction and connection object from the handling context and pass it to the data access routines

snippet: TransactionContext

NOTE: The code above can be moved to a shared infrastructure class e.g. a [behavior](/nservicebus/pipeline/manipulate-with-behaviors.md) that establishes the object-relational mapper's unit of work.

partial: content
