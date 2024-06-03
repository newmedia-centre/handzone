// import components
import Image from 'next/image'

export const Footer = () => (
	<footer className='flex items-center justify-center gap-8 border-t border-300 bg-white p-4'>
		<a href='https://newmediacentre.tudelft.nl'>
			<Image
				src='/logo-nmc.png'
				alt='NewMedia Centre Logo'
				width={256}
				height={32}
				priority
			/>

		</a>
		<a href='https://newmediacentre.tudelft.nl/xr'>
			<Image
				src='/logo-xr.png'
				alt='XR-Zone Logo'
				width={128}
				height={32}
				priority
			/>
		</a>
	</footer>
)