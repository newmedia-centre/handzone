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
import { validateRequest } from "@/server/db/auth-next"

// import components
import { StudentCalendar, StudentRequestDashboard } from "./student"
import {
    TeacherCalendar,
    TeacherRequestDashboard,
    TeacherCalendarBlockTool,
    TeacherRobotMonitoringDashboard,
} from "./teacher"

export default async function Page() {
    // get the user
    const { user } = await validateRequest()

    return (
        <main className="container mx-auto flex grow flex-col items-center gap-8 overflow-y-auto p-8">
            {user!.admin && (
                <div className="w-full rounded border border-300 bg-white shadow-md">
                    <TeacherRobotMonitoringDashboard />
                </div>
            )}
            <div className="flex w-full flex-col gap-8 lg:flex-row">
                <div className="aspect-square basis-full divide-y divide-300 rounded border border-300 bg-white shadow-md lg:basis-1/3">
                    {user!.admin ? <TeacherCalendar /> : <StudentCalendar />}
                </div>
                {user!.admin ? (
                    <div className="flex basis-full flex-col gap-8 lg:basis-2/3">
                        <div className="grow divide-y divide-300 rounded border border-300 bg-white shadow-md">
                            <TeacherRequestDashboard />
                        </div>
                        <div className="divide-y divide-300 rounded border border-300 bg-white shadow-md">
                            <TeacherCalendarBlockTool />
                        </div>
                    </div>
                ) : (
                    <div className="basis-full divide-y divide-300 rounded border border-300 bg-white shadow-md lg:basis-2/3">
                        <StudentRequestDashboard user={user!} />
                    </div>
                )}
            </div>
        </main>
    )
}
