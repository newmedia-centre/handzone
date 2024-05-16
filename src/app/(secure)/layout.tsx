// import dependencies
import { redirect } from 'next/navigation'
import { validateRequest } from '@/server/db/auth'

// import components
import { UserProvider } from '@/hooks/user'
import Image from 'next/image'

// export layout
export default async function Layout({
	children,
}: Readonly<{
	children: React.ReactNode;
}>) {
	// check if the user is logged in
	const { user } = await validateRequest()

	// redirect to login if user is not available
	if (!user) {
		return redirect('/login')
	}

	// show the website after securing
	return (
		<UserProvider user={user}>
			<header className='flex items-center justify-between p-4'>
				<div>
					Connect VR
				</div>
				<a href='/' className='p-2'>
					<h1 className='text-2xl'>
						HANDZONe
					</h1>
				</a>
				<div>
					Buttons
				</div>
			</header>
			{children}
			<footer className='flex items-center justify-center p-4'>
				<a href='https://newmediacentre.tudelft.nl'>
					<Image
						src='/logo-nmc.png'
						alt='NewMedia Centre Logo'
						width={256}
						height={32}
						priority
					/>
				</a>
			</footer>
		</UserProvider>
	)
}
