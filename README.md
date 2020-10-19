# Exercise 5 message queue

- Author: Ville Penttinen

An implementation of the message queue exercise for `COMP.SE 140`.

## Requirements

- Docker

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

## Benefits of topic-based communication

- A single message sent to a topic may be read by multiple listeners
- Listener may subscribe to multiple topics
- Publishers and subscribers are loosely coupled
- Publishers may simply send messages to the specific topic without having to know about subscribers

## What I learned from this excercise

- Improved understanding of RabbitMQ
- How to utilize RabbitMQ's `C#`-client
- How to setup a shared docker volume where file written to by one container may be read by another
- Improved understanding of topic-based communication

## Structure

The application contains 5 .NET projects.

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

Very basic HttpServer which simply reads the file written by `Observer` when a http request is made.
By default listens for `http://localhost:8080`.
