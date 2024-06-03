// import dependencies
import fs from 'fs'
import path from 'path'
import env from '@/server/environment'
import moment from 'moment'

// import components
import { InfoPopup } from '../popup'

export default function Page({ params }: { params: { date: string } }) {
	// read the logs
	const logs = fs.readFileSync(path.resolve(env.LOGS_PATH, `${params.date}.log`), 'utf-8').split('\n').filter(x => x.length > 0)

	const getLevelColor = (level: string) => {
		switch (level) {
			case 'error':
				return 'text-red-500'
			case 'warn':
				return 'text-yellow-500'
			case 'http':
				return 'text-500'
			default:
				return ''
		}
	}

	return (
		<table className='table w-full grow table-fixed'>
			<thead>
				<tr className='border-b border-300'>
					<th className='w-24 p-2 text-left'>Level</th>
					<th className='w-72 p-2 text-left'>Label</th>
					<th className='w-36 p-2 text-left'>Category</th>
					<th className='w-36 p-2 text-left'>Entity</th>
					<th className='p-2 text-left'>Message</th>
					<th className='w-56 p-2 text-left'>Timestamp</th>
					<th className='w-48 p-2 text-center'>More Info</th>
				</tr>
			</thead>
			<tbody>
				{logs.map((string, index) => {
					const content = JSON.parse(string)
					return (
						<tr key={index} className={`border-b border-300 hover:bg-50 ${getLevelColor(content.level)}`}>
							<td className='p-2'>{content.level}</td>
							<td className='p-2'>{content.label}</td>
							<td className='p-2'>{content.category}</td>
							<td className='p-2'>{content.entity}</td>
							<td className='p-2'>{content.message}</td>
							<td className='p-2'>{moment(content.timestamp).format('DD-MM-YYYY HH:mm:ss:SSS')}</td>
							<td className='cursor-pointer p-2 text-center'>
								<InfoPopup data={string} />
							</td>
						</tr>
					)
				})}
			</tbody>
		</table>
	)
}
