'use client'

// import dependencies
import { useUser } from '@/hooks/user'

export const WelcomeUser = () => {
	// get the user
	const user = useUser()

	// return the welcome message
	return (
		<p className='fixed left-0 top-0 flex w-full justify-center border-b border-gray-300 bg-gradient-to-b from-zinc-200 pb-6 pt-8 backdrop-blur-2xl lg:static lg:w-auto lg:rounded-xl lg:border lg:bg-gray-200  lg:p-4 dark:border-neutral-800 dark:bg-zinc-800/30 dark:from-inherit lg:dark:bg-zinc-800/30'>
			Welcome {user.name}!
		</p>
	)
}