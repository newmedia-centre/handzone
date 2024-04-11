import { env } from '@/server/environment'

export default async function Page() {
	return (
		<>
			<h1>Sign in</h1>
			<a href='/api/auth/oauth'>Sign in with {env.OAUTH.name ?? 'SSO'}</a>
		</>
	)
}