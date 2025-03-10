/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

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
	MotionEncoderUnwindDeltaTickCountCallback,
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
import type { CallbackFn } from '.'

export interface MotionClientToServer {
	'motion:conveyor_pulse_decode': (payload: MotionConveyorPulseDecodeIn) => void
	'motion:encoder_enable_pulse_decode': (payload: MotionEncoderEnablePulseDecodeIn) => void
	'motion:encoder_enable_set_tick_count': (payload: MotionEncoderEnableSetTickCountIn) => void
	'motion:encoder_get_tick_count': (payload: MotionEncoderGetTickCountIn, callback: CallbackFn<MotionEncoderGetTickCountCallback>) => void
	'motion:set_tcp': (payload: MotionSetTCPIn) => void
	'motion:encoder_set_tick_count': (payload: MotionEncoderSetTickCountIn) => void
	'motion:encoder_unwind_delta_tick_count': (payload: MotionEncoderUnwindDeltaTickCountIn, callback: CallbackFn<MotionEncoderUnwindDeltaTickCountCallback>) => void
	'motion:end_force_mode': () => void
	'motion:end_freedrive_mode': () => void
	'motion:end_teach_mode': () => void
	'motion:force_mode': (payload: MotionForceModeIn) => void
	'motion:force_mode_set_damping': (payload: MotionForceModeSetDampingIn) => void
	'motion:freedrive_mode': () => void
	'motion:freedrive_mode_no_incorrect_payload_check': () => void
	'motion:get_conveyor_tick_count': (callback: CallbackFn<MotionGetConveyorTickCountCallback>) => void
	'motion:get_target_tcp_pose_along_path': (callback: CallbackFn<MotionGetTargetTCPPoseAlongPathCallback>) => void
	'motion:get_target_tcp_speed_along_path': (callback: CallbackFn<MotionGetTargetTCPSpeedAlongPathCallback>) => void
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