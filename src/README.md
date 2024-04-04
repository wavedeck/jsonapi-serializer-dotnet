# WavedeckLabs.JsonApiSerializer for C# / .NET 8

WIP: 🚧 This project is production ready but not yet feature complete

JsonApiSerializer simply serializes any object into a JSON:API v1.1 conforming document. This is useful for quickly building JSON:API Documents for use with ASP.NET Core APIs.

## Features

This library serializes any kind of object into either a single Resource Document or a ResourceCollection Document. It also has the ability to

- Change the name of the used Id Parameter
- Change the resulting JsonNamingPolicy

by providing a `JsonApiSerializerConfig` to the serialization methods.

## Roadmap

I'm planning to extend this library to also support serializing meta data and links / relations. The goal is to support most of the JSON:API v1.1 spec by version 1.0 of this library

## Contributing and License

Any contributions to this projects are welcome.

This project is MIT licensed, without any warranty or liability.

Do whatever you want with this code, but don't attempt make me responsible.

## Stability, Security and Performance

This library is already in use for some of my projects at my day job, so I keep best efforts to ensure this library is stable, fast and secure.

Due to relying on reflection for the conversion of objects to JSON:API resources, i do not recommend serializing user provided data without prior sanitization. Sanitizing user input should however be a standard security measure regardless of the library used.

Current benchmarks of this project result in an average of 2.13 μs per serialization invocation. This means about 500_000 possible invocations per second.

Benchmarks were performed on a Mac with the following specs:

CPU: Apple Silicon M1 Pro
RAM: 32 GB DDR4
OS: macOS Sonoma 14.4
SDK: 8.0.201