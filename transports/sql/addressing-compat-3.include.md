Version 3.0 of SQL Server transport does not recognize the catalog part of the address. If such an endpoint receives a three-part address, e.g. `MyTable@[MySchema]@[MyCatalog]` (either as a reply-to address or as a subscriber address), the Version 3.0 transport endpoint will drop the last part (catalog) when parsing the address.

If the communicating endpoints use different catalogs, the Version 3.0 endpoint needs to be configured to use [multi-instance mode](/transports/sql/deployment-options.md?version=SqlTransport_3#multi-instance) with `MyTable@[MySchema]` address bound to a connection string that specifies `MyCatalog` as an initial catalog.