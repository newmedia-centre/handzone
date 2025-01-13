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

// import components
import Image from 'next/image'

export const Footer = () => (
    <footer className='flex shrink-0 items-center justify-center gap-8 border-t border-300 bg-white p-4 shadow-md'>
        <a href='https://newmediacentre.tudelft.nl'>
            <Image src='/logo-bk.png' alt='TU Delft Architecture and the Built Environment Logo' width={256} height={32} priority />
        </a>
        <a href='https://newmediacentre.tudelft.nl'>
            <Image src='/logo-nmc.png' alt='NewMedia Centre Logo' width={256} height={32} priority />
        </a>
        <a href='https://newmediacentre.tudelft.nl/xr'>
            <Image src='/logo-xr.png' alt='XR-Zone Logo' width={128} height={32} priority />
        </a>
    </footer>
)
