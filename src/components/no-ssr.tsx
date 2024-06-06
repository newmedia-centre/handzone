// import dependencies
import dynamic from 'next/dynamic'

// import components
import { Fragment } from 'react'

const NoSsr = ({ children }: { children: React.ReactNode; }) => (
	<Fragment>{children}</Fragment>
)

export default dynamic(() => Promise.resolve(NoSsr), {
	ssr: false
})