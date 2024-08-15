// import dependencies
import { validateRequest } from '@/server/db/auth'
import { validatePin } from '@/server/db/pin'

export const ValidateVRPin = async () => {

	const validate = async (formData: FormData) => {
		'use server'

		// get the user
		const { user } = await validateRequest()
		if (!user) return

		// get the pin
		const pin = formData.get('pin')

		// validate the pin
		await validatePin(pin as string, user)
		console.log('User', user)
	}


	return (
		<form action={validate} className='flex flex-col gap-2'>
			<input
				type='text'
				name='pin'
				className='rounded border border-300 p-2 hover:bg-50'
				placeholder='Enter your VR pin'
			/>
			<button className='rounded border border-300 bg-white p-2 hover:bg-200' type='submit'>Check</button>
		</form>
	)
}