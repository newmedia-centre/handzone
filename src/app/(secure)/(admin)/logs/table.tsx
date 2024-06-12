'use client'

// import dependencies
import { useState } from 'react'
import moment from 'moment'
import {
	useReactTable,
	createColumnHelper,
	getCoreRowModel,
	getFilteredRowModel,
	getPaginationRowModel,
	getFacetedUniqueValues,
	flexRender
} from '@tanstack/react-table'

// import components
import { Listbox, ListboxButton, ListboxOptions, ListboxOption } from '@headlessui/react'
import { Fragment } from 'react'
import { InfoPopup } from './popup'

// import types
import type { PaginationState, ColumnFiltersState, Column } from '@tanstack/react-table'

// create logs table type
export type LogData = {
	level: string
	label: string
	category: string
	entity: string
	message: string
	timestamp: string
	raw: string
}

// define the table columns
const columnHelper = createColumnHelper<LogData>()
const columns = [
	columnHelper.accessor('level', {
		header: () => <th className='w-24 border-b border-300 p-2 text-left'>Level</th>,
		cell: info => <td className='border-b border-300 p-2'>{info.getValue()}</td>,
		footer: info => (
			<th className='border-t border-300 p-2'>
				<SelectFilter column={info.column} className='w-20' />
			</th>
		),
		enableColumnFilter: true,
		filterFn: 'arrIncludesSome',
	}),
	columnHelper.accessor('label', {
		header: () => <th className='w-72 border-b border-300 p-2 text-left'>Label</th>,
		cell: info => <td className='border-b border-300 p-2'>{info.getValue()}</td>,
		footer: info => (
			<th className='border-t border-300 p-2'>
				<SelectFilter column={info.column} className='w-[17rem]' />
			</th>
		),
		enableColumnFilter: true,
		filterFn: 'arrIncludesSome',
	}),
	columnHelper.accessor('category', {
		header: () => <th className='w-36 border-b border-300 p-2 text-left'>Category</th>,
		cell: info => <td className='border-b border-300 p-2'>{info.getValue()}</td>,
		footer: info => (
			<th className='border-t border-300 p-2'>
				<SelectFilter column={info.column} className='w-32' />
			</th>
		),
		enableColumnFilter: true,
		filterFn: 'arrIncludesSome',
	}),
	columnHelper.accessor('entity', {
		header: () => <th className='w-36 border-b border-300 p-2 text-left'>Entity</th>,
		cell: info => <td className='border-b border-300 p-2'>{info.getValue()}</td>,
		footer: info => (
			<th className='border-t border-300 p-2'>
				<SelectFilter column={info.column} className='w-32' />
			</th>
		),
		enableColumnFilter: true,
		filterFn: 'arrIncludesSome',
	}),
	columnHelper.accessor('message', {
		header: () => <th className='border-b border-300 p-2 text-left'>Message</th>,
		cell: info => <td className='border-b border-300 p-2'>{info.getValue()}</td>,
		footer: () => <th className='border-t border-300 p-2'></th>
	}),
	columnHelper.accessor('timestamp', {
		header: () => <th className='w-56 border-b border-300 p-2 text-left'>Timestamp</th>,
		cell: info => <td className='border-b border-300 p-2'>{moment.utc(info.getValue()).format('DD-MM-YYYY HH:mm:ss:SSS')}</td>,
		footer: () => <th className='border-t border-300 p-2'></th>
	}),
	columnHelper.accessor('raw', {
		id: 'info',
		header: () => <th className='w-48 border-b border-300 p-2 text-center'>More Info</th>,
		cell: info => (
			<td className='w-48 cursor-pointer border-b border-300 p-2 text-center'>
				<InfoPopup data={info.getValue()} />
			</td>
		),
		footer: row => (
			<th className='border-t border-300'>
				<div className='flex items-center justify-center gap-2'>
					<button className='disabled:text-400' onClick={() => row.table.firstPage()} disabled={!row.table.getCanPreviousPage()}>First</button>
					<button className='disabled:text-400' onClick={() => row.table.previousPage()} disabled={!row.table.getCanPreviousPage()}>Prev</button>
					<button className='disabled:text-400' onClick={() => row.table.nextPage()} disabled={!row.table.getCanNextPage()}>Next</button>
					<button className='disabled:text-400' onClick={() => row.table.lastPage()} disabled={!row.table.getCanNextPage()}>Last</button>
				</div>
			</th>
		)
	}),
]

