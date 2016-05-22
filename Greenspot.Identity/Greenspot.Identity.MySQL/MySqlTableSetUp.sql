

--
-- Table structure for table `greenspot_roles`
--

CREATE TABLE IF NOT EXISTS `greenspot_roles` (
  `Id` varchar(128) NOT NULL,
  `Name` varchar(256) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=UTF8;

-- --------------------------------------------------------

--
-- Table structure for table `greenspot_user_claims`
--

CREATE TABLE IF NOT EXISTS `greenspot_user_claims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` varchar(128) NOT NULL,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id` (`Id`),
  KEY `UserId` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=UTF8 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- Table structure for table `greenspot_user_logins`
--

CREATE TABLE IF NOT EXISTS `greenspot_user_logins` (
  `LoginProvider` varchar(128) NOT NULL,
  `ProviderKey` varchar(128) NOT NULL,
  `UserId` varchar(128) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`,`UserId`),
  KEY `ApplicationUser_Logins` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=UTF8;

-- --------------------------------------------------------

--
-- Table structure for table `greenspot_user_roles`
--

CREATE TABLE IF NOT EXISTS `greenspot_user_roles` (
  `UserId` varchar(128) NOT NULL,
  `RoleId` varchar(128) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IdentityRole_Users` (`RoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=UTF8;

-- --------------------------------------------------------

--
-- Table structure for table `greenspot_users`
--

CREATE TABLE IF NOT EXISTS `greenspot_users` (
  `Id` varchar(128) NOT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext,
  `SecurityStamp` longtext,
  `PhoneNumber` longtext,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEndDateUtc` datetime DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int(11) NOT NULL,
  `UserName` varchar(256),
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=UTF8;

-- --------------------------------------------------------
--
-- Table structure for table `greenspot_user_sns_info`
--

CREATE TABLE IF NOT EXISTS `greenspot_user_snsinfos` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` varchar(128) NOT NULL,
  `SnsName` varchar(128),
  `InfoKey` varchar(256),
  `InfoValue` longtext,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id` (`Id`),
  KEY `UserId` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=UTF8 AUTO_INCREMENT=1 ;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `greenspot_user_claims`
--
ALTER TABLE `greenspot_user_claims`
  ADD CONSTRAINT `GreenspotUser_Claims` FOREIGN KEY (`UserId`) REFERENCES `greenspot_users` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Constraints for table `greenspot_user_logins`
--
ALTER TABLE `greenspot_user_logins`
  ADD CONSTRAINT `GreenspotUser_Logins` FOREIGN KEY (`UserId`) REFERENCES `greenspot_users` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION;

--
-- Constraints for table `greenspot_user_roles`
--
ALTER TABLE `greenspot_user_roles`
  ADD CONSTRAINT `GreenspotUser_Roles` FOREIGN KEY (`UserId`) REFERENCES `greenspot_users` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  ADD CONSTRAINT `GreenspotRole_Users` FOREIGN KEY (`RoleId`) REFERENCES `greenspot_roles` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION;

-- Constraints for table `greenspot_user_sns_info`
--
ALTER TABLE `greenspot_user_snsinfos`
  ADD CONSTRAINT `GreenspotUser_SnsInfos` FOREIGN KEY (`UserId`) REFERENCES `greenspot_users` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION;