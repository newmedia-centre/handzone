/*
 * Copyright (c) 2024 NewMedia Centre - Delft University of Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the 'License');
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an 'AS IS' BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// import dependencies
import { env } from '@/server/environment'

// import types
import type { Socket } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'

export const handleMotionEvents = (socket: Socket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>) => {

	// handle the motion:conveyor_pulse_decode event
	socket.on('motion:conveyor_pulse_decode', ({ type, A, B }) => {
		socket.data.robot.send(`conveyor_pulse_decode(${type},${A},${B})\n`)
	})

	// handle the motion:encoder_enable_pulse_decode event
	socket.on('motion:encoder_enable_pulse_decode', ({ encoder_index, decoder_type, A, B }) => {
		socket.data.robot.send(`encoder_enable_pulse_decode(${encoder_index},${decoder_type},${A},${B})\n`)
	})

	// handle the motion:encoder_enable_set_tick_count event
	socket.on('motion:encoder_enable_set_tick_count', ({ encoder_index, range_id }) => {
		socket.data.robot.send(`encoder_enable_set_tick_count(${encoder_index},${range_id})\n`)
	})

	// handle the motion:encoder_get_tick_count event
	socket.on('motion:encoder_get_tick_count', async ({ encoder_index }, callback) => {
		const instruction = `def get_value():\nsocket_open("${env.HOSTNAME}", ${env.TCP_PORT}, "socket_value")\nvalue=encoder_get_tick_count(${encoder_index})\nsocket_send_string(to_str(value), "socket_value")\nend\n`
		try {
			const res = await socket.data.robot.sendCallback(instruction)
			const count = JSON.parse(res.toString('utf8'))
			callback(true, { count })
		} catch (e) {
			callback(false, 'Error getting inverse kinematics')
		}
	})

	// handle the motion:set_pose event
	socket.on('motion:set_tcp', (pose) => {
		socket.data.robot.send(`set_tcp(p[${pose}])\n`)
	})

	// handle the motion:encoder_set_tick_count event
	socket.on('motion:encoder_set_tick_count', ({ encoder_index, count }) => {
		socket.data.robot.send(`encoder_set_tick_count(${encoder_index},${count})\n`)
	})

	// handle the motion:encoder_unwind_delta_tick_count event
	socket.on('motion:encoder_unwind_delta_tick_count', async ({ encoder_index, delta_tick_count }, callback) => {
		const instruction = `def get_value():\nsocket_open("${env.HOSTNAME}", ${env.TCP_PORT}, "socket_value")\nvalue=encoder_unwind_delta_tick_count(${encoder_index}, ${delta_tick_count})\nsocket_send_string(to_str(value), "socket_value")\nend\n`
		try {
			const res = await socket.data.robot.sendCallback(instruction)
			const count = JSON.parse(res.toString('utf8'))
			callback(true, { count })
		} catch (e) {
			callback(false, 'Error getting inverse kinematics')
		}
	})

	// handle the motion:end_force_mode event
	socket.on('motion:end_force_mode', () => {
		socket.data.robot.send(`end_force_mode()\n`)
	})

	// handle the motion:end_freedrive_mode event
	socket.on('motion:end_freedrive_mode', () => {
		socket.data.robot.send(`end_freedrive_mode()\n`)
	})

	// handle the motion:end_teach_mode event
	socket.on('motion:end_teach_mode', () => {
		socket.data.robot.send(`end_teach_mode()\n`)
	})

	// handle the motion:force_mode event
	socket.on('motion:force_mode', ({ task_frame, selection_vector, wrench, type, limits }) => {
		socket.data.robot.send(`force_mode([${task_frame}],[${selection_vector}],[${wrench}],${type},[${limits}])\n`)
	})

	// handle the motion:force_mode_set_damping event
	socket.on('motion:force_mode_set_damping', (damping) => {
		socket.data.robot.send(`force_mode_set_damping(${damping})\n`)
	})

	// handle the motion:freedrive_mode event
	socket.on('motion:freedrive_mode', () => {
		socket.data.robot.send(`freedrive_mode()\n`)
	})

	// handle the motion:freedrive_mode_no_incorrect_payload_check event
	socket.on('motion:freedrive_mode_no_incorrect_payload_check', () => {
		socket.data.robot.send(`freedrive_mode_no_incorrect_payload_check()\n`)
	})

	// handle the motion:get_conveyor_tick_count event
	socket.on('motion:get_conveyor_tick_count', async (callback) => {
		const instruction = `def get_value():\nsocket_open("${env.HOSTNAME}", ${env.TCP_PORT}, "socket_value")\nvalue=get_conveyor_tick_count()\nsocket_send_string(to_str(value), "socket_value")\nend\n`
		try {
			const res = await socket.data.robot.sendCallback(instruction)
			const count = JSON.parse(res.toString('utf8'))
			callback(true, { count })
		} catch (e) {
			callback(false, 'Error getting inverse kinematics')
		}
	})

	// handle the motion:get_target_tcp_pose_along_path event
	socket.on('motion:get_target_tcp_pose_along_path', async (callback) => {
		const instruction = `def get_value():\nsocket_open("${env.HOSTNAME}", ${env.TCP_PORT}, "socket_value")\nvalue=get_target_tcp_pose_along_path()\nsocket_send_string(to_str(value), "socket_value")\nend\n`
		try {
			const res = await socket.data.robot.sendCallback(instruction)
			const pose = JSON.parse(res.toString('utf8'))
			callback(true, { pose })
		} catch (e) {
			callback(false, 'Error getting inverse kinematics')
		}
	})

	// handle the motion:get_target_tcp_speed_along_path event
	socket.on('motion:get_target_tcp_speed_along_path', async (callback) => {
		const instruction = `def get_value():\nsocket_open("${env.HOSTNAME}", ${env.TCP_PORT}, "socket_value")\nvalue=get_target_tcp_speed_along_path()\nsocket_send_string(to_str(value), "socket_value")\nend\n`
		try {
			const res = await socket.data.robot.sendCallback(instruction)
			const speed = JSON.parse(res.toString('utf8'))
			callback(true, { speed })
		} catch (e) {
			callback(false, 'Error getting inverse kinematics')
		}
	})

	// handle the motion:movec event
	socket.on('motion:movec', ({ pose_via, pose_to, a, v, r, mode }) => {
		socket.data.robot.send(`movec([${pose_via}],[${pose_to}],a=${a},v=${v},r=${r},mode=${mode})\n`)
	})

	// handle the motion:movej event
	socket.on('motion:movej', ({ q, a, v, t, r }) => {
		socket.data.robot.send(`movej([${q}],a=${a},v=${v},t=${t},r=${r})\n`)
	})

	// handle the motion:movel event
	socket.on('motion:movel', ({ pose, a, v, t, r }) => {
		socket.data.robot.send(`movel([${pose}], a=${a}, v=${v}, t=${t}, r=${r})\n`)
	})

	// handle the motion:movep event
	socket.on('motion:movep', ({ pose, a, v, r }) => {
		socket.data.robot.send(`movep([${pose}], a=${a}, v=${v}, r=${r})\n`)
	})

	// handle the motion:pause_on_error_code event
	socket.on('motion:pause_on_error_code', ({ code, argument }) => {
		socket.data.robot.send(`pause_on_error_code(${code}${argument ? `, ${argument}` : ''})\n`)
	})

	// handle the motion:position_deviation_warning event
	socket.on('motion:position_deviation_warning', ({ enabled, threshold }) => {
		socket.data.robot.send(`position_deviation_warning(${enabled}${threshold ? `, ${threshold}` : ''})\n`)
	})

	// handle the motion:reset_revolution_counter event
	socket.on('motion:reset_revolution_counter', (qNear) => {
		socket.data.robot.send(`reset_revolution_counter(${qNear ? `qNear=[${qNear}]` : ''})\n`)
	})

	// handle the motion:servoj event
	socket.on('motion:servoj', ({ q, a, v, t, lookahead_time, gain }) => {
		socket.data.robot.send(`servoj([${q}], a=${a}, v=${v}${t ? `, t=${t}` : ''}${lookahead_time ? `, lookahead_time=${lookahead_time}` : ''}${gain ? `, gain=${gain}` : ''})\n`)
	})

	// handle the motion:set_conveyor_tick_count event
	socket.on('motion:set_conveyor_tick_count', ({ tick_count, absolute_encoder_resolution }) => {
		socket.data.robot.send(`set_conveyor_tick_count(${tick_count}${absolute_encoder_resolution ? `, absolute_encoder_resolution=${absolute_encoder_resolution}` : ''})\n`)
	})

	// handle the motion:set_pos event
	socket.on('motion:set_pos', (q) => {
		socket.data.robot.send(`set_pos([${q}])\n`)
	})

	// handle the motion:set_safety_mode_transition_hardness event
	socket.on('motion:set_safety_mode_transition_hardness', (type) => {
		socket.data.robot.send(`set_safety_mode_transition_hardness(${type})\n`)
	})

	// handle the motion:speedj event
	socket.on('motion:speedj', ({ qd, a, t }) => {
		socket.data.robot.send(`speedj([${qd}],a=${a}${t ? `,t=${t}` : ''})\n`)
	})

	// handle the motion:speedl event
	socket.on('motion:speedl', ({ xd, a, t, aRot }) => {
		socket.data.robot.send(`speedl([${xd}],a=${a}${t && `,t=${t}`}${aRot ? `,aRot=${aRot}` : ''})\n`)
	})

	// handle the motion:stop_conveyor_tracking event
	socket.on('motion:stop_conveyor_tracking', (a) => {
		socket.data.robot.send(`stop_conveyor_tracking(${a ? `a=[${a}]` : ''})\n`)
	})

	// handle the motion:stopj event
	socket.on('motion:stopj', (a) => {
		socket.data.robot.send(`stopj(${a})\n`)
	})

	// handle the motion:stopl event
	socket.on('motion:stopl', ({ a, aRot }) => {
		socket.data.robot.send(`stopl(${a}${aRot ? `,aRot=${aRot}` : ''})\n`)
	})

	// handle the motion:teach_mode event
	socket.on('motion:teach_mode', () => {
		socket.data.robot.send(`teach_mode()\n`)
	})

	// handle the motion:track_conveyor_circular event
	socket.on('motion:track_conveyor_circular', ({ center, ticksPerRevolution, rotateTool, encoderIndex }) => {
		socket.data.robot.send(`track_conveyor_circular([${center}], ${ticksPerRevolution}, ${rotateTool}${encoderIndex ? `, encoder_index=${encoderIndex}` : ''})\n`)
	})

	// handle the motion:track_conveyor_linear event
	socket.on('motion:track_conveyor_linear', ({ direction, ticksPerMeter, encoderIndex }) => {
		socket.data.robot.send(`track_conveyor_linear([${direction}], ${ticksPerMeter}${encoderIndex ? `, encoder_index=${encoderIndex}` : ''})\n`)
	})

}