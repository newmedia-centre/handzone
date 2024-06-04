// import components
import { Header } from './header'
import { Footer } from './footer'

// import styles
import { Inter } from 'next/font/google'
import './globals.css'

// import types
import type { Metadata } from 'next'

// load the Inter font
const inter = Inter({ subsets: ['latin'] })

// export metadata for the website
export const metadata: Metadata = {
	title: 'HANDZONe',
	description: 'Control robots in VR',
}

// export common layout for the entire website
export default function RootLayout({
	children,
}: Readonly<{
	children: React.ReactNode;
}>) {
	return (
		<html lang='en'>
			<body className={'flex flex-col size-full min-h-screen max-h-screen overflow-hidden mx-auto bg-100 ' + inter.className}>
				<Header />
				{children}
				<Footer />
			</body>
		</html>
	)
}
