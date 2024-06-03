'use client'

// import dependencies
import { useRouter } from 'next/navigation'
import path from 'path'

// import components
import { Listbox, ListboxButton, ListboxOption, ListboxOptions } from '@headlessui/react'

// logs select component
export const LogSelect = ({ options }: { options: string[] }) => {
	const router = useRouter()

	return (
		<Listbox value='' onChange={value => router.push(`/logs/${value}`)}>
			<ListboxButton className='inline-flex w-48 items-center justify-between rounded border px-4 py-2 text-center hover:bg-200'>
				<span>Select Log File</span>
				<svg className='ms-3 size-2.5' aria-hidden='true' xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 10 6'>
					<path stroke='currentColor' strokeLinecap='round' strokeLinejoin='round' strokeWidth='2' d='m1 1 4 4 4-4' />
				</svg>
			</ListboxButton>
			<ListboxOptions anchor='bottom' className='z-30 mt-1 w-48 rounded border border-300 bg-white text-center'>
				{options.map((option, index) => (
					<ListboxOption value={path.parse(option).name} key={index} className='cursor-pointer p-2 hover:bg-200'>
						{path.parse(option).name}
					</ListboxOption>
				))}
			</ListboxOptions>
		</Listbox>
	)
}