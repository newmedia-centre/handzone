// import dependencies
import { prisma } from '@/server/db'

// import components
import { RequestSession } from '@/components/request'

// import types
import type { User } from '@prisma/client'

export const StudentCalendar = async () => {
	return (
		<>
			<div className='flex shrink-0 items-center justify-between p-2'>
				<h2 className='text-2xl leading-none'>Availability Calendar</h2>
				<div className='flex gap-2'>
				</div>
			</div>
			<div className='grow p-2'></div>
		</>
	)
}

export const StudentRequestDashboard = async ({ user }: { user: User }) => {
	// get the user's requests
	const requests = await prisma.robotSessionRequest.findMany({
		where: {
			userId: user.id,
		},
		include: {
			session: {
				include: {
					robot: true
				}
			}
		}
	})

	// get all robots
	const robots = await prisma.robot.findMany()

	return (
		<>
			<div className='flex shrink-0 items-stretch justify-between'>
				<h2 className='p-2 text-2xl leading-none'>My Requests</h2>
				<div className='flex justify-end gap-2 divide-x divide-300 border-l border-300'>
					<RequestSession robots={robots} />
				</div>
			</div>
			<div className='grow p-2'>
				{requests.map((request) => (
					<span key={request.sessionId}>{request.sessionId}</span>
				))}
			</div>
		</>
	)
}