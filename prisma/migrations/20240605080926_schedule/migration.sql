/*
  Warnings:

  - You are about to drop the `Admin` table. If the table is not empty, all the data it contains will be lost.
  - A unique constraint covering the columns `[name]` on the table `Robot` will be added. If there are existing duplicate values, this will fail.

*/
-- CreateEnum
CREATE TYPE "RequestStatus" AS ENUM ('REQUESTED', 'ACCEPTED', 'REJECTED', 'AVAILABLE');

-- DropForeignKey
ALTER TABLE "Admin" DROP CONSTRAINT "Admin_userId_fkey";

-- AlterTable
ALTER TABLE "Robot" ADD COLUMN     "active" BOOLEAN NOT NULL DEFAULT false;

-- AlterTable
ALTER TABLE "User" ADD COLUMN     "admin" BOOLEAN NOT NULL DEFAULT false;

-- DropTable
DROP TABLE "Admin";

-- CreateTable
CREATE TABLE "Availability" (
    "id" TEXT NOT NULL,
    "robotId" TEXT NOT NULL,
    "start" TIMESTAMP(3) NOT NULL,
    "end" TIMESTAMP(3) NOT NULL,

    CONSTRAINT "Availability_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "RobotSession" (
    "id" TEXT NOT NULL,
    "robotId" TEXT NOT NULL,
    "start" TIMESTAMP(3) NOT NULL,
    "end" TIMESTAMP(3) NOT NULL,

    CONSTRAINT "RobotSession_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "RobotSessionRequest" (
    "sessionId" TEXT NOT NULL,
    "userId" TEXT NOT NULL,
    "status" "RequestStatus" NOT NULL DEFAULT 'REQUESTED'
);

-- CreateIndex
CREATE UNIQUE INDEX "RobotSessionRequest_sessionId_userId_key" ON "RobotSessionRequest"("sessionId", "userId");

-- CreateIndex
CREATE UNIQUE INDEX "Robot_name_key" ON "Robot"("name");

-- AddForeignKey
ALTER TABLE "Availability" ADD CONSTRAINT "Availability_robotId_fkey" FOREIGN KEY ("robotId") REFERENCES "Robot"("id") ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "RobotSession" ADD CONSTRAINT "RobotSession_robotId_fkey" FOREIGN KEY ("robotId") REFERENCES "Robot"("id") ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "RobotSessionRequest" ADD CONSTRAINT "RobotSessionRequest_sessionId_fkey" FOREIGN KEY ("sessionId") REFERENCES "RobotSession"("id") ON DELETE RESTRICT ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "RobotSessionRequest" ADD CONSTRAINT "RobotSessionRequest_userId_fkey" FOREIGN KEY ("userId") REFERENCES "User"("id") ON DELETE RESTRICT ON UPDATE CASCADE;
