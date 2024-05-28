// import components
import Image from 'next/image'

export const Footer = () => (
	<footer className='flex items-center justify-center border-t border-300 bg-white p-4'>
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
)