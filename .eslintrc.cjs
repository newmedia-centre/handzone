
module.exports = {
	env: {
		node: true, es2022: true, browser: true,
	},
	extends: ['eslint:recommended', 'plugin:react/recommended', 'plugin:react/jsx-runtime', 'plugin:tailwindcss/recommended', 'plugin:@typescript-eslint/recommended'],
	parser: '@typescript-eslint/parser',
	parserOptions: {
		ecmaVersion: 'latest', sourceType: 'module', project: ['./tsconfig.json']
	},
	ignorePatterns: ['*.config.js', '*.config.ts', 'schema/**'],
	plugins: ['react', '@typescript-eslint', 'node', '@stylistic'],
	settings: {
		react: {
			version: 'detect'
		}
	},
	rules: {
		'quotes': ['warn', 'single', { avoidEscape: true, allowTemplateLiterals: true }],
		'jsx-quotes': ['warn', 'prefer-single'],
		'@stylistic/semi': ['warn', 'never'],
		'padded-blocks': 'off',
		'no-trailing-spaces': ['warn', { skipBlankLines: true }],
		'no-tabs': 'off',
		'capitalized-comments': ['warn', 'never',
			{
				line: {
					'ignoreConsecutiveComments': true
				},
				block: {
					'ignorePattern': '.*'
				}
			}
		],
		'array-element-newline': 'off',
		'react/prop-types': 'off',
		'@typescript-eslint/no-var-requires': 'off',
		'@typescript-eslint/indent': ['warn', 'tab', { SwitchCase: 1 }],
		'@typescript-eslint/no-unused-vars': ['warn', { vars: 'all', args: 'after-used', ignoreRestSiblings: false }],
		'@typescript-eslint/consistent-type-imports': ['warn', { prefer: 'type-imports', fixStyle: 'separate-type-imports' }],
		'@typescript-eslint/consistent-type-exports': 'warn',
		'tailwindcss/no-custom-classname': 'off',
		'node/no-process-env': 'warn'
	}
}
