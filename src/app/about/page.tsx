// export page
export default async function Page() {
	return (
		<main className='container mx-auto flex grow gap-8'>
			<div className='flex flex-1 shrink-0 grow flex-col items-center justify-center'>
				<div className='flex h-2/3 w-full items-center justify-center rounded-xl border border-300 bg-white text-400'>Picture</div>
			</div>
			<div className='flex flex-1 shrink-0 grow flex-col items-center justify-center'>
				<div className='flex w-2/3 flex-col gap-4'>
					<h2 className='text-4xl font-light'>About HANDZONe</h2>
					<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent sit amet turpis libero. Aenean molestie euismod purus sed faucibus. Pellentesque gravida augue vel mi luctus tincidunt. Phasellus semper mauris convallis lectus porta consectetur. Duis faucibus consectetur nibh a ullamcorper. Pellentesque non tortor lacus. Ut vitae sem nec eros gravida dictum sed a lectus. Nullam congue sit amet massa et imperdiet. Proin efficitur nulla id arcu luctus dapibus. Maecenas euismod nisl at tellus pulvinar imperdiet eu vel elit. Fusce tempus et nulla ac placerat.</p>
				</div>
			</div>
		</main>
	)
}