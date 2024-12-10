/*
 * Copyright (c) 2024 NewMedia Centre - Delft University of Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the 'License');
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an 'AS IS' BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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