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
import { sync } from "glob"
import path from "path"
import env from "@/server/environment"

// import components
import { LogSelect } from "./select"

// export layout
export default async function Layout({
    children,
    params,
}: Readonly<{ children: React.ReactNode; params: { date: string } }>) {
    // get the log files
    const files = sync(path.resolve(env.LOGS_PATH, "**", "????-??-??*"))

    return (
        <main className="container mx-auto flex grow flex-col overflow-hidden p-8">
            <div className="flex flex-col divide-y divide-300 overflow-hidden rounded border border-300 bg-white shadow-md">
                <div className="flex shrink-0 items-center justify-between p-2">
                    <h2 className="text-2xl leading-none">LOGS: {params.date}</h2>
                    <div className="flex gap-2">
                        <a
                            href="/logs"
                            className="w-24 rounded border bg-white p-2 text-center text-red-500 hover:bg-red-200"
                        >
                            Errors
                        </a>
                        <LogSelect options={files} />
                    </div>
                </div>
                <div className="overflow-auto">{children}</div>
            </div>
        </main>
    )
}
