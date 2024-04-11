// import dependencies
import { env } from '@/server/environment'

// import components
import Image from 'next/image'

// export page
export default async function Page() {
	return (
		<div className='flex h-screen w-screen flex-col items-center justify-between'>
			<div></div>
			<div className='flex flex-col items-center gap-4'>
				<h1 className='font-semibold uppercase text-stone-400'>Sign in to use Handzone</h1>
				<a href='/api/auth/oauth' className='rounded border bg-stone-50 p-2 hover:bg-stone-100'>Sign in with {env.OAUTH.name ?? 'SSO'}</a>
			</div>
			<div className='p-4'>
				<a href='https://newmediacentre.tudelft.nl'>
					<Image
						src='/logo-nmc.png'
						alt='NewMedia Centre Logo'
						width={256}
						height={32}
						priority
					/>
				</a>
			</div>
		</div>
	)
}