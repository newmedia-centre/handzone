// import payload types from shared schema
import type {
	InterfacesSetToolDigitalOutIn,
	InterfacesSetStandardDigitalOutIn,
} from '@/types/Socket/Interfaces'

export interface InterfacesClientToServer {
	'interfaces:set_tool_digital_out': (payload: InterfacesSetToolDigitalOutIn) => void
	'interfaces:set_standard_digital_out': (payload: InterfacesSetStandardDigitalOutIn) => void
}