// import dependencies
import { validateRequest } from '@/server/db/auth'

// import components
import { StudentCalendar, StudentRequestDashboard } from './student'
import { TeacherCalendar, TeacherRequestDashboard, TeacherCalendarBlockTool, TeacherRobotMonitoringDashboard } from './teacher'

export default async function Page() {
	// get the user
	const { user } = await validateRequest()

	return (
		<main className='container mx-auto flex grow flex-col items-center gap-8 overflow-y-auto p-8'>
			{user!.admin && (
				<div className='w-full divide-y divide-300 rounded border border-300 bg-white shadow-md'>
					<TeacherRobotMonitoringDashboard />
				</div>
			)}
			<div className='flex w-full flex-col gap-8 lg:flex-row'>
				<div className='aspect-square basis-full divide-y divide-300 rounded border border-300 bg-white shadow-md lg:basis-1/3'>
					{user!.admin ? (
						<TeacherCalendar />
					) : (
						<StudentCalendar />
					)}
				</div>
				{user!.admin ? (
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
						<StudentRequestDashboard user={user!} />
					</div>
				)}
			</div>
		</main>
	)
}