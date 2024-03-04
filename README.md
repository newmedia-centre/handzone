# Running HandZone Guide

All software required is already installed and placed on the desktop. Instructions to run the experience is explained below.

## 1. Run URSim Docker Container
1. Launch `Docker Desktop` from desktop.
2. In the `Containers` tab press the `Start` button on ursim under the Actions.
3. When the container turns green. Click the following link and open it in your browser.
> http://127.0.0.1:6080/vnc.html?host=127.0.0.1&port=6080
4. Within this URL you can view the Polyscope interface.

## 2. Run HandZONE server Docker Container

1. Launch `Docker Desktop` from desktop.
2. In the `Containers` tab press the `Start` button on handzone-server under the Actions.
3. When the container turns green, click on it. Open the `Exec` tab.
4. Type the following commands in the Exec terminal:
```sh
cd app
npm run dev
```

This will start the web server on the docker container.

> Note: after entering `npm run dev` and pressing enter. The server should start and must mention something similar to `[ROBOT:172.17.0.2] Connected` this means the server is up and running.

> Note: If the connection is not found. You need to make sure to that the URSim Docker Container is running and has an IP address assigned to it. And make sure that the IP address is also inside the server configuration file. 

## 3. Establish Oculus Link

1. Launch `Oculus` from desktop.
2. Turn on Oculus headset and connect the Link Cable to the PC & headset.
3. When asked to `Allow access to data` in the headset, select `Allow`.
4. It will prompt to `Enable Oculus Link`, select `Enable`.

> Note: When `Enable Oculus Link` prompt is not shown. You can enable link through the Quick settings menu, which is found in the task bar below when wearing the headset.

5. After Oculus Link is enabled it will load a new gray environment. 

## 4. Launch HandZone Unity project

1. Launch `Unity Hub` from desktop.
2. Open `handzone-unity` to load the Unity project.
3. After the project is loaded press the Play button on top.

**Free Software, Hell Yeah!**
