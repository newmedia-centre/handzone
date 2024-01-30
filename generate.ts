#!/usr/bin/env ts-node

import { readFileSync } from "fs";
import {
	quicktype,
	InputData,
	JSONSchemaInput,
	FetchingJSONSchemaStore
} from "quicktype-core";

async function exportTypescript(typeName: string, jsonSchemaString: string) {
	const schemaInput = new JSONSchemaInput(new FetchingJSONSchemaStore());

	// We could add multiple schemas for multiple types,
	// but here we're just making one type from JSON schema.
	await schemaInput.addSource({ name: typeName, schema: jsonSchemaString });

	const inputData = new InputData();
	inputData.addInput(schemaInput);

	return await quicktype({
		inputData,
		lang: 'csharp',
		rendererOptions: {
			namespace: 'RealtimeData',
			features: 'just-types'
		}
	});
}

/*
async function exportCSharp(typeName: string, jsonSchemaString: string) {
	const schemaInput = new JSONSchemaInput(new FetchingJSONSchemaStore());

	// We could add multiple schemas for multiple types,
	// but here we're just making one type from JSON schema.
	await schemaInput.addSource({ name: typeName, schema: jsonSchemaString });

	const inputData = new InputData();
	inputData.addInput(schemaInput);

	return await quicktype({
		inputData,
		lang: 'typescript',
	});
}
*/

async function main() {
	const schema = readFileSync("socket/Vector6d.schema.json", "utf8");

	const { lines: typescriptOut } = await exportTypescript("Vector6D", schema);
	console.log(typescriptOut.join("\n"));

	//const { lines: csharpOut } = await exportCSharp("Vector6D", schema);
	//console.log(csharpOut.join("\n"));
}

main();