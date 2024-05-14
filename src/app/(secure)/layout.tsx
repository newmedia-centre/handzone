// import dependencies
import { redirect } from 'next/navigation'
import { validateRequest } from '@/server/db/auth'

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
		<>{children}</>
	)
}
