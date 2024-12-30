CREATE TABLE `nsb_ordersaga` (
    `Id` CHAR(36) NOT NULL,                     -- Saga Id (Primary Key)
    `PersistenceVersion` VARCHAR(23) NOT NULL, -- NServiceBus Persistence Version
    `SagaTypeVersion` VARCHAR(255) NOT NULL,   -- Saga Type Version
    `Data` JSON NOT NULL,                       -- Serialized Saga Data
    `Metadata` JSON NOT NULL,                   -- Metadata for Saga
    `OrderId` CHAR(36),                         -- Saga Data: OrderId
    `UserId` CHAR(36),                          -- Saga Data: UserId
    `PaymentId` CHAR(36),                       -- Saga Data: PaymentId
    `Name` VARCHAR(255),                        -- Saga Data: Name
    `Email` VARCHAR(255),                       -- Saga Data: Email
    `Password` VARCHAR(255),                    -- Saga Data: Password
    `Street` VARCHAR(255),                      -- Saga Data: Street
    `City` VARCHAR(255),                        -- Saga Data: City
    `PostalCode` VARCHAR(50),                   -- Saga Data: PostalCode
    `Concurrency` BIGINT NOT NULL DEFAULT 1,    -- Concurrency column
    `Correlation_OrderId` CHAR(36) NULL,        -- Correlation column
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_OrderSaga_OrderId` (`OrderId`) -- Index for finding saga by OrderId
) ENGINE=InnoDB;

ALTER TABLE Orders MODIFY COLUMN `Reason` VARCHAR(255) DEFAULT '';
ALTER TABLE Orders MODIFY COLUMN `Status` VARCHAR(255) DEFAULT '';
