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
import moment from "moment"

// import components
import { Listbox, ListboxButton, ListboxOptions, ListboxOption } from "@headlessui/react"

// import types
import type { Robot, RobotSession, RobotSessionRequest, User, RequestStatus } from "@prisma/client"

export const RequestRow = ({
    request,
}: {
    request: RobotSessionRequest & { session: RobotSession & { robot: Robot } }
}) => {
    const router = useRouter()

    const remove = async () => {
        const res = await fetch(`/api/session/${request.session.id}/status`, {
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
        <div className="flex items-center justify-between gap-2 border-b border-300 p-2">
            <span>{request.session.robot.name}</span>
            <span>Start: {moment(request.session.start).format("DD-MM-YYYY HH:mm")}</span>
            <span>End: {moment(request.session.end).format("DD-MM-YYYY HH:mm")}</span>
            <span>{request.status}</span>
            <button
                onClick={remove}
                className="w-36 rounded border bg-white p-2 text-center enabled:hover:bg-200 disabled:cursor-default disabled:text-400"
            >
                Delete
            </button>
        </div>
    )
}

export const SessionRequestRow = ({
    session,
}: {
    session: RobotSession & { robot: Robot; requests: (RobotSessionRequest & { user: User })[] }
}) => {
    const router = useRouter()

    const remove = async () => {
        const res = await fetch(`/api/session/${session.id}`, {
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
            <div className="flex items-center justify-between gap-2 p-2">
                <span>{session.robot.name}</span>
                <span>Start: {moment(session.start).format("DD-MM-YYYY HH:mm")}</span>
                <span>End: {moment(session.end).format("DD-MM-YYYY HH:mm")}</span>
                <button onClick={remove} className="w-36 rounded border bg-white p-2 text-center hover:bg-200">
                    Cancel Session
                </button>
            </div>
            <div className="grid grid-cols-1 divide-x divide-100 lg:grid-cols-2">
                {session.requests.map((request) => (
                    <SessionRequestRowUser key={request.userId} request={request} />
                ))}
            </div>
        </div>
    )
}

export const SessionRequestRowUser = ({ request }: { request: RobotSessionRequest & { user: User } }) => {
    const router = useRouter()

    const update = async (status: RequestStatus) => {
        const res = await fetch(`/api/session/${request.sessionId}/status`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                user: request.user.id,
                status,
            }),
        })

        if (res.ok) {
            router.refresh()
        }
    }

    return (
        <div className="flex items-center justify-between gap-2 p-2">
            <a href={`mailto:${request.user.email}`}>{request.user.name}</a>
            <Listbox value={request.status} onChange={update}>
                <ListboxButton className="inline-flex w-36 items-center justify-between rounded border p-2 text-center enabled:hover:bg-200 disabled:cursor-default disabled:text-400">
                    <span className="capitalize">{request.status.toLowerCase()}</span>
                    <svg
                        className="ms-3 size-2.5"
                        aria-hidden="true"
                        xmlns="http://www.w3.org/2000/svg"
                        fill="none"
                        viewBox="0 0 10 6"
                    >
                        <path
                            stroke="currentColor"
                            strokeLinecap="round"
                            strokeLinejoin="round"
                            strokeWidth="2"
                            d="m1 1 4 4 4-4"
                        />
                    </svg>
                </ListboxButton>
                <ListboxOptions
                    anchor="bottom"
                    className="z-30 mt-1 w-36 rounded border border-300 bg-white text-center"
                >
                    <ListboxOption value="ACCEPTED" className="cursor-pointer p-2 hover:bg-200">
                        Accepted
                    </ListboxOption>
                    <ListboxOption value="REJECTED" className="cursor-pointer p-2 hover:bg-200">
                        Rejected
                    </ListboxOption>
                </ListboxOptions>
            </Listbox>
        </div>
    )
}
