# HANDZONe Project

## Overview
HANDZONe server is a web application designed to control and monitor robots for a virtual reality environment. It provides a user-friendly interface for managing and requesting robot sessions for both simulated and real robots, monitoring their status, and interacting with them in real-time.

## Features
- **Robot Control**: Start, stop, and manage robot sessions for simulated and real robots.
- **Real-time Monitoring**: View the status and logs of robots in real-time.
- **User Authentication**: Secure access to the application with user roles (admin and regular users).
- **Virtual Reality Integration**: Control robots in a VR environment.
- **Grasshopper Integration**: Run programs inside Grasshopper with [https://github.com/visose/Robots](Robots) plugin. Uploading meshes from Grasshopper to the Virtual Reality session.
- **Logging**: Keep track of robot activities and errors through detailed logs.

## Prerequisites
Before you begin, ensure you have met the following requirements:
- **[Docker](https://www.docker.com/)**: For containerized deployment.

## Getting Started

### 1. Clone the Repository
Clone the project repository to your local machine:
```bash
git clone https://github.com/newmedia-centre/handzone.git
cd server
```

### 2. Set Up Environment Variables
Create a `.env` file from the root directory and configure the necessary environment variables. You can use the provided `.env_template` as a reference.

### 3. Start the HANDZONe server
Use Docker Compose to start the application in development mode. Run the following command in the project directory:
```bash
# Run the Docker server and database in the background
docker-compose -f docker-compose-dev.yml up -d
```

### 4. Start the Docker Proxy (Optional)
> This step is optional and needed whenever you want to run the server and the Unity application on same machine.

To set up the Docker socket proxy, run:
```bash
# Run the Docker socket proxy in the background
docker-compose -f docker-compose-proxy.yml up -d
```

### 5. Access the Application
Once the containers are running, you can access the application at `http://localhost:3000`.

## Build for Production

### 1. Building the application
To build the application for production, run:
```bash
npm run build
```

### 2. Start the Production Server
After building, you can start the production server with:
```bash
npm start
```

## Contributing
We welcome contributions to the HANDZONe project! If you would like to contribute, please follow these steps.
1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Make your changes and commit them.
4. Push your changes to your forked repository.
5. Submit a pull request.

## Contact
For any inquiries or feedback, please reach out to the project maintainers.