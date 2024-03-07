// import dependencies
import { robots } from '@/server/robot'

// import types
import type { Socket } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'
import type { RealtimeData } from '@/types/Socket/Realtime/RealtimeData'

export const handleRealtimeEvents = (socket: Socket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>) => {

	// forward the robot events
	const robot = robots.connections.get(socket.data.robot)
	if (!robot) return false

	// forward the raw realtime data
	robot.on('realtime:raw', (data) => {
		socket.emit('realtime:raw', data.toString('base64'))
	})

	// forward the parsed realtime data
	robot.on('realtime:parsed', (data) => {
		socket.emit('realtime:data', data)
	})

	return true

}

export const parseRealtimeData = async (data: Buffer): Promise<RealtimeData> => {
	return {
		message_size: data.readUInt32BE(0),
		time: data.readDoubleBE(4),
		q_target: [
			data.readDoubleBE(12),
			data.readDoubleBE(20),
			data.readDoubleBE(28),
			data.readDoubleBE(36),
			data.readDoubleBE(44),
			data.readDoubleBE(52)
		],
		qd_target: [
			data.readDoubleBE(60),
			data.readDoubleBE(68),
			data.readDoubleBE(76),
			data.readDoubleBE(84),
			data.readDoubleBE(92),
			data.readDoubleBE(100)
		],
		qdd_target: [
			data.readDoubleBE(108),
			data.readDoubleBE(116),
			data.readDoubleBE(124),
			data.readDoubleBE(132),
			data.readDoubleBE(140),
			data.readDoubleBE(148)
		],
		i_target: [
			data.readDoubleBE(156),
			data.readDoubleBE(164),
			data.readDoubleBE(172),
			data.readDoubleBE(180),
			data.readDoubleBE(188),
			data.readDoubleBE(196)
		],
		m_target: [
			data.readDoubleBE(204),
			data.readDoubleBE(212),
			data.readDoubleBE(220),
			data.readDoubleBE(228),
			data.readDoubleBE(236),
			data.readDoubleBE(244)
		],
		q_actual: [
			data.readDoubleBE(252),
			data.readDoubleBE(260),
			data.readDoubleBE(268),
			data.readDoubleBE(276),
			data.readDoubleBE(284),
			data.readDoubleBE(292)
		],
		qd_actual: [
			data.readDoubleBE(300),
			data.readDoubleBE(308),
			data.readDoubleBE(316),
			data.readDoubleBE(324),
			data.readDoubleBE(332),
			data.readDoubleBE(340)
		],
		i_actual: [
			data.readDoubleBE(348),
			data.readDoubleBE(356),
			data.readDoubleBE(364),
			data.readDoubleBE(372),
			data.readDoubleBE(380),
			data.readDoubleBE(388)
		],
		i_control: [
			data.readDoubleBE(396),
			data.readDoubleBE(404),
			data.readDoubleBE(412),
			data.readDoubleBE(420),
			data.readDoubleBE(428),
			data.readDoubleBE(436)
		],
		tool_vector_actual: [
			data.readDoubleBE(444),
			data.readDoubleBE(452),
			data.readDoubleBE(460),
			data.readDoubleBE(468),
			data.readDoubleBE(476),
			data.readDoubleBE(484)
		],
		tcp_speed_actual: [
			data.readDoubleBE(492),
			data.readDoubleBE(500),
			data.readDoubleBE(508),
			data.readDoubleBE(516),
			data.readDoubleBE(524),
			data.readDoubleBE(532)
		],
		tcp_force: [
			data.readDoubleBE(540),
			data.readDoubleBE(548),
			data.readDoubleBE(556),
			data.readDoubleBE(564),
			data.readDoubleBE(572),
			data.readDoubleBE(580)
		],
		tool_vector_target: [
			data.readDoubleBE(588),
			data.readDoubleBE(596),
			data.readDoubleBE(604),
			data.readDoubleBE(612),
			data.readDoubleBE(620),
			data.readDoubleBE(628)
		],
		tcp_speed_target: [
			data.readDoubleBE(636),
			data.readDoubleBE(644),
			data.readDoubleBE(652),
			data.readDoubleBE(660),
			data.readDoubleBE(668),
			data.readDoubleBE(676)
		],
		digital_input_bits: data.readDoubleBE(684),
		motor_temperatures: [
			data.readDoubleBE(692),
			data.readDoubleBE(700),
			data.readDoubleBE(708),
			data.readDoubleBE(716),
			data.readDoubleBE(724),
			data.readDoubleBE(732)
		],
		controller_timer: data.readDoubleBE(740),
		robot_mode: data.readDoubleBE(756),
		joint_modes: [
			data.readDoubleBE(764),
			data.readDoubleBE(772),
			data.readDoubleBE(780),
			data.readDoubleBE(788),
			data.readDoubleBE(796),
			data.readDoubleBE(804)
		],
		safety_mode: data.readDoubleBE(812),
		tool_accelerometer_values: [
			data.readDoubleBE(868),
			data.readDoubleBE(876),
			data.readDoubleBE(884)
		],
		speed_scaling: data.readDoubleBE(940),
		linear_momentum_norm: data.readDoubleBE(948),
		v_main: data.readDoubleBE(972),
		v_robot: data.readDoubleBE(980),
		i_robot: data.readDoubleBE(988),
		v_actual: [
			data.readDoubleBE(996),
			data.readDoubleBE(1004),
			data.readDoubleBE(1012),
			data.readDoubleBE(1020),
			data.readDoubleBE(1028),
			data.readDoubleBE(1036)
		],
		digital_outputs: data.readDoubleBE(1044),
		program_state: data.readDoubleBE(1052),
		elbow_position: [
			data.readDoubleBE(1060),
			data.readDoubleBE(1068),
			data.readDoubleBE(1076),
		],
		elbow_velocity: [
			data.readDoubleBE(1084),
			data.readDoubleBE(1092),
			data.readDoubleBE(1100),
		],
		safety_status: data.readDoubleBE(1108),
		payload_mass: data.readDoubleBE(1140),
		payload_cog: [
			data.readDoubleBE(1148),
			data.readDoubleBE(1156),
			data.readDoubleBE(1164),
		],
		payload_inertia: [
			data.readDoubleBE(1172),
			data.readDoubleBE(1180),
			data.readDoubleBE(1188),
			data.readDoubleBE(1196),
			data.readDoubleBE(1204),
			data.readDoubleBE(1212),
		]
	}
}