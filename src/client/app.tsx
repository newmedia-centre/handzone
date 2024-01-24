import { useEffect, useMemo, useState } from 'react'
import type { Socket } from 'socket.io-client'
import { io } from 'socket.io-client'
import type { ServerToClientEvents, ClientToServerEvents } from '@/server/socket/interfaces'

function App() {
	const [robots, setRobots] = useState<string[]>([])
	const [robot, setRobot] = useState<Socket<ServerToClientEvents, ClientToServerEvents>>()

	// initialize socket.io connection
	const socket = useMemo(() => {
		return io() as Socket<ServerToClientEvents, ClientToServerEvents>
	}, [])

	useEffect(() => {
		socket.on('write', robots => {
			setRobots(robots)
		})
	}, [])

	return (
		<div className='flex size-full items-center justify-center bg-slate-200'>
			<div className='flex flex-col items-center gap-2 rounded bg-white p-4'>
				<h1>Handzone Web Interface</h1>
				<div className='flex flex-col p-2'>
					{robots.map(robot => (
						<button key={robot} className='rounded bg-slate-200 p-1 hover:bg-slate-100' onClick={() => setRobot(io(`/${robot}`, { forceNew: true }))}>
							{robot}
						</button>
					))}
				</div>
				<button disabled={!robot} className='rounded bg-slate-200 p-2 hover:bg-slate-100 disabled:bg-none' onClick={() => robot?.emit('motion:movej', [0, 0, 0, 0, 0, 0], 1, 1, 1, 1)}>
					Send Command to Robot
				</button>
			</div>
		</div >
	)
}

export default App
