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

// import components
import NoSSR from "@/components/no-ssr"
import { NewSessionRequest } from "@/components/new-request"
import { JoinSessionRequest } from "@/components/join-request"
import { RequestRow } from "@/components/request-row"

// import types
import type { User } from "@prisma/client"

export const StudentCalendar = async () => {
    return (
        <>
            <div className="flex shrink-0 items-center justify-between p-2">
                <h2 className="text-2xl leading-none">Availability Calendar</h2>
                <div className="flex gap-2"></div>
            </div>
            <div className="grow p-2"></div>
        </>
    )
}

export const StudentRequestDashboard = async ({ user }: { user: User }) => {
    // get the user's requests
    const requests = await prisma.robotSessionRequest.findMany({
        where: {
            userId: user.id,
            session: {
                end: {
                    gte: new Date().toISOString(),
                },
            },
        },
        include: {
            session: {
                include: {
                    robot: true,
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
                <h2 className="p-2 text-2xl leading-none">My Requests</h2>
                <div className="flex justify-end divide-x divide-300 border-l border-300">
                    <JoinSessionRequest robots={robots} />
                    <NewSessionRequest robots={robots} />
                </div>
            </div>
            <div className="flex grow flex-col">
                <NoSSR>
                    {requests.map((request) => (
                        <RequestRow key={request.session.id} request={request} />
                    ))}
                </NoSSR>
            </div>
        </>
    )
}
