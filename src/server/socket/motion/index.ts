// import types
import type { Socket } from 'socket.io'
import type { ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData } from '../interface'
import type { TCPServer } from '@/server/tcp'

export const handleMotionEvents = (socket: Socket<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>, tcp: TCPServer) => {

	// handle the motion:conveyor_pulse_decode event
	socket.on('motion:conveyor_pulse_decode', (type, A, B) => {
		tcp.send(socket.data.robot, `conveyor_pulse_decode(${type},${A},${B})\n`)
	})

	// handle the motion:encoder_enable_pulse_decode event
	socket.on('motion:encoder_enable_pulse_decode', (encoder_index, decoder_type, A, B) => {
		tcp.send(socket.data.robot, `encoder_enable_pulse_decode(${encoder_index},${decoder_type},${A},${B})\n`)
	})

	// handle the motion:encoder_enable_set_tick_count event
	socket.on('motion:encoder_enable_set_tick_count', (encoder_index, range_id) => {
		tcp.send(socket.data.robot, `encoder_enable_set_tick_count(${encoder_index},${range_id})\n`)
	})

	// handle the motion:encoder_get_tick_count event
	socket.on('motion:encoder_get_tick_count', () => {
		throw new Error('Get requests not implemented')
	})

	// handle the motion:encoder_set_tick_count event
	socket.on('motion:encoder_set_tick_count', (encoder_index, count) => {
		tcp.send(socket.data.robot, `encoder_set_tick_count(${encoder_index},${count})\n`)
	})

	// handle the motion:encoder_unwind_delta_tick_count event
	socket.on('motion:encoder_unwind_delta_tick_count', () => {
		throw new Error('Get requests not implemented')
	})

	// handle the motion:end_force_mode event
	socket.on('motion:end_force_mode', () => {
		tcp.send(socket.data.robot, `end_force_mode()\n`)
	})

	// handle the motion:end_freedrive_mode event
	socket.on('motion:end_freedrive_mode', () => {
		tcp.send(socket.data.robot, `end_freedrive_mode()\n`)
	})

	// handle the motion:end_teach_mode event
	socket.on('motion:end_teach_mode', () => {
		tcp.send(socket.data.robot, `end_teach_mode()\n`)
	})

	// handle the motion:force_mode event
	socket.on('motion:force_mode', (task_frame, selection_vector, wrench, type, limits) => {
		tcp.send(socket.data.robot, `force_mode([${task_frame}],[${selection_vector}],[${wrench}],${type},[${limits}])\n`)
	})

	// handle the motion:force_mode_set_damping event
	socket.on('motion:force_mode_set_damping', (damping) => {
		tcp.send(socket.data.robot, `force_mode_set_damping(${damping})\n`)
	})

	// handle the motion:freedrive_mode event
	socket.on('motion:freedrive_mode', () => {
		tcp.send(socket.data.robot, `freedrive_mode()\n`)
	})

	// handle the motion:freedrive_mode_no_incorrect_payload_check event
	socket.on('motion:freedrive_mode_no_incorrect_payload_check', () => {
		tcp.send(socket.data.robot, `freedrive_mode_no_incorrect_payload_check()\n`)
	})

	// handle the motion:get_conveyor_tick_count event
	socket.on('motion:get_conveyor_tick_count', () => {
		throw new Error('Get requests not implemented')
	})

	// handle the motion:get_target_tcp_pose_along_path event
	socket.on('motion:get_target_tcp_pose_along_path', () => {
		throw new Error('Get requests not implemented')
	})

	// handle the motion:get_target_tcp_speed_along_path event
	socket.on('motion:get_target_tcp_speed_along_path', () => {
		throw new Error('Get requests not implemented')
	})

	// handle the motion:movec event
	socket.on('motion:movec', (pose_via, pose_to, a, v, r, mode) => {
		tcp.send(socket.data.robot, `movec([${pose_via}],[${pose_to}],a=${a},v=${v},r=${r},mode=${mode})\n`)
	})

	// handle the motion:movej event
	socket.on('motion:movej', (q, a, v, t, r) => {
		tcp.send(socket.data.robot, `movej([${q}],a=${a},v=${v},t=${t},r=${r})\n`)
	})

	// handle the motion:movel event
	socket.on('motion:movel', (pose, a, v, t, r) => {
		tcp.send(socket.data.robot, `movel([${pose}], a=${a}, v=${v}, t=${t}, r=${r})\n`)
	})

	// handle the motion:movep event
	socket.on('motion:movep', (pose, a, v, r) => {
		tcp.send(socket.data.robot, `movep([${pose}], a=${a}, v=${v}, r=${r})\n`)
	})

	// handle the motion:pause_on_error_code event
	socket.on('motion:pause_on_error_code', (code, argument) => {
		tcp.send(socket.data.robot, `pause_on_error_code(${code}${argument ? `, ${argument}` : ''})\n`)
	})

	// handle the motion:position_deviation_warning event
	socket.on('motion:position_deviation_warning', (enabled, threshold) => {
		tcp.send(socket.data.robot, `position_deviation_warning(${enabled}${threshold ? `, ${threshold}` : ''})\n`)
	})

	// handle the motion:reset_revolution_counter event
	socket.on('motion:reset_revolution_counter', (qNear) => {
		tcp.send(socket.data.robot, `reset_revolution_counter(${qNear ? `qNear=[${qNear}]` : ''})\n`)
	})

	// handle the motion:servoj event
	socket.on('motion:servoj', (q, a, v, t, lookahead_time, gain) => {
		tcp.send(socket.data.robot, `servoj([${q}], a=${a}, v=${v}${t ? `, t=${t}` : ''}${lookahead_time ? `, lookahead_time=${lookahead_time}` : ''}${gain ? `, gain=${gain}` : ''})\n`)
	})

	// handle the motion:set_conveyor_tick_count event
	socket.on('motion:set_conveyor_tick_count', (tick_count, absolute_encoder_resolution) => {
		tcp.send(socket.data.robot, `set_conveyor_tick_count(${tick_count}${absolute_encoder_resolution ? `, absolute_encoder_resolution=${absolute_encoder_resolution}` : ''})\n`)
	})

	// handle the motion:set_pos event
	socket.on('motion:set_pos', (q) => {
		tcp.send(socket.data.robot, `set_pos([${q}])\n`)
	})

	// handle the motion:set_safety_mode_transition_hardness event
	socket.on('motion:set_safety_mode_transition_hardness', (type) => {
		tcp.send(socket.data.robot, `set_safety_mode_transition_hardness(${type})\n`)
	})

	// handle the motion:speedj event
	socket.on('motion:speedj', (qd, a, t) => {
		tcp.send(socket.data.robot, `speedj([${qd}],a=${a}${t ? `,t=${t}` : ''})\n`)
	})

	// handle the motion:speedl event
	socket.on('motion:speedl', (xd, a, t, aRot) => {
		tcp.send(socket.data.robot, `speedl([${xd}],a=${a}${t ? `,t=${t}` : ''}${aRot ? `,aRot=${aRot}` : ''})\n`)
	})

	// handle the motion:stop_conveyor_tracking event
	socket.on('motion:stop_conveyor_tracking', (a) => {
		tcp.send(socket.data.robot, `stop_conveyor_tracking(${a ? `a=[${a}]` : ''})\n`)
	})

	// handle the motion:stopj event
	socket.on('motion:stopj', (a) => {
		tcp.send(socket.data.robot, `stopj(${a})\n`)
	})

	// handle the motion:stopl event
	socket.on('motion:stopl', (a, aRot) => {
		tcp.send(socket.data.robot, `stopl(${a}${aRot ? `,aRot=${aRot}` : ''})\n`)
	})

	// handle the motion:teach_mode event
	socket.on('motion:teach_mode', () => {
		tcp.send(socket.data.robot, `teach_mode()\n`)
	})

	// handle the motion:track_conveyor_circular event
	socket.on('motion:track_conveyor_circular', (center, ticksPerRevolution, rotateTool, encoderIndex) => {
		tcp.send(socket.data.robot, `track_conveyor_circular([${center}], ${ticksPerRevolution}, ${rotateTool}${encoderIndex ? `, encoder_index=${encoderIndex}` : ''})\n`)
	})

	// handle the motion:track_conveyor_linear event
	socket.on('motion:track_conveyor_linear', (direction, ticksPerMeter, encoderIndex) => {
		tcp.send(socket.data.robot, `track_conveyor_linear([${direction}], ${ticksPerMeter}${encoderIndex ? `, encoder_index=${encoderIndex}` : ''})\n`)
	})

}