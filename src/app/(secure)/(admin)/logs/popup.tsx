'use client'

// import dependencies
import { useState } from 'react'

// logs info popup
export const InfoPopup = ({ data }: { data: string }) => {
	const [open, setOpen] = useState(false)

	return (
		<div>
			<button onClick={() => setOpen(true)} className='size-full'>Show</button>
			<dialog open={open} onClick={() => setOpen(false)} className='fixed inset-0 z-50 size-full items-center justify-center bg-black/10 open:flex'>
				<div onClick={e => e.stopPropagation()} className='gap-2 rounded border border-gray-300 bg-white p-4'>
					<pre className='text-left'>{JSON.stringify(JSON.parse(data), null, 2)}</pre>
				</div>
			</dialog>
		</div>
	)
}