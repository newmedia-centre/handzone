'use client'

// import dependencies
import { createContext, useContext } from 'react'

// import types
import type { User } from '@prisma/client'

// create context
const UserContext = createContext<User | undefined>(undefined)
export const useUser = () => {
	const context = useContext(UserContext)

	if (!context) {
		throw new Error('No UserProvider found when calling useUser.')
	}

	return context
}

// create uuser provider props type
export type UserProviderProps = {
	children?: React.ReactNode | React.ReactNode[]
	user: User
}

export const UserProvider = ({ children, user }: UserProviderProps) => (
	<UserContext.Provider value={user}>
		{children}
	</UserContext.Provider>
)