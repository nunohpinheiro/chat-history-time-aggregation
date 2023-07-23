# chat-history-time-aggregation
Chat room interface in which the user can view chat history at varying levels of time-based aggregation

## Run the application

In the root of the repository, execute:
* `docker compose --profile infrastructure up` to lift up the required infrastructure external to the app
    * Includes InfluxDb and its seeder
    * For development/testing purposes, with the app being ran from other means (IDE, console,...)
* `docker compose --profile app up` to lift up the application (Web REST API) and the required external infrastructure
    * Includes the `ChatHistory API` (running in port `8080`), InfluxDb and its seeder
        * Use the API with its Swagger UI: [http://localhost:8080/docs/index](http://localhost:8080/docs/index)
    * For "production-like" purposes, to actually use the API
