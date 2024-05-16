// import components
import { WelcomeUser } from './welcome'
import { ValidateVRPin } from './validate-vr-pin'

export default async function Page() {
	return (
		<main className='flex grow flex-col items-center justify-between p-24'>
			<div className='z-10 w-full max-w-5xl items-center justify-center font-mono text-sm lg:flex'>
				<div className='flex flex-col gap-2 p-2'>
					<WelcomeUser />
					<ValidateVRPin />
				</div>
			</div>
		</main>
	)
}
