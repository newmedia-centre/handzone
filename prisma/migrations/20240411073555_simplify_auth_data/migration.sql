/*
  Warnings:

  - You are about to drop the column `code` on the `User` table. All the data in the column will be lost.
  - You are about to drop the column `emailVerified` on the `User` table. All the data in the column will be lost.
  - You are about to drop the column `role` on the `User` table. All the data in the column will be lost.
  - You are about to drop the column `uid` on the `User` table. All the data in the column will be lost.

*/
-- DropIndex
DROP INDEX "User_uid_code_key";

-- AlterTable
ALTER TABLE "User" DROP COLUMN "code",
DROP COLUMN "emailVerified",
DROP COLUMN "role",
DROP COLUMN "uid";
