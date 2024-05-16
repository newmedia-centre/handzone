// import dependencies
import { env } from '@/server/environment'

// export page
export default async function Page() {
	return (
		<main className='flex grow flex-col items-center justify-center'>
			<div className='flex flex-col items-center gap-4'>
				<h1 className='font-semibold uppercase text-stone-400'>Sign in to use Handzone</h1>
				<a href='/api/auth/oauth' className='rounded border bg-stone-50 p-2 hover:bg-stone-100'>Sign in with {env.OAUTH.name ?? 'SSO'}</a>
			</div>
		</main>
	)
}