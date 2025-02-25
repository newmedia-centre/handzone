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
                        <h2 className='text-4xl font-light'>About HANDZONe</h2>
                        <div className='flex flex-col gap-2'>
                            <p>HANDZONe is a Hybrid Learning Environment developed to support education on robotics, specifically within architecture and built environment studies. It features a virtual lab with tutorials and exercises introducing fundamental knowledge and skills for programming and operating a robotic arm. It utilizes the Unity engine to create a digital twin of the robotic arm and a web server to enable real-time communication between the physical and digital labs.</p>
                            <p>The users can use HANDZONe (with or without a VR headset) to follow tutorials, practice exercises, and upload and run their programs on the virtual robot. They can also use it to connect to the physical robot to control and monitor its operation in real time remotely.</p>
                            <p>HANDZONe is available to use for anyone with a SURFconext ID. It is also available open-source on GitHub.</p>
                        </div>
                        <div className='flex flex-col gap-2 text-sm'>
                            <p>This project was supported by the Open and Online Education Incentive Scheme of SURF (Project Code: OO22-13) between 01.09.2022 and 31.12.2024. It was developed in collaboration between the Faculty of Architecture and the Built Environment and the XR Zone at Delft University of Technology.</p>
                            <span>Project coordinator: Serdar Aşut</span>
                            <span>Project team: Arno Freeke, Friso Gouwetor, Luuk Goossen, Roland van Roijen, Sharif Bayoumy, Yosua Pranata Andoko</span>
                            <span>Student assistants: Jirri van den Bos, Timothy Zonnenberg</span>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    )
}
