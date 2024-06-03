// import dependencies
import { redirect } from 'next/navigation'
import { validateRequest } from '@/server/db/auth'

// import components
import { UserProvider } from '@/hooks/user'

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
		return redirect('/about')
	}

	// show the website after securing
	return (
		<UserProvider user={user}>
			{children}
		</UserProvider>
	)
}
