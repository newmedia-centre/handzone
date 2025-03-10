/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

module.exports = {
	env: {
		node: true, es2022: true, browser: true,
	},
	extends: ['next/core-web-vitals', 'plugin:react/recommended', 'plugin:react/jsx-runtime', 'plugin:tailwindcss/recommended', 'plugin:@typescript-eslint/recommended'],
	parser: '@typescript-eslint/parser',
	parserOptions: {
		ecmaVersion: 'latest', sourceType: 'module', project: ['./tsconfig.json']
	},
	ignorePatterns: ['*.config.js', '*.config.cjs', '*.config.ts', 'schema/generate.ts'],
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
