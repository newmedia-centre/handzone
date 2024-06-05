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
		header: () => <div className='w-24 p-2 text-left'>Level</div>,
		cell: info => <div className='w-24 p-2'>{info.getValue()}</div>,
		footer: info => (
			<div className='w-24 p-2'>
				<SelectFilter column={info.column} />
			</div>
		),
		enableColumnFilter: true,
		filterFn: 'arrIncludesSome',
	}),
	columnHelper.accessor('label', {
		header: () => <div className='w-72 p-2 text-left'>Label</div>,
		cell: info => <div className='w-72 p-2'>{info.getValue()}</div>,
		footer: info => (
			<div className='w-72 p-2'>
				<SelectFilter column={info.column} />
			</div>
		),
		enableColumnFilter: true,
		filterFn: 'arrIncludesSome',
	}),
	columnHelper.accessor('category', {
		header: () => <div className='w-36 p-2 text-left'>Category</div>,
		cell: info => <div className='w-36 p-2'>{info.getValue()}</div>,
		footer: info => (
			<div className='w-36 p-2'>
				<SelectFilter column={info.column} />
			</div>
		),
		enableColumnFilter: true,
		filterFn: 'arrIncludesSome',
	}),
	columnHelper.accessor('entity', {
		header: () => <div className='w-36 p-2 text-left'>Entity</div>,
		cell: info => <div className='w-36 p-2'>{info.getValue()}</div>,
		footer: info => (
			<div className='w-36 p-2'>
				<SelectFilter column={info.column} />
			</div>
		),
		enableColumnFilter: true,
		filterFn: 'arrIncludesSome',
	}),
	columnHelper.accessor('message', {
		header: () => <div className='grow p-2 text-left'>Message</div>,
		cell: info => <div className='grow p-2'>{info.getValue()}</div>,
		footer: () => <div className='grow p-2'></div>
	}),
	columnHelper.accessor('timestamp', {
		header: () => <div className='w-56 p-2 text-left'>Timestamp</div>,
		cell: info => <div className='w-56 p-2'>{moment.utc(info.getValue()).format('DD-MM-YYYY HH:mm:ss:SSS')}</div>,
		footer: () => <div className='grow p-2'></div>
	}),
	columnHelper.accessor('raw', {
		id: 'info',
		header: () => <div className='w-48 p-2 text-center'>More Info</div>,
		cell: info => (
			<div className='w-48 cursor-pointer p-2 text-center'>
				<InfoPopup data={info.getValue()} />
			</div>
		),
		footer: row => (
			<div className='flex w-48 items-center justify-center gap-2 p-2'>
				<button className='disabled:text-400' onClick={() => row.table.firstPage()} disabled={!row.table.getCanPreviousPage()}>First</button>
				<button className='disabled:text-400' onClick={() => row.table.previousPage()} disabled={!row.table.getCanPreviousPage()}>Prev</button>
				<button className='disabled:text-400' onClick={() => row.table.nextPage()} disabled={!row.table.getCanNextPage()}>Next</button>
				<button className='disabled:text-400' onClick={() => row.table.lastPage()} disabled={!row.table.getCanNextPage()}>Last</button>
			</div>
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
		<div className='flex w-full flex-col overflow-hidden'>
			<div className='flex shrink-0 flex-col'>
				{table.getHeaderGroups().map(headerGroup => (
					<div key={headerGroup.id} className='flex border-b border-300 pr-4'>
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
					</div>
				))}
			</div>
			<div className='flex flex-col divide-y divide-300 overflow-y-scroll'>
				{table.getRowModel().rows.map(row => (
					<div key={row.id} className={`flex border-r border-300 hover:bg-50 ${getLevelColor(row.original.level)}`}>
						{row.getVisibleCells().map(cell => (
							<Fragment key={cell.id}>
								{flexRender(cell.column.columnDef.cell, cell.getContext())}
							</Fragment>
						))}
					</div>
				))}
			</div>
			<div className='flex shrink-0 flex-col'>
				{table.getFooterGroups().map(footerGroup => (
					<div key={footerGroup.id} className='flex items-center border-t border-300 pr-4'>
						{footerGroup.headers.map(header => (
							<Fragment key={header.id}>
								{flexRender(header.column.columnDef.footer, header.getContext())}
							</Fragment>
						))}
					</div>
				))}
			</div>
		</div>
	)
}

// create a select filter
const SelectFilter = ({ column }: { column: Column<LogData, unknown> }) => {
	const columnFilterValue = column.getFilterValue() as string[]

	return (
		<Listbox value={columnFilterValue} onChange={value => column.setFilterValue(value)} multiple>
			<ListboxButton className='inline-flex w-full items-center justify-between rounded border px-2 py-1 text-center capitalize hover:bg-200'>{column.id} {columnFilterValue ? `[${columnFilterValue.length}]` : ''}</ListboxButton>
			<ListboxOptions anchor='top start' className='z-30 mb-1 overflow-x-hidden rounded border border-300 bg-white text-center'>
				{[...column.getFacetedUniqueValues().keys()].map((x) => (
					<ListboxOption key={x} value={x} className='cursor-pointer p-2 capitalize text-400 hover:bg-200 data-[selected]:text-black'>
						{x}
					</ListboxOption>
				))}
			</ListboxOptions>
		</Listbox>
	)
}