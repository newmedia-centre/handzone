// import dependencies
import { env } from '@/server/environment'
import { validateRequest } from '@/server/db/auth'

// import components
import { ConnectVR } from '@/components/connect'

export const Header = async () => {
	// get the user
	const { user } = await validateRequest()

	// check if the user is admin
	const admin = user?.id === 'dc35f37334f3d9d881f1e3276295d37ca0944d64'

	return (
		<header className='flex shrink-0 items-center justify-between border-b border-300 bg-white p-2 shadow-md'>
			<div className='flex flex-1 items-center justify-start gap-2 px-2'>
				<a href='/tutorials' className='w-24 rounded border bg-white p-2 text-center hover:bg-200'>Tutorials</a>
				{user && (
					<>
						<a href='/about' className='w-24 rounded border bg-white p-2 text-center hover:bg-200'>About</a>
						<ConnectVR />
					</>
				)}
			</div>
			<a href='/' className='p-2'>
				<h1 className='text-2xl'>
					HANDZONe
				</h1>
			</a>
			<div className='flex flex-1 items-center justify-end gap-2 px-2'>
				{admin && (
					<>
						<a href='/data' className='w-24 rounded border bg-white p-2 text-center hover:bg-200'>Data</a>
						<a href='/logs' className='w-24 rounded border bg-white p-2 text-center hover:bg-200'>Logs</a>
					</>
				)}
				{user ? (
					<a href='/api/auth/logout' className='w-24 rounded border bg-white p-2 text-center hover:bg-200'>Log out</a>
				) : (
					<a href='/api/auth/oauth' className='rounded border bg-white p-2 hover:bg-200'>Log in with {env.OAUTH.name ?? 'SSO'}</a>
				)}
			</div>
		</header>
	)
}