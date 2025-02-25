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
        <main className="container mx-auto flex grow gap-8">
            <div className="flex flex-1 shrink-0 grow flex-col items-center justify-center">
                <div className="flex h-2/3 w-full items-center justify-center rounded-xl border border-300 bg-white text-400 shadow-md">
                    Picture
                </div>
            </div>
            <div className="flex flex-1 shrink-0 grow flex-col items-center justify-center">
                <div className="flex w-2/3 flex-col gap-4">
                    <h2 className="text-4xl font-light">About HANDZONe</h2>
                    <p>
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent sit amet turpis libero. Aenean
                        molestie euismod purus sed faucibus. Pellentesque gravida augue vel mi luctus tincidunt.
                        Phasellus semper mauris convallis lectus porta consectetur. Duis faucibus consectetur nibh a
                        ullamcorper. Pellentesque non tortor lacus. Ut vitae sem nec eros gravida dictum sed a lectus.
                        Nullam congue sit amet massa et imperdiet. Proin efficitur nulla id arcu luctus dapibus.
                        Maecenas euismod nisl at tellus pulvinar imperdiet eu vel elit. Fusce tempus et nulla ac
                        placerat.
                    </p>
                </div>
            </div>
        </main>
    )
}
