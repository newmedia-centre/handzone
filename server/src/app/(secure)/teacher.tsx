/*
 * Copyright (c) 2024 NewMedia Centre - Delft University of Technology
 *
 * Licensed under the Apache License, Version 2.0 (the 'License');
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an 'AS IS' BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// import dependencies
import { prisma } from "@/server/db"
import { robots } from "@/server/robot"
import { namespaces } from "@/server/socket"
import env from "@/server/environment"

// import components
import NoSSR from "@/components/no-ssr"
import { NewSessionRequest } from "@/components/new-request"
import { JoinSessionRequest } from "@/components/join-request"
import { SessionRequestRow } from "@/components/request-row"
import { RobotMonitoringDashboard, VirtualRobotMonitoringDashboard } from "@/components/robot-monitoring"
import { Tab, TabGroup, TabList, TabPanel, TabPanels } from "@headlessui/react"

export const TeacherRobotMonitoringDashboard = async () => {
    const virtual = Array.from(robots.connections.values())
        .filter((robot) => !!robot.virtual)
        .map((robot) => ({ ...robot.info, status: robot.virtual }))

    return (
        <TabGroup className="divide-y divide-300">
            <div className="flex shrink-0 items-stretch justify-between">
                <h2 className="p-2 text-2xl leading-none">Robot Monitoring</h2>
                <TabList className="flex justify-end divide-x divide-300 border-l border-300">
                    {env.ROBOTS.map((robot) => (
                        <Tab
                            key={robot.name}
                            className="flex items-center gap-4 px-4 capitalize outline-none hover:bg-200"
                        >
                            <span>{robot.name}</span>
                            {robots.connections.has(robot.name) ? (
                                robots.connections.get(robot.name)!.active ? (
                                    <div className="size-2 rounded-full bg-green-500"></div>
                                ) : (
                                    <div className="size-2 rounded-full bg-yellow-500"></div>
                                )
                            ) : (
                                <div className="size-2 rounded-full bg-red-500"></div>
                            )}
                        </Tab>
                    ))}
                    <Tab className="px-4 outline-none hover:bg-200">{`Virtual Robots`}</Tab>
                </TabList>
            </div>
            <TabPanels className="grow">
                {env.ROBOTS.map((robot) => (
                    <TabPanel key={robot.name}>
                        <RobotMonitoringDashboard
                            robot={robot}
                            users={Array.from(namespaces.get(robot.name)?.nsp.sockets.values() ?? []).map((socket) => ({
                                ...socket.data.user,
                                paused: socket.data.paused,
                            }))}
                            status={robots.connections.has(robot.name)}
                            active={robots.connections.get(robot.name)?.active ?? false}
                            paused={robots.connections.get(robot.name)?.paused ?? false}
                        />
                    </TabPanel>
                ))}
                <TabPanel>
                    <VirtualRobotMonitoringDashboard robots={virtual} />
                </TabPanel>
            </TabPanels>
        </TabGroup>
    )
}

export const TeacherCalendar = async () => {
    return (
        <>
            <div className="flex shrink-0 items-center justify-between p-2">
                <h2 className="text-2xl leading-none">Calendar</h2>
                <div className="flex gap-2"></div>
            </div>
            <div className="grow p-2"></div>
        </>
    )
}

export const TeacherRequestDashboard = async () => {
    // get all the sessions requests
    const sessions = await prisma.robotSession.findMany({
        where: {
            end: {
                gte: new Date().toISOString(),
            },
        },
        include: {
            robot: true,
            requests: {
                include: {
                    user: true,
                },
            },
        },
    })

    // get all robots
    const robots = await prisma.robot.findMany({
        include: {
            sessions: {
                where: {
                    end: {
                        gte: new Date().toISOString(),
                    },
                },
            },
        },
    })

    return (
        <>
            <div className="flex shrink-0 items-stretch justify-between">
                <h2 className="p-2 text-2xl leading-none">Request Dashboard</h2>
                <div className="flex justify-end divide-x divide-300 border-l border-300">
                    <JoinSessionRequest robots={robots} />
                    <NewSessionRequest robots={robots} />
                </div>
            </div>
            <div className="flex grow flex-col">
                <NoSSR>
                    {sessions.map((session) => (
                        <SessionRequestRow key={session.id} session={session} />
                    ))}
                </NoSSR>
            </div>
        </>
    )
}

export const TeacherCalendarBlockTool = async () => {
    return (
        <>
            <div className="flex shrink-0 items-center justify-between p-2">
                <h2 className="text-2xl leading-none">Calendar Block Tool</h2>
                <div className="flex gap-2"></div>
            </div>
            <div className="grow p-2"></div>
        </>
    )
}
