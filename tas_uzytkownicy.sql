CREATE TABLE `uzytkownicy` (
	`id_uzytkownika` INT(11) NOT NULL AUTO_INCREMENT,
	`login` VARCHAR(32) NOT NULL,
	`haslo` VARCHAR(64) NOT NULL,
	`email` VARCHAR(128) NOT NULL,
	PRIMARY KEY (`id_uzytkownika`),
	UNIQUE INDEX `login_UNIQUE` (`login`),
	UNIQUE INDEX `email_UNIQUE` (`email`)
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;
