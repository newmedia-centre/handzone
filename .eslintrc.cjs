module.exports = {
	env: {
		node: true, es2022: true, browser: true,
	},
	extends: ['eslint:recommended', 'plugin:react/recommended', 'plugin:react/jsx-runtime', 'plugin:tailwindcss/recommended', 'plugin:@typescript-eslint/recommended'],
	overrides: [{
		files: ['*.ts', '*.tsx'],
		extends: ['plugin:@typescript-eslint/recommended', 'plugin:@typescript-eslint/recommended-requiring-type-checking',],
		parserOptions: {
			project: ['./tsconfig.json'],
		},
		excludedFiles: ['*.config.js', '*.config.ts'],
	}],
	parser: '@typescript-eslint/parser',
	parserOptions: {
		ecmaVersion: 'latest', sourceType: 'module', project: ['./tsconfig.json']
	},
	plugins: ['react', '@typescript-eslint'],
	settings: {
		react: {
			version: 'detect'
		}
	},
	rules: {
		'quotes': ['warn', 'single', { avoidEscape: true, allowTemplateLiterals: true }],
		'jsx-quotes': ['warn', 'prefer-single'],
		'semi': ['warn', 'never'],
		'padded-blocks': 'off',
		'no-trailing-spaces': ['warn', { skipBlankLines: true }],
		'no-tabs': 'off',
		'capitalized-comments': ['warn', 'never', { 'ignoreConsecutiveComments': true }],
		'array-element-newline': 'off',
		'react/prop-types': 'off',
		'@typescript-eslint/no-var-requires': 'off',
		'@typescript-eslint/indent': ['warn', 'tab', { SwitchCase: 1 }],
		'@typescript-eslint/no-unused-vars': ['warn', { vars: 'all', args: 'after-used', ignoreRestSiblings: false }],
		'@typescript-eslint/consistent-type-imports': ['warn', { prefer: 'type-imports', fixStyle: 'separate-type-imports' }],
		'@typescript-eslint/consistent-type-exports': 'warn',
		'tailwindcss/no-custom-classname': 'off'
	}
}
