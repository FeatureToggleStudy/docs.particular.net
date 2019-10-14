In the single-schema mode all tables, both for business data and message data, for all endpoints exist in a single schema of a single catalog. Given two endpoints, Sales and Billing, here's the mapping of the endpoint names to the queue table names:

| Endpoint | Queue table name                                                      |
|----------|-----------------------------------------------------------------------|
| Sales    | `[sql_server_instance_01]`.`[shared_database]`.`[shared_schema]`.`[Sales]`  |
| Billing  | `[sql_server_instance_01]`.`[shared_database]`.`[shared_schema]`.`[Billing]`|

This mode does not require any transport-specific addressing configuration so it is easy to set up. Another advantage of the single-schema mode is the fact that a snapshot (backup) of the entire system state can be done by backing up a single database.

A downside of single-schema mode is that it becomes complex to manage when the number of endpoints increases. The fact that each endpoint can access any data means that boundaries enforced by message contracts can be compromised on the data access level. Consider using the single-schema approach for deployments of up to five endpoints.

A way to mitigate the above problem is to assign each endpoint a separate catalog for data storage and use the shared catalog only for the queues. That mode requires that all data access code in the message handlers uses fully-qualified table names that include the catalog name. The advantage of running this mode is ability to fine-tune the data storage options for the messaging catalog.