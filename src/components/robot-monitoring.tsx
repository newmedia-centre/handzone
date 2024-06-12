'use client'

// import dependencies
import { useRouter } from 'next/navigation'

// import types
import type { RobotInfo } from '@/server/robot/connection'
import type { SessionType } from '@/types/Socket/Index'

export const RobotMonitoringDashboard = ({ robot, status, active, paused }: { robot: RobotInfo, status?: boolean, active: boolean, paused: boolean }) => {
	const router = useRouter()

	const activate = async () => {
		const res = await fetch(`/api/robot/${robot.name}/active`, {
			method: 'PUT',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify({
				active: !active
			})
		})

		if (res.ok) {
			router.refresh()
		}
	}

	const pause = async () => {
		const res = await fetch(`/api/robot/${robot.name}/active`, {
			method: 'PUT',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify({
				paused: !paused
			})
		})

		if (res.ok) {
			router.refresh()
		}
	}

	if (!status) return (
		<div className='flex items-center justify-center bg-50 p-4'>
			Could not connect to {robot.name}
		</div>
	)

	return (
		<div className='flex flex-col divide-y divide-100 border-b border-300'>
			<div className='flex items-center justify-between gap-2 p-2'>
				<span className='p-2 capitalize'>{robot.name}</span>
				<div className='flex justify-end gap-2'>
					<button onClick={pause} disabled={!active} className='w-36 rounded border bg-white p-2 text-center enabled:hover:bg-200 disabled:text-400'>Pause</button>
					<button onClick={activate} className='w-36 rounded border bg-white p-2 text-center hover:bg-200'>{active ? 'Kill' : 'Activate'}</button>
				</div>
			</div>
			<div className='grid grid-cols-1 divide-x divide-100 p-2 lg:grid-cols-2'>
				Users
			</div>
		</div>
	)
}

export const VirtualRobotMonitoringDashboard = ({ robots }: { robots: (RobotInfo & { status: SessionType | null })[] }) => {
	const router = useRouter()

	const kill = async (robot: RobotInfo) => {
		const res = await fetch(`/api/robot/${robot.name}/active`, {
			method: 'DELETE',
			headers: {
				'Content-Type': 'application/json'
			}
		})

		if (res.ok) {
			router.refresh()
		}
	}

	return (
		<div className='flex flex-col divide-y divide-100 border-b border-300'>
			{robots.map(robot => (
				<div key={robot.name} className='flex items-center justify-between gap-2 p-2'>
					<span className='p-2 capitalize'>{robot.name}</span>
					<div className='flex justify-end gap-2'>
						<button onClick={() => kill(robot)} className='w-36 rounded border bg-white p-2 text-center hover:bg-200'>Shut Down</button>
					</div>
				</div>
			))}
		</div>
	)
}