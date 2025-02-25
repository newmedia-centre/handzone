#!/bin/bash

LOG_OUTPUT="NONE"
if [[ ! $1 == "" ]]; then
	LOG_OUTPUT=$1
fi

file=/tmp/.X1-lock
if test -f $file; then
  rm /tmp/.X1-lock
fi

# Setup VNC server
Xvfb :1 -screen 0 800x600x24 &
x11vnc -bg -quiet -forever -shared -display :1 -snapfb >/dev/null 2>/dev/null

# Copy urcaps into bundle, to be installed properly, when the simulator is started
cp -r /urcaps/*.jar /ursim/GUI/bundle/ 2>/dev/null

# find path to daemon run file
runsvdir=$(find /etc/service/ -name "runsvdir*")
run_file="$runsvdir/run"

# Correct path in run file and make executable
mkdir -p /home/root/service
sed -i 's|/ursim/service|/home/root/service|g' $run_file
chmod +x $run_file

# Run daemon service
runsv $runsvdir/ &
rm -r /ursim/service

# Create webserver interface for vnc
sed -i 's/$(hostname)/localhost/g' /usr/share/novnc/utils/novnc_proxy
/usr/share/novnc/utils/novnc_proxy --vnc localhost:5900 >/dev/null 2>/dev/null &

# Get container ip address
docker_ip=$(hostname -i)

# User instructions
echo -e "Universal Robots simulator for CB3:${VERSION}\n\n"

echo -e "IP address of the simulator\n"
echo -e "     $docker_ip\n\n"

echo -e "Access the robots user interface through this URL:\n"
echo -e "     http://$docker_ip:6080/vnc.html?host=$docker_ip&port=6080\n\n"

echo -e "Access the robots user interface with a VNC application on this address:\n"
echo -e "     $docker_ip:5900\n\n"

echo -e "You can find documentation on how to use this container on dockerhub:\n"
echo -e "     https://hub.docker.com/r/universalrobots/ursim_cb3\n\n"

echo -e "Press Crtl-C to exit\n\n"

polyscope_file=/ursim/polyscope.log
urcontrol_file=/ursim/URControl.log

# Execute URSim
if [ ${LOG_OUTPUT} == "polyscope_log" ]; then
  /ursim/start-ursim.sh ${ROBOT_MODEL} >${polyscope_file} 2>${polyscope_file} &
  tail -f -n10 ${polyscope_file}
elif [ ${LOG_OUTPUT} == "control_log" ]; then
  /ursim/start-ursim.sh ${ROBOT_MODEL} >${polyscope_file} 2>${polyscope_file} &
  while ! tail -f ${urcontrol_file} 2>/dev/null; do sleep 1 ; done
  tail -f -n10 ${urcontrol_file}
else
  /ursim/start-ursim.sh ${ROBOT_MODEL} >${polyscope_file} 2>${polyscope_file}
fi

echo -e "\n"
