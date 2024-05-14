// import payload types from shared schema
import type {
	GrasshopperProgramIn,
	GrasshopperProgramOut
} from '@/types/Socket/Grasshopper'

export interface GrasshopperClientToServer {
	'grasshopper:program': (payload: GrasshopperProgramIn) => void
}

export interface GrasshopperServerToClient {
	'grasshopper:program': (payload: GrasshopperProgramOut) => void
}