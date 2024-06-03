-- CreateEnum
CREATE TYPE "RobotType" AS ENUM ('URSIM_CB3_3_15_8');

-- CreateTable
CREATE TABLE "Robot" (
    "id" TEXT NOT NULL,
    "name" TEXT NOT NULL,
    "type" "RobotType" NOT NULL,

    CONSTRAINT "Robot_pkey" PRIMARY KEY ("id")
);
