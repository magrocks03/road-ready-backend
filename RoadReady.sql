/*
================================================================
RoadReadyDB - SELECT Scripts for All Tables (Current Schema)
================================================================
Use these commands in SQL Server Management Studio (SSMS) to 
view the data in your database.
*/

USE RoadReadyDB;
GO

-- 1. Master Tables (Lookup Data)
-- ================================================================
PRINT '--- Roles ---';
SELECT * FROM Roles;
GO

PRINT '--- Brands ---';
SELECT * FROM Brands;
GO

PRINT '--- BookingStatuses ---';
SELECT * FROM BookingStatuses;
GO

PRINT '--- Locations ---';
SELECT * FROM Locations;
GO

PRINT '--- Extras ---';
SELECT * FROM Extras;
GO


-- 2. Transactional & User Management Tables
-- ================================================================
PRINT '--- Users ---';
SELECT * FROM Users;
GO

PRINT '--- Vehicles ---';
SELECT * FROM Vehicles;
GO

PRINT '--- Bookings ---';
SELECT * FROM Bookings;
GO

PRINT '--- Payments ---';
SELECT * FROM Payments;
GO

PRINT '--- Reviews ---';
SELECT * FROM Reviews;
GO

PRINT '--- Issues ---';
SELECT * FROM Issues;
GO

PRINT '--- Refunds ---';
SELECT * FROM Refunds;
GO


-- 3. Junction Tables (Linking Tables)
-- ================================================================
PRINT '--- UserRoles ---';
SELECT * FROM UserRoles;
GO

PRINT '--- BookingExtras ---';
SELECT * FROM BookingExtras;
GO



--  ======================================================================================================



-- To Activate the Rental Period



USE RoadReadyDB;
GO

-- IMPORTANT: Replace the '0' below with the actual bookingId you got from Postman.
DECLARE @BookingIdToActivate INT = "REPLACE"; 

-- This script updates the booking to be active right now for testing purposes.
UPDATE Bookings
SET 
    StartDate = DATEADD(day, -1, GETUTCDATE()), -- Sets the start date to yesterday
    EndDate = DATEADD(day, 2, GETUTCDATE())   -- Sets the end date to 2 days from now
WHERE 
    Id = @BookingIdToActivate;

PRINT 'Booking dates updated to make it active.';

GO
