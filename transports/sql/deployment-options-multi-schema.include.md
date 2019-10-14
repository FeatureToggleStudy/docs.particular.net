In the multi-schema mode queue tables exist in a single catalog are assigned different schemas. Given two endpoints, Sales and Billing, here's the mapping of the endpoint names to the queue table names:

| Endpoint | Queue table name                                                      |
|----------|-----------------------------------------------------------------------|
| Sales    | `[sql_server_instance_01]`.`[shared_database]`.**`[sales_schema]`**.`[Sales]`     |
| Billing  | `[sql_server_instance_01]`.`[shared_database]`.**`[billing_schema]`**.`[Billing]` |

In order to use this mode the transport-specific [addressing](/transports/sql/addressing.md) information has to be provided to map endpoints and/or queues to schemas.

