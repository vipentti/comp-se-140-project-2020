# COMP.SE.140 Project

- Author: Ville Penttinen

An implementation of the project work for `COMP.SE.140`.
The project is based on an earlier [exercise](https://github.com/vipentti/ex-5-amqp).

## Requirements

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/)
- [curl](https://curl.se/)

The application should run on any platform with Docker and Docker Compose installed.
It has been tested on Windows and on Linux.

**NOTE:** The various scripts and commands assume that the user has rights to use docker and docker-compose commands without sudo. See [here](https://docs.docker.com/engine/install/linux-postinstall/) for more details.

## Getting started

1. Clone the repository
2. Navigate into the folder and run
   ```
   docker-compose build
   docker-compose up -d
   ```
   - This will take a while the first time
3. Wait for up to 30 seconds
4. Then run
   ```sh
   curl http://localhost:8081/messages
   ```

This should return response similiar to:

```text
2020-12-03T09:41:59.525Z Topic my.o: MSG_1
2020-12-03T09:42:00.526Z Topic my.i: Got MSG_1
2020-12-03T09:42:02.500Z Topic my.o: MSG_2
2020-12-03T09:42:03.506Z Topic my.i: Got MSG_2
2020-12-03T09:42:05.505Z Topic my.o: MSG_3
2020-12-03T09:42:06.512Z Topic my.i: Got MSG_3
```

Afterwards you may pause sending of further messages by running the following command

```sh
curl --request PUT --url http://localhost:8081/state --header 'content-type: text/plain' --data PAUSED
```

## Testing

Tests may be found under `tests/` directory. Tests are implemented utilizing [xUnit.net](https://xunit.net/).

A utility script is provided in [DevOps/scripts/run-e2e-tests.sh](DevOps/scripts/run-e2e-tests.sh) for running the full test suite, including end-to-end tests using docker.

Run the following in the root directory

```sh
./DevOps/scripts/run-e2e-tests.sh
```

The results of the test run may then be found under `testresults/`

Test results should contain several JUnit XML-files in addition to a text-file containing the console log of running the tests.

Optionally, the JUnit XML-files may be utilized in creating a self-contained HTML file, for example by utilizing [Xunit Viewer](https://github.com/lukejpreston/xunit-viewer) (requires that NodeJS is installed).

```
npx xunit-viewer --results testresults/ --title "TestResults" --output TestResults.html
```

This should result in a file `TestResults.html` which can be opened in a browser.

**NOTE:** As of 3.12.2020 Xunit Viewer has a problem where the generator does not exit even if the file was written successfully, this may occur with NodeJS v15, see [https://github.com/lukejpreston/xunit-viewer/issues/92](https://github.com/lukejpreston/xunit-viewer/issues/92) for more details.

Solution is to simply press `Ctrl + C` once the file has been written into.

The following section describes the features of the application in more details.

## Application and its features

The application consists of five [.NET 5](https://devblogs.microsoft.com/dotnet/introducing-net-5/) projects. In addition both [RabbitMQ](https://www.rabbitmq.com/) and [Redis](https://redis.io/) are utilized.

All of the services can found in [docker-compose.yml](docker-compose.yml).

### `src/APIGateway`

The main entrypoint of the application. By default exposed via `http://localhost:8081`. Consists of various endpoint which can be utilized for both getting information about the current state of the application in addition to updating the current state. The endpoints are explained in more detail in [API Gateway endpoints](#api-gateway-endpoints)

### `src/HttpServer`

Basic HTTP Server which reads the shared text-file containing the messages written by the `Observer` and returns its contents when requested.

### `src/Original`

Original continuously publishes (unless paused) messages in the form of `MSQ_<NR>` where number is an incrementing number starting from 1 to the topic `my.o`. The messages are sent with a configurable delay between them, by default 3 seconds.

### `src/Intermediate`

Intermediate subscribes to the topic `my.o` and when a message is received Intermediate sends a new message in the format `Got <the received message>` to the topic `my.i` after a small delay, by default 1 second.

### `src/Observer`

Observer subscribes to both topics `my.o` and `my.i`, when it receives a message it writes into a file. This file can be found locally, by default in `.shared_volume/message.txt` after running the application.


## API Gateway endpoints

The following sections each describe an endpoint supported by the `APIGateway`.

### GET /state

Get the current state

```sh
curl --request GET --url http://localhost:8081/state
```

State may be one of the following

- INIT
- RUNNING
- PAUSED
- SHUTDOWN

### GET /messages

Get the messages that have been sent

```sh
curl --request GET --url http://localhost:8081/messages
```

### GET /run-log

Get logs regarding the state changes

```sh
curl --request GET --url http://localhost:8081/run-log
```

### PUT /state

Set the application state to the given state. If the given state is equal to the current state, nothing is done and no run-log entry will be created. If the given state is different, a run-log entry is created and the new state is returned.

State may be one of the following:

#### INIT

Re-initializes the application and `Original` starts sending messages again. State is automatically updated to `RUNNING`.

```sh
curl --request PUT --url http://localhost:8081/state --header 'content-type: text/plain' --data INIT
```

#### RUNNING

Resumes sending of messages, if the state has been previously set to `PAUSED`.

```sh
curl --request PUT --url http://localhost:8081/state --header 'content-type: text/plain' --data RUNNING
```

#### PAUSED

Pauses `Original` from sending messages.

```sh
curl --request PUT --url http://localhost:8081/state --header 'content-type: text/plain' --data PAUSED
```

#### SHUTDOWN

Shuts down the containers, except for `rabbitmq` and `redis`. After `SHUTDOWN` has been issued, the only way to start sending again is to start the application again for example by using `docker-compose up -d`

```sh
curl --request PUT --url http://localhost:8081/state --header 'content-type: text/plain' --data SHUTDOWN
```

### GET /node-statistic

Get statistics regarding the [RabbitMQ node](https://www.rabbitmq.com/monitoring.html#node-metrics).

```sh
curl --request GET --url http://localhost:8081/node-statistic
```

Returns data in the following JSON format:

```json
{
  "name": "rabbit@rabbitmq",
  "fd_used": 37,
  "mem_used": 113664000,
  "sockets_used": 4,
  "proc_used": 607,
  "uptime": 4646780
}
```

### GET /queue-statistic

Get statistics regarding the [RabbitMQ queues](https://www.rabbitmq.com/monitoring.html#queue-metrics).

```sh
curl --request GET --url http://localhost:8081/queue-statistic
```

Returns data in the following JSON format:

```json
[
  {
    "name": "amq.gen--3bMLm5lFcOZ9LVfcnXUaw",
    "messages_delivered_recently": 3,
    "message_delivery_rate": 0.0,
    "messages_published_recently": 3,
    "message_publish_rate": 0.0
  },
  {
    "name": "amq.gen-Z93qxRl3nLbfkYM8542E9A",
    "messages_delivered_recently": 3,
    "message_delivery_rate": 0.0,
    "messages_published_recently": 3,
    "message_publish_rate": 0.0
  },
  {
    "name": "amq.gen-_pZeB-ufMbNExB2foJgy0g",
    "messages_delivered_recently": 3,
    "message_delivery_rate": 0.0,
    "messages_published_recently": 3,
    "message_publish_rate": 0.0
  },
  {
    "name": "amq.gen-o3ThsfBy6Yycbgovwyoyrg",
    "messages_delivered_recently": 6,
    "message_delivery_rate": 0.0,
    "messages_published_recently": 6,
    "message_publish_rate": 0.0
  }
]
```

