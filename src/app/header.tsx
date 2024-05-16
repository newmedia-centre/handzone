// import dependencies
import { env } from '@/server/environment'
import { validateRequest } from '@/server/db/auth'

// import components
import { ConnectVR } from '@/components/connect'

export const Header = async () => {
	// get the user
	const { user } = await validateRequest()

	// check if the user is admin
	const admin = true

	return (
		<header className='flex items-center justify-between p-4'>
			<div className='flex flex-1 items-center justify-start gap-2 p-2'>
				<a href='/tutorials' className='rounded border bg-stone-50 p-2 hover:bg-stone-100'>Tutorials</a>
				{user && (
					<>
						<a href='/about' className='rounded border bg-stone-50 p-2 hover:bg-stone-100'>About</a>
						<ConnectVR />
					</>
				)}
			</div>
			<a href='/' className='p-2'>
				<h1 className='text-2xl'>
					HANDZONe
				</h1>
			</a>
			<div className='flex flex-1 items-center justify-end gap-2 p-2'>
				{admin && (
					<>
						<a href='/data' className='rounded border bg-stone-50 p-2 hover:bg-stone-100'>Data</a>
						<a href='/logs' className='rounded border bg-stone-50 p-2 hover:bg-stone-100'>Logs</a>
					</>
				)}
				{user ? (
					<a href='/api/auth/logout' className='rounded border bg-stone-50 p-2 hover:bg-stone-100'>Log out</a>
				) : (
					<a href='/api/auth/oauth' className='rounded border bg-stone-50 p-2 hover:bg-stone-100'>Log in with {env.OAUTH.name ?? 'SSO'}</a>
				)}
			</div>
		</header>
	)
}