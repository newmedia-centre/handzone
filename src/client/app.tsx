import { useEffect, useMemo, useState } from 'react'
import type { Socket } from 'socket.io-client'
import { io } from 'socket.io-client'
import type { ServerToClientEvents, ClientToServerEvents } from '@/server/socket/interface'

function App() {
	const [robots, setRobots] = useState<string[]>([])
	const [robot, setRobot] = useState<Socket<ServerToClientEvents, ClientToServerEvents>>()

	// initialize socket.io connection
	const socket = useMemo(() => {
		return io() as Socket<ServerToClientEvents, ClientToServerEvents>
	}, [])

	useEffect(() => {
		socket.on('robots', robots => {
			console.log('robots', robots)
			setRobots(robots)
		})
	}, [socket])

	return (
		<div className='flex size-full items-center justify-center bg-slate-200'>
			<div className='flex flex-col items-center gap-2 rounded bg-white p-4'>
				<h1>Handzone Web Interface</h1>
				<div className='flex flex-col p-2'>
					{robots.map(robot => (
						<button key={robot} className='flex items-center gap-2 rounded bg-slate-200 p-2 hover:bg-slate-100' onClick={() => setRobot(io(`/${robot}`, { forceNew: true }))}>
							<span>{robot}</span>
						</button>
					))}
				</div>
				<button disabled={!robot} className='rounded bg-slate-200 p-2 hover:bg-slate-100 disabled:bg-white disabled:text-slate-400' onClick={() => robot?.emit('motion:movej', [0, 1.57, -1.57, 3.14, -1.57, 1.57], 1, 1, 0, 0)}>
					Send Command to Robot
				</button>
			</div>
		</div >
	)
}

export default App
