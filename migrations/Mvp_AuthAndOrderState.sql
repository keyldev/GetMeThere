CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;
DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230805125517_Initial Migration') THEN

    ALTER DATABASE CHARACTER SET utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230805125517_Initial Migration') THEN

    CREATE TABLE `Users` (
        `Id` char(36) COLLATE ascii_general_ci NOT NULL,
        `Login` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Password` longtext CHARACTER SET utf8mb4 NOT NULL,
        `Name` longtext CHARACTER SET utf8mb4 NOT NULL,
        CONSTRAINT `PK_Users` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230805125517_Initial Migration') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230805125517_Initial Migration', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230809135215_RefreshTokens_Table') THEN

    CREATE TABLE `RefreshTokens` (
        `TokenId` int NOT NULL AUTO_INCREMENT,
        `UserId` char(36) COLLATE ascii_general_ci NOT NULL,
        `TokenString` longtext CHARACTER SET utf8mb4 NULL,
        `ExpiryTime` datetime(6) NOT NULL,
        CONSTRAINT `PK_RefreshTokens` PRIMARY KEY (`TokenId`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230809135215_RefreshTokens_Table') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230809135215_RefreshTokens_Table', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230812190254_Taxi tables') THEN

    CREATE TABLE `Vehicles` (
        `VehicleId` char(36) COLLATE ascii_general_ci NOT NULL,
        `Number` longtext CHARACTER SET utf8mb4 NULL,
        `Model` longtext CHARACTER SET utf8mb4 NULL,
        `SeatsCount` int NOT NULL,
        `Color` longtext CHARACTER SET utf8mb4 NULL,
        `HaveChildSeat` tinyint(1) NOT NULL,
        `Type` int NOT NULL,
        CONSTRAINT `PK_Vehicles` PRIMARY KEY (`VehicleId`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230812190254_Taxi tables') THEN

    CREATE TABLE `TaxiDrivers` (
        `DriverId` char(36) COLLATE ascii_general_ci NOT NULL,
        `UserId` char(36) COLLATE ascii_general_ci NOT NULL,
        `VehicleId` char(36) COLLATE ascii_general_ci NOT NULL,
        CONSTRAINT `PK_TaxiDrivers` PRIMARY KEY (`DriverId`),
        CONSTRAINT `FK_TaxiDrivers_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_TaxiDrivers_Vehicles_VehicleId` FOREIGN KEY (`VehicleId`) REFERENCES `Vehicles` (`VehicleId`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230812190254_Taxi tables') THEN

    CREATE TABLE `TaxiOrders` (
        `OrderId` char(36) COLLATE ascii_general_ci NOT NULL,
        `ClientId` char(36) COLLATE ascii_general_ci NOT NULL,
        `ClientName` longtext CHARACTER SET utf8mb4 NULL,
        `PickupAddress` longtext CHARACTER SET utf8mb4 NULL,
        `PickupLatitude` double NOT NULL,
        `PickupLongitude` double NOT NULL,
        `DestinationAddress` longtext CHARACTER SET utf8mb4 NULL,
        `DestinationLatitude` double NOT NULL,
        `DestinationLongitude` double NOT NULL,
        `VehicleType` int NOT NULL,
        `PickupTime` datetime(6) NOT NULL,
        `OrderPrice` float NOT NULL,
        `IsCompleted` tinyint(1) NOT NULL,
        `IsConfirmed` tinyint(1) NOT NULL,
        `NeedChildSeat` tinyint(1) NOT NULL,
        `SeatsCount` int NOT NULL,
        `DriverId` char(36) COLLATE ascii_general_ci NULL,
        `TaxiDriverDriverId` char(36) COLLATE ascii_general_ci NULL,
        CONSTRAINT `PK_TaxiOrders` PRIMARY KEY (`OrderId`),
        CONSTRAINT `FK_TaxiOrders_TaxiDrivers_TaxiDriverDriverId` FOREIGN KEY (`TaxiDriverDriverId`) REFERENCES `TaxiDrivers` (`DriverId`)
    ) CHARACTER SET=utf8mb4;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230812190254_Taxi tables') THEN

    CREATE INDEX `IX_TaxiDrivers_UserId` ON `TaxiDrivers` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230812190254_Taxi tables') THEN

    CREATE INDEX `IX_TaxiDrivers_VehicleId` ON `TaxiDrivers` (`VehicleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230812190254_Taxi tables') THEN

    CREATE INDEX `IX_TaxiOrders_TaxiDriverDriverId` ON `TaxiOrders` (`TaxiDriverDriverId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230812190254_Taxi tables') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230812190254_Taxi tables', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230814193627_User_Table_Updates') THEN

    ALTER TABLE `Users` ADD `ConnectionId` longtext CHARACTER SET utf8mb4 NOT NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20230814193627_User_Table_Updates') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20230814193627_User_Table_Updates', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    ALTER TABLE `TaxiOrders` DROP COLUMN `IsCompleted`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    ALTER TABLE `TaxiOrders` DROP COLUMN `IsConfirmed`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    ALTER TABLE `Users` RENAME COLUMN `Password` TO `PasswordHash`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    ALTER TABLE `Users` MODIFY COLUMN `ConnectionId` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    ALTER TABLE `Users` ADD `FcmDeviceToken` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    ALTER TABLE `Users` ADD `PhoneNumber` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    ALTER TABLE `TaxiOrders` ADD `AssignedAt` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    ALTER TABLE `TaxiOrders` ADD `DriverArrivedAt` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    ALTER TABLE `TaxiOrders` ADD `EstimatedArrival` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    ALTER TABLE `TaxiOrders` ADD `RoutePolyline` longtext CHARACTER SET utf8mb4 NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    ALTER TABLE `TaxiOrders` ADD `Status` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    ALTER TABLE `TaxiDrivers` ADD `CurrentLatitude` double NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    ALTER TABLE `TaxiDrivers` ADD `CurrentLongitude` double NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    ALTER TABLE `TaxiDrivers` ADD `LastLocationUpdate` datetime(6) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    ALTER TABLE `TaxiDrivers` ADD `Status` int NOT NULL DEFAULT 0;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20260427180752_Mvp_AuthAndOrderState') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20260427180752_Mvp_AuthAndOrderState', '9.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

