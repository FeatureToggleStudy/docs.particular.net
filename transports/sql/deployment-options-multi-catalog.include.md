In the multi-catalog mode each endpoint is assigned a separate catalog within a single SQL Server instance. That catalog holds both the business data and the message data. Given two endpoints, Sales and Billing, here's the mapping of the endpoint names to the queue table names:

| Endpoint | Queue table name                                                      |
|----------|-----------------------------------------------------------------------|
| Sales    | `[sql_server_instance_01]`.**`[sales_database]`**.`[shared_database]`.`[Sales]`     |
| Billing  | `[sql_server_instance_01]`.**`[billing_database]`**.`[shared_database]`.`[Billing]` |

In order to use this mode the transport-specific [addressing](/transports/sql/addressing.md) information has to be provided to map endpoints and/or queues to catalogs.

The multi-catalog mode can be combined with the multi-schema mode so that groups of endpoints are assigned catalogs and, within each catalog, each endpoint (or a sub-group of endpoints) has its own schema.