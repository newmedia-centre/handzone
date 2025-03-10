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
import { useState } from "react"
import { useRouter } from "next/navigation"

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
    Input,
} from "@headlessui/react"

// import types
import type { Robot } from "@prisma/client"

// connect to vr button
export const NewSessionRequest = ({ robots }: { robots: Robot[] }) => {
    const [open, setOpen] = useState(false)
    const [robot, setRobot] = useState<Robot | null>(null)
    const [start, setStart] = useState("")
    const [end, setEnd] = useState("")
    const router = useRouter()

    const create = async () => {
        if (!robot || !start || !end) return

        const res = await fetch("/api/session", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                robot: robot.id,
                start: new Date(start).toISOString(),
                end: new Date(end).toISOString(),
            }),
        })

        if (res.ok) {
            setOpen(false)
            router.refresh()
        }
    }

    return (
        <>
            <button onClick={() => setOpen(true)} className="px-4 hover:bg-200">
                New Session
            </button>
            <Dialog open={open} onClose={() => setOpen(false)} className="relative z-50">
                <div className="fixed inset-0 flex items-center justify-center bg-black/25 p-4">
                    <DialogPanel className="max-w-lg divide-y divide-300 rounded border border-300 bg-white shadow-md">
                        <DialogTitle className="flex shrink-0 items-center justify-between p-2 text-xl leading-none">
                            New Session
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
                                <Label className="w-24">Start Time</Label>
                                <Input
                                    value={start}
                                    onChange={(e) => setStart(e.target.value)}
                                    type="datetime-local"
                                    className="w-56 rounded border p-2 outline-none hover:bg-50"
                                />
                            </Field>
                            <Field className="flex items-center">
                                <Label className="w-24">End Time</Label>
                                <Input
                                    value={end}
                                    onChange={(e) => setEnd(e.target.value)}
                                    type="datetime-local"
                                    className="w-56 rounded border p-2 outline-none hover:bg-50"
                                />
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
