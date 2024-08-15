// import dependencies
import { validateRequest } from '@/server/db/auth'

// import components
import { WelcomeUser } from './welcome'

export default async function Page() {
	// get the user
	const { user } = await validateRequest()

	// check if the user is admin
	const admin = user?.id === 'dc35f37334f3d9d881f1e3276295d37ca0944d64'

	return (
		<main className='container mx-auto flex grow flex-col items-center gap-8 overflow-y-auto p-8'>
			{admin && (
				<div className='w-full divide-y divide-300 rounded border border-300 bg-white shadow-md'>
					<TeacherRobotMonitoringDashboard />
				</div>
			)}
			<div className='flex w-full flex-col gap-8 lg:flex-row'>
				<div className='aspect-square basis-full divide-y divide-300 rounded border border-300 bg-white shadow-md lg:basis-1/3'>
					{admin ? (
						<TeacherCalendar />
					) : (
						<StudentCalendar />
					)}
				</div>
				{admin ? (
					<div className='flex basis-full flex-col gap-8 lg:basis-2/3'>
						<div className='grow divide-y divide-300 rounded border border-300 bg-white shadow-md'>
							<TeacherRequestDashboard />
						</div>
						<div className='divide-y divide-300 rounded border border-300 bg-white shadow-md'>
							<TeacherCalendarBlockTool />
						</div>
					</div>
				) : (
					<div className='basis-full divide-y divide-300 rounded border border-300 bg-white shadow-md lg:basis-2/3'>
						<StudentRequestDashboard />
					</div>
				)}
			</div>
			<div className='z-10 w-full max-w-5xl items-center justify-center font-mono text-sm lg:flex'>
				<div className='flex flex-col gap-2 p-2'>
					<WelcomeUser />
				</div>
			</div>
		</main>
	)
}

const StudentCalendar = () => {
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

const StudentRequestDashboard = () => {
	return (
		<>
			<div className='flex shrink-0 items-center justify-between p-2'>
				<h2 className='text-2xl leading-none'>My Requests</h2>
				<div className='flex gap-2'>
				</div>
			</div>
			<div className='grow p-2'></div>
		</>
	)
}

const TeacherRobotMonitoringDashboard = () => {
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

const TeacherCalendar = () => {
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

const TeacherRequestDashboard = () => {
	return (
		<>
			<div className='flex shrink-0 items-center justify-between p-2'>
				<h2 className='text-2xl leading-none'>Request Dashboard</h2>
				<div className='flex gap-2'>
				</div>
			</div>
			<div className='grow p-2'></div>
		</>
	)
}

const TeacherCalendarBlockTool = () => {
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