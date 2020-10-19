# Exercise 5 message queue

- Author: Ville Penttinen

An implementation of the message queue exercise for `COMP.SE 140`.

## Requirements

- Docker

## Purpose

To demonstrate the ability to publish messages into a message queue utilizing C# and RabbitMQ.

Original publishes messages in the form of `MSQ_<NR>` where number is an incrementing number starting from 1 to the topic `my.o`.

Intermediate listens subscribes to listen to `my.o` and then when a message is received Intermediate sends a new message in the format `Got <the received message>` to the topic `my.i`

Observer subscribes to both topics `my.o` and `my.i`, when it receives a message it writes into a file. This file can be found locally, by default in `.shared_volume/message.txt` after running the application.

HttpServer is a simple http server, which listens to a GET request and then reads the contents of the file written to by Observer and returns the contents of that file.


## Getting started

1. Clone the repository
2. Navigate into the folder and run
   ```
   docker-compose up --build -d
   ```
   - This may take a while the first time
3. Wait for up to 30 seconds
4. Then run
   ```
   curl http://localhost:8080
   ```

This should return response similiar to:

```text
2020-10-19T19:10:12.9517147Z Topic my.o: MSG_1
2020-10-19T19:10:13.9489234Z Topic my.i: Got MSG_1
2020-10-19T19:10:15.9460544Z Topic my.o: MSG_2
2020-10-19T19:10:16.9507742Z Topic my.i: Got MSG_2
2020-10-19T19:10:18.9478337Z Topic my.o: MSG_3
2020-10-19T19:10:19.9498052Z Topic my.i: Got MSG_3
```

## Benefits of topic-based communication

- A single message sent to a topic may be read by multiple listeners
- Listener may subscribe to multiple topics
- Publishers and subscribers are loosely coupled
- Publishers may simply send messages to the specific topic without having to know about subscribers

## What I learned from this exercise

- Improved understanding of RabbitMQ
- How to utilize RabbitMQ's `C#`-client
- How to setup a shared docker volume where file written to by one container may be read by another
- Improved understanding of topic-based communication

## Structure

### src/Common

Common and shared functionality

### src/Original

This service starts up, connects to the RabbitMQ instance and then starts sending messages.
The service sends total of 3 messages with a 3 second delay between each message.

### src/Intermediate

Listens for the messages sent by Original, then waits for 1 second before sending a message to its own topic.

### src/Observer

Listens for messages from both Original and Intermediate, writes them to the file `OutFilePath`.
By default when running with `docker-compose` the `OutFilePath` is set to a shared volume which is mounted to `.shared_volume/` locally.

### src/HttpServer

Very basic http server which simply reads the file written by `Observer` when a http request is made.
By default listens for `http://localhost:8080`.
