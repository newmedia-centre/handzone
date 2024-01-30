#!/usr/bin/env ts-node

//import { readFileSync } from "fs";
import * as path from 'path'
import { glob } from 'glob'
import { readFileSync, writeFileSync } from 'fs'
import {
	quicktype,
	InputData,
	JSONSchemaInput,
	FetchingJSONSchemaStore
} from 'quicktype-core'

async function exportCSharp(name: string, namespace: string, schema: string) {
	const schemaInput = new JSONSchemaInput(new FetchingJSONSchemaStore())

	// we could add multiple schemas for multiple types,
	// but here we're just making one type from JSON schema.
	await schemaInput.addSource({ name, schema })

	const inputData = new InputData()
	inputData.addInput(schemaInput)

	return await quicktype({
		inputData,
		lang: 'cs',
		rendererOptions: {
			namespace,
			features: 'just-types-and-namespace'
		}
	})
}

async function ExportTypescript(typeName: string, jsonSchemaString: string) {
	const schemaInput = new JSONSchemaInput(new FetchingJSONSchemaStore())

	// we could add multiple schemas for multiple types,
	// but here we're just making one type from JSON schema.
	await schemaInput.addSource({ name: typeName, schema: jsonSchemaString })

	const inputData = new InputData()
	inputData.addInput(schemaInput)

	return await quicktype({
		inputData,
		lang: 'typescript',
		rendererOptions: {
			'just-types': true,
		}
	})
}

async function main() {
	// get all the schema files in the project
	const files = await glob(__dirname + '/**/*.schema.json', {})

	// iterate over all schema files
	files.forEach(async (file) => {
		const namespace = 'Schema.' + path.dirname(file.replace(__dirname, '').replace(/^\/+/g, '')).replace(/\/|\\/g, '.')
		const name = path.basename(file).replace('.schema.json', '')

		// read the schema
		const schema = readFileSync(file, 'utf8')

		// generate typescript
		const { lines: typescript } = await ExportTypescript(name, schema)
		writeFileSync(path.join(path.dirname(file), `${name}.ts`), '/* eslint-disable */\n' + typescript.join('\n'))

		// generate csharp
		const { lines: csharp } = await exportCSharp(name, namespace, schema)
		writeFileSync(path.join(path.dirname(file), `${name}.cs`), csharp.join('\n'))
	})
}

main()