CREATE TABLE `przedmiot` (
	`id_przedmiotu` INT(11) NOT NULL AUTO_INCREMENT,
	`id_kategorii` INT(11) NULL DEFAULT NULL,
	`nazwa` VARCHAR(45) NOT NULL,
	`cena` DECIMAL(10,2) NULL DEFAULT NULL,
	PRIMARY KEY (`id_przedmiotu`),
	UNIQUE INDEX `nazwa_UNIQUE` (`nazwa`),
	INDEX `fk_kategoria_przedmiotu_idx` (`id_kategorii`),
	CONSTRAINT `fk_kategoria_przedmiotu` FOREIGN KEY (`id_kategorii`) REFERENCES `kategoria` (`id`)
)
COLLATE='latin1_swedish_ci'
ENGINE=InnoDB
AUTO_INCREMENT=3
;
