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

// export page
export default async function Page() {
    return (
        <div className='flex grow overflow-y-auto'>
            <main className='container mx-auto flex grow gap-8'>
                <div className='mt-16 flex flex-1 shrink-0 grow flex-col'>
                    <div className='flex w-full items-center justify-center overflow-hidden rounded-xl border border-300 bg-white text-400 shadow-md'>
                        <img src='/cover.jpg' />
                    </div>
                </div>
                <div className='flex flex-1 shrink-0 grow flex-col'>
                    <div className='flex flex-col gap-4 py-16'>
                        <p>HANDZONe tutorials are currently hosted on DigiPedia. They introduce you to the basic features of robot anatomy, the different types of robot movements, waypoints, toolpaths, frames, and end effector setups. They also introduce how to create simple programs in PolyScope and more complex programs using the Robots plug-in in Rhino/Grasshopper for pick-and-place operations.</p>
                        <p>After following these tutorials, you may practice with the virtual robot in HANDZONe or visit <a href="https://www.tudelft.nl/bk/onderzoek/bk-labs/lama" target="_blank" className="text-blue-400 hover:underline">LAMA</a> (Laboratory for Additive Manufacturing in Architecture) at TU Delft Faculty of Architecture and the Built Environment to develop hands-on skills.</p>
                        <a href="https://digipedia.tudelft.nl/subject/ur5-tutorials/?tab=chapter-0" target="_blank" className="text-blue-400 hover:underline">Click here to access the tutorials on DigiPedia.</a>
                    </div>
                </div>
            </main>
        </div>
    )
}
