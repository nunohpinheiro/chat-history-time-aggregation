# chat-history-time-aggregation

Chat History API, in which the user can query chat history at varying levels of time-based aggregation.

The API has its persistence in InfluxDB, which is brought up - with data seed - using `docker-compose`

## Index
1. [API description](#api-description)
2. [Infrastructure in Docker](#infrastructure-in-docker)
1. [Run the application and use it](#run-the-application-and-use-it)

## API description

* Open API file is generated whenever the main project `ServiceApi` builds
    * Find it in [`ChatHistoryAPI_OpenAPI.yaml`](https://github.com/nunohpinheiro/chat-history-time-aggregation/blob/main/ChatHistoryAPI_OpenAPI.yaml)
        * With visuals: https://petstore.swagger.io/?url=https://github.com/nunohpinheiro/chat-history-time-aggregation/blob/main/ChatHistoryAPI_OpenAPI.yaml

### Brief endpoints description
The API has two endpoints:

#### `GET /v1/chat-records`
Queries the chat records collection according to different query parameters:
* `granularity`, `pageNumber`, `pageSize`, `startDateTime`, `endDateTime`
* `granularity` can be `MinuteByMinute`, `Hourly`, `Daily`, `Monthly`, `Yearly`
    * With `MinuteByMinute`, the endpoint will return a list of detailed chat records, _e.g._:
    ```
    ["2023-7-23, 01:51pm: User1 comments: \"sample comment 1\"",
    "2023-7-23, 01:56pm: User2 comments: \"sample comment 2\"",
    "2023-7-23, 02:01pm: User1 enters the room",
    "2023-7-23, 02:06pm: User2 enters the room",
    "2023-7-23, 02:11pm: User1 high-fives \"Mr. User\"",
    "2023-7-23, 02:13pm: User2 high-fives \"Mrs. User\"",
    "2023-7-23, 02:16pm: User2 high-fives \"Mrs. Another User\"",
    "2023-7-23, 02:21pm: User1 leaves",
    "2023-7-23, 02:26pm: User2 leaves"]
    ```
    * With the other options, the endpoint will return a list of aggregated chat records, _e.g._ (for `Hourly`):
    ```
    ["2023-7-21, 01pm: 2 comments",
    "2023-7-21, 02pm: 2 people entered",
    "2023-7-21, 02pm: 2 people left",
    "2023-7-21, 02pm: 2 people high-fived 3 other people"]
    ```
    * More examples of responses can be found in folder [`tests/ServiceApi.IntegrationTests/test-snapshots`](https://github.com/nunohpinheiro/chat-history-time-aggregation/tree/main/tests/ServiceApi.IntegrationTests/test-snapshots)
        * These are response snapshots produced by integration tests running against the API itself, using snapshot testing

#### `POST /v1/chat-records`
Creates a new chat record in each call, with the following request body:
```json
{
  "eventType": "enter-the-room", // Or "leave-the-room", "comment", "high-five-another-user"
  "timestamp": "2023-07-17T00:00:00Z", // When the event occurs, must be in format "yyyy-MM-ddTHH:mm:ssZ"
  "user": "string", // Who performs the event, must respect the regex "^[ A-Za-z0-9_@.-]*$"
  "commentText": "string", // Used in "comment" event types (the comment's text)
  "highFivedPerson": "string" // Used in "high-five-another-user" event types (the person that receives the high-five)
}
```

## Infrastructure in Docker

All the infrastructure in this project can be lift up using Docker (check the file [`docker-compose.yml`](https://github.com/nunohpinheiro/chat-history-time-aggregation/blob/main/docker-compose.yml))

Three services are included in the `docker-compose`:
* `chat-history-api`: The API itself to interact with
* `influxdb-chat-history`: InfluxDB instance that persists the chat records
* `influxcli-seeder-chat-history`: InfluxDB data seeder, which puts initial chat records into InfluxDB - data and Dockerfile can be found in folder [influxdb](https://github.com/nunohpinheiro/chat-history-time-aggregation/tree/main/influxdb)

## Run the application and use it

In the root of the repository, execute:
* `docker compose --profile infrastructure up` to lift up the required infrastructure external to the app
    * Includes InfluxDb and its seeder
    * For development/testing purposes, with the app being ran from other means (IDE, console,...)
* `docker compose --profile app up` to lift up the application (Web REST API) and the required external infrastructure
    * Includes the `ChatHistory API` (running in port `8080`), InfluxDb and its seeder
        * Use the API with its Swagger UI: [http://localhost:8080/docs/index](http://localhost:8080/docs/index)
    * For "production-like" purposes, to actually use the API
