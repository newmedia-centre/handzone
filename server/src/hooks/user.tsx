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
import { createContext, useContext } from "react"

// import types
import type { User } from "@prisma/client"

// create context
const UserContext = createContext<User | undefined>(undefined)
export const useUser = () => {
    const context = useContext(UserContext)

    if (!context) {
        throw new Error("No UserProvider found when calling useUser.")
    }

    return context
}

// create uuser provider props type
export type UserProviderProps = {
    children?: React.ReactNode | React.ReactNode[]
    user: User
}

export const UserProvider = ({ children, user }: UserProviderProps) => (
    <UserContext.Provider value={user}>{children}</UserContext.Provider>
)
