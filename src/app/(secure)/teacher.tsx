// import dependencies
import moment from 'moment'
import { prisma } from '@/server/db'

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
				gte: new Date()
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

	return (
		<>
			<div className='flex shrink-0 items-center justify-between p-2'>
				<h2 className='text-2xl leading-none'>Request Dashboard</h2>
				<div className='flex gap-2'>
				</div>
			</div>
			<div className='flex grow flex-col'>
				{sessions.map((session) => (
					<div key={session.id} className='flex gap-2 p-2'>
						<span>{session.robot.name}</span>
						<span>{moment(session.start).format('DD-MM-YYYY HH:mm')}</span>
						<span>{moment(session.end).format('DD-MM-YYYY HH:mm')}</span>
						<div className='flex gap-2'>
							{session.requests.map((request => (
								<div key={request.userId} className='flex gap-2'>
									<span>{request.user.name}</span>
									<span>{request.status}</span>
								</div>
							)))}
						</div>
					</div>
				))}
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