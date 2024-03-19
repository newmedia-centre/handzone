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