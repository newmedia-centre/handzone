// import dependencies
import { validatePin } from '@/server/db/pin'

export const ValidateVRPin = async () => {

	const validate = async (formData: FormData) => {
		'use server'

		// get the pin
		const pin = formData.get('pin')

		// validate the pin
		const user = await validatePin(pin as string)
		console.log('User', user)
	}


	return (
		<form action={validate} className='fixed left-0 top-0 flex w-full justify-center gap-4 border-b border-gray-300 bg-gradient-to-b from-zinc-200 pb-6 pt-8 backdrop-blur-2xl lg:static lg:w-auto lg:rounded-xl lg:border lg:bg-gray-200  lg:p-4 dark:border-neutral-800 dark:bg-zinc-800/30 dark:from-inherit lg:dark:bg-zinc-800/30'>
			<input
				type='text'
				name='pin'
				className='rounded-lg border border-gray-300 p-2'
				placeholder='Enter your VR pin'
			/>
			<button className='rounded-lg border border-gray-300 bg-white p-2 hover:bg-gray-100' type='submit'>Check</button>
		</form>
	)
}