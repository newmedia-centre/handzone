'use client'

// import dependencies
import { useState } from 'react'

// connect to vr button
export const ConnectVR = () => {
	const [open, setOpen] = useState(false)
	const [pin, setPin] = useState('')

	// validate a pin as the logged in user
	const validate = async () => {
		await fetch('/api/auth/pin', {
			method: 'PUT',
			body: JSON.stringify({ pin }),
		})
		setPin('')
		setOpen(false)
	}

	return (
		<div>
			<button onClick={() => setOpen(true)} className='w-24 rounded border bg-white p-2 text-center hover:bg-200'>PIN</button>
			<dialog open={open} className='fixed inset-0 z-50 size-full items-center justify-center bg-black/10 open:flex'>
				<form action={validate} className='flex flex-col items-center gap-2 rounded border border-300 bg-white p-4'>
					<input
						type='text'
						name='pin'
						value={pin}
						onChange={e => setPin(e.target.value)}
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