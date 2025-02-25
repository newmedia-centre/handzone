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

// import payload types from shared schema
import type {
	UnityMessageIn,
	UnityMessageOut,
	UnityPendantOut,
	UnityPlayerIn,
	UnityPlayersOut,
	UnityRunIn
} from '@/types/Socket/Unity'

export interface UnityClientToServer {
	'unity:message': (payload: UnityMessageIn) => void
	'unity:player': (payload: UnityPlayerIn) => void
	'unity:pendant': () => void
	'unity:run': (payload: UnityRunIn) => void
}

export interface UnityServerToClient {
	'unity:message': (payload: UnityMessageOut) => void
	'unity:players': (payload: UnityPlayersOut) => void
	'unity:pendant': (payload: UnityPendantOut) => void
}