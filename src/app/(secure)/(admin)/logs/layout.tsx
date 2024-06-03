// import dependencies
import { sync } from 'glob'
import path from 'path'
import env from '@/server/environment'

// import components
import { LogSelect } from './select'

// export layout
export default async function Layout({ children, params }: Readonly<{ children: React.ReactNode, params: { date: string } }>) {
	// get the log files
	const files = sync(path.resolve(env.LOGS_PATH, '**', '????-??-??*'))

	return (
		<main className='flex grow flex-col p-8'>
			<div className='flex grow flex-col divide-y divide-300 rounded border border-300 bg-white'>
				<div className='flex items-center justify-between p-2'>
					<h2 className='text-2xl leading-none'>LOGS: {params.date}</h2>
					<div className='flex gap-2'>
						<a href='/logs' className='w-24 rounded border bg-white p-2 text-center text-red-500 hover:bg-red-200'>Errors</a>
						<LogSelect options={files} />
					</div>
				</div>
				<div className='grow'>
					{children}
				</div>
			</div>
		</main>
	)
}
