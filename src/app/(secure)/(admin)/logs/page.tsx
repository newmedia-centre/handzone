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
import fs from "fs"
import path from "path"
import env from "@/server/environment"

// import components
import { LogTable } from "./table"

// import types
import type { LogData } from "./table"

export default function Page() {
    // read the logs
    const errors = fs
        .readFileSync(path.resolve(env.LOGS_PATH, "errors.log"), "utf-8")
        .split("\n")
        .filter((x) => x.length > 0)
        .map((x) => ({ ...JSON.parse(x), raw: x } as LogData))

    return <LogTable data={errors} />
}
