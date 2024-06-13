/** @type {import('next').NextConfig} */
const nextConfig = {
	output: 'standalone',
	webpack: (config) => {
		config.module.rules.push({
			test: /\.node$/,
			loader: 'node-loader',
		})
		return config
	},
}

export default nextConfig
