// import payload types from shared schema
import type {
	GrasshopperProgramIn,
	GrasshopperRunIn,
	GrasshopperProgramOut
} from '@/types/Socket/Grasshopper'

export interface GrasshopperClientToServer {
	'grasshopper:program': (payload: GrasshopperProgramIn) => void
	'grasshopper:run': (payload: GrasshopperRunIn) => void
}

export interface GrasshopperServerToClient {
	'grasshopper:program': (payload: GrasshopperProgramOut) => void
}