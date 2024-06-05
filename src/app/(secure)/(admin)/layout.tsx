// import dependencies
import { validateRequest } from '@/server/db/auth'
import { redirect } from 'next/navigation'

// export layout
export default async function Layout({
	children,
}: Readonly<{
	children: React.ReactNode;
}>) {
	// get the user
	const { user } = await validateRequest()

	// redirect to login if user is not available
	if (!user?.admin) {
		return redirect('/')
	}

	// show the website after securing
	return (
		<>{children}</>
	)
}
