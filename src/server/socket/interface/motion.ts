// import payload types from shared schema
import type {
	MotionConveyorPulseDecodeIn,
	MotionEncoderEnablePulseDecodeIn,
	MotionEncoderEnableSetTickCountIn,
	MotionEncoderGetTickCountIn,
	MotionEncoderGetTickCountCallback,
	MotionSetTCPIn,
	MotionEncoderSetTickCountIn,
	MotionEncoderUnwindDeltaTickCountIn,
	MotionForceModeIn,
	MotionForceModeSetDampingIn,
	MotionGetConveyorTickCountCallback,
	MotionGetTargetTCPPoseAlongPathCallback,
	MotionGetTargetTCPSpeedAlongPathCallback,
	MotionMoveCIn,
	MotionMoveJIn,
	MotionMoveLIn,
	MotionMovePIn,
	MotionPauseOnErrorCodeIn,
	MotionPositionDeviationWarningIn,
	MotionResetRevolutionCounterIn,
	MotionServoJIn,
	MotionSetConveyorTickCountIn,
	MotionSetPosIn,
	MotionSetSafetyModeTransitionHardnessIn,
	MotionSpeedJIn,
	MotionSpeedLIn,
	MotionStopConveyorTrackingIn,
	MotionStopJIn,
	MotionStopLIn,
	MotionTrackConveyorCircularIn,
	MotionTrackConveyorLinearIn,
} from '@/types/Socket/Motion'

export interface MotionClientToServer {
	'motion:conveyor_pulse_decode': (payload: MotionConveyorPulseDecodeIn) => void
	'motion:encoder_enable_pulse_decode': (payload: MotionEncoderEnablePulseDecodeIn) => void
	'motion:encoder_enable_set_tick_count': (payload: MotionEncoderEnableSetTickCountIn) => void
	'motion:encoder_get_tick_count': (payload: MotionEncoderGetTickCountIn, callback: (success: boolean, payload?: MotionEncoderGetTickCountCallback) => void) => void
	'motion:set_tcp': (payload: MotionSetTCPIn) => void
	'motion:encoder_set_tick_count': (payload: MotionEncoderSetTickCountIn) => void
	'motion:encoder_unwind_delta_tick_count': (payload: MotionEncoderUnwindDeltaTickCountIn) => number
	'motion:end_force_mode': () => void
	'motion:end_freedrive_mode': () => void
	'motion:end_teach_mode': () => void
	'motion:force_mode': (payload: MotionForceModeIn) => void
	'motion:force_mode_set_damping': (payload: MotionForceModeSetDampingIn) => void
	'motion:freedrive_mode': () => void
	'motion:freedrive_mode_no_incorrect_payload_check': () => void
	'motion:get_conveyor_tick_count': (callback: (success: boolean, payload?: MotionGetConveyorTickCountCallback) => void) => void
	'motion:get_target_tcp_pose_along_path': (callback: (success: boolean, payload?: MotionGetTargetTCPPoseAlongPathCallback) => void) => void
	'motion:get_target_tcp_speed_along_path': (callback: (success: boolean, payload?: MotionGetTargetTCPSpeedAlongPathCallback) => void) => void
	'motion:movec': (payload: MotionMoveCIn) => void
	'motion:movej': (payload: MotionMoveJIn) => void
	'motion:movel': (payload: MotionMoveLIn) => void
	'motion:movep': (payload: MotionMovePIn) => void
	'motion:pause_on_error_code': (payload: MotionPauseOnErrorCodeIn) => void
	'motion:position_deviation_warning': (payload: MotionPositionDeviationWarningIn) => void
	'motion:reset_revolution_counter': (payload: MotionResetRevolutionCounterIn) => void
	'motion:servoj': (payload: MotionServoJIn) => void
	'motion:set_conveyor_tick_count': (payload: MotionSetConveyorTickCountIn) => void
	'motion:set_pos': (payload: MotionSetPosIn) => void
	'motion:set_safety_mode_transition_hardness': (payload: MotionSetSafetyModeTransitionHardnessIn) => void
	'motion:speedj': (payload: MotionSpeedJIn) => void
	'motion:speedl': (payload: MotionSpeedLIn) => void
	'motion:stop_conveyor_tracking': (payload: MotionStopConveyorTrackingIn) => void
	'motion:stopj': (payload: MotionStopJIn) => void
	'motion:stopl': (payload: MotionStopLIn) => void
	'motion:teach_mode': () => void
	'motion:track_conveyor_circular': (payload: MotionTrackConveyorCircularIn) => void
	'motion:track_conveyor_linear': (payload: MotionTrackConveyorLinearIn) => void
}