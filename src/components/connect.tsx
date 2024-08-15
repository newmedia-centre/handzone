'use client'

// import dependencies
import { useState } from 'react'
import { validateRequest } from '@/server/db/auth'
import { validatePin } from '@/server/db/pin'

// connect to vr button
export const ConnectVR = () => {
	const [open, setOpen] = useState<boolean>()

	// validate a pin as the logged in user
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
		<div>
			<button onClick={() => setOpen(true)} className='w-24 rounded border bg-white p-2 text-center hover:bg-200'>Validate PIN</button>
			<dialog open={open} className='fixed inset-0 z-50 size-full items-center justify-center bg-black/10 open:flex'>
				<form action={validate} method='dialog' className='flex flex-col items-center gap-2 rounded border border-300 bg-white p-4'>
					<input
						type='text'
						name='pin'
						className='rounded border border-300 p-2 hover:bg-50'
						placeholder='Enter the given pin'
					/>
					<div className='flex gap-2'>
						<button autoFocus onClick={() => setOpen(false)} className='rounded border border-300 bg-stone-50 p-2 hover:bg-stone-100'>Cancel</button>
						<button className='rounded border border-300 bg-stone-50 p-2 hover:bg-stone-100' type='submit'>Validate</button>
					</div>
				</form>
			</dialog>
		</div>
	)
}