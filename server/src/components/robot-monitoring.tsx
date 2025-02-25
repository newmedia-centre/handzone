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

"use client"

// import dependencies
import { useRouter } from "next/navigation"

// import types
import type { RobotInfo } from "@/server/robot/connection"
import type { SessionType } from "@/types/Socket/Index"
import type { User } from "@prisma/client"

export const RobotMonitoringDashboard = ({
    robot,
    users,
    status,
    active,
    paused,
}: {
    robot: RobotInfo
    users: (User & { paused: boolean })[]
    status?: boolean
    active: boolean
    paused: boolean
}) => {
    const router = useRouter()

    // filter users
    const uniqueUsers = users.filter((value, index, array) => array.findIndex((x) => x.id === value.id) === index)

    const activate = async () => {
        const res = await fetch(`/api/robot/${robot.name}/active`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                active: !active,
            }),
        })

        if (res.ok) {
            router.refresh()
        }
    }

    const pause = async () => {
        const res = await fetch(`/api/robot/${robot.name}/pause`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                paused: !paused,
            }),
        })

        if (res.ok) {
            router.refresh()
        }
    }

    if (!status)
        return <div className="flex items-center justify-center bg-50 p-4">Could not connect to {robot.name}</div>

    return (
        <div className="flex flex-col divide-y divide-100 border-b border-300">
            <div className="flex items-center justify-between gap-2 p-2">
                <span className="p-2 capitalize">{robot.name}</span>
                <div className="flex justify-end gap-2">
                    <button
                        onClick={pause}
                        disabled={!active}
                        className="w-36 rounded border bg-white p-2 text-center enabled:hover:bg-200 disabled:text-400"
                    >
                        {paused ? "Resume" : "Pause"}
                    </button>
                    <button onClick={activate} className="w-36 rounded border bg-white p-2 text-center hover:bg-200">
                        {active ? "Close" : "Activate"}
                    </button>
                </div>
            </div>
            <div className="grid grid-cols-1 divide-x divide-100 lg:grid-cols-2">
                {uniqueUsers.map((user) => (
                    <RobotMonitoringUser key={user.id} robot={robot} user={user} />
                ))}
            </div>
        </div>
    )
}

const RobotMonitoringUser = ({ robot, user }: { robot: RobotInfo; user: User & { paused: boolean } }) => {
    const router = useRouter()

    const pause = async () => {
        const res = await fetch(`/api/robot/${robot.name}/user`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                id: user.id,
                paused: !user.paused,
            }),
        })

        if (res.ok) {
            router.refresh()
        }
    }

    const kick = async () => {
        const res = await fetch(`/api/robot/${robot.name}/user`, {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                id: user.id,
            }),
        })

        if (res.ok) {
            router.refresh()
        }
    }

    return (
        <div className="flex items-center justify-between gap-2 p-2">
            <a href={`mailto:${user.email}`}>{user.name}</a>
            <div className="flex justify-end gap-2">
                <button onClick={pause} className="w-36 rounded border bg-white p-2 text-center hover:bg-200">
                    {user.paused ? "Resume" : "Pause"}
                </button>
                <button onClick={kick} className="w-36 rounded border bg-white p-2 text-center hover:bg-200">
                    Kick
                </button>
            </div>
        </div>
    )
}

export const VirtualRobotMonitoringDashboard = ({
    robots,
}: {
    robots: (RobotInfo & { status: SessionType | null })[]
}) => {
    const router = useRouter()

    const kill = async (robot: RobotInfo) => {
        const res = await fetch(`/api/robot/${robot.name}`, {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json",
            },
        })

        if (res.ok) {
            router.refresh()
        }
    }

    return (
        <div className="flex flex-col divide-y divide-100 border-b border-300">
            {robots.map((robot) => (
                <div key={robot.name} className="flex items-center justify-between gap-2 p-2">
                    <span className="p-2 capitalize">{robot.name}</span>
                    <div className="flex justify-end gap-2">
                        <button
                            onClick={() => kill(robot)}
                            className="w-36 rounded border bg-white p-2 text-center hover:bg-200"
                        >
                            Shut Down
                        </button>
                    </div>
                </div>
            ))}
        </div>
    )
}
