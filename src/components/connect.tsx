'use client'

// import dependencies
import { useState } from 'react'

// connect to vr button
export const ConnectVR = () => {
	const [pin, setPin] = useState<string>()

	// get a pin from the server
	const generatePin = async () => {
		const response = await fetch('/api/auth/pin')
		const data = await response.text()
		setPin(data)
	}

	return (
		<div>
			<button onClick={generatePin} className='rounded border bg-stone-50 p-2 hover:bg-stone-100'>Connect VR</button>
			<dialog open={!!pin} className='fixed inset-0 z-50 size-full items-center justify-center bg-black/10 open:flex'>
				<div className='flex flex-col items-center gap-2 rounded border border-gray-300 bg-white p-4'>
					<p>{pin}</p>
					<form method='dialog'>
						<button autoFocus onClick={() => setPin(undefined)} className='rounded border bg-stone-50 p-2 hover:bg-stone-100'>OK</button>
					</form>
				</div>
			</dialog>
		</div>
	)
}