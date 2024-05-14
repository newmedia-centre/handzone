// import payload types from shared schema
import type {
	InternalsGetInverseKinIn,
	InternalsGetInverseKinCallback
} from '@/types/Socket/Internals'

export interface InternalsClientToServer {
	'internals:get_inverse_kin': (payload: InternalsGetInverseKinIn, callback: (success: boolean, payload?: InternalsGetInverseKinCallback) => void) => void
}