const getLevelColor = (level: string) => {
	switch (level) {
		case 'error':
			return 'text-red-500'
		case 'warn':
			return 'text-yellow-500'
		case 'http':
			return 'text-500'
		default:
			return ''
	}
}

export const LogTable = ({ data }: { data: LogData[] }) => {
	const [pagination, setPagination] = useState<PaginationState>({
		pageIndex: 0,
		pageSize: 1000,
	})
	const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>(
		[]
	)

	// create the table
	const table = useReactTable({
		data,
		columns,
		state: {
			pagination,
			columnFilters,
		},
		enableFilters: true,
		getCoreRowModel: getCoreRowModel(),
		getFilteredRowModel: getFilteredRowModel(),
		getPaginationRowModel: getPaginationRowModel(),
		getFacetedUniqueValues: getFacetedUniqueValues(),
		onPaginationChange: setPagination,
		onColumnFiltersChange: setColumnFilters,
	})

	return (
		<table className='table w-full table-fixed border-separate border-spacing-0'>
			<thead className='sticky top-0 bg-white'>
				{table.getHeaderGroups().map(headerGroup => (
					<tr key={headerGroup.id} className='pr-4'>
						{headerGroup.headers.map(header => (
							<Fragment key={header.id}>
								{header.isPlaceholder
									? null
									: flexRender(
										header.column.columnDef.header,
										header.getContext()
									)}
							</Fragment>
						))}
					</tr>
				))}
			</thead>
			<tbody className='divide-y divide-300'>
				{table.getRowModel().rows.map(row => (
					<tr key={row.id} className={`border-r border-300 hover:bg-50 ${getLevelColor(row.original.level)}`}>
						{row.getVisibleCells().map(cell => (
							<Fragment key={cell.id}>
								{flexRender(cell.column.columnDef.cell, cell.getContext())}
							</Fragment>
						))}
					</tr>
				))}
			</tbody>
			<tfoot className='sticky bottom-0 bg-white'>
				{table.getFooterGroups().map(footerGroup => (
					<tr key={footerGroup.id} className='border-t border-300 pr-4'>
						{footerGroup.headers.map(header => (
							<Fragment key={header.id}>
								{flexRender(header.column.columnDef.footer, header.getContext())}
							</Fragment>
						))}
					</tr>
				))}
			</tfoot>
		</table>
	)
}

// create a select filter
const SelectFilter = ({ column, className }: { column: Column<LogData, unknown>, className?: string }) => {
	const columnFilterValue = column.getFilterValue() as string[]

	return (
		<Listbox value={columnFilterValue} onChange={value => column.setFilterValue(value)} multiple>
			<ListboxButton className='inline-flex w-full items-center justify-between rounded border px-2 py-1 text-center capitalize hover:bg-200'>{column.id} {columnFilterValue ? `[${columnFilterValue.length}]` : ''}</ListboxButton>
			<ListboxOptions anchor='top start' className={'z-30 mb-1 overflow-x-hidden rounded border border-300 bg-white text-center ' + className}>
				{[...column.getFacetedUniqueValues().keys()].map((x) => (
					<ListboxOption key={x} value={x} className='cursor-pointer p-2 capitalize text-400 hover:bg-200 data-[selected]:text-black'>
						{x}
					</ListboxOption>
				))}
			</ListboxOptions>
		</Listbox>
	)
}