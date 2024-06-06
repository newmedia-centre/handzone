// import dependencies
import { prisma } from '@/server/db'

// import components
import NoSSR from '@/components/no-ssr'
import { NewSessionRequest } from '@/components/new-request'
import { JoinSessionRequest } from '@/components/join-request'
import { SessionRequestRow } from '@/components/request-row'

export const TeacherRobotMonitoringDashboard = async () => {
	return (
		<>
			<div className='flex shrink-0 items-center justify-between p-2'>
				<h2 className='text-2xl leading-none'>Robot Monitoring</h2>
				<div className='flex gap-2'>
				</div>
			</div>
			<div className='grow p-2'></div>
		</>
	)
}

export const TeacherCalendar = async () => {
	return (
		<>
			<div className='flex shrink-0 items-center justify-between p-2'>
				<h2 className='text-2xl leading-none'>Calendar</h2>
				<div className='flex gap-2'>
				</div>
			</div>
			<div className='grow p-2'></div>
		</>
	)
}

export const TeacherRequestDashboard = async () => {
	// get all the sessions requests
	const sessions = await prisma.robotSession.findMany({
		where: {
			end: {
				gte: new Date().toISOString()
			}
		},
		include: {
			robot: true,
			requests: {
				include: {
					user: true
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
				<h2 className='p-2 text-2xl leading-none'>Request Dashboard</h2>
				<div className='flex justify-end divide-x divide-300 border-l border-300'>
					<JoinSessionRequest robots={robots} />
					<NewSessionRequest robots={robots} />
				</div>
			</div>
			<div className='flex grow flex-col'>
				<NoSSR >
					{sessions.map((session) => (
						<SessionRequestRow key={session.id} session={session} />
					))}
				</NoSSR>
			</div>
		</>
	)
}

export const TeacherCalendarBlockTool = async () => {
	return (
		<>
			<div className='flex shrink-0 items-center justify-between p-2'>
				<h2 className='text-2xl leading-none'>Calendar Block Tool</h2>
				<div className='flex gap-2'>
				</div>
			</div>
			<div className='grow p-2'></div>
		</>
	)
}