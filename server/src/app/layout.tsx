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
import { Header } from "./header"
import { Footer } from "./footer"

// import styles
import { Inter } from "next/font/google"
import "./globals.css"

// import types
import type { Metadata } from "next"

// load the Inter font
const inter = Inter({ subsets: ["latin"] })

// export metadata for the website
export const metadata: Metadata = {
    title: "HANDZONe",
    description: "Control robots in VR",
}

// export common layout for the entire website
export default function RootLayout({
    children,
}: Readonly<{
    children: React.ReactNode
}>) {
    return (
        <html lang="en">
            <body
                className={
                    "flex flex-col size-full min-h-screen max-h-screen overflow-hidden mx-auto bg-100 " +
                    inter.className
                }
            >
                <Header />
                {children}
                <Footer />
            </body>
        </html>
    )
}
