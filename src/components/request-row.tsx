'use client'

// import dependencies
import { useState } from 'react'
import { useRouter } from 'next/navigation'
import moment from 'moment'

// import components
import {
	Listbox,
	ListboxButton,
	ListboxOptions,
	ListboxOption,
} from '@headlessui/react'

// import types
import type { Robot, RobotSession, RobotSessionRequest, User, RequestStatus } from '@prisma/client'

export const SessionRequestRow = ({ session }: { session: (RobotSession & { robot: Robot, requests: (RobotSessionRequest & { user: User })[] }) }) => {
	const router = useRouter()

	return (
		<div className='flex flex-col divide-y divide-100 border-b border-300'>
			<div className='flex justify-between gap-2 p-2'>
				<span>{session.robot.name}</span>
				<span>Start: {moment(session.start).format('DD-MM-YYYY HH:mm')}</span>
				<span>End: {moment(session.end).format('DD-MM-YYYY HH:mm')}</span>
				<button>Cancel Session</button>
			</div>
			<div className='grid grid-cols-1 divide-x divide-100 lg:grid-cols-2'>
				{session.requests.map((request => (
					<SessionRequestRowUser key={request.userId} request={request} />
				)))}
			</div>
		</div>
	)
}

export const SessionRequestRowUser = ({ request }: { request: RobotSessionRequest & { user: User } }) => {
	const router = useRouter()

	const update = async (status: RequestStatus) => {
		const res = await fetch(`/api/session/${request.sessionId}/status`, {
			method: 'PUT',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify({
				user: request.user.id,
				status
			})
		})

		if (res.ok) {
			router.refresh()
		}
	}

	return (
		<div className='flex items-center justify-between gap-2 p-2'>
			<a href={`mailto:${request.user.email}`}>{request.user.name}</a>
			<Listbox value={request.status} onChange={update}>
				<ListboxButton className='inline-flex w-36 items-center justify-between rounded border p-2 text-center hover:bg-200'>
					<span className='capitalize'>{request.status.toLowerCase()}</span>
					<svg className='ms-3 size-2.5' aria-hidden='true' xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 10 6'>
						<path stroke='currentColor' strokeLinecap='round' strokeLinejoin='round' strokeWidth='2' d='m1 1 4 4 4-4' />
					</svg>
				</ListboxButton>
				<ListboxOptions anchor='bottom' className='z-30 mt-1 w-36 rounded border border-300 bg-white text-center'>
					<ListboxOption value='REQUESTED' className='p-2 text-400' disabled>Requested</ListboxOption>
					<ListboxOption value='ACCEPTED' className='cursor-pointer p-2 hover:bg-200'>Accepted</ListboxOption>
					<ListboxOption value='REJECTED' className='cursor-pointer p-2 hover:bg-200'>Rejected</ListboxOption>
					<ListboxOption value='AVAILABLE' className='cursor-pointer p-2 hover:bg-200'>Available</ListboxOption>
				</ListboxOptions>
			</Listbox>
		</div>
	)
}