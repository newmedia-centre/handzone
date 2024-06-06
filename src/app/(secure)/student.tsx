// import dependencies
import { prisma } from '@/server/db'

// import components
import NoSSR from '@/components/no-ssr'
import { NewSessionRequest } from '@/components/new-request'
import { JoinSessionRequest } from '@/components/join-request'
import { RequestRow } from '@/components/request-row'

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
	const robots = await prisma.robot.findMany({
		include: {
			sessions: {
				where: {
					end: {
						gte: new Date().toISOString()
					}
				}
			}
		}
	})

	return (
		<>
			<div className='flex shrink-0 items-stretch justify-between'>
				<h2 className='p-2 text-2xl leading-none'>My Requests</h2>
				<div className='flex justify-end divide-x divide-300 border-l border-300'>
					<JoinSessionRequest robots={robots} />
					<NewSessionRequest robots={robots} />
				</div>
			</div>
			<div className='flex grow flex-col'>
				<NoSSR >
					{requests.map((request) => (
						<RequestRow key={request.session.id} request={request} />
					))}
				</NoSSR>
			</div>
		</>
	)
}