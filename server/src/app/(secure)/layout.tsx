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
import { redirect } from "next/navigation"
import { validateRequest } from "@/server/db/auth-next"

// import components
import { UserProvider } from "@/hooks/user"

// export layout
export default async function Layout({
    children,
}: Readonly<{
    children: React.ReactNode
}>) {
    // check if the user is logged in
    const { user } = await validateRequest()

    // redirect to login if user is not available
    if (!user) {
        return redirect("/about")
    }

    // show the website after securing
    return <UserProvider user={user}>{children}</UserProvider>
}
