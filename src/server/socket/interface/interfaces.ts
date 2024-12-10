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
	InterfacesSetToolDigitalOutIn,
	InterfacesSetStandardDigitalOutIn,
} from '@/types/Socket/Interfaces'

export interface InterfacesClientToServer {
	'interfaces:set_tool_digital_out': (payload: InterfacesSetToolDigitalOutIn) => void
	'interfaces:set_standard_digital_out': (payload: InterfacesSetStandardDigitalOutIn) => void
}