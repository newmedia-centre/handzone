// import dependencies
import { redirect } from 'next/navigation'

// export layout
export default async function Layout({
	children,
}: Readonly<{
	children: React.ReactNode;
}>) {
	// check if the user is admin
	const admin = true

	// redirect to login if user is not available
	if (!admin) {
		return redirect('/')
	}

	// show the website after securing
	return (
		<>{children}</>
	)
}
