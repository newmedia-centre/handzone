#!/usr/bin/env ts-node

//import { readFileSync } from "fs";
import * as path from 'path'
import { glob } from 'glob'
import { writeFileSync } from 'fs'
import {
	quicktype,
	InputData,
	JSONSchemaInput,
	FetchingJSONSchemaStore,
} from 'quicktype-core'
import { schemaForTypeScriptSources } from 'quicktype-typescript-input'

async function exportCSharp(namespace: string, inputData: InputData) {
	return await quicktype({
		inputData,
		lang: 'cs',
		rendererOptions: {
			namespace,
			features: 'just-types-and-namespace',

		}
	})
}

/*
async function exportCPlusPlus(namespace: string, inputData: InputData) {
	return await quicktype({
		inputData,
		lang: 'cpp',
		rendererOptions: {
			namespace,
			'just-types': true,
			'code-format': 'with-struct',
		}
	})
}
*/

async function main() {
	// get all the schema files in the project
	const files = await glob(__dirname + '/src/**/*.ts', {})

	// iterate over all schema files
	files.forEach(async (file) => {
		const namespace = 'Schema.' + path.dirname(file.replace(__dirname + '/src', '').replace(/^\/+/g, '')).replace(/\/|\\/g, '.')
		const name = path.basename(file).replace('.ts', '')

		console.log('Processing:', file, '->', `${namespace}.${name}`)

		const schemaInput = new JSONSchemaInput(new FetchingJSONSchemaStore())
		const typescriptSchema = schemaForTypeScriptSources([file])
		await schemaInput.addSource(typescriptSchema)

		const inputData = new InputData()
		inputData.addInput(schemaInput)

		// generate csharp
		const { lines: csharp } = await exportCSharp(`${namespace}.${name}`, inputData)
		writeFileSync(path.join(path.dirname(file), `${name}.cs`), csharp.join('\n'))


		/*
		// generate c++
		const { lines: cpp } = await exportCPlusPlus(`${namespace}.${name}`, inputData)
		writeFileSync(path.join(path.dirname(file), `${name}.h`), cpp.join('\n'))
		*/
	})
}

main()