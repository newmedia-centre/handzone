// import dependencies
import { validateRequest } from '@/server/db/auth'
import { generatePin } from '@/server/db/pin'

export const ConnectVRPin = async () => {
	// get the pin for the user
	const { user } = await validateRequest()
	if (!user) throw new Error('User not authenticated')

	const pin = await generatePin(user)

	// return the welcome message
	return (
		<p className='fixed left-0 top-0 flex w-full justify-center border-b border-gray-300 bg-gradient-to-b from-zinc-200 pb-6 pt-8 backdrop-blur-2xl lg:static lg:w-auto lg:rounded-xl lg:border lg:bg-gray-200  lg:p-4 dark:border-neutral-800 dark:bg-zinc-800/30 dark:from-inherit lg:dark:bg-zinc-800/30'>
			{pin}
		</p>
	)
}