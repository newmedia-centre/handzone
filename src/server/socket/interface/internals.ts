// import payload types from shared schema
import type {
	InternalsGetInverseKinIn,
	InternalsGetInverseKinCallback
} from '@/types/Socket/Internals'
import type { CallbackFn } from '.'

export interface InternalsClientToServer {
	'internals:get_inverse_kin': (payload: InternalsGetInverseKinIn, callback: CallbackFn<InternalsGetInverseKinCallback>) => void
}