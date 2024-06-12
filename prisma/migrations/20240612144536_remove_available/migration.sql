/*
  Warnings:

  - The values [AVAILABLE] on the enum `RequestStatus` will be removed. If these variants are still used in the database, this will fail.

*/
-- AlterEnum
BEGIN;
CREATE TYPE "RequestStatus_new" AS ENUM ('REQUESTED', 'ACCEPTED', 'REJECTED');
ALTER TABLE "RobotSessionRequest" ALTER COLUMN "status" DROP DEFAULT;
ALTER TABLE "RobotSessionRequest" ALTER COLUMN "status" TYPE "RequestStatus_new" USING ("status"::text::"RequestStatus_new");
ALTER TYPE "RequestStatus" RENAME TO "RequestStatus_old";
ALTER TYPE "RequestStatus_new" RENAME TO "RequestStatus";
DROP TYPE "RequestStatus_old";
ALTER TABLE "RobotSessionRequest" ALTER COLUMN "status" SET DEFAULT 'REQUESTED';
COMMIT;
