-- DropForeignKey
ALTER TABLE "Availability" DROP CONSTRAINT "Availability_robotId_fkey";

-- DropForeignKey
ALTER TABLE "RobotSession" DROP CONSTRAINT "RobotSession_robotId_fkey";

-- DropForeignKey
ALTER TABLE "RobotSessionRequest" DROP CONSTRAINT "RobotSessionRequest_sessionId_fkey";

-- DropForeignKey
ALTER TABLE "RobotSessionRequest" DROP CONSTRAINT "RobotSessionRequest_userId_fkey";

-- AddForeignKey
ALTER TABLE "Availability" ADD CONSTRAINT "Availability_robotId_fkey" FOREIGN KEY ("robotId") REFERENCES "Robot"("id") ON DELETE CASCADE ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "RobotSession" ADD CONSTRAINT "RobotSession_robotId_fkey" FOREIGN KEY ("robotId") REFERENCES "Robot"("id") ON DELETE CASCADE ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "RobotSessionRequest" ADD CONSTRAINT "RobotSessionRequest_sessionId_fkey" FOREIGN KEY ("sessionId") REFERENCES "RobotSession"("id") ON DELETE CASCADE ON UPDATE CASCADE;

-- AddForeignKey
ALTER TABLE "RobotSessionRequest" ADD CONSTRAINT "RobotSessionRequest_userId_fkey" FOREIGN KEY ("userId") REFERENCES "User"("id") ON DELETE CASCADE ON UPDATE CASCADE;
