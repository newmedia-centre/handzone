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
import { useEffect, useState } from "react"
import { useRouter } from "next/navigation"
import moment from "moment"

// import components
import {
    Dialog,
    DialogPanel,
    DialogTitle,
    Field,
    Label,
    Listbox,
    ListboxButton,
    ListboxOptions,
    ListboxOption,
} from "@headlessui/react"

// import types
import type { Robot, RobotSession } from "@prisma/client"

// connect to vr button
export const JoinSessionRequest = ({ robots }: { robots: (Robot & { sessions: RobotSession[] })[] }) => {
    const [open, setOpen] = useState(false)
    const [robot, setRobot] = useState<(Robot & { sessions: RobotSession[] }) | null>(null)
    const [session, setSession] = useState<RobotSession | null>(null)
    const router = useRouter()

    const create = async () => {
        if (!robot || !session) return

        const res = await fetch("/api/session", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                robot: robot.id,
                start: new Date(session.start).toISOString(),
                end: new Date(session.end).toISOString(),
                session: session.id,
            }),
        })

        if (res.ok) {
            setOpen(false)
            router.refresh()
        }
    }

    useEffect(() => {
        setSession(null)
    }, [robot])

    return (
        <>
            <button onClick={() => setOpen(true)} className="px-4 hover:bg-200">
                Join Session
            </button>
            <Dialog open={open} onClose={() => setOpen(false)} className="relative z-50">
                <div className="fixed inset-0 flex items-center justify-center bg-black/25 p-4">
                    <DialogPanel className="max-w-lg divide-y divide-300 rounded border border-300 bg-white shadow-md">
                        <DialogTitle className="flex shrink-0 items-center justify-between p-2 text-xl leading-none">
                            Join Session
                        </DialogTitle>
                        <div className="flex flex-col gap-2 p-2">
                            <Field className="flex items-center">
                                <Label className="w-24">Robot</Label>
                                <Listbox value={robot} onChange={setRobot}>
                                    <ListboxButton className="inline-flex w-56 items-center justify-between rounded border p-2 text-center hover:bg-200">
                                        <span>{robot?.name ?? "Select Robot"}</span>
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
                                        className="z-30 mt-1 w-56 rounded border border-300 bg-white text-center"
                                    >
                                        {robots.map((robot) => (
                                            <ListboxOption
                                                value={robot}
                                                key={robot.id}
                                                className="cursor-pointer p-2 hover:bg-200"
                                            >
                                                {robot.name}
                                            </ListboxOption>
                                        ))}
                                    </ListboxOptions>
                                </Listbox>
                            </Field>
                            <Field className="flex items-center">
                                <Label className="w-24">Session</Label>
                                <Listbox value={session} onChange={setSession}>
                                    <ListboxButton className="inline-flex w-56 items-center justify-between rounded border p-2 text-center hover:bg-200">
                                        {session ? (
                                            <div className="flex flex-col">
                                                <span>{moment(session.start).format("DD-MM-YYYY HH:mm")}</span>
                                                <span>{moment(session.end).format("DD-MM-YYYY HH:mm")}</span>
                                            </div>
                                        ) : (
                                            <span>Select Session</span>
                                        )}
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
                                        className="z-30 mt-1 w-56 rounded border border-300 bg-white text-center"
                                    >
                                        {robot?.sessions.map((session) => (
                                            <ListboxOption
                                                value={session}
                                                key={session.id}
                                                className="flex cursor-pointer flex-col p-2 hover:bg-200"
                                            >
                                                <span>{moment(session.start).format("DD-MM-YYYY HH:mm")}</span>
                                                <span>{moment(session.end).format("DD-MM-YYYY HH:mm")}</span>
                                            </ListboxOption>
                                        ))}
                                    </ListboxOptions>
                                </Listbox>
                            </Field>
                            <button className="rounded border bg-white p-2 text-center hover:bg-200" onClick={create}>
                                Create Request
                            </button>
                        </div>
                    </DialogPanel>
                </div>
            </Dialog>
        </>
    )
}
