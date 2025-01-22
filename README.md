# HANDZONe Project

## Overview

The HANDZONe project aims to develop a VR-Supported Hybrid Learning Environment for Architectural Robotics. 
This application features a digital twin of a robotic arm, providing users with tutorials to enhance their learning experience in the field of architectural robotics.

## Features

- **Digital Twin of Robotic Arm**: A virtual representation of a Universal Robot CB3 robot that allows users to interact and learn about its functionalities, connecting to either a simulated or real robot managed by the [HANDZONe Server](https://github.com/newmedia-centre/handzone-server).
- **Interactive Tutorials**: Step-by-step guides that help users understand the operation and programming of the robotic arm.
- **Exercises**: Practical tasks designed to reinforce learning and provide hands-on experience with the robotic arm.
- **VR Support**: An immersive experience that allows users to engage with the content in a virtual reality environment.
- **Grasshopper integration**: Allows users to create a Grasshopper session to import meshes directly from Grasshopper to the 3D environment and the possibility to upload programs that have been created with Grasshopper [Robots plugin](https://github.com/visose/Robots). 

## Getting Started

To get started with the HANDZONe project, install the prerequisites below to set up the development environment.

1. Clone this Unity repository to your local machine.
2. Make sure to run the configured [HANDZONe Server](https://github.com/newmedia-centre/handzone-server) docker instance.
3. With the server running, you can load this Unity project and connect to the server to start using the application by configuring the Global Client component and changing the host and port to the address where the docker instance is running on.

### Prerequisites

- Unity Editor (2022.3 or later)
- [HANDZONe Server](https://github.com/newmedia-centre/handzone-server) to manage robots and devices to digital twin inside the VR application.
- [HANDZONe Grasshopper plugin](https://github.com/newmedia-centre/handzone-grasshopper) when connecting Grasshopper to Unity.
- Meta Quest 2 or 3. 

## Contributing

We welcome contributions to the HANDZONe project! If you would like to contribute, please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Make your changes and commit them.
4. Push your changes to your forked repository.
5. Submit a pull request.

## Contact

For any inquiries or feedback, please reach out to the project maintainers.