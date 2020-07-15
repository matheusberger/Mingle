# Mingle Prototype

> A prototype of virtual enviroment that enables people to interact in a 3D space

![header](image)

## Introduction

The goal of this project is to create a virtual alternative to real world enviroments, enable people to interact within a 3D space with others, while seeing their video from a camera and hearing their voice through their microphone. The catch is that the audio must be heard only by those nearby it's source. The biggest reference to this project is this website: 

https://yorb.itp.io/

Check the website for reference and inspiration.

## Getting Started

This prototype is made of 2 parts: the Unity3D project and a Node.js server that is hosted at [Heroku](https://mingle-server-1.herokuapp.com/), but the code is available [here](https://github.com/matheusberger/Mingle-Server), along with instructions if you wish to run it locally.

### Requirements

- Unity 2019.4 or later
- Node.js (if running locally)
- Internet connection

## Usage

After cloning this repo, open it with Unity 2019.4 (or later) and open the SampleScene in the "Scenes" folder, in the "Project" window. In the "Hierachy" window you will see two objects of interest: Jammo and Networking.

Jammo is a 3D character available for free at the Unity's Asset Store. I have changed it's controller and movement input code to convert the original Third Person View to a First Person View.

Networking is an empty gameobject that handles all networking aspects through its children' scripts. There are 2 prefabs called SocketIOController that comes from a paid assset in the Asset Store. It handles all Socket.io shenanigans that enables the Unity project to comunicate with the server. The other object inside Networking is a SocketManager that has the DataReceiver and DataSender scripts.

By default the SampleScene is configured to connect to the server at Heroku. Hit play to test the connection and you should see some debug message in the coonsole that verifys the connection with the server. If you with to test changes in the servers code, you will need to deactivate SocketIOController and activate SocketIOController(Local), and also drag it to the DataReceiver and Sender scripts. That will configure the SampleScene to connect to the local server instead of the Heroku one. (instructions on how to run the server at the server's repo Readme)

## Roadmap

### Node Server

- [x] Handle new connections
- [ ] Handle desconnections
- [x] Send and receive users' position info
- [ ] Send and receive users' audio stream
- [ ] Send and receive users' video stream
- [x] Host the server online

### Unity App

- [x] Make first person view movement and camera
- [x] Prototype 3D audio
- [x] Establish connection with server via Socket.io
- [x] Send and receive users' position info
- [ ] Send and receive users' audio stream
- [ ] Send and receive users' video stream
- [ ] Build to WebGL and host it online
- [ ] Add images to facilitate readme's understanding

## Known Issues

## Copyright

This project is copyright of [VoxarLabs](https://www.cin.ufpe.br/~voxarlabs/)

## Contributors

- Matheus Berger [@matheusberger](https://github.com/matheusberger)
- Lucas Figueiredo [@lsf](https://github.com/lsfcin)

## Links used in this project

- [StackOverflow: video/audio streaming with socket.io](https://stackoverflow.com/questions/49868353/send-video-audio-over-socket-io-from-browser)
- [NPM Package: socket.io streaming](https://www.npmjs.com/package/socket.io-stream)
- [Node.js: Stream API](https://nodejs.org/api/stream.html)
- [Github: JS Stream Handbook](https://github.com/substack/stream-handbook)