import type {
	InterfacesSetToolDigitalOutIn,
	InterfacesSetStandardDigitalOutIn,
	InterfacesGetInverseKinIn,
	InterfacesGetInverseKinCallback
} from '@/types/Socket/Interfaces'

export interface InterfacesClientToServer {
	'interfaces:set_tool_digital_out': (payload: InterfacesSetToolDigitalOutIn) => void
	'interfaces:set_standard_digital_out': (payload: InterfacesSetStandardDigitalOutIn) => void
	'interfaces:get_inverse_kin': (payload: InterfacesGetInverseKinIn, callback: (payload: InterfacesGetInverseKinCallback) => void) => void
}