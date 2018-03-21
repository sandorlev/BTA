USE `bus-transportation-app`;

DROP TABLE IF EXISTS `bus`;
CREATE TABLE `bus` (
    `id` INTEGER AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(255) NOT NULL UNIQUE
);

DROP TABLE IF EXISTS `stop`;
CREATE TABLE `stop` (
    `id` INTEGER AUTO_INCREMENT PRIMARY KEY,
    `name` VARCHAR(255) NOT NULL UNIQUE,
    `latitude` FLOAT(10, 6) NOT NULL,
    `longitude` FLOAT(10, 6) NOT NULL
);

DROP TABLE IF EXISTS `route`;
CREATE TABLE `route` (
    `id` INTEGER AUTO_INCREMENT PRIMARY KEY,
    `bus_id` INTEGER FOREIGN KEY REFERENCES `bus` (`id`)
);

DROP TABLE IF EXISTS `route_element`;
CREATE TABLE `route_element` (
    `route_id` INTEGER FOREIGN KEY REFERENCES `route` (`id`),
    `stop_id` INTEGER FOREIGN KEY REFERENCES `stop` (`id`),
    `position` INTEGER NOT NULL,
    PRIMARY KEY (`route_id`, `stop_id`),
    UNIQUE (`route_id`, `stop_id`, `position`)
);
