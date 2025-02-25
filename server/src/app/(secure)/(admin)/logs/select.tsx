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
import path from "path"

// import components
import { Listbox, ListboxButton, ListboxOption, ListboxOptions } from "@headlessui/react"

// logs select component
export const LogSelect = ({ options }: { options: string[] }) => {
    const router = useRouter()

    return (
        <Listbox value="" onChange={(value) => router.push(`/logs/${value}`)}>
            <ListboxButton className="inline-flex w-48 items-center justify-between rounded border px-4 py-2 text-center hover:bg-200">
                <span>Select Log File</span>
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
            <ListboxOptions anchor="bottom" className="z-30 mt-1 w-48 rounded border border-300 bg-white text-center">
                {options.map((option, index) => (
                    <ListboxOption
                        value={path.parse(option).name}
                        key={index}
                        className="cursor-pointer p-2 hover:bg-200"
                    >
                        {path.parse(option).name}
                    </ListboxOption>
                ))}
            </ListboxOptions>
        </Listbox>
    )
}
