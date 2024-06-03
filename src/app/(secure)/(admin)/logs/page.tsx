// import dependencies
import fs from 'fs'
import path from 'path'
import env from '@/server/environment'
import moment from 'moment'

// import components
import { InfoPopup } from './popup'

export default function Page() {
	// read the logs
	const errors = fs.readFileSync(path.resolve(env.LOGS_PATH, 'errors.log'), 'utf-8').split('\n').filter(x => x.length > 0)

	return (
		<table className='table w-full grow table-fixed'>
			<thead>
				<tr className='border-b border-300'>
					<th className='w-72 p-2 text-left'>Label</th>
					<th className='w-36 p-2 text-left'>Category</th>
					<th className='w-36 p-2 text-left'>Entity</th>
					<th className='p-2 text-left'>Message</th>
					<th className='w-56 p-2 text-left'>Timestamp</th>
					<th className='w-48 p-2 text-center'>More Info</th>
				</tr>
			</thead>
			<tbody>
				{errors.map((string, index) => {
					const content = JSON.parse(string)
					return (
						<tr key={index} className='border-b border-300 hover:bg-50'>
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
