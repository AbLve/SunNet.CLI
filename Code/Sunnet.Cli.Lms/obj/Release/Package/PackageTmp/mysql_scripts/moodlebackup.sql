/*
SQLyog v10.2 
MySQL - 5.5.40 : Database - moodle
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`moodle` /*!40100 DEFAULT CHARACTER SET utf8 COLLATE utf8_unicode_ci */;

USE `moodle`;

/*Table structure for table `mdl_assign` */

DROP TABLE IF EXISTS `mdl_assign`;

CREATE TABLE `mdl_assign` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci NOT NULL,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `alwaysshowdescription` tinyint(2) NOT NULL DEFAULT '0',
  `nosubmissions` tinyint(2) NOT NULL DEFAULT '0',
  `submissiondrafts` tinyint(2) NOT NULL DEFAULT '0',
  `sendnotifications` tinyint(2) NOT NULL DEFAULT '0',
  `sendlatenotifications` tinyint(2) NOT NULL DEFAULT '0',
  `duedate` bigint(10) NOT NULL DEFAULT '0',
  `allowsubmissionsfromdate` bigint(10) NOT NULL DEFAULT '0',
  `grade` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `requiresubmissionstatement` tinyint(2) NOT NULL DEFAULT '0',
  `completionsubmit` tinyint(2) NOT NULL DEFAULT '0',
  `cutoffdate` bigint(10) NOT NULL DEFAULT '0',
  `teamsubmission` tinyint(2) NOT NULL DEFAULT '0',
  `requireallteammemberssubmit` tinyint(2) NOT NULL DEFAULT '0',
  `teamsubmissiongroupingid` bigint(10) NOT NULL DEFAULT '0',
  `blindmarking` tinyint(2) NOT NULL DEFAULT '0',
  `revealidentities` tinyint(2) NOT NULL DEFAULT '0',
  `attemptreopenmethod` varchar(10) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'none',
  `maxattempts` mediumint(6) NOT NULL DEFAULT '-1',
  `markingworkflow` tinyint(2) NOT NULL DEFAULT '0',
  `markingallocation` tinyint(2) NOT NULL DEFAULT '0',
  `sendstudentnotifications` tinyint(2) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `mdl_assi_cou_ix` (`course`),
  KEY `mdl_assi_tea_ix` (`teamsubmissiongroupingid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table saves information about an instance of mod_assign';

/*Data for the table `mdl_assign` */

/*Table structure for table `mdl_assign_grades` */

DROP TABLE IF EXISTS `mdl_assign_grades`;

CREATE TABLE `mdl_assign_grades` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `assignment` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `grader` bigint(10) NOT NULL DEFAULT '0',
  `grade` decimal(10,5) DEFAULT '0.00000',
  `attemptnumber` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_assigrad_assuseatt_uix` (`assignment`,`userid`,`attemptnumber`),
  KEY `mdl_assigrad_use_ix` (`userid`),
  KEY `mdl_assigrad_att_ix` (`attemptnumber`),
  KEY `mdl_assigrad_ass_ix` (`assignment`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Grading information about a single assignment submission.';

/*Data for the table `mdl_assign_grades` */

/*Table structure for table `mdl_assign_plugin_config` */

DROP TABLE IF EXISTS `mdl_assign_plugin_config`;

CREATE TABLE `mdl_assign_plugin_config` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `assignment` bigint(10) NOT NULL DEFAULT '0',
  `plugin` varchar(28) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `subtype` varchar(28) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `name` varchar(28) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `mdl_assiplugconf_plu_ix` (`plugin`),
  KEY `mdl_assiplugconf_sub_ix` (`subtype`),
  KEY `mdl_assiplugconf_nam_ix` (`name`),
  KEY `mdl_assiplugconf_ass_ix` (`assignment`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Config data for an instance of a plugin in an assignment.';

/*Data for the table `mdl_assign_plugin_config` */

/*Table structure for table `mdl_assign_submission` */

DROP TABLE IF EXISTS `mdl_assign_submission`;

CREATE TABLE `mdl_assign_submission` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `assignment` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `status` varchar(10) COLLATE utf8_unicode_ci DEFAULT NULL,
  `groupid` bigint(10) NOT NULL DEFAULT '0',
  `attemptnumber` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_assisubm_assusegroatt_uix` (`assignment`,`userid`,`groupid`,`attemptnumber`),
  KEY `mdl_assisubm_use_ix` (`userid`),
  KEY `mdl_assisubm_att_ix` (`attemptnumber`),
  KEY `mdl_assisubm_ass_ix` (`assignment`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table keeps information about student interactions with';

/*Data for the table `mdl_assign_submission` */

/*Table structure for table `mdl_assign_user_flags` */

DROP TABLE IF EXISTS `mdl_assign_user_flags`;

CREATE TABLE `mdl_assign_user_flags` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `assignment` bigint(10) NOT NULL DEFAULT '0',
  `locked` bigint(10) NOT NULL DEFAULT '0',
  `mailed` smallint(4) NOT NULL DEFAULT '0',
  `extensionduedate` bigint(10) NOT NULL DEFAULT '0',
  `workflowstate` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
  `allocatedmarker` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_assiuserflag_mai_ix` (`mailed`),
  KEY `mdl_assiuserflag_use_ix` (`userid`),
  KEY `mdl_assiuserflag_ass_ix` (`assignment`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='List of flags that can be set for a single user in a single ';

/*Data for the table `mdl_assign_user_flags` */

/*Table structure for table `mdl_assign_user_mapping` */

DROP TABLE IF EXISTS `mdl_assign_user_mapping`;

CREATE TABLE `mdl_assign_user_mapping` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `assignment` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_assiusermapp_ass_ix` (`assignment`),
  KEY `mdl_assiusermapp_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Map an assignment specific id number to a user';

/*Data for the table `mdl_assign_user_mapping` */

/*Table structure for table `mdl_assignfeedback_comments` */

DROP TABLE IF EXISTS `mdl_assignfeedback_comments`;

CREATE TABLE `mdl_assignfeedback_comments` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `assignment` bigint(10) NOT NULL DEFAULT '0',
  `grade` bigint(10) NOT NULL DEFAULT '0',
  `commenttext` longtext COLLATE utf8_unicode_ci,
  `commentformat` smallint(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_assicomm_ass_ix` (`assignment`),
  KEY `mdl_assicomm_gra_ix` (`grade`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Text feedback for submitted assignments';

/*Data for the table `mdl_assignfeedback_comments` */

/*Table structure for table `mdl_assignfeedback_editpdf_annot` */

DROP TABLE IF EXISTS `mdl_assignfeedback_editpdf_annot`;

CREATE TABLE `mdl_assignfeedback_editpdf_annot` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `gradeid` bigint(10) NOT NULL DEFAULT '0',
  `pageno` bigint(10) NOT NULL DEFAULT '0',
  `x` bigint(10) DEFAULT '0',
  `y` bigint(10) DEFAULT '0',
  `endx` bigint(10) DEFAULT '0',
  `endy` bigint(10) DEFAULT '0',
  `path` longtext COLLATE utf8_unicode_ci,
  `type` varchar(10) COLLATE utf8_unicode_ci DEFAULT 'line',
  `colour` varchar(10) COLLATE utf8_unicode_ci DEFAULT 'black',
  `draft` tinyint(2) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `mdl_assieditanno_grapag_ix` (`gradeid`,`pageno`),
  KEY `mdl_assieditanno_gra_ix` (`gradeid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='stores annotations added to pdfs submitted by students';

/*Data for the table `mdl_assignfeedback_editpdf_annot` */

/*Table structure for table `mdl_assignfeedback_editpdf_cmnt` */

DROP TABLE IF EXISTS `mdl_assignfeedback_editpdf_cmnt`;

CREATE TABLE `mdl_assignfeedback_editpdf_cmnt` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `gradeid` bigint(10) NOT NULL DEFAULT '0',
  `x` bigint(10) DEFAULT '0',
  `y` bigint(10) DEFAULT '0',
  `width` bigint(10) DEFAULT '120',
  `rawtext` longtext COLLATE utf8_unicode_ci,
  `pageno` bigint(10) NOT NULL DEFAULT '0',
  `colour` varchar(10) COLLATE utf8_unicode_ci DEFAULT 'black',
  `draft` tinyint(2) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `mdl_assieditcmnt_grapag_ix` (`gradeid`,`pageno`),
  KEY `mdl_assieditcmnt_gra_ix` (`gradeid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores comments added to pdfs';

/*Data for the table `mdl_assignfeedback_editpdf_cmnt` */

/*Table structure for table `mdl_assignfeedback_editpdf_quick` */

DROP TABLE IF EXISTS `mdl_assignfeedback_editpdf_quick`;

CREATE TABLE `mdl_assignfeedback_editpdf_quick` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `rawtext` longtext COLLATE utf8_unicode_ci NOT NULL,
  `width` bigint(10) NOT NULL DEFAULT '120',
  `colour` varchar(10) COLLATE utf8_unicode_ci DEFAULT 'yellow',
  PRIMARY KEY (`id`),
  KEY `mdl_assieditquic_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores teacher specified quicklist comments';

/*Data for the table `mdl_assignfeedback_editpdf_quick` */

/*Table structure for table `mdl_assignfeedback_file` */

DROP TABLE IF EXISTS `mdl_assignfeedback_file`;

CREATE TABLE `mdl_assignfeedback_file` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `assignment` bigint(10) NOT NULL DEFAULT '0',
  `grade` bigint(10) NOT NULL DEFAULT '0',
  `numfiles` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_assifile_ass2_ix` (`assignment`),
  KEY `mdl_assifile_gra_ix` (`grade`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores info about the number of files submitted by a student';

/*Data for the table `mdl_assignfeedback_file` */

/*Table structure for table `mdl_assignment` */

DROP TABLE IF EXISTS `mdl_assignment`;

CREATE TABLE `mdl_assignment` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci NOT NULL,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `assignmenttype` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `resubmit` tinyint(2) NOT NULL DEFAULT '0',
  `preventlate` tinyint(2) NOT NULL DEFAULT '0',
  `emailteachers` tinyint(2) NOT NULL DEFAULT '0',
  `var1` bigint(10) DEFAULT '0',
  `var2` bigint(10) DEFAULT '0',
  `var3` bigint(10) DEFAULT '0',
  `var4` bigint(10) DEFAULT '0',
  `var5` bigint(10) DEFAULT '0',
  `maxbytes` bigint(10) NOT NULL DEFAULT '100000',
  `timedue` bigint(10) NOT NULL DEFAULT '0',
  `timeavailable` bigint(10) NOT NULL DEFAULT '0',
  `grade` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_assi_cou2_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines assignments';

/*Data for the table `mdl_assignment` */

/*Table structure for table `mdl_assignment_submissions` */

DROP TABLE IF EXISTS `mdl_assignment_submissions`;

CREATE TABLE `mdl_assignment_submissions` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `assignment` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `numfiles` bigint(10) NOT NULL DEFAULT '0',
  `data1` longtext COLLATE utf8_unicode_ci,
  `data2` longtext COLLATE utf8_unicode_ci,
  `grade` bigint(11) NOT NULL DEFAULT '0',
  `submissioncomment` longtext COLLATE utf8_unicode_ci NOT NULL,
  `format` smallint(4) NOT NULL DEFAULT '0',
  `teacher` bigint(10) NOT NULL DEFAULT '0',
  `timemarked` bigint(10) NOT NULL DEFAULT '0',
  `mailed` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_assisubm_use2_ix` (`userid`),
  KEY `mdl_assisubm_mai_ix` (`mailed`),
  KEY `mdl_assisubm_tim_ix` (`timemarked`),
  KEY `mdl_assisubm_ass2_ix` (`assignment`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Info about submitted assignments';

/*Data for the table `mdl_assignment_submissions` */

/*Table structure for table `mdl_assignment_upgrade` */

DROP TABLE IF EXISTS `mdl_assignment_upgrade`;

CREATE TABLE `mdl_assignment_upgrade` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `oldcmid` bigint(10) NOT NULL DEFAULT '0',
  `oldinstance` bigint(10) NOT NULL DEFAULT '0',
  `newcmid` bigint(10) NOT NULL DEFAULT '0',
  `newinstance` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_assiupgr_old_ix` (`oldcmid`),
  KEY `mdl_assiupgr_old2_ix` (`oldinstance`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Info about upgraded assignments';

/*Data for the table `mdl_assignment_upgrade` */

/*Table structure for table `mdl_assignsubmission_file` */

DROP TABLE IF EXISTS `mdl_assignsubmission_file`;

CREATE TABLE `mdl_assignsubmission_file` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `assignment` bigint(10) NOT NULL DEFAULT '0',
  `submission` bigint(10) NOT NULL DEFAULT '0',
  `numfiles` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_assifile_ass_ix` (`assignment`),
  KEY `mdl_assifile_sub_ix` (`submission`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Info about file submissions for assignments';

/*Data for the table `mdl_assignsubmission_file` */

/*Table structure for table `mdl_assignsubmission_onlinetext` */

DROP TABLE IF EXISTS `mdl_assignsubmission_onlinetext`;

CREATE TABLE `mdl_assignsubmission_onlinetext` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `assignment` bigint(10) NOT NULL DEFAULT '0',
  `submission` bigint(10) NOT NULL DEFAULT '0',
  `onlinetext` longtext COLLATE utf8_unicode_ci,
  `onlineformat` smallint(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_assionli_ass_ix` (`assignment`),
  KEY `mdl_assionli_sub_ix` (`submission`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Info about onlinetext submission';

/*Data for the table `mdl_assignsubmission_onlinetext` */

/*Table structure for table `mdl_backup_controllers` */

DROP TABLE IF EXISTS `mdl_backup_controllers`;

CREATE TABLE `mdl_backup_controllers` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `backupid` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `operation` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'backup',
  `type` varchar(10) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `itemid` bigint(10) NOT NULL,
  `format` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `interactive` smallint(4) NOT NULL,
  `purpose` smallint(4) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `status` smallint(4) NOT NULL,
  `execution` smallint(4) NOT NULL,
  `executiontime` bigint(10) NOT NULL,
  `checksum` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timecreated` bigint(10) NOT NULL,
  `timemodified` bigint(10) NOT NULL,
  `controller` longtext COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_backcont_bac_uix` (`backupid`),
  KEY `mdl_backcont_typite_ix` (`type`,`itemid`),
  KEY `mdl_backcont_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='To store the backup_controllers as they are used';

/*Data for the table `mdl_backup_controllers` */

/*Table structure for table `mdl_backup_courses` */

DROP TABLE IF EXISTS `mdl_backup_courses`;

CREATE TABLE `mdl_backup_courses` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `laststarttime` bigint(10) NOT NULL DEFAULT '0',
  `lastendtime` bigint(10) NOT NULL DEFAULT '0',
  `laststatus` varchar(1) COLLATE utf8_unicode_ci NOT NULL DEFAULT '5',
  `nextstarttime` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_backcour_cou_uix` (`courseid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='To store every course backup status';

/*Data for the table `mdl_backup_courses` */

/*Table structure for table `mdl_backup_logs` */

DROP TABLE IF EXISTS `mdl_backup_logs`;

CREATE TABLE `mdl_backup_logs` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `backupid` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `loglevel` smallint(4) NOT NULL,
  `message` longtext COLLATE utf8_unicode_ci NOT NULL,
  `timecreated` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_backlogs_bacid_uix` (`backupid`,`id`),
  KEY `mdl_backlogs_bac_ix` (`backupid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='To store all the logs from backup and restore operations (by';

/*Data for the table `mdl_backup_logs` */

/*Table structure for table `mdl_badge` */

DROP TABLE IF EXISTS `mdl_badge`;

CREATE TABLE `mdl_badge` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci,
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `usercreated` bigint(10) NOT NULL,
  `usermodified` bigint(10) NOT NULL,
  `issuername` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `issuerurl` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `issuercontact` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `expiredate` bigint(10) DEFAULT NULL,
  `expireperiod` bigint(10) DEFAULT NULL,
  `type` tinyint(1) NOT NULL DEFAULT '1',
  `courseid` bigint(10) DEFAULT NULL,
  `message` longtext COLLATE utf8_unicode_ci NOT NULL,
  `messagesubject` longtext COLLATE utf8_unicode_ci NOT NULL,
  `attachment` tinyint(1) NOT NULL DEFAULT '1',
  `notification` tinyint(1) NOT NULL DEFAULT '1',
  `status` tinyint(1) NOT NULL DEFAULT '0',
  `nextcron` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_badg_typ_ix` (`type`),
  KEY `mdl_badg_cou_ix` (`courseid`),
  KEY `mdl_badg_use_ix` (`usermodified`),
  KEY `mdl_badg_use2_ix` (`usercreated`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines badge';

/*Data for the table `mdl_badge` */

/*Table structure for table `mdl_badge_backpack` */

DROP TABLE IF EXISTS `mdl_badge_backpack`;

CREATE TABLE `mdl_badge_backpack` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `email` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `backpackurl` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `backpackuid` bigint(10) NOT NULL,
  `autosync` tinyint(1) NOT NULL DEFAULT '0',
  `password` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_badgback_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines settings for connecting external backpack';

/*Data for the table `mdl_badge_backpack` */

/*Table structure for table `mdl_badge_criteria` */

DROP TABLE IF EXISTS `mdl_badge_criteria`;

CREATE TABLE `mdl_badge_criteria` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `badgeid` bigint(10) NOT NULL DEFAULT '0',
  `criteriatype` bigint(10) DEFAULT NULL,
  `method` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_badgcrit_badcri_uix` (`badgeid`,`criteriatype`),
  KEY `mdl_badgcrit_cri_ix` (`criteriatype`),
  KEY `mdl_badgcrit_bad_ix` (`badgeid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines criteria for issuing badges';

/*Data for the table `mdl_badge_criteria` */

/*Table structure for table `mdl_badge_criteria_met` */

DROP TABLE IF EXISTS `mdl_badge_criteria_met`;

CREATE TABLE `mdl_badge_criteria_met` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `issuedid` bigint(10) DEFAULT NULL,
  `critid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `datemet` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_badgcritmet_cri_ix` (`critid`),
  KEY `mdl_badgcritmet_use_ix` (`userid`),
  KEY `mdl_badgcritmet_iss_ix` (`issuedid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines criteria that were met for an issued badge';

/*Data for the table `mdl_badge_criteria_met` */

/*Table structure for table `mdl_badge_criteria_param` */

DROP TABLE IF EXISTS `mdl_badge_criteria_param`;

CREATE TABLE `mdl_badge_criteria_param` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `critid` bigint(10) NOT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_badgcritpara_cri_ix` (`critid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines parameters for badges criteria';

/*Data for the table `mdl_badge_criteria_param` */

/*Table structure for table `mdl_badge_external` */

DROP TABLE IF EXISTS `mdl_badge_external`;

CREATE TABLE `mdl_badge_external` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `backpackid` bigint(10) NOT NULL,
  `collectionid` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_badgexte_bac_ix` (`backpackid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Setting for external badges display';

/*Data for the table `mdl_badge_external` */

/*Table structure for table `mdl_badge_issued` */

DROP TABLE IF EXISTS `mdl_badge_issued`;

CREATE TABLE `mdl_badge_issued` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `badgeid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `uniquehash` longtext COLLATE utf8_unicode_ci NOT NULL,
  `dateissued` bigint(10) NOT NULL DEFAULT '0',
  `dateexpire` bigint(10) DEFAULT NULL,
  `visible` tinyint(1) NOT NULL DEFAULT '0',
  `issuernotified` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_badgissu_baduse_uix` (`badgeid`,`userid`),
  KEY `mdl_badgissu_bad_ix` (`badgeid`),
  KEY `mdl_badgissu_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines issued badges';

/*Data for the table `mdl_badge_issued` */

/*Table structure for table `mdl_badge_manual_award` */

DROP TABLE IF EXISTS `mdl_badge_manual_award`;

CREATE TABLE `mdl_badge_manual_award` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `badgeid` bigint(10) NOT NULL,
  `recipientid` bigint(10) NOT NULL,
  `issuerid` bigint(10) NOT NULL,
  `issuerrole` bigint(10) NOT NULL,
  `datemet` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_badgmanuawar_bad_ix` (`badgeid`),
  KEY `mdl_badgmanuawar_rec_ix` (`recipientid`),
  KEY `mdl_badgmanuawar_iss_ix` (`issuerid`),
  KEY `mdl_badgmanuawar_iss2_ix` (`issuerrole`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Track manual award criteria for badges';

/*Data for the table `mdl_badge_manual_award` */

/*Table structure for table `mdl_block` */

DROP TABLE IF EXISTS `mdl_block`;

CREATE TABLE `mdl_block` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `cron` bigint(10) NOT NULL DEFAULT '0',
  `lastcron` bigint(10) NOT NULL DEFAULT '0',
  `visible` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_bloc_nam_uix` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=44 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='contains all installed blocks';

/*Data for the table `mdl_block` */

insert  into `mdl_block`(`id`,`name`,`cron`,`lastcron`,`visible`) values (1,'activity_modules',0,0,1),(2,'admin_bookmarks',0,0,1),(3,'badges',0,0,1),(4,'blog_menu',0,0,1),(5,'blog_recent',0,0,1),(6,'blog_tags',0,0,1),(7,'calendar_month',0,0,1),(8,'calendar_upcoming',0,0,1),(9,'comments',0,0,1),(10,'community',0,0,1),(11,'completionstatus',0,0,1),(12,'course_list',0,0,1),(13,'course_overview',0,0,1),(14,'course_summary',0,0,1),(15,'feedback',0,0,0),(16,'glossary_random',0,0,1),(17,'html',0,0,1),(18,'login',0,0,1),(19,'mentees',0,0,1),(20,'messages',0,0,1),(21,'mnet_hosts',0,0,1),(22,'myprofile',0,0,1),(23,'navigation',0,0,1),(24,'news_items',0,0,1),(25,'online_users',0,0,1),(26,'participants',0,0,1),(27,'private_files',0,0,1),(28,'quiz_results',0,0,1),(29,'recent_activity',86400,0,1),(30,'rss_client',300,0,1),(31,'search_forums',0,0,1),(32,'section_links',0,0,1),(33,'selfcompletion',0,0,1),(34,'settings',0,0,1),(35,'site_main_menu',0,0,1),(36,'social_activities',0,0,1),(37,'tag_flickr',0,0,1),(38,'tag_youtube',0,0,1),(39,'tags',0,0,1),(40,'courserequest',0,0,1),(41,'elisadmin',0,0,1),(42,'enrolsurvey',0,0,1),(43,'repository',0,0,1);

/*Table structure for table `mdl_block_community` */

DROP TABLE IF EXISTS `mdl_block_community`;

CREATE TABLE `mdl_block_community` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL,
  `coursename` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `coursedescription` longtext COLLATE utf8_unicode_ci,
  `courseurl` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `imageurl` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Community block';

/*Data for the table `mdl_block_community` */

/*Table structure for table `mdl_block_courserequest` */

DROP TABLE IF EXISTS `mdl_block_courserequest`;

CREATE TABLE `mdl_block_courserequest` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `title` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `firstname` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `lastname` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `email` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `requeststatus` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'pending',
  `classid` bigint(10) NOT NULL DEFAULT '0',
  `usecoursetemplate` tinyint(1) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `modifiedby` bigint(10) NOT NULL DEFAULT '0',
  `statusnote` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `mdl_bloccour_use_ix` (`userid`),
  KEY `mdl_bloccour_mod_ix` (`modifiedby`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores an indivudual request';

/*Data for the table `mdl_block_courserequest` */

/*Table structure for table `mdl_block_courserequest_data` */

DROP TABLE IF EXISTS `mdl_block_courserequest_data`;

CREATE TABLE `mdl_block_courserequest_data` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `requestid` bigint(10) NOT NULL DEFAULT '0',
  `fieldid` bigint(10) NOT NULL DEFAULT '0',
  `data` longtext COLLATE utf8_unicode_ci,
  `contextlevel` bigint(10) NOT NULL DEFAULT '0',
  `multiple` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_bloccourdata_req_ix` (`requestid`),
  KEY `mdl_bloccourdata_fie_ix` (`fieldid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Keeps track of custom field values in individual course requ';

/*Data for the table `mdl_block_courserequest_data` */

/*Table structure for table `mdl_block_courserequest_fields` */

DROP TABLE IF EXISTS `mdl_block_courserequest_fields`;

CREATE TABLE `mdl_block_courserequest_fields` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `fieldid` bigint(10) NOT NULL DEFAULT '0',
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  `contextlevel` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_bloccourfiel_fie_ix` (`fieldid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Associates custom fields with the course request form';

/*Data for the table `mdl_block_courserequest_fields` */

/*Table structure for table `mdl_block_enrolsurvey_taken` */

DROP TABLE IF EXISTS `mdl_block_enrolsurvey_taken`;

CREATE TABLE `mdl_block_enrolsurvey_taken` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `blockinstanceid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_blocenrotake_blo_ix` (`blockinstanceid`),
  KEY `mdl_blocenrotake_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='whether or not a user has taken the survey';

/*Data for the table `mdl_block_enrolsurvey_taken` */

/*Table structure for table `mdl_block_instances` */

DROP TABLE IF EXISTS `mdl_block_instances`;

CREATE TABLE `mdl_block_instances` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `blockname` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `parentcontextid` bigint(10) NOT NULL,
  `showinsubcontexts` smallint(4) NOT NULL,
  `pagetypepattern` varchar(64) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `subpagepattern` varchar(16) COLLATE utf8_unicode_ci DEFAULT NULL,
  `defaultregion` varchar(16) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `defaultweight` bigint(10) NOT NULL,
  `configdata` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `mdl_blocinst_parshopagsub_ix` (`parentcontextid`,`showinsubcontexts`,`pagetypepattern`,`subpagepattern`),
  KEY `mdl_blocinst_par_ix` (`parentcontextid`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table stores block instances. The type of block this is';

/*Data for the table `mdl_block_instances` */

insert  into `mdl_block_instances`(`id`,`blockname`,`parentcontextid`,`showinsubcontexts`,`pagetypepattern`,`subpagepattern`,`defaultregion`,`defaultweight`,`configdata`) values (1,'site_main_menu',2,0,'site-index',NULL,'side-pre',0,''),(2,'course_summary',2,0,'site-index',NULL,'side-post',0,''),(3,'calendar_month',2,0,'site-index',NULL,'side-post',1,''),(4,'navigation',1,1,'*',NULL,'side-pre',0,''),(5,'settings',1,1,'*',NULL,'side-pre',1,''),(6,'admin_bookmarks',1,0,'admin-*',NULL,'side-pre',0,''),(7,'private_files',1,0,'my-index','2','side-post',0,''),(8,'online_users',1,0,'my-index','2','side-post',1,''),(9,'course_overview',1,0,'my-index','2','content',0,''),(10,'elisadmin',1,1,'*',NULL,'side-pre',-999,NULL);

/*Table structure for table `mdl_block_positions` */

DROP TABLE IF EXISTS `mdl_block_positions`;

CREATE TABLE `mdl_block_positions` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `blockinstanceid` bigint(10) NOT NULL,
  `contextid` bigint(10) NOT NULL,
  `pagetype` varchar(64) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `subpage` varchar(16) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `visible` smallint(4) NOT NULL,
  `region` varchar(16) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `weight` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_blocposi_bloconpagsub_uix` (`blockinstanceid`,`contextid`,`pagetype`,`subpage`),
  KEY `mdl_blocposi_blo_ix` (`blockinstanceid`),
  KEY `mdl_blocposi_con_ix` (`contextid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores the position of a sticky block_instance on a another ';

/*Data for the table `mdl_block_positions` */

/*Table structure for table `mdl_block_recent_activity` */

DROP TABLE IF EXISTS `mdl_block_recent_activity`;

CREATE TABLE `mdl_block_recent_activity` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL,
  `cmid` bigint(10) NOT NULL,
  `timecreated` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `action` tinyint(1) NOT NULL,
  `modname` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_blocreceacti_coutim_ix` (`courseid`,`timecreated`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Recent activity block';

/*Data for the table `mdl_block_recent_activity` */

/*Table structure for table `mdl_block_rss_client` */

DROP TABLE IF EXISTS `mdl_block_rss_client`;

CREATE TABLE `mdl_block_rss_client` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `title` longtext COLLATE utf8_unicode_ci NOT NULL,
  `preferredtitle` varchar(64) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci NOT NULL,
  `shared` tinyint(2) NOT NULL DEFAULT '0',
  `url` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Remote news feed information. Contains the news feed id, the';

/*Data for the table `mdl_block_rss_client` */

/*Table structure for table `mdl_blog_association` */

DROP TABLE IF EXISTS `mdl_blog_association`;

CREATE TABLE `mdl_blog_association` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `contextid` bigint(10) NOT NULL,
  `blogid` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_blogasso_con_ix` (`contextid`),
  KEY `mdl_blogasso_blo_ix` (`blogid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Associations of blog entries with courses and module instanc';

/*Data for the table `mdl_blog_association` */

/*Table structure for table `mdl_blog_external` */

DROP TABLE IF EXISTS `mdl_blog_external`;

CREATE TABLE `mdl_blog_external` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci,
  `url` longtext COLLATE utf8_unicode_ci NOT NULL,
  `filtertags` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `failedlastsync` tinyint(1) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) DEFAULT NULL,
  `timefetched` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_blogexte_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='External blog links used for RSS copying of blog entries to ';

/*Data for the table `mdl_blog_external` */

/*Table structure for table `mdl_book` */

DROP TABLE IF EXISTS `mdl_book`;

CREATE TABLE `mdl_book` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `numbering` smallint(4) NOT NULL DEFAULT '0',
  `customtitles` tinyint(2) NOT NULL DEFAULT '0',
  `revision` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines book';

/*Data for the table `mdl_book` */

/*Table structure for table `mdl_book_chapters` */

DROP TABLE IF EXISTS `mdl_book_chapters`;

CREATE TABLE `mdl_book_chapters` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `bookid` bigint(10) NOT NULL DEFAULT '0',
  `pagenum` bigint(10) NOT NULL DEFAULT '0',
  `subchapter` bigint(10) NOT NULL DEFAULT '0',
  `title` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `content` longtext COLLATE utf8_unicode_ci NOT NULL,
  `contentformat` smallint(4) NOT NULL DEFAULT '0',
  `hidden` tinyint(2) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `importsrc` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines book_chapters';

/*Data for the table `mdl_book_chapters` */

/*Table structure for table `mdl_cache_filters` */

DROP TABLE IF EXISTS `mdl_cache_filters`;

CREATE TABLE `mdl_cache_filters` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `filter` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `version` bigint(10) NOT NULL DEFAULT '0',
  `md5key` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `rawtext` longtext COLLATE utf8_unicode_ci NOT NULL,
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_cachfilt_filmd5_ix` (`filter`,`md5key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='For keeping information about cached data';

/*Data for the table `mdl_cache_filters` */

/*Table structure for table `mdl_cache_flags` */

DROP TABLE IF EXISTS `mdl_cache_flags`;

CREATE TABLE `mdl_cache_flags` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `flagtype` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `value` longtext COLLATE utf8_unicode_ci NOT NULL,
  `expiry` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_cachflag_fla_ix` (`flagtype`),
  KEY `mdl_cachflag_nam_ix` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Cache of time-sensitive flags';

/*Data for the table `mdl_cache_flags` */

insert  into `mdl_cache_flags`(`id`,`flagtype`,`name`,`timemodified`,`value`,`expiry`) values (1,'userpreferenceschanged','2',1414028442,'1',1414028742),(2,'userpreferenceschanged','3',1413862469,'1',1413869669),(3,'userpreferenceschanged','4',1413975353,'1',1413975653),(4,'accesslib/dirtycontexts','/1',1413975354,'1',1413975654);

/*Table structure for table `mdl_capabilities` */

DROP TABLE IF EXISTS `mdl_capabilities`;

CREATE TABLE `mdl_capabilities` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `captype` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `contextlevel` bigint(10) NOT NULL DEFAULT '0',
  `component` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `riskbitmask` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_capa_nam_uix` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=588 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='this defines all capabilities';

/*Data for the table `mdl_capabilities` */

insert  into `mdl_capabilities`(`id`,`name`,`captype`,`contextlevel`,`component`,`riskbitmask`) values (1,'moodle/site:config','write',10,'moodle',62),(2,'moodle/site:readallmessages','read',10,'moodle',8),(3,'moodle/site:sendmessage','write',10,'moodle',16),(4,'moodle/site:approvecourse','write',10,'moodle',4),(5,'moodle/backup:backupcourse','write',50,'moodle',28),(6,'moodle/backup:backupsection','write',50,'moodle',28),(7,'moodle/backup:backupactivity','write',70,'moodle',28),(8,'moodle/backup:backuptargethub','write',50,'moodle',28),(9,'moodle/backup:backuptargetimport','write',50,'moodle',28),(10,'moodle/backup:downloadfile','write',50,'moodle',28),(11,'moodle/backup:configure','write',50,'moodle',28),(12,'moodle/backup:userinfo','read',50,'moodle',8),(13,'moodle/backup:anonymise','read',50,'moodle',8),(14,'moodle/restore:restorecourse','write',50,'moodle',28),(15,'moodle/restore:restoresection','write',50,'moodle',28),(16,'moodle/restore:restoreactivity','write',50,'moodle',28),(17,'moodle/restore:viewautomatedfilearea','write',50,'moodle',28),(18,'moodle/restore:restoretargethub','write',50,'moodle',28),(19,'moodle/restore:restoretargetimport','write',50,'moodle',28),(20,'moodle/restore:uploadfile','write',50,'moodle',28),(21,'moodle/restore:configure','write',50,'moodle',28),(22,'moodle/restore:rolldates','write',50,'moodle',0),(23,'moodle/restore:userinfo','write',50,'moodle',30),(24,'moodle/restore:createuser','write',10,'moodle',24),(25,'moodle/site:manageblocks','write',80,'moodle',20),(26,'moodle/site:accessallgroups','read',50,'moodle',0),(27,'moodle/site:viewfullnames','read',50,'moodle',0),(28,'moodle/site:viewuseridentity','read',50,'moodle',0),(29,'moodle/site:viewreports','read',50,'moodle',8),(30,'moodle/site:trustcontent','write',50,'moodle',4),(31,'moodle/site:uploadusers','write',10,'moodle',24),(32,'moodle/filter:manage','write',50,'moodle',0),(33,'moodle/user:create','write',10,'moodle',24),(34,'moodle/user:delete','write',10,'moodle',8),(35,'moodle/user:update','write',10,'moodle',24),(36,'moodle/user:viewdetails','read',50,'moodle',0),(37,'moodle/user:viewalldetails','read',30,'moodle',8),(38,'moodle/user:viewlastip','read',30,'moodle',8),(39,'moodle/user:viewhiddendetails','read',50,'moodle',8),(40,'moodle/user:loginas','write',50,'moodle',30),(41,'moodle/user:managesyspages','write',10,'moodle',0),(42,'moodle/user:manageblocks','write',30,'moodle',0),(43,'moodle/user:manageownblocks','write',10,'moodle',0),(44,'moodle/user:manageownfiles','write',10,'moodle',0),(45,'moodle/user:ignoreuserquota','write',10,'moodle',0),(46,'moodle/my:configsyspages','write',10,'moodle',0),(47,'moodle/role:assign','write',50,'moodle',28),(48,'moodle/role:review','read',50,'moodle',8),(49,'moodle/role:override','write',50,'moodle',28),(50,'moodle/role:safeoverride','write',50,'moodle',16),(51,'moodle/role:manage','write',10,'moodle',28),(52,'moodle/role:switchroles','read',50,'moodle',12),(53,'moodle/category:manage','write',40,'moodle',4),(54,'moodle/category:viewhiddencategories','read',40,'moodle',0),(55,'moodle/cohort:manage','write',40,'moodle',0),(56,'moodle/cohort:assign','write',40,'moodle',0),(57,'moodle/cohort:view','read',50,'moodle',0),(58,'moodle/course:create','write',40,'moodle',4),(59,'moodle/course:request','write',10,'moodle',0),(60,'moodle/course:delete','write',50,'moodle',32),(61,'moodle/course:update','write',50,'moodle',4),(62,'moodle/course:view','read',50,'moodle',0),(63,'moodle/course:enrolreview','read',50,'moodle',8),(64,'moodle/course:enrolconfig','write',50,'moodle',8),(65,'moodle/course:reviewotherusers','read',50,'moodle',0),(66,'moodle/course:bulkmessaging','write',50,'moodle',16),(67,'moodle/course:viewhiddenuserfields','read',50,'moodle',8),(68,'moodle/course:viewhiddencourses','read',50,'moodle',0),(69,'moodle/course:visibility','write',50,'moodle',0),(70,'moodle/course:managefiles','write',50,'moodle',4),(71,'moodle/course:ignorefilesizelimits','write',50,'moodle',0),(72,'moodle/course:manageactivities','write',70,'moodle',4),(73,'moodle/course:activityvisibility','write',70,'moodle',0),(74,'moodle/course:viewhiddenactivities','write',70,'moodle',0),(75,'moodle/course:viewparticipants','read',50,'moodle',0),(76,'moodle/course:changefullname','write',50,'moodle',4),(77,'moodle/course:changeshortname','write',50,'moodle',4),(78,'moodle/course:changeidnumber','write',50,'moodle',4),(79,'moodle/course:changecategory','write',50,'moodle',4),(80,'moodle/course:changesummary','write',50,'moodle',4),(81,'moodle/site:viewparticipants','read',10,'moodle',0),(82,'moodle/course:isincompletionreports','read',50,'moodle',0),(83,'moodle/course:viewscales','read',50,'moodle',0),(84,'moodle/course:managescales','write',50,'moodle',0),(85,'moodle/course:managegroups','write',50,'moodle',0),(86,'moodle/course:reset','write',50,'moodle',32),(87,'moodle/course:viewsuspendedusers','read',10,'moodle',0),(88,'moodle/blog:view','read',10,'moodle',0),(89,'moodle/blog:search','read',10,'moodle',0),(90,'moodle/blog:viewdrafts','read',10,'moodle',8),(91,'moodle/blog:create','write',10,'moodle',16),(92,'moodle/blog:manageentries','write',10,'moodle',16),(93,'moodle/blog:manageexternal','write',10,'moodle',16),(94,'moodle/blog:associatecourse','write',50,'moodle',0),(95,'moodle/blog:associatemodule','write',70,'moodle',0),(96,'moodle/calendar:manageownentries','write',50,'moodle',16),(97,'moodle/calendar:managegroupentries','write',50,'moodle',16),(98,'moodle/calendar:manageentries','write',50,'moodle',16),(99,'moodle/user:editprofile','write',30,'moodle',24),(100,'moodle/user:editownprofile','write',10,'moodle',16),(101,'moodle/user:changeownpassword','write',10,'moodle',0),(102,'moodle/user:readuserposts','read',30,'moodle',0),(103,'moodle/user:readuserblogs','read',30,'moodle',0),(104,'moodle/user:viewuseractivitiesreport','read',30,'moodle',8),(105,'moodle/user:editmessageprofile','write',30,'moodle',16),(106,'moodle/user:editownmessageprofile','write',10,'moodle',0),(107,'moodle/question:managecategory','write',50,'moodle',20),(108,'moodle/question:add','write',50,'moodle',20),(109,'moodle/question:editmine','write',50,'moodle',20),(110,'moodle/question:editall','write',50,'moodle',20),(111,'moodle/question:viewmine','read',50,'moodle',0),(112,'moodle/question:viewall','read',50,'moodle',0),(113,'moodle/question:usemine','read',50,'moodle',0),(114,'moodle/question:useall','read',50,'moodle',0),(115,'moodle/question:movemine','write',50,'moodle',0),(116,'moodle/question:moveall','write',50,'moodle',0),(117,'moodle/question:config','write',10,'moodle',2),(118,'moodle/question:flag','write',50,'moodle',0),(119,'moodle/site:doclinks','read',10,'moodle',0),(120,'moodle/course:sectionvisibility','write',50,'moodle',0),(121,'moodle/course:useremail','write',50,'moodle',0),(122,'moodle/course:viewhiddensections','write',50,'moodle',0),(123,'moodle/course:setcurrentsection','write',50,'moodle',0),(124,'moodle/course:movesections','write',50,'moodle',0),(125,'moodle/site:mnetlogintoremote','read',10,'moodle',0),(126,'moodle/grade:viewall','read',50,'moodle',8),(127,'moodle/grade:view','read',50,'moodle',0),(128,'moodle/grade:viewhidden','read',50,'moodle',8),(129,'moodle/grade:import','write',50,'moodle',12),(130,'moodle/grade:export','read',50,'moodle',8),(131,'moodle/grade:manage','write',50,'moodle',12),(132,'moodle/grade:edit','write',50,'moodle',12),(133,'moodle/grade:managegradingforms','write',50,'moodle',12),(134,'moodle/grade:sharegradingforms','write',10,'moodle',4),(135,'moodle/grade:managesharedforms','write',10,'moodle',4),(136,'moodle/grade:manageoutcomes','write',50,'moodle',0),(137,'moodle/grade:manageletters','write',50,'moodle',0),(138,'moodle/grade:hide','write',50,'moodle',0),(139,'moodle/grade:lock','write',50,'moodle',0),(140,'moodle/grade:unlock','write',50,'moodle',0),(141,'moodle/my:manageblocks','write',10,'moodle',0),(142,'moodle/notes:view','read',50,'moodle',0),(143,'moodle/notes:manage','write',50,'moodle',16),(144,'moodle/tag:manage','write',10,'moodle',16),(145,'moodle/tag:create','write',10,'moodle',16),(146,'moodle/tag:edit','write',10,'moodle',16),(147,'moodle/tag:flag','write',10,'moodle',16),(148,'moodle/tag:editblocks','write',10,'moodle',0),(149,'moodle/block:view','read',80,'moodle',0),(150,'moodle/block:edit','write',80,'moodle',20),(151,'moodle/portfolio:export','read',10,'moodle',0),(152,'moodle/comment:view','read',50,'moodle',0),(153,'moodle/comment:post','write',50,'moodle',24),(154,'moodle/comment:delete','write',50,'moodle',32),(155,'moodle/webservice:createtoken','write',10,'moodle',62),(156,'moodle/webservice:createmobiletoken','write',10,'moodle',24),(157,'moodle/rating:view','read',50,'moodle',0),(158,'moodle/rating:viewany','read',50,'moodle',8),(159,'moodle/rating:viewall','read',50,'moodle',8),(160,'moodle/rating:rate','write',50,'moodle',0),(161,'moodle/course:publish','write',10,'moodle',24),(162,'moodle/course:markcomplete','write',50,'moodle',0),(163,'moodle/community:add','write',10,'moodle',0),(164,'moodle/community:download','write',10,'moodle',0),(165,'moodle/badges:manageglobalsettings','write',10,'moodle',34),(166,'moodle/badges:viewbadges','read',50,'moodle',0),(167,'moodle/badges:manageownbadges','write',30,'moodle',0),(168,'moodle/badges:viewotherbadges','read',30,'moodle',0),(169,'moodle/badges:earnbadge','write',50,'moodle',0),(170,'moodle/badges:createbadge','write',50,'moodle',16),(171,'moodle/badges:deletebadge','write',50,'moodle',32),(172,'moodle/badges:configuredetails','write',50,'moodle',16),(173,'moodle/badges:configurecriteria','write',50,'moodle',0),(174,'moodle/badges:configuremessages','write',50,'moodle',16),(175,'moodle/badges:awardbadge','write',50,'moodle',16),(176,'moodle/badges:viewawarded','read',50,'moodle',8),(177,'moodle/site:forcelanguage','read',10,'moodle',0),(178,'mod/assign:view','read',70,'mod_assign',0),(179,'mod/assign:submit','write',70,'mod_assign',0),(180,'mod/assign:grade','write',70,'mod_assign',4),(181,'mod/assign:exportownsubmission','read',70,'mod_assign',0),(182,'mod/assign:addinstance','write',50,'mod_assign',4),(183,'mod/assign:editothersubmission','write',70,'mod_assign',41),(184,'mod/assign:grantextension','write',70,'mod_assign',0),(185,'mod/assign:revealidentities','write',70,'mod_assign',0),(186,'mod/assign:reviewgrades','write',70,'mod_assign',0),(187,'mod/assign:releasegrades','write',70,'mod_assign',0),(188,'mod/assign:managegrades','write',70,'mod_assign',0),(189,'mod/assign:manageallocations','write',70,'mod_assign',0),(190,'mod/assign:viewgrades','read',70,'mod_assign',0),(191,'mod/assignment:view','read',70,'mod_assignment',0),(192,'mod/assignment:addinstance','write',50,'mod_assignment',4),(193,'mod/assignment:submit','write',70,'mod_assignment',0),(194,'mod/assignment:grade','write',70,'mod_assignment',4),(195,'mod/assignment:exportownsubmission','read',70,'mod_assignment',0),(196,'mod/book:addinstance','write',50,'mod_book',4),(197,'mod/book:read','read',70,'mod_book',0),(198,'mod/book:viewhiddenchapters','read',70,'mod_book',0),(199,'mod/book:edit','write',70,'mod_book',4),(200,'mod/chat:addinstance','write',50,'mod_chat',4),(201,'mod/chat:chat','write',70,'mod_chat',16),(202,'mod/chat:readlog','read',70,'mod_chat',0),(203,'mod/chat:deletelog','write',70,'mod_chat',0),(204,'mod/chat:exportparticipatedsession','read',70,'mod_chat',8),(205,'mod/chat:exportsession','read',70,'mod_chat',8),(206,'mod/choice:addinstance','write',50,'mod_choice',4),(207,'mod/choice:choose','write',70,'mod_choice',0),(208,'mod/choice:readresponses','read',70,'mod_choice',0),(209,'mod/choice:deleteresponses','write',70,'mod_choice',0),(210,'mod/choice:downloadresponses','read',70,'mod_choice',0),(211,'mod/data:addinstance','write',50,'mod_data',4),(212,'mod/data:viewentry','read',70,'mod_data',0),(213,'mod/data:writeentry','write',70,'mod_data',16),(214,'mod/data:comment','write',70,'mod_data',16),(215,'mod/data:rate','write',70,'mod_data',0),(216,'mod/data:viewrating','read',70,'mod_data',0),(217,'mod/data:viewanyrating','read',70,'mod_data',8),(218,'mod/data:viewallratings','read',70,'mod_data',8),(219,'mod/data:approve','write',70,'mod_data',16),(220,'mod/data:manageentries','write',70,'mod_data',16),(221,'mod/data:managecomments','write',70,'mod_data',16),(222,'mod/data:managetemplates','write',70,'mod_data',20),(223,'mod/data:viewalluserpresets','read',70,'mod_data',0),(224,'mod/data:manageuserpresets','write',70,'mod_data',20),(225,'mod/data:exportentry','read',70,'mod_data',8),(226,'mod/data:exportownentry','read',70,'mod_data',0),(227,'mod/data:exportallentries','read',70,'mod_data',8),(228,'mod/data:exportuserinfo','read',70,'mod_data',8),(229,'mod/feedback:addinstance','write',50,'mod_feedback',4),(230,'mod/feedback:view','read',70,'mod_feedback',0),(231,'mod/feedback:complete','write',70,'mod_feedback',16),(232,'mod/feedback:viewanalysepage','read',70,'mod_feedback',8),(233,'mod/feedback:deletesubmissions','write',70,'mod_feedback',0),(234,'mod/feedback:mapcourse','write',70,'mod_feedback',0),(235,'mod/feedback:edititems','write',70,'mod_feedback',20),(236,'mod/feedback:createprivatetemplate','write',70,'mod_feedback',16),(237,'mod/feedback:createpublictemplate','write',70,'mod_feedback',16),(238,'mod/feedback:deletetemplate','write',70,'mod_feedback',0),(239,'mod/feedback:viewreports','read',70,'mod_feedback',8),(240,'mod/feedback:receivemail','read',70,'mod_feedback',8),(241,'mod/folder:addinstance','write',50,'mod_folder',4),(242,'mod/folder:view','read',70,'mod_folder',0),(243,'mod/folder:managefiles','write',70,'mod_folder',16),(244,'mod/forum:addinstance','write',50,'mod_forum',4),(245,'mod/forum:viewdiscussion','read',70,'mod_forum',0),(246,'mod/forum:viewhiddentimedposts','read',70,'mod_forum',0),(247,'mod/forum:startdiscussion','write',70,'mod_forum',16),(248,'mod/forum:replypost','write',70,'mod_forum',16),(249,'mod/forum:addnews','write',70,'mod_forum',16),(250,'mod/forum:replynews','write',70,'mod_forum',16),(251,'mod/forum:viewrating','read',70,'mod_forum',0),(252,'mod/forum:viewanyrating','read',70,'mod_forum',8),(253,'mod/forum:viewallratings','read',70,'mod_forum',8),(254,'mod/forum:rate','write',70,'mod_forum',0),(255,'mod/forum:createattachment','write',70,'mod_forum',16),(256,'mod/forum:deleteownpost','read',70,'mod_forum',0),(257,'mod/forum:deleteanypost','read',70,'mod_forum',0),(258,'mod/forum:splitdiscussions','read',70,'mod_forum',0),(259,'mod/forum:movediscussions','read',70,'mod_forum',0),(260,'mod/forum:editanypost','write',70,'mod_forum',16),(261,'mod/forum:viewqandawithoutposting','read',70,'mod_forum',0),(262,'mod/forum:viewsubscribers','read',70,'mod_forum',0),(263,'mod/forum:managesubscriptions','read',70,'mod_forum',16),(264,'mod/forum:postwithoutthrottling','write',70,'mod_forum',16),(265,'mod/forum:exportdiscussion','read',70,'mod_forum',8),(266,'mod/forum:exportpost','read',70,'mod_forum',8),(267,'mod/forum:exportownpost','read',70,'mod_forum',8),(268,'mod/forum:addquestion','write',70,'mod_forum',16),(269,'mod/forum:allowforcesubscribe','read',70,'mod_forum',0),(270,'mod/glossary:addinstance','write',50,'mod_glossary',4),(271,'mod/glossary:view','read',70,'mod_glossary',0),(272,'mod/glossary:write','write',70,'mod_glossary',16),(273,'mod/glossary:manageentries','write',70,'mod_glossary',16),(274,'mod/glossary:managecategories','write',70,'mod_glossary',16),(275,'mod/glossary:comment','write',70,'mod_glossary',16),(276,'mod/glossary:managecomments','write',70,'mod_glossary',16),(277,'mod/glossary:import','write',70,'mod_glossary',16),(278,'mod/glossary:export','read',70,'mod_glossary',0),(279,'mod/glossary:approve','write',70,'mod_glossary',16),(280,'mod/glossary:rate','write',70,'mod_glossary',0),(281,'mod/glossary:viewrating','read',70,'mod_glossary',0),(282,'mod/glossary:viewanyrating','read',70,'mod_glossary',8),(283,'mod/glossary:viewallratings','read',70,'mod_glossary',8),(284,'mod/glossary:exportentry','read',70,'mod_glossary',8),(285,'mod/glossary:exportownentry','read',70,'mod_glossary',0),(286,'mod/imscp:view','read',70,'mod_imscp',0),(287,'mod/imscp:addinstance','write',50,'mod_imscp',4),(288,'mod/label:addinstance','write',50,'mod_label',4),(289,'mod/lesson:addinstance','write',50,'mod_lesson',4),(290,'mod/lesson:edit','write',70,'mod_lesson',4),(291,'mod/lesson:manage','write',70,'mod_lesson',0),(292,'mod/lti:view','read',70,'mod_lti',0),(293,'mod/lti:addinstance','write',50,'mod_lti',4),(294,'mod/lti:grade','write',70,'mod_lti',8),(295,'mod/lti:manage','write',70,'mod_lti',8),(296,'mod/lti:addcoursetool','write',50,'mod_lti',0),(297,'mod/lti:requesttooladd','write',50,'mod_lti',0),(298,'mod/page:view','read',70,'mod_page',0),(299,'mod/page:addinstance','write',50,'mod_page',4),(300,'mod/quiz:view','read',70,'mod_quiz',0),(301,'mod/quiz:addinstance','write',50,'mod_quiz',4),(302,'mod/quiz:attempt','write',70,'mod_quiz',16),(303,'mod/quiz:reviewmyattempts','read',70,'mod_quiz',0),(304,'mod/quiz:manage','write',70,'mod_quiz',16),(305,'mod/quiz:manageoverrides','write',70,'mod_quiz',0),(306,'mod/quiz:preview','write',70,'mod_quiz',0),(307,'mod/quiz:grade','write',70,'mod_quiz',16),(308,'mod/quiz:regrade','write',70,'mod_quiz',16),(309,'mod/quiz:viewreports','read',70,'mod_quiz',8),(310,'mod/quiz:deleteattempts','write',70,'mod_quiz',32),(311,'mod/quiz:ignoretimelimits','read',70,'mod_quiz',0),(312,'mod/quiz:emailconfirmsubmission','read',70,'mod_quiz',0),(313,'mod/quiz:emailnotifysubmission','read',70,'mod_quiz',0),(314,'mod/quiz:emailwarnoverdue','read',70,'mod_quiz',0),(315,'mod/resource:view','read',70,'mod_resource',0),(316,'mod/resource:addinstance','write',50,'mod_resource',4),(317,'mod/scorm:addinstance','write',50,'mod_scorm',4),(318,'mod/scorm:viewreport','read',70,'mod_scorm',0),(319,'mod/scorm:skipview','write',70,'mod_scorm',0),(320,'mod/scorm:savetrack','write',70,'mod_scorm',0),(321,'mod/scorm:viewscores','read',70,'mod_scorm',0),(322,'mod/scorm:deleteresponses','read',70,'mod_scorm',0),(323,'mod/scorm:deleteownresponses','write',70,'mod_scorm',0),(324,'mod/survey:addinstance','write',50,'mod_survey',4),(325,'mod/survey:participate','read',70,'mod_survey',0),(326,'mod/survey:readresponses','read',70,'mod_survey',0),(327,'mod/survey:download','read',70,'mod_survey',0),(328,'mod/url:view','read',70,'mod_url',0),(329,'mod/url:addinstance','write',50,'mod_url',4),(330,'mod/wiki:addinstance','write',50,'mod_wiki',4),(331,'mod/wiki:viewpage','read',70,'mod_wiki',0),(332,'mod/wiki:editpage','write',70,'mod_wiki',16),(333,'mod/wiki:createpage','write',70,'mod_wiki',16),(334,'mod/wiki:viewcomment','read',70,'mod_wiki',0),(335,'mod/wiki:editcomment','write',70,'mod_wiki',16),(336,'mod/wiki:managecomment','write',70,'mod_wiki',0),(337,'mod/wiki:managefiles','write',70,'mod_wiki',0),(338,'mod/wiki:overridelock','write',70,'mod_wiki',0),(339,'mod/wiki:managewiki','write',70,'mod_wiki',0),(340,'mod/workshop:view','read',70,'mod_workshop',0),(341,'mod/workshop:addinstance','write',50,'mod_workshop',4),(342,'mod/workshop:switchphase','write',70,'mod_workshop',0),(343,'mod/workshop:editdimensions','write',70,'mod_workshop',4),(344,'mod/workshop:submit','write',70,'mod_workshop',0),(345,'mod/workshop:peerassess','write',70,'mod_workshop',0),(346,'mod/workshop:manageexamples','write',70,'mod_workshop',0),(347,'mod/workshop:allocate','write',70,'mod_workshop',0),(348,'mod/workshop:publishsubmissions','write',70,'mod_workshop',0),(349,'mod/workshop:viewauthornames','read',70,'mod_workshop',0),(350,'mod/workshop:viewreviewernames','read',70,'mod_workshop',0),(351,'mod/workshop:viewallsubmissions','read',70,'mod_workshop',0),(352,'mod/workshop:viewpublishedsubmissions','read',70,'mod_workshop',0),(353,'mod/workshop:viewauthorpublished','read',70,'mod_workshop',0),(354,'mod/workshop:viewallassessments','read',70,'mod_workshop',0),(355,'mod/workshop:overridegrades','write',70,'mod_workshop',0),(356,'mod/workshop:ignoredeadlines','write',70,'mod_workshop',0),(357,'enrol/category:synchronised','write',10,'enrol_category',0),(358,'enrol/cohort:config','write',50,'enrol_cohort',0),(359,'enrol/cohort:unenrol','write',50,'enrol_cohort',0),(360,'enrol/database:unenrol','write',50,'enrol_database',0),(361,'enrol/flatfile:manage','write',50,'enrol_flatfile',0),(362,'enrol/flatfile:unenrol','write',50,'enrol_flatfile',0),(363,'enrol/guest:config','write',50,'enrol_guest',0),(364,'enrol/ldap:manage','write',50,'enrol_ldap',0),(365,'enrol/manual:config','write',50,'enrol_manual',0),(366,'enrol/manual:enrol','write',50,'enrol_manual',0),(367,'enrol/manual:manage','write',50,'enrol_manual',0),(368,'enrol/manual:unenrol','write',50,'enrol_manual',0),(369,'enrol/manual:unenrolself','write',50,'enrol_manual',0),(370,'enrol/meta:config','write',50,'enrol_meta',0),(371,'enrol/meta:selectaslinked','read',50,'enrol_meta',0),(372,'enrol/meta:unenrol','write',50,'enrol_meta',0),(373,'enrol/paypal:config','write',50,'enrol_paypal',0),(374,'enrol/paypal:manage','write',50,'enrol_paypal',0),(375,'enrol/paypal:unenrol','write',50,'enrol_paypal',0),(376,'enrol/paypal:unenrolself','write',50,'enrol_paypal',0),(377,'enrol/self:config','write',50,'enrol_self',0),(378,'enrol/self:manage','write',50,'enrol_self',0),(379,'enrol/self:unenrolself','write',50,'enrol_self',0),(380,'enrol/self:unenrol','write',50,'enrol_self',0),(381,'message/airnotifier:managedevice','write',10,'message_airnotifier',0),(382,'block/activity_modules:addinstance','write',80,'block_activity_modules',20),(383,'block/admin_bookmarks:myaddinstance','write',10,'block_admin_bookmarks',0),(384,'block/admin_bookmarks:addinstance','write',80,'block_admin_bookmarks',20),(385,'block/badges:addinstance','read',80,'block_badges',0),(386,'block/badges:myaddinstance','read',10,'block_badges',8),(387,'block/blog_menu:addinstance','write',80,'block_blog_menu',20),(388,'block/blog_recent:addinstance','write',80,'block_blog_recent',20),(389,'block/blog_tags:addinstance','write',80,'block_blog_tags',20),(390,'block/calendar_month:myaddinstance','write',10,'block_calendar_month',0),(391,'block/calendar_month:addinstance','write',80,'block_calendar_month',20),(392,'block/calendar_upcoming:myaddinstance','write',10,'block_calendar_upcoming',0),(393,'block/calendar_upcoming:addinstance','write',80,'block_calendar_upcoming',20),(394,'block/comments:myaddinstance','write',10,'block_comments',0),(395,'block/comments:addinstance','write',80,'block_comments',20),(396,'block/community:myaddinstance','write',10,'block_community',0),(397,'block/community:addinstance','write',80,'block_community',20),(398,'block/completionstatus:addinstance','write',80,'block_completionstatus',20),(399,'block/course_list:myaddinstance','write',10,'block_course_list',0),(400,'block/course_list:addinstance','write',80,'block_course_list',20),(401,'block/course_overview:myaddinstance','write',10,'block_course_overview',0),(402,'block/course_overview:addinstance','write',80,'block_course_overview',20),(403,'block/course_summary:addinstance','write',80,'block_course_summary',20),(404,'block/feedback:addinstance','write',80,'block_feedback',20),(405,'block/glossary_random:myaddinstance','write',10,'block_glossary_random',0),(406,'block/glossary_random:addinstance','write',80,'block_glossary_random',20),(407,'block/html:myaddinstance','write',10,'block_html',0),(408,'block/html:addinstance','write',80,'block_html',20),(409,'block/login:addinstance','write',80,'block_login',20),(410,'block/mentees:myaddinstance','write',10,'block_mentees',0),(411,'block/mentees:addinstance','write',80,'block_mentees',20),(412,'block/messages:myaddinstance','write',10,'block_messages',0),(413,'block/messages:addinstance','write',80,'block_messages',20),(414,'block/mnet_hosts:myaddinstance','write',10,'block_mnet_hosts',0),(415,'block/mnet_hosts:addinstance','write',80,'block_mnet_hosts',20),(416,'block/myprofile:myaddinstance','write',10,'block_myprofile',0),(417,'block/myprofile:addinstance','write',80,'block_myprofile',20),(418,'block/navigation:myaddinstance','write',10,'block_navigation',0),(419,'block/navigation:addinstance','write',80,'block_navigation',20),(420,'block/news_items:myaddinstance','write',10,'block_news_items',0),(421,'block/news_items:addinstance','write',80,'block_news_items',20),(422,'block/online_users:myaddinstance','write',10,'block_online_users',0),(423,'block/online_users:addinstance','write',80,'block_online_users',20),(424,'block/online_users:viewlist','read',80,'block_online_users',0),(425,'block/participants:addinstance','write',80,'block_participants',20),(426,'block/private_files:myaddinstance','write',10,'block_private_files',0),(427,'block/private_files:addinstance','write',80,'block_private_files',20),(428,'block/quiz_results:addinstance','write',80,'block_quiz_results',20),(429,'block/recent_activity:addinstance','write',80,'block_recent_activity',20),(430,'block/recent_activity:viewaddupdatemodule','read',50,'block_recent_activity',0),(431,'block/recent_activity:viewdeletemodule','read',50,'block_recent_activity',0),(432,'block/rss_client:myaddinstance','write',10,'block_rss_client',0),(433,'block/rss_client:addinstance','write',80,'block_rss_client',20),(434,'block/rss_client:manageownfeeds','write',80,'block_rss_client',0),(435,'block/rss_client:manageanyfeeds','write',80,'block_rss_client',16),(436,'block/search_forums:addinstance','write',80,'block_search_forums',20),(437,'block/section_links:addinstance','write',80,'block_section_links',20),(438,'block/selfcompletion:addinstance','write',80,'block_selfcompletion',20),(439,'block/settings:myaddinstance','write',10,'block_settings',0),(440,'block/settings:addinstance','write',80,'block_settings',20),(441,'block/site_main_menu:addinstance','write',80,'block_site_main_menu',20),(442,'block/social_activities:addinstance','write',80,'block_social_activities',20),(443,'block/tag_flickr:addinstance','write',80,'block_tag_flickr',20),(444,'block/tag_youtube:addinstance','write',80,'block_tag_youtube',20),(445,'block/tags:myaddinstance','write',10,'block_tags',0),(446,'block/tags:addinstance','write',80,'block_tags',20),(447,'report/completion:view','read',50,'report_completion',8),(448,'report/courseoverview:view','read',10,'report_courseoverview',8),(449,'report/log:view','read',50,'report_log',8),(450,'report/log:viewtoday','read',50,'report_log',8),(451,'report/loglive:view','read',50,'report_loglive',8),(452,'report/outline:view','read',50,'report_outline',8),(453,'report/participation:view','read',50,'report_participation',8),(454,'report/performance:view','read',10,'report_performance',2),(455,'report/progress:view','read',50,'report_progress',8),(456,'report/questioninstances:view','read',10,'report_questioninstances',0),(457,'report/security:view','read',10,'report_security',2),(458,'report/stats:view','read',50,'report_stats',8),(459,'gradeexport/ods:view','read',50,'gradeexport_ods',8),(460,'gradeexport/ods:publish','read',50,'gradeexport_ods',8),(461,'gradeexport/txt:view','read',50,'gradeexport_txt',8),(462,'gradeexport/txt:publish','read',50,'gradeexport_txt',8),(463,'gradeexport/xls:view','read',50,'gradeexport_xls',8),(464,'gradeexport/xls:publish','read',50,'gradeexport_xls',8),(465,'gradeexport/xml:view','read',50,'gradeexport_xml',8),(466,'gradeexport/xml:publish','read',50,'gradeexport_xml',8),(467,'gradeimport/csv:view','write',50,'gradeimport_csv',0),(468,'gradeimport/xml:view','write',50,'gradeimport_xml',0),(469,'gradeimport/xml:publish','write',50,'gradeimport_xml',0),(470,'gradereport/grader:view','read',50,'gradereport_grader',8),(471,'gradereport/outcomes:view','read',50,'gradereport_outcomes',8),(472,'gradereport/overview:view','read',50,'gradereport_overview',8),(473,'gradereport/user:view','read',50,'gradereport_user',8),(474,'webservice/amf:use','read',50,'webservice_amf',62),(475,'webservice/rest:use','read',50,'webservice_rest',62),(476,'webservice/soap:use','read',50,'webservice_soap',62),(477,'webservice/xmlrpc:use','read',50,'webservice_xmlrpc',62),(478,'repository/alfresco:view','read',70,'repository_alfresco',0),(479,'repository/areafiles:view','read',70,'repository_areafiles',0),(480,'repository/boxnet:view','read',70,'repository_boxnet',0),(481,'repository/coursefiles:view','read',70,'repository_coursefiles',0),(482,'repository/dropbox:view','read',70,'repository_dropbox',0),(483,'repository/equella:view','read',70,'repository_equella',0),(484,'repository/filesystem:view','read',70,'repository_filesystem',0),(485,'repository/flickr:view','read',70,'repository_flickr',0),(486,'repository/flickr_public:view','read',70,'repository_flickr_public',0),(487,'repository/googledocs:view','read',70,'repository_googledocs',0),(488,'repository/local:view','read',70,'repository_local',0),(489,'repository/merlot:view','read',70,'repository_merlot',0),(490,'repository/picasa:view','read',70,'repository_picasa',0),(491,'repository/recent:view','read',70,'repository_recent',0),(492,'repository/s3:view','read',70,'repository_s3',0),(493,'repository/skydrive:view','read',70,'repository_skydrive',0),(494,'repository/upload:view','read',70,'repository_upload',0),(495,'repository/url:view','read',70,'repository_url',0),(496,'repository/user:view','read',70,'repository_user',0),(497,'repository/webdav:view','read',70,'repository_webdav',0),(498,'repository/wikimedia:view','read',70,'repository_wikimedia',0),(499,'repository/youtube:view','read',70,'repository_youtube',0),(500,'tool/customlang:view','read',10,'tool_customlang',2),(501,'tool/customlang:edit','write',10,'tool_customlang',6),(502,'tool/uploaduser:uploaduserpictures','write',10,'tool_uploaduser',16),(503,'booktool/exportimscp:export','read',70,'booktool_exportimscp',0),(504,'booktool/importhtml:import','write',70,'booktool_importhtml',4),(505,'booktool/print:print','read',70,'booktool_print',0),(506,'quiz/grading:viewstudentnames','read',70,'quiz_grading',0),(507,'quiz/grading:viewidnumber','read',70,'quiz_grading',0),(508,'quiz/statistics:view','read',70,'quiz_statistics',0),(509,'enrol/elis:config','write',50,'enrol_elis',0),(510,'enrol/elis:unenrol','write',50,'enrol_elis',0),(511,'block/courserequest:request','write',10,'block_courserequest',0),(512,'block/courserequest:config','write',10,'block_courserequest',0),(513,'block/courserequest:approve','write',10,'block_courserequest',0),(514,'block/courserequest:addinstance','write',80,'block_courserequest',0),(515,'block/elisadmin:addinstance','write',80,'block_elisadmin',10),(516,'block/enrolsurvey:edit','write',10,'block_enrolsurvey',40),(517,'block/enrolsurvey:take','write',10,'block_enrolsurvey',32),(518,'block/enrolsurvey:addinstance','write',80,'block_enrolsurvey',0),(519,'block/repository:addinstance','write',80,'block_repository',20),(520,'repository/elisfiles:view','read',70,'repository_elisfiles',0),(521,'repository/elisfiles:createsitecontent','write',10,'repository_elisfiles',36),(522,'repository/elisfiles:viewsitecontent','read',10,'repository_elisfiles',0),(523,'repository/elisfiles:createsharedcontent','write',10,'repository_elisfiles',36),(524,'repository/elisfiles:viewsharedcontent','read',10,'repository_elisfiles',0),(525,'repository/elisfiles:createcoursecontent','write',50,'repository_elisfiles',36),(526,'repository/elisfiles:viewcoursecontent','read',50,'repository_elisfiles',0),(527,'repository/elisfiles:createowncontent','write',10,'repository_elisfiles',0),(528,'repository/elisfiles:viewowncontent','read',10,'repository_elisfiles',0),(529,'repository/elisfiles:createusersetcontent','write',10,'repository_elisfiles',36),(530,'repository/elisfiles:viewusersetcontent','read',10,'repository_elisfiles',0),(531,'local/datahub:addinstance','write',80,'local_datahub',60),(532,'local/elisprogram:config','write',10,'local_elisprogram',62),(533,'local/elisprogram:manage','write',10,'local_elisprogram',40),(534,'local/elisprogram:program_view','read',10,'local_elisprogram',40),(535,'local/elisprogram:program_create','write',10,'local_elisprogram',40),(536,'local/elisprogram:program_edit','write',10,'local_elisprogram',40),(537,'local/elisprogram:program_delete','write',10,'local_elisprogram',40),(538,'local/elisprogram:program_enrol','write',10,'local_elisprogram',40),(539,'local/elisprogram:track_view','read',10,'local_elisprogram',40),(540,'local/elisprogram:track_create','write',10,'local_elisprogram',40),(541,'local/elisprogram:track_edit','write',10,'local_elisprogram',40),(542,'local/elisprogram:track_delete','write',10,'local_elisprogram',40),(543,'local/elisprogram:track_enrol','write',10,'local_elisprogram',40),(544,'local/elisprogram:userset_view','read',10,'local_elisprogram',40),(545,'local/elisprogram:userset_create','write',10,'local_elisprogram',40),(546,'local/elisprogram:userset_edit','write',10,'local_elisprogram',40),(547,'local/elisprogram:userset_delete','write',10,'local_elisprogram',40),(548,'local/elisprogram:userset_enrol','write',10,'local_elisprogram',40),(549,'local/elisprogram:course_view','read',10,'local_elisprogram',40),(550,'local/elisprogram:course_create','write',10,'local_elisprogram',40),(551,'local/elisprogram:course_edit','write',10,'local_elisprogram',40),(552,'local/elisprogram:course_delete','write',10,'local_elisprogram',40),(553,'local/elisprogram:class_view','read',10,'local_elisprogram',40),(554,'local/elisprogram:class_create','write',10,'local_elisprogram',40),(555,'local/elisprogram:class_edit','write',10,'local_elisprogram',40),(556,'local/elisprogram:class_delete','write',10,'local_elisprogram',40),(557,'local/elisprogram:class_enrol','write',10,'local_elisprogram',40),(558,'local/elisprogram:assign_class_instructor','write',10,'local_elisprogram',40),(559,'local/elisprogram:user_view','read',10,'local_elisprogram',40),(560,'local/elisprogram:user_create','write',10,'local_elisprogram',40),(561,'local/elisprogram:user_edit','write',10,'local_elisprogram',40),(562,'local/elisprogram:user_delete','write',10,'local_elisprogram',40),(563,'local/elisprogram:viewownreports','read',10,'local_elisprogram',0),(564,'local/elisprogram:managefiles','write',10,'local_elisprogram',44),(565,'local/elisprogram:notify_trackenrol','read',10,'local_elisprogram',8),(566,'local/elisprogram:notify_classenrol','read',50,'local_elisprogram',8),(567,'local/elisprogram:notify_classcomplete','read',50,'local_elisprogram',8),(568,'local/elisprogram:notify_classnotstart','read',50,'local_elisprogram',8),(569,'local/elisprogram:notify_classnotcomplete','read',50,'local_elisprogram',8),(570,'local/elisprogram:notify_courserecurrence','read',10,'local_elisprogram',8),(571,'local/elisprogram:notify_programrecurrence','read',10,'local_elisprogram',8),(572,'local/elisprogram:notify_programcomplete','read',10,'local_elisprogram',8),(573,'local/elisprogram:notify_programnotcomplete','read',10,'local_elisprogram',8),(574,'local/elisprogram:notify_programdue','read',10,'local_elisprogram',8),(575,'local/elisprogram:notify_coursedue','read',10,'local_elisprogram',8),(576,'local/elisprogram:program_enrol_userset_user','write',10,'local_elisprogram',40),(577,'local/elisprogram:track_enrol_userset_user','write',10,'local_elisprogram',40),(578,'local/elisprogram:userset_enrol_userset_user','write',10,'local_elisprogram',40),(579,'local/elisprogram:class_enrol_userset_user','write',10,'local_elisprogram',40),(580,'local/elisprogram:assign_userset_user_class_instructor','write',10,'local_elisprogram',40),(581,'local/elisprogram:viewcoursecatalog','read',10,'local_elisprogram',0),(582,'local/elisprogram:overrideclasslimit','write',10,'local_elisprogram',0),(583,'local/elisprogram:userset_role_assign_userset_users','write',10,'local_elisprogram',0),(584,'local/elisprogram:associate','write',10,'local_elisprogram',0),(585,'local/elisreports:view','write',10,'local_elisreports',8),(586,'local/elisreports:schedule','write',10,'local_elisreports',8),(587,'local/elisreports:manageschedules','write',10,'local_elisreports',40);

/*Table structure for table `mdl_chat` */

DROP TABLE IF EXISTS `mdl_chat`;

CREATE TABLE `mdl_chat` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci NOT NULL,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `keepdays` bigint(11) NOT NULL DEFAULT '0',
  `studentlogs` smallint(4) NOT NULL DEFAULT '0',
  `chattime` bigint(10) NOT NULL DEFAULT '0',
  `schedule` smallint(4) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_chat_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Each of these is a chat room';

/*Data for the table `mdl_chat` */

/*Table structure for table `mdl_chat_messages` */

DROP TABLE IF EXISTS `mdl_chat_messages`;

CREATE TABLE `mdl_chat_messages` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `chatid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `groupid` bigint(10) NOT NULL DEFAULT '0',
  `system` tinyint(1) NOT NULL DEFAULT '0',
  `message` longtext COLLATE utf8_unicode_ci NOT NULL,
  `timestamp` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_chatmess_use_ix` (`userid`),
  KEY `mdl_chatmess_gro_ix` (`groupid`),
  KEY `mdl_chatmess_timcha_ix` (`timestamp`,`chatid`),
  KEY `mdl_chatmess_cha_ix` (`chatid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores all the actual chat messages';

/*Data for the table `mdl_chat_messages` */

/*Table structure for table `mdl_chat_messages_current` */

DROP TABLE IF EXISTS `mdl_chat_messages_current`;

CREATE TABLE `mdl_chat_messages_current` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `chatid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `groupid` bigint(10) NOT NULL DEFAULT '0',
  `system` tinyint(1) NOT NULL DEFAULT '0',
  `message` longtext COLLATE utf8_unicode_ci NOT NULL,
  `timestamp` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_chatmesscurr_use_ix` (`userid`),
  KEY `mdl_chatmesscurr_gro_ix` (`groupid`),
  KEY `mdl_chatmesscurr_timcha_ix` (`timestamp`,`chatid`),
  KEY `mdl_chatmesscurr_cha_ix` (`chatid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores current session';

/*Data for the table `mdl_chat_messages_current` */

/*Table structure for table `mdl_chat_users` */

DROP TABLE IF EXISTS `mdl_chat_users`;

CREATE TABLE `mdl_chat_users` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `chatid` bigint(11) NOT NULL DEFAULT '0',
  `userid` bigint(11) NOT NULL DEFAULT '0',
  `groupid` bigint(11) NOT NULL DEFAULT '0',
  `version` varchar(16) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `ip` varchar(45) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `firstping` bigint(10) NOT NULL DEFAULT '0',
  `lastping` bigint(10) NOT NULL DEFAULT '0',
  `lastmessageping` bigint(10) NOT NULL DEFAULT '0',
  `sid` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `course` bigint(10) NOT NULL DEFAULT '0',
  `lang` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_chatuser_use_ix` (`userid`),
  KEY `mdl_chatuser_las_ix` (`lastping`),
  KEY `mdl_chatuser_gro_ix` (`groupid`),
  KEY `mdl_chatuser_cha_ix` (`chatid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Keeps track of which users are in which chat rooms';

/*Data for the table `mdl_chat_users` */

/*Table structure for table `mdl_choice` */

DROP TABLE IF EXISTS `mdl_choice`;

CREATE TABLE `mdl_choice` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci NOT NULL,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `publish` tinyint(2) NOT NULL DEFAULT '0',
  `showresults` tinyint(2) NOT NULL DEFAULT '0',
  `display` smallint(4) NOT NULL DEFAULT '0',
  `allowupdate` tinyint(2) NOT NULL DEFAULT '0',
  `showunanswered` tinyint(2) NOT NULL DEFAULT '0',
  `limitanswers` tinyint(2) NOT NULL DEFAULT '0',
  `timeopen` bigint(10) NOT NULL DEFAULT '0',
  `timeclose` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `completionsubmit` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_choi_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Available choices are stored here';

/*Data for the table `mdl_choice` */

/*Table structure for table `mdl_choice_answers` */

DROP TABLE IF EXISTS `mdl_choice_answers`;

CREATE TABLE `mdl_choice_answers` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `choiceid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `optionid` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_choiansw_use_ix` (`userid`),
  KEY `mdl_choiansw_cho_ix` (`choiceid`),
  KEY `mdl_choiansw_opt_ix` (`optionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='choices performed by users';

/*Data for the table `mdl_choice_answers` */

/*Table structure for table `mdl_choice_options` */

DROP TABLE IF EXISTS `mdl_choice_options`;

CREATE TABLE `mdl_choice_options` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `choiceid` bigint(10) NOT NULL DEFAULT '0',
  `text` longtext COLLATE utf8_unicode_ci,
  `maxanswers` bigint(10) DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_choiopti_cho_ix` (`choiceid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='available options to choice';

/*Data for the table `mdl_choice_options` */

/*Table structure for table `mdl_cohort` */

DROP TABLE IF EXISTS `mdl_cohort`;

CREATE TABLE `mdl_cohort` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `contextid` bigint(10) NOT NULL,
  `name` varchar(254) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `idnumber` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` tinyint(2) NOT NULL,
  `component` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timecreated` bigint(10) NOT NULL,
  `timemodified` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_coho_con_ix` (`contextid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Each record represents one cohort (aka site-wide group).';

/*Data for the table `mdl_cohort` */

/*Table structure for table `mdl_cohort_members` */

DROP TABLE IF EXISTS `mdl_cohort_members`;

CREATE TABLE `mdl_cohort_members` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `cohortid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `timeadded` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_cohomemb_cohuse_uix` (`cohortid`,`userid`),
  KEY `mdl_cohomemb_coh_ix` (`cohortid`),
  KEY `mdl_cohomemb_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Link a user to a cohort.';

/*Data for the table `mdl_cohort_members` */

/*Table structure for table `mdl_comments` */

DROP TABLE IF EXISTS `mdl_comments`;

CREATE TABLE `mdl_comments` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `contextid` bigint(10) NOT NULL,
  `commentarea` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `itemid` bigint(10) NOT NULL,
  `content` longtext COLLATE utf8_unicode_ci NOT NULL,
  `format` tinyint(2) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL,
  `timecreated` bigint(10) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='moodle comments module';

/*Data for the table `mdl_comments` */

/*Table structure for table `mdl_config` */

DROP TABLE IF EXISTS `mdl_config`;

CREATE TABLE `mdl_config` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` longtext COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_conf_nam_uix` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=449 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Moodle configuration variables';

/*Data for the table `mdl_config` */

insert  into `mdl_config`(`id`,`name`,`value`) values (2,'rolesactive','1'),(3,'auth','email,cliauth'),(4,'auth_pop3mailbox','INBOX'),(5,'enrol_plugins_enabled','manual,guest,self,cohort,elis'),(6,'theme','clean'),(7,'filter_multilang_converted','1'),(8,'siteidentifier','NBcxYuz5ssqD8mxT6Hg9C5i4k04VDpaTlms.sunnet.cc'),(9,'backup_version','2008111700'),(10,'backup_release','2.0 dev'),(11,'mnet_dispatcher_mode','off'),(12,'sessiontimeout','300'),(13,'stringfilters',''),(14,'filterall','0'),(15,'texteditors','atto,tinymce,textarea'),(16,'mnet_localhost_id','1'),(17,'mnet_all_hosts_id','2'),(18,'siteguest','1'),(19,'siteadmins','2'),(20,'themerev','1413861558'),(21,'jsrev','1413861558'),(22,'gdversion','2'),(23,'licenses','unknown,allrightsreserved,public,cc,cc-nd,cc-nc-nd,cc-nc,cc-nc-sa,cc-sa'),(24,'version','2014051202'),(25,'enableoutcomes','0'),(26,'usecomments','1'),(27,'usetags','1'),(28,'enablenotes','1'),(29,'enableportfolios','0'),(30,'enablewebservices','0'),(31,'messaging','1'),(32,'messaginghidereadnotifications','0'),(33,'messagingdeletereadnotificationsdelay','604800'),(34,'messagingallowemailoverride','0'),(35,'enablestats','0'),(36,'enablerssfeeds','0'),(37,'enableblogs','1'),(38,'enablecompletion','0'),(39,'completiondefault','1'),(40,'enableavailability','0'),(41,'enableplagiarism','0'),(42,'enablebadges','1'),(43,'autologinguests','0'),(44,'hiddenuserfields',''),(45,'showuseridentity','email'),(46,'fullnamedisplay','language'),(47,'maxusersperpage','100'),(48,'enablegravatar','0'),(49,'gravatardefaulturl','mm'),(50,'enablecourserequests','0'),(51,'defaultrequestcategory','1'),(52,'requestcategoryselection','0'),(53,'courserequestnotify',''),(54,'grade_profilereport','user'),(55,'grade_aggregationposition','1'),(56,'grade_includescalesinaggregation','1'),(57,'grade_hiddenasdate','0'),(58,'gradepublishing','0'),(59,'grade_export_displaytype','1'),(60,'grade_export_decimalpoints','2'),(61,'grade_navmethod','0'),(62,'grade_export_userprofilefields','firstname,lastname,idnumber,institution,department,email'),(63,'grade_export_customprofilefields',''),(64,'recovergradesdefault','0'),(65,'gradeexport',''),(66,'unlimitedgrades','0'),(67,'gradepointmax','100'),(68,'gradepointdefault','100'),(69,'grade_hideforcedsettings','1'),(70,'grade_aggregation','11'),(71,'grade_aggregation_flag','0'),(72,'grade_aggregations_visible','0,10,11,12,2,4,6,8,13'),(73,'grade_aggregateonlygraded','1'),(74,'grade_aggregateonlygraded_flag','2'),(75,'grade_aggregateoutcomes','0'),(76,'grade_aggregateoutcomes_flag','2'),(77,'grade_aggregatesubcats','0'),(78,'grade_aggregatesubcats_flag','2'),(79,'grade_keephigh','0'),(80,'grade_keephigh_flag','3'),(81,'grade_droplow','0'),(82,'grade_droplow_flag','2'),(83,'grade_displaytype','1'),(84,'grade_decimalpoints','2'),(85,'grade_item_advanced','iteminfo,idnumber,gradepass,plusfactor,multfactor,display,decimals,hiddenuntil,locktime'),(86,'grade_report_studentsperpage','100'),(87,'grade_report_showonlyactiveenrol','1'),(88,'grade_report_quickgrading','1'),(89,'grade_report_showquickfeedback','0'),(90,'grade_report_fixedstudents','0'),(91,'grade_report_meanselection','1'),(92,'grade_report_enableajax','0'),(93,'grade_report_showcalculations','0'),(94,'grade_report_showeyecons','0'),(95,'grade_report_showaverages','1'),(96,'grade_report_showlocks','0'),(97,'grade_report_showranges','0'),(98,'grade_report_showanalysisicon','1'),(99,'grade_report_showuserimage','1'),(100,'grade_report_showactivityicons','1'),(101,'grade_report_shownumberofgrades','0'),(102,'grade_report_averagesdisplaytype','inherit'),(103,'grade_report_rangesdisplaytype','inherit'),(104,'grade_report_averagesdecimalpoints','inherit'),(105,'grade_report_rangesdecimalpoints','inherit'),(106,'grade_report_overview_showrank','0'),(107,'grade_report_overview_showtotalsifcontainhidden','0'),(108,'grade_report_user_showrank','0'),(109,'grade_report_user_showpercentage','1'),(110,'grade_report_user_showgrade','1'),(111,'grade_report_user_showfeedback','1'),(112,'grade_report_user_showrange','1'),(113,'grade_report_user_showweight','0'),(114,'grade_report_user_showaverage','0'),(115,'grade_report_user_showlettergrade','0'),(116,'grade_report_user_rangedecimals','0'),(117,'grade_report_user_showhiddenitems','1'),(118,'grade_report_user_showtotalsifcontainhidden','0'),(119,'badges_defaultissuername',''),(120,'badges_defaultissuercontact',''),(121,'badges_badgesalt','badges1413856721'),(122,'badges_allowexternalbackpack','1'),(123,'badges_allowcoursebadges','1'),(124,'timezone','99'),(125,'forcetimezone','99'),(126,'country','0'),(127,'defaultcity',''),(128,'geoipfile','E:\\PhpProject\\moodledata/geoip/GeoLiteCity.dat'),(129,'googlemapkey3',''),(130,'allcountrycodes',''),(131,'autolang','1'),(132,'lang','zh_cn'),(133,'langmenu','1'),(134,'langlist',''),(135,'langrev','1413861558'),(136,'langcache','1'),(137,'langstringcache','1'),(138,'locale',''),(139,'latinexcelexport','0'),(140,'filteruploadedfiles','0'),(141,'filtermatchoneperpage','0'),(142,'filtermatchonepertext','0'),(143,'repositorycacheexpire','120'),(144,'repositorygetfiletimeout','30'),(145,'repositorysyncfiletimeout','1'),(146,'repositorysyncimagetimeout','3'),(147,'repositoryallowexternallinks','1'),(148,'legacyfilesinnewcourses','0'),(149,'legacyfilesaddallowed','1'),(151,'authloginviaemail','0'),(152,'authpreventaccountcreation','0'),(153,'loginpageautofocus','0'),(154,'guestloginbutton','1'),(155,'alternateloginurl',''),(156,'forgottenpasswordurl',''),(157,'auth_instructions',''),(158,'allowemailaddresses',''),(159,'denyemailaddresses',''),(160,'verifychangedemail','1'),(161,'recaptchapublickey',''),(162,'recaptchaprivatekey',''),(163,'mobilecssurl',''),(164,'enablewsdocumentation','0'),(165,'sitedefaultlicense','allrightsreserved'),(166,'portfolio_moderate_filesize_threshold','1048576'),(167,'portfolio_high_filesize_threshold','5242880'),(168,'portfolio_moderate_db_threshold','20'),(169,'portfolio_high_db_threshold','50'),(170,'allowbeforeblock','0'),(171,'allowedip',''),(172,'blockedip',''),(173,'protectusernames','1'),(174,'forcelogin','1'),(175,'forceloginforprofiles','1'),(176,'forceloginforprofileimage','0'),(177,'opentogoogle','0'),(178,'maxbytes','0'),(179,'userquota','104857600'),(180,'allowobjectembed','0'),(181,'enabletrusttext','0'),(182,'maxeditingtime','1800'),(183,'extendedusernamechars','0'),(184,'sitepolicy',''),(185,'sitepolicyguest',''),(186,'keeptagnamecase','1'),(187,'profilesforenrolledusersonly','1'),(188,'cronclionly','0'),(189,'cronremotepassword',''),(190,'lockoutthreshold','0'),(191,'lockoutwindow','1800'),(192,'lockoutduration','1800'),(193,'passwordpolicy','1'),(194,'minpasswordlength','8'),(195,'minpassworddigits','1'),(196,'minpasswordlower','1'),(197,'minpasswordupper','1'),(198,'minpasswordnonalphanum','1'),(199,'maxconsecutiveidentchars','0'),(200,'pwresettime','1800'),(201,'groupenrolmentkeypolicy','1'),(202,'disableuserimages','0'),(203,'emailchangeconfirmation','1'),(204,'rememberusername','2'),(205,'strictformsrequired','0'),(206,'loginhttps','0'),(207,'cookiesecure','1'),(208,'cookiehttponly','1'),(209,'allowframembedding','0'),(210,'loginpasswordautocomplete','0'),(211,'displayloginfailures','0'),(212,'notifyloginfailures',''),(213,'notifyloginthreshold','10'),(214,'runclamonupload','0'),(215,'pathtoclam',''),(216,'quarantinedir',''),(217,'clamfailureonupload','donothing'),(218,'themelist',''),(219,'themedesignermode','0'),(220,'allowuserthemes','0'),(221,'allowcoursethemes','0'),(222,'allowcategorythemes','0'),(223,'allowthemechangeonurl','0'),(224,'allowuserblockhiding','1'),(225,'allowblockstodock','1'),(226,'custommenuitems',''),(227,'enabledevicedetection','1'),(228,'devicedetectregex','[]'),(229,'calendartype','gregorian'),(230,'calendar_adminseesall','0'),(231,'calendar_site_timeformat','0'),(232,'calendar_startwday','0'),(233,'calendar_weekend','65'),(234,'calendar_lookahead','21'),(235,'calendar_maxevents','10'),(236,'enablecalendarexport','1'),(237,'calendar_customexport','1'),(238,'calendar_exportlookahead','365'),(239,'calendar_exportlookback','5'),(240,'calendar_exportsalt','l0KszmRz5mgXM8bMhpSIAwtxDr6m3k51CXuLnKjfQTdYdFH8K2krgtsfmNJQ'),(241,'calendar_showicalsource','1'),(242,'useblogassociations','1'),(243,'bloglevel','4'),(244,'useexternalblogs','1'),(245,'externalblogcrontime','86400'),(246,'maxexternalblogsperuser','1'),(247,'blogusecomments','1'),(248,'blogshowcommentscount','1'),(249,'defaulthomepage','0'),(250,'allowguestmymoodle','1'),(251,'navshowfullcoursenames','0'),(252,'navshowcategories','1'),(253,'navshowmycoursecategories','0'),(254,'navshowallcourses','0'),(255,'navsortmycoursessort','sortorder'),(256,'navcourselimit','20'),(257,'usesitenameforsitepages','0'),(258,'linkadmincategories','0'),(259,'navshowfrontpagemods','1'),(260,'navadduserpostslinks','1'),(261,'formatstringstriptags','1'),(262,'emoticons','[{\"text\":\":-)\",\"imagename\":\"s\\/smiley\",\"imagecomponent\":\"core\",\"altidentifier\":\"smiley\",\"altcomponent\":\"core_pix\"},{\"text\":\":)\",\"imagename\":\"s\\/smiley\",\"imagecomponent\":\"core\",\"altidentifier\":\"smiley\",\"altcomponent\":\"core_pix\"},{\"text\":\":-D\",\"imagename\":\"s\\/biggrin\",\"imagecomponent\":\"core\",\"altidentifier\":\"biggrin\",\"altcomponent\":\"core_pix\"},{\"text\":\";-)\",\"imagename\":\"s\\/wink\",\"imagecomponent\":\"core\",\"altidentifier\":\"wink\",\"altcomponent\":\"core_pix\"},{\"text\":\":-\\/\",\"imagename\":\"s\\/mixed\",\"imagecomponent\":\"core\",\"altidentifier\":\"mixed\",\"altcomponent\":\"core_pix\"},{\"text\":\"V-.\",\"imagename\":\"s\\/thoughtful\",\"imagecomponent\":\"core\",\"altidentifier\":\"thoughtful\",\"altcomponent\":\"core_pix\"},{\"text\":\":-P\",\"imagename\":\"s\\/tongueout\",\"imagecomponent\":\"core\",\"altidentifier\":\"tongueout\",\"altcomponent\":\"core_pix\"},{\"text\":\":-p\",\"imagename\":\"s\\/tongueout\",\"imagecomponent\":\"core\",\"altidentifier\":\"tongueout\",\"altcomponent\":\"core_pix\"},{\"text\":\"B-)\",\"imagename\":\"s\\/cool\",\"imagecomponent\":\"core\",\"altidentifier\":\"cool\",\"altcomponent\":\"core_pix\"},{\"text\":\"^-)\",\"imagename\":\"s\\/approve\",\"imagecomponent\":\"core\",\"altidentifier\":\"approve\",\"altcomponent\":\"core_pix\"},{\"text\":\"8-)\",\"imagename\":\"s\\/wideeyes\",\"imagecomponent\":\"core\",\"altidentifier\":\"wideeyes\",\"altcomponent\":\"core_pix\"},{\"text\":\":o)\",\"imagename\":\"s\\/clown\",\"imagecomponent\":\"core\",\"altidentifier\":\"clown\",\"altcomponent\":\"core_pix\"},{\"text\":\":-(\",\"imagename\":\"s\\/sad\",\"imagecomponent\":\"core\",\"altidentifier\":\"sad\",\"altcomponent\":\"core_pix\"},{\"text\":\":(\",\"imagename\":\"s\\/sad\",\"imagecomponent\":\"core\",\"altidentifier\":\"sad\",\"altcomponent\":\"core_pix\"},{\"text\":\"8-.\",\"imagename\":\"s\\/shy\",\"imagecomponent\":\"core\",\"altidentifier\":\"shy\",\"altcomponent\":\"core_pix\"},{\"text\":\":-I\",\"imagename\":\"s\\/blush\",\"imagecomponent\":\"core\",\"altidentifier\":\"blush\",\"altcomponent\":\"core_pix\"},{\"text\":\":-X\",\"imagename\":\"s\\/kiss\",\"imagecomponent\":\"core\",\"altidentifier\":\"kiss\",\"altcomponent\":\"core_pix\"},{\"text\":\"8-o\",\"imagename\":\"s\\/surprise\",\"imagecomponent\":\"core\",\"altidentifier\":\"surprise\",\"altcomponent\":\"core_pix\"},{\"text\":\"P-|\",\"imagename\":\"s\\/blackeye\",\"imagecomponent\":\"core\",\"altidentifier\":\"blackeye\",\"altcomponent\":\"core_pix\"},{\"text\":\"8-[\",\"imagename\":\"s\\/angry\",\"imagecomponent\":\"core\",\"altidentifier\":\"angry\",\"altcomponent\":\"core_pix\"},{\"text\":\"(grr)\",\"imagename\":\"s\\/angry\",\"imagecomponent\":\"core\",\"altidentifier\":\"angry\",\"altcomponent\":\"core_pix\"},{\"text\":\"xx-P\",\"imagename\":\"s\\/dead\",\"imagecomponent\":\"core\",\"altidentifier\":\"dead\",\"altcomponent\":\"core_pix\"},{\"text\":\"|-.\",\"imagename\":\"s\\/sleepy\",\"imagecomponent\":\"core\",\"altidentifier\":\"sleepy\",\"altcomponent\":\"core_pix\"},{\"text\":\"}-]\",\"imagename\":\"s\\/evil\",\"imagecomponent\":\"core\",\"altidentifier\":\"evil\",\"altcomponent\":\"core_pix\"},{\"text\":\"(h)\",\"imagename\":\"s\\/heart\",\"imagecomponent\":\"core\",\"altidentifier\":\"heart\",\"altcomponent\":\"core_pix\"},{\"text\":\"(heart)\",\"imagename\":\"s\\/heart\",\"imagecomponent\":\"core\",\"altidentifier\":\"heart\",\"altcomponent\":\"core_pix\"},{\"text\":\"(y)\",\"imagename\":\"s\\/yes\",\"imagecomponent\":\"core\",\"altidentifier\":\"yes\",\"altcomponent\":\"core\"},{\"text\":\"(n)\",\"imagename\":\"s\\/no\",\"imagecomponent\":\"core\",\"altidentifier\":\"no\",\"altcomponent\":\"core\"},{\"text\":\"(martin)\",\"imagename\":\"s\\/martin\",\"imagecomponent\":\"core\",\"altidentifier\":\"martin\",\"altcomponent\":\"core_pix\"},{\"text\":\"( )\",\"imagename\":\"s\\/egg\",\"imagecomponent\":\"core\",\"altidentifier\":\"egg\",\"altcomponent\":\"core_pix\"}]'),(263,'core_media_enable_youtube','1'),(264,'core_media_enable_vimeo','0'),(265,'core_media_enable_mp3','1'),(266,'core_media_enable_flv','1'),(267,'core_media_enable_swf','1'),(268,'core_media_enable_html5audio','1'),(269,'core_media_enable_html5video','1'),(270,'core_media_enable_qt','1'),(271,'core_media_enable_wmp','1'),(272,'core_media_enable_rm','1'),(273,'docroot','http://docs.moodle.org'),(274,'doclang',''),(275,'doctonewwindow','0'),(276,'courselistshortnames','0'),(277,'coursesperpage','20'),(278,'courseswithsummarieslimit','10'),(279,'courseoverviewfileslimit','1'),(280,'courseoverviewfilesext','.jpg,.gif,.png'),(281,'useexternalyui','0'),(282,'yuicomboloading','1'),(283,'cachejs','1'),(284,'modchooserdefault','1'),(285,'modeditingmenu','1'),(286,'blockeditingmenu','1'),(287,'additionalhtmlhead',''),(288,'additionalhtmltopofbody',''),(289,'additionalhtmlfooter',''),(290,'pathtodu',''),(291,'aspellpath',''),(292,'pathtodot',''),(293,'supportpage',''),(294,'dbsessions','0'),(295,'sessioncookie','9772793214ED714F3082A04E18682F58'),(296,'sessioncookiepath','/'),(297,'sessioncookiedomain',''),(298,'statsfirstrun','none'),(299,'statsmaxruntime','0'),(300,'statsruntimedays','31'),(301,'statsruntimestarthour','0'),(302,'statsruntimestartminute','0'),(303,'statsuserthreshold','0'),(304,'slasharguments','1'),(305,'getremoteaddrconf','0'),(306,'proxyhost',''),(307,'proxyport','0'),(308,'proxytype','HTTP'),(309,'proxyuser',''),(310,'proxypassword',''),(311,'proxybypass','localhost, 127.0.0.1'),(312,'maintenance_enabled','0'),(313,'maintenance_message',''),(314,'deleteunconfirmed','168'),(315,'deleteincompleteusers','0'),(316,'disablegradehistory','0'),(317,'gradehistorylifetime','0'),(318,'extramemorylimit','512M'),(319,'maxtimelimit','0'),(320,'curlcache','120'),(321,'curltimeoutkbitrate','56'),(322,'updateautocheck','1'),(323,'updateautodeploy','0'),(324,'updateminmaturity','200'),(325,'updatenotifybuilds','0'),(326,'enablesafebrowserintegration','0'),(327,'enablegroupmembersonly','0'),(328,'dndallowtextandlinks','0'),(329,'enablecssoptimiser','0'),(330,'enabletgzbackups','0'),(331,'debug','0'),(332,'debugdisplay','0'),(333,'debugsmtp','0'),(334,'perfdebug','7'),(335,'debugstringids','0'),(336,'debugvalidators','0'),(337,'debugpageinfo','0'),(338,'profilingenabled','0'),(339,'profilingincluded',''),(340,'profilingexcluded',''),(341,'profilingautofrec','0'),(342,'profilingallowme','0'),(343,'profilingallowall','0'),(344,'profilinglifetime','1440'),(345,'profilingimportprefix','(I)'),(346,'release','2.7.2 (Build: 20140908)'),(347,'branch','27'),(348,'localcachedirpurged','1413861558'),(349,'scheduledtaskreset','1413861559'),(351,'allversionshash','049c6c6fcebce1002c56bf5701540d63eacfb93c'),(353,'notloggedinroleid','6'),(354,'guestroleid','6'),(355,'defaultuserroleid','7'),(356,'creatornewroleid','3'),(357,'restorernewroleid','3'),(358,'gradebookroles','5'),(359,'block_course_list_adminview','all'),(360,'block_course_list_hideallcourseslink','0'),(361,'block_html_allowcssclasses','0'),(362,'block_online_users_timetosee','5'),(363,'block_rss_client_num_entries','5'),(364,'block_rss_client_timeout','30'),(365,'block_tags_showcoursetags','0'),(366,'filter_censor_badwords',''),(367,'filter_multilang_force_old','0'),(368,'chat_method','ajax'),(369,'chat_refresh_userlist','10'),(370,'chat_old_ping','35'),(371,'chat_refresh_room','5'),(372,'chat_normal_updatemode','jsupdate'),(373,'chat_serverhost','lms.sunnet.cc'),(374,'chat_serverip','127.0.0.1'),(375,'chat_serverport','9111'),(376,'chat_servermax','100'),(377,'data_enablerssfeeds','0'),(378,'feedback_allowfullanonymous','0'),(379,'forum_displaymode','3'),(380,'forum_replytouser','1'),(381,'forum_shortpost','300'),(382,'forum_longpost','600'),(383,'forum_manydiscussions','100'),(384,'forum_maxbytes','512000'),(385,'forum_maxattachments','9'),(386,'forum_trackingtype','1'),(387,'forum_trackreadposts','1'),(388,'forum_allowforcedreadtracking','0'),(389,'forum_oldpostdays','14'),(390,'forum_usermarksread','0'),(391,'forum_cleanreadtime','2'),(392,'digestmailtime','17'),(393,'forum_enablerssfeeds','0'),(394,'forum_enabletimedposts','0'),(395,'glossary_entbypage','10'),(396,'glossary_dupentries','0'),(397,'glossary_allowcomments','0'),(398,'glossary_linkbydefault','1'),(399,'glossary_defaultapproval','1'),(400,'glossary_enablerssfeeds','0'),(401,'glossary_linkentries','0'),(402,'glossary_casesensitive','0'),(403,'glossary_fullmatch','0'),(404,'lesson_slideshowwidth','640'),(405,'lesson_slideshowheight','480'),(406,'lesson_slideshowbgcolor','#FFFFFF'),(407,'lesson_mediawidth','640'),(408,'lesson_mediaheight','480'),(409,'lesson_mediaclose','0'),(410,'lesson_maxhighscores','10'),(411,'lesson_maxanswers','4'),(412,'lesson_defaultnextpage','0'),(413,'airnotifierurl','https://messages.moodle.net'),(414,'airnotifierport','443'),(415,'airnotifiermobileappname','com.moodle.moodlemobile'),(416,'airnotifierappname','commoodlemoodlemobile'),(417,'airnotifieraccesskey',''),(418,'smtphosts',''),(419,'smtpsecure',''),(420,'smtpuser',''),(421,'smtppass',''),(422,'smtpmaxbulk','1'),(423,'noreplyaddress','noreply@lms.sunnet.cc'),(424,'emailonlyfromnoreplyaddress','0'),(425,'sitemailcharset','0'),(426,'allowusermailcharset','0'),(427,'allowattachments','1'),(428,'mailnewline','LF'),(429,'jabberhost',''),(430,'jabberserver',''),(431,'jabberusername',''),(432,'jabberpassword',''),(433,'jabberport','5222'),(434,'logguests','1'),(435,'loglifetime','0'),(436,'profileroles','3'),(437,'coursecontact','3'),(438,'frontpage','6'),(439,'frontpageloggedin','6'),(440,'maxcategorydepth','2'),(441,'frontpagecourselimit','200'),(442,'commentsperpage','15'),(443,'defaultfrontpageroleid','8'),(444,'supportname','用户管理'),(445,'supportemail','xiaowuq@sunnet.us'),(446,'registerauth',''),(448,'custom_context_classes','a:6:{i:11;s:34:\"\\local_elisprogram\\context\\program\";i:12;s:32:\"\\local_elisprogram\\context\\track\";i:13;s:33:\"\\local_elisprogram\\context\\course\";i:14;s:34:\"\\local_elisprogram\\context\\pmclass\";i:15;s:31:\"\\local_elisprogram\\context\\user\";i:16;s:34:\"\\local_elisprogram\\context\\userset\";}');

/*Table structure for table `mdl_config_log` */

DROP TABLE IF EXISTS `mdl_config_log`;

CREATE TABLE `mdl_config_log` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL,
  `timemodified` bigint(10) NOT NULL,
  `plugin` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `name` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` longtext COLLATE utf8_unicode_ci,
  `oldvalue` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `mdl_conflog_tim_ix` (`timemodified`),
  KEY `mdl_conflog_use_ix` (`userid`)
) ENGINE=InnoDB AUTO_INCREMENT=994 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Changes done in server configuration through admin UI';

/*Data for the table `mdl_config_log` */

insert  into `mdl_config_log`(`id`,`userid`,`timemodified`,`plugin`,`name`,`value`,`oldvalue`) values (1,0,1413856777,NULL,'enableoutcomes','0',NULL),(2,0,1413856777,NULL,'usecomments','1',NULL),(3,0,1413856777,NULL,'usetags','1',NULL),(4,0,1413856777,NULL,'enablenotes','1',NULL),(5,0,1413856778,NULL,'enableportfolios','0',NULL),(6,0,1413856778,NULL,'enablewebservices','0',NULL),(7,0,1413856778,NULL,'messaging','1',NULL),(8,0,1413856778,NULL,'messaginghidereadnotifications','0',NULL),(9,0,1413856778,NULL,'messagingdeletereadnotificationsdelay','604800',NULL),(10,0,1413856778,NULL,'messagingallowemailoverride','0',NULL),(11,0,1413856778,NULL,'enablestats','0',NULL),(12,0,1413856778,NULL,'enablerssfeeds','0',NULL),(13,0,1413856778,NULL,'enableblogs','1',NULL),(14,0,1413856778,NULL,'enablecompletion','0',NULL),(15,0,1413856779,NULL,'completiondefault','1',NULL),(16,0,1413856779,NULL,'enableavailability','0',NULL),(17,0,1413856779,NULL,'enableplagiarism','0',NULL),(18,0,1413856779,NULL,'enablebadges','1',NULL),(19,0,1413856779,NULL,'autologinguests','0',NULL),(20,0,1413856779,NULL,'hiddenuserfields','',NULL),(21,0,1413856779,NULL,'showuseridentity','email',NULL),(22,0,1413856779,NULL,'fullnamedisplay','language',NULL),(23,0,1413856779,NULL,'maxusersperpage','100',NULL),(24,0,1413856780,NULL,'enablegravatar','0',NULL),(25,0,1413856780,NULL,'gravatardefaulturl','mm',NULL),(26,0,1413856780,'moodlecourse','visible','1',NULL),(27,0,1413856780,'moodlecourse','format','weeks',NULL),(28,0,1413856780,'moodlecourse','maxsections','52',NULL),(29,0,1413856780,'moodlecourse','numsections','10',NULL),(30,0,1413856780,'moodlecourse','hiddensections','0',NULL),(31,0,1413856780,'moodlecourse','coursedisplay','0',NULL),(32,0,1413856780,'moodlecourse','lang','',NULL),(33,0,1413856781,'moodlecourse','newsitems','5',NULL),(34,0,1413856781,'moodlecourse','showgrades','1',NULL),(35,0,1413856781,'moodlecourse','showreports','0',NULL),(36,0,1413856781,'moodlecourse','maxbytes','0',NULL),(37,0,1413856781,'moodlecourse','enablecompletion','0',NULL),(38,0,1413856781,'moodlecourse','groupmode','0',NULL),(39,0,1413856781,'moodlecourse','groupmodeforce','0',NULL),(40,0,1413856781,NULL,'enablecourserequests','0',NULL),(41,0,1413856781,NULL,'defaultrequestcategory','1',NULL),(42,0,1413856782,NULL,'requestcategoryselection','0',NULL),(43,0,1413856782,NULL,'courserequestnotify','',NULL),(44,0,1413856782,'backup','loglifetime','30',NULL),(45,0,1413856782,'backup','backup_general_users','1',NULL),(46,0,1413856782,'backup','backup_general_users_locked','',NULL),(47,0,1413856782,'backup','backup_general_anonymize','0',NULL),(48,0,1413856782,'backup','backup_general_anonymize_locked','',NULL),(49,0,1413856783,'backup','backup_general_role_assignments','1',NULL),(50,0,1413856783,'backup','backup_general_role_assignments_locked','',NULL),(51,0,1413856783,'backup','backup_general_activities','1',NULL),(52,0,1413856783,'backup','backup_general_activities_locked','',NULL),(53,0,1413856783,'backup','backup_general_blocks','1',NULL),(54,0,1413856783,'backup','backup_general_blocks_locked','',NULL),(55,0,1413856783,'backup','backup_general_filters','1',NULL),(56,0,1413856783,'backup','backup_general_filters_locked','',NULL),(57,0,1413856783,'backup','backup_general_comments','1',NULL),(58,0,1413856783,'backup','backup_general_comments_locked','',NULL),(59,0,1413856784,'backup','backup_general_badges','1',NULL),(60,0,1413856784,'backup','backup_general_badges_locked','',NULL),(61,0,1413856784,'backup','backup_general_userscompletion','1',NULL),(62,0,1413856784,'backup','backup_general_userscompletion_locked','',NULL),(63,0,1413856784,'backup','backup_general_logs','0',NULL),(64,0,1413856784,'backup','backup_general_logs_locked','',NULL),(65,0,1413856784,'backup','backup_general_histories','0',NULL),(66,0,1413856784,'backup','backup_general_histories_locked','',NULL),(67,0,1413856785,'backup','backup_general_questionbank','1',NULL),(68,0,1413856785,'backup','backup_general_questionbank_locked','',NULL),(69,0,1413856785,'backup','import_general_maxresults','10',NULL),(70,0,1413856785,'backup','backup_auto_active','0',NULL),(71,0,1413856785,'backup','backup_auto_weekdays','0000000',NULL),(72,0,1413856785,'backup','backup_auto_hour','0',NULL),(73,0,1413856785,'backup','backup_auto_minute','0',NULL),(74,0,1413856785,'backup','backup_auto_storage','0',NULL),(75,0,1413856785,'backup','backup_auto_destination','',NULL),(76,0,1413856786,'backup','backup_auto_keep','1',NULL),(77,0,1413856786,'backup','backup_shortname','0',NULL),(78,0,1413856786,'backup','backup_auto_skip_hidden','1',NULL),(79,0,1413856786,'backup','backup_auto_skip_modif_days','30',NULL),(80,0,1413856786,'backup','backup_auto_skip_modif_prev','0',NULL),(81,0,1413856786,'backup','backup_auto_users','1',NULL),(82,0,1413856786,'backup','backup_auto_role_assignments','1',NULL),(83,0,1413856786,'backup','backup_auto_activities','1',NULL),(84,0,1413856787,'backup','backup_auto_blocks','1',NULL),(85,0,1413856787,'backup','backup_auto_filters','1',NULL),(86,0,1413856787,'backup','backup_auto_comments','1',NULL),(87,0,1413856787,'backup','backup_auto_badges','1',NULL),(88,0,1413856787,'backup','backup_auto_userscompletion','1',NULL),(89,0,1413856787,'backup','backup_auto_logs','0',NULL),(90,0,1413856787,'backup','backup_auto_histories','0',NULL),(91,0,1413856787,'backup','backup_auto_questionbank','1',NULL),(92,0,1413856788,NULL,'grade_profilereport','user',NULL),(93,0,1413856788,NULL,'grade_aggregationposition','1',NULL),(94,0,1413856788,NULL,'grade_includescalesinaggregation','1',NULL),(95,0,1413856788,NULL,'grade_hiddenasdate','0',NULL),(96,0,1413856788,NULL,'gradepublishing','0',NULL),(97,0,1413856788,NULL,'grade_export_displaytype','1',NULL),(98,0,1413856788,NULL,'grade_export_decimalpoints','2',NULL),(99,0,1413856788,NULL,'grade_navmethod','0',NULL),(100,0,1413856788,NULL,'grade_export_userprofilefields','firstname,lastname,idnumber,institution,department,email',NULL),(101,0,1413856789,NULL,'grade_export_customprofilefields','',NULL),(102,0,1413856789,NULL,'recovergradesdefault','0',NULL),(103,0,1413856789,NULL,'gradeexport','',NULL),(104,0,1413856789,NULL,'unlimitedgrades','0',NULL),(105,0,1413856789,NULL,'gradepointmax','100',NULL),(106,0,1413856789,NULL,'gradepointdefault','100',NULL),(107,0,1413856789,NULL,'grade_hideforcedsettings','1',NULL),(108,0,1413856789,NULL,'grade_aggregation','11',NULL),(109,0,1413856790,NULL,'grade_aggregation_flag','0',NULL),(110,0,1413856790,NULL,'grade_aggregations_visible','0,10,11,12,2,4,6,8,13',NULL),(111,0,1413856790,NULL,'grade_aggregateonlygraded','1',NULL),(112,0,1413856790,NULL,'grade_aggregateonlygraded_flag','2',NULL),(113,0,1413856790,NULL,'grade_aggregateoutcomes','0',NULL),(114,0,1413856790,NULL,'grade_aggregateoutcomes_flag','2',NULL),(115,0,1413856790,NULL,'grade_aggregatesubcats','0',NULL),(116,0,1413856790,NULL,'grade_aggregatesubcats_flag','2',NULL),(117,0,1413856790,NULL,'grade_keephigh','0',NULL),(118,0,1413856791,NULL,'grade_keephigh_flag','3',NULL),(119,0,1413856791,NULL,'grade_droplow','0',NULL),(120,0,1413856791,NULL,'grade_droplow_flag','2',NULL),(121,0,1413856791,NULL,'grade_displaytype','1',NULL),(122,0,1413856791,NULL,'grade_decimalpoints','2',NULL),(123,0,1413856791,NULL,'grade_item_advanced','iteminfo,idnumber,gradepass,plusfactor,multfactor,display,decimals,hiddenuntil,locktime',NULL),(124,0,1413856791,NULL,'grade_report_studentsperpage','100',NULL),(125,0,1413856792,NULL,'grade_report_showonlyactiveenrol','1',NULL),(126,0,1413856792,NULL,'grade_report_quickgrading','1',NULL),(127,0,1413856792,NULL,'grade_report_showquickfeedback','0',NULL),(128,0,1413856792,NULL,'grade_report_fixedstudents','0',NULL),(129,0,1413856792,NULL,'grade_report_meanselection','1',NULL),(130,0,1413856792,NULL,'grade_report_enableajax','0',NULL),(131,0,1413856792,NULL,'grade_report_showcalculations','0',NULL),(132,0,1413856793,NULL,'grade_report_showeyecons','0',NULL),(133,0,1413856793,NULL,'grade_report_showaverages','1',NULL),(134,0,1413856793,NULL,'grade_report_showlocks','0',NULL),(135,0,1413856793,NULL,'grade_report_showranges','0',NULL),(136,0,1413856793,NULL,'grade_report_showanalysisicon','1',NULL),(137,0,1413856794,NULL,'grade_report_showuserimage','1',NULL),(138,0,1413856794,NULL,'grade_report_showactivityicons','1',NULL),(139,0,1413856794,NULL,'grade_report_shownumberofgrades','0',NULL),(140,0,1413856794,NULL,'grade_report_averagesdisplaytype','inherit',NULL),(141,0,1413856794,NULL,'grade_report_rangesdisplaytype','inherit',NULL),(142,0,1413856794,NULL,'grade_report_averagesdecimalpoints','inherit',NULL),(143,0,1413856794,NULL,'grade_report_rangesdecimalpoints','inherit',NULL),(144,0,1413856794,NULL,'grade_report_overview_showrank','0',NULL),(145,0,1413856794,NULL,'grade_report_overview_showtotalsifcontainhidden','0',NULL),(146,0,1413856795,NULL,'grade_report_user_showrank','0',NULL),(147,0,1413856795,NULL,'grade_report_user_showpercentage','1',NULL),(148,0,1413856795,NULL,'grade_report_user_showgrade','1',NULL),(149,0,1413856795,NULL,'grade_report_user_showfeedback','1',NULL),(150,0,1413856795,NULL,'grade_report_user_showrange','1',NULL),(151,0,1413856795,NULL,'grade_report_user_showweight','0',NULL),(152,0,1413856795,NULL,'grade_report_user_showaverage','0',NULL),(153,0,1413856796,NULL,'grade_report_user_showlettergrade','0',NULL),(154,0,1413856796,NULL,'grade_report_user_rangedecimals','0',NULL),(155,0,1413856796,NULL,'grade_report_user_showhiddenitems','1',NULL),(156,0,1413856796,NULL,'grade_report_user_showtotalsifcontainhidden','0',NULL),(157,0,1413856796,NULL,'badges_defaultissuername','',NULL),(158,0,1413856796,NULL,'badges_defaultissuercontact','',NULL),(159,0,1413856796,NULL,'badges_badgesalt','badges1413856721',NULL),(160,0,1413856797,NULL,'badges_allowexternalbackpack','1',NULL),(161,0,1413856797,NULL,'badges_allowcoursebadges','1',NULL),(162,0,1413856797,NULL,'timezone','99',NULL),(163,0,1413856797,NULL,'forcetimezone','99',NULL),(164,0,1413856797,NULL,'country','0',NULL),(165,0,1413856797,NULL,'defaultcity','',NULL),(166,0,1413856797,NULL,'geoipfile','E:\\PhpProject\\moodledata/geoip/GeoLiteCity.dat',NULL),(167,0,1413856797,NULL,'googlemapkey3','',NULL),(168,0,1413856797,NULL,'allcountrycodes','',NULL),(169,0,1413856798,NULL,'autolang','1',NULL),(170,0,1413856798,NULL,'lang','zh_cn',NULL),(171,0,1413856798,NULL,'langmenu','1',NULL),(172,0,1413856798,NULL,'langlist','',NULL),(173,0,1413856798,NULL,'langcache','1',NULL),(174,0,1413856798,NULL,'langstringcache','1',NULL),(175,0,1413856798,NULL,'locale','',NULL),(176,0,1413856798,NULL,'latinexcelexport','0',NULL),(177,0,1413856798,NULL,'filteruploadedfiles','0',NULL),(178,0,1413856798,NULL,'filtermatchoneperpage','0',NULL),(179,0,1413856799,NULL,'filtermatchonepertext','0',NULL),(180,0,1413856799,NULL,'repositorycacheexpire','120',NULL),(181,0,1413856799,NULL,'repositorygetfiletimeout','30',NULL),(182,0,1413856799,NULL,'repositorysyncfiletimeout','1',NULL),(183,0,1413856799,NULL,'repositorysyncimagetimeout','3',NULL),(184,0,1413856799,NULL,'repositoryallowexternallinks','1',NULL),(185,0,1413856799,NULL,'legacyfilesinnewcourses','0',NULL),(186,0,1413856799,NULL,'legacyfilesaddallowed','1',NULL),(187,0,1413856800,NULL,'registerauth','',NULL),(188,0,1413856800,NULL,'authloginviaemail','0',NULL),(189,0,1413856800,NULL,'authpreventaccountcreation','0',NULL),(190,0,1413856800,NULL,'loginpageautofocus','0',NULL),(191,0,1413856800,NULL,'guestloginbutton','1',NULL),(192,0,1413856800,NULL,'alternateloginurl','',NULL),(193,0,1413856800,NULL,'forgottenpasswordurl','',NULL),(194,0,1413856800,NULL,'auth_instructions','',NULL),(195,0,1413856800,NULL,'allowemailaddresses','',NULL),(196,0,1413856801,NULL,'denyemailaddresses','',NULL),(197,0,1413856801,NULL,'verifychangedemail','1',NULL),(198,0,1413856801,NULL,'recaptchapublickey','',NULL),(199,0,1413856801,NULL,'recaptchaprivatekey','',NULL),(200,0,1413856801,'question_preview','behaviour','deferredfeedback',NULL),(201,0,1413856801,'question_preview','correctness','1',NULL),(202,0,1413856801,'question_preview','marks','1',NULL),(203,0,1413856801,'question_preview','markdp','2',NULL),(204,0,1413856802,'question_preview','feedback','1',NULL),(205,0,1413856802,'question_preview','generalfeedback','1',NULL),(206,0,1413856802,'question_preview','rightanswer','1',NULL),(207,0,1413856802,'question_preview','history','0',NULL),(208,0,1413856802,NULL,'mobilecssurl','',NULL),(209,0,1413856802,NULL,'enablewsdocumentation','0',NULL),(210,0,1413856803,NULL,'sitedefaultlicense','allrightsreserved',NULL),(211,0,1413856803,NULL,'portfolio_moderate_filesize_threshold','1048576',NULL),(212,0,1413856803,NULL,'portfolio_high_filesize_threshold','5242880',NULL),(213,0,1413856803,NULL,'portfolio_moderate_db_threshold','20',NULL),(214,0,1413856803,NULL,'portfolio_high_db_threshold','50',NULL),(215,0,1413856803,'cachestore_memcache','testservers','',NULL),(216,0,1413856804,'cachestore_memcached','testservers','',NULL),(217,0,1413856804,'cachestore_mongodb','testserver','',NULL),(218,0,1413856804,NULL,'allowbeforeblock','0',NULL),(219,0,1413856804,NULL,'allowedip','',NULL),(220,0,1413856804,NULL,'blockedip','',NULL),(221,0,1413856804,NULL,'protectusernames','1',NULL),(222,0,1413856804,NULL,'forcelogin','0',NULL),(223,0,1413856804,NULL,'forceloginforprofiles','1',NULL),(224,0,1413856805,NULL,'forceloginforprofileimage','0',NULL),(225,0,1413856805,NULL,'opentogoogle','0',NULL),(226,0,1413856805,NULL,'maxbytes','0',NULL),(227,0,1413856805,NULL,'userquota','104857600',NULL),(228,0,1413856805,NULL,'allowobjectembed','0',NULL),(229,0,1413856805,NULL,'enabletrusttext','0',NULL),(230,0,1413856805,NULL,'maxeditingtime','1800',NULL),(231,0,1413856806,NULL,'extendedusernamechars','0',NULL),(232,0,1413856806,NULL,'sitepolicy','',NULL),(233,0,1413856806,NULL,'sitepolicyguest','',NULL),(234,0,1413856806,NULL,'keeptagnamecase','1',NULL),(235,0,1413856806,NULL,'profilesforenrolledusersonly','1',NULL),(236,0,1413856806,NULL,'cronclionly','0',NULL),(237,0,1413856806,NULL,'cronremotepassword','',NULL),(238,0,1413856807,NULL,'lockoutthreshold','0',NULL),(239,0,1413856807,NULL,'lockoutwindow','1800',NULL),(240,0,1413856807,NULL,'lockoutduration','1800',NULL),(241,0,1413856807,NULL,'passwordpolicy','1',NULL),(242,0,1413856807,NULL,'minpasswordlength','8',NULL),(243,0,1413856807,NULL,'minpassworddigits','1',NULL),(244,0,1413856807,NULL,'minpasswordlower','1',NULL),(245,0,1413856807,NULL,'minpasswordupper','1',NULL),(246,0,1413856808,NULL,'minpasswordnonalphanum','1',NULL),(247,0,1413856808,NULL,'maxconsecutiveidentchars','0',NULL),(248,0,1413856808,NULL,'pwresettime','1800',NULL),(249,0,1413856808,NULL,'groupenrolmentkeypolicy','1',NULL),(250,0,1413856808,NULL,'disableuserimages','0',NULL),(251,0,1413856808,NULL,'emailchangeconfirmation','1',NULL),(252,0,1413856808,NULL,'rememberusername','2',NULL),(253,0,1413856808,NULL,'strictformsrequired','0',NULL),(254,0,1413856809,NULL,'loginhttps','0',NULL),(255,0,1413856809,NULL,'cookiesecure','0',NULL),(256,0,1413856809,NULL,'cookiehttponly','0',NULL),(257,0,1413856809,NULL,'allowframembedding','0',NULL),(258,0,1413856809,NULL,'loginpasswordautocomplete','0',NULL),(259,0,1413856809,NULL,'displayloginfailures','0',NULL),(260,0,1413856809,NULL,'notifyloginfailures','',NULL),(261,0,1413856810,NULL,'notifyloginthreshold','10',NULL),(262,0,1413856810,NULL,'runclamonupload','0',NULL),(263,0,1413856810,NULL,'pathtoclam','',NULL),(264,0,1413856810,NULL,'quarantinedir','',NULL),(265,0,1413856810,NULL,'clamfailureonupload','donothing',NULL),(266,0,1413856810,NULL,'themelist','',NULL),(267,0,1413856810,NULL,'themedesignermode','0',NULL),(268,0,1413856810,NULL,'allowuserthemes','0',NULL),(269,0,1413856811,NULL,'allowcoursethemes','0',NULL),(270,0,1413856811,NULL,'allowcategorythemes','0',NULL),(271,0,1413856811,NULL,'allowthemechangeonurl','0',NULL),(272,0,1413856811,NULL,'allowuserblockhiding','1',NULL),(273,0,1413856811,NULL,'allowblockstodock','1',NULL),(274,0,1413856811,NULL,'custommenuitems','',NULL),(275,0,1413856812,NULL,'enabledevicedetection','1',NULL),(276,0,1413856812,NULL,'devicedetectregex','[]',NULL),(277,0,1413856812,'theme_clean','invert','0',NULL),(278,0,1413856812,'theme_clean','logo','',NULL),(279,0,1413856812,'theme_clean','customcss','',NULL),(280,0,1413856812,'theme_clean','footnote','',NULL),(281,0,1413856812,'theme_more','textcolor','#333366',NULL),(282,0,1413856813,'theme_more','linkcolor','#FF6500',NULL),(283,0,1413856813,'theme_more','bodybackground','',NULL),(284,0,1413856813,'theme_more','backgroundimage','',NULL),(285,0,1413856813,'theme_more','backgroundrepeat','repeat',NULL),(286,0,1413856813,'theme_more','backgroundposition','0',NULL),(287,0,1413856813,'theme_more','backgroundfixed','0',NULL),(288,0,1413856813,'theme_more','contentbackground','#FFFFFF',NULL),(289,0,1413856813,'theme_more','secondarybackground','#FFFFFF',NULL),(290,0,1413856814,'theme_more','invert','1',NULL),(291,0,1413856814,'theme_more','logo','',NULL),(292,0,1413856814,'theme_more','customcss','',NULL),(293,0,1413856814,'theme_more','footnote','',NULL),(294,0,1413856814,NULL,'calendartype','gregorian',NULL),(295,0,1413856814,NULL,'calendar_adminseesall','0',NULL),(296,0,1413856815,NULL,'calendar_site_timeformat','0',NULL),(297,0,1413856815,NULL,'calendar_startwday','0',NULL),(298,0,1413856815,NULL,'calendar_weekend','65',NULL),(299,0,1413856815,NULL,'calendar_lookahead','21',NULL),(300,0,1413856815,NULL,'calendar_maxevents','10',NULL),(301,0,1413856815,NULL,'enablecalendarexport','1',NULL),(302,0,1413856815,NULL,'calendar_customexport','1',NULL),(303,0,1413856816,NULL,'calendar_exportlookahead','365',NULL),(304,0,1413856816,NULL,'calendar_exportlookback','5',NULL),(305,0,1413856816,NULL,'calendar_exportsalt','l0KszmRz5mgXM8bMhpSIAwtxDr6m3k51CXuLnKjfQTdYdFH8K2krgtsfmNJQ',NULL),(306,0,1413856816,NULL,'calendar_showicalsource','1',NULL),(307,0,1413856816,NULL,'useblogassociations','1',NULL),(308,0,1413856816,NULL,'bloglevel','4',NULL),(309,0,1413856816,NULL,'useexternalblogs','1',NULL),(310,0,1413856817,NULL,'externalblogcrontime','86400',NULL),(311,0,1413856817,NULL,'maxexternalblogsperuser','1',NULL),(312,0,1413856817,NULL,'blogusecomments','1',NULL),(313,0,1413856817,NULL,'blogshowcommentscount','1',NULL),(314,0,1413856817,NULL,'defaulthomepage','0',NULL),(315,0,1413856817,NULL,'allowguestmymoodle','1',NULL),(316,0,1413856818,NULL,'navshowfullcoursenames','0',NULL),(317,0,1413856818,NULL,'navshowcategories','1',NULL),(318,0,1413856818,NULL,'navshowmycoursecategories','0',NULL),(319,0,1413856818,NULL,'navshowallcourses','0',NULL),(320,0,1413856818,NULL,'navsortmycoursessort','sortorder',NULL),(321,0,1413856818,NULL,'navcourselimit','20',NULL),(322,0,1413856818,NULL,'usesitenameforsitepages','0',NULL),(323,0,1413856818,NULL,'linkadmincategories','0',NULL),(324,0,1413856819,NULL,'navshowfrontpagemods','1',NULL),(325,0,1413856819,NULL,'navadduserpostslinks','1',NULL),(326,0,1413856819,NULL,'formatstringstriptags','1',NULL),(327,0,1413856819,NULL,'emoticons','[{\"text\":\":-)\",\"imagename\":\"s\\/smiley\",\"imagecomponent\":\"core\",\"altidentifier\":\"smiley\",\"altcomponent\":\"core_pix\"},{\"text\":\":)\",\"imagename\":\"s\\/smiley\",\"imagecomponent\":\"core\",\"altidentifier\":\"smiley\",\"altcomponent\":\"core_pix\"},{\"text\":\":-D\",\"imagename\":\"s\\/biggrin\",\"imagecomponent\":\"core\",\"altidentifier\":\"biggrin\",\"altcomponent\":\"core_pix\"},{\"text\":\";-)\",\"imagename\":\"s\\/wink\",\"imagecomponent\":\"core\",\"altidentifier\":\"wink\",\"altcomponent\":\"core_pix\"},{\"text\":\":-\\/\",\"imagename\":\"s\\/mixed\",\"imagecomponent\":\"core\",\"altidentifier\":\"mixed\",\"altcomponent\":\"core_pix\"},{\"text\":\"V-.\",\"imagename\":\"s\\/thoughtful\",\"imagecomponent\":\"core\",\"altidentifier\":\"thoughtful\",\"altcomponent\":\"core_pix\"},{\"text\":\":-P\",\"imagename\":\"s\\/tongueout\",\"imagecomponent\":\"core\",\"altidentifier\":\"tongueout\",\"altcomponent\":\"core_pix\"},{\"text\":\":-p\",\"imagename\":\"s\\/tongueout\",\"imagecomponent\":\"core\",\"altidentifier\":\"tongueout\",\"altcomponent\":\"core_pix\"},{\"text\":\"B-)\",\"imagename\":\"s\\/cool\",\"imagecomponent\":\"core\",\"altidentifier\":\"cool\",\"altcomponent\":\"core_pix\"},{\"text\":\"^-)\",\"imagename\":\"s\\/approve\",\"imagecomponent\":\"core\",\"altidentifier\":\"approve\",\"altcomponent\":\"core_pix\"},{\"text\":\"8-)\",\"imagename\":\"s\\/wideeyes\",\"imagecomponent\":\"core\",\"altidentifier\":\"wideeyes\",\"altcomponent\":\"core_pix\"},{\"text\":\":o)\",\"imagename\":\"s\\/clown\",\"imagecomponent\":\"core\",\"altidentifier\":\"clown\",\"altcomponent\":\"core_pix\"},{\"text\":\":-(\",\"imagename\":\"s\\/sad\",\"imagecomponent\":\"core\",\"altidentifier\":\"sad\",\"altcomponent\":\"core_pix\"},{\"text\":\":(\",\"imagename\":\"s\\/sad\",\"imagecomponent\":\"core\",\"altidentifier\":\"sad\",\"altcomponent\":\"core_pix\"},{\"text\":\"8-.\",\"imagename\":\"s\\/shy\",\"imagecomponent\":\"core\",\"altidentifier\":\"shy\",\"altcomponent\":\"core_pix\"},{\"text\":\":-I\",\"imagename\":\"s\\/blush\",\"imagecomponent\":\"core\",\"altidentifier\":\"blush\",\"altcomponent\":\"core_pix\"},{\"text\":\":-X\",\"imagename\":\"s\\/kiss\",\"imagecomponent\":\"core\",\"altidentifier\":\"kiss\",\"altcomponent\":\"core_pix\"},{\"text\":\"8-o\",\"imagename\":\"s\\/surprise\",\"imagecomponent\":\"core\",\"altidentifier\":\"surprise\",\"altcomponent\":\"core_pix\"},{\"text\":\"P-|\",\"imagename\":\"s\\/blackeye\",\"imagecomponent\":\"core\",\"altidentifier\":\"blackeye\",\"altcomponent\":\"core_pix\"},{\"text\":\"8-[\",\"imagename\":\"s\\/angry\",\"imagecomponent\":\"core\",\"altidentifier\":\"angry\",\"altcomponent\":\"core_pix\"},{\"text\":\"(grr)\",\"imagename\":\"s\\/angry\",\"imagecomponent\":\"core\",\"altidentifier\":\"angry\",\"altcomponent\":\"core_pix\"},{\"text\":\"xx-P\",\"imagename\":\"s\\/dead\",\"imagecomponent\":\"core\",\"altidentifier\":\"dead\",\"altcomponent\":\"core_pix\"},{\"text\":\"|-.\",\"imagename\":\"s\\/sleepy\",\"imagecomponent\":\"core\",\"altidentifier\":\"sleepy\",\"altcomponent\":\"core_pix\"},{\"text\":\"}-]\",\"imagename\":\"s\\/evil\",\"imagecomponent\":\"core\",\"altidentifier\":\"evil\",\"altcomponent\":\"core_pix\"},{\"text\":\"(h)\",\"imagename\":\"s\\/heart\",\"imagecomponent\":\"core\",\"altidentifier\":\"heart\",\"altcomponent\":\"core_pix\"},{\"text\":\"(heart)\",\"imagename\":\"s\\/heart\",\"imagecomponent\":\"core\",\"altidentifier\":\"heart\",\"altcomponent\":\"core_pix\"},{\"text\":\"(y)\",\"imagename\":\"s\\/yes\",\"imagecomponent\":\"core\",\"altidentifier\":\"yes\",\"altcomponent\":\"core\"},{\"text\":\"(n)\",\"imagename\":\"s\\/no\",\"imagecomponent\":\"core\",\"altidentifier\":\"no\",\"altcomponent\":\"core\"},{\"text\":\"(martin)\",\"imagename\":\"s\\/martin\",\"imagecomponent\":\"core\",\"altidentifier\":\"martin\",\"altcomponent\":\"core_pix\"},{\"text\":\"( )\",\"imagename\":\"s\\/egg\",\"imagecomponent\":\"core\",\"altidentifier\":\"egg\",\"altcomponent\":\"core_pix\"}]',NULL),(328,0,1413856819,NULL,'core_media_enable_youtube','1',NULL),(329,0,1413856819,NULL,'core_media_enable_vimeo','0',NULL),(330,0,1413856819,NULL,'core_media_enable_mp3','1',NULL),(331,0,1413856820,NULL,'core_media_enable_flv','1',NULL),(332,0,1413856820,NULL,'core_media_enable_swf','1',NULL),(333,0,1413856820,NULL,'core_media_enable_html5audio','1',NULL),(334,0,1413856820,NULL,'core_media_enable_html5video','1',NULL),(335,0,1413856820,NULL,'core_media_enable_qt','1',NULL),(336,0,1413856820,NULL,'core_media_enable_wmp','1',NULL),(337,0,1413856820,NULL,'core_media_enable_rm','1',NULL),(338,0,1413856821,NULL,'docroot','http://docs.moodle.org',NULL),(339,0,1413856821,NULL,'doclang','',NULL),(340,0,1413856821,NULL,'doctonewwindow','0',NULL),(341,0,1413856821,NULL,'courselistshortnames','0',NULL),(342,0,1413856821,NULL,'coursesperpage','20',NULL),(343,0,1413856821,NULL,'courseswithsummarieslimit','10',NULL),(344,0,1413856821,NULL,'courseoverviewfileslimit','1',NULL),(345,0,1413856822,NULL,'courseoverviewfilesext','.jpg,.gif,.png',NULL),(346,0,1413856822,NULL,'useexternalyui','0',NULL),(347,0,1413856822,NULL,'yuicomboloading','1',NULL),(348,0,1413856822,NULL,'cachejs','1',NULL),(349,0,1413856822,NULL,'modchooserdefault','1',NULL),(350,0,1413856822,NULL,'modeditingmenu','1',NULL),(351,0,1413856822,NULL,'blockeditingmenu','1',NULL),(352,0,1413856823,NULL,'additionalhtmlhead','',NULL),(353,0,1413856823,NULL,'additionalhtmltopofbody','',NULL),(354,0,1413856823,NULL,'additionalhtmlfooter','',NULL),(355,0,1413856823,NULL,'pathtodu','',NULL),(356,0,1413856823,NULL,'aspellpath','',NULL),(357,0,1413856823,NULL,'pathtodot','',NULL),(358,0,1413856823,NULL,'supportpage','',NULL),(359,0,1413856823,NULL,'dbsessions','0',NULL),(360,0,1413856823,NULL,'sessioncookie','',NULL),(361,0,1413856824,NULL,'sessioncookiepath','',NULL),(362,0,1413856824,NULL,'sessioncookiedomain','',NULL),(363,0,1413856824,NULL,'statsfirstrun','none',NULL),(364,0,1413856824,NULL,'statsmaxruntime','0',NULL),(365,0,1413856824,NULL,'statsruntimedays','31',NULL),(366,0,1413856825,NULL,'statsruntimestarthour','0',NULL),(367,0,1413856825,NULL,'statsruntimestartminute','0',NULL),(368,0,1413856825,NULL,'statsuserthreshold','0',NULL),(369,0,1413856825,NULL,'slasharguments','1',NULL),(370,0,1413856825,NULL,'getremoteaddrconf','0',NULL),(371,0,1413856825,NULL,'proxyhost','',NULL),(372,0,1413856825,NULL,'proxyport','0',NULL),(373,0,1413856826,NULL,'proxytype','HTTP',NULL),(374,0,1413856826,NULL,'proxyuser','',NULL),(375,0,1413856826,NULL,'proxypassword','',NULL),(376,0,1413856826,NULL,'proxybypass','localhost, 127.0.0.1',NULL),(377,0,1413856826,NULL,'maintenance_enabled','0',NULL),(378,0,1413856826,NULL,'maintenance_message','',NULL),(379,0,1413856826,NULL,'deleteunconfirmed','168',NULL),(380,0,1413856827,NULL,'deleteincompleteusers','0',NULL),(381,0,1413856827,NULL,'disablegradehistory','0',NULL),(382,0,1413856827,NULL,'gradehistorylifetime','0',NULL),(383,0,1413856827,NULL,'extramemorylimit','512M',NULL),(384,0,1413856827,NULL,'maxtimelimit','0',NULL),(385,0,1413856827,NULL,'curlcache','120',NULL),(386,0,1413856828,NULL,'curltimeoutkbitrate','56',NULL),(387,0,1413856828,NULL,'updateautocheck','1',NULL),(388,0,1413856828,NULL,'updateautodeploy','0',NULL),(389,0,1413856828,NULL,'updateminmaturity','200',NULL),(390,0,1413856828,NULL,'updatenotifybuilds','0',NULL),(391,0,1413856828,NULL,'enablesafebrowserintegration','0',NULL),(392,0,1413856828,NULL,'enablegroupmembersonly','0',NULL),(393,0,1413856829,NULL,'dndallowtextandlinks','0',NULL),(394,0,1413856829,NULL,'enablecssoptimiser','0',NULL),(395,0,1413856829,NULL,'enabletgzbackups','0',NULL),(396,0,1413856829,NULL,'debug','0',NULL),(397,0,1413856829,NULL,'debugdisplay','0',NULL),(398,0,1413856829,NULL,'debugsmtp','0',NULL),(399,0,1413856829,NULL,'perfdebug','7',NULL),(400,0,1413856830,NULL,'debugstringids','0',NULL),(401,0,1413856830,NULL,'debugvalidators','0',NULL),(402,0,1413856830,NULL,'debugpageinfo','0',NULL),(403,0,1413856830,NULL,'profilingenabled','0',NULL),(404,0,1413856830,NULL,'profilingincluded','',NULL),(405,0,1413856830,NULL,'profilingexcluded','',NULL),(406,0,1413856830,NULL,'profilingautofrec','0',NULL),(407,0,1413856830,NULL,'profilingallowme','0',NULL),(408,0,1413856831,NULL,'profilingallowall','0',NULL),(409,0,1413856831,NULL,'profilinglifetime','1440',NULL),(410,0,1413856831,NULL,'profilingimportprefix','(I)',NULL),(411,0,1413856984,'activitynames','filter_active','1',''),(412,0,1413856988,'mathjaxloader','filter_active','1',''),(413,0,1413856989,'mediaplugin','filter_active','1',''),(414,2,1413858057,NULL,'notloggedinroleid','6',NULL),(415,2,1413858057,NULL,'guestroleid','6',NULL),(416,2,1413858057,NULL,'defaultuserroleid','7',NULL),(417,2,1413858057,NULL,'creatornewroleid','3',NULL),(418,2,1413858057,NULL,'restorernewroleid','3',NULL),(419,2,1413858057,NULL,'gradebookroles','5',NULL),(420,2,1413858057,NULL,'block_course_list_adminview','all',NULL),(421,2,1413858057,NULL,'block_course_list_hideallcourseslink','0',NULL),(422,2,1413858057,'block_course_overview','defaultmaxcourses','10',NULL),(423,2,1413858058,'block_course_overview','forcedefaultmaxcourses','0',NULL),(424,2,1413858058,'block_course_overview','showchildren','0',NULL),(425,2,1413858058,'block_course_overview','showwelcomearea','0',NULL),(426,2,1413858058,NULL,'block_html_allowcssclasses','0',NULL),(427,2,1413858058,NULL,'block_online_users_timetosee','5',NULL),(428,2,1413858058,NULL,'block_rss_client_num_entries','5',NULL),(429,2,1413858058,NULL,'block_rss_client_timeout','30',NULL),(430,2,1413858058,'block_section_links','numsections1','22',NULL),(431,2,1413858058,'block_section_links','incby1','2',NULL),(432,2,1413858058,'block_section_links','numsections2','40',NULL),(433,2,1413858058,'block_section_links','incby2','5',NULL),(434,2,1413858058,NULL,'block_tags_showcoursetags','0',NULL),(435,2,1413858058,NULL,'filter_censor_badwords','',NULL),(436,2,1413858059,'filter_emoticon','formats','1,4,0',NULL),(437,2,1413858059,'filter_mathjaxloader','httpurl','http://cdn.mathjax.org/mathjax/2.3-latest/MathJax.js',NULL),(438,2,1413858059,'filter_mathjaxloader','httpsurl','https://cdn.mathjax.org/mathjax/2.3-latest/MathJax.js',NULL),(439,2,1413858059,'filter_mathjaxloader','texfiltercompatibility','0',NULL),(440,2,1413858059,'filter_mathjaxloader','mathjaxconfig','\nMathJax.Hub.Config({\n    config: [\"MMLorHTML.js\", \"Safe.js\"],\n    jax: [\"input/TeX\",\"input/MathML\",\"output/HTML-CSS\",\"output/NativeMML\"],\n    extensions: [\"tex2jax.js\",\"mml2jax.js\",\"MathMenu.js\",\"MathZoom.js\"],\n    TeX: {\n        extensions: [\"AMSmath.js\",\"AMSsymbols.js\",\"noErrors.js\",\"noUndefined.js\"]\n    },\n    menuSettings: {\n        zoom: \"Double-Click\",\n        mpContext: true,\n        mpMouse: true\n    },\n    errorSettings: { message: [\"!\"] },\n    skipStartupTypeset: true,\n    messageStyle: \"none\"\n});\n',NULL),(441,2,1413858059,'filter_mathjaxloader','additionaldelimiters','',NULL),(442,2,1413858059,NULL,'filter_multilang_force_old','0',NULL),(443,2,1413858059,'filter_tex','latexpreamble','\\usepackage[latin1]{inputenc}\n\\usepackage{amsmath}\n\\usepackage{amsfonts}\n\\RequirePackage{amsmath,amssymb,latexsym}\n',NULL),(444,2,1413858059,'filter_tex','latexbackground','#FFFFFF',NULL),(445,2,1413858059,'filter_tex','density','120',NULL),(446,2,1413858059,'filter_tex','pathlatex','c:\\texmf\\miktex\\bin\\latex.exe',NULL),(447,2,1413858059,'filter_tex','pathdvips','c:\\texmf\\miktex\\bin\\dvips.exe',NULL),(448,2,1413858059,'filter_tex','pathconvert','c:\\imagemagick\\convert.exe',NULL),(449,2,1413858060,'filter_tex','pathmimetex','',NULL),(450,2,1413858060,'filter_tex','convertformat','gif',NULL),(451,2,1413858060,'filter_urltolink','formats','0',NULL),(452,2,1413858060,'filter_urltolink','embedimages','1',NULL),(453,2,1413858060,'assign','feedback_plugin_for_gradebook','assignfeedback_comments',NULL),(454,2,1413858060,'assign','showrecentsubmissions','0',NULL),(455,2,1413858060,'assign','submissionreceipts','1',NULL),(456,2,1413858060,'assign','submissionstatement','整个作品都是我自己完成的，除非在作品中我有声明某些地方有引用他人的成果。',NULL),(457,2,1413858060,'assign','alwaysshowdescription','1',NULL),(458,2,1413858060,'assign','alwaysshowdescription_adv','',NULL),(459,2,1413858060,'assign','alwaysshowdescription_locked','',NULL),(460,2,1413858060,'assign','allowsubmissionsfromdate','0',NULL),(461,2,1413858061,'assign','allowsubmissionsfromdate_enabled','1',NULL),(462,2,1413858061,'assign','allowsubmissionsfromdate_adv','',NULL),(463,2,1413858061,'assign','duedate','604800',NULL),(464,2,1413858061,'assign','duedate_enabled','1',NULL),(465,2,1413858061,'assign','duedate_adv','',NULL),(466,2,1413858061,'assign','cutoffdate','1209600',NULL),(467,2,1413858061,'assign','cutoffdate_enabled','',NULL),(468,2,1413858061,'assign','cutoffdate_adv','',NULL),(469,2,1413858061,'assign','submissiondrafts','0',NULL),(470,2,1413858061,'assign','submissiondrafts_adv','',NULL),(471,2,1413858061,'assign','submissiondrafts_locked','',NULL),(472,2,1413858061,'assign','requiresubmissionstatement','0',NULL),(473,2,1413858061,'assign','requiresubmissionstatement_adv','',NULL),(474,2,1413858062,'assign','requiresubmissionstatement_locked','',NULL),(475,2,1413858062,'assign','attemptreopenmethod','none',NULL),(476,2,1413858062,'assign','attemptreopenmethod_adv','',NULL),(477,2,1413858062,'assign','attemptreopenmethod_locked','',NULL),(478,2,1413858062,'assign','maxattempts','-1',NULL),(479,2,1413858062,'assign','maxattempts_adv','',NULL),(480,2,1413858062,'assign','maxattempts_locked','',NULL),(481,2,1413858062,'assign','teamsubmission','0',NULL),(482,2,1413858062,'assign','teamsubmission_adv','',NULL),(483,2,1413858062,'assign','teamsubmission_locked','',NULL),(484,2,1413858062,'assign','requireallteammemberssubmit','0',NULL),(485,2,1413858062,'assign','requireallteammemberssubmit_adv','',NULL),(486,2,1413858062,'assign','requireallteammemberssubmit_locked','',NULL),(487,2,1413858063,'assign','teamsubmissiongroupingid','',NULL),(488,2,1413858063,'assign','teamsubmissiongroupingid_adv','',NULL),(489,2,1413858063,'assign','sendnotifications','0',NULL),(490,2,1413858063,'assign','sendnotifications_adv','',NULL),(491,2,1413858063,'assign','sendnotifications_locked','',NULL),(492,2,1413858063,'assign','sendlatenotifications','0',NULL),(493,2,1413858063,'assign','sendlatenotifications_adv','',NULL),(494,2,1413858063,'assign','sendlatenotifications_locked','',NULL),(495,2,1413858063,'assign','sendstudentnotifications','1',NULL),(496,2,1413858063,'assign','sendstudentnotifications_adv','',NULL),(497,2,1413858063,'assign','sendstudentnotifications_locked','',NULL),(498,2,1413858063,'assign','blindmarking','0',NULL),(499,2,1413858063,'assign','blindmarking_adv','',NULL),(500,2,1413858064,'assign','blindmarking_locked','',NULL),(501,2,1413858064,'assign','markingworkflow','0',NULL),(502,2,1413858064,'assign','markingworkflow_adv','',NULL),(503,2,1413858064,'assign','markingworkflow_locked','',NULL),(504,2,1413858064,'assign','markingallocation','0',NULL),(505,2,1413858064,'assign','markingallocation_adv','',NULL),(506,2,1413858064,'assign','markingallocation_locked','',NULL),(507,2,1413858064,'assignsubmission_file','default','1',NULL),(508,2,1413858064,'assignsubmission_file','maxbytes','1048576',NULL),(509,2,1413858064,'assignsubmission_onlinetext','default','0',NULL),(510,2,1413858064,'assignfeedback_comments','default','1',NULL),(511,2,1413858064,'assignfeedback_comments','inline','0',NULL),(512,2,1413858064,'assignfeedback_comments','inline_adv','',NULL),(513,2,1413858065,'assignfeedback_comments','inline_locked','',NULL),(514,2,1413858065,'assignfeedback_editpdf','stamps','',NULL),(515,2,1413858065,'assignfeedback_editpdf','gspath','/usr/bin/gs',NULL),(516,2,1413858065,'assignfeedback_file','default','0',NULL),(517,2,1413858065,'assignfeedback_offline','default','0',NULL),(518,2,1413858065,'book','requiremodintro','1',NULL),(519,2,1413858065,'book','numberingoptions','0,1,2,3',NULL),(520,2,1413858066,'book','numbering','1',NULL),(521,2,1413858066,NULL,'chat_method','ajax',NULL),(522,2,1413858066,NULL,'chat_refresh_userlist','10',NULL),(523,2,1413858066,NULL,'chat_old_ping','35',NULL),(524,2,1413858066,NULL,'chat_refresh_room','5',NULL),(525,2,1413858066,NULL,'chat_normal_updatemode','jsupdate',NULL),(526,2,1413858066,NULL,'chat_serverhost','lms.sunnet.cc',NULL),(527,2,1413858066,NULL,'chat_serverip','127.0.0.1',NULL),(528,2,1413858066,NULL,'chat_serverport','9111',NULL),(529,2,1413858066,NULL,'chat_servermax','100',NULL),(530,2,1413858066,NULL,'data_enablerssfeeds','0',NULL),(531,2,1413858066,NULL,'feedback_allowfullanonymous','0',NULL),(532,2,1413858067,'folder','requiremodintro','1',NULL),(533,2,1413858067,'folder','showexpanded','1',NULL),(534,2,1413858067,NULL,'forum_displaymode','3',NULL),(535,2,1413858067,NULL,'forum_replytouser','1',NULL),(536,2,1413858067,NULL,'forum_shortpost','300',NULL),(537,2,1413858067,NULL,'forum_longpost','600',NULL),(538,2,1413858067,NULL,'forum_manydiscussions','100',NULL),(539,2,1413858067,NULL,'forum_maxbytes','512000',NULL),(540,2,1413858067,NULL,'forum_maxattachments','9',NULL),(541,2,1413858067,NULL,'forum_trackingtype','1',NULL),(542,2,1413858067,NULL,'forum_trackreadposts','1',NULL),(543,2,1413858068,NULL,'forum_allowforcedreadtracking','0',NULL),(544,2,1413858068,NULL,'forum_oldpostdays','14',NULL),(545,2,1413858068,NULL,'forum_usermarksread','0',NULL),(546,2,1413858068,NULL,'forum_cleanreadtime','2',NULL),(547,2,1413858068,NULL,'digestmailtime','17',NULL),(548,2,1413858068,NULL,'forum_enablerssfeeds','0',NULL),(549,2,1413858068,NULL,'forum_enabletimedposts','0',NULL),(550,2,1413858068,NULL,'glossary_entbypage','10',NULL),(551,2,1413858068,NULL,'glossary_dupentries','0',NULL),(552,2,1413858068,NULL,'glossary_allowcomments','0',NULL),(553,2,1413858069,NULL,'glossary_linkbydefault','1',NULL),(554,2,1413858069,NULL,'glossary_defaultapproval','1',NULL),(555,2,1413858069,NULL,'glossary_enablerssfeeds','0',NULL),(556,2,1413858069,NULL,'glossary_linkentries','0',NULL),(557,2,1413858069,NULL,'glossary_casesensitive','0',NULL),(558,2,1413858069,NULL,'glossary_fullmatch','0',NULL),(559,2,1413858069,'imscp','requiremodintro','1',NULL),(560,2,1413858069,'imscp','keepold','1',NULL),(561,2,1413858069,'imscp','keepold_adv','',NULL),(562,2,1413858069,'label','dndmedia','1',NULL),(563,2,1413858069,'label','dndresizewidth','400',NULL),(564,2,1413858070,'label','dndresizeheight','400',NULL),(565,2,1413858070,NULL,'lesson_slideshowwidth','640',NULL),(566,2,1413858070,NULL,'lesson_slideshowheight','480',NULL),(567,2,1413858070,NULL,'lesson_slideshowbgcolor','#FFFFFF',NULL),(568,2,1413858070,NULL,'lesson_mediawidth','640',NULL),(569,2,1413858070,NULL,'lesson_mediaheight','480',NULL),(570,2,1413858070,NULL,'lesson_mediaclose','0',NULL),(571,2,1413858070,NULL,'lesson_maxhighscores','10',NULL),(572,2,1413858070,NULL,'lesson_maxanswers','4',NULL),(573,2,1413858070,NULL,'lesson_defaultnextpage','0',NULL),(574,2,1413858070,'page','requiremodintro','1',NULL),(575,2,1413858070,'page','displayoptions','5',NULL),(576,2,1413858071,'page','printheading','1',NULL),(577,2,1413858071,'page','printintro','0',NULL),(578,2,1413858071,'page','display','5',NULL),(579,2,1413858071,'page','popupwidth','620',NULL),(580,2,1413858071,'page','popupheight','450',NULL),(581,2,1413858071,'quiz','timelimit','0',NULL),(582,2,1413858071,'quiz','timelimit_adv','',NULL),(583,2,1413858071,'quiz','overduehandling','autoabandon',NULL),(584,2,1413858071,'quiz','overduehandling_adv','',NULL),(585,2,1413858072,'quiz','graceperiod','86400',NULL),(586,2,1413858072,'quiz','graceperiod_adv','',NULL),(587,2,1413858072,'quiz','graceperiodmin','60',NULL),(588,2,1413858072,'quiz','attempts','0',NULL),(589,2,1413858072,'quiz','attempts_adv','',NULL),(590,2,1413858072,'quiz','grademethod','1',NULL),(591,2,1413858072,'quiz','grademethod_adv','',NULL),(592,2,1413858072,'quiz','maximumgrade','10',NULL),(593,2,1413858072,'quiz','shufflequestions','0',NULL),(594,2,1413858072,'quiz','shufflequestions_adv','',NULL),(595,2,1413858072,'quiz','questionsperpage','1',NULL),(596,2,1413858072,'quiz','questionsperpage_adv','',NULL),(597,2,1413858073,'quiz','navmethod','free',NULL),(598,2,1413858073,'quiz','navmethod_adv','1',NULL),(599,2,1413858073,'quiz','shuffleanswers','1',NULL),(600,2,1413858073,'quiz','shuffleanswers_adv','',NULL),(601,2,1413858073,'quiz','preferredbehaviour','deferredfeedback',NULL),(602,2,1413858073,'quiz','attemptonlast','0',NULL),(603,2,1413858073,'quiz','attemptonlast_adv','1',NULL),(604,2,1413858073,'quiz','reviewattempt','69904',NULL),(605,2,1413858073,'quiz','reviewcorrectness','69904',NULL),(606,2,1413858073,'quiz','reviewmarks','69904',NULL),(607,2,1413858074,'quiz','reviewspecificfeedback','69904',NULL),(608,2,1413858074,'quiz','reviewgeneralfeedback','69904',NULL),(609,2,1413858074,'quiz','reviewrightanswer','69904',NULL),(610,2,1413858074,'quiz','reviewoverallfeedback','4368',NULL),(611,2,1413858074,'quiz','showuserpicture','0',NULL),(612,2,1413858074,'quiz','showuserpicture_adv','',NULL),(613,2,1413858074,'quiz','decimalpoints','2',NULL),(614,2,1413858074,'quiz','decimalpoints_adv','',NULL),(615,2,1413858074,'quiz','questiondecimalpoints','-1',NULL),(616,2,1413858074,'quiz','questiondecimalpoints_adv','1',NULL),(617,2,1413858074,'quiz','showblocks','0',NULL),(618,2,1413858075,'quiz','showblocks_adv','1',NULL),(619,2,1413858075,'quiz','password','',NULL),(620,2,1413858075,'quiz','password_adv','1',NULL),(621,2,1413858075,'quiz','subnet','',NULL),(622,2,1413858075,'quiz','subnet_adv','1',NULL),(623,2,1413858075,'quiz','delay1','0',NULL),(624,2,1413858075,'quiz','delay1_adv','1',NULL),(625,2,1413858075,'quiz','delay2','0',NULL),(626,2,1413858075,'quiz','delay2_adv','1',NULL),(627,2,1413858076,'quiz','browsersecurity','-',NULL),(628,2,1413858076,'quiz','browsersecurity_adv','1',NULL),(629,2,1413858076,'quiz','autosaveperiod','0',NULL),(630,2,1413858076,'resource','framesize','130',NULL),(631,2,1413858076,'resource','requiremodintro','1',NULL),(632,2,1413858076,'resource','displayoptions','0,1,4,5,6',NULL),(633,2,1413858076,'resource','printintro','1',NULL),(634,2,1413858076,'resource','display','0',NULL),(635,2,1413858076,'resource','showsize','0',NULL),(636,2,1413858077,'resource','showtype','0',NULL),(637,2,1413858077,'resource','popupwidth','620',NULL),(638,2,1413858077,'resource','popupheight','450',NULL),(639,2,1413858077,'resource','filterfiles','0',NULL),(640,2,1413858077,'scorm','displaycoursestructure','0',NULL),(641,2,1413858077,'scorm','displaycoursestructure_adv','',NULL),(642,2,1413858077,'scorm','popup','0',NULL),(643,2,1413858077,'scorm','popup_adv','',NULL),(644,2,1413858077,'scorm','displayactivityname','1',NULL),(645,2,1413858078,'scorm','framewidth','100',NULL),(646,2,1413858078,'scorm','framewidth_adv','1',NULL),(647,2,1413858078,'scorm','frameheight','500',NULL),(648,2,1413858078,'scorm','frameheight_adv','1',NULL),(649,2,1413858078,'scorm','winoptgrp_adv','1',NULL),(650,2,1413858078,'scorm','scrollbars','0',NULL),(651,2,1413858078,'scorm','directories','0',NULL),(652,2,1413858078,'scorm','location','0',NULL),(653,2,1413858078,'scorm','menubar','0',NULL),(654,2,1413858078,'scorm','toolbar','0',NULL),(655,2,1413858079,'scorm','status','0',NULL),(656,2,1413858079,'scorm','skipview','0',NULL),(657,2,1413858079,'scorm','skipview_adv','1',NULL),(658,2,1413858079,'scorm','hidebrowse','0',NULL),(659,2,1413858079,'scorm','hidebrowse_adv','1',NULL),(660,2,1413858079,'scorm','hidetoc','0',NULL),(661,2,1413858079,'scorm','hidetoc_adv','1',NULL),(662,2,1413858079,'scorm','nav','1',NULL),(663,2,1413858079,'scorm','nav_adv','1',NULL),(664,2,1413858079,'scorm','navpositionleft','-100',NULL),(665,2,1413858079,'scorm','navpositionleft_adv','1',NULL),(666,2,1413858080,'scorm','navpositiontop','-100',NULL),(667,2,1413858080,'scorm','navpositiontop_adv','1',NULL),(668,2,1413858080,'scorm','collapsetocwinsize','767',NULL),(669,2,1413858080,'scorm','collapsetocwinsize_adv','1',NULL),(670,2,1413858080,'scorm','displayattemptstatus','1',NULL),(671,2,1413858080,'scorm','displayattemptstatus_adv','',NULL),(672,2,1413858080,'scorm','grademethod','1',NULL),(673,2,1413858080,'scorm','maxgrade','100',NULL),(674,2,1413858080,'scorm','maxattempt','0',NULL),(675,2,1413858080,'scorm','whatgrade','0',NULL),(676,2,1413858080,'scorm','forcecompleted','0',NULL),(677,2,1413858080,'scorm','forcenewattempt','0',NULL),(678,2,1413858081,'scorm','lastattemptlock','0',NULL),(679,2,1413858081,'scorm','auto','0',NULL),(680,2,1413858081,'scorm','updatefreq','0',NULL),(681,2,1413858081,'scorm','scorm12standard','1',NULL),(682,2,1413858081,'scorm','allowtypeexternal','0',NULL),(683,2,1413858081,'scorm','allowtypelocalsync','0',NULL),(684,2,1413858081,'scorm','allowtypeexternalaicc','0',NULL),(685,2,1413858081,'scorm','allowaicchacp','0',NULL),(686,2,1413858081,'scorm','aicchacptimeout','30',NULL),(687,2,1413858081,'scorm','aicchacpkeepsessiondata','1',NULL),(688,2,1413858082,'scorm','forcejavascript','1',NULL),(689,2,1413858082,'scorm','allowapidebug','0',NULL),(690,2,1413858082,'scorm','apidebugmask','.*',NULL),(691,2,1413858082,'url','framesize','130',NULL),(692,2,1413858082,'url','requiremodintro','1',NULL),(693,2,1413858082,'url','secretphrase','',NULL),(694,2,1413858082,'url','rolesinparams','0',NULL),(695,2,1413858082,'url','displayoptions','0,1,5,6',NULL),(696,2,1413858082,'url','printintro','1',NULL),(697,2,1413858082,'url','display','0',NULL),(698,2,1413858082,'url','popupwidth','620',NULL),(699,2,1413858083,'url','popupheight','450',NULL),(700,2,1413858083,'workshop','grade','80',NULL),(701,2,1413858083,'workshop','gradinggrade','20',NULL),(702,2,1413858083,'workshop','gradedecimals','0',NULL),(703,2,1413858083,'workshop','maxbytes','0',NULL),(704,2,1413858083,'workshop','strategy','accumulative',NULL),(705,2,1413858083,'workshop','examplesmode','0',NULL),(706,2,1413858083,'workshopallocation_random','numofreviews','5',NULL),(707,2,1413858083,'workshopform_numerrors','grade0','否',NULL),(708,2,1413858084,'workshopform_numerrors','grade1','是',NULL),(709,2,1413858084,'workshopeval_best','comparison','5',NULL),(710,2,1413858084,'format_singleactivity','activitytype','forum',NULL),(711,2,1413858084,'editor_atto','toolbar','collapse = collapse\nstyle1 = title, bold, italic\nlist = unorderedlist, orderedlist\nlinks = link\nfiles = image, media, managefiles\nstyle2 = underline, strike, subscript, superscript\nalign = align\nindent = indent\ninsert = equation, charmap, table, clear\nundo = undo\naccessibility = accessibilitychecker, accessibilityhelper\nother = html',NULL),(712,2,1413858084,'atto_collapse','showgroups','5',NULL),(713,2,1413858084,'atto_equation','librarygroup1','\n\\cdot\n\\times\n\\ast\n\\div\n\\diamond\n\\pm\n\\mp\n\\oplus\n\\ominus\n\\otimes\n\\oslash\n\\odot\n\\circ\n\\bullet\n\\asymp\n\\equiv\n\\subseteq\n\\supseteq\n\\leq\n\\geq\n\\preceq\n\\succeq\n\\sim\n\\simeq\n\\approx\n\\subset\n\\supset\n\\ll\n\\gg\n\\prec\n\\succ\n\\infty\n\\in\n\\ni\n\\forall\n\\exists\n\\neq\n',NULL),(714,2,1413858084,'atto_equation','librarygroup2','\n\\leftarrow\n\\rightarrow\n\\uparrow\n\\downarrow\n\\leftrightarrow\n\\nearrow\n\\searrow\n\\swarrow\n\\nwarrow\n\\Leftarrow\n\\Rightarrow\n\\Uparrow\n\\Downarrow\n\\Leftrightarrow\n',NULL),(715,2,1413858084,'atto_equation','librarygroup3','\n\\alpha\n\\beta\n\\gamma\n\\delta\n\\epsilon\n\\zeta\n\\eta\n\\theta\n\\iota\n\\kappa\n\\lambda\n\\mu\n\\nu\n\\xi\n\\pi\n\\rho\n\\sigma\n\\tau\n\\upsilon\n\\phi\n\\chi\n\\psi\n\\omega\n\\Gamma\n\\Delta\n\\Theta\n\\Lambda\n\\Xi\n\\Pi\n\\Sigma\n\\Upsilon\n\\Phi\n\\Psi\n\\Omega\n',NULL),(716,2,1413858085,'atto_equation','librarygroup4','\n\\sum{a,b}\n\\int_{a}^{b}{c}\n\\iint_{a}^{b}{c}\n\\iiint_{a}^{b}{c}\n\\oint{a}\n(a)\n[a]\n\\lbrace{a}\\rbrace\n\\left| \\begin{matrix} a_1 & a_2 \\ a_3 & a_4 \\end{matrix} \\right|\n',NULL),(717,2,1413858085,'editor_tinymce','customtoolbar','wrap,formatselect,wrap,bold,italic,wrap,bullist,numlist,wrap,link,unlink,wrap,image\n\nundo,redo,wrap,underline,strikethrough,sub,sup,wrap,justifyleft,justifycenter,justifyright,wrap,outdent,indent,wrap,forecolor,backcolor,wrap,ltr,rtl\n\nfontselect,fontsizeselect,wrap,code,search,replace,wrap,nonbreaking,charmap,table,wrap,cleanup,removeformat,pastetext,pasteword,wrap,fullscreen',NULL),(718,2,1413858085,'editor_tinymce','fontselectlist','Trebuchet=Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;Arial=arial,helvetica,sans-serif;Courier New=courier new,courier,monospace;Georgia=georgia,times new roman,times,serif;Tahoma=tahoma,arial,helvetica,sans-serif;Times New Roman=times new roman,times,serif;Verdana=verdana,arial,helvetica,sans-serif;Impact=impact;Wingdings=wingdings',NULL),(719,2,1413858085,'editor_tinymce','customconfig','',NULL),(720,2,1413858085,'tinymce_dragmath','requiretex','1',NULL),(721,2,1413858085,'tinymce_moodleemoticon','requireemoticon','1',NULL),(722,2,1413858085,'tinymce_spellchecker','spellengine','',NULL),(723,2,1413858085,'tinymce_spellchecker','spelllanguagelist','+English=en,Danish=da,Dutch=nl,Finnish=fi,French=fr,German=de,Italian=it,Polish=pl,Portuguese=pt,Spanish=es,Swedish=sv',NULL),(724,2,1413858085,NULL,'airnotifierurl','https://messages.moodle.net',NULL),(725,2,1413858085,NULL,'airnotifierport','443',NULL),(726,2,1413858086,NULL,'airnotifiermobileappname','com.moodle.moodlemobile',NULL),(727,2,1413858086,NULL,'airnotifierappname','commoodlemoodlemobile',NULL),(728,2,1413858086,NULL,'airnotifieraccesskey','',NULL),(729,2,1413858086,NULL,'smtphosts','',NULL),(730,2,1413858086,NULL,'smtpsecure','',NULL),(731,2,1413858086,NULL,'smtpuser','',NULL),(732,2,1413858086,NULL,'smtppass','',NULL),(733,2,1413858086,NULL,'smtpmaxbulk','1',NULL),(734,2,1413858087,NULL,'noreplyaddress','noreply@lms.sunnet.cc',NULL),(735,2,1413858087,NULL,'emailonlyfromnoreplyaddress','0',NULL),(736,2,1413858087,NULL,'sitemailcharset','0',NULL),(737,2,1413858087,NULL,'allowusermailcharset','0',NULL),(738,2,1413858087,NULL,'allowattachments','1',NULL),(739,2,1413858087,NULL,'mailnewline','LF',NULL),(740,2,1413858087,NULL,'jabberhost','',NULL),(741,2,1413858087,NULL,'jabberserver','',NULL),(742,2,1413858087,NULL,'jabberusername','',NULL),(743,2,1413858088,NULL,'jabberpassword','',NULL),(744,2,1413858088,NULL,'jabberport','5222',NULL),(745,2,1413858088,'enrol_cohort','roleid','5',NULL),(746,2,1413858088,'enrol_cohort','unenrolaction','0',NULL),(747,2,1413858088,'enrol_database','dbtype','',NULL),(748,2,1413858088,'enrol_database','dbhost','localhost',NULL),(749,2,1413858088,'enrol_database','dbuser','',NULL),(750,2,1413858088,'enrol_database','dbpass','',NULL),(751,2,1413858088,'enrol_database','dbname','',NULL),(752,2,1413858089,'enrol_database','dbencoding','utf-8',NULL),(753,2,1413858089,'enrol_database','dbsetupsql','',NULL),(754,2,1413858089,'enrol_database','dbsybasequoting','0',NULL),(755,2,1413858089,'enrol_database','debugdb','0',NULL),(756,2,1413858089,'enrol_database','localcoursefield','idnumber',NULL),(757,2,1413858089,'enrol_database','localuserfield','idnumber',NULL),(758,2,1413858089,'enrol_database','localrolefield','shortname',NULL),(759,2,1413858089,'enrol_database','localcategoryfield','id',NULL),(760,2,1413858089,'enrol_database','remoteenroltable','',NULL),(761,2,1413858090,'enrol_database','remotecoursefield','',NULL),(762,2,1413858090,'enrol_database','remoteuserfield','',NULL),(763,2,1413858090,'enrol_database','remoterolefield','',NULL),(764,2,1413858090,'enrol_database','defaultrole','5',NULL),(765,2,1413858090,'enrol_database','ignorehiddencourses','0',NULL),(766,2,1413858090,'enrol_database','unenrolaction','0',NULL),(767,2,1413858090,'enrol_database','newcoursetable','',NULL),(768,2,1413858090,'enrol_database','newcoursefullname','fullname',NULL),(769,2,1413858090,'enrol_database','newcourseshortname','shortname',NULL),(770,2,1413858090,'enrol_database','newcourseidnumber','idnumber',NULL),(771,2,1413858091,'enrol_database','newcoursecategory','',NULL),(772,2,1413858091,'enrol_database','defaultcategory','1',NULL),(773,2,1413858091,'enrol_database','templatecourse','',NULL),(774,2,1413858091,'enrol_flatfile','location','',NULL),(775,2,1413858091,'enrol_flatfile','encoding','UTF-8',NULL),(776,2,1413858091,'enrol_flatfile','mailstudents','0',NULL),(777,2,1413858091,'enrol_flatfile','mailteachers','0',NULL),(778,2,1413858091,'enrol_flatfile','mailadmins','0',NULL),(779,2,1413858091,'enrol_flatfile','unenrolaction','3',NULL),(780,2,1413858091,'enrol_flatfile','expiredaction','3',NULL),(781,2,1413858092,'enrol_guest','requirepassword','0',NULL),(782,2,1413858092,'enrol_guest','usepasswordpolicy','0',NULL),(783,2,1413858092,'enrol_guest','showhint','0',NULL),(784,2,1413858092,'enrol_guest','defaultenrol','1',NULL),(785,2,1413858092,'enrol_guest','status','1',NULL),(786,2,1413858092,'enrol_guest','status_adv','',NULL),(787,2,1413858092,'enrol_imsenterprise','imsfilelocation','',NULL),(788,2,1413858092,'enrol_imsenterprise','logtolocation','',NULL),(789,2,1413858092,'enrol_imsenterprise','mailadmins','0',NULL),(790,2,1413858092,'enrol_imsenterprise','createnewusers','0',NULL),(791,2,1413858092,'enrol_imsenterprise','imsdeleteusers','0',NULL),(792,2,1413858092,'enrol_imsenterprise','fixcaseusernames','0',NULL),(793,2,1413858093,'enrol_imsenterprise','fixcasepersonalnames','0',NULL),(794,2,1413858093,'enrol_imsenterprise','imssourcedidfallback','0',NULL),(795,2,1413858093,'enrol_imsenterprise','imsrolemap01','5',NULL),(796,2,1413858093,'enrol_imsenterprise','imsrolemap02','3',NULL),(797,2,1413858093,'enrol_imsenterprise','imsrolemap03','3',NULL),(798,2,1413858093,'enrol_imsenterprise','imsrolemap04','5',NULL),(799,2,1413858093,'enrol_imsenterprise','imsrolemap05','0',NULL),(800,2,1413858093,'enrol_imsenterprise','imsrolemap06','4',NULL),(801,2,1413858093,'enrol_imsenterprise','imsrolemap07','0',NULL),(802,2,1413858093,'enrol_imsenterprise','imsrolemap08','4',NULL),(803,2,1413858094,'enrol_imsenterprise','truncatecoursecodes','0',NULL),(804,2,1413858094,'enrol_imsenterprise','createnewcourses','0',NULL),(805,2,1413858094,'enrol_imsenterprise','createnewcategories','0',NULL),(806,2,1413858094,'enrol_imsenterprise','imsunenrol','0',NULL),(807,2,1413858094,'enrol_imsenterprise','imscoursemapshortname','coursecode',NULL),(808,2,1413858094,'enrol_imsenterprise','imscoursemapfullname','short',NULL),(809,2,1413858094,'enrol_imsenterprise','imscoursemapsummary','ignore',NULL),(810,2,1413858094,'enrol_imsenterprise','imsrestricttarget','',NULL),(811,2,1413858094,'enrol_imsenterprise','imscapitafix','0',NULL),(812,2,1413858094,'enrol_manual','expiredaction','1',NULL),(813,2,1413858094,'enrol_manual','expirynotifyhour','6',NULL),(814,2,1413858094,'enrol_manual','defaultenrol','1',NULL),(815,2,1413858095,'enrol_manual','status','0',NULL),(816,2,1413858095,'enrol_manual','roleid','5',NULL),(817,2,1413858095,'enrol_manual','enrolperiod','0',NULL),(818,2,1413858095,'enrol_manual','expirynotify','0',NULL),(819,2,1413858095,'enrol_manual','expirythreshold','86400',NULL),(820,2,1413858095,'enrol_meta','nosyncroleids','',NULL),(821,2,1413858095,'enrol_meta','syncall','1',NULL),(822,2,1413858095,'enrol_meta','unenrolaction','3',NULL),(823,2,1413858095,'enrol_mnet','roleid','5',NULL),(824,2,1413858095,'enrol_mnet','roleid_adv','1',NULL),(825,2,1413858095,'enrol_paypal','paypalbusiness','',NULL),(826,2,1413858096,'enrol_paypal','mailstudents','0',NULL),(827,2,1413858096,'enrol_paypal','mailteachers','0',NULL),(828,2,1413858096,'enrol_paypal','mailadmins','0',NULL),(829,2,1413858096,'enrol_paypal','expiredaction','3',NULL),(830,2,1413858096,'enrol_paypal','status','1',NULL),(831,2,1413858096,'enrol_paypal','cost','0',NULL),(832,2,1413858096,'enrol_paypal','currency','USD',NULL),(833,2,1413858096,'enrol_paypal','roleid','5',NULL),(834,2,1413858096,'enrol_paypal','enrolperiod','0',NULL),(835,2,1413858096,'enrol_self','requirepassword','0',NULL),(836,2,1413858097,'enrol_self','usepasswordpolicy','0',NULL),(837,2,1413858097,'enrol_self','showhint','0',NULL),(838,2,1413858097,'enrol_self','expiredaction','1',NULL),(839,2,1413858097,'enrol_self','expirynotifyhour','6',NULL),(840,2,1413858097,'enrol_self','defaultenrol','1',NULL),(841,2,1413858097,'enrol_self','status','1',NULL),(842,2,1413858098,'enrol_self','newenrols','1',NULL),(843,2,1413858098,'enrol_self','groupkey','0',NULL),(844,2,1413858098,'enrol_self','roleid','5',NULL),(845,2,1413858098,'enrol_self','enrolperiod','0',NULL),(846,2,1413858098,'enrol_self','expirynotify','0',NULL),(847,2,1413858098,'enrol_self','expirythreshold','86400',NULL),(848,2,1413858098,'enrol_self','longtimenosee','0',NULL),(849,2,1413858098,'enrol_self','maxenrolled','0',NULL),(850,2,1413858098,'enrol_self','sendcoursewelcomemessage','1',NULL),(851,2,1413858099,'logstore_database','dbdriver','',NULL),(852,2,1413858099,'logstore_database','dbhost','',NULL),(853,2,1413858099,'logstore_database','dbuser','',NULL),(854,2,1413858099,'logstore_database','dbpass','',NULL),(855,2,1413858099,'logstore_database','dbname','',NULL),(856,2,1413858099,'logstore_database','dbtable','',NULL),(857,2,1413858099,'logstore_database','dbpersist','0',NULL),(858,2,1413858099,'logstore_database','dbsocket','',NULL),(859,2,1413858100,'logstore_database','dbport','',NULL),(860,2,1413858100,'logstore_database','dbschema','',NULL),(861,2,1413858100,'logstore_database','dbcollation','',NULL),(862,2,1413858100,'logstore_database','buffersize','50',NULL),(863,2,1413858100,'logstore_database','logguests','0',NULL),(864,2,1413858100,'logstore_database','includelevels','1,2,0',NULL),(865,2,1413858100,'logstore_database','includeactions','c,r,u,d',NULL),(866,2,1413858100,'logstore_legacy','loglegacy','0',NULL),(867,2,1413858100,NULL,'logguests','1',NULL),(868,2,1413858101,NULL,'loglifetime','0',NULL),(869,2,1413858101,'logstore_standard','logguests','1',NULL),(870,2,1413858101,'logstore_standard','loglifetime','0',NULL),(871,2,1413858101,'logstore_standard','buffersize','50',NULL),(872,2,1413858101,NULL,'profileroles','5,4,3',NULL),(873,2,1413858101,NULL,'coursecontact','3',NULL),(874,2,1413858101,NULL,'frontpage','6',NULL),(875,2,1413858101,NULL,'frontpageloggedin','6',NULL),(876,2,1413858101,NULL,'maxcategorydepth','2',NULL),(877,2,1413858102,NULL,'frontpagecourselimit','200',NULL),(878,2,1413858102,NULL,'commentsperpage','15',NULL),(879,2,1413858102,NULL,'defaultfrontpageroleid','8',NULL),(880,2,1413858102,NULL,'supportname','用户管理',NULL),(881,2,1413858102,NULL,'supportemail','xiaowuq@sunnet.us',NULL),(882,2,1413858116,NULL,'registerauth','',NULL),(883,2,1413861838,'block_courserequest','course_role','0',NULL),(884,2,1413861838,'block_courserequest','class_role','0',NULL),(885,2,1413861838,'block_courserequest','use_template_by_default','0',NULL),(886,2,1413861839,'block_courserequest','use_course_fields','0',NULL),(887,2,1413861839,'block_courserequest','use_class_fields','1',NULL),(888,2,1413861839,'block_courserequest','create_class_with_course','1',NULL),(889,2,1413861839,'dhimport_version1','identfield_idnumber','1',NULL),(890,2,1413861839,'dhimport_version1','identfield_username','1',NULL),(891,2,1413861839,'dhimport_version1','identfield_email','1',NULL),(892,2,1413861839,'dhimport_version1','creategroupsandgroupings','0',NULL),(893,2,1413861839,'dhimport_version1','createorupdate','0',NULL),(894,2,1413861840,'dhimport_version1','schedule_files_path','/datahub/dhimport_version1',NULL),(895,2,1413861840,'dhimport_version1','user_schedule_file','user.csv',NULL),(896,2,1413861840,'dhimport_version1','course_schedule_file','course.csv',NULL),(897,2,1413861840,'dhimport_version1','enrolment_schedule_file','enroll.csv',NULL),(898,2,1413861840,'dhimport_version1','logfilelocation','/datahub/log',NULL),(899,2,1413861840,'dhimport_version1','emailnotification','',NULL),(900,2,1413861840,'dhimport_version1','allowduplicateemails','0',NULL),(901,2,1413861840,'dhimport_version1','newuseremailenabled','0',NULL),(902,2,1413861841,'dhimport_version1','newuseremailsubject','',NULL),(903,2,1413861841,'dhimport_version1','newuseremailtemplate','',NULL),(904,2,1413861841,'dhimport_version1','newenrolmentemailenabled','0',NULL),(905,2,1413861841,'dhimport_version1','newenrolmentemailfrom','admin',NULL),(906,2,1413861841,'dhimport_version1','newenrolmentemailsubject','',NULL),(907,2,1413861841,'dhimport_version1','newenrolmentemailtemplate','',NULL),(908,2,1413861841,'dhimport_version1elis','identfield_idnumber','1',NULL),(909,2,1413861842,'dhimport_version1elis','identfield_username','1',NULL),(910,2,1413861842,'dhimport_version1elis','identfield_email','1',NULL),(911,2,1413861842,'dhimport_version1elis','createorupdate','0',NULL),(912,2,1413861842,'dhimport_version1elis','schedule_files_path','/datahub/dhimport_version1elis',NULL),(913,2,1413861842,'dhimport_version1elis','user_schedule_file','user.csv',NULL),(914,2,1413861842,'dhimport_version1elis','course_schedule_file','course.csv',NULL),(915,2,1413861842,'dhimport_version1elis','enrolment_schedule_file','enroll.csv',NULL),(916,2,1413861842,'dhimport_version1elis','logfilelocation','/datahub/log',NULL),(917,2,1413861843,'dhimport_version1elis','emailnotification','',NULL),(918,2,1413861843,'dhimport_version1elis','allowduplicateemails','0',NULL),(919,2,1413861843,'dhimport_version1elis','newuseremailenabled','0',NULL),(920,2,1413861843,'dhimport_version1elis','newuseremailsubject','',NULL),(921,2,1413861843,'dhimport_version1elis','newuseremailtemplate','',NULL),(922,2,1413861843,'dhimport_version1elis','newenrolmentemailenabled','0',NULL),(923,2,1413861844,'dhimport_version1elis','newenrolmentemailfrom','admin',NULL),(924,2,1413861844,'dhimport_version1elis','newenrolmentemailsubject','',NULL),(925,2,1413861844,'dhimport_version1elis','newenrolmentemailtemplate','',NULL),(926,2,1413861844,'dhexport_version1','export_path','/datahub/dhexport_version1',NULL),(927,2,1413861844,'dhexport_version1','export_file','export_version1.csv',NULL),(928,2,1413861844,'dhexport_version1','export_file_timestamp','1',NULL),(929,2,1413861844,'dhexport_version1','logfilelocation','/datahub/log',NULL),(930,2,1413861844,'dhexport_version1','emailnotification','',NULL),(931,2,1413861845,'dhexport_version1','nonincremental','0',NULL),(932,2,1413861845,'dhexport_version1','incrementaldelta','1d',NULL),(933,2,1413861845,'dhexport_version1elis','export_path','/datahub/dhexport_version1elis',NULL),(934,2,1413861845,'dhexport_version1elis','export_file','export_version1elis.csv',NULL),(935,2,1413861845,'dhexport_version1elis','export_file_timestamp','1',NULL),(936,2,1413861845,'dhexport_version1elis','logfilelocation','/datahub/log',NULL),(937,2,1413861845,'dhexport_version1elis','emailnotification','',NULL),(938,2,1413861845,'dhexport_version1elis','nonincremental','0',NULL),(939,2,1413861845,'dhexport_version1elis','incrementaldelta','1d',NULL),(940,2,1413861846,'local_datahub','disableincron','0',NULL),(941,2,1413861846,'enrol_elis','defaultenrol','1',NULL),(942,2,1413861846,'enrol_elis','enrol_from_course_catalog','1',NULL),(943,2,1413861846,'enrol_elis','unenrol_from_course_catalog','0',NULL),(944,2,1413861846,'enrol_elis','roleid','5',NULL),(945,2,1413861846,'local_elisprogram','userdefinedtrack','0',NULL),(946,2,1413861846,'local_elisprogram','disablecoursecatalog','0',NULL),(947,2,1413861846,'local_elisprogram','catalog_collapse_count','4',NULL),(948,2,1413861846,'local_elisprogram','enable_curriculum_expiration','0',NULL),(949,2,1413861847,'local_elisprogram','curriculum_expiration_start','1',NULL),(950,2,1413861847,'local_elisprogram','display_completed_courses','1',NULL),(951,2,1413861847,'local_elisprogram','disablecoursecertificates','0',NULL),(952,2,1413861847,'local_elisprogram','disablecertificates','1',NULL),(953,2,1413861847,'local_elisprogram','certificate_border_image','none',NULL),(954,2,1413861847,'local_elisprogram','certificate_seal_image','none',NULL),(955,2,1413861847,'local_elisprogram','certificate_template_file','default.php',NULL),(956,2,1413861847,'local_elisprogram','time_format_12h','0',NULL),(957,2,1413861847,'local_elisprogram','mymoodle_redirect','0',NULL),(958,2,1413861847,'local_elisprogram','auto_assign_user_idnumber','1',NULL),(959,2,1413861847,'local_elisprogram','default_instructor_role','0',NULL),(960,2,1413861848,'local_elisprogram','force_unenrol_in_moodle','0',NULL),(961,2,1413861848,'local_elisprogram','num_block_icons','5',NULL),(962,2,1413861848,'local_elisprogram','display_clusters_at_top_level','1',NULL),(963,2,1413861848,'local_elisprogram','display_curricula_at_top_level','0',NULL),(964,2,1413861849,'local_elisprogram','default_cluster_role_id','0',NULL),(965,2,1413861849,'local_elisprogram','default_curriculum_role_id','0',NULL),(966,2,1413861849,'local_elisprogram','default_course_role_id','0',NULL),(967,2,1413861849,'local_elisprogram','default_class_role_id','0',NULL),(968,2,1413861849,'local_elisprogram','default_track_role_id','0',NULL),(969,2,1413861849,'local_elisprogram','autocreated_unknown_is_yes','1',NULL),(970,2,1413861849,'elisprogram_usetgroups','userset_groups','0',NULL),(971,2,1413861849,'elisprogram_usetgroups','site_course_userset_groups','0',NULL),(972,2,1413861850,'elisprogram_usetgroups','userset_groupings','0',NULL),(973,2,1413861850,'local_elisprogram','legacy_show_inactive_users','0',NULL),(974,2,1413863275,NULL,'loginhttps','1','0'),(975,2,1413863276,NULL,'cookiesecure','1','0'),(976,2,1413863276,NULL,'cookiehttponly','1','0'),(977,2,1413863307,NULL,'loginhttps','0','1'),(978,2,1413864564,NULL,'forcelogin','1','0'),(979,2,1413864564,NULL,'profileroles','3','5,4,3'),(980,2,1413864590,NULL,'forcelogin','0','1'),(981,2,1413864604,NULL,'forcelogin','1','0'),(982,2,1413872889,NULL,'sessioncookie','9772793214ED714F3082A04E18682F58',''),(983,2,1413872889,NULL,'sessioncookiepath','/',''),(984,2,1413872986,NULL,'sessioncookie','9772793214ED714F3082A04E18682F58',''),(985,2,1413873055,NULL,'sessioncookiedomain','engage.sunnet.cc',''),(986,2,1413873110,NULL,'sessioncookiedomain','sunnet.cc','engage.sunnet.cc'),(987,2,1413873119,NULL,'sessioncookiedomain','engage.sunnet.cc','sunnet.cc'),(988,2,1413873319,NULL,'sessioncookiedomain','sunnet.cc','engage.sunnet.cc'),(989,2,1413873402,NULL,'sessioncookie','9772793214ED714F3082A04E18682F58',''),(990,2,1413873402,NULL,'sessioncookiedomain','sunnet.us','sunnet.cc'),(991,2,1413874948,NULL,'sessioncookiepath','/',''),(992,2,1413874948,NULL,'sessioncookiedomain','','sunnet.us'),(993,2,1413875293,NULL,'sessiontimeout','300','7200');

/*Table structure for table `mdl_config_plugins` */

DROP TABLE IF EXISTS `mdl_config_plugins`;

CREATE TABLE `mdl_config_plugins` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `plugin` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'core',
  `name` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` longtext COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_confplug_plunam_uix` (`plugin`,`name`)
) ENGINE=InnoDB AUTO_INCREMENT=1237 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Moodle modules and plugins configuration variables';

/*Data for the table `mdl_config_plugins` */

insert  into `mdl_config_plugins`(`id`,`plugin`,`name`,`value`) values (1,'moodlecourse','visible','1'),(2,'moodlecourse','format','weeks'),(3,'moodlecourse','maxsections','52'),(4,'moodlecourse','numsections','10'),(5,'moodlecourse','hiddensections','0'),(6,'moodlecourse','coursedisplay','0'),(7,'moodlecourse','lang',''),(8,'moodlecourse','newsitems','5'),(9,'moodlecourse','showgrades','1'),(10,'moodlecourse','showreports','0'),(11,'moodlecourse','maxbytes','0'),(12,'moodlecourse','enablecompletion','0'),(13,'moodlecourse','groupmode','0'),(14,'moodlecourse','groupmodeforce','0'),(15,'backup','loglifetime','30'),(16,'backup','backup_general_users','1'),(17,'backup','backup_general_users_locked',''),(18,'backup','backup_general_anonymize','0'),(19,'backup','backup_general_anonymize_locked',''),(20,'backup','backup_general_role_assignments','1'),(21,'backup','backup_general_role_assignments_locked',''),(22,'backup','backup_general_activities','1'),(23,'backup','backup_general_activities_locked',''),(24,'backup','backup_general_blocks','1'),(25,'backup','backup_general_blocks_locked',''),(26,'backup','backup_general_filters','1'),(27,'backup','backup_general_filters_locked',''),(28,'backup','backup_general_comments','1'),(29,'backup','backup_general_comments_locked',''),(30,'backup','backup_general_badges','1'),(31,'backup','backup_general_badges_locked',''),(32,'backup','backup_general_userscompletion','1'),(33,'backup','backup_general_userscompletion_locked',''),(34,'backup','backup_general_logs','0'),(35,'backup','backup_general_logs_locked',''),(36,'backup','backup_general_histories','0'),(37,'backup','backup_general_histories_locked',''),(38,'backup','backup_general_questionbank','1'),(39,'backup','backup_general_questionbank_locked',''),(40,'backup','import_general_maxresults','10'),(41,'backup','backup_auto_active','0'),(42,'backup','backup_auto_weekdays','0000000'),(43,'backup','backup_auto_hour','0'),(44,'backup','backup_auto_minute','0'),(45,'backup','backup_auto_storage','0'),(46,'backup','backup_auto_destination',''),(47,'backup','backup_auto_keep','1'),(48,'backup','backup_shortname','0'),(49,'backup','backup_auto_skip_hidden','1'),(50,'backup','backup_auto_skip_modif_days','30'),(51,'backup','backup_auto_skip_modif_prev','0'),(52,'backup','backup_auto_users','1'),(53,'backup','backup_auto_role_assignments','1'),(54,'backup','backup_auto_activities','1'),(55,'backup','backup_auto_blocks','1'),(56,'backup','backup_auto_filters','1'),(57,'backup','backup_auto_comments','1'),(58,'backup','backup_auto_badges','1'),(59,'backup','backup_auto_userscompletion','1'),(60,'backup','backup_auto_logs','0'),(61,'backup','backup_auto_histories','0'),(62,'backup','backup_auto_questionbank','1'),(63,'question_preview','behaviour','deferredfeedback'),(64,'question_preview','correctness','1'),(65,'question_preview','marks','1'),(66,'question_preview','markdp','2'),(67,'question_preview','feedback','1'),(68,'question_preview','generalfeedback','1'),(69,'question_preview','rightanswer','1'),(70,'question_preview','history','0'),(71,'cachestore_memcache','testservers',''),(72,'cachestore_memcached','testservers',''),(73,'cachestore_mongodb','testserver',''),(74,'theme_clean','invert','0'),(75,'theme_clean','logo',''),(76,'theme_clean','customcss',''),(77,'theme_clean','footnote',''),(78,'theme_more','textcolor','#333366'),(79,'theme_more','linkcolor','#FF6500'),(80,'theme_more','bodybackground',''),(81,'theme_more','backgroundimage',''),(82,'theme_more','backgroundrepeat','repeat'),(83,'theme_more','backgroundposition','0'),(84,'theme_more','backgroundfixed','0'),(85,'theme_more','contentbackground','#FFFFFF'),(86,'theme_more','secondarybackground','#FFFFFF'),(87,'theme_more','invert','1'),(88,'theme_more','logo',''),(89,'theme_more','customcss',''),(90,'theme_more','footnote',''),(91,'availability_completion','version','2014051200'),(92,'availability_date','version','2014051200'),(93,'availability_grade','version','2014051200'),(94,'availability_group','version','2014051200'),(95,'availability_grouping','version','2014051200'),(96,'availability_profile','version','2014051200'),(97,'qtype_calculated','version','2014051200'),(98,'qtype_calculatedmulti','version','2014051200'),(99,'qtype_calculatedsimple','version','2014051200'),(100,'qtype_description','version','2014051200'),(101,'qtype_essay','version','2014051200'),(102,'qtype_match','version','2014051200'),(103,'qtype_missingtype','version','2014051200'),(104,'qtype_multianswer','version','2014051200'),(105,'qtype_multichoice','version','2014051200'),(106,'qtype_numerical','version','2014051200'),(107,'qtype_random','version','2014051201'),(108,'qtype_randomsamatch','version','2014051200'),(109,'qtype_shortanswer','version','2014051200'),(110,'qtype_truefalse','version','2014051200'),(111,'mod_assign','version','2014051201'),(112,'mod_assignment','version','2014051200'),(114,'mod_book','version','2014051200'),(115,'mod_chat','version','2014051200'),(116,'mod_choice','version','2014051200'),(117,'mod_data','version','2014051200'),(118,'mod_feedback','version','2014051200'),(120,'mod_folder','version','2014051200'),(122,'mod_forum','version','2014051202'),(123,'mod_glossary','version','2014051200'),(124,'mod_imscp','version','2014051200'),(126,'mod_label','version','2014051200'),(127,'mod_lesson','version','2014051200'),(128,'mod_lti','version','2014051200'),(129,'mod_page','version','2014051200'),(131,'mod_quiz','version','2014051200'),(132,'mod_resource','version','2014051200'),(133,'mod_scorm','version','2014051200'),(134,'mod_survey','version','2014051200'),(136,'mod_url','version','2014051200'),(138,'mod_wiki','version','2014051200'),(140,'mod_workshop','version','2014051200'),(141,'auth_cas','version','2014051200'),(143,'auth_db','version','2014051200'),(145,'auth_email','version','2014051200'),(146,'auth_fc','version','2014051200'),(148,'auth_imap','version','2014051200'),(150,'auth_ldap','version','2014051200'),(152,'auth_manual','version','2014051200'),(153,'auth_mnet','version','2014051200'),(155,'auth_nntp','version','2014051200'),(157,'auth_nologin','version','2014051200'),(158,'auth_none','version','2014051200'),(159,'auth_pam','version','2014051200'),(161,'auth_pop3','version','2014051200'),(163,'auth_radius','version','2014051200'),(165,'auth_shibboleth','version','2014051200'),(167,'auth_webservice','version','2014051200'),(168,'calendartype_gregorian','version','2014051200'),(169,'enrol_category','version','2014051200'),(171,'enrol_cohort','version','2014051200'),(172,'enrol_database','version','2014051200'),(174,'enrol_flatfile','version','2014051200'),(176,'enrol_flatfile','map_1','manager'),(177,'enrol_flatfile','map_2','coursecreator'),(178,'enrol_flatfile','map_3','editingteacher'),(179,'enrol_flatfile','map_4','teacher'),(180,'enrol_flatfile','map_5','student'),(181,'enrol_flatfile','map_6','guest'),(182,'enrol_flatfile','map_7','user'),(183,'enrol_flatfile','map_8','frontpage'),(184,'enrol_guest','version','2014051200'),(185,'enrol_imsenterprise','version','2014051200'),(187,'enrol_ldap','version','2014051200'),(189,'enrol_manual','version','2014051200'),(191,'enrol_meta','version','2014051200'),(193,'enrol_mnet','version','2014051200'),(194,'enrol_paypal','version','2014051200'),(195,'enrol_self','version','2014051200'),(197,'message_airnotifier','version','2014051200'),(199,'message','airnotifier_provider_enrol_flatfile_flatfile_enrolment_permitted','permitted'),(200,'message','airnotifier_provider_enrol_imsenterprise_imsenterprise_enrolment_permitted','permitted'),(201,'message','airnotifier_provider_enrol_manual_expiry_notification_permitted','permitted'),(202,'message','airnotifier_provider_enrol_paypal_paypal_enrolment_permitted','permitted'),(203,'message','airnotifier_provider_enrol_self_expiry_notification_permitted','permitted'),(204,'message','airnotifier_provider_mod_assign_assign_notification_permitted','permitted'),(205,'message','airnotifier_provider_mod_assignment_assignment_updates_permitted','permitted'),(206,'message','airnotifier_provider_mod_feedback_submission_permitted','permitted'),(207,'message','airnotifier_provider_mod_feedback_message_permitted','permitted'),(208,'message','airnotifier_provider_mod_forum_posts_permitted','permitted'),(209,'message','airnotifier_provider_mod_lesson_graded_essay_permitted','permitted'),(210,'message','airnotifier_provider_mod_quiz_submission_permitted','permitted'),(211,'message','airnotifier_provider_mod_quiz_confirmation_permitted','permitted'),(212,'message','airnotifier_provider_mod_quiz_attempt_overdue_permitted','permitted'),(213,'message','airnotifier_provider_moodle_notices_permitted','permitted'),(214,'message','airnotifier_provider_moodle_errors_permitted','permitted'),(215,'message','airnotifier_provider_moodle_availableupdate_permitted','permitted'),(216,'message','airnotifier_provider_moodle_instantmessage_permitted','permitted'),(217,'message','airnotifier_provider_moodle_backup_permitted','permitted'),(218,'message','airnotifier_provider_moodle_courserequested_permitted','permitted'),(219,'message','airnotifier_provider_moodle_courserequestapproved_permitted','permitted'),(220,'message','airnotifier_provider_moodle_courserequestrejected_permitted','permitted'),(221,'message','airnotifier_provider_moodle_badgerecipientnotice_permitted','permitted'),(222,'message','airnotifier_provider_moodle_badgecreatornotice_permitted','permitted'),(223,'message_email','version','2014051200'),(225,'message','email_provider_enrol_flatfile_flatfile_enrolment_permitted','permitted'),(226,'message','message_provider_enrol_flatfile_flatfile_enrolment_loggedin','email'),(227,'message','message_provider_enrol_flatfile_flatfile_enrolment_loggedoff','email'),(228,'message','email_provider_enrol_imsenterprise_imsenterprise_enrolment_permitted','permitted'),(229,'message','message_provider_enrol_imsenterprise_imsenterprise_enrolment_loggedin','email'),(230,'message','message_provider_enrol_imsenterprise_imsenterprise_enrolment_loggedoff','email'),(231,'message','email_provider_enrol_manual_expiry_notification_permitted','permitted'),(232,'message','message_provider_enrol_manual_expiry_notification_loggedin','email'),(233,'message','message_provider_enrol_manual_expiry_notification_loggedoff','email'),(234,'message','email_provider_enrol_paypal_paypal_enrolment_permitted','permitted'),(235,'message','message_provider_enrol_paypal_paypal_enrolment_loggedin','email'),(236,'message','message_provider_enrol_paypal_paypal_enrolment_loggedoff','email'),(237,'message','email_provider_enrol_self_expiry_notification_permitted','permitted'),(238,'message','message_provider_enrol_self_expiry_notification_loggedin','email'),(239,'message','message_provider_enrol_self_expiry_notification_loggedoff','email'),(240,'message','email_provider_mod_assign_assign_notification_permitted','permitted'),(241,'message','message_provider_mod_assign_assign_notification_loggedin','email'),(242,'message','message_provider_mod_assign_assign_notification_loggedoff','email'),(243,'message','email_provider_mod_assignment_assignment_updates_permitted','permitted'),(244,'message','message_provider_mod_assignment_assignment_updates_loggedin','email'),(245,'message','message_provider_mod_assignment_assignment_updates_loggedoff','email'),(246,'message','email_provider_mod_feedback_submission_permitted','permitted'),(247,'message','message_provider_mod_feedback_submission_loggedin','email'),(248,'message','message_provider_mod_feedback_submission_loggedoff','email'),(249,'message','email_provider_mod_feedback_message_permitted','permitted'),(250,'message','message_provider_mod_feedback_message_loggedin','email'),(251,'message','message_provider_mod_feedback_message_loggedoff','email'),(252,'message','email_provider_mod_forum_posts_permitted','permitted'),(253,'message','message_provider_mod_forum_posts_loggedin','email'),(254,'message','message_provider_mod_forum_posts_loggedoff','email'),(255,'message','email_provider_mod_lesson_graded_essay_permitted','permitted'),(256,'message','message_provider_mod_lesson_graded_essay_loggedin','email'),(257,'message','message_provider_mod_lesson_graded_essay_loggedoff','email'),(258,'message','email_provider_mod_quiz_submission_permitted','permitted'),(259,'message','message_provider_mod_quiz_submission_loggedin','email'),(260,'message','message_provider_mod_quiz_submission_loggedoff','email'),(261,'message','email_provider_mod_quiz_confirmation_permitted','permitted'),(262,'message','message_provider_mod_quiz_confirmation_loggedin','email'),(263,'message','message_provider_mod_quiz_confirmation_loggedoff','email'),(264,'message','email_provider_mod_quiz_attempt_overdue_permitted','permitted'),(265,'message','message_provider_mod_quiz_attempt_overdue_loggedin','email'),(266,'message','message_provider_mod_quiz_attempt_overdue_loggedoff','email'),(267,'message','email_provider_moodle_notices_permitted','permitted'),(268,'message','message_provider_moodle_notices_loggedin','email'),(269,'message','message_provider_moodle_notices_loggedoff','email'),(270,'message','email_provider_moodle_errors_permitted','permitted'),(271,'message','message_provider_moodle_errors_loggedin','email'),(272,'message','message_provider_moodle_errors_loggedoff','email'),(273,'message','email_provider_moodle_availableupdate_permitted','permitted'),(274,'message','message_provider_moodle_availableupdate_loggedin','email'),(275,'message','message_provider_moodle_availableupdate_loggedoff','email'),(276,'message','email_provider_moodle_instantmessage_permitted','permitted'),(277,'message','message_provider_moodle_instantmessage_loggedoff','popup,email'),(278,'message','email_provider_moodle_backup_permitted','permitted'),(279,'message','message_provider_moodle_backup_loggedin','email'),(280,'message','message_provider_moodle_backup_loggedoff','email'),(281,'message','email_provider_moodle_courserequested_permitted','permitted'),(282,'message','message_provider_moodle_courserequested_loggedin','email'),(283,'message','message_provider_moodle_courserequested_loggedoff','email'),(284,'message','email_provider_moodle_courserequestapproved_permitted','permitted'),(285,'message','message_provider_moodle_courserequestapproved_loggedin','email'),(286,'message','message_provider_moodle_courserequestapproved_loggedoff','email'),(287,'message','email_provider_moodle_courserequestrejected_permitted','permitted'),(288,'message','message_provider_moodle_courserequestrejected_loggedin','email'),(289,'message','message_provider_moodle_courserequestrejected_loggedoff','email'),(290,'message','email_provider_moodle_badgerecipientnotice_permitted','permitted'),(291,'message','message_provider_moodle_badgerecipientnotice_loggedoff','popup,email'),(292,'message','email_provider_moodle_badgecreatornotice_permitted','permitted'),(293,'message','message_provider_moodle_badgecreatornotice_loggedoff','email'),(294,'message_jabber','version','2014051200'),(296,'message','jabber_provider_enrol_flatfile_flatfile_enrolment_permitted','permitted'),(297,'message','jabber_provider_enrol_imsenterprise_imsenterprise_enrolment_permitted','permitted'),(298,'message','jabber_provider_enrol_manual_expiry_notification_permitted','permitted'),(299,'message','jabber_provider_enrol_paypal_paypal_enrolment_permitted','permitted'),(300,'message','jabber_provider_enrol_self_expiry_notification_permitted','permitted'),(301,'message','jabber_provider_mod_assign_assign_notification_permitted','permitted'),(302,'message','jabber_provider_mod_assignment_assignment_updates_permitted','permitted'),(303,'message','jabber_provider_mod_feedback_submission_permitted','permitted'),(304,'message','jabber_provider_mod_feedback_message_permitted','permitted'),(305,'message','jabber_provider_mod_forum_posts_permitted','permitted'),(306,'message','jabber_provider_mod_lesson_graded_essay_permitted','permitted'),(307,'message','jabber_provider_mod_quiz_submission_permitted','permitted'),(308,'message','jabber_provider_mod_quiz_confirmation_permitted','permitted'),(309,'message','jabber_provider_mod_quiz_attempt_overdue_permitted','permitted'),(310,'message','jabber_provider_moodle_notices_permitted','permitted'),(311,'message','jabber_provider_moodle_errors_permitted','permitted'),(312,'message','jabber_provider_moodle_availableupdate_permitted','permitted'),(313,'message','jabber_provider_moodle_instantmessage_permitted','permitted'),(314,'message','jabber_provider_moodle_backup_permitted','permitted'),(315,'message','jabber_provider_moodle_courserequested_permitted','permitted'),(316,'message','jabber_provider_moodle_courserequestapproved_permitted','permitted'),(317,'message','jabber_provider_moodle_courserequestrejected_permitted','permitted'),(318,'message','jabber_provider_moodle_badgerecipientnotice_permitted','permitted'),(319,'message','jabber_provider_moodle_badgecreatornotice_permitted','permitted'),(320,'message_popup','version','2014051200'),(322,'message','popup_provider_enrol_flatfile_flatfile_enrolment_permitted','permitted'),(323,'message','popup_provider_enrol_imsenterprise_imsenterprise_enrolment_permitted','permitted'),(324,'message','popup_provider_enrol_manual_expiry_notification_permitted','permitted'),(325,'message','popup_provider_enrol_paypal_paypal_enrolment_permitted','permitted'),(326,'message','popup_provider_enrol_self_expiry_notification_permitted','permitted'),(327,'message','popup_provider_mod_assign_assign_notification_permitted','permitted'),(328,'message','popup_provider_mod_assignment_assignment_updates_permitted','permitted'),(329,'message','popup_provider_mod_feedback_submission_permitted','permitted'),(330,'message','popup_provider_mod_feedback_message_permitted','permitted'),(331,'message','popup_provider_mod_forum_posts_permitted','permitted'),(332,'message','popup_provider_mod_lesson_graded_essay_permitted','permitted'),(333,'message','popup_provider_mod_quiz_submission_permitted','permitted'),(334,'message','popup_provider_mod_quiz_confirmation_permitted','permitted'),(335,'message','popup_provider_mod_quiz_attempt_overdue_permitted','permitted'),(336,'message','popup_provider_moodle_notices_permitted','permitted'),(337,'message','popup_provider_moodle_errors_permitted','permitted'),(338,'message','popup_provider_moodle_availableupdate_permitted','permitted'),(339,'message','popup_provider_moodle_instantmessage_permitted','permitted'),(340,'message','message_provider_moodle_instantmessage_loggedin','popup'),(341,'message','popup_provider_moodle_backup_permitted','permitted'),(342,'message','popup_provider_moodle_courserequested_permitted','permitted'),(343,'message','popup_provider_moodle_courserequestapproved_permitted','permitted'),(344,'message','popup_provider_moodle_courserequestrejected_permitted','permitted'),(345,'message','popup_provider_moodle_badgerecipientnotice_permitted','permitted'),(346,'message','message_provider_moodle_badgerecipientnotice_loggedin','popup'),(347,'message','popup_provider_moodle_badgecreatornotice_permitted','permitted'),(348,'block_activity_modules','version','2014051200'),(349,'block_admin_bookmarks','version','2014051200'),(350,'block_badges','version','2014051200'),(351,'block_blog_menu','version','2014051200'),(352,'block_blog_recent','version','2014051200'),(353,'block_blog_tags','version','2014051200'),(354,'block_calendar_month','version','2014051200'),(355,'block_calendar_upcoming','version','2014051200'),(356,'block_comments','version','2014051200'),(357,'block_community','version','2014051200'),(358,'block_completionstatus','version','2014051200'),(359,'block_course_list','version','2014051200'),(360,'block_course_overview','version','2014051200'),(361,'block_course_summary','version','2014051200'),(362,'block_feedback','version','2014051200'),(364,'block_glossary_random','version','2014051200'),(365,'block_html','version','2014051200'),(366,'block_login','version','2014051200'),(367,'block_mentees','version','2014051200'),(368,'block_messages','version','2014051200'),(369,'block_mnet_hosts','version','2014051200'),(370,'block_myprofile','version','2014051200'),(371,'block_navigation','version','2014051200'),(372,'block_news_items','version','2014051200'),(373,'block_online_users','version','2014051200'),(374,'block_participants','version','2014051200'),(375,'block_private_files','version','2014051200'),(376,'block_quiz_results','version','2014051200'),(377,'block_recent_activity','version','2014051200'),(378,'block_rss_client','version','2014051200'),(379,'block_search_forums','version','2014051200'),(380,'block_section_links','version','2014051200'),(381,'block_selfcompletion','version','2014051200'),(382,'block_settings','version','2014051200'),(383,'block_site_main_menu','version','2014051200'),(384,'block_social_activities','version','2014051200'),(385,'block_tag_flickr','version','2014051200'),(386,'block_tag_youtube','version','2014051200'),(387,'block_tags','version','2014051200'),(388,'filter_activitynames','version','2014051200'),(390,'filter_algebra','version','2014051200'),(391,'filter_censor','version','2014051200'),(392,'filter_data','version','2014051200'),(394,'filter_emailprotect','version','2014051200'),(395,'filter_emoticon','version','2014051200'),(396,'filter_glossary','version','2014051200'),(398,'filter_mathjaxloader','version','2014051201'),(400,'filter_mediaplugin','version','2014051200'),(402,'filter_multilang','version','2014051200'),(403,'filter_tex','version','2014051200'),(405,'filter_tidy','version','2014051200'),(406,'filter_urltolink','version','2014051200'),(407,'editor_atto','version','2014051200'),(409,'editor_textarea','version','2014051200'),(410,'editor_tinymce','version','2014051201'),(411,'format_singleactivity','version','2014051200'),(412,'format_social','version','2014051200'),(413,'format_topics','version','2014051200'),(414,'format_weeks','version','2014051200'),(415,'profilefield_checkbox','version','2014051200'),(416,'profilefield_datetime','version','2014051200'),(417,'profilefield_menu','version','2014051200'),(418,'profilefield_text','version','2014051200'),(419,'profilefield_textarea','version','2014051200'),(420,'report_backups','version','2014051200'),(421,'report_completion','version','2014051200'),(423,'report_configlog','version','2014051200'),(424,'report_courseoverview','version','2014051200'),(425,'report_eventlist','version','2014051200'),(426,'report_log','version','2014051200'),(428,'report_loglive','version','2014051200'),(429,'report_outline','version','2014051200'),(431,'report_participation','version','2014051200'),(433,'report_performance','version','2014051200'),(434,'report_progress','version','2014051200'),(436,'report_questioninstances','version','2014051200'),(437,'report_security','version','2014051200'),(438,'report_stats','version','2014051200'),(440,'gradeexport_ods','version','2014051200'),(441,'gradeexport_txt','version','2014051200'),(442,'gradeexport_xls','version','2014051200'),(443,'gradeexport_xml','version','2014051200'),(444,'gradeimport_csv','version','2014051200'),(445,'gradeimport_xml','version','2014051200'),(446,'gradereport_grader','version','2014051200'),(447,'gradereport_outcomes','version','2014051200'),(448,'gradereport_overview','version','2014051200'),(449,'gradereport_user','version','2014051200'),(450,'gradingform_guide','version','2014051200'),(451,'gradingform_rubric','version','2014051200'),(452,'mnetservice_enrol','version','2014051200'),(453,'webservice_amf','version','2014051200'),(454,'webservice_rest','version','2014051200'),(455,'webservice_soap','version','2014051200'),(456,'webservice_xmlrpc','version','2014051200'),(457,'repository_alfresco','version','2014051200'),(458,'repository_areafiles','version','2014051200'),(460,'areafiles','enablecourseinstances','0'),(461,'areafiles','enableuserinstances','0'),(462,'repository_boxnet','version','2014051200'),(463,'repository_coursefiles','version','2014051200'),(464,'repository_dropbox','version','2014051200'),(465,'repository_equella','version','2014051200'),(466,'repository_filesystem','version','2014051200'),(467,'repository_flickr','version','2014051200'),(468,'repository_flickr_public','version','2014051200'),(469,'repository_googledocs','version','2014051200'),(470,'repository_local','version','2014051200'),(472,'local','enablecourseinstances','0'),(473,'local','enableuserinstances','0'),(474,'repository_merlot','version','2014051200'),(475,'repository_picasa','version','2014051200'),(476,'repository_recent','version','2014051200'),(478,'recent','enablecourseinstances','0'),(479,'recent','enableuserinstances','0'),(480,'repository_s3','version','2014051200'),(481,'repository_skydrive','version','2014051200'),(482,'repository_upload','version','2014051200'),(484,'upload','enablecourseinstances','0'),(485,'upload','enableuserinstances','0'),(486,'repository_url','version','2014051200'),(488,'url','enablecourseinstances','0'),(489,'url','enableuserinstances','0'),(490,'repository_user','version','2014051200'),(492,'user','enablecourseinstances','0'),(493,'user','enableuserinstances','0'),(494,'repository_webdav','version','2014051200'),(495,'repository_wikimedia','version','2014051200'),(497,'wikimedia','enablecourseinstances','0'),(498,'wikimedia','enableuserinstances','0'),(499,'repository_youtube','version','2014051200'),(501,'youtube','enablecourseinstances','0'),(502,'youtube','enableuserinstances','0'),(503,'portfolio_boxnet','version','2014051200'),(504,'portfolio_download','version','2014051200'),(505,'portfolio_flickr','version','2014051200'),(506,'portfolio_googledocs','version','2014051200'),(507,'portfolio_mahara','version','2014051200'),(508,'portfolio_picasa','version','2014051200'),(509,'qbehaviour_adaptive','version','2014051200'),(510,'qbehaviour_adaptivenopenalty','version','2014051200'),(511,'qbehaviour_deferredcbm','version','2014051200'),(512,'qbehaviour_deferredfeedback','version','2014051200'),(513,'qbehaviour_immediatecbm','version','2014051200'),(514,'qbehaviour_immediatefeedback','version','2014051200'),(515,'qbehaviour_informationitem','version','2014051200'),(516,'qbehaviour_interactive','version','2014051200'),(517,'qbehaviour_interactivecountback','version','2014051200'),(518,'qbehaviour_manualgraded','version','2014051200'),(520,'question','disabledbehaviours','manualgraded'),(521,'qbehaviour_missing','version','2014051200'),(522,'qformat_aiken','version','2014051200'),(523,'qformat_blackboard_six','version','2014051200'),(524,'qformat_examview','version','2014051200'),(525,'qformat_gift','version','2014051200'),(526,'qformat_learnwise','version','2014051200'),(527,'qformat_missingword','version','2014051200'),(528,'qformat_multianswer','version','2014051200'),(529,'qformat_webct','version','2014051200'),(530,'qformat_xhtml','version','2014051200'),(531,'qformat_xml','version','2014051200'),(532,'tool_assignmentupgrade','version','2014051200'),(533,'tool_availabilityconditions','version','2014051200'),(534,'tool_behat','version','2014051200'),(535,'tool_capability','version','2014051200'),(536,'tool_customlang','version','2014051200'),(538,'tool_dbtransfer','version','2014051200'),(539,'tool_generator','version','2014051200'),(540,'tool_health','version','2014051200'),(541,'tool_innodb','version','2014051200'),(542,'tool_installaddon','version','2014051200'),(543,'tool_langimport','version','2014051200'),(544,'tool_log','version','2014051200'),(546,'tool_log','enabled_stores','logstore_standard'),(547,'tool_multilangupgrade','version','2014051200'),(548,'tool_phpunit','version','2014051200'),(549,'tool_profiling','version','2014051200'),(550,'tool_replace','version','2014051200'),(551,'tool_spamcleaner','version','2014051200'),(552,'tool_task','version','2014051200'),(553,'tool_timezoneimport','version','2014051200'),(554,'tool_unsuproles','version','2014051200'),(556,'tool_uploadcourse','version','2014051200'),(557,'tool_uploaduser','version','2014051200'),(558,'tool_xmldb','version','2014051200'),(559,'cachestore_file','version','2014051200'),(560,'cachestore_memcache','version','2014051200'),(561,'cachestore_memcached','version','2014051200'),(562,'cachestore_mongodb','version','2014051200'),(563,'cachestore_session','version','2014051200'),(564,'cachestore_static','version','2014051200'),(565,'cachelock_file','version','2014051200'),(566,'theme_base','version','2014051200'),(567,'theme_bootstrapbase','version','2014051200'),(568,'theme_canvas','version','2014051200'),(569,'theme_clean','version','2014051200'),(570,'theme_more','version','2014051200'),(572,'assignsubmission_comments','version','2014051200'),(574,'assignsubmission_file','sortorder','1'),(575,'assignsubmission_comments','sortorder','2'),(576,'assignsubmission_onlinetext','sortorder','0'),(577,'assignsubmission_file','version','2014051200'),(578,'assignsubmission_onlinetext','version','2014051200'),(580,'assignfeedback_comments','version','2014051200'),(582,'assignfeedback_comments','sortorder','0'),(583,'assignfeedback_editpdf','sortorder','1'),(584,'assignfeedback_file','sortorder','3'),(585,'assignfeedback_offline','sortorder','2'),(586,'assignfeedback_editpdf','version','2014051200'),(588,'assignfeedback_file','version','2014051200'),(590,'assignfeedback_offline','version','2014051200'),(591,'assignment_offline','version','2014051200'),(592,'assignment_online','version','2014051200'),(593,'assignment_upload','version','2014051200'),(594,'assignment_uploadsingle','version','2014051200'),(595,'booktool_exportimscp','version','2014051200'),(596,'booktool_importhtml','version','2014051200'),(597,'booktool_print','version','2014051200'),(598,'datafield_checkbox','version','2014051200'),(599,'datafield_date','version','2014051200'),(600,'datafield_file','version','2014051200'),(601,'datafield_latlong','version','2014051200'),(602,'datafield_menu','version','2014051200'),(603,'datafield_multimenu','version','2014051200'),(604,'datafield_number','version','2014051200'),(605,'datafield_picture','version','2014051200'),(606,'datafield_radiobutton','version','2014051200'),(607,'datafield_text','version','2014051200'),(608,'datafield_textarea','version','2014051200'),(609,'datafield_url','version','2014051200'),(610,'datapreset_imagegallery','version','2014051200'),(611,'quiz_grading','version','2014051200'),(613,'quiz_overview','version','2014051200'),(615,'quiz_responses','version','2014051200'),(617,'quiz_statistics','version','2014051200'),(619,'quizaccess_delaybetweenattempts','version','2014051200'),(620,'quizaccess_ipaddress','version','2014051200'),(621,'quizaccess_numattempts','version','2014051200'),(622,'quizaccess_openclosedate','version','2014051200'),(623,'quizaccess_password','version','2014051200'),(624,'quizaccess_safebrowser','version','2014051200'),(625,'quizaccess_securewindow','version','2014051200'),(626,'quizaccess_timelimit','version','2014051200'),(627,'scormreport_basic','version','2014051200'),(628,'scormreport_graphs','version','2014051200'),(629,'scormreport_interactions','version','2014051200'),(630,'scormreport_objectives','version','2014051200'),(631,'workshopform_accumulative','version','2014051200'),(633,'workshopform_comments','version','2014051200'),(634,'workshopform_numerrors','version','2014051200'),(636,'workshopform_rubric','version','2014051200'),(638,'workshopallocation_manual','version','2014051200'),(639,'workshopallocation_random','version','2014051200'),(640,'workshopallocation_scheduled','version','2014051200'),(641,'workshopeval_best','version','2014051200'),(642,'atto_accessibilitychecker','version','2014051200'),(643,'atto_accessibilityhelper','version','2014051200'),(644,'atto_align','version','2014051200'),(645,'atto_backcolor','version','2014051200'),(646,'atto_bold','version','2014051200'),(647,'atto_charmap','version','2014051200'),(648,'atto_clear','version','2014051200'),(649,'atto_collapse','version','2014051200'),(650,'atto_emoticon','version','2014051200'),(651,'atto_equation','version','2014051200'),(652,'atto_fontcolor','version','2014051200'),(653,'atto_html','version','2014051200'),(654,'atto_image','version','2014051200'),(655,'atto_indent','version','2014051200'),(656,'atto_italic','version','2014051200'),(657,'atto_link','version','2014051200'),(658,'atto_managefiles','version','2014051200'),(659,'atto_media','version','2014051200'),(660,'atto_noautolink','version','2014051200'),(661,'atto_orderedlist','version','2014051200'),(662,'atto_rtl','version','2014051200'),(663,'atto_strike','version','2014051200'),(664,'atto_subscript','version','2014051200'),(665,'atto_superscript','version','2014051200'),(666,'atto_table','version','2014051200'),(667,'atto_title','version','2014051200'),(668,'atto_underline','version','2014051200'),(669,'atto_undo','version','2014051200'),(670,'atto_unorderedlist','version','2014051200'),(671,'tinymce_ctrlhelp','version','2014051200'),(672,'tinymce_dragmath','version','2014051200'),(673,'tinymce_managefiles','version','2014051200'),(674,'tinymce_moodleemoticon','version','2014051200'),(675,'tinymce_moodleimage','version','2014051200'),(676,'tinymce_moodlemedia','version','2014051200'),(677,'tinymce_moodlenolink','version','2014051200'),(678,'tinymce_pdw','version','2014051200'),(679,'tinymce_spellchecker','version','2014051200'),(681,'tinymce_wrap','version','2014051200'),(682,'logstore_database','version','2014051200'),(683,'logstore_legacy','version','2014051200'),(684,'logstore_standard','version','2014051200'),(685,'block_course_overview','defaultmaxcourses','10'),(686,'block_course_overview','forcedefaultmaxcourses','0'),(687,'block_course_overview','showchildren','0'),(688,'block_course_overview','showwelcomearea','0'),(689,'block_section_links','numsections1','22'),(690,'block_section_links','incby1','2'),(691,'block_section_links','numsections2','40'),(692,'block_section_links','incby2','5'),(693,'filter_emoticon','formats','1,4,0'),(694,'filter_mathjaxloader','httpurl','http://cdn.mathjax.org/mathjax/2.3-latest/MathJax.js'),(695,'filter_mathjaxloader','httpsurl','https://cdn.mathjax.org/mathjax/2.3-latest/MathJax.js'),(696,'filter_mathjaxloader','texfiltercompatibility','0'),(697,'filter_mathjaxloader','mathjaxconfig','\nMathJax.Hub.Config({\n    config: [\"MMLorHTML.js\", \"Safe.js\"],\n    jax: [\"input/TeX\",\"input/MathML\",\"output/HTML-CSS\",\"output/NativeMML\"],\n    extensions: [\"tex2jax.js\",\"mml2jax.js\",\"MathMenu.js\",\"MathZoom.js\"],\n    TeX: {\n        extensions: [\"AMSmath.js\",\"AMSsymbols.js\",\"noErrors.js\",\"noUndefined.js\"]\n    },\n    menuSettings: {\n        zoom: \"Double-Click\",\n        mpContext: true,\n        mpMouse: true\n    },\n    errorSettings: { message: [\"!\"] },\n    skipStartupTypeset: true,\n    messageStyle: \"none\"\n});\n'),(698,'filter_mathjaxloader','additionaldelimiters',''),(699,'filter_tex','latexpreamble','\\usepackage[latin1]{inputenc}\n\\usepackage{amsmath}\n\\usepackage{amsfonts}\n\\RequirePackage{amsmath,amssymb,latexsym}\n'),(700,'filter_tex','latexbackground','#FFFFFF'),(701,'filter_tex','density','120'),(702,'filter_tex','pathlatex','c:\\texmf\\miktex\\bin\\latex.exe'),(703,'filter_tex','pathdvips','c:\\texmf\\miktex\\bin\\dvips.exe'),(704,'filter_tex','pathconvert','c:\\imagemagick\\convert.exe'),(705,'filter_tex','pathmimetex',''),(706,'filter_tex','convertformat','gif'),(707,'filter_urltolink','formats','0'),(708,'filter_urltolink','embedimages','1'),(709,'assign','feedback_plugin_for_gradebook','assignfeedback_comments'),(710,'assign','showrecentsubmissions','0'),(711,'assign','submissionreceipts','1'),(712,'assign','submissionstatement','整个作品都是我自己完成的，除非在作品中我有声明某些地方有引用他人的成果。'),(713,'assign','alwaysshowdescription','1'),(714,'assign','alwaysshowdescription_adv',''),(715,'assign','alwaysshowdescription_locked',''),(716,'assign','allowsubmissionsfromdate','0'),(717,'assign','allowsubmissionsfromdate_enabled','1'),(718,'assign','allowsubmissionsfromdate_adv',''),(719,'assign','duedate','604800'),(720,'assign','duedate_enabled','1'),(721,'assign','duedate_adv',''),(722,'assign','cutoffdate','1209600'),(723,'assign','cutoffdate_enabled',''),(724,'assign','cutoffdate_adv',''),(725,'assign','submissiondrafts','0'),(726,'assign','submissiondrafts_adv',''),(727,'assign','submissiondrafts_locked',''),(728,'assign','requiresubmissionstatement','0'),(729,'assign','requiresubmissionstatement_adv',''),(730,'assign','requiresubmissionstatement_locked',''),(731,'assign','attemptreopenmethod','none'),(732,'assign','attemptreopenmethod_adv',''),(733,'assign','attemptreopenmethod_locked',''),(734,'assign','maxattempts','-1'),(735,'assign','maxattempts_adv',''),(736,'assign','maxattempts_locked',''),(737,'assign','teamsubmission','0'),(738,'assign','teamsubmission_adv',''),(739,'assign','teamsubmission_locked',''),(740,'assign','requireallteammemberssubmit','0'),(741,'assign','requireallteammemberssubmit_adv',''),(742,'assign','requireallteammemberssubmit_locked',''),(743,'assign','teamsubmissiongroupingid',''),(744,'assign','teamsubmissiongroupingid_adv',''),(745,'assign','sendnotifications','0'),(746,'assign','sendnotifications_adv',''),(747,'assign','sendnotifications_locked',''),(748,'assign','sendlatenotifications','0'),(749,'assign','sendlatenotifications_adv',''),(750,'assign','sendlatenotifications_locked',''),(751,'assign','sendstudentnotifications','1'),(752,'assign','sendstudentnotifications_adv',''),(753,'assign','sendstudentnotifications_locked',''),(754,'assign','blindmarking','0'),(755,'assign','blindmarking_adv',''),(756,'assign','blindmarking_locked',''),(757,'assign','markingworkflow','0'),(758,'assign','markingworkflow_adv',''),(759,'assign','markingworkflow_locked',''),(760,'assign','markingallocation','0'),(761,'assign','markingallocation_adv',''),(762,'assign','markingallocation_locked',''),(763,'assignsubmission_file','default','1'),(764,'assignsubmission_file','maxbytes','1048576'),(765,'assignsubmission_onlinetext','default','0'),(766,'assignfeedback_comments','default','1'),(767,'assignfeedback_comments','inline','0'),(768,'assignfeedback_comments','inline_adv',''),(769,'assignfeedback_comments','inline_locked',''),(770,'assignfeedback_editpdf','stamps',''),(771,'assignfeedback_editpdf','gspath','/usr/bin/gs'),(772,'assignfeedback_file','default','0'),(773,'assignfeedback_offline','default','0'),(774,'book','requiremodintro','1'),(775,'book','numberingoptions','0,1,2,3'),(776,'book','numbering','1'),(777,'folder','requiremodintro','1'),(778,'folder','showexpanded','1'),(779,'imscp','requiremodintro','1'),(780,'imscp','keepold','1'),(781,'imscp','keepold_adv',''),(782,'label','dndmedia','1'),(783,'label','dndresizewidth','400'),(784,'label','dndresizeheight','400'),(785,'page','requiremodintro','1'),(786,'page','displayoptions','5'),(787,'page','printheading','1'),(788,'page','printintro','0'),(789,'page','display','5'),(790,'page','popupwidth','620'),(791,'page','popupheight','450'),(792,'quiz','timelimit','0'),(793,'quiz','timelimit_adv',''),(794,'quiz','overduehandling','autoabandon'),(795,'quiz','overduehandling_adv',''),(796,'quiz','graceperiod','86400'),(797,'quiz','graceperiod_adv',''),(798,'quiz','graceperiodmin','60'),(799,'quiz','attempts','0'),(800,'quiz','attempts_adv',''),(801,'quiz','grademethod','1'),(802,'quiz','grademethod_adv',''),(803,'quiz','maximumgrade','10'),(804,'quiz','shufflequestions','0'),(805,'quiz','shufflequestions_adv',''),(806,'quiz','questionsperpage','1'),(807,'quiz','questionsperpage_adv',''),(808,'quiz','navmethod','free'),(809,'quiz','navmethod_adv','1'),(810,'quiz','shuffleanswers','1'),(811,'quiz','shuffleanswers_adv',''),(812,'quiz','preferredbehaviour','deferredfeedback'),(813,'quiz','attemptonlast','0'),(814,'quiz','attemptonlast_adv','1'),(815,'quiz','reviewattempt','69904'),(816,'quiz','reviewcorrectness','69904'),(817,'quiz','reviewmarks','69904'),(818,'quiz','reviewspecificfeedback','69904'),(819,'quiz','reviewgeneralfeedback','69904'),(820,'quiz','reviewrightanswer','69904'),(821,'quiz','reviewoverallfeedback','4368'),(822,'quiz','showuserpicture','0'),(823,'quiz','showuserpicture_adv',''),(824,'quiz','decimalpoints','2'),(825,'quiz','decimalpoints_adv',''),(826,'quiz','questiondecimalpoints','-1'),(827,'quiz','questiondecimalpoints_adv','1'),(828,'quiz','showblocks','0'),(829,'quiz','showblocks_adv','1'),(830,'quiz','password',''),(831,'quiz','password_adv','1'),(832,'quiz','subnet',''),(833,'quiz','subnet_adv','1'),(834,'quiz','delay1','0'),(835,'quiz','delay1_adv','1'),(836,'quiz','delay2','0'),(837,'quiz','delay2_adv','1'),(838,'quiz','browsersecurity','-'),(839,'quiz','browsersecurity_adv','1'),(840,'quiz','autosaveperiod','0'),(841,'resource','framesize','130'),(842,'resource','requiremodintro','1'),(843,'resource','displayoptions','0,1,4,5,6'),(844,'resource','printintro','1'),(845,'resource','display','0'),(846,'resource','showsize','0'),(847,'resource','showtype','0'),(848,'resource','popupwidth','620'),(849,'resource','popupheight','450'),(850,'resource','filterfiles','0'),(851,'scorm','displaycoursestructure','0'),(852,'scorm','displaycoursestructure_adv',''),(853,'scorm','popup','0'),(854,'scorm','popup_adv',''),(855,'scorm','displayactivityname','1'),(856,'scorm','framewidth','100'),(857,'scorm','framewidth_adv','1'),(858,'scorm','frameheight','500'),(859,'scorm','frameheight_adv','1'),(860,'scorm','winoptgrp_adv','1'),(861,'scorm','scrollbars','0'),(862,'scorm','directories','0'),(863,'scorm','location','0'),(864,'scorm','menubar','0'),(865,'scorm','toolbar','0'),(866,'scorm','status','0'),(867,'scorm','skipview','0'),(868,'scorm','skipview_adv','1'),(869,'scorm','hidebrowse','0'),(870,'scorm','hidebrowse_adv','1'),(871,'scorm','hidetoc','0'),(872,'scorm','hidetoc_adv','1'),(873,'scorm','nav','1'),(874,'scorm','nav_adv','1'),(875,'scorm','navpositionleft','-100'),(876,'scorm','navpositionleft_adv','1'),(877,'scorm','navpositiontop','-100'),(878,'scorm','navpositiontop_adv','1'),(879,'scorm','collapsetocwinsize','767'),(880,'scorm','collapsetocwinsize_adv','1'),(881,'scorm','displayattemptstatus','1'),(882,'scorm','displayattemptstatus_adv',''),(883,'scorm','grademethod','1'),(884,'scorm','maxgrade','100'),(885,'scorm','maxattempt','0'),(886,'scorm','whatgrade','0'),(887,'scorm','forcecompleted','0'),(888,'scorm','forcenewattempt','0'),(889,'scorm','lastattemptlock','0'),(890,'scorm','auto','0'),(891,'scorm','updatefreq','0'),(892,'scorm','scorm12standard','1'),(893,'scorm','allowtypeexternal','0'),(894,'scorm','allowtypelocalsync','0'),(895,'scorm','allowtypeexternalaicc','0'),(896,'scorm','allowaicchacp','0'),(897,'scorm','aicchacptimeout','30'),(898,'scorm','aicchacpkeepsessiondata','1'),(899,'scorm','forcejavascript','1'),(900,'scorm','allowapidebug','0'),(901,'scorm','apidebugmask','.*'),(902,'url','framesize','130'),(903,'url','requiremodintro','1'),(904,'url','secretphrase',''),(905,'url','rolesinparams','0'),(906,'url','displayoptions','0,1,5,6'),(907,'url','printintro','1'),(908,'url','display','0'),(909,'url','popupwidth','620'),(910,'url','popupheight','450'),(911,'workshop','grade','80'),(912,'workshop','gradinggrade','20'),(913,'workshop','gradedecimals','0'),(914,'workshop','maxbytes','0'),(915,'workshop','strategy','accumulative'),(916,'workshop','examplesmode','0'),(917,'workshopallocation_random','numofreviews','5'),(918,'workshopform_numerrors','grade0','否'),(919,'workshopform_numerrors','grade1','是'),(920,'workshopeval_best','comparison','5'),(921,'format_singleactivity','activitytype','forum'),(922,'editor_atto','toolbar','collapse = collapse\nstyle1 = title, bold, italic\nlist = unorderedlist, orderedlist\nlinks = link\nfiles = image, media, managefiles\nstyle2 = underline, strike, subscript, superscript\nalign = align\nindent = indent\ninsert = equation, charmap, table, clear\nundo = undo\naccessibility = accessibilitychecker, accessibilityhelper\nother = html'),(923,'atto_collapse','showgroups','5'),(924,'atto_equation','librarygroup1','\n\\cdot\n\\times\n\\ast\n\\div\n\\diamond\n\\pm\n\\mp\n\\oplus\n\\ominus\n\\otimes\n\\oslash\n\\odot\n\\circ\n\\bullet\n\\asymp\n\\equiv\n\\subseteq\n\\supseteq\n\\leq\n\\geq\n\\preceq\n\\succeq\n\\sim\n\\simeq\n\\approx\n\\subset\n\\supset\n\\ll\n\\gg\n\\prec\n\\succ\n\\infty\n\\in\n\\ni\n\\forall\n\\exists\n\\neq\n'),(925,'atto_equation','librarygroup2','\n\\leftarrow\n\\rightarrow\n\\uparrow\n\\downarrow\n\\leftrightarrow\n\\nearrow\n\\searrow\n\\swarrow\n\\nwarrow\n\\Leftarrow\n\\Rightarrow\n\\Uparrow\n\\Downarrow\n\\Leftrightarrow\n'),(926,'atto_equation','librarygroup3','\n\\alpha\n\\beta\n\\gamma\n\\delta\n\\epsilon\n\\zeta\n\\eta\n\\theta\n\\iota\n\\kappa\n\\lambda\n\\mu\n\\nu\n\\xi\n\\pi\n\\rho\n\\sigma\n\\tau\n\\upsilon\n\\phi\n\\chi\n\\psi\n\\omega\n\\Gamma\n\\Delta\n\\Theta\n\\Lambda\n\\Xi\n\\Pi\n\\Sigma\n\\Upsilon\n\\Phi\n\\Psi\n\\Omega\n'),(927,'atto_equation','librarygroup4','\n\\sum{a,b}\n\\int_{a}^{b}{c}\n\\iint_{a}^{b}{c}\n\\iiint_{a}^{b}{c}\n\\oint{a}\n(a)\n[a]\n\\lbrace{a}\\rbrace\n\\left| \\begin{matrix} a_1 & a_2 \\ a_3 & a_4 \\end{matrix} \\right|\n'),(928,'editor_tinymce','customtoolbar','wrap,formatselect,wrap,bold,italic,wrap,bullist,numlist,wrap,link,unlink,wrap,image\n\nundo,redo,wrap,underline,strikethrough,sub,sup,wrap,justifyleft,justifycenter,justifyright,wrap,outdent,indent,wrap,forecolor,backcolor,wrap,ltr,rtl\n\nfontselect,fontsizeselect,wrap,code,search,replace,wrap,nonbreaking,charmap,table,wrap,cleanup,removeformat,pastetext,pasteword,wrap,fullscreen'),(929,'editor_tinymce','fontselectlist','Trebuchet=Trebuchet MS,Verdana,Arial,Helvetica,sans-serif;Arial=arial,helvetica,sans-serif;Courier New=courier new,courier,monospace;Georgia=georgia,times new roman,times,serif;Tahoma=tahoma,arial,helvetica,sans-serif;Times New Roman=times new roman,times,serif;Verdana=verdana,arial,helvetica,sans-serif;Impact=impact;Wingdings=wingdings'),(930,'editor_tinymce','customconfig',''),(931,'tinymce_dragmath','requiretex','1'),(932,'tinymce_moodleemoticon','requireemoticon','1'),(933,'tinymce_spellchecker','spellengine',''),(934,'tinymce_spellchecker','spelllanguagelist','+English=en,Danish=da,Dutch=nl,Finnish=fi,French=fr,German=de,Italian=it,Polish=pl,Portuguese=pt,Spanish=es,Swedish=sv'),(935,'enrol_cohort','roleid','5'),(936,'enrol_cohort','unenrolaction','0'),(937,'enrol_database','dbtype',''),(938,'enrol_database','dbhost','localhost'),(939,'enrol_database','dbuser',''),(940,'enrol_database','dbpass',''),(941,'enrol_database','dbname',''),(942,'enrol_database','dbencoding','utf-8'),(943,'enrol_database','dbsetupsql',''),(944,'enrol_database','dbsybasequoting','0'),(945,'enrol_database','debugdb','0'),(946,'enrol_database','localcoursefield','idnumber'),(947,'enrol_database','localuserfield','idnumber'),(948,'enrol_database','localrolefield','shortname'),(949,'enrol_database','localcategoryfield','id'),(950,'enrol_database','remoteenroltable',''),(951,'enrol_database','remotecoursefield',''),(952,'enrol_database','remoteuserfield',''),(953,'enrol_database','remoterolefield',''),(954,'enrol_database','defaultrole','5'),(955,'enrol_database','ignorehiddencourses','0'),(956,'enrol_database','unenrolaction','0'),(957,'enrol_database','newcoursetable',''),(958,'enrol_database','newcoursefullname','fullname'),(959,'enrol_database','newcourseshortname','shortname'),(960,'enrol_database','newcourseidnumber','idnumber'),(961,'enrol_database','newcoursecategory',''),(962,'enrol_database','defaultcategory','1'),(963,'enrol_database','templatecourse',''),(964,'enrol_flatfile','location',''),(965,'enrol_flatfile','encoding','UTF-8'),(966,'enrol_flatfile','mailstudents','0'),(967,'enrol_flatfile','mailteachers','0'),(968,'enrol_flatfile','mailadmins','0'),(969,'enrol_flatfile','unenrolaction','3'),(970,'enrol_flatfile','expiredaction','3'),(971,'enrol_guest','requirepassword','0'),(972,'enrol_guest','usepasswordpolicy','0'),(973,'enrol_guest','showhint','0'),(974,'enrol_guest','defaultenrol','1'),(975,'enrol_guest','status','1'),(976,'enrol_guest','status_adv',''),(977,'enrol_imsenterprise','imsfilelocation',''),(978,'enrol_imsenterprise','logtolocation',''),(979,'enrol_imsenterprise','mailadmins','0'),(980,'enrol_imsenterprise','createnewusers','0'),(981,'enrol_imsenterprise','imsdeleteusers','0'),(982,'enrol_imsenterprise','fixcaseusernames','0'),(983,'enrol_imsenterprise','fixcasepersonalnames','0'),(984,'enrol_imsenterprise','imssourcedidfallback','0'),(985,'enrol_imsenterprise','imsrolemap01','5'),(986,'enrol_imsenterprise','imsrolemap02','3'),(987,'enrol_imsenterprise','imsrolemap03','3'),(988,'enrol_imsenterprise','imsrolemap04','5'),(989,'enrol_imsenterprise','imsrolemap05','0'),(990,'enrol_imsenterprise','imsrolemap06','4'),(991,'enrol_imsenterprise','imsrolemap07','0'),(992,'enrol_imsenterprise','imsrolemap08','4'),(993,'enrol_imsenterprise','truncatecoursecodes','0'),(994,'enrol_imsenterprise','createnewcourses','0'),(995,'enrol_imsenterprise','createnewcategories','0'),(996,'enrol_imsenterprise','imsunenrol','0'),(997,'enrol_imsenterprise','imscoursemapshortname','coursecode'),(998,'enrol_imsenterprise','imscoursemapfullname','short'),(999,'enrol_imsenterprise','imscoursemapsummary','ignore'),(1000,'enrol_imsenterprise','imsrestricttarget',''),(1001,'enrol_imsenterprise','imscapitafix','0'),(1002,'enrol_manual','expiredaction','1'),(1003,'enrol_manual','expirynotifyhour','6'),(1004,'enrol_manual','defaultenrol','1'),(1005,'enrol_manual','status','0'),(1006,'enrol_manual','roleid','5'),(1007,'enrol_manual','enrolperiod','0'),(1008,'enrol_manual','expirynotify','0'),(1009,'enrol_manual','expirythreshold','86400'),(1010,'enrol_meta','nosyncroleids',''),(1011,'enrol_meta','syncall','1'),(1012,'enrol_meta','unenrolaction','3'),(1013,'enrol_mnet','roleid','5'),(1014,'enrol_mnet','roleid_adv','1'),(1015,'enrol_paypal','paypalbusiness',''),(1016,'enrol_paypal','mailstudents','0'),(1017,'enrol_paypal','mailteachers','0'),(1018,'enrol_paypal','mailadmins','0'),(1019,'enrol_paypal','expiredaction','3'),(1020,'enrol_paypal','status','1'),(1021,'enrol_paypal','cost','0'),(1022,'enrol_paypal','currency','USD'),(1023,'enrol_paypal','roleid','5'),(1024,'enrol_paypal','enrolperiod','0'),(1025,'enrol_self','requirepassword','0'),(1026,'enrol_self','usepasswordpolicy','0'),(1027,'enrol_self','showhint','0'),(1028,'enrol_self','expiredaction','1'),(1029,'enrol_self','expirynotifyhour','6'),(1030,'enrol_self','defaultenrol','1'),(1031,'enrol_self','status','1'),(1032,'enrol_self','newenrols','1'),(1033,'enrol_self','groupkey','0'),(1034,'enrol_self','roleid','5'),(1035,'enrol_self','enrolperiod','0'),(1036,'enrol_self','expirynotify','0'),(1037,'enrol_self','expirythreshold','86400'),(1038,'enrol_self','longtimenosee','0'),(1039,'enrol_self','maxenrolled','0'),(1040,'enrol_self','sendcoursewelcomemessage','1'),(1041,'logstore_database','dbdriver',''),(1042,'logstore_database','dbhost',''),(1043,'logstore_database','dbuser',''),(1044,'logstore_database','dbpass',''),(1045,'logstore_database','dbname',''),(1046,'logstore_database','dbtable',''),(1047,'logstore_database','dbpersist','0'),(1048,'logstore_database','dbsocket',''),(1049,'logstore_database','dbport',''),(1050,'logstore_database','dbschema',''),(1051,'logstore_database','dbcollation',''),(1052,'logstore_database','buffersize','50'),(1053,'logstore_database','logguests','0'),(1054,'logstore_database','includelevels','1,2,0'),(1055,'logstore_database','includeactions','c,r,u,d'),(1056,'logstore_legacy','loglegacy','0'),(1057,'logstore_standard','logguests','1'),(1058,'logstore_standard','loglifetime','0'),(1059,'logstore_standard','buffersize','50'),(1060,'auth_cliauth','version','2014091501'),(1061,'auth_elisfilessso','version','2014030700'),(1062,'enrol_elis','version','2014030700'),(1064,'block_courserequest','version','2014030701'),(1066,'block_elisadmin','version','2014030701'),(1068,'block_enrolsurvey','version','2014030701'),(1070,'block_repository','version','2014030700'),(1071,'repository_elisfiles','version','2014030702'),(1073,'local_datahub','version','2014030701'),(1075,'local_eliscore','version','2014030701'),(1077,'local_elisprogram','version','2014030701'),(1079,'local_elisprogram','notify_addedtowaitlist_user','1'),(1080,'local_elisprogram','notify_enroledfromwaitlist_user','1'),(1081,'local_elisprogram','notify_incompletecourse_user','1'),(1082,'message','airnotifier_provider_local_elisprogram_notify_pm_permitted','permitted'),(1083,'message','email_provider_local_elisprogram_notify_pm_permitted','permitted'),(1084,'message','jabber_provider_local_elisprogram_notify_pm_permitted','permitted'),(1085,'message','popup_provider_local_elisprogram_notify_pm_permitted','permitted'),(1086,'message','message_provider_local_elisprogram_notify_pm_loggedin','popup'),(1087,'message','message_provider_local_elisprogram_notify_pm_loggedoff','email,popup'),(1088,'local_elisreports','version','2014030701'),(1090,'dhimport_header','version','2014030700'),(1091,'dhimport_multiple','version','2014030700'),(1092,'dhimport_sample','version','2014030700'),(1093,'dhimport_version1','version','2014030701'),(1095,'dhimport_version1elis','version','2014030701'),(1097,'dhexport_version1','version','2014030700'),(1099,'dhexport_version1elis','version','2014030700'),(1101,'dhfile_csv','version','2014030700'),(1103,'dhfile_log','version','2014030700'),(1105,'elisfields_manual','version','2014030700'),(1106,'elisfields_moodleprofile','version','2014030700'),(1107,'eliscore_etl','version','2014030700'),(1109,'usetenrol_manual','version','2014030700'),(1110,'usetenrol_moodleprofile','version','2014030701'),(1112,'elisprogram_archive','version','2014030700'),(1114,'elisprogram_enrolrolesync','version','2014030700'),(1116,'elisprogram_enrolrolesync','student_role','0'),(1117,'elisprogram_enrolrolesync','instructor_role','0'),(1118,'elisprogram_preposttest','version','2014030700'),(1120,'elisprogram_usetclassify','version','2014030700'),(1122,'elisprogram_usetdisppriority','version','2014030700'),(1124,'elisprogram_usetgroups','version','2014030700'),(1126,'elisprogram_usetthemes','version','2014030700'),(1128,'rlreport_class_completion_gas_gauge','version','2014030700'),(1129,'rlreport_class_roster','version','2014030700'),(1130,'rlreport_course_completion_by_cluster','version','2014030700'),(1131,'rlreport_course_completion_gas_gauge','version','2014030700'),(1132,'rlreport_course_progress_summary','version','2014030700'),(1133,'rlreport_course_usage_summary','version','2014030700'),(1134,'rlreport_curricula','version','2014030700'),(1135,'rlreport_individual_course_progress','version','2014030700'),(1136,'rlreport_individual_user','version','2014030700'),(1137,'rlreport_nonstarter','version','2014030700'),(1138,'rlreport_registrants_by_course','version','2014030700'),(1139,'rlreport_registrants_by_student','version','2014030700'),(1140,'rlreport_sitewide_course_completion','version','2014030700'),(1141,'rlreport_sitewide_time_summary','version','2014030700'),(1142,'rlreport_sitewide_transcript','version','2014030700'),(1143,'rlreport_user_class_completion','version','2014030700'),(1144,'rlreport_user_class_completion_details','version','2014030700'),(1145,'block_courserequest','course_role','0'),(1146,'block_courserequest','class_role','0'),(1147,'block_courserequest','use_template_by_default','0'),(1148,'block_courserequest','use_course_fields','0'),(1149,'block_courserequest','use_class_fields','1'),(1150,'block_courserequest','create_class_with_course','1'),(1151,'dhimport_version1','identfield_idnumber','1'),(1152,'dhimport_version1','identfield_username','1'),(1153,'dhimport_version1','identfield_email','1'),(1154,'dhimport_version1','creategroupsandgroupings','0'),(1155,'dhimport_version1','createorupdate','0'),(1156,'dhimport_version1','schedule_files_path','/datahub/dhimport_version1'),(1157,'dhimport_version1','user_schedule_file','user.csv'),(1158,'dhimport_version1','course_schedule_file','course.csv'),(1159,'dhimport_version1','enrolment_schedule_file','enroll.csv'),(1160,'dhimport_version1','logfilelocation','/datahub/log'),(1161,'dhimport_version1','emailnotification',''),(1162,'dhimport_version1','allowduplicateemails','0'),(1163,'dhimport_version1','newuseremailenabled','0'),(1164,'dhimport_version1','newuseremailsubject',''),(1165,'dhimport_version1','newuseremailtemplate',''),(1166,'dhimport_version1','newenrolmentemailenabled','0'),(1167,'dhimport_version1','newenrolmentemailfrom','admin'),(1168,'dhimport_version1','newenrolmentemailsubject',''),(1169,'dhimport_version1','newenrolmentemailtemplate',''),(1170,'dhimport_version1elis','identfield_idnumber','1'),(1171,'dhimport_version1elis','identfield_username','1'),(1172,'dhimport_version1elis','identfield_email','1'),(1173,'dhimport_version1elis','createorupdate','0'),(1174,'dhimport_version1elis','schedule_files_path','/datahub/dhimport_version1elis'),(1175,'dhimport_version1elis','user_schedule_file','user.csv'),(1176,'dhimport_version1elis','course_schedule_file','course.csv'),(1177,'dhimport_version1elis','enrolment_schedule_file','enroll.csv'),(1178,'dhimport_version1elis','logfilelocation','/datahub/log'),(1179,'dhimport_version1elis','emailnotification',''),(1180,'dhimport_version1elis','allowduplicateemails','0'),(1181,'dhimport_version1elis','newuseremailenabled','0'),(1182,'dhimport_version1elis','newuseremailsubject',''),(1183,'dhimport_version1elis','newuseremailtemplate',''),(1184,'dhimport_version1elis','newenrolmentemailenabled','0'),(1185,'dhimport_version1elis','newenrolmentemailfrom','admin'),(1186,'dhimport_version1elis','newenrolmentemailsubject',''),(1187,'dhimport_version1elis','newenrolmentemailtemplate',''),(1188,'dhexport_version1','export_path','/datahub/dhexport_version1'),(1189,'dhexport_version1','export_file','export_version1.csv'),(1190,'dhexport_version1','export_file_timestamp','1'),(1191,'dhexport_version1','logfilelocation','/datahub/log'),(1192,'dhexport_version1','emailnotification',''),(1193,'dhexport_version1','nonincremental','0'),(1194,'dhexport_version1','incrementaldelta','1d'),(1195,'dhexport_version1elis','export_path','/datahub/dhexport_version1elis'),(1196,'dhexport_version1elis','export_file','export_version1elis.csv'),(1197,'dhexport_version1elis','export_file_timestamp','1'),(1198,'dhexport_version1elis','logfilelocation','/datahub/log'),(1199,'dhexport_version1elis','emailnotification',''),(1200,'dhexport_version1elis','nonincremental','0'),(1201,'dhexport_version1elis','incrementaldelta','1d'),(1202,'local_datahub','disableincron','0'),(1203,'enrol_elis','defaultenrol','1'),(1204,'enrol_elis','enrol_from_course_catalog','1'),(1205,'enrol_elis','unenrol_from_course_catalog','0'),(1206,'enrol_elis','roleid','5'),(1207,'local_elisprogram','userdefinedtrack','0'),(1208,'local_elisprogram','disablecoursecatalog','0'),(1209,'local_elisprogram','catalog_collapse_count','4'),(1210,'local_elisprogram','enable_curriculum_expiration','0'),(1211,'local_elisprogram','curriculum_expiration_start','1'),(1212,'local_elisprogram','display_completed_courses','1'),(1213,'local_elisprogram','disablecoursecertificates','0'),(1214,'local_elisprogram','disablecertificates','1'),(1215,'local_elisprogram','certificate_border_image','none'),(1216,'local_elisprogram','certificate_seal_image','none'),(1217,'local_elisprogram','certificate_template_file','default.php'),(1218,'local_elisprogram','time_format_12h','0'),(1219,'local_elisprogram','mymoodle_redirect','0'),(1220,'local_elisprogram','auto_assign_user_idnumber','1'),(1221,'local_elisprogram','default_instructor_role','0'),(1222,'local_elisprogram','force_unenrol_in_moodle','0'),(1223,'local_elisprogram','num_block_icons','5'),(1224,'local_elisprogram','display_clusters_at_top_level','1'),(1225,'local_elisprogram','display_curricula_at_top_level','0'),(1226,'local_elisprogram','default_cluster_role_id','0'),(1227,'local_elisprogram','default_curriculum_role_id','0'),(1228,'local_elisprogram','default_course_role_id','0'),(1229,'local_elisprogram','default_class_role_id','0'),(1230,'local_elisprogram','default_track_role_id','0'),(1231,'local_elisprogram','autocreated_unknown_is_yes','1'),(1232,'elisprogram_usetgroups','userset_groups','0'),(1233,'elisprogram_usetgroups','site_course_userset_groups','0'),(1234,'elisprogram_usetgroups','userset_groupings','0'),(1235,'local_elisprogram','legacy_show_inactive_users','0'),(1236,'auth/googleoauth2','lastusernumber','1');

/*Table structure for table `mdl_context` */

DROP TABLE IF EXISTS `mdl_context`;

CREATE TABLE `mdl_context` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `contextlevel` bigint(10) NOT NULL DEFAULT '0',
  `instanceid` bigint(10) NOT NULL DEFAULT '0',
  `path` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `depth` tinyint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_cont_conins_uix` (`contextlevel`,`instanceid`),
  KEY `mdl_cont_ins_ix` (`instanceid`),
  KEY `mdl_cont_pat_ix` (`path`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='one of these must be set';

/*Data for the table `mdl_context` */

insert  into `mdl_context`(`id`,`contextlevel`,`instanceid`,`path`,`depth`) values (1,10,0,'/1',1),(2,50,1,'/1/2',2),(3,40,1,'/1/3',2),(4,30,1,'/1/4',2),(5,30,2,'/1/5',2),(6,80,1,'/1/2/6',3),(7,80,2,'/1/2/7',3),(8,80,3,'/1/2/8',3),(9,80,4,'/1/9',2),(10,80,5,'/1/10',2),(11,80,6,'/1/11',2),(12,80,7,'/1/12',2),(13,80,8,'/1/13',2),(14,80,9,'/1/14',2),(15,80,10,'/1/15',2),(16,30,3,'/1/16',2),(17,30,4,'/1/17',2),(18,15,2,'/1/18',2),(19,15,3,'/1/19',2);

/*Table structure for table `mdl_context_temp` */

DROP TABLE IF EXISTS `mdl_context_temp`;

CREATE TABLE `mdl_context_temp` (
  `id` bigint(10) NOT NULL,
  `path` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `depth` tinyint(2) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Used by build_context_path() in upgrade and cron to keep con';

/*Data for the table `mdl_context_temp` */

/*Table structure for table `mdl_course` */

DROP TABLE IF EXISTS `mdl_course`;

CREATE TABLE `mdl_course` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `category` bigint(10) NOT NULL DEFAULT '0',
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  `fullname` varchar(254) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `shortname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `idnumber` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `summary` longtext COLLATE utf8_unicode_ci,
  `summaryformat` tinyint(2) NOT NULL DEFAULT '0',
  `format` varchar(21) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'topics',
  `showgrades` tinyint(2) NOT NULL DEFAULT '1',
  `newsitems` mediumint(5) NOT NULL DEFAULT '1',
  `startdate` bigint(10) NOT NULL DEFAULT '0',
  `marker` bigint(10) NOT NULL DEFAULT '0',
  `maxbytes` bigint(10) NOT NULL DEFAULT '0',
  `legacyfiles` smallint(4) NOT NULL DEFAULT '0',
  `showreports` smallint(4) NOT NULL DEFAULT '0',
  `visible` tinyint(1) NOT NULL DEFAULT '1',
  `visibleold` tinyint(1) NOT NULL DEFAULT '1',
  `groupmode` smallint(4) NOT NULL DEFAULT '0',
  `groupmodeforce` smallint(4) NOT NULL DEFAULT '0',
  `defaultgroupingid` bigint(10) NOT NULL DEFAULT '0',
  `lang` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `calendartype` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `theme` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `requested` tinyint(1) NOT NULL DEFAULT '0',
  `enablecompletion` tinyint(1) NOT NULL DEFAULT '0',
  `completionnotify` tinyint(1) NOT NULL DEFAULT '0',
  `cacherev` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_cour_cat_ix` (`category`),
  KEY `mdl_cour_idn_ix` (`idnumber`),
  KEY `mdl_cour_sho_ix` (`shortname`),
  KEY `mdl_cour_sor_ix` (`sortorder`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Central course table';

/*Data for the table `mdl_course` */

insert  into `mdl_course`(`id`,`category`,`sortorder`,`fullname`,`shortname`,`idnumber`,`summary`,`summaryformat`,`format`,`showgrades`,`newsitems`,`startdate`,`marker`,`maxbytes`,`legacyfiles`,`showreports`,`visible`,`visibleold`,`groupmode`,`groupmodeforce`,`defaultgroupingid`,`lang`,`calendartype`,`theme`,`timecreated`,`timemodified`,`requested`,`enablecompletion`,`completionnotify`,`cacherev`) values (1,0,0,'lms.sunnet.us','lms.sunnet.us','','lms.sunnet.us',0,'site',1,3,0,0,0,0,0,1,1,0,0,0,'','','',1413856721,1413858116,0,0,0,1413861558);

/*Table structure for table `mdl_course_categories` */

DROP TABLE IF EXISTS `mdl_course_categories`;

CREATE TABLE `mdl_course_categories` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `idnumber` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` tinyint(2) NOT NULL DEFAULT '0',
  `parent` bigint(10) NOT NULL DEFAULT '0',
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  `coursecount` bigint(10) NOT NULL DEFAULT '0',
  `visible` tinyint(1) NOT NULL DEFAULT '1',
  `visibleold` tinyint(1) NOT NULL DEFAULT '1',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `depth` bigint(10) NOT NULL DEFAULT '0',
  `path` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `theme` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_courcate_par_ix` (`parent`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Course categories';

/*Data for the table `mdl_course_categories` */

insert  into `mdl_course_categories`(`id`,`name`,`idnumber`,`description`,`descriptionformat`,`parent`,`sortorder`,`coursecount`,`visible`,`visibleold`,`timemodified`,`depth`,`path`,`theme`) values (1,'其他',NULL,NULL,0,0,10000,0,1,1,1413856722,1,'/1',NULL);

/*Table structure for table `mdl_course_completion_aggr_methd` */

DROP TABLE IF EXISTS `mdl_course_completion_aggr_methd`;

CREATE TABLE `mdl_course_completion_aggr_methd` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `criteriatype` bigint(10) DEFAULT NULL,
  `method` tinyint(1) NOT NULL DEFAULT '0',
  `value` decimal(10,5) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_courcompaggrmeth_coucr_uix` (`course`,`criteriatype`),
  KEY `mdl_courcompaggrmeth_cou_ix` (`course`),
  KEY `mdl_courcompaggrmeth_cri_ix` (`criteriatype`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Course completion aggregation methods for criteria';

/*Data for the table `mdl_course_completion_aggr_methd` */

/*Table structure for table `mdl_course_completion_crit_compl` */

DROP TABLE IF EXISTS `mdl_course_completion_crit_compl`;

CREATE TABLE `mdl_course_completion_crit_compl` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `course` bigint(10) NOT NULL DEFAULT '0',
  `criteriaid` bigint(10) NOT NULL DEFAULT '0',
  `gradefinal` decimal(10,5) DEFAULT NULL,
  `unenroled` bigint(10) DEFAULT NULL,
  `timecompleted` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_courcompcritcomp_useco_uix` (`userid`,`course`,`criteriaid`),
  KEY `mdl_courcompcritcomp_use_ix` (`userid`),
  KEY `mdl_courcompcritcomp_cou_ix` (`course`),
  KEY `mdl_courcompcritcomp_cri_ix` (`criteriaid`),
  KEY `mdl_courcompcritcomp_tim_ix` (`timecompleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Course completion user records';

/*Data for the table `mdl_course_completion_crit_compl` */

/*Table structure for table `mdl_course_completion_criteria` */

DROP TABLE IF EXISTS `mdl_course_completion_criteria`;

CREATE TABLE `mdl_course_completion_criteria` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `criteriatype` bigint(10) NOT NULL DEFAULT '0',
  `module` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `moduleinstance` bigint(10) DEFAULT NULL,
  `courseinstance` bigint(10) DEFAULT NULL,
  `enrolperiod` bigint(10) DEFAULT NULL,
  `timeend` bigint(10) DEFAULT NULL,
  `gradepass` decimal(10,5) DEFAULT NULL,
  `role` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_courcompcrit_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Course completion criteria';

/*Data for the table `mdl_course_completion_criteria` */

/*Table structure for table `mdl_course_completions` */

DROP TABLE IF EXISTS `mdl_course_completions`;

CREATE TABLE `mdl_course_completions` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `course` bigint(10) NOT NULL DEFAULT '0',
  `timeenrolled` bigint(10) NOT NULL DEFAULT '0',
  `timestarted` bigint(10) NOT NULL DEFAULT '0',
  `timecompleted` bigint(10) DEFAULT NULL,
  `reaggregate` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_courcomp_usecou_uix` (`userid`,`course`),
  KEY `mdl_courcomp_use_ix` (`userid`),
  KEY `mdl_courcomp_cou_ix` (`course`),
  KEY `mdl_courcomp_tim_ix` (`timecompleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Course completion records';

/*Data for the table `mdl_course_completions` */

/*Table structure for table `mdl_course_format_options` */

DROP TABLE IF EXISTS `mdl_course_format_options`;

CREATE TABLE `mdl_course_format_options` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL,
  `format` varchar(21) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `sectionid` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_courformopti_couforsec_uix` (`courseid`,`format`,`sectionid`,`name`),
  KEY `mdl_courformopti_cou_ix` (`courseid`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores format-specific options for the course or course sect';

/*Data for the table `mdl_course_format_options` */

insert  into `mdl_course_format_options`(`id`,`courseid`,`format`,`sectionid`,`name`,`value`) values (1,1,'site',0,'numsections','1');

/*Table structure for table `mdl_course_modules` */

DROP TABLE IF EXISTS `mdl_course_modules`;

CREATE TABLE `mdl_course_modules` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `module` bigint(10) NOT NULL DEFAULT '0',
  `instance` bigint(10) NOT NULL DEFAULT '0',
  `section` bigint(10) NOT NULL DEFAULT '0',
  `idnumber` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `added` bigint(10) NOT NULL DEFAULT '0',
  `score` smallint(4) NOT NULL DEFAULT '0',
  `indent` mediumint(5) NOT NULL DEFAULT '0',
  `visible` tinyint(1) NOT NULL DEFAULT '1',
  `visibleold` tinyint(1) NOT NULL DEFAULT '1',
  `groupmode` smallint(4) NOT NULL DEFAULT '0',
  `groupingid` bigint(10) NOT NULL DEFAULT '0',
  `groupmembersonly` smallint(4) NOT NULL DEFAULT '0',
  `completion` tinyint(1) NOT NULL DEFAULT '0',
  `completiongradeitemnumber` bigint(10) DEFAULT NULL,
  `completionview` tinyint(1) NOT NULL DEFAULT '0',
  `completionexpected` bigint(10) NOT NULL DEFAULT '0',
  `showdescription` tinyint(1) NOT NULL DEFAULT '0',
  `availability` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `mdl_courmodu_vis_ix` (`visible`),
  KEY `mdl_courmodu_cou_ix` (`course`),
  KEY `mdl_courmodu_mod_ix` (`module`),
  KEY `mdl_courmodu_ins_ix` (`instance`),
  KEY `mdl_courmodu_idncou_ix` (`idnumber`,`course`),
  KEY `mdl_courmodu_gro_ix` (`groupingid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='course_modules table retrofitted from MySQL';

/*Data for the table `mdl_course_modules` */

/*Table structure for table `mdl_course_modules_completion` */

DROP TABLE IF EXISTS `mdl_course_modules_completion`;

CREATE TABLE `mdl_course_modules_completion` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `coursemoduleid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `completionstate` tinyint(1) NOT NULL,
  `viewed` tinyint(1) DEFAULT NULL,
  `timemodified` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_courmoducomp_usecou_uix` (`userid`,`coursemoduleid`),
  KEY `mdl_courmoducomp_cou_ix` (`coursemoduleid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores the completion state (completed or not completed, etc';

/*Data for the table `mdl_course_modules_completion` */

/*Table structure for table `mdl_course_published` */

DROP TABLE IF EXISTS `mdl_course_published`;

CREATE TABLE `mdl_course_published` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `huburl` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `courseid` bigint(10) NOT NULL,
  `timepublished` bigint(10) NOT NULL,
  `enrollable` tinyint(1) NOT NULL DEFAULT '1',
  `hubcourseid` bigint(10) NOT NULL,
  `status` tinyint(1) DEFAULT '0',
  `timechecked` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Information about how and when an local courses were publish';

/*Data for the table `mdl_course_published` */

/*Table structure for table `mdl_course_request` */

DROP TABLE IF EXISTS `mdl_course_request`;

CREATE TABLE `mdl_course_request` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `fullname` varchar(254) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `shortname` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `summary` longtext COLLATE utf8_unicode_ci NOT NULL,
  `summaryformat` tinyint(2) NOT NULL DEFAULT '0',
  `category` bigint(10) NOT NULL DEFAULT '0',
  `reason` longtext COLLATE utf8_unicode_ci NOT NULL,
  `requester` bigint(10) NOT NULL DEFAULT '0',
  `password` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_courrequ_sho_ix` (`shortname`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='course requests';

/*Data for the table `mdl_course_request` */

/*Table structure for table `mdl_course_sections` */

DROP TABLE IF EXISTS `mdl_course_sections`;

CREATE TABLE `mdl_course_sections` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `section` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `summary` longtext COLLATE utf8_unicode_ci,
  `summaryformat` tinyint(2) NOT NULL DEFAULT '0',
  `sequence` longtext COLLATE utf8_unicode_ci,
  `visible` tinyint(1) NOT NULL DEFAULT '1',
  `availability` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_coursect_cousec_uix` (`course`,`section`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='to define the sections for each course';

/*Data for the table `mdl_course_sections` */

/*Table structure for table `mdl_data` */

DROP TABLE IF EXISTS `mdl_data`;

CREATE TABLE `mdl_data` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci NOT NULL,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `comments` smallint(4) NOT NULL DEFAULT '0',
  `timeavailablefrom` bigint(10) NOT NULL DEFAULT '0',
  `timeavailableto` bigint(10) NOT NULL DEFAULT '0',
  `timeviewfrom` bigint(10) NOT NULL DEFAULT '0',
  `timeviewto` bigint(10) NOT NULL DEFAULT '0',
  `requiredentries` int(8) NOT NULL DEFAULT '0',
  `requiredentriestoview` int(8) NOT NULL DEFAULT '0',
  `maxentries` int(8) NOT NULL DEFAULT '0',
  `rssarticles` smallint(4) NOT NULL DEFAULT '0',
  `singletemplate` longtext COLLATE utf8_unicode_ci,
  `listtemplate` longtext COLLATE utf8_unicode_ci,
  `listtemplateheader` longtext COLLATE utf8_unicode_ci,
  `listtemplatefooter` longtext COLLATE utf8_unicode_ci,
  `addtemplate` longtext COLLATE utf8_unicode_ci,
  `rsstemplate` longtext COLLATE utf8_unicode_ci,
  `rsstitletemplate` longtext COLLATE utf8_unicode_ci,
  `csstemplate` longtext COLLATE utf8_unicode_ci,
  `jstemplate` longtext COLLATE utf8_unicode_ci,
  `asearchtemplate` longtext COLLATE utf8_unicode_ci,
  `approval` smallint(4) NOT NULL DEFAULT '0',
  `scale` bigint(10) NOT NULL DEFAULT '0',
  `assessed` bigint(10) NOT NULL DEFAULT '0',
  `assesstimestart` bigint(10) NOT NULL DEFAULT '0',
  `assesstimefinish` bigint(10) NOT NULL DEFAULT '0',
  `defaultsort` bigint(10) NOT NULL DEFAULT '0',
  `defaultsortdir` smallint(4) NOT NULL DEFAULT '0',
  `editany` smallint(4) NOT NULL DEFAULT '0',
  `notification` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_data_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='all database activities';

/*Data for the table `mdl_data` */

/*Table structure for table `mdl_data_content` */

DROP TABLE IF EXISTS `mdl_data_content`;

CREATE TABLE `mdl_data_content` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `fieldid` bigint(10) NOT NULL DEFAULT '0',
  `recordid` bigint(10) NOT NULL DEFAULT '0',
  `content` longtext COLLATE utf8_unicode_ci,
  `content1` longtext COLLATE utf8_unicode_ci,
  `content2` longtext COLLATE utf8_unicode_ci,
  `content3` longtext COLLATE utf8_unicode_ci,
  `content4` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `mdl_datacont_rec_ix` (`recordid`),
  KEY `mdl_datacont_fie_ix` (`fieldid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='the content introduced in each record/fields';

/*Data for the table `mdl_data_content` */

/*Table structure for table `mdl_data_fields` */

DROP TABLE IF EXISTS `mdl_data_fields`;

CREATE TABLE `mdl_data_fields` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `dataid` bigint(10) NOT NULL DEFAULT '0',
  `type` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci NOT NULL,
  `param1` longtext COLLATE utf8_unicode_ci,
  `param2` longtext COLLATE utf8_unicode_ci,
  `param3` longtext COLLATE utf8_unicode_ci,
  `param4` longtext COLLATE utf8_unicode_ci,
  `param5` longtext COLLATE utf8_unicode_ci,
  `param6` longtext COLLATE utf8_unicode_ci,
  `param7` longtext COLLATE utf8_unicode_ci,
  `param8` longtext COLLATE utf8_unicode_ci,
  `param9` longtext COLLATE utf8_unicode_ci,
  `param10` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `mdl_datafiel_typdat_ix` (`type`,`dataid`),
  KEY `mdl_datafiel_dat_ix` (`dataid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='every field available';

/*Data for the table `mdl_data_fields` */

/*Table structure for table `mdl_data_records` */

DROP TABLE IF EXISTS `mdl_data_records`;

CREATE TABLE `mdl_data_records` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `groupid` bigint(10) NOT NULL DEFAULT '0',
  `dataid` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `approved` smallint(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_datareco_dat_ix` (`dataid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='every record introduced';

/*Data for the table `mdl_data_records` */

/*Table structure for table `mdl_dhexport_version1_field` */

DROP TABLE IF EXISTS `mdl_dhexport_version1_field`;

CREATE TABLE `mdl_dhexport_version1_field` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `fieldid` bigint(10) NOT NULL,
  `header` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `fieldorder` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_dhexversfiel_fie_uix` (`fieldid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Custom fields used by version 1 export plugin';

/*Data for the table `mdl_dhexport_version1_field` */

/*Table structure for table `mdl_dhexport_version1elis_fld` */

DROP TABLE IF EXISTS `mdl_dhexport_version1elis_fld`;

CREATE TABLE `mdl_dhexport_version1elis_fld` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `fieldset` varchar(127) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `field` varchar(127) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `header` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `fieldorder` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_dhexversfld_fiefie_uix` (`fieldset`,`field`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Custom fields used by version 1 elis export plugin';

/*Data for the table `mdl_dhexport_version1elis_fld` */

/*Table structure for table `mdl_dhimport_version1_mapping` */

DROP TABLE IF EXISTS `mdl_dhimport_version1_mapping`;

CREATE TABLE `mdl_dhimport_version1_mapping` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `entitytype` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `standardfieldname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `customfieldname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_dhimversmapp_entsta_uix` (`entitytype`,`standardfieldname`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Field mapping configuration for version 1 import plugin';

/*Data for the table `mdl_dhimport_version1_mapping` */

/*Table structure for table `mdl_dhimport_version1elis_map` */

DROP TABLE IF EXISTS `mdl_dhimport_version1elis_map`;

CREATE TABLE `mdl_dhimport_version1elis_map` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `entitytype` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `standardfieldname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `customfieldname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_dhimversmap_entsta_uix` (`entitytype`,`standardfieldname`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Field mapping configuration for version ELIS 1 import plugin';

/*Data for the table `mdl_dhimport_version1elis_map` */

/*Table structure for table `mdl_eliscore_etl_modactivity` */

DROP TABLE IF EXISTS `mdl_eliscore_etl_modactivity`;

CREATE TABLE `mdl_eliscore_etl_modactivity` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL,
  `courseid` bigint(10) NOT NULL,
  `cmid` bigint(10) NOT NULL,
  `hour` bigint(10) NOT NULL,
  `duration` smallint(4) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_elisetlmoda_usecmihou_uix` (`userid`,`cmid`,`hour`),
  KEY `mdl_elisetlmoda_cmi_ix` (`cmid`),
  KEY `mdl_elisetlmoda_hou_ix` (`hour`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='User activity session data per activity module';

/*Data for the table `mdl_eliscore_etl_modactivity` */

/*Table structure for table `mdl_eliscore_etl_useractivity` */

DROP TABLE IF EXISTS `mdl_eliscore_etl_useractivity`;

CREATE TABLE `mdl_eliscore_etl_useractivity` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL,
  `courseid` bigint(10) NOT NULL,
  `hour` bigint(10) NOT NULL,
  `duration` smallint(4) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_elisetluser_usecouhou_uix` (`userid`,`courseid`,`hour`),
  KEY `mdl_elisetluser_cou_ix` (`courseid`),
  KEY `mdl_elisetluser_hou_ix` (`hour`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='User activity sesssion data';

/*Data for the table `mdl_eliscore_etl_useractivity` */

/*Table structure for table `mdl_elisprogram_usetclassify` */

DROP TABLE IF EXISTS `mdl_elisprogram_usetclassify`;

CREATE TABLE `mdl_elisprogram_usetclassify` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `shortname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `params` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_elisuset_sho_uix` (`shortname`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Userset classifications';

/*Data for the table `mdl_elisprogram_usetclassify` */

insert  into `mdl_elisprogram_usetclassify`(`id`,`shortname`,`name`,`params`) values (1,'regular','User Set','a:2:{s:19:\"autoenrol_curricula\";i:1;s:16:\"autoenrol_tracks\";i:1;}');

/*Table structure for table `mdl_enrol` */

DROP TABLE IF EXISTS `mdl_enrol`;

CREATE TABLE `mdl_enrol` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `enrol` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `status` bigint(10) NOT NULL DEFAULT '0',
  `courseid` bigint(10) NOT NULL,
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `enrolperiod` bigint(10) DEFAULT '0',
  `enrolstartdate` bigint(10) DEFAULT '0',
  `enrolenddate` bigint(10) DEFAULT '0',
  `expirynotify` tinyint(1) DEFAULT '0',
  `expirythreshold` bigint(10) DEFAULT '0',
  `notifyall` tinyint(1) DEFAULT '0',
  `password` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `cost` varchar(20) COLLATE utf8_unicode_ci DEFAULT NULL,
  `currency` varchar(3) COLLATE utf8_unicode_ci DEFAULT NULL,
  `roleid` bigint(10) DEFAULT '0',
  `customint1` bigint(10) DEFAULT NULL,
  `customint2` bigint(10) DEFAULT NULL,
  `customint3` bigint(10) DEFAULT NULL,
  `customint4` bigint(10) DEFAULT NULL,
  `customint5` bigint(10) DEFAULT NULL,
  `customint6` bigint(10) DEFAULT NULL,
  `customint7` bigint(10) DEFAULT NULL,
  `customint8` bigint(10) DEFAULT NULL,
  `customchar1` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `customchar2` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `customchar3` varchar(1333) COLLATE utf8_unicode_ci DEFAULT NULL,
  `customdec1` decimal(12,7) DEFAULT NULL,
  `customdec2` decimal(12,7) DEFAULT NULL,
  `customtext1` longtext COLLATE utf8_unicode_ci,
  `customtext2` longtext COLLATE utf8_unicode_ci,
  `customtext3` longtext COLLATE utf8_unicode_ci,
  `customtext4` longtext COLLATE utf8_unicode_ci,
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_enro_enr_ix` (`enrol`),
  KEY `mdl_enro_cou_ix` (`courseid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Instances of enrolment plugins used in courses, fields marke';

/*Data for the table `mdl_enrol` */

/*Table structure for table `mdl_enrol_flatfile` */

DROP TABLE IF EXISTS `mdl_enrol_flatfile`;

CREATE TABLE `mdl_enrol_flatfile` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `action` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `roleid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `courseid` bigint(10) NOT NULL,
  `timestart` bigint(10) NOT NULL DEFAULT '0',
  `timeend` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_enroflat_cou_ix` (`courseid`),
  KEY `mdl_enroflat_use_ix` (`userid`),
  KEY `mdl_enroflat_rol_ix` (`roleid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='enrol_flatfile table retrofitted from MySQL';

/*Data for the table `mdl_enrol_flatfile` */

/*Table structure for table `mdl_enrol_paypal` */

DROP TABLE IF EXISTS `mdl_enrol_paypal`;

CREATE TABLE `mdl_enrol_paypal` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `business` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `receiver_email` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `receiver_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `item_name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `instanceid` bigint(10) NOT NULL DEFAULT '0',
  `memo` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `tax` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `option_name1` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `option_selection1_x` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `option_name2` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `option_selection2_x` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `payment_status` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `pending_reason` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `reason_code` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `txn_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `parent_txn_id` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `payment_type` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timeupdated` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Holds all known information about PayPal transactions';

/*Data for the table `mdl_enrol_paypal` */

/*Table structure for table `mdl_event` */

DROP TABLE IF EXISTS `mdl_event`;

CREATE TABLE `mdl_event` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` longtext COLLATE utf8_unicode_ci NOT NULL,
  `description` longtext COLLATE utf8_unicode_ci NOT NULL,
  `format` smallint(4) NOT NULL DEFAULT '0',
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `groupid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `repeatid` bigint(10) NOT NULL DEFAULT '0',
  `modulename` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `instance` bigint(10) NOT NULL DEFAULT '0',
  `eventtype` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timestart` bigint(10) NOT NULL DEFAULT '0',
  `timeduration` bigint(10) NOT NULL DEFAULT '0',
  `visible` smallint(4) NOT NULL DEFAULT '1',
  `uuid` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `sequence` bigint(10) NOT NULL DEFAULT '1',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `subscriptionid` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_even_cou_ix` (`courseid`),
  KEY `mdl_even_use_ix` (`userid`),
  KEY `mdl_even_tim_ix` (`timestart`),
  KEY `mdl_even_tim2_ix` (`timeduration`),
  KEY `mdl_even_grocouvisuse_ix` (`groupid`,`courseid`,`visible`,`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='For everything with a time associated to it';

/*Data for the table `mdl_event` */

/*Table structure for table `mdl_event_subscriptions` */

DROP TABLE IF EXISTS `mdl_event_subscriptions`;

CREATE TABLE `mdl_event_subscriptions` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `url` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `groupid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `eventtype` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `pollinterval` bigint(10) NOT NULL DEFAULT '0',
  `lastupdated` bigint(10) DEFAULT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Tracks subscriptions to remote calendars.';

/*Data for the table `mdl_event_subscriptions` */

/*Table structure for table `mdl_events_handlers` */

DROP TABLE IF EXISTS `mdl_events_handlers`;

CREATE TABLE `mdl_events_handlers` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `eventname` varchar(166) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `component` varchar(166) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `handlerfile` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `handlerfunction` longtext COLLATE utf8_unicode_ci,
  `schedule` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `status` bigint(10) NOT NULL DEFAULT '0',
  `internal` tinyint(2) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_evenhand_evecom_uix` (`eventname`,`component`)
) ENGINE=InnoDB AUTO_INCREMENT=36 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table is for storing which components requests what typ';

/*Data for the table `mdl_events_handlers` */

insert  into `mdl_events_handlers`(`id`,`eventname`,`component`,`handlerfile`,`handlerfunction`,`schedule`,`status`,`internal`) values (1,'user_deleted','repository_elisfiles','/repository/elisfiles/lib/eventlib.php','s:23:\"elis_files_user_deleted\";','instant',0,1),(2,'role_unassigned','repository_elisfiles','/repository/elisfiles/lib/eventlib.php','s:26:\"elis_files_role_unassigned\";','instant',0,1),(3,'cluster_assigned','repository_elisfiles','/repository/elisfiles/lib/eventlib.php','s:27:\"elis_files_userset_assigned\";','instant',0,1),(4,'cluster_deassigned','repository_elisfiles','/repository/elisfiles/lib/eventlib.php','s:29:\"elis_files_userset_deassigned\";','instant',0,1),(5,'user_created','repository_elisfiles','/repository/elisfiles/lib/eventlib.php','s:23:\"elis_files_user_created\";','instant',0,1),(6,'course_deleted','repository_elisfiles','/repository/elisfiles/lib/eventlib.php','s:25:\"elis_files_course_deleted\";','instant',0,1),(7,'cluster_deleted','repository_elisfiles','/repository/elisfiles/lib/eventlib.php','s:26:\"elis_files_userset_deleted\";','instant',0,1),(8,'message_send','local_elisprogram','/local/elisprogram/lib/notifications.php','s:22:\"pm_notify_send_handler\";','instant',0,1),(9,'role_assigned','local_elisprogram','/local/elisprogram/lib/notifications.php','s:29:\"pm_notify_role_assign_handler\";','instant',0,1),(10,'role_unassigned','local_elisprogram','/local/elisprogram/lib/notifications.php','s:31:\"pm_notify_role_unassign_handler\";','instant',0,1),(11,'track_assigned','local_elisprogram','/local/elisprogram/lib/notifications.php','s:30:\"pm_notify_track_assign_handler\";','instant',0,1),(12,'class_completed','local_elisprogram','/local/elisprogram/lib/notifications.php','s:33:\"pm_notify_class_completed_handler\";','instant',0,1),(13,'class_notcompleted','local_elisprogram','/local/elisprogram/lib/data/student.class.php','a:2:{i:0;s:7:\"student\";i:1;s:26:\"class_notcompleted_handler\";}','instant',0,1),(14,'class_notstarted','local_elisprogram','/local/elisprogram/lib/data/student.class.php','a:2:{i:0;s:7:\"student\";i:1;s:24:\"class_notstarted_handler\";}','instant',0,1),(15,'course_recurrence','local_elisprogram','/local/elisprogram/lib/data/course.class.php','a:2:{i:0;s:6:\"course\";i:1;s:25:\"course_recurrence_handler\";}','instant',0,1),(16,'curriculum_completed','local_elisprogram','/local/elisprogram/lib/data/curriculumstudent.class.php','a:2:{i:0;s:17:\"curriculumstudent\";i:1;s:28:\"curriculum_completed_handler\";}','instant',0,1),(17,'curriculum_notcompleted','local_elisprogram','/local/elisprogram/lib/data/curriculumstudent.class.php','a:2:{i:0;s:17:\"curriculumstudent\";i:1;s:31:\"curriculum_notcompleted_handler\";}','instant',0,1),(18,'curriculum_recurrence','local_elisprogram','/local/elisprogram/lib/data/curriculum.class.php','a:2:{i:0;s:10:\"curriculum\";i:1;s:29:\"curriculum_recurrence_handler\";}','instant',0,1),(19,'cluster_assigned','local_elisprogram','/local/elisprogram/lib/data/userset.class.php','a:2:{i:0;s:7:\"userset\";i:1;s:24:\"cluster_assigned_handler\";}','instant',0,1),(20,'cluster_deassigned','local_elisprogram','/local/elisprogram/lib/data/userset.class.php','a:2:{i:0;s:7:\"userset\";i:1;s:26:\"cluster_deassigned_handler\";}','instant',0,1),(21,'user_created','local_elisprogram','/local/elisprogram/lib/lib.php','s:20:\"pm_moodle_user_to_pm\";','instant',0,1),(22,'local_elisprogram_cls_completed','local_elisprogram','/local/elisprogram/lib/lib.php','s:18:\"pm_course_complete\";','instant',0,1),(23,'crlm_instructor_assigned','local_elisprogram','/local/elisprogram/lib/notifications.php','s:37:\"pm_notify_instructor_assigned_handler\";','instant',0,1),(24,'crlm_instructor_unassigned','local_elisprogram','/local/elisprogram/lib/notifications.php','s:39:\"pm_notify_instructor_unassigned_handler\";','instant',0,1),(25,'user_deleted','local_elisprogram','/local/elisprogram/lib/data/user.class.php','a:2:{i:0;s:4:\"user\";i:1;s:20:\"user_deleted_handler\";}','instant',0,1),(26,'user_updated','usetenrol_moodleprofile','/local/elisprogram/enrol/userset/moodleprofile/lib.php','s:30:\"cluster_profile_update_handler\";','cron',0,1),(27,'user_created','usetenrol_moodleprofile','/local/elisprogram/enrol/userset/moodleprofile/lib.php','s:30:\"cluster_profile_update_handler\";','cron',0,1),(28,'role_assigned','elisprogram_enrolrolesync','/local/elisprogram/plugins/enrolrolesync/lib.php','a:2:{i:0;s:19:\"enrolment_role_sync\";i:1;s:13:\"role_assigned\";}','instant',0,1),(29,'cluster_assigned','elisprogram_usetgroups','/local/elisprogram/plugins/usetgroups/lib.php','s:39:\"userset_groups_userset_assigned_handler\";','instant',0,1),(30,'pm_classinstance_associated','elisprogram_usetgroups','/local/elisprogram/plugins/usetgroups/lib.php','s:50:\"userset_groups_pm_classinstance_associated_handler\";','instant',0,1),(31,'pm_track_class_associated','elisprogram_usetgroups','/local/elisprogram/plugins/usetgroups/lib.php','s:48:\"userset_groups_pm_track_class_associated_handler\";','instant',0,1),(32,'pm_userset_track_associated','elisprogram_usetgroups','/local/elisprogram/plugins/usetgroups/lib.php','s:50:\"userset_groups_pm_userset_track_associated_handler\";','instant',0,1),(33,'pm_userset_updated','elisprogram_usetgroups','/local/elisprogram/plugins/usetgroups/lib.php','s:41:\"userset_groups_pm_userset_updated_handler\";','instant',0,1),(34,'pm_userset_created','elisprogram_usetgroups','/local/elisprogram/plugins/usetgroups/lib.php','s:41:\"userset_groups_pm_userset_created_handler\";','instant',0,1),(35,'role_assigned','elisprogram_usetgroups','/local/elisprogram/plugins/usetgroups/lib.php','s:36:\"userset_groups_role_assigned_handler\";','instant',0,1);

/*Table structure for table `mdl_events_queue` */

DROP TABLE IF EXISTS `mdl_events_queue`;

CREATE TABLE `mdl_events_queue` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `eventdata` longtext COLLATE utf8_unicode_ci NOT NULL,
  `stackdump` longtext COLLATE utf8_unicode_ci,
  `userid` bigint(10) DEFAULT NULL,
  `timecreated` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_evenqueu_use_ix` (`userid`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table is for storing queued events. It stores only one ';

/*Data for the table `mdl_events_queue` */

insert  into `mdl_events_queue`(`id`,`eventdata`,`stackdump`,`userid`,`timecreated`) values (1,'Tzo4OiJzdGRDbGFzcyI6NTM6e3M6MjoiaWQiO3M6MToiMyI7czo0OiJhdXRoIjtzOjc6ImNsaWF1dGgiO3M6OToiY29uZmlybWVkIjtzOjE6IjEiO3M6MTI6InBvbGljeWFncmVlZCI7czoxOiIwIjtzOjc6ImRlbGV0ZWQiO3M6MToiMCI7czo5OiJzdXNwZW5kZWQiO3M6MToiMCI7czoxMDoibW5ldGhvc3RpZCI7czoxOiIxIjtzOjg6InVzZXJuYW1lIjtzOjE5OiJjbGlzdW5uZXRAZ21haWwuY29tIjtzOjg6InBhc3N3b3JkIjtzOjEwOiJub3QgY2FjaGVkIjtzOjg6ImlkbnVtYmVyIjtzOjA6IiI7czo5OiJmaXJzdG5hbWUiO3M6MDoiIjtzOjg6Imxhc3RuYW1lIjtzOjA6IiI7czo1OiJlbWFpbCI7czowOiIiO3M6OToiZW1haWxzdG9wIjtzOjE6IjAiO3M6MzoiaWNxIjtzOjA6IiI7czo1OiJza3lwZSI7czowOiIiO3M6NToieWFob28iO3M6MDoiIjtzOjM6ImFpbSI7czowOiIiO3M6MzoibXNuIjtzOjA6IiI7czo2OiJwaG9uZTEiO3M6MDoiIjtzOjY6InBob25lMiI7czowOiIiO3M6MTE6Imluc3RpdHV0aW9uIjtzOjA6IiI7czoxMDoiZGVwYXJ0bWVudCI7czowOiIiO3M6NzoiYWRkcmVzcyI7czowOiIiO3M6NDoiY2l0eSI7czowOiIiO3M6NzoiY291bnRyeSI7czowOiIiO3M6NDoibGFuZyI7czo1OiJ6aF9jbiI7czoxMjoiY2FsZW5kYXJ0eXBlIjtzOjk6ImdyZWdvcmlhbiI7czo1OiJ0aGVtZSI7czowOiIiO3M6ODoidGltZXpvbmUiO3M6MjoiOTkiO3M6MTE6ImZpcnN0YWNjZXNzIjtzOjE6IjAiO3M6MTA6Imxhc3RhY2Nlc3MiO3M6MToiMCI7czo5OiJsYXN0bG9naW4iO3M6MToiMCI7czoxMjoiY3VycmVudGxvZ2luIjtzOjE6IjAiO3M6NjoibGFzdGlwIjtzOjk6IjEyNy4wLjAuMSI7czo2OiJzZWNyZXQiO3M6MDoiIjtzOjc6InBpY3R1cmUiO3M6MToiMCI7czozOiJ1cmwiO3M6MDoiIjtzOjExOiJkZXNjcmlwdGlvbiI7TjtzOjE3OiJkZXNjcmlwdGlvbmZvcm1hdCI7czoxOiIxIjtzOjEwOiJtYWlsZm9ybWF0IjtzOjE6IjEiO3M6MTA6Im1haWxkaWdlc3QiO3M6MToiMCI7czoxMToibWFpbGRpc3BsYXkiO3M6MToiMiI7czoxMzoiYXV0b3N1YnNjcmliZSI7czoxOiIxIjtzOjExOiJ0cmFja2ZvcnVtcyI7czoxOiIwIjtzOjExOiJ0aW1lY3JlYXRlZCI7czoxMDoiMTQxMzg2MjQ2OSI7czoxMjoidGltZW1vZGlmaWVkIjtzOjEwOiIxNDEzODYyNDY5IjtzOjEyOiJ0cnVzdGJpdG1hc2siO3M6MToiMCI7czo4OiJpbWFnZWFsdCI7TjtzOjE2OiJsYXN0bmFtZXBob25ldGljIjtOO3M6MTc6ImZpcnN0bmFtZXBob25ldGljIjtOO3M6MTA6Im1pZGRsZW5hbWUiO047czoxMzoiYWx0ZXJuYXRlbmFtZSI7Tjt9','',2,1413862469),(2,'Tzo4OiJzdGRDbGFzcyI6NTM6e3M6MjoiaWQiO3M6MToiNCI7czo0OiJhdXRoIjtzOjc6ImNsaWF1dGgiO3M6OToiY29uZmlybWVkIjtzOjE6IjEiO3M6MTI6InBvbGljeWFncmVlZCI7czoxOiIwIjtzOjc6ImRlbGV0ZWQiO3M6MToiMCI7czo5OiJzdXNwZW5kZWQiO3M6MToiMCI7czoxMDoibW5ldGhvc3RpZCI7czoxOiIxIjtzOjg6InVzZXJuYW1lIjtzOjE5OiJjbGlzdW5uZXRAZ21haWwuY29tIjtzOjg6InBhc3N3b3JkIjtzOjEwOiJub3QgY2FjaGVkIjtzOjg6ImlkbnVtYmVyIjtzOjA6IiI7czo5OiJmaXJzdG5hbWUiO3M6MDoiIjtzOjg6Imxhc3RuYW1lIjtzOjA6IiI7czo1OiJlbWFpbCI7czowOiIiO3M6OToiZW1haWxzdG9wIjtzOjE6IjAiO3M6MzoiaWNxIjtzOjA6IiI7czo1OiJza3lwZSI7czowOiIiO3M6NToieWFob28iO3M6MDoiIjtzOjM6ImFpbSI7czowOiIiO3M6MzoibXNuIjtzOjA6IiI7czo2OiJwaG9uZTEiO3M6MDoiIjtzOjY6InBob25lMiI7czowOiIiO3M6MTE6Imluc3RpdHV0aW9uIjtzOjA6IiI7czoxMDoiZGVwYXJ0bWVudCI7czowOiIiO3M6NzoiYWRkcmVzcyI7czowOiIiO3M6NDoiY2l0eSI7czowOiIiO3M6NzoiY291bnRyeSI7czowOiIiO3M6NDoibGFuZyI7czo1OiJ6aF9jbiI7czoxMjoiY2FsZW5kYXJ0eXBlIjtzOjk6ImdyZWdvcmlhbiI7czo1OiJ0aGVtZSI7czowOiIiO3M6ODoidGltZXpvbmUiO3M6MjoiOTkiO3M6MTE6ImZpcnN0YWNjZXNzIjtzOjE6IjAiO3M6MTA6Imxhc3RhY2Nlc3MiO3M6MToiMCI7czo5OiJsYXN0bG9naW4iO3M6MToiMCI7czoxMjoiY3VycmVudGxvZ2luIjtzOjE6IjAiO3M6NjoibGFzdGlwIjtzOjk6IjEyNy4wLjAuMSI7czo2OiJzZWNyZXQiO3M6MDoiIjtzOjc6InBpY3R1cmUiO3M6MToiMCI7czozOiJ1cmwiO3M6MDoiIjtzOjExOiJkZXNjcmlwdGlvbiI7TjtzOjE3OiJkZXNjcmlwdGlvbmZvcm1hdCI7czoxOiIxIjtzOjEwOiJtYWlsZm9ybWF0IjtzOjE6IjEiO3M6MTA6Im1haWxkaWdlc3QiO3M6MToiMCI7czoxMToibWFpbGRpc3BsYXkiO3M6MToiMiI7czoxMzoiYXV0b3N1YnNjcmliZSI7czoxOiIxIjtzOjExOiJ0cmFja2ZvcnVtcyI7czoxOiIwIjtzOjExOiJ0aW1lY3JlYXRlZCI7czoxMDoiMTQxMzg2MjU2NCI7czoxMjoidGltZW1vZGlmaWVkIjtzOjEwOiIxNDEzODYyNTY0IjtzOjEyOiJ0cnVzdGJpdG1hc2siO3M6MToiMCI7czo4OiJpbWFnZWFsdCI7TjtzOjE2OiJsYXN0bmFtZXBob25ldGljIjtOO3M6MTc6ImZpcnN0bmFtZXBob25ldGljIjtOO3M6MTA6Im1pZGRsZW5hbWUiO047czoxMzoiYWx0ZXJuYXRlbmFtZSI7Tjt9','',3,1413862564);

/*Table structure for table `mdl_events_queue_handlers` */

DROP TABLE IF EXISTS `mdl_events_queue_handlers`;

CREATE TABLE `mdl_events_queue_handlers` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `queuedeventid` bigint(10) NOT NULL,
  `handlerid` bigint(10) NOT NULL,
  `status` bigint(10) DEFAULT NULL,
  `errormessage` longtext COLLATE utf8_unicode_ci,
  `timemodified` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_evenqueuhand_que_ix` (`queuedeventid`),
  KEY `mdl_evenqueuhand_han_ix` (`handlerid`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This is the list of queued handlers for processing. The even';

/*Data for the table `mdl_events_queue_handlers` */

insert  into `mdl_events_queue_handlers`(`id`,`queuedeventid`,`handlerid`,`status`,`errormessage`,`timemodified`) values (1,1,27,0,'',1413862469),(2,2,27,0,'',1413862564);

/*Table structure for table `mdl_external_functions` */

DROP TABLE IF EXISTS `mdl_external_functions`;

CREATE TABLE `mdl_external_functions` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(200) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `classname` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `methodname` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `classpath` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `component` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `capabilities` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_extefunc_nam_uix` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=148 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='list of all external functions';

/*Data for the table `mdl_external_functions` */

insert  into `mdl_external_functions`(`id`,`name`,`classname`,`methodname`,`classpath`,`component`,`capabilities`) values (1,'core_cohort_create_cohorts','core_cohort_external','create_cohorts','cohort/externallib.php','moodle','moodle/cohort:manage'),(2,'core_cohort_delete_cohorts','core_cohort_external','delete_cohorts','cohort/externallib.php','moodle','moodle/cohort:manage'),(3,'core_cohort_get_cohorts','core_cohort_external','get_cohorts','cohort/externallib.php','moodle','moodle/cohort:view'),(4,'core_cohort_update_cohorts','core_cohort_external','update_cohorts','cohort/externallib.php','moodle','moodle/cohort:manage'),(5,'core_cohort_add_cohort_members','core_cohort_external','add_cohort_members','cohort/externallib.php','moodle','moodle/cohort:assign'),(6,'core_cohort_delete_cohort_members','core_cohort_external','delete_cohort_members','cohort/externallib.php','moodle','moodle/cohort:assign'),(7,'core_cohort_get_cohort_members','core_cohort_external','get_cohort_members','cohort/externallib.php','moodle','moodle/cohort:view'),(8,'core_grades_get_grades','core_grades_external','get_grades',NULL,'moodle','moodle/grade:view, moodle/grade:viewall'),(9,'core_grades_update_grades','core_grades_external','update_grades',NULL,'moodle',''),(10,'moodle_group_create_groups','core_group_external','create_groups','group/externallib.php','moodle','moodle/course:managegroups'),(11,'core_group_create_groups','core_group_external','create_groups','group/externallib.php','moodle','moodle/course:managegroups'),(12,'moodle_group_get_groups','core_group_external','get_groups','group/externallib.php','moodle','moodle/course:managegroups'),(13,'core_group_get_groups','core_group_external','get_groups','group/externallib.php','moodle','moodle/course:managegroups'),(14,'moodle_group_get_course_groups','core_group_external','get_course_groups','group/externallib.php','moodle','moodle/course:managegroups'),(15,'core_group_get_course_groups','core_group_external','get_course_groups','group/externallib.php','moodle','moodle/course:managegroups'),(16,'moodle_group_delete_groups','core_group_external','delete_groups','group/externallib.php','moodle','moodle/course:managegroups'),(17,'core_group_delete_groups','core_group_external','delete_groups','group/externallib.php','moodle','moodle/course:managegroups'),(18,'moodle_group_get_groupmembers','core_group_external','get_group_members','group/externallib.php','moodle','moodle/course:managegroups'),(19,'core_group_get_group_members','core_group_external','get_group_members','group/externallib.php','moodle','moodle/course:managegroups'),(20,'moodle_group_add_groupmembers','core_group_external','add_group_members','group/externallib.php','moodle','moodle/course:managegroups'),(21,'core_group_add_group_members','core_group_external','add_group_members','group/externallib.php','moodle','moodle/course:managegroups'),(22,'moodle_group_delete_groupmembers','core_group_external','delete_group_members','group/externallib.php','moodle','moodle/course:managegroups'),(23,'core_group_delete_group_members','core_group_external','delete_group_members','group/externallib.php','moodle','moodle/course:managegroups'),(24,'core_group_create_groupings','core_group_external','create_groupings','group/externallib.php','moodle',''),(25,'core_group_update_groupings','core_group_external','update_groupings','group/externallib.php','moodle',''),(26,'core_group_get_groupings','core_group_external','get_groupings','group/externallib.php','moodle',''),(27,'core_group_get_course_groupings','core_group_external','get_course_groupings','group/externallib.php','moodle',''),(28,'core_group_delete_groupings','core_group_external','delete_groupings','group/externallib.php','moodle',''),(29,'core_group_assign_grouping','core_group_external','assign_grouping','group/externallib.php','moodle',''),(30,'core_group_unassign_grouping','core_group_external','unassign_grouping','group/externallib.php','moodle',''),(31,'moodle_file_get_files','core_files_external','get_files','files/externallib.php','moodle',''),(32,'core_files_get_files','core_files_external','get_files','files/externallib.php','moodle',''),(33,'moodle_file_upload','core_files_external','upload','files/externallib.php','moodle',''),(34,'core_files_upload','core_files_external','upload','files/externallib.php','moodle',''),(35,'moodle_user_create_users','core_user_external','create_users','user/externallib.php','moodle','moodle/user:create'),(36,'core_user_create_users','core_user_external','create_users','user/externallib.php','moodle','moodle/user:create'),(37,'core_user_get_users','core_user_external','get_users','user/externallib.php','moodle','moodle/user:viewdetails, moodle/user:viewhiddendetails, moodle/course:useremail, moodle/user:update'),(38,'moodle_user_get_users_by_id','core_user_external','get_users_by_id','user/externallib.php','moodle','moodle/user:viewdetails, moodle/user:viewhiddendetails, moodle/course:useremail, moodle/user:update'),(39,'core_user_get_users_by_field','core_user_external','get_users_by_field','user/externallib.php','moodle','moodle/user:viewdetails, moodle/user:viewhiddendetails, moodle/course:useremail, moodle/user:update'),(40,'core_user_get_users_by_id','core_user_external','get_users_by_id','user/externallib.php','moodle','moodle/user:viewdetails, moodle/user:viewhiddendetails, moodle/course:useremail, moodle/user:update'),(41,'moodle_user_get_users_by_courseid','core_enrol_external','get_enrolled_users','enrol/externallib.php','moodle','moodle/user:viewdetails, moodle/user:viewhiddendetails, moodle/course:useremail, moodle/user:update, moodle/site:accessallgroups'),(42,'moodle_user_get_course_participants_by_id','core_user_external','get_course_user_profiles','user/externallib.php','moodle','moodle/user:viewdetails, moodle/user:viewhiddendetails, moodle/course:useremail, moodle/user:update, moodle/site:accessallgroups'),(43,'core_user_get_course_user_profiles','core_user_external','get_course_user_profiles','user/externallib.php','moodle','moodle/user:viewdetails, moodle/user:viewhiddendetails, moodle/course:useremail, moodle/user:update, moodle/site:accessallgroups'),(44,'moodle_user_delete_users','core_user_external','delete_users','user/externallib.php','moodle','moodle/user:delete'),(45,'core_user_delete_users','core_user_external','delete_users','user/externallib.php','moodle','moodle/user:delete'),(46,'moodle_user_update_users','core_user_external','update_users','user/externallib.php','moodle','moodle/user:update'),(47,'core_user_update_users','core_user_external','update_users','user/externallib.php','moodle','moodle/user:update'),(48,'core_user_add_user_device','core_user_external','add_user_device','user/externallib.php','moodle',''),(49,'core_enrol_get_enrolled_users_with_capability','core_enrol_external','get_enrolled_users_with_capability','enrol/externallib.php','moodle',''),(50,'moodle_enrol_get_enrolled_users','moodle_enrol_external','get_enrolled_users','enrol/externallib.php','moodle','moodle/site:viewparticipants, moodle/course:viewparticipants,\n            moodle/role:review, moodle/site:accessallgroups, moodle/course:enrolreview'),(51,'core_enrol_get_enrolled_users','core_enrol_external','get_enrolled_users','enrol/externallib.php','moodle','moodle/user:viewdetails, moodle/user:viewhiddendetails, moodle/course:useremail, moodle/user:update, moodle/site:accessallgroups'),(52,'moodle_enrol_get_users_courses','core_enrol_external','get_users_courses','enrol/externallib.php','moodle','moodle/course:viewparticipants'),(53,'core_enrol_get_users_courses','core_enrol_external','get_users_courses','enrol/externallib.php','moodle','moodle/course:viewparticipants'),(54,'core_enrol_get_course_enrolment_methods','core_enrol_external','get_course_enrolment_methods','enrol/externallib.php','moodle',''),(55,'moodle_role_assign','core_role_external','assign_roles','enrol/externallib.php','moodle','moodle/role:assign'),(56,'core_role_assign_roles','core_role_external','assign_roles','enrol/externallib.php','moodle','moodle/role:assign'),(57,'moodle_role_unassign','core_role_external','unassign_roles','enrol/externallib.php','moodle','moodle/role:assign'),(58,'core_role_unassign_roles','core_role_external','unassign_roles','enrol/externallib.php','moodle','moodle/role:assign'),(59,'core_course_get_contents','core_course_external','get_course_contents','course/externallib.php','moodle','moodle/course:update,moodle/course:viewhiddencourses'),(60,'moodle_course_get_courses','core_course_external','get_courses','course/externallib.php','moodle','moodle/course:view,moodle/course:update,moodle/course:viewhiddencourses'),(61,'core_course_get_courses','core_course_external','get_courses','course/externallib.php','moodle','moodle/course:view,moodle/course:update,moodle/course:viewhiddencourses'),(62,'moodle_course_create_courses','core_course_external','create_courses','course/externallib.php','moodle','moodle/course:create,moodle/course:visibility'),(63,'core_course_create_courses','core_course_external','create_courses','course/externallib.php','moodle','moodle/course:create,moodle/course:visibility'),(64,'core_course_delete_courses','core_course_external','delete_courses','course/externallib.php','moodle','moodle/course:delete'),(65,'core_course_delete_modules','core_course_external','delete_modules','course/externallib.php','moodle','moodle/course:manageactivities'),(66,'core_course_duplicate_course','core_course_external','duplicate_course','course/externallib.php','moodle','moodle/backup:backupcourse,moodle/restore:restorecourse,moodle/course:create'),(67,'core_course_update_courses','core_course_external','update_courses','course/externallib.php','moodle','moodle/course:update,moodle/course:changecategory,moodle/course:changefullname,moodle/course:changeshortname,moodle/course:changeidnumber,moodle/course:changesummary,moodle/course:visibility'),(68,'core_course_get_categories','core_course_external','get_categories','course/externallib.php','moodle','moodle/category:viewhiddencategories'),(69,'core_course_create_categories','core_course_external','create_categories','course/externallib.php','moodle','moodle/category:manage'),(70,'core_course_update_categories','core_course_external','update_categories','course/externallib.php','moodle','moodle/category:manage'),(71,'core_course_delete_categories','core_course_external','delete_categories','course/externallib.php','moodle','moodle/category:manage'),(72,'core_course_import_course','core_course_external','import_course','course/externallib.php','moodle','moodle/backup:backuptargetimport, moodle/restore:restoretargetimport'),(73,'moodle_message_send_instantmessages','core_message_external','send_instant_messages','message/externallib.php','moodle','moodle/site:sendmessage'),(74,'core_message_send_instant_messages','core_message_external','send_instant_messages','message/externallib.php','moodle','moodle/site:sendmessage'),(75,'core_message_create_contacts','core_message_external','create_contacts','message/externallib.php','moodle',''),(76,'core_message_delete_contacts','core_message_external','delete_contacts','message/externallib.php','moodle',''),(77,'core_message_block_contacts','core_message_external','block_contacts','message/externallib.php','moodle',''),(78,'core_message_unblock_contacts','core_message_external','unblock_contacts','message/externallib.php','moodle',''),(79,'core_message_get_contacts','core_message_external','get_contacts','message/externallib.php','moodle',''),(80,'core_message_search_contacts','core_message_external','search_contacts','message/externallib.php','moodle',''),(81,'moodle_notes_create_notes','core_notes_external','create_notes','notes/externallib.php','moodle','moodle/notes:manage'),(82,'core_notes_create_notes','core_notes_external','create_notes','notes/externallib.php','moodle','moodle/notes:manage'),(83,'core_notes_delete_notes','core_notes_external','delete_notes','notes/externallib.php','moodle','moodle/notes:manage'),(84,'core_notes_get_notes','core_notes_external','get_notes','notes/externallib.php','moodle','moodle/notes:view'),(85,'core_notes_update_notes','core_notes_external','update_notes','notes/externallib.php','moodle','moodle/notes:manage'),(86,'core_grading_get_definitions','core_grading_external','get_definitions',NULL,'moodle',''),(87,'core_grade_get_definitions','core_grade_external','get_definitions','grade/externallib.php','moodle',''),(88,'core_grading_get_gradingform_instances','core_grading_external','get_gradingform_instances',NULL,'moodle',''),(89,'moodle_webservice_get_siteinfo','core_webservice_external','get_site_info','webservice/externallib.php','moodle',''),(90,'core_webservice_get_site_info','core_webservice_external','get_site_info','webservice/externallib.php','moodle',''),(91,'core_get_string','core_external','get_string','lib/external/externallib.php','moodle',''),(92,'core_get_strings','core_external','get_strings','lib/external/externallib.php','moodle',''),(93,'core_get_component_strings','core_external','get_component_strings','lib/external/externallib.php','moodle',''),(94,'core_calendar_delete_calendar_events','core_calendar_external','delete_calendar_events','calendar/externallib.php','moodle','moodle/calendar:manageentries'),(95,'core_calendar_get_calendar_events','core_calendar_external','get_calendar_events','calendar/externallib.php','moodle','moodle/calendar:manageentries'),(96,'core_calendar_create_calendar_events','core_calendar_external','create_calendar_events','calendar/externallib.php','moodle','moodle/calendar:manageentries'),(97,'mod_assign_get_grades','mod_assign_external','get_grades','mod/assign/externallib.php','mod_assign',''),(98,'mod_assign_get_assignments','mod_assign_external','get_assignments','mod/assign/externallib.php','mod_assign',''),(99,'mod_assign_get_submissions','mod_assign_external','get_submissions','mod/assign/externallib.php','mod_assign',''),(100,'mod_assign_get_user_flags','mod_assign_external','get_user_flags','mod/assign/externallib.php','mod_assign',''),(101,'mod_assign_set_user_flags','mod_assign_external','set_user_flags','mod/assign/externallib.php','mod_assign','mod/assign:grade'),(102,'mod_assign_get_user_mappings','mod_assign_external','get_user_mappings','mod/assign/externallib.php','mod_assign',''),(103,'mod_assign_revert_submissions_to_draft','mod_assign_external','revert_submissions_to_draft','mod/assign/externallib.php','mod_assign',''),(104,'mod_assign_lock_submissions','mod_assign_external','lock_submissions','mod/assign/externallib.php','mod_assign',''),(105,'mod_assign_unlock_submissions','mod_assign_external','unlock_submissions','mod/assign/externallib.php','mod_assign',''),(106,'mod_assign_save_submission','mod_assign_external','save_submission','mod/assign/externallib.php','mod_assign',''),(107,'mod_assign_submit_for_grading','mod_assign_external','submit_for_grading','mod/assign/externallib.php','mod_assign',''),(108,'mod_assign_save_grade','mod_assign_external','save_grade','mod/assign/externallib.php','mod_assign',''),(109,'mod_assign_save_grades','mod_assign_external','save_grades','mod/assign/externallib.php','mod_assign',''),(110,'mod_assign_save_user_extensions','mod_assign_external','save_user_extensions','mod/assign/externallib.php','mod_assign',''),(111,'mod_assign_reveal_identities','mod_assign_external','reveal_identities','mod/assign/externallib.php','mod_assign',''),(112,'mod_forum_get_forums_by_courses','mod_forum_external','get_forums_by_courses','mod/forum/externallib.php','mod_forum','mod/forum:viewdiscussion'),(113,'mod_forum_get_forum_discussions','mod_forum_external','get_forum_discussions','mod/forum/externallib.php','mod_forum','mod/forum:viewdiscussion, mod/forum:viewqandawithoutposting'),(114,'mod_forum_get_forum_discussion_posts','mod_forum_external','get_forum_discussion_posts','mod/forum/externallib.php','mod_forum','mod/forum:viewdiscussion, mod/forum:viewqandawithoutposting'),(115,'moodle_enrol_manual_enrol_users','enrol_manual_external','enrol_users','enrol/manual/externallib.php','enrol_manual','enrol/manual:enrol'),(116,'enrol_manual_enrol_users','enrol_manual_external','enrol_users','enrol/manual/externallib.php','enrol_manual','enrol/manual:enrol'),(117,'enrol_self_get_instance_info','enrol_self_external','get_instance_info','enrol/self/externallib.php','enrol_self',''),(118,'message_airnotifier_is_system_configured','message_airnotifier_external','is_system_configured','message/output/airnotifier/externallib.php','message_airnotifier',''),(119,'message_airnotifier_are_notification_preferences_configured','message_airnotifier_external','are_notification_preferences_configured','message/output/airnotifier/externallib.php','message_airnotifier',''),(120,'local_datahub_elis_user_create','local_datahub_elis_user_create','user_create','local/datahub/ws/elis/user_create.class.php','local_datahub',''),(121,'local_datahub_elis_user_update','local_datahub_elis_user_update','user_update','local/datahub/ws/elis/user_update.class.php','local_datahub',''),(122,'local_datahub_elis_user_update_identifiers','local_datahub_elis_user_update_identifiers','user_update_identifiers','local/datahub/ws/elis/user_update_identifiers.class.php','local_datahub',''),(123,'local_datahub_elis_user_delete','local_datahub_elis_user_delete','user_delete','local/datahub/ws/elis/user_delete.class.php','local_datahub',''),(124,'local_datahub_elis_program_enrolment_create','local_datahub_elis_program_enrolment_create','program_enrolment_create','local/datahub/ws/elis/program_enrolment_create.class.php','local_datahub',''),(125,'local_datahub_elis_program_enrolment_delete','local_datahub_elis_program_enrolment_delete','program_enrolment_delete','local/datahub/ws/elis/program_enrolment_delete.class.php','local_datahub',''),(126,'local_datahub_elis_track_enrolment_create','local_datahub_elis_track_enrolment_create','track_enrolment_create','local/datahub/ws/elis/track_enrolment_create.class.php','local_datahub',''),(127,'local_datahub_elis_track_enrolment_delete','local_datahub_elis_track_enrolment_delete','track_enrolment_delete','local/datahub/ws/elis/track_enrolment_delete.class.php','local_datahub',''),(128,'local_datahub_elis_class_enrolment_create','local_datahub_elis_class_enrolment_create','class_enrolment_create','local/datahub/ws/elis/class_enrolment_create.class.php','local_datahub',''),(129,'local_datahub_elis_class_enrolment_update','local_datahub_elis_class_enrolment_update','class_enrolment_update','local/datahub/ws/elis/class_enrolment_update.class.php','local_datahub',''),(130,'local_datahub_elis_class_enrolment_delete','local_datahub_elis_class_enrolment_delete','class_enrolment_delete','local/datahub/ws/elis/class_enrolment_delete.class.php','local_datahub',''),(131,'local_datahub_elis_userset_enrolment_create','local_datahub_elis_userset_enrolment_create','userset_enrolment_create','local/datahub/ws/elis/userset_enrolment_create.class.php','local_datahub',''),(132,'local_datahub_elis_userset_enrolment_delete','local_datahub_elis_userset_enrolment_delete','userset_enrolment_delete','local/datahub/ws/elis/userset_enrolment_delete.class.php','local_datahub',''),(133,'local_datahub_elis_course_create','local_datahub_elis_course_create','course_create','local/datahub/ws/elis/course_create.class.php','local_datahub',''),(134,'local_datahub_elis_course_update','local_datahub_elis_course_update','course_update','local/datahub/ws/elis/course_update.class.php','local_datahub',''),(135,'local_datahub_elis_course_delete','local_datahub_elis_course_delete','course_delete','local/datahub/ws/elis/course_delete.class.php','local_datahub',''),(136,'local_datahub_elis_class_create','local_datahub_elis_class_create','class_create','local/datahub/ws/elis/class_create.class.php','local_datahub',''),(137,'local_datahub_elis_class_update','local_datahub_elis_class_update','class_update','local/datahub/ws/elis/class_update.class.php','local_datahub',''),(138,'local_datahub_elis_class_delete','local_datahub_elis_class_delete','class_delete','local/datahub/ws/elis/class_delete.class.php','local_datahub',''),(139,'local_datahub_elis_program_create','local_datahub_elis_program_create','program_create','local/datahub/ws/elis/program_create.class.php','local_datahub',''),(140,'local_datahub_elis_program_update','local_datahub_elis_program_update','program_update','local/datahub/ws/elis/program_update.class.php','local_datahub',''),(141,'local_datahub_elis_program_delete','local_datahub_elis_program_delete','program_delete','local/datahub/ws/elis/program_delete.class.php','local_datahub',''),(142,'local_datahub_elis_track_create','local_datahub_elis_track_create','track_create','local/datahub/ws/elis/track_create.class.php','local_datahub',''),(143,'local_datahub_elis_track_update','local_datahub_elis_track_update','track_update','local/datahub/ws/elis/track_update.class.php','local_datahub',''),(144,'local_datahub_elis_track_delete','local_datahub_elis_track_delete','track_delete','local/datahub/ws/elis/track_delete.class.php','local_datahub',''),(145,'local_datahub_elis_userset_create','local_datahub_elis_userset_create','userset_create','local/datahub/ws/elis/userset_create.class.php','local_datahub',''),(146,'local_datahub_elis_userset_update','local_datahub_elis_userset_update','userset_update','local/datahub/ws/elis/userset_update.class.php','local_datahub',''),(147,'local_datahub_elis_userset_delete','local_datahub_elis_userset_delete','userset_delete','local/datahub/ws/elis/userset_delete.class.php','local_datahub','');

/*Table structure for table `mdl_external_services` */

DROP TABLE IF EXISTS `mdl_external_services`;

CREATE TABLE `mdl_external_services` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(200) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `enabled` tinyint(1) NOT NULL,
  `requiredcapability` varchar(150) COLLATE utf8_unicode_ci DEFAULT NULL,
  `restrictedusers` tinyint(1) NOT NULL,
  `component` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `timecreated` bigint(10) NOT NULL,
  `timemodified` bigint(10) DEFAULT NULL,
  `shortname` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `downloadfiles` tinyint(1) NOT NULL DEFAULT '0',
  `uploadfiles` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_exteserv_nam_uix` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='built in and custom external services';

/*Data for the table `mdl_external_services` */

insert  into `mdl_external_services`(`id`,`name`,`enabled`,`requiredcapability`,`restrictedusers`,`component`,`timecreated`,`timemodified`,`shortname`,`downloadfiles`,`uploadfiles`) values (1,'Moodle mobile web service',0,NULL,0,'moodle',1413856772,NULL,'moodle_mobile_app',1,1),(2,'RLDH Webservices',1,NULL,0,'local_datahub',1413861482,NULL,'rldh_webservices',0,0);

/*Table structure for table `mdl_external_services_functions` */

DROP TABLE IF EXISTS `mdl_external_services_functions`;

CREATE TABLE `mdl_external_services_functions` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `externalserviceid` bigint(10) NOT NULL,
  `functionname` varchar(200) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_exteservfunc_ext_ix` (`externalserviceid`)
) ENGINE=InnoDB AUTO_INCREMENT=70 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='lists functions available in each service group';

/*Data for the table `mdl_external_services_functions` */

insert  into `mdl_external_services_functions`(`id`,`externalserviceid`,`functionname`) values (1,1,'moodle_enrol_get_users_courses'),(2,1,'moodle_enrol_get_enrolled_users'),(3,1,'moodle_user_get_users_by_id'),(4,1,'moodle_webservice_get_siteinfo'),(5,1,'moodle_notes_create_notes'),(6,1,'moodle_user_get_course_participants_by_id'),(7,1,'moodle_user_get_users_by_courseid'),(8,1,'moodle_message_send_instantmessages'),(9,1,'core_course_get_contents'),(10,1,'core_get_component_strings'),(11,1,'core_user_add_user_device'),(12,1,'core_calendar_get_calendar_events'),(13,1,'core_enrol_get_users_courses'),(14,1,'core_enrol_get_enrolled_users'),(15,1,'core_user_get_users_by_id'),(16,1,'core_webservice_get_site_info'),(17,1,'core_notes_create_notes'),(18,1,'core_user_get_course_user_profiles'),(19,1,'core_enrol_get_enrolled_users'),(20,1,'core_message_send_instant_messages'),(21,1,'mod_assign_get_grades'),(22,1,'mod_assign_get_assignments'),(23,1,'mod_assign_get_submissions'),(24,1,'mod_assign_get_user_flags'),(25,1,'mod_assign_set_user_flags'),(26,1,'mod_assign_get_user_mappings'),(27,1,'mod_assign_revert_submissions_to_draft'),(28,1,'mod_assign_lock_submissions'),(29,1,'mod_assign_unlock_submissions'),(30,1,'mod_assign_save_submission'),(31,1,'mod_assign_submit_for_grading'),(32,1,'mod_assign_save_grade'),(33,1,'mod_assign_save_user_extensions'),(34,1,'mod_assign_reveal_identities'),(35,1,'message_airnotifier_is_system_configured'),(36,1,'message_airnotifier_are_notification_preferences_configured'),(37,1,'core_grades_get_grades'),(38,1,'core_grades_update_grades'),(39,1,'mod_forum_get_forums_by_courses'),(40,1,'mod_forum_get_forum_discussions'),(41,1,'mod_forum_get_forum_discussion_posts'),(42,2,'local_datahub_elis_user_create'),(43,2,'local_datahub_elis_user_update'),(44,2,'local_datahub_elis_user_update_identifiers'),(45,2,'local_datahub_elis_user_delete'),(46,2,'local_datahub_elis_program_enrolment_create'),(47,2,'local_datahub_elis_program_enrolment_delete'),(48,2,'local_datahub_elis_track_enrolment_create'),(49,2,'local_datahub_elis_track_enrolment_delete'),(50,2,'local_datahub_elis_class_enrolment_create'),(51,2,'local_datahub_elis_class_enrolment_update'),(52,2,'local_datahub_elis_class_enrolment_delete'),(53,2,'local_datahub_elis_userset_enrolment_create'),(54,2,'local_datahub_elis_userset_enrolment_delete'),(55,2,'local_datahub_elis_course_create'),(56,2,'local_datahub_elis_course_update'),(57,2,'local_datahub_elis_course_delete'),(58,2,'local_datahub_elis_class_create'),(59,2,'local_datahub_elis_class_update'),(60,2,'local_datahub_elis_class_delete'),(61,2,'local_datahub_elis_program_create'),(62,2,'local_datahub_elis_program_update'),(63,2,'local_datahub_elis_program_delete'),(64,2,'local_datahub_elis_track_create'),(65,2,'local_datahub_elis_track_update'),(66,2,'local_datahub_elis_track_delete'),(67,2,'local_datahub_elis_userset_create'),(68,2,'local_datahub_elis_userset_update'),(69,2,'local_datahub_elis_userset_delete');

/*Table structure for table `mdl_external_services_users` */

DROP TABLE IF EXISTS `mdl_external_services_users`;

CREATE TABLE `mdl_external_services_users` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `externalserviceid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `iprestriction` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `validuntil` bigint(10) DEFAULT NULL,
  `timecreated` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_exteservuser_ext_ix` (`externalserviceid`),
  KEY `mdl_exteservuser_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='users allowed to use services with restricted users flag';

/*Data for the table `mdl_external_services_users` */

/*Table structure for table `mdl_external_tokens` */

DROP TABLE IF EXISTS `mdl_external_tokens`;

CREATE TABLE `mdl_external_tokens` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `token` varchar(128) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `tokentype` smallint(4) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `externalserviceid` bigint(10) NOT NULL,
  `sid` varchar(128) COLLATE utf8_unicode_ci DEFAULT NULL,
  `contextid` bigint(10) NOT NULL,
  `creatorid` bigint(10) NOT NULL DEFAULT '1',
  `iprestriction` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `validuntil` bigint(10) DEFAULT NULL,
  `timecreated` bigint(10) NOT NULL,
  `lastaccess` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_extetoke_use_ix` (`userid`),
  KEY `mdl_extetoke_ext_ix` (`externalserviceid`),
  KEY `mdl_extetoke_con_ix` (`contextid`),
  KEY `mdl_extetoke_cre_ix` (`creatorid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Security tokens for accessing of external services';

/*Data for the table `mdl_external_tokens` */

/*Table structure for table `mdl_feedback` */

DROP TABLE IF EXISTS `mdl_feedback`;

CREATE TABLE `mdl_feedback` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci NOT NULL,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `anonymous` tinyint(1) NOT NULL DEFAULT '1',
  `email_notification` tinyint(1) NOT NULL DEFAULT '1',
  `multiple_submit` tinyint(1) NOT NULL DEFAULT '1',
  `autonumbering` tinyint(1) NOT NULL DEFAULT '1',
  `site_after_submit` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `page_after_submit` longtext COLLATE utf8_unicode_ci NOT NULL,
  `page_after_submitformat` tinyint(2) NOT NULL DEFAULT '0',
  `publish_stats` tinyint(1) NOT NULL DEFAULT '0',
  `timeopen` bigint(10) NOT NULL DEFAULT '0',
  `timeclose` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `completionsubmit` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_feed_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='all feedbacks';

/*Data for the table `mdl_feedback` */

/*Table structure for table `mdl_feedback_completed` */

DROP TABLE IF EXISTS `mdl_feedback_completed`;

CREATE TABLE `mdl_feedback_completed` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `feedback` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `random_response` bigint(10) NOT NULL DEFAULT '0',
  `anonymous_response` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_feedcomp_use_ix` (`userid`),
  KEY `mdl_feedcomp_fee_ix` (`feedback`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='filled out feedback';

/*Data for the table `mdl_feedback_completed` */

/*Table structure for table `mdl_feedback_completedtmp` */

DROP TABLE IF EXISTS `mdl_feedback_completedtmp`;

CREATE TABLE `mdl_feedback_completedtmp` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `feedback` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `guestid` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `random_response` bigint(10) NOT NULL DEFAULT '0',
  `anonymous_response` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_feedcomp_use2_ix` (`userid`),
  KEY `mdl_feedcomp_fee2_ix` (`feedback`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='filled out feedback';

/*Data for the table `mdl_feedback_completedtmp` */

/*Table structure for table `mdl_feedback_item` */

DROP TABLE IF EXISTS `mdl_feedback_item`;

CREATE TABLE `mdl_feedback_item` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `feedback` bigint(10) NOT NULL DEFAULT '0',
  `template` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `label` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `presentation` longtext COLLATE utf8_unicode_ci NOT NULL,
  `typ` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `hasvalue` tinyint(1) NOT NULL DEFAULT '0',
  `position` smallint(3) NOT NULL DEFAULT '0',
  `required` tinyint(1) NOT NULL DEFAULT '0',
  `dependitem` bigint(10) NOT NULL DEFAULT '0',
  `dependvalue` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `options` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_feeditem_fee_ix` (`feedback`),
  KEY `mdl_feeditem_tem_ix` (`template`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='feedback_items';

/*Data for the table `mdl_feedback_item` */

/*Table structure for table `mdl_feedback_sitecourse_map` */

DROP TABLE IF EXISTS `mdl_feedback_sitecourse_map`;

CREATE TABLE `mdl_feedback_sitecourse_map` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `feedbackid` bigint(10) NOT NULL DEFAULT '0',
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_feedsitemap_cou_ix` (`courseid`),
  KEY `mdl_feedsitemap_fee_ix` (`feedbackid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='feedback sitecourse map';

/*Data for the table `mdl_feedback_sitecourse_map` */

/*Table structure for table `mdl_feedback_template` */

DROP TABLE IF EXISTS `mdl_feedback_template`;

CREATE TABLE `mdl_feedback_template` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `ispublic` tinyint(1) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_feedtemp_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='templates of feedbackstructures';

/*Data for the table `mdl_feedback_template` */

/*Table structure for table `mdl_feedback_tracking` */

DROP TABLE IF EXISTS `mdl_feedback_tracking`;

CREATE TABLE `mdl_feedback_tracking` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `feedback` bigint(10) NOT NULL DEFAULT '0',
  `completed` bigint(10) NOT NULL DEFAULT '0',
  `tmp_completed` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_feedtrac_use_ix` (`userid`),
  KEY `mdl_feedtrac_fee_ix` (`feedback`),
  KEY `mdl_feedtrac_com_ix` (`completed`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='feedback trackingdata';

/*Data for the table `mdl_feedback_tracking` */

/*Table structure for table `mdl_feedback_value` */

DROP TABLE IF EXISTS `mdl_feedback_value`;

CREATE TABLE `mdl_feedback_value` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course_id` bigint(10) NOT NULL DEFAULT '0',
  `item` bigint(10) NOT NULL DEFAULT '0',
  `completed` bigint(10) NOT NULL DEFAULT '0',
  `tmp_completed` bigint(10) NOT NULL DEFAULT '0',
  `value` longtext COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_feedvalu_cou_ix` (`course_id`),
  KEY `mdl_feedvalu_ite_ix` (`item`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='values of the completeds';

/*Data for the table `mdl_feedback_value` */

/*Table structure for table `mdl_feedback_valuetmp` */

DROP TABLE IF EXISTS `mdl_feedback_valuetmp`;

CREATE TABLE `mdl_feedback_valuetmp` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course_id` bigint(10) NOT NULL DEFAULT '0',
  `item` bigint(10) NOT NULL DEFAULT '0',
  `completed` bigint(10) NOT NULL DEFAULT '0',
  `tmp_completed` bigint(10) NOT NULL DEFAULT '0',
  `value` longtext COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_feedvalu_cou2_ix` (`course_id`),
  KEY `mdl_feedvalu_ite2_ix` (`item`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='values of the completedstmp';

/*Data for the table `mdl_feedback_valuetmp` */

/*Table structure for table `mdl_files` */

DROP TABLE IF EXISTS `mdl_files`;

CREATE TABLE `mdl_files` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `contenthash` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `pathnamehash` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `contextid` bigint(10) NOT NULL,
  `component` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `filearea` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `itemid` bigint(10) NOT NULL,
  `filepath` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `filename` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `userid` bigint(10) DEFAULT NULL,
  `filesize` bigint(10) NOT NULL,
  `mimetype` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `status` bigint(10) NOT NULL DEFAULT '0',
  `source` longtext COLLATE utf8_unicode_ci,
  `author` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `license` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `timecreated` bigint(10) NOT NULL,
  `timemodified` bigint(10) NOT NULL,
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  `referencefileid` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_file_pat_uix` (`pathnamehash`),
  KEY `mdl_file_comfilconite_ix` (`component`,`filearea`,`contextid`,`itemid`),
  KEY `mdl_file_con_ix` (`contenthash`),
  KEY `mdl_file_con2_ix` (`contextid`),
  KEY `mdl_file_use_ix` (`userid`),
  KEY `mdl_file_ref_ix` (`referencefileid`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='description of files, content is stored in sha1 file pool';

/*Data for the table `mdl_files` */

insert  into `mdl_files`(`id`,`contenthash`,`pathnamehash`,`contextid`,`component`,`filearea`,`itemid`,`filepath`,`filename`,`userid`,`filesize`,`mimetype`,`status`,`source`,`author`,`license`,`timecreated`,`timemodified`,`sortorder`,`referencefileid`) values (1,'41cfeee5884a43a4650a851f4f85e7b28316fcc9','a48e186a2cc853a9e94e9305f4e9bc086391212d',1,'theme_more','backgroundimage',0,'/','background.jpg',2,4451,'image/jpeg',0,NULL,NULL,NULL,1413857061,1413857061,0,NULL),(2,'da39a3ee5e6b4b0d3255bfef95601890afd80709','d1da7ab1bb9c08a926037367bf8ce9a838034ead',1,'theme_more','backgroundimage',0,'/','.',2,0,NULL,0,NULL,NULL,NULL,1413857061,1413857061,0,NULL),(3,'fb262df98d67c4e2a5c9802403821e00cf2992af','508e674d49c30d4fde325fe6c7f6fd3d56b247e1',1,'assignfeedback_editpdf','stamps',0,'/','smile.png',2,1600,'image/png',0,NULL,NULL,NULL,1413857065,1413857065,0,NULL),(4,'da39a3ee5e6b4b0d3255bfef95601890afd80709','70b7cdade7b4e27d4e83f0cdaad10d6a3c0cccb5',1,'assignfeedback_editpdf','stamps',0,'/','.',2,0,NULL,0,NULL,NULL,NULL,1413857065,1413857065,0,NULL),(5,'a4f146f120e7e00d21291b924e26aaabe9f4297a','68317eab56c67d32aeaee5acf509a0c4aa828b6b',1,'assignfeedback_editpdf','stamps',0,'/','sad.png',2,1702,'image/png',0,NULL,NULL,NULL,1413857065,1413857065,0,NULL),(6,'33957e31ba9c763a74638b825f0a9154acf475e1','695a55ff780e61c9e59428aa425430b0d6bde53b',1,'assignfeedback_editpdf','stamps',0,'/','tick.png',2,1187,'image/png',0,NULL,NULL,NULL,1413857065,1413857065,0,NULL),(7,'d613d55f37bb76d38d4ffb4b7b83e6c694778c30','373e63af262a9b8466ba8632551520be793c37ff',1,'assignfeedback_editpdf','stamps',0,'/','cross.png',2,1230,'image/png',0,NULL,NULL,NULL,1413857065,1413857065,0,NULL);

/*Table structure for table `mdl_files_reference` */

DROP TABLE IF EXISTS `mdl_files_reference`;

CREATE TABLE `mdl_files_reference` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `repositoryid` bigint(10) NOT NULL,
  `lastsync` bigint(10) DEFAULT NULL,
  `reference` longtext COLLATE utf8_unicode_ci,
  `referencehash` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_filerefe_repref_uix` (`repositoryid`,`referencehash`),
  KEY `mdl_filerefe_rep_ix` (`repositoryid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Store files references';

/*Data for the table `mdl_files_reference` */

/*Table structure for table `mdl_filter_active` */

DROP TABLE IF EXISTS `mdl_filter_active`;

CREATE TABLE `mdl_filter_active` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `filter` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `contextid` bigint(10) NOT NULL,
  `active` smallint(4) NOT NULL,
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_filtacti_confil_uix` (`contextid`,`filter`),
  KEY `mdl_filtacti_con_ix` (`contextid`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores information about which filters are active in which c';

/*Data for the table `mdl_filter_active` */

insert  into `mdl_filter_active`(`id`,`filter`,`contextid`,`active`,`sortorder`) values (1,'activitynames',1,1,2),(2,'mathjaxloader',1,1,1),(3,'mediaplugin',1,1,3);

/*Table structure for table `mdl_filter_config` */

DROP TABLE IF EXISTS `mdl_filter_config`;

CREATE TABLE `mdl_filter_config` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `filter` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `contextid` bigint(10) NOT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_filtconf_confilnam_uix` (`contextid`,`filter`,`name`),
  KEY `mdl_filtconf_con_ix` (`contextid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores per-context configuration settings for filters which ';

/*Data for the table `mdl_filter_config` */

/*Table structure for table `mdl_folder` */

DROP TABLE IF EXISTS `mdl_folder`;

CREATE TABLE `mdl_folder` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `revision` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `display` smallint(4) NOT NULL DEFAULT '0',
  `showexpanded` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `mdl_fold_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='each record is one folder resource';

/*Data for the table `mdl_folder` */

/*Table structure for table `mdl_forum` */

DROP TABLE IF EXISTS `mdl_forum`;

CREATE TABLE `mdl_forum` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `type` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'general',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci NOT NULL,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `assessed` bigint(10) NOT NULL DEFAULT '0',
  `assesstimestart` bigint(10) NOT NULL DEFAULT '0',
  `assesstimefinish` bigint(10) NOT NULL DEFAULT '0',
  `scale` bigint(10) NOT NULL DEFAULT '0',
  `maxbytes` bigint(10) NOT NULL DEFAULT '0',
  `maxattachments` bigint(10) NOT NULL DEFAULT '1',
  `forcesubscribe` tinyint(1) NOT NULL DEFAULT '0',
  `trackingtype` tinyint(2) NOT NULL DEFAULT '1',
  `rsstype` tinyint(2) NOT NULL DEFAULT '0',
  `rssarticles` tinyint(2) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `warnafter` bigint(10) NOT NULL DEFAULT '0',
  `blockafter` bigint(10) NOT NULL DEFAULT '0',
  `blockperiod` bigint(10) NOT NULL DEFAULT '0',
  `completiondiscussions` int(9) NOT NULL DEFAULT '0',
  `completionreplies` int(9) NOT NULL DEFAULT '0',
  `completionposts` int(9) NOT NULL DEFAULT '0',
  `displaywordcount` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_foru_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Forums contain and structure discussion';

/*Data for the table `mdl_forum` */

/*Table structure for table `mdl_forum_digests` */

DROP TABLE IF EXISTS `mdl_forum_digests`;

CREATE TABLE `mdl_forum_digests` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL,
  `forum` bigint(10) NOT NULL,
  `maildigest` tinyint(1) NOT NULL DEFAULT '-1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_forudige_forusemai_uix` (`forum`,`userid`,`maildigest`),
  KEY `mdl_forudige_use_ix` (`userid`),
  KEY `mdl_forudige_for_ix` (`forum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Keeps track of user mail delivery preferences for each forum';

/*Data for the table `mdl_forum_digests` */

/*Table structure for table `mdl_forum_discussions` */

DROP TABLE IF EXISTS `mdl_forum_discussions`;

CREATE TABLE `mdl_forum_discussions` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `forum` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `firstpost` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `groupid` bigint(10) NOT NULL DEFAULT '-1',
  `assessed` tinyint(1) NOT NULL DEFAULT '1',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `usermodified` bigint(10) NOT NULL DEFAULT '0',
  `timestart` bigint(10) NOT NULL DEFAULT '0',
  `timeend` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_forudisc_use_ix` (`userid`),
  KEY `mdl_forudisc_for_ix` (`forum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Forums are composed of discussions';

/*Data for the table `mdl_forum_discussions` */

/*Table structure for table `mdl_forum_posts` */

DROP TABLE IF EXISTS `mdl_forum_posts`;

CREATE TABLE `mdl_forum_posts` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `discussion` bigint(10) NOT NULL DEFAULT '0',
  `parent` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `created` bigint(10) NOT NULL DEFAULT '0',
  `modified` bigint(10) NOT NULL DEFAULT '0',
  `mailed` tinyint(2) NOT NULL DEFAULT '0',
  `subject` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `message` longtext COLLATE utf8_unicode_ci NOT NULL,
  `messageformat` tinyint(2) NOT NULL DEFAULT '0',
  `messagetrust` tinyint(2) NOT NULL DEFAULT '0',
  `attachment` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `totalscore` smallint(4) NOT NULL DEFAULT '0',
  `mailnow` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_forupost_use_ix` (`userid`),
  KEY `mdl_forupost_cre_ix` (`created`),
  KEY `mdl_forupost_mai_ix` (`mailed`),
  KEY `mdl_forupost_dis_ix` (`discussion`),
  KEY `mdl_forupost_par_ix` (`parent`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='All posts are stored in this table';

/*Data for the table `mdl_forum_posts` */

/*Table structure for table `mdl_forum_queue` */

DROP TABLE IF EXISTS `mdl_forum_queue`;

CREATE TABLE `mdl_forum_queue` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `discussionid` bigint(10) NOT NULL DEFAULT '0',
  `postid` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_foruqueu_use_ix` (`userid`),
  KEY `mdl_foruqueu_dis_ix` (`discussionid`),
  KEY `mdl_foruqueu_pos_ix` (`postid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='For keeping track of posts that will be mailed in digest for';

/*Data for the table `mdl_forum_queue` */

/*Table structure for table `mdl_forum_read` */

DROP TABLE IF EXISTS `mdl_forum_read`;

CREATE TABLE `mdl_forum_read` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `forumid` bigint(10) NOT NULL DEFAULT '0',
  `discussionid` bigint(10) NOT NULL DEFAULT '0',
  `postid` bigint(10) NOT NULL DEFAULT '0',
  `firstread` bigint(10) NOT NULL DEFAULT '0',
  `lastread` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_foruread_usefor_ix` (`userid`,`forumid`),
  KEY `mdl_foruread_usedis_ix` (`userid`,`discussionid`),
  KEY `mdl_foruread_posuse_ix` (`postid`,`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Tracks each users read posts';

/*Data for the table `mdl_forum_read` */

/*Table structure for table `mdl_forum_subscriptions` */

DROP TABLE IF EXISTS `mdl_forum_subscriptions`;

CREATE TABLE `mdl_forum_subscriptions` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `forum` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_forusubs_use_ix` (`userid`),
  KEY `mdl_forusubs_for_ix` (`forum`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Keeps track of who is subscribed to what forum';

/*Data for the table `mdl_forum_subscriptions` */

/*Table structure for table `mdl_forum_track_prefs` */

DROP TABLE IF EXISTS `mdl_forum_track_prefs`;

CREATE TABLE `mdl_forum_track_prefs` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `forumid` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_forutracpref_usefor_ix` (`userid`,`forumid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Tracks each users untracked forums';

/*Data for the table `mdl_forum_track_prefs` */

/*Table structure for table `mdl_glossary` */

DROP TABLE IF EXISTS `mdl_glossary`;

CREATE TABLE `mdl_glossary` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci NOT NULL,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `allowduplicatedentries` tinyint(2) NOT NULL DEFAULT '0',
  `displayformat` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'dictionary',
  `mainglossary` tinyint(2) NOT NULL DEFAULT '0',
  `showspecial` tinyint(2) NOT NULL DEFAULT '1',
  `showalphabet` tinyint(2) NOT NULL DEFAULT '1',
  `showall` tinyint(2) NOT NULL DEFAULT '1',
  `allowcomments` tinyint(2) NOT NULL DEFAULT '0',
  `allowprintview` tinyint(2) NOT NULL DEFAULT '1',
  `usedynalink` tinyint(2) NOT NULL DEFAULT '1',
  `defaultapproval` tinyint(2) NOT NULL DEFAULT '1',
  `approvaldisplayformat` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'default',
  `globalglossary` tinyint(2) NOT NULL DEFAULT '0',
  `entbypage` smallint(3) NOT NULL DEFAULT '10',
  `editalways` tinyint(2) NOT NULL DEFAULT '0',
  `rsstype` tinyint(2) NOT NULL DEFAULT '0',
  `rssarticles` tinyint(2) NOT NULL DEFAULT '0',
  `assessed` bigint(10) NOT NULL DEFAULT '0',
  `assesstimestart` bigint(10) NOT NULL DEFAULT '0',
  `assesstimefinish` bigint(10) NOT NULL DEFAULT '0',
  `scale` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `completionentries` int(9) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_glos_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='all glossaries';

/*Data for the table `mdl_glossary` */

/*Table structure for table `mdl_glossary_alias` */

DROP TABLE IF EXISTS `mdl_glossary_alias`;

CREATE TABLE `mdl_glossary_alias` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `entryid` bigint(10) NOT NULL DEFAULT '0',
  `alias` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_glosalia_ent_ix` (`entryid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='entries alias';

/*Data for the table `mdl_glossary_alias` */

/*Table structure for table `mdl_glossary_categories` */

DROP TABLE IF EXISTS `mdl_glossary_categories`;

CREATE TABLE `mdl_glossary_categories` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `glossaryid` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `usedynalink` tinyint(2) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `mdl_gloscate_glo_ix` (`glossaryid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='all categories for glossary entries';

/*Data for the table `mdl_glossary_categories` */

/*Table structure for table `mdl_glossary_entries` */

DROP TABLE IF EXISTS `mdl_glossary_entries`;

CREATE TABLE `mdl_glossary_entries` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `glossaryid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `concept` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `definition` longtext COLLATE utf8_unicode_ci NOT NULL,
  `definitionformat` tinyint(2) NOT NULL DEFAULT '0',
  `definitiontrust` tinyint(2) NOT NULL DEFAULT '0',
  `attachment` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `teacherentry` tinyint(2) NOT NULL DEFAULT '0',
  `sourceglossaryid` bigint(10) NOT NULL DEFAULT '0',
  `usedynalink` tinyint(2) NOT NULL DEFAULT '1',
  `casesensitive` tinyint(2) NOT NULL DEFAULT '0',
  `fullmatch` tinyint(2) NOT NULL DEFAULT '1',
  `approved` tinyint(2) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `mdl_glosentr_use_ix` (`userid`),
  KEY `mdl_glosentr_con_ix` (`concept`),
  KEY `mdl_glosentr_glo_ix` (`glossaryid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='all glossary entries';

/*Data for the table `mdl_glossary_entries` */

/*Table structure for table `mdl_glossary_entries_categories` */

DROP TABLE IF EXISTS `mdl_glossary_entries_categories`;

CREATE TABLE `mdl_glossary_entries_categories` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `categoryid` bigint(10) NOT NULL DEFAULT '0',
  `entryid` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_glosentrcate_cat_ix` (`categoryid`),
  KEY `mdl_glosentrcate_ent_ix` (`entryid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='categories of each glossary entry';

/*Data for the table `mdl_glossary_entries_categories` */

/*Table structure for table `mdl_glossary_formats` */

DROP TABLE IF EXISTS `mdl_glossary_formats`;

CREATE TABLE `mdl_glossary_formats` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `popupformatname` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `visible` tinyint(2) NOT NULL DEFAULT '1',
  `showgroup` tinyint(2) NOT NULL DEFAULT '1',
  `defaultmode` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `defaulthook` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `sortkey` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `sortorder` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Setting of the display formats';

/*Data for the table `mdl_glossary_formats` */

insert  into `mdl_glossary_formats`(`id`,`name`,`popupformatname`,`visible`,`showgroup`,`defaultmode`,`defaulthook`,`sortkey`,`sortorder`) values (1,'continuous','continuous',1,1,'','','',''),(2,'dictionary','dictionary',1,1,'','','',''),(3,'encyclopedia','encyclopedia',1,1,'','','',''),(4,'entrylist','entrylist',1,1,'','','',''),(5,'faq','faq',1,1,'','','',''),(6,'fullwithauthor','fullwithauthor',1,1,'','','',''),(7,'fullwithoutauthor','fullwithoutauthor',1,1,'','','','');

/*Table structure for table `mdl_grade_categories` */

DROP TABLE IF EXISTS `mdl_grade_categories`;

CREATE TABLE `mdl_grade_categories` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL,
  `parent` bigint(10) DEFAULT NULL,
  `depth` bigint(10) NOT NULL DEFAULT '0',
  `path` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `fullname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `aggregation` bigint(10) NOT NULL DEFAULT '0',
  `keephigh` bigint(10) NOT NULL DEFAULT '0',
  `droplow` bigint(10) NOT NULL DEFAULT '0',
  `aggregateonlygraded` tinyint(1) NOT NULL DEFAULT '0',
  `aggregateoutcomes` tinyint(1) NOT NULL DEFAULT '0',
  `aggregatesubcats` tinyint(1) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL,
  `timemodified` bigint(10) NOT NULL,
  `hidden` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_gradcate_cou_ix` (`courseid`),
  KEY `mdl_gradcate_par_ix` (`parent`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table keeps information about categories, used for grou';

/*Data for the table `mdl_grade_categories` */

/*Table structure for table `mdl_grade_categories_history` */

DROP TABLE IF EXISTS `mdl_grade_categories_history`;

CREATE TABLE `mdl_grade_categories_history` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `action` bigint(10) NOT NULL DEFAULT '0',
  `oldid` bigint(10) NOT NULL,
  `source` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `timemodified` bigint(10) DEFAULT NULL,
  `loggeduser` bigint(10) DEFAULT NULL,
  `courseid` bigint(10) NOT NULL,
  `parent` bigint(10) DEFAULT NULL,
  `depth` bigint(10) NOT NULL DEFAULT '0',
  `path` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `fullname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `aggregation` bigint(10) NOT NULL DEFAULT '0',
  `keephigh` bigint(10) NOT NULL DEFAULT '0',
  `droplow` bigint(10) NOT NULL DEFAULT '0',
  `aggregateonlygraded` tinyint(1) NOT NULL DEFAULT '0',
  `aggregateoutcomes` tinyint(1) NOT NULL DEFAULT '0',
  `aggregatesubcats` tinyint(1) NOT NULL DEFAULT '0',
  `hidden` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_gradcatehist_act_ix` (`action`),
  KEY `mdl_gradcatehist_old_ix` (`oldid`),
  KEY `mdl_gradcatehist_cou_ix` (`courseid`),
  KEY `mdl_gradcatehist_par_ix` (`parent`),
  KEY `mdl_gradcatehist_log_ix` (`loggeduser`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='History of grade_categories';

/*Data for the table `mdl_grade_categories_history` */

/*Table structure for table `mdl_grade_grades` */

DROP TABLE IF EXISTS `mdl_grade_grades`;

CREATE TABLE `mdl_grade_grades` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `itemid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `rawgrade` decimal(10,5) DEFAULT NULL,
  `rawgrademax` decimal(10,5) NOT NULL DEFAULT '100.00000',
  `rawgrademin` decimal(10,5) NOT NULL DEFAULT '0.00000',
  `rawscaleid` bigint(10) DEFAULT NULL,
  `usermodified` bigint(10) DEFAULT NULL,
  `finalgrade` decimal(10,5) DEFAULT NULL,
  `hidden` bigint(10) NOT NULL DEFAULT '0',
  `locked` bigint(10) NOT NULL DEFAULT '0',
  `locktime` bigint(10) NOT NULL DEFAULT '0',
  `exported` bigint(10) NOT NULL DEFAULT '0',
  `overridden` bigint(10) NOT NULL DEFAULT '0',
  `excluded` bigint(10) NOT NULL DEFAULT '0',
  `feedback` longtext COLLATE utf8_unicode_ci,
  `feedbackformat` bigint(10) NOT NULL DEFAULT '0',
  `information` longtext COLLATE utf8_unicode_ci,
  `informationformat` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) DEFAULT NULL,
  `timemodified` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_gradgrad_useite_uix` (`userid`,`itemid`),
  KEY `mdl_gradgrad_locloc_ix` (`locked`,`locktime`),
  KEY `mdl_gradgrad_ite_ix` (`itemid`),
  KEY `mdl_gradgrad_use_ix` (`userid`),
  KEY `mdl_gradgrad_raw_ix` (`rawscaleid`),
  KEY `mdl_gradgrad_use2_ix` (`usermodified`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='grade_grades  This table keeps individual grades for each us';

/*Data for the table `mdl_grade_grades` */

/*Table structure for table `mdl_grade_grades_history` */

DROP TABLE IF EXISTS `mdl_grade_grades_history`;

CREATE TABLE `mdl_grade_grades_history` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `action` bigint(10) NOT NULL DEFAULT '0',
  `oldid` bigint(10) NOT NULL,
  `source` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `timemodified` bigint(10) DEFAULT NULL,
  `loggeduser` bigint(10) DEFAULT NULL,
  `itemid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `rawgrade` decimal(10,5) DEFAULT NULL,
  `rawgrademax` decimal(10,5) NOT NULL DEFAULT '100.00000',
  `rawgrademin` decimal(10,5) NOT NULL DEFAULT '0.00000',
  `rawscaleid` bigint(10) DEFAULT NULL,
  `usermodified` bigint(10) DEFAULT NULL,
  `finalgrade` decimal(10,5) DEFAULT NULL,
  `hidden` bigint(10) NOT NULL DEFAULT '0',
  `locked` bigint(10) NOT NULL DEFAULT '0',
  `locktime` bigint(10) NOT NULL DEFAULT '0',
  `exported` bigint(10) NOT NULL DEFAULT '0',
  `overridden` bigint(10) NOT NULL DEFAULT '0',
  `excluded` bigint(10) NOT NULL DEFAULT '0',
  `feedback` longtext COLLATE utf8_unicode_ci,
  `feedbackformat` bigint(10) NOT NULL DEFAULT '0',
  `information` longtext COLLATE utf8_unicode_ci,
  `informationformat` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_gradgradhist_act_ix` (`action`),
  KEY `mdl_gradgradhist_tim_ix` (`timemodified`),
  KEY `mdl_gradgradhist_old_ix` (`oldid`),
  KEY `mdl_gradgradhist_ite_ix` (`itemid`),
  KEY `mdl_gradgradhist_use_ix` (`userid`),
  KEY `mdl_gradgradhist_raw_ix` (`rawscaleid`),
  KEY `mdl_gradgradhist_use2_ix` (`usermodified`),
  KEY `mdl_gradgradhist_log_ix` (`loggeduser`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='History table';

/*Data for the table `mdl_grade_grades_history` */

/*Table structure for table `mdl_grade_import_newitem` */

DROP TABLE IF EXISTS `mdl_grade_import_newitem`;

CREATE TABLE `mdl_grade_import_newitem` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `itemname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `importcode` bigint(10) NOT NULL,
  `importer` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_gradimponewi_imp_ix` (`importer`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='temporary table for storing new grade_item names from grade ';

/*Data for the table `mdl_grade_import_newitem` */

/*Table structure for table `mdl_grade_import_values` */

DROP TABLE IF EXISTS `mdl_grade_import_values`;

CREATE TABLE `mdl_grade_import_values` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `itemid` bigint(10) DEFAULT NULL,
  `newgradeitem` bigint(10) DEFAULT NULL,
  `userid` bigint(10) NOT NULL,
  `finalgrade` decimal(10,5) DEFAULT NULL,
  `feedback` longtext COLLATE utf8_unicode_ci,
  `importcode` bigint(10) NOT NULL,
  `importer` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_gradimpovalu_ite_ix` (`itemid`),
  KEY `mdl_gradimpovalu_new_ix` (`newgradeitem`),
  KEY `mdl_gradimpovalu_imp_ix` (`importer`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Temporary table for importing grades';

/*Data for the table `mdl_grade_import_values` */

/*Table structure for table `mdl_grade_items` */

DROP TABLE IF EXISTS `mdl_grade_items`;

CREATE TABLE `mdl_grade_items` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) DEFAULT NULL,
  `categoryid` bigint(10) DEFAULT NULL,
  `itemname` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `itemtype` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `itemmodule` varchar(30) COLLATE utf8_unicode_ci DEFAULT NULL,
  `iteminstance` bigint(10) DEFAULT NULL,
  `itemnumber` bigint(10) DEFAULT NULL,
  `iteminfo` longtext COLLATE utf8_unicode_ci,
  `idnumber` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `calculation` longtext COLLATE utf8_unicode_ci,
  `gradetype` smallint(4) NOT NULL DEFAULT '1',
  `grademax` decimal(10,5) NOT NULL DEFAULT '100.00000',
  `grademin` decimal(10,5) NOT NULL DEFAULT '0.00000',
  `scaleid` bigint(10) DEFAULT NULL,
  `outcomeid` bigint(10) DEFAULT NULL,
  `gradepass` decimal(10,5) NOT NULL DEFAULT '0.00000',
  `multfactor` decimal(10,5) NOT NULL DEFAULT '1.00000',
  `plusfactor` decimal(10,5) NOT NULL DEFAULT '0.00000',
  `aggregationcoef` decimal(10,5) NOT NULL DEFAULT '0.00000',
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  `display` bigint(10) NOT NULL DEFAULT '0',
  `decimals` tinyint(1) DEFAULT NULL,
  `hidden` bigint(10) NOT NULL DEFAULT '0',
  `locked` bigint(10) NOT NULL DEFAULT '0',
  `locktime` bigint(10) NOT NULL DEFAULT '0',
  `needsupdate` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) DEFAULT NULL,
  `timemodified` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_graditem_locloc_ix` (`locked`,`locktime`),
  KEY `mdl_graditem_itenee_ix` (`itemtype`,`needsupdate`),
  KEY `mdl_graditem_gra_ix` (`gradetype`),
  KEY `mdl_graditem_idncou_ix` (`idnumber`,`courseid`),
  KEY `mdl_graditem_cou_ix` (`courseid`),
  KEY `mdl_graditem_cat_ix` (`categoryid`),
  KEY `mdl_graditem_sca_ix` (`scaleid`),
  KEY `mdl_graditem_out_ix` (`outcomeid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table keeps information about gradeable items (ie colum';

/*Data for the table `mdl_grade_items` */

/*Table structure for table `mdl_grade_items_history` */

DROP TABLE IF EXISTS `mdl_grade_items_history`;

CREATE TABLE `mdl_grade_items_history` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `action` bigint(10) NOT NULL DEFAULT '0',
  `oldid` bigint(10) NOT NULL,
  `source` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `timemodified` bigint(10) DEFAULT NULL,
  `loggeduser` bigint(10) DEFAULT NULL,
  `courseid` bigint(10) DEFAULT NULL,
  `categoryid` bigint(10) DEFAULT NULL,
  `itemname` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `itemtype` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `itemmodule` varchar(30) COLLATE utf8_unicode_ci DEFAULT NULL,
  `iteminstance` bigint(10) DEFAULT NULL,
  `itemnumber` bigint(10) DEFAULT NULL,
  `iteminfo` longtext COLLATE utf8_unicode_ci,
  `idnumber` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `calculation` longtext COLLATE utf8_unicode_ci,
  `gradetype` smallint(4) NOT NULL DEFAULT '1',
  `grademax` decimal(10,5) NOT NULL DEFAULT '100.00000',
  `grademin` decimal(10,5) NOT NULL DEFAULT '0.00000',
  `scaleid` bigint(10) DEFAULT NULL,
  `outcomeid` bigint(10) DEFAULT NULL,
  `gradepass` decimal(10,5) NOT NULL DEFAULT '0.00000',
  `multfactor` decimal(10,5) NOT NULL DEFAULT '1.00000',
  `plusfactor` decimal(10,5) NOT NULL DEFAULT '0.00000',
  `aggregationcoef` decimal(10,5) NOT NULL DEFAULT '0.00000',
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  `hidden` bigint(10) NOT NULL DEFAULT '0',
  `locked` bigint(10) NOT NULL DEFAULT '0',
  `locktime` bigint(10) NOT NULL DEFAULT '0',
  `needsupdate` bigint(10) NOT NULL DEFAULT '0',
  `display` bigint(10) NOT NULL DEFAULT '0',
  `decimals` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_graditemhist_act_ix` (`action`),
  KEY `mdl_graditemhist_old_ix` (`oldid`),
  KEY `mdl_graditemhist_cou_ix` (`courseid`),
  KEY `mdl_graditemhist_cat_ix` (`categoryid`),
  KEY `mdl_graditemhist_sca_ix` (`scaleid`),
  KEY `mdl_graditemhist_out_ix` (`outcomeid`),
  KEY `mdl_graditemhist_log_ix` (`loggeduser`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='History of grade_items';

/*Data for the table `mdl_grade_items_history` */

/*Table structure for table `mdl_grade_letters` */

DROP TABLE IF EXISTS `mdl_grade_letters`;

CREATE TABLE `mdl_grade_letters` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `contextid` bigint(10) NOT NULL,
  `lowerboundary` decimal(10,5) NOT NULL,
  `letter` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_gradlett_conlowlet_uix` (`contextid`,`lowerboundary`,`letter`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Repository for grade letters, for courses and other moodle e';

/*Data for the table `mdl_grade_letters` */

/*Table structure for table `mdl_grade_outcomes` */

DROP TABLE IF EXISTS `mdl_grade_outcomes`;

CREATE TABLE `mdl_grade_outcomes` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) DEFAULT NULL,
  `shortname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `fullname` longtext COLLATE utf8_unicode_ci NOT NULL,
  `scaleid` bigint(10) DEFAULT NULL,
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` tinyint(2) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) DEFAULT NULL,
  `timemodified` bigint(10) DEFAULT NULL,
  `usermodified` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_gradoutc_cousho_uix` (`courseid`,`shortname`),
  KEY `mdl_gradoutc_cou_ix` (`courseid`),
  KEY `mdl_gradoutc_sca_ix` (`scaleid`),
  KEY `mdl_gradoutc_use_ix` (`usermodified`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table describes the outcomes used in the system. An out';

/*Data for the table `mdl_grade_outcomes` */

/*Table structure for table `mdl_grade_outcomes_courses` */

DROP TABLE IF EXISTS `mdl_grade_outcomes_courses`;

CREATE TABLE `mdl_grade_outcomes_courses` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL,
  `outcomeid` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_gradoutccour_couout_uix` (`courseid`,`outcomeid`),
  KEY `mdl_gradoutccour_cou_ix` (`courseid`),
  KEY `mdl_gradoutccour_out_ix` (`outcomeid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='stores what outcomes are used in what courses.';

/*Data for the table `mdl_grade_outcomes_courses` */

/*Table structure for table `mdl_grade_outcomes_history` */

DROP TABLE IF EXISTS `mdl_grade_outcomes_history`;

CREATE TABLE `mdl_grade_outcomes_history` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `action` bigint(10) NOT NULL DEFAULT '0',
  `oldid` bigint(10) NOT NULL,
  `source` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `timemodified` bigint(10) DEFAULT NULL,
  `loggeduser` bigint(10) DEFAULT NULL,
  `courseid` bigint(10) DEFAULT NULL,
  `shortname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `fullname` longtext COLLATE utf8_unicode_ci NOT NULL,
  `scaleid` bigint(10) DEFAULT NULL,
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` tinyint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_gradoutchist_act_ix` (`action`),
  KEY `mdl_gradoutchist_old_ix` (`oldid`),
  KEY `mdl_gradoutchist_cou_ix` (`courseid`),
  KEY `mdl_gradoutchist_sca_ix` (`scaleid`),
  KEY `mdl_gradoutchist_log_ix` (`loggeduser`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='History table';

/*Data for the table `mdl_grade_outcomes_history` */

/*Table structure for table `mdl_grade_settings` */

DROP TABLE IF EXISTS `mdl_grade_settings`;

CREATE TABLE `mdl_grade_settings` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_gradsett_counam_uix` (`courseid`,`name`),
  KEY `mdl_gradsett_cou_ix` (`courseid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='gradebook settings';

/*Data for the table `mdl_grade_settings` */

/*Table structure for table `mdl_grading_areas` */

DROP TABLE IF EXISTS `mdl_grading_areas`;

CREATE TABLE `mdl_grading_areas` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `contextid` bigint(10) NOT NULL,
  `component` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `areaname` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `activemethod` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_gradarea_concomare_uix` (`contextid`,`component`,`areaname`),
  KEY `mdl_gradarea_con_ix` (`contextid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Identifies gradable areas where advanced grading can happen.';

/*Data for the table `mdl_grading_areas` */

/*Table structure for table `mdl_grading_definitions` */

DROP TABLE IF EXISTS `mdl_grading_definitions`;

CREATE TABLE `mdl_grading_definitions` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `areaid` bigint(10) NOT NULL,
  `method` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` tinyint(2) DEFAULT NULL,
  `status` bigint(10) NOT NULL DEFAULT '0',
  `copiedfromid` bigint(10) DEFAULT NULL,
  `timecreated` bigint(10) NOT NULL,
  `usercreated` bigint(10) NOT NULL,
  `timemodified` bigint(10) NOT NULL,
  `usermodified` bigint(10) NOT NULL,
  `timecopied` bigint(10) DEFAULT '0',
  `options` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_graddefi_aremet_uix` (`areaid`,`method`),
  KEY `mdl_graddefi_are_ix` (`areaid`),
  KEY `mdl_graddefi_use_ix` (`usermodified`),
  KEY `mdl_graddefi_use2_ix` (`usercreated`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Contains the basic information about an advanced grading for';

/*Data for the table `mdl_grading_definitions` */

/*Table structure for table `mdl_grading_instances` */

DROP TABLE IF EXISTS `mdl_grading_instances`;

CREATE TABLE `mdl_grading_instances` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `definitionid` bigint(10) NOT NULL,
  `raterid` bigint(10) NOT NULL,
  `itemid` bigint(10) DEFAULT NULL,
  `rawgrade` decimal(10,5) DEFAULT NULL,
  `status` bigint(10) NOT NULL DEFAULT '0',
  `feedback` longtext COLLATE utf8_unicode_ci,
  `feedbackformat` tinyint(2) DEFAULT NULL,
  `timemodified` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_gradinst_def_ix` (`definitionid`),
  KEY `mdl_gradinst_rat_ix` (`raterid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Grading form instance is an assessment record for one gradab';

/*Data for the table `mdl_grading_instances` */

/*Table structure for table `mdl_gradingform_guide_comments` */

DROP TABLE IF EXISTS `mdl_gradingform_guide_comments`;

CREATE TABLE `mdl_gradingform_guide_comments` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `definitionid` bigint(10) NOT NULL,
  `sortorder` bigint(10) NOT NULL,
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` tinyint(2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_gradguidcomm_def_ix` (`definitionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='frequently used comments used in marking guide';

/*Data for the table `mdl_gradingform_guide_comments` */

/*Table structure for table `mdl_gradingform_guide_criteria` */

DROP TABLE IF EXISTS `mdl_gradingform_guide_criteria`;

CREATE TABLE `mdl_gradingform_guide_criteria` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `definitionid` bigint(10) NOT NULL,
  `sortorder` bigint(10) NOT NULL,
  `shortname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` tinyint(2) DEFAULT NULL,
  `descriptionmarkers` longtext COLLATE utf8_unicode_ci,
  `descriptionmarkersformat` tinyint(2) DEFAULT NULL,
  `maxscore` decimal(10,5) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_gradguidcrit_def_ix` (`definitionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores the rows of the criteria grid.';

/*Data for the table `mdl_gradingform_guide_criteria` */

/*Table structure for table `mdl_gradingform_guide_fillings` */

DROP TABLE IF EXISTS `mdl_gradingform_guide_fillings`;

CREATE TABLE `mdl_gradingform_guide_fillings` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `instanceid` bigint(10) NOT NULL,
  `criterionid` bigint(10) NOT NULL,
  `remark` longtext COLLATE utf8_unicode_ci,
  `remarkformat` tinyint(2) DEFAULT NULL,
  `score` decimal(10,5) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_gradguidfill_inscri_uix` (`instanceid`,`criterionid`),
  KEY `mdl_gradguidfill_ins_ix` (`instanceid`),
  KEY `mdl_gradguidfill_cri_ix` (`criterionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores the data of how the guide is filled by a particular r';

/*Data for the table `mdl_gradingform_guide_fillings` */

/*Table structure for table `mdl_gradingform_rubric_criteria` */

DROP TABLE IF EXISTS `mdl_gradingform_rubric_criteria`;

CREATE TABLE `mdl_gradingform_rubric_criteria` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `definitionid` bigint(10) NOT NULL,
  `sortorder` bigint(10) NOT NULL,
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` tinyint(2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_gradrubrcrit_def_ix` (`definitionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores the rows of the rubric grid.';

/*Data for the table `mdl_gradingform_rubric_criteria` */

/*Table structure for table `mdl_gradingform_rubric_fillings` */

DROP TABLE IF EXISTS `mdl_gradingform_rubric_fillings`;

CREATE TABLE `mdl_gradingform_rubric_fillings` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `instanceid` bigint(10) NOT NULL,
  `criterionid` bigint(10) NOT NULL,
  `levelid` bigint(10) DEFAULT NULL,
  `remark` longtext COLLATE utf8_unicode_ci,
  `remarkformat` tinyint(2) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_gradrubrfill_inscri_uix` (`instanceid`,`criterionid`),
  KEY `mdl_gradrubrfill_lev_ix` (`levelid`),
  KEY `mdl_gradrubrfill_ins_ix` (`instanceid`),
  KEY `mdl_gradrubrfill_cri_ix` (`criterionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores the data of how the rubric is filled by a particular ';

/*Data for the table `mdl_gradingform_rubric_fillings` */

/*Table structure for table `mdl_gradingform_rubric_levels` */

DROP TABLE IF EXISTS `mdl_gradingform_rubric_levels`;

CREATE TABLE `mdl_gradingform_rubric_levels` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `criterionid` bigint(10) NOT NULL,
  `score` decimal(10,5) NOT NULL,
  `definition` longtext COLLATE utf8_unicode_ci,
  `definitionformat` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_gradrubrleve_cri_ix` (`criterionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores the columns of the rubric grid.';

/*Data for the table `mdl_gradingform_rubric_levels` */

/*Table structure for table `mdl_groupings` */

DROP TABLE IF EXISTS `mdl_groupings`;

CREATE TABLE `mdl_groupings` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `idnumber` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` tinyint(2) NOT NULL DEFAULT '0',
  `configdata` longtext COLLATE utf8_unicode_ci,
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_grou_idn2_ix` (`idnumber`),
  KEY `mdl_grou_cou2_ix` (`courseid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='A grouping is a collection of groups. WAS: groups_groupings';

/*Data for the table `mdl_groupings` */

/*Table structure for table `mdl_groupings_groups` */

DROP TABLE IF EXISTS `mdl_groupings_groups`;

CREATE TABLE `mdl_groupings_groups` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `groupingid` bigint(10) NOT NULL DEFAULT '0',
  `groupid` bigint(10) NOT NULL DEFAULT '0',
  `timeadded` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_grougrou_gro_ix` (`groupingid`),
  KEY `mdl_grougrou_gro2_ix` (`groupid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Link a grouping to a group (note, groups can be in multiple ';

/*Data for the table `mdl_groupings_groups` */

/*Table structure for table `mdl_groups` */

DROP TABLE IF EXISTS `mdl_groups`;

CREATE TABLE `mdl_groups` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL,
  `idnumber` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `name` varchar(254) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` tinyint(2) NOT NULL DEFAULT '0',
  `enrolmentkey` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `picture` bigint(10) NOT NULL DEFAULT '0',
  `hidepicture` tinyint(1) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_grou_idn_ix` (`idnumber`),
  KEY `mdl_grou_cou_ix` (`courseid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Each record represents a group.';

/*Data for the table `mdl_groups` */

/*Table structure for table `mdl_groups_members` */

DROP TABLE IF EXISTS `mdl_groups_members`;

CREATE TABLE `mdl_groups_members` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `groupid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `timeadded` bigint(10) NOT NULL DEFAULT '0',
  `component` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `itemid` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_groumemb_gro_ix` (`groupid`),
  KEY `mdl_groumemb_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Link a user to a group.';

/*Data for the table `mdl_groups_members` */

/*Table structure for table `mdl_imscp` */

DROP TABLE IF EXISTS `mdl_imscp`;

CREATE TABLE `mdl_imscp` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `revision` bigint(10) NOT NULL DEFAULT '0',
  `keepold` bigint(10) NOT NULL DEFAULT '-1',
  `structure` longtext COLLATE utf8_unicode_ci,
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_imsc_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='each record is one imscp resource';

/*Data for the table `mdl_imscp` */

/*Table structure for table `mdl_label` */

DROP TABLE IF EXISTS `mdl_label`;

CREATE TABLE `mdl_label` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci NOT NULL,
  `introformat` smallint(4) DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_labe_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines labels';

/*Data for the table `mdl_label` */

/*Table structure for table `mdl_lesson` */

DROP TABLE IF EXISTS `mdl_lesson`;

CREATE TABLE `mdl_lesson` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `practice` smallint(3) NOT NULL DEFAULT '0',
  `modattempts` smallint(3) NOT NULL DEFAULT '0',
  `usepassword` smallint(3) NOT NULL DEFAULT '0',
  `password` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `dependency` bigint(10) NOT NULL DEFAULT '0',
  `conditions` longtext COLLATE utf8_unicode_ci NOT NULL,
  `grade` smallint(3) NOT NULL DEFAULT '0',
  `custom` smallint(3) NOT NULL DEFAULT '0',
  `ongoing` smallint(3) NOT NULL DEFAULT '0',
  `usemaxgrade` smallint(3) NOT NULL DEFAULT '0',
  `maxanswers` smallint(3) NOT NULL DEFAULT '4',
  `maxattempts` smallint(3) NOT NULL DEFAULT '5',
  `review` smallint(3) NOT NULL DEFAULT '0',
  `nextpagedefault` smallint(3) NOT NULL DEFAULT '0',
  `feedback` smallint(3) NOT NULL DEFAULT '1',
  `minquestions` smallint(3) NOT NULL DEFAULT '0',
  `maxpages` smallint(3) NOT NULL DEFAULT '0',
  `timed` smallint(3) NOT NULL DEFAULT '0',
  `maxtime` bigint(10) NOT NULL DEFAULT '0',
  `retake` smallint(3) NOT NULL DEFAULT '1',
  `activitylink` bigint(10) NOT NULL DEFAULT '0',
  `mediafile` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `mediaheight` bigint(10) NOT NULL DEFAULT '100',
  `mediawidth` bigint(10) NOT NULL DEFAULT '650',
  `mediaclose` smallint(3) NOT NULL DEFAULT '0',
  `slideshow` smallint(3) NOT NULL DEFAULT '0',
  `width` bigint(10) NOT NULL DEFAULT '640',
  `height` bigint(10) NOT NULL DEFAULT '480',
  `bgcolor` varchar(7) COLLATE utf8_unicode_ci NOT NULL DEFAULT '#FFFFFF',
  `displayleft` smallint(3) NOT NULL DEFAULT '0',
  `displayleftif` smallint(3) NOT NULL DEFAULT '0',
  `progressbar` smallint(3) NOT NULL DEFAULT '0',
  `highscores` smallint(3) NOT NULL DEFAULT '0',
  `maxhighscores` bigint(10) NOT NULL DEFAULT '0',
  `available` bigint(10) NOT NULL DEFAULT '0',
  `deadline` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_less_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines lesson';

/*Data for the table `mdl_lesson` */

/*Table structure for table `mdl_lesson_answers` */

DROP TABLE IF EXISTS `mdl_lesson_answers`;

CREATE TABLE `mdl_lesson_answers` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `lessonid` bigint(10) NOT NULL DEFAULT '0',
  `pageid` bigint(10) NOT NULL DEFAULT '0',
  `jumpto` bigint(11) NOT NULL DEFAULT '0',
  `grade` smallint(4) NOT NULL DEFAULT '0',
  `score` bigint(10) NOT NULL DEFAULT '0',
  `flags` smallint(3) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `answer` longtext COLLATE utf8_unicode_ci,
  `answerformat` tinyint(2) NOT NULL DEFAULT '0',
  `response` longtext COLLATE utf8_unicode_ci,
  `responseformat` tinyint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_lessansw_les_ix` (`lessonid`),
  KEY `mdl_lessansw_pag_ix` (`pageid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines lesson_answers';

/*Data for the table `mdl_lesson_answers` */

/*Table structure for table `mdl_lesson_attempts` */

DROP TABLE IF EXISTS `mdl_lesson_attempts`;

CREATE TABLE `mdl_lesson_attempts` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `lessonid` bigint(10) NOT NULL DEFAULT '0',
  `pageid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `answerid` bigint(10) NOT NULL DEFAULT '0',
  `retry` smallint(3) NOT NULL DEFAULT '0',
  `correct` bigint(10) NOT NULL DEFAULT '0',
  `useranswer` longtext COLLATE utf8_unicode_ci,
  `timeseen` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_lessatte_use_ix` (`userid`),
  KEY `mdl_lessatte_les_ix` (`lessonid`),
  KEY `mdl_lessatte_pag_ix` (`pageid`),
  KEY `mdl_lessatte_ans_ix` (`answerid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines lesson_attempts';

/*Data for the table `mdl_lesson_attempts` */

/*Table structure for table `mdl_lesson_branch` */

DROP TABLE IF EXISTS `mdl_lesson_branch`;

CREATE TABLE `mdl_lesson_branch` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `lessonid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `pageid` bigint(10) NOT NULL DEFAULT '0',
  `retry` bigint(10) NOT NULL DEFAULT '0',
  `flag` smallint(3) NOT NULL DEFAULT '0',
  `timeseen` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_lessbran_use_ix` (`userid`),
  KEY `mdl_lessbran_les_ix` (`lessonid`),
  KEY `mdl_lessbran_pag_ix` (`pageid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='branches for each lesson/user';

/*Data for the table `mdl_lesson_branch` */

/*Table structure for table `mdl_lesson_grades` */

DROP TABLE IF EXISTS `mdl_lesson_grades`;

CREATE TABLE `mdl_lesson_grades` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `lessonid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `grade` double NOT NULL DEFAULT '0',
  `late` smallint(3) NOT NULL DEFAULT '0',
  `completed` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_lessgrad_use_ix` (`userid`),
  KEY `mdl_lessgrad_les_ix` (`lessonid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines lesson_grades';

/*Data for the table `mdl_lesson_grades` */

/*Table structure for table `mdl_lesson_high_scores` */

DROP TABLE IF EXISTS `mdl_lesson_high_scores`;

CREATE TABLE `mdl_lesson_high_scores` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `lessonid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `gradeid` bigint(10) NOT NULL DEFAULT '0',
  `nickname` varchar(5) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_lesshighscor_use_ix` (`userid`),
  KEY `mdl_lesshighscor_les_ix` (`lessonid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='high scores for each lesson';

/*Data for the table `mdl_lesson_high_scores` */

/*Table structure for table `mdl_lesson_pages` */

DROP TABLE IF EXISTS `mdl_lesson_pages`;

CREATE TABLE `mdl_lesson_pages` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `lessonid` bigint(10) NOT NULL DEFAULT '0',
  `prevpageid` bigint(10) NOT NULL DEFAULT '0',
  `nextpageid` bigint(10) NOT NULL DEFAULT '0',
  `qtype` smallint(3) NOT NULL DEFAULT '0',
  `qoption` smallint(3) NOT NULL DEFAULT '0',
  `layout` smallint(3) NOT NULL DEFAULT '1',
  `display` smallint(3) NOT NULL DEFAULT '1',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `title` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `contents` longtext COLLATE utf8_unicode_ci NOT NULL,
  `contentsformat` tinyint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_lesspage_les_ix` (`lessonid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines lesson_pages';

/*Data for the table `mdl_lesson_pages` */

/*Table structure for table `mdl_lesson_timer` */

DROP TABLE IF EXISTS `mdl_lesson_timer`;

CREATE TABLE `mdl_lesson_timer` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `lessonid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `starttime` bigint(10) NOT NULL DEFAULT '0',
  `lessontime` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_lesstime_use_ix` (`userid`),
  KEY `mdl_lesstime_les_ix` (`lessonid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='lesson timer for each lesson';

/*Data for the table `mdl_lesson_timer` */

/*Table structure for table `mdl_license` */

DROP TABLE IF EXISTS `mdl_license`;

CREATE TABLE `mdl_license` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `shortname` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `fullname` longtext COLLATE utf8_unicode_ci,
  `source` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `enabled` tinyint(1) NOT NULL DEFAULT '1',
  `version` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='store licenses used by moodle';

/*Data for the table `mdl_license` */

insert  into `mdl_license`(`id`,`shortname`,`fullname`,`source`,`enabled`,`version`) values (1,'unknown','Unknown license','',1,2010033100),(2,'allrightsreserved','All rights reserved','http://en.wikipedia.org/wiki/All_rights_reserved',1,2010033100),(3,'public','Public Domain','http://creativecommons.org/licenses/publicdomain/',1,2010033100),(4,'cc','Creative Commons','http://creativecommons.org/licenses/by/3.0/',1,2010033100),(5,'cc-nd','Creative Commons - NoDerivs','http://creativecommons.org/licenses/by-nd/3.0/',1,2010033100),(6,'cc-nc-nd','Creative Commons - No Commercial NoDerivs','http://creativecommons.org/licenses/by-nc-nd/3.0/',1,2010033100),(7,'cc-nc','Creative Commons - No Commercial','http://creativecommons.org/licenses/by-nc/3.0/',1,2013051500),(8,'cc-nc-sa','Creative Commons - No Commercial ShareAlike','http://creativecommons.org/licenses/by-nc-sa/3.0/',1,2010033100),(9,'cc-sa','Creative Commons - ShareAlike','http://creativecommons.org/licenses/by-sa/3.0/',1,2010033100);

/*Table structure for table `mdl_local_datahub_schedule` */

DROP TABLE IF EXISTS `mdl_local_datahub_schedule`;

CREATE TABLE `mdl_local_datahub_schedule` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL,
  `plugin` varchar(63) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `config` longtext COLLATE utf8_unicode_ci NOT NULL,
  `lastruntime` bigint(10) NOT NULL DEFAULT '0',
  `nextruntime` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locadatasche_use_ix` (`userid`),
  KEY `mdl_locadatasche_plu_ix` (`plugin`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Scheduled DH imports and exports';

/*Data for the table `mdl_local_datahub_schedule` */

/*Table structure for table `mdl_local_datahub_summary_logs` */

DROP TABLE IF EXISTS `mdl_local_datahub_summary_logs`;

CREATE TABLE `mdl_local_datahub_summary_logs` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `export` tinyint(1) NOT NULL DEFAULT '0',
  `plugin` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `userid` bigint(10) NOT NULL,
  `targetstarttime` bigint(10) NOT NULL,
  `starttime` bigint(10) NOT NULL,
  `endtime` bigint(10) NOT NULL,
  `filesuccesses` bigint(10) NOT NULL,
  `filefailures` bigint(10) NOT NULL,
  `storedsuccesses` bigint(10) NOT NULL,
  `storedfailures` bigint(10) NOT NULL,
  `statusmessage` longtext COLLATE utf8_unicode_ci NOT NULL,
  `dbops` bigint(10) NOT NULL DEFAULT '-1',
  `unmetdependency` bigint(10) NOT NULL DEFAULT '0',
  `logpath` longtext COLLATE utf8_unicode_ci,
  `entitytype` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `mdl_locadatasummlogs_plu_ix` (`plugin`),
  KEY `mdl_locadatasummlogs_use_ix` (`userid`),
  KEY `mdl_locadatasummlogs_tar_ix` (`targetstarttime`),
  KEY `mdl_locadatasummlogs_sta_ix` (`starttime`),
  KEY `mdl_locadatasummlogs_end_ix` (`endtime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Storage for per-run errors and statistics';

/*Data for the table `mdl_local_datahub_summary_logs` */

/*Table structure for table `mdl_local_eliscore_field` */

DROP TABLE IF EXISTS `mdl_local_eliscore_field`;

CREATE TABLE `mdl_local_eliscore_field` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `shortname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `name` longtext COLLATE utf8_unicode_ci NOT NULL,
  `datatype` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci,
  `categoryid` bigint(10) NOT NULL,
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  `multivalued` tinyint(1) DEFAULT '0',
  `forceunique` tinyint(1) NOT NULL DEFAULT '0',
  `params` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisfiel_sho_ix` (`shortname`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Custom field definitions';

/*Data for the table `mdl_local_eliscore_field` */

insert  into `mdl_local_eliscore_field`(`id`,`shortname`,`name`,`datatype`,`description`,`categoryid`,`sortorder`,`multivalued`,`forceunique`,`params`) values (1,'_elis_program_archive','Archive Program','bool',NULL,1,0,0,0,'a:0:{}'),(2,'_elis_course_pretest','Pre-test','char',NULL,2,0,0,0,'a:0:{}'),(3,'_elis_course_posttest','Post-test','char',NULL,2,0,0,0,'a:0:{}'),(4,'_elis_userset_classification','User Set classification','char',NULL,3,0,0,0,'a:0:{}'),(5,'_elis_userset_display_priority','Display Priority','int',NULL,4,0,0,0,'a:0:{}'),(6,'userset_group','Enable Corresponding Group','bool',NULL,5,0,0,0,'a:0:{}'),(7,'userset_groupings','Autoenrol users in groupings','bool',NULL,5,0,0,0,'a:0:{}'),(8,'_elis_userset_themepriority','Theme Priority','int',NULL,6,0,0,0,'a:0:{}'),(9,'_elis_userset_theme','Theme','char',NULL,6,0,0,0,'a:0:{}');

/*Table structure for table `mdl_local_eliscore_field_cats` */

DROP TABLE IF EXISTS `mdl_local_eliscore_field_cats`;

CREATE TABLE `mdl_local_eliscore_field_cats` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Custom field category definitions';

/*Data for the table `mdl_local_eliscore_field_cats` */

insert  into `mdl_local_eliscore_field_cats`(`id`,`name`,`sortorder`) values (1,'Archive Settings',0),(2,'Pre- and Post-tests',0),(3,'User Set classification',0),(4,'User Set Display Settings',0),(5,'Associated Group',0),(6,'User Set Theme',0);

/*Table structure for table `mdl_local_eliscore_field_clevels` */

DROP TABLE IF EXISTS `mdl_local_eliscore_field_clevels`;

CREATE TABLE `mdl_local_eliscore_field_clevels` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `fieldid` bigint(10) NOT NULL,
  `contextlevel` bigint(10) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Which context levels a custom field applies to';

/*Data for the table `mdl_local_eliscore_field_clevels` */

insert  into `mdl_local_eliscore_field_clevels`(`id`,`fieldid`,`contextlevel`) values (1,1,11),(2,2,13),(3,3,13),(4,4,16),(5,5,16),(6,6,16),(7,7,16),(8,8,16),(9,9,16);

/*Table structure for table `mdl_local_eliscore_field_owner` */

DROP TABLE IF EXISTS `mdl_local_eliscore_field_owner`;

CREATE TABLE `mdl_local_eliscore_field_owner` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `fieldid` bigint(10) DEFAULT NULL,
  `plugin` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `exclude` tinyint(1) DEFAULT '0',
  `params` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisfielowne_fie_ix` (`fieldid`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Custom field data owners';

/*Data for the table `mdl_local_eliscore_field_owner` */

insert  into `mdl_local_eliscore_field_owner`(`id`,`fieldid`,`plugin`,`exclude`,`params`) values (1,1,'manual',0,'a:6:{s:8:\"required\";i:0;s:15:\"view_capability\";s:0:\"\";s:15:\"edit_capability\";s:0:\"\";s:7:\"control\";s:8:\"checkbox\";s:14:\"options_source\";s:0:\"\";s:9:\"help_file\";s:35:\"elisprogram_archive/archive_program\";}'),(2,2,'manual',0,'a:5:{s:15:\"view_capability\";s:0:\"\";s:15:\"edit_capability\";s:0:\"\";s:7:\"control\";s:4:\"menu\";s:14:\"options_source\";s:19:\"learning_objectives\";s:9:\"help_file\";s:32:\"elisprogram_preposttest/pre_test\";}'),(3,3,'manual',0,'a:5:{s:15:\"view_capability\";s:0:\"\";s:15:\"edit_capability\";s:0:\"\";s:7:\"control\";s:4:\"menu\";s:14:\"options_source\";s:19:\"learning_objectives\";s:9:\"help_file\";s:33:\"elisprogram_preposttest/post_test\";}'),(4,4,'userset_classifications',0,'a:0:{}'),(5,4,'manual',0,'a:5:{s:15:\"view_capability\";s:0:\"\";s:15:\"edit_capability\";s:18:\"moodle/user:update\";s:7:\"control\";s:4:\"menu\";s:14:\"options_source\";s:23:\"userset_classifications\";s:9:\"help_file\";s:47:\"elisprogram_usetclassify/cluster_classification\";}'),(6,5,'manual',0,'a:5:{s:15:\"view_capability\";s:0:\"\";s:15:\"edit_capability\";s:0:\"\";s:7:\"control\";s:4:\"text\";s:14:\"options_source\";s:24:\"userset_display_priority\";s:9:\"help_file\";s:45:\"elisprogram_usetdisppriority/display_priority\";}'),(7,6,'manual',0,'a:8:{s:8:\"required\";i:0;s:15:\"edit_capability\";s:0:\"\";s:15:\"view_capability\";s:0:\"\";s:7:\"control\";s:8:\"checkbox\";s:7:\"columns\";i:30;s:4:\"rows\";i:10;s:9:\"maxlength\";i:2048;s:9:\"help_file\";s:36:\"elisprogram_usetgroups/userset_group\";}'),(8,7,'manual',0,'a:8:{s:8:\"required\";i:0;s:15:\"edit_capability\";s:0:\"\";s:15:\"view_capability\";s:0:\"\";s:7:\"control\";s:8:\"checkbox\";s:7:\"columns\";i:30;s:4:\"rows\";i:10;s:9:\"maxlength\";i:2048;s:9:\"help_file\";s:42:\"elisprogram_usetgroups/autoenrol_groupings\";}'),(9,8,'manual',0,'a:8:{s:8:\"required\";i:0;s:15:\"edit_capability\";s:0:\"\";s:15:\"view_capability\";s:0:\"\";s:7:\"control\";s:4:\"text\";s:7:\"columns\";i:30;s:4:\"rows\";i:10;s:9:\"maxlength\";i:2048;s:9:\"help_file\";s:50:\"elisprogram_usetthemes/_elis_userset_themepriority\";}'),(10,9,'manual',0,'a:9:{s:7:\"control\";s:4:\"menu\";s:14:\"options_source\";s:6:\"themes\";s:8:\"required\";i:0;s:15:\"edit_capability\";s:0:\"\";s:15:\"view_capability\";s:0:\"\";s:7:\"columns\";i:30;s:4:\"rows\";i:10;s:9:\"maxlength\";i:2048;s:9:\"help_file\";s:42:\"elisprogram_usetthemes/_elis_userset_theme\";}');

/*Table structure for table `mdl_local_eliscore_fld_cat_ctx` */

DROP TABLE IF EXISTS `mdl_local_eliscore_fld_cat_ctx`;

CREATE TABLE `mdl_local_eliscore_fld_cat_ctx` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `categoryid` bigint(10) DEFAULT NULL,
  `contextlevel` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisfldcatctx_con_ix` (`contextlevel`),
  KEY `mdl_locaelisfldcatctx_cat_ix` (`categoryid`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Which context levels a custom field category applies to';

/*Data for the table `mdl_local_eliscore_fld_cat_ctx` */

insert  into `mdl_local_eliscore_fld_cat_ctx`(`id`,`categoryid`,`contextlevel`) values (1,1,11),(2,2,13),(3,3,16),(4,4,16),(5,5,16),(6,6,16);

/*Table structure for table `mdl_local_eliscore_fld_data_char` */

DROP TABLE IF EXISTS `mdl_local_eliscore_fld_data_char`;

CREATE TABLE `mdl_local_eliscore_fld_data_char` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `contextid` bigint(10) DEFAULT NULL,
  `fieldid` bigint(10) NOT NULL,
  `data` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisflddatachar_con_ix` (`contextid`),
  KEY `mdl_locaelisflddatachar_fie_ix` (`fieldid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Custom field data for integer data types';

/*Data for the table `mdl_local_eliscore_fld_data_char` */

/*Table structure for table `mdl_local_eliscore_fld_data_int` */

DROP TABLE IF EXISTS `mdl_local_eliscore_fld_data_int`;

CREATE TABLE `mdl_local_eliscore_fld_data_int` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `contextid` bigint(10) DEFAULT NULL,
  `fieldid` bigint(10) NOT NULL,
  `data` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisflddataint_con_ix` (`contextid`),
  KEY `mdl_locaelisflddataint_fie_ix` (`fieldid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Custom field data for integer data types';

/*Data for the table `mdl_local_eliscore_fld_data_int` */

/*Table structure for table `mdl_local_eliscore_fld_data_num` */

DROP TABLE IF EXISTS `mdl_local_eliscore_fld_data_num`;

CREATE TABLE `mdl_local_eliscore_fld_data_num` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `contextid` bigint(10) DEFAULT NULL,
  `fieldid` bigint(10) NOT NULL,
  `data` decimal(15,5) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisflddatanum_con_ix` (`contextid`),
  KEY `mdl_locaelisflddatanum_fie_ix` (`fieldid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Custom field data for integer data types';

/*Data for the table `mdl_local_eliscore_fld_data_num` */

/*Table structure for table `mdl_local_eliscore_fld_data_text` */

DROP TABLE IF EXISTS `mdl_local_eliscore_fld_data_text`;

CREATE TABLE `mdl_local_eliscore_fld_data_text` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `contextid` bigint(10) DEFAULT NULL,
  `fieldid` bigint(10) NOT NULL,
  `data` longtext COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisflddatatext_con_ix` (`contextid`),
  KEY `mdl_locaelisflddatatext_fie_ix` (`fieldid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Custom field data for (long) textual data types';

/*Data for the table `mdl_local_eliscore_fld_data_text` */

/*Table structure for table `mdl_local_eliscore_sched_tasks` */

DROP TABLE IF EXISTS `mdl_local_eliscore_sched_tasks`;

CREATE TABLE `mdl_local_eliscore_sched_tasks` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `plugin` varchar(166) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `taskname` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `callfile` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `callfunction` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `lastruntime` bigint(10) NOT NULL DEFAULT '0',
  `nextruntime` bigint(10) NOT NULL DEFAULT '0',
  `blocking` tinyint(1) NOT NULL DEFAULT '0',
  `minute` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `hour` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `day` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `month` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `dayofweek` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timezone` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '99',
  `runsremaining` bigint(10) DEFAULT NULL,
  `startdate` bigint(10) DEFAULT NULL,
  `enddate` bigint(10) DEFAULT NULL,
  `customized` tinyint(1) NOT NULL DEFAULT '0',
  `blocked` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisschetask_plutas_ix` (`plugin`,`taskname`),
  KEY `mdl_locaelisschetask_nex_ix` (`nextruntime`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Scheduled tasks';

/*Data for the table `mdl_local_eliscore_sched_tasks` */

insert  into `mdl_local_eliscore_sched_tasks`(`id`,`plugin`,`taskname`,`callfile`,`callfunction`,`lastruntime`,`nextruntime`,`blocking`,`minute`,`hour`,`day`,`month`,`dayofweek`,`timezone`,`runsremaining`,`startdate`,`enddate`,`customized`,`blocked`) values (1,'local_elisprogram',NULL,'/local/elisprogram/lib/lib.php','s:7:\"pm_cron\";',0,1413861600,0,'*/5','*','*','*','*','99',NULL,NULL,NULL,0,0),(2,'local_elisprogram',NULL,'/local/elisprogram/lib/resultsengine.php','s:19:\"results_engine_cron\";',0,1413861600,0,'*/5','*','*','*','*','99',NULL,NULL,NULL,0,0),(3,'eliscore_etl',NULL,'/local/eliscore/plugins/etl/etl.php','s:22:\"user_activity_etl_cron\";',0,1413914400,0,'*','2-6','*','*','*','99',NULL,NULL,NULL,0,0);

/*Table structure for table `mdl_local_eliscore_wkflow_inst` */

DROP TABLE IF EXISTS `mdl_local_eliscore_wkflow_inst`;

CREATE TABLE `mdl_local_eliscore_wkflow_inst` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `type` varchar(127) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `subtype` varchar(127) COLLATE utf8_unicode_ci DEFAULT NULL,
  `userid` bigint(10) NOT NULL,
  `data` longtext COLLATE utf8_unicode_ci,
  `timemodified` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_locaeliswkflinst_usetyp_ix` (`userid`,`type`,`subtype`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Data about in-progress workflows';

/*Data for the table `mdl_local_eliscore_wkflow_inst` */

/*Table structure for table `mdl_local_elisprogram_certcfg` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_certcfg`;

CREATE TABLE `mdl_local_elisprogram_certcfg` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `entity_id` bigint(10) NOT NULL,
  `entity_type` varchar(9) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `cert_border` varchar(200) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `cert_seal` varchar(200) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `cert_template` varchar(200) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `disable` tinyint(1) NOT NULL DEFAULT '1',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_locaeliscert_entent_uix` (`entity_id`,`entity_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Certificate settings for ELIS entity instances and types';

/*Data for the table `mdl_local_elisprogram_certcfg` */

/*Table structure for table `mdl_local_elisprogram_certissued` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_certissued`;

CREATE TABLE `mdl_local_elisprogram_certissued` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `cm_userid` bigint(10) NOT NULL,
  `cert_setting_id` bigint(10) NOT NULL,
  `cert_code` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timeissued` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_locaeliscert_cer_uix` (`cert_code`),
  KEY `mdl_locaeliscert_cm__ix` (`cm_userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Certificates that have been issued.';

/*Data for the table `mdl_local_elisprogram_certissued` */

/*Table structure for table `mdl_local_elisprogram_cls` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_cls`;

CREATE TABLE `mdl_local_elisprogram_cls` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `idnumber` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `startdate` bigint(10) NOT NULL DEFAULT '0',
  `enddate` bigint(10) NOT NULL DEFAULT '0',
  `duration` bigint(10) NOT NULL DEFAULT '0',
  `starttimehour` bigint(10) NOT NULL DEFAULT '0',
  `starttimeminute` bigint(10) NOT NULL DEFAULT '0',
  `endtimehour` bigint(10) NOT NULL DEFAULT '0',
  `endtimeminute` bigint(10) NOT NULL DEFAULT '0',
  `maxstudents` bigint(10) NOT NULL DEFAULT '0',
  `environmentid` bigint(10) NOT NULL DEFAULT '0',
  `enrol_from_waitlist` bigint(10) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaeliscls_cou_ix` (`courseid`),
  KEY `mdl_locaeliscls_env_ix` (`environmentid`),
  KEY `mdl_locaeliscls_idn_ix` (`idnumber`),
  KEY `mdl_locaeliscls_end_ix` (`enddate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='local_elisprogram_cls table retrofitted from MySQL';

/*Data for the table `mdl_local_elisprogram_cls` */

/*Table structure for table `mdl_local_elisprogram_cls_enrol` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_cls_enrol`;

CREATE TABLE `mdl_local_elisprogram_cls_enrol` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `classid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `enrolmenttime` bigint(10) NOT NULL DEFAULT '0',
  `completetime` bigint(10) NOT NULL DEFAULT '0',
  `endtime` bigint(10) NOT NULL DEFAULT '0',
  `completestatusid` bigint(10) NOT NULL DEFAULT '0',
  `grade` decimal(10,5) NOT NULL DEFAULT '0.00000',
  `credits` decimal(10,2) DEFAULT '0.00',
  `locked` tinyint(2) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisclsenro_cla_ix` (`classid`),
  KEY `mdl_locaelisclsenro_use_ix` (`userid`),
  KEY `mdl_locaelisclsenro_com_ix` (`completetime`),
  KEY `mdl_locaelisclsenro_com2_ix` (`completestatusid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='local_elisprogram_cls_enrol table retrofitted from MySQL';

/*Data for the table `mdl_local_elisprogram_cls_enrol` */

/*Table structure for table `mdl_local_elisprogram_cls_graded` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_cls_graded`;

CREATE TABLE `mdl_local_elisprogram_cls_graded` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `classid` bigint(10) DEFAULT '0',
  `userid` bigint(10) DEFAULT '0',
  `completionid` bigint(10) DEFAULT '0',
  `grade` decimal(10,5) DEFAULT '0.00000',
  `locked` tinyint(2) DEFAULT '0',
  `timegraded` bigint(10) DEFAULT '0',
  `timemodified` bigint(10) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisclsgrad_cla_ix` (`classid`),
  KEY `mdl_locaelisclsgrad_use_ix` (`userid`),
  KEY `mdl_locaelisclsgrad_clause_ix` (`classid`,`userid`),
  KEY `mdl_locaelisclsgrad_com_ix` (`completionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Elements from local_elisprogram_crs_cmp that have been recor';

/*Data for the table `mdl_local_elisprogram_cls_graded` */

/*Table structure for table `mdl_local_elisprogram_cls_mdl` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_cls_mdl`;

CREATE TABLE `mdl_local_elisprogram_cls_mdl` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `classid` bigint(10) NOT NULL DEFAULT '0',
  `moodlecourseid` bigint(10) NOT NULL DEFAULT '0',
  `enroltype` tinyint(2) DEFAULT '0',
  `enrolplugin` varchar(20) COLLATE utf8_unicode_ci DEFAULT 'crlm',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `autocreated` tinyint(2) NOT NULL DEFAULT '-1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_locaelisclsmdl_clamoo_uix` (`classid`,`moodlecourseid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='local_elisprogram_cls_mdl table retrofitted from MySQL';

/*Data for the table `mdl_local_elisprogram_cls_mdl` */

/*Table structure for table `mdl_local_elisprogram_cls_nstrct` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_cls_nstrct`;

CREATE TABLE `mdl_local_elisprogram_cls_nstrct` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `classid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `assigntime` bigint(10) NOT NULL DEFAULT '0',
  `completetime` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisclsnstr_cla_ix` (`classid`),
  KEY `mdl_locaelisclsnstr_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='local_elisprogram_cls_nstrct table retrofitted from MySQL';

/*Data for the table `mdl_local_elisprogram_cls_nstrct` */

/*Table structure for table `mdl_local_elisprogram_crs` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_crs`;

CREATE TABLE `mdl_local_elisprogram_crs` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `code` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `idnumber` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `syllabus` longtext COLLATE utf8_unicode_ci NOT NULL,
  `documents` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `lengthdescription` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `length` bigint(10) NOT NULL DEFAULT '0',
  `credits` varchar(10) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `completion_grade` bigint(10) DEFAULT '0',
  `environmentid` bigint(10) NOT NULL DEFAULT '0',
  `cost` varchar(10) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `version` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_locaeliscrs_cod_ix` (`code`),
  KEY `mdl_locaeliscrs_idn_ix` (`idnumber`),
  KEY `mdl_locaeliscrs_nam_ix` (`name`),
  KEY `mdl_locaeliscrs_cre_ix` (`credits`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='local_elisprogram_crs table retrofitted from MySQL';

/*Data for the table `mdl_local_elisprogram_crs` */

/*Table structure for table `mdl_local_elisprogram_crs_cmp` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_crs_cmp`;

CREATE TABLE `mdl_local_elisprogram_crs_cmp` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `idnumber` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `description` longtext COLLATE utf8_unicode_ci,
  `completion_grade` bigint(10) DEFAULT '0',
  `required` tinyint(2) DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `mdl_locaeliscrscmp_cou_ix` (`courseid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines any extra elements that must be completed for a cour';

/*Data for the table `mdl_local_elisprogram_crs_cmp` */

/*Table structure for table `mdl_local_elisprogram_crs_coreq` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_crs_coreq`;

CREATE TABLE `mdl_local_elisprogram_crs_coreq` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `curriculumcourseid` bigint(10) NOT NULL DEFAULT '0',
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaeliscrscore_cur_ix` (`curriculumcourseid`),
  KEY `mdl_locaeliscrscore_cou_ix` (`courseid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='local_elisprogram_crs_coreq table retrofitted from MySQL';

/*Data for the table `mdl_local_elisprogram_crs_coreq` */

/*Table structure for table `mdl_local_elisprogram_crs_prereq` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_crs_prereq`;

CREATE TABLE `mdl_local_elisprogram_crs_prereq` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `curriculumcourseid` bigint(10) NOT NULL DEFAULT '0',
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaeliscrsprer_cur_ix` (`curriculumcourseid`),
  KEY `mdl_locaeliscrsprer_cou_ix` (`courseid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='local_elisprogram_crs_prereq table retrofitted from MySQL';

/*Data for the table `mdl_local_elisprogram_crs_prereq` */

/*Table structure for table `mdl_local_elisprogram_crs_tpl` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_crs_tpl`;

CREATE TABLE `mdl_local_elisprogram_crs_tpl` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL,
  `location` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `templateclass` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_locaeliscrstpl_cou_uix` (`courseid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Course templates';

/*Data for the table `mdl_local_elisprogram_crs_tpl` */

/*Table structure for table `mdl_local_elisprogram_env` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_env`;

CREATE TABLE `mdl_local_elisprogram_env` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) COLLATE utf8_unicode_ci NOT NULL DEFAULT '0',
  `description` longtext COLLATE utf8_unicode_ci NOT NULL,
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='local_elisprogram_env table retrofitted from MySQL';

/*Data for the table `mdl_local_elisprogram_env` */

/*Table structure for table `mdl_local_elisprogram_notifylog` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_notifylog`;

CREATE TABLE `mdl_local_elisprogram_notifylog` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `event` varchar(166) COLLATE utf8_unicode_ci DEFAULT NULL,
  `instance` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `fromuserid` bigint(10) NOT NULL DEFAULT '0',
  `data` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisnoti_useinseve_ix` (`userid`,`instance`,`event`),
  KEY `mdl_locaelisnoti_froinseve_ix` (`fromuserid`,`instance`,`event`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Contains logs of notifications for events.';

/*Data for the table `mdl_local_elisprogram_notifylog` */

/*Table structure for table `mdl_local_elisprogram_pgm` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_pgm`;

CREATE TABLE `mdl_local_elisprogram_pgm` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `idnumber` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `name` varchar(64) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci,
  `reqcredits` decimal(10,2) DEFAULT NULL,
  `iscustom` tinyint(1) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `timetocomplete` varchar(64) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `frequency` varchar(64) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `priority` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelispgm_nam_ix` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='local_elisprogram_pgm table retrofitted from MySQL';

/*Data for the table `mdl_local_elisprogram_pgm` */

/*Table structure for table `mdl_local_elisprogram_pgm_assign` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_pgm_assign`;

CREATE TABLE `mdl_local_elisprogram_pgm_assign` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `curriculumid` bigint(10) NOT NULL DEFAULT '0',
  `completed` tinyint(2) NOT NULL DEFAULT '0',
  `timecompleted` bigint(10) NOT NULL DEFAULT '0',
  `timeexpired` bigint(10) NOT NULL DEFAULT '0',
  `credits` decimal(10,2) NOT NULL DEFAULT '0.00',
  `locked` tinyint(2) NOT NULL DEFAULT '0',
  `certificatecode` varchar(40) COLLATE utf8_unicode_ci DEFAULT NULL,
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelispgmassi_usecurc_ix` (`userid`,`curriculumid`,`completed`),
  KEY `mdl_locaelispgmassi_com_ix` (`completed`),
  KEY `mdl_locaelispgmassi_cer_ix` (`certificatecode`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='local_elisprogram_pgm_assign table retrofitted from MySQL';

/*Data for the table `mdl_local_elisprogram_pgm_assign` */

/*Table structure for table `mdl_local_elisprogram_pgm_crs` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_pgm_crs`;

CREATE TABLE `mdl_local_elisprogram_pgm_crs` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `curriculumid` bigint(10) NOT NULL DEFAULT '0',
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `required` tinyint(2) DEFAULT '1',
  `frequency` bigint(10) DEFAULT '0',
  `timeperiod` longtext COLLATE utf8_unicode_ci,
  `position` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelispgmcrs_cur_ix` (`curriculumid`),
  KEY `mdl_locaelispgmcrs_cou_ix` (`courseid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='local_elisprogram_pgm_crs table retrofitted from MySQL';

/*Data for the table `mdl_local_elisprogram_pgm_crs` */

/*Table structure for table `mdl_local_elisprogram_res` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_res`;

CREATE TABLE `mdl_local_elisprogram_res` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `contextid` bigint(10) NOT NULL,
  `active` tinyint(1) NOT NULL DEFAULT '0',
  `eventtriggertype` tinyint(1) NOT NULL DEFAULT '0',
  `lockedgrade` tinyint(1) NOT NULL DEFAULT '0',
  `triggerstartdate` tinyint(1) NOT NULL DEFAULT '0',
  `days` tinyint(2) NOT NULL DEFAULT '0',
  `criteriatype` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_locaelisres_con_uix` (`contextid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='results engine settings';

/*Data for the table `mdl_local_elisprogram_res` */

/*Table structure for table `mdl_local_elisprogram_res_action` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_res_action`;

CREATE TABLE `mdl_local_elisprogram_res_action` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `resultsid` bigint(10) NOT NULL,
  `actiontype` tinyint(1) NOT NULL DEFAULT '0',
  `minimum` float(10,5) NOT NULL,
  `maximum` float(10,5) NOT NULL,
  `trackid` bigint(10) NOT NULL DEFAULT '0',
  `classid` bigint(10) NOT NULL DEFAULT '0',
  `fieldid` bigint(10) NOT NULL DEFAULT '0',
  `fielddata` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisresacti_res_ix` (`resultsid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='results engine action settings';

/*Data for the table `mdl_local_elisprogram_res_action` */

/*Table structure for table `mdl_local_elisprogram_res_clslog` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_res_clslog`;

CREATE TABLE `mdl_local_elisprogram_res_clslog` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `classid` bigint(10) NOT NULL,
  `datescheduled` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `daterun` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisresclsl_cla_ix` (`classid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='results engine class logs';

/*Data for the table `mdl_local_elisprogram_res_clslog` */

/*Table structure for table `mdl_local_elisprogram_res_stulog` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_res_stulog`;

CREATE TABLE `mdl_local_elisprogram_res_stulog` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `classlogid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `action` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `daterun` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisresstul_cla_ix` (`classlogid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='results engine student logs';

/*Data for the table `mdl_local_elisprogram_res_stulog` */

/*Table structure for table `mdl_local_elisprogram_tag` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_tag`;

CREATE TABLE `mdl_local_elisprogram_tag` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci NOT NULL,
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='local_elisprogram_tag table retrofitted from MySQL';

/*Data for the table `mdl_local_elisprogram_tag` */

/*Table structure for table `mdl_local_elisprogram_tag_inst` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_tag_inst`;

CREATE TABLE `mdl_local_elisprogram_tag_inst` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `instancetype` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '0',
  `instanceid` bigint(10) NOT NULL DEFAULT '0',
  `tagid` bigint(10) NOT NULL DEFAULT '0',
  `data` longtext COLLATE utf8_unicode_ci NOT NULL,
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_locaelistaginst_insins_uix` (`instancetype`,`instanceid`,`tagid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='local_elisprogram_tag_inst table retrofitted from MySQL';

/*Data for the table `mdl_local_elisprogram_tag_inst` */

/*Table structure for table `mdl_local_elisprogram_trk` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_trk`;

CREATE TABLE `mdl_local_elisprogram_trk` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `curid` bigint(10) NOT NULL DEFAULT '0',
  `idnumber` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `description` longtext COLLATE utf8_unicode_ci,
  `startdate` bigint(10) NOT NULL DEFAULT '0',
  `enddate` bigint(10) NOT NULL DEFAULT '0',
  `defaulttrack` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelistrk_cur_ix` (`curid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Track table';

/*Data for the table `mdl_local_elisprogram_trk` */

/*Table structure for table `mdl_local_elisprogram_trk_cls` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_trk_cls`;

CREATE TABLE `mdl_local_elisprogram_trk_cls` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `trackid` bigint(10) NOT NULL DEFAULT '0',
  `classid` bigint(10) NOT NULL DEFAULT '0',
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `autoenrol` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelistrkcls_tra_ix` (`trackid`),
  KEY `mdl_locaelistrkcls_cla_ix` (`classid`),
  KEY `mdl_locaelistrkcls_cou_ix` (`courseid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Track class table';

/*Data for the table `mdl_local_elisprogram_trk_cls` */

/*Table structure for table `mdl_local_elisprogram_uset` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_uset`;

CREATE TABLE `mdl_local_elisprogram_uset` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `display` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `parent` bigint(10) NOT NULL DEFAULT '0',
  `depth` bigint(10) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Cluster definitions.';

/*Data for the table `mdl_local_elisprogram_uset` */

/*Table structure for table `mdl_local_elisprogram_uset_asign` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_uset_asign`;

CREATE TABLE `mdl_local_elisprogram_uset_asign` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `clusterid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `plugin` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `autoenrol` bigint(10) NOT NULL DEFAULT '1',
  `leader` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisusetasig_clu_ix` (`clusterid`),
  KEY `mdl_locaelisusetasig_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Cluster assignments from cluster plugins.';

/*Data for the table `mdl_local_elisprogram_uset_asign` */

/*Table structure for table `mdl_local_elisprogram_uset_pgm` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_uset_pgm`;

CREATE TABLE `mdl_local_elisprogram_uset_pgm` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `clusterid` bigint(10) NOT NULL,
  `curriculumid` bigint(10) NOT NULL,
  `autoenrol` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisusetpgm_clu_ix` (`clusterid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Association between clusters and curricula';

/*Data for the table `mdl_local_elisprogram_uset_pgm` */

/*Table structure for table `mdl_local_elisprogram_uset_prfle` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_uset_prfle`;

CREATE TABLE `mdl_local_elisprogram_uset_prfle` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `clusterid` bigint(10) NOT NULL,
  `fieldid` bigint(10) NOT NULL,
  `value` longtext COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisusetprfl_clu_ix` (`clusterid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Profile fields to use for automatic cluster association.';

/*Data for the table `mdl_local_elisprogram_uset_prfle` */

/*Table structure for table `mdl_local_elisprogram_uset_trk` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_uset_trk`;

CREATE TABLE `mdl_local_elisprogram_uset_trk` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `clusterid` bigint(10) NOT NULL,
  `trackid` bigint(10) NOT NULL,
  `autoenrol` tinyint(1) NOT NULL DEFAULT '1',
  `autounenrol` tinyint(1) NOT NULL DEFAULT '0',
  `enrolmenttime` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisusettrk_clu_ix` (`clusterid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Association between clusters and tracks';

/*Data for the table `mdl_local_elisprogram_uset_trk` */

/*Table structure for table `mdl_local_elisprogram_usr` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_usr`;

CREATE TABLE `mdl_local_elisprogram_usr` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `username` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `password` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `idnumber` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `firstname` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `lastname` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `mi` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `email` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `email2` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `address` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `address2` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `city` varchar(120) COLLATE utf8_unicode_ci DEFAULT NULL,
  `state` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `postalcode` varchar(32) COLLATE utf8_unicode_ci DEFAULT NULL,
  `country` varchar(2) COLLATE utf8_unicode_ci DEFAULT NULL,
  `phone` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `phone2` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `fax` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `birthdate` varchar(10) COLLATE utf8_unicode_ci DEFAULT NULL,
  `gender` varchar(1) COLLATE utf8_unicode_ci DEFAULT NULL,
  `language` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `transfercredits` bigint(10) DEFAULT '0',
  `comments` longtext COLLATE utf8_unicode_ci,
  `notes` longtext COLLATE utf8_unicode_ci,
  `timecreated` bigint(10) DEFAULT NULL,
  `timeapproved` bigint(10) DEFAULT NULL,
  `timemodified` bigint(10) DEFAULT NULL,
  `inactive` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_locaelisusr_idn_ix` (`idnumber`),
  KEY `mdl_locaelisusr_las_ix` (`lastname`),
  KEY `mdl_locaelisusr_fir_ix` (`firstname`),
  KEY `mdl_locaelisusr_use_ix` (`username`),
  KEY `mdl_locaelisusr_idn2_ix` (`idnumber`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='local_elisprogram_usr table retrofitted from MySQL';

/*Data for the table `mdl_local_elisprogram_usr` */

insert  into `mdl_local_elisprogram_usr`(`id`,`username`,`password`,`idnumber`,`firstname`,`lastname`,`mi`,`email`,`email2`,`address`,`address2`,`city`,`state`,`postalcode`,`country`,`phone`,`phone2`,`fax`,`birthdate`,`gender`,`language`,`transfercredits`,`comments`,`notes`,`timecreated`,`timeapproved`,`timemodified`,`inactive`) values (2,'clisunnet@gmail.com','','clisunnet@gmail.com','John','Li',NULL,'xiaowuq@sunnet.us',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,0),(3,'admin','$2y$10$jLeyKZWWU2Yv7D7pE7gvw.YbJOq2PR0X5ASYmGEe7gx3zN0007e1m','admin','管理','用户',NULL,'xiaowuq@sunnet.us',NULL,'',NULL,'',NULL,NULL,'',NULL,NULL,NULL,NULL,NULL,'zh_cn',0,NULL,NULL,1413873059,NULL,1413873059,0);

/*Table structure for table `mdl_local_elisprogram_usr_mdl` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_usr_mdl`;

CREATE TABLE `mdl_local_elisprogram_usr_mdl` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `cuserid` bigint(10) NOT NULL,
  `muserid` bigint(10) NOT NULL,
  `idnumber` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_locaelisusrmdl_idn_uix` (`idnumber`),
  KEY `mdl_locaelisusrmdl_cus_ix` (`cuserid`),
  KEY `mdl_locaelisusrmdl_mus_ix` (`muserid`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Association between Moodle and CM users';

/*Data for the table `mdl_local_elisprogram_usr_mdl` */

insert  into `mdl_local_elisprogram_usr_mdl`(`id`,`cuserid`,`muserid`,`idnumber`) values (2,2,4,'clisunnet@gmail.com'),(3,3,2,'admin');

/*Table structure for table `mdl_local_elisprogram_usr_trk` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_usr_trk`;

CREATE TABLE `mdl_local_elisprogram_usr_trk` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL,
  `trackid` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_locaelisusrtrk_usetra_uix` (`userid`,`trackid`),
  KEY `mdl_locaelisusrtrk_use_ix` (`userid`),
  KEY `mdl_locaelisusrtrk_tra_ix` (`trackid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='User enrolment in tracks';

/*Data for the table `mdl_local_elisprogram_usr_trk` */

/*Table structure for table `mdl_local_elisprogram_waitlist` */

DROP TABLE IF EXISTS `mdl_local_elisprogram_waitlist`;

CREATE TABLE `mdl_local_elisprogram_waitlist` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `classid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `position` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Wait list for classes';

/*Data for the table `mdl_local_elisprogram_waitlist` */

/*Table structure for table `mdl_local_elisreports_schedule` */

DROP TABLE IF EXISTS `mdl_local_elisreports_schedule`;

CREATE TABLE `mdl_local_elisreports_schedule` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL,
  `report` varchar(63) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `config` longtext COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_locaelissche_use_ix` (`userid`),
  KEY `mdl_locaelissche_rep_ix` (`report`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Scheduled reports';

/*Data for the table `mdl_local_elisreports_schedule` */

/*Table structure for table `mdl_lock_db` */

DROP TABLE IF EXISTS `mdl_lock_db`;

CREATE TABLE `mdl_lock_db` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `resourcekey` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `expires` bigint(10) DEFAULT NULL,
  `owner` varchar(36) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_lockdb_res_uix` (`resourcekey`),
  KEY `mdl_lockdb_exp_ix` (`expires`),
  KEY `mdl_lockdb_own_ix` (`owner`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores active and inactive lock types for db locking method.';

/*Data for the table `mdl_lock_db` */

/*Table structure for table `mdl_log` */

DROP TABLE IF EXISTS `mdl_log`;

CREATE TABLE `mdl_log` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `time` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `ip` varchar(45) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `course` bigint(10) NOT NULL DEFAULT '0',
  `module` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `cmid` bigint(10) NOT NULL DEFAULT '0',
  `action` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `url` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `info` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_log_coumodact_ix` (`course`,`module`,`action`),
  KEY `mdl_log_tim_ix` (`time`),
  KEY `mdl_log_act_ix` (`action`),
  KEY `mdl_log_usecou_ix` (`userid`,`course`),
  KEY `mdl_log_cmi_ix` (`cmid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Every action is logged as far as possible';

/*Data for the table `mdl_log` */

/*Table structure for table `mdl_log_display` */

DROP TABLE IF EXISTS `mdl_log_display`;

CREATE TABLE `mdl_log_display` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `module` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `action` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `mtable` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `field` varchar(200) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `component` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_logdisp_modact_uix` (`module`,`action`)
) ENGINE=InnoDB AUTO_INCREMENT=188 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='For a particular module/action, specifies a moodle table/fie';

/*Data for the table `mdl_log_display` */

insert  into `mdl_log_display`(`id`,`module`,`action`,`mtable`,`field`,`component`) values (1,'course','user report','user','CONCAT(firstname, \' \', lastname)','moodle'),(2,'course','view','course','fullname','moodle'),(3,'course','view section','course_sections','name','moodle'),(4,'course','update','course','fullname','moodle'),(5,'course','hide','course','fullname','moodle'),(6,'course','show','course','fullname','moodle'),(7,'course','move','course','fullname','moodle'),(8,'course','enrol','course','fullname','moodle'),(9,'course','unenrol','course','fullname','moodle'),(10,'course','report log','course','fullname','moodle'),(11,'course','report live','course','fullname','moodle'),(12,'course','report outline','course','fullname','moodle'),(13,'course','report participation','course','fullname','moodle'),(14,'course','report stats','course','fullname','moodle'),(15,'category','add','course_categories','name','moodle'),(16,'category','hide','course_categories','name','moodle'),(17,'category','move','course_categories','name','moodle'),(18,'category','show','course_categories','name','moodle'),(19,'category','update','course_categories','name','moodle'),(20,'message','write','user','CONCAT(firstname, \' \', lastname)','moodle'),(21,'message','read','user','CONCAT(firstname, \' \', lastname)','moodle'),(22,'message','add contact','user','CONCAT(firstname, \' \', lastname)','moodle'),(23,'message','remove contact','user','CONCAT(firstname, \' \', lastname)','moodle'),(24,'message','block contact','user','CONCAT(firstname, \' \', lastname)','moodle'),(25,'message','unblock contact','user','CONCAT(firstname, \' \', lastname)','moodle'),(26,'group','view','groups','name','moodle'),(27,'tag','update','tag','name','moodle'),(28,'tag','flag','tag','name','moodle'),(29,'user','view','user','CONCAT(firstname, \' \', lastname)','moodle'),(30,'assign','add','assign','name','mod_assign'),(31,'assign','delete mod','assign','name','mod_assign'),(32,'assign','download all submissions','assign','name','mod_assign'),(33,'assign','grade submission','assign','name','mod_assign'),(34,'assign','lock submission','assign','name','mod_assign'),(35,'assign','reveal identities','assign','name','mod_assign'),(36,'assign','revert submission to draft','assign','name','mod_assign'),(37,'assign','set marking workflow state','assign','name','mod_assign'),(38,'assign','submission statement accepted','assign','name','mod_assign'),(39,'assign','submit','assign','name','mod_assign'),(40,'assign','submit for grading','assign','name','mod_assign'),(41,'assign','unlock submission','assign','name','mod_assign'),(42,'assign','update','assign','name','mod_assign'),(43,'assign','upload','assign','name','mod_assign'),(44,'assign','view','assign','name','mod_assign'),(45,'assign','view all','course','fullname','mod_assign'),(46,'assign','view confirm submit assignment form','assign','name','mod_assign'),(47,'assign','view grading form','assign','name','mod_assign'),(48,'assign','view submission','assign','name','mod_assign'),(49,'assign','view submission grading table','assign','name','mod_assign'),(50,'assign','view submit assignment form','assign','name','mod_assign'),(51,'assign','view feedback','assign','name','mod_assign'),(52,'assign','view batch set marking workflow state','assign','name','mod_assign'),(53,'assignment','view','assignment','name','mod_assignment'),(54,'assignment','add','assignment','name','mod_assignment'),(55,'assignment','update','assignment','name','mod_assignment'),(56,'assignment','view submission','assignment','name','mod_assignment'),(57,'assignment','upload','assignment','name','mod_assignment'),(58,'book','add','book','name','mod_book'),(59,'book','update','book','name','mod_book'),(60,'book','view','book','name','mod_book'),(61,'book','add chapter','book_chapters','title','mod_book'),(62,'book','update chapter','book_chapters','title','mod_book'),(63,'book','view chapter','book_chapters','title','mod_book'),(64,'chat','view','chat','name','mod_chat'),(65,'chat','add','chat','name','mod_chat'),(66,'chat','update','chat','name','mod_chat'),(67,'chat','report','chat','name','mod_chat'),(68,'chat','talk','chat','name','mod_chat'),(69,'choice','view','choice','name','mod_choice'),(70,'choice','update','choice','name','mod_choice'),(71,'choice','add','choice','name','mod_choice'),(72,'choice','report','choice','name','mod_choice'),(73,'choice','choose','choice','name','mod_choice'),(74,'choice','choose again','choice','name','mod_choice'),(75,'data','view','data','name','mod_data'),(76,'data','add','data','name','mod_data'),(77,'data','update','data','name','mod_data'),(78,'data','record delete','data','name','mod_data'),(79,'data','fields add','data_fields','name','mod_data'),(80,'data','fields update','data_fields','name','mod_data'),(81,'data','templates saved','data','name','mod_data'),(82,'data','templates def','data','name','mod_data'),(83,'feedback','startcomplete','feedback','name','mod_feedback'),(84,'feedback','submit','feedback','name','mod_feedback'),(85,'feedback','delete','feedback','name','mod_feedback'),(86,'feedback','view','feedback','name','mod_feedback'),(87,'feedback','view all','course','shortname','mod_feedback'),(88,'folder','view','folder','name','mod_folder'),(89,'folder','view all','folder','name','mod_folder'),(90,'folder','update','folder','name','mod_folder'),(91,'folder','add','folder','name','mod_folder'),(92,'forum','add','forum','name','mod_forum'),(93,'forum','update','forum','name','mod_forum'),(94,'forum','add discussion','forum_discussions','name','mod_forum'),(95,'forum','add post','forum_posts','subject','mod_forum'),(96,'forum','update post','forum_posts','subject','mod_forum'),(97,'forum','user report','user','CONCAT(firstname, \' \', lastname)','mod_forum'),(98,'forum','move discussion','forum_discussions','name','mod_forum'),(99,'forum','view subscribers','forum','name','mod_forum'),(100,'forum','view discussion','forum_discussions','name','mod_forum'),(101,'forum','view forum','forum','name','mod_forum'),(102,'forum','subscribe','forum','name','mod_forum'),(103,'forum','unsubscribe','forum','name','mod_forum'),(104,'glossary','add','glossary','name','mod_glossary'),(105,'glossary','update','glossary','name','mod_glossary'),(106,'glossary','view','glossary','name','mod_glossary'),(107,'glossary','view all','glossary','name','mod_glossary'),(108,'glossary','add entry','glossary','name','mod_glossary'),(109,'glossary','update entry','glossary','name','mod_glossary'),(110,'glossary','add category','glossary','name','mod_glossary'),(111,'glossary','update category','glossary','name','mod_glossary'),(112,'glossary','delete category','glossary','name','mod_glossary'),(113,'glossary','approve entry','glossary','name','mod_glossary'),(114,'glossary','disapprove entry','glossary','name','mod_glossary'),(115,'glossary','view entry','glossary_entries','concept','mod_glossary'),(116,'imscp','view','imscp','name','mod_imscp'),(117,'imscp','view all','imscp','name','mod_imscp'),(118,'imscp','update','imscp','name','mod_imscp'),(119,'imscp','add','imscp','name','mod_imscp'),(120,'label','add','label','name','mod_label'),(121,'label','update','label','name','mod_label'),(122,'lesson','start','lesson','name','mod_lesson'),(123,'lesson','end','lesson','name','mod_lesson'),(124,'lesson','view','lesson_pages','title','mod_lesson'),(125,'lti','view','lti','name','mod_lti'),(126,'lti','launch','lti','name','mod_lti'),(127,'lti','view all','lti','name','mod_lti'),(128,'page','view','page','name','mod_page'),(129,'page','view all','page','name','mod_page'),(130,'page','update','page','name','mod_page'),(131,'page','add','page','name','mod_page'),(132,'quiz','add','quiz','name','mod_quiz'),(133,'quiz','update','quiz','name','mod_quiz'),(134,'quiz','view','quiz','name','mod_quiz'),(135,'quiz','report','quiz','name','mod_quiz'),(136,'quiz','attempt','quiz','name','mod_quiz'),(137,'quiz','submit','quiz','name','mod_quiz'),(138,'quiz','review','quiz','name','mod_quiz'),(139,'quiz','editquestions','quiz','name','mod_quiz'),(140,'quiz','preview','quiz','name','mod_quiz'),(141,'quiz','start attempt','quiz','name','mod_quiz'),(142,'quiz','close attempt','quiz','name','mod_quiz'),(143,'quiz','continue attempt','quiz','name','mod_quiz'),(144,'quiz','edit override','quiz','name','mod_quiz'),(145,'quiz','delete override','quiz','name','mod_quiz'),(146,'quiz','view summary','quiz','name','mod_quiz'),(147,'resource','view','resource','name','mod_resource'),(148,'resource','view all','resource','name','mod_resource'),(149,'resource','update','resource','name','mod_resource'),(150,'resource','add','resource','name','mod_resource'),(151,'scorm','view','scorm','name','mod_scorm'),(152,'scorm','review','scorm','name','mod_scorm'),(153,'scorm','update','scorm','name','mod_scorm'),(154,'scorm','add','scorm','name','mod_scorm'),(155,'survey','add','survey','name','mod_survey'),(156,'survey','update','survey','name','mod_survey'),(157,'survey','download','survey','name','mod_survey'),(158,'survey','view form','survey','name','mod_survey'),(159,'survey','view graph','survey','name','mod_survey'),(160,'survey','view report','survey','name','mod_survey'),(161,'survey','submit','survey','name','mod_survey'),(162,'url','view','url','name','mod_url'),(163,'url','view all','url','name','mod_url'),(164,'url','update','url','name','mod_url'),(165,'url','add','url','name','mod_url'),(166,'workshop','add','workshop','name','mod_workshop'),(167,'workshop','update','workshop','name','mod_workshop'),(168,'workshop','view','workshop','name','mod_workshop'),(169,'workshop','view all','workshop','name','mod_workshop'),(170,'workshop','add submission','workshop_submissions','title','mod_workshop'),(171,'workshop','update submission','workshop_submissions','title','mod_workshop'),(172,'workshop','view submission','workshop_submissions','title','mod_workshop'),(173,'workshop','add assessment','workshop_submissions','title','mod_workshop'),(174,'workshop','update assessment','workshop_submissions','title','mod_workshop'),(175,'workshop','add example','workshop_submissions','title','mod_workshop'),(176,'workshop','update example','workshop_submissions','title','mod_workshop'),(177,'workshop','view example','workshop_submissions','title','mod_workshop'),(178,'workshop','add reference assessment','workshop_submissions','title','mod_workshop'),(179,'workshop','update reference assessment','workshop_submissions','title','mod_workshop'),(180,'workshop','add example assessment','workshop_submissions','title','mod_workshop'),(181,'workshop','update example assessment','workshop_submissions','title','mod_workshop'),(182,'workshop','update aggregate grades','workshop','name','mod_workshop'),(183,'workshop','update clear aggregated grades','workshop','name','mod_workshop'),(184,'workshop','update clear assessments','workshop','name','mod_workshop'),(185,'book','exportimscp','book','name','booktool_exportimscp'),(186,'book','print','book','name','booktool_print'),(187,'book','print chapter','book_chapters','title','booktool_print');

/*Table structure for table `mdl_log_queries` */

DROP TABLE IF EXISTS `mdl_log_queries`;

CREATE TABLE `mdl_log_queries` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `qtype` mediumint(5) NOT NULL,
  `sqltext` longtext COLLATE utf8_unicode_ci NOT NULL,
  `sqlparams` longtext COLLATE utf8_unicode_ci,
  `error` mediumint(5) NOT NULL DEFAULT '0',
  `info` longtext COLLATE utf8_unicode_ci,
  `backtrace` longtext COLLATE utf8_unicode_ci,
  `exectime` decimal(10,5) NOT NULL,
  `timelogged` bigint(10) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Logged database queries.';

/*Data for the table `mdl_log_queries` */

/*Table structure for table `mdl_logstore_standard_log` */

DROP TABLE IF EXISTS `mdl_logstore_standard_log`;

CREATE TABLE `mdl_logstore_standard_log` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `eventname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `component` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `action` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `target` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `objecttable` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `objectid` bigint(10) DEFAULT NULL,
  `crud` varchar(1) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `edulevel` tinyint(1) NOT NULL,
  `contextid` bigint(10) NOT NULL,
  `contextlevel` bigint(10) NOT NULL,
  `contextinstanceid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `courseid` bigint(10) DEFAULT NULL,
  `relateduserid` bigint(10) DEFAULT NULL,
  `anonymous` tinyint(1) NOT NULL DEFAULT '0',
  `other` longtext COLLATE utf8_unicode_ci,
  `timecreated` bigint(10) NOT NULL,
  `origin` varchar(10) COLLATE utf8_unicode_ci DEFAULT NULL,
  `ip` varchar(45) COLLATE utf8_unicode_ci DEFAULT NULL,
  `realuserid` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_logsstanlog_tim_ix` (`timecreated`),
  KEY `mdl_logsstanlog_couanotim_ix` (`courseid`,`anonymous`,`timecreated`),
  KEY `mdl_logsstanlog_useconconcr_ix` (`userid`,`contextlevel`,`contextinstanceid`,`crud`,`edulevel`,`timecreated`)
) ENGINE=InnoDB AUTO_INCREMENT=460 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Standard log table';

/*Data for the table `mdl_logstore_standard_log` */

insert  into `mdl_logstore_standard_log`(`id`,`eventname`,`component`,`action`,`target`,`objecttable`,`objectid`,`crud`,`edulevel`,`contextid`,`contextlevel`,`contextinstanceid`,`userid`,`courseid`,`relateduserid`,`anonymous`,`other`,`timecreated`,`origin`,`ip`,`realuserid`) values (1,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413857253,'web','127.0.0.1',NULL),(2,'\\core\\event\\user_password_updated','core','updated','user_password',NULL,NULL,'u',0,5,30,2,2,0,2,0,'a:1:{s:14:\"forgottenreset\";b:0;}',1413858055,'web','127.0.0.1',NULL),(3,'\\core\\event\\user_updated','core','updated','user','user',2,'u',0,5,30,2,2,0,2,0,'N;',1413858055,'web','127.0.0.1',NULL),(4,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413858117,'web','127.0.0.1',NULL),(5,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413859008,'web','127.0.0.1',NULL),(6,'\\core\\event\\user_loggedout','core','loggedout','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"ullu1etcs9bbod343o0nifbja3\";}',1413859033,'web','127.0.0.1',NULL),(7,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,0,1,NULL,0,'N;',1413859088,'web','127.0.0.1',NULL),(8,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413859093,'web','127.0.0.1',NULL),(9,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413859093,'web','127.0.0.1',NULL),(10,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413859122,'web','127.0.0.1',NULL),(11,'\\core\\event\\user_loggedout','core','loggedout','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"7asod97vmvrmdl5dsmoothh330\";}',1413859133,'web','127.0.0.1',NULL),(12,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,0,1,NULL,0,'N;',1413859137,'web','127.0.0.1',NULL),(13,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,0,1,NULL,0,'N;',1413859137,'web','127.0.0.1',NULL),(14,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413859142,'web','127.0.0.1',NULL),(15,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413859142,'web','127.0.0.1',NULL),(16,'\\core\\event\\user_password_updated','core','updated','user_password',NULL,NULL,'u',0,16,30,3,2,0,3,0,'a:1:{s:14:\"forgottenreset\";b:0;}',1413862469,'web','127.0.0.1',NULL),(17,'\\core\\event\\user_created','core','created','user','user',3,'c',0,16,30,3,2,0,3,0,'N;',1413862469,'web','127.0.0.1',NULL),(18,'\\core\\event\\user_loggedin','core','loggedin','user','user',3,'r',0,1,10,0,3,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413862470,'web','127.0.0.1',NULL),(19,'\\core\\event\\user_password_updated','core','updated','user_password',NULL,NULL,'u',0,17,30,4,3,0,4,0,'a:1:{s:14:\"forgottenreset\";b:0;}',1413862564,'web','127.0.0.1',NULL),(20,'\\core\\event\\user_created','core','created','user','user',4,'c',0,17,30,4,3,0,4,0,'N;',1413862564,'web','127.0.0.1',NULL),(21,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413862564,'web','127.0.0.1',NULL),(22,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:1;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413862564,'web','127.0.0.1',NULL),(23,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413862565,'web','127.0.0.1',NULL),(24,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,0,1,NULL,0,'N;',1413862615,'web','127.0.0.1',NULL),(25,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413862729,'web','127.0.0.1',NULL),(26,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413862730,'web','127.0.0.1',NULL),(27,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"jldrru0g960ekck6ljjm18dqs3\";}',1413862763,'web','127.0.0.1',NULL),(28,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863210,'web','127.0.0.1',NULL),(29,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863212,'web','127.0.0.1',NULL),(30,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"nlbeh9am7gvrkocbqfq7e4p990\";}',1413863289,'web','127.0.0.1',NULL),(31,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863294,'web','127.0.0.1',NULL),(32,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863317,'web','127.0.0.1',NULL),(33,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863342,'web','127.0.0.1',NULL),(34,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863346,'web','127.0.0.1',NULL),(35,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863347,'web','127.0.0.1',NULL),(36,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863378,'web','127.0.0.1',NULL),(37,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"dtaltjp46joj2knn5gevff1262\";}',1413863445,'web','127.0.0.1',NULL),(38,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863461,'web','127.0.0.1',NULL),(39,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"55gtsmcbo4skuf5p57dbm1eer0\";}',1413863481,'web','127.0.0.1',NULL),(40,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863491,'web','127.0.0.1',NULL),(41,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"ls13hrcthfi45lhki5ih636up6\";}',1413863625,'web','127.0.0.1',NULL),(42,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863699,'web','127.0.0.1',NULL),(43,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"eog206bottdev6hbd8rdk6ghr2\";}',1413863702,'web','127.0.0.1',NULL),(44,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863718,'web','127.0.0.1',NULL),(45,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"s4bptu30hmcbq2tuf4fbp2ooh3\";}',1413863721,'web','127.0.0.1',NULL),(46,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863742,'web','127.0.0.1',NULL),(47,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863744,'web','127.0.0.1',NULL),(48,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863744,'web','127.0.0.1',NULL),(49,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863744,'web','127.0.0.1',NULL),(50,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863745,'web','127.0.0.1',NULL),(51,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863745,'web','127.0.0.1',NULL),(52,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"58f1kn02bsuuusj23lr875c310\";}',1413863747,'web','127.0.0.1',NULL),(53,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863749,'web','127.0.0.1',NULL),(54,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"mqc78qbo5jvo5mgv8j8ulru8r5\";}',1413863795,'web','127.0.0.1',NULL),(55,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"7ms13iqf53et0c5fgfqpcfqkl4\";}',1413863803,'web','127.0.0.1',NULL),(56,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413863829,'web','127.0.0.1',NULL),(57,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413863830,'web','127.0.0.1',NULL),(58,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413863831,'web','127.0.0.1',NULL),(59,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413863831,'web','127.0.0.1',NULL),(60,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413863832,'web','127.0.0.1',NULL),(61,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413863832,'web','127.0.0.1',NULL),(62,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"1091dlreo9lrbolddhvmk6f0i0\";}',1413863901,'web','127.0.0.1',NULL),(63,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413863995,'web','127.0.0.1',NULL),(64,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"doiah7bifjg0ncrs0nigkvtrj2\";}',1413864037,'web','127.0.0.1',NULL),(65,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413864081,'web','127.0.0.1',NULL),(66,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413864081,'web','127.0.0.1',NULL),(67,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"7cljv9qdipu37v5obf0qrhv9p1\";}',1413864083,'web','127.0.0.1',NULL),(68,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413864195,'web','127.0.0.1',NULL),(69,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413864195,'web','127.0.0.1',NULL),(70,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"rjt1f9ciqou2kph2qvpjr3rj36\";}',1413864200,'web','127.0.0.1',NULL),(71,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413864217,'web','127.0.0.1',NULL),(72,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413864217,'web','127.0.0.1',NULL),(73,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"eji0ns45i9547ck2civc2s25p6\";}',1413864247,'web','127.0.0.1',NULL),(74,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413864251,'web','127.0.0.1',NULL),(75,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413864251,'web','127.0.0.1',NULL),(76,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,0,1,NULL,0,'N;',1413864595,'web','127.0.0.1',NULL),(77,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413872832,'web','127.0.0.1',NULL),(78,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413872835,'web','127.0.0.1',NULL),(79,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413872835,'web','127.0.0.1',NULL),(80,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413872835,'web','127.0.0.1',NULL),(81,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413872836,'web','127.0.0.1',NULL),(82,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413872836,'web','127.0.0.1',NULL),(83,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413872836,'web','127.0.0.1',NULL),(84,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413872836,'web','127.0.0.1',NULL),(85,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413872836,'web','127.0.0.1',NULL),(86,'\\core\\event\\user_loggedout','core','loggedout','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"42i5b6d86hlqurn8vbpvlk2rc3\";}',1413872925,'web','127.0.0.1',NULL),(87,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413872932,'web','127.0.0.1',NULL),(88,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413872932,'web','127.0.0.1',NULL),(89,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413872934,'web','127.0.0.1',NULL),(90,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413872934,'web','127.0.0.1',NULL),(91,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413872934,'web','127.0.0.1',NULL),(92,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413872934,'web','127.0.0.1',NULL),(93,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413873002,'web','127.0.0.1',NULL),(94,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873002,'web','127.0.0.1',NULL),(95,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873003,'web','127.0.0.1',NULL),(96,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873003,'web','127.0.0.1',NULL),(97,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873004,'web','127.0.0.1',NULL),(98,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873004,'web','127.0.0.1',NULL),(99,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873004,'web','127.0.0.1',NULL),(100,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873004,'web','127.0.0.1',NULL),(101,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873004,'web','127.0.0.1',NULL),(102,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873057,'web','127.0.0.1',NULL),(103,'\\core\\event\\user_loggedout','core','loggedout','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"hfbgshavme4143hvqnsbuaan27\";}',1413873065,'web','127.0.0.1',NULL),(104,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413873077,'web','127.0.0.1',NULL),(105,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873077,'web','127.0.0.1',NULL),(106,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873238,'web','127.0.0.1',NULL),(107,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873240,'web','127.0.0.1',NULL),(108,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873240,'web','127.0.0.1',NULL),(109,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873240,'web','127.0.0.1',NULL),(110,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873240,'web','127.0.0.1',NULL),(111,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873241,'web','127.0.0.1',NULL),(112,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873241,'web','127.0.0.1',NULL),(113,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873241,'web','127.0.0.1',NULL),(114,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873241,'web','127.0.0.1',NULL),(115,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413873277,'web','127.0.0.1',NULL),(116,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:1:\"1\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413873277,'web','127.0.0.1',NULL),(117,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:2;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413873277,'web','127.0.0.1',NULL),(118,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413873278,'web','127.0.0.1',NULL),(119,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:1:\"2\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413873278,'web','127.0.0.1',NULL),(120,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:3;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413873278,'web','127.0.0.1',NULL),(121,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413873278,'web','127.0.0.1',NULL),(122,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:1:\"3\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413873278,'web','127.0.0.1',NULL),(123,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:4;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413873278,'web','127.0.0.1',NULL),(124,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873278,'web','127.0.0.1',NULL),(125,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873281,'web','127.0.0.1',NULL),(126,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"r0e771400beman545i4vu0afn2\";}',1413873287,'web','127.0.0.1',NULL),(127,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413873297,'web','127.0.0.1',NULL),(128,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873297,'web','127.0.0.1',NULL),(129,'\\core\\event\\user_loggedout','core','loggedout','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"2igkn528j7vibt3ku2hs8f8tm2\";}',1413873321,'web','127.0.0.1',NULL),(130,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413873324,'web','127.0.0.1',NULL),(131,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:1:\"4\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413873324,'web','127.0.0.1',NULL),(132,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:5;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413873324,'web','127.0.0.1',NULL),(133,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413873331,'web','127.0.0.1',NULL),(134,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:1:\"5\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413873331,'web','127.0.0.1',NULL),(135,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:6;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413873331,'web','127.0.0.1',NULL),(136,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413873372,'web','127.0.0.1',NULL),(137,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873372,'web','127.0.0.1',NULL),(138,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413873428,'web','127.0.0.1',NULL),(139,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413873432,'web','127.0.0.1',NULL),(140,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413873434,'web','127.0.0.1',NULL),(141,'\\core\\event\\user_loggedout','core','loggedout','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"qiq6dd68cvotpli9hfmquu3a23\";}',1413873469,'web','127.0.0.1',NULL),(142,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413873477,'web','127.0.0.1',NULL),(143,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873477,'web','127.0.0.1',NULL),(144,'\\core\\event\\user_loggedout','core','loggedout','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"5mktk8oejqeq78r2v3iuttc906\";}',1413873552,'web','127.0.0.1',NULL),(145,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413873621,'web','127.0.0.1',NULL),(146,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413873621,'web','127.0.0.1',NULL),(147,'\\core\\event\\user_loggedout','core','loggedout','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"b7c3jsfltaapb7tj7lee44mio6\";}',1413873623,'web','127.0.0.1',NULL),(148,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413873626,'web','127.0.0.1',NULL),(149,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:1:\"6\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413873626,'web','127.0.0.1',NULL),(150,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:7;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413873626,'web','127.0.0.1',NULL),(151,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873626,'web','127.0.0.1',NULL),(152,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873628,'web','127.0.0.1',NULL),(153,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873629,'web','127.0.0.1',NULL),(154,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873629,'web','127.0.0.1',NULL),(155,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873630,'web','127.0.0.1',NULL),(156,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873630,'web','127.0.0.1',NULL),(157,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873630,'web','127.0.0.1',NULL),(158,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873630,'web','127.0.0.1',NULL),(159,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873630,'web','127.0.0.1',NULL),(160,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873631,'web','127.0.0.1',NULL),(161,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873733,'web','127.0.0.1',NULL),(162,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"9ssnteop4oj13cuuri249csf65\";}',1413873738,'web','127.0.0.1',NULL),(163,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413873778,'web','127.0.0.1',NULL),(164,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:1:\"7\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413873778,'web','127.0.0.1',NULL),(165,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:8;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413873779,'web','127.0.0.1',NULL),(166,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873779,'web','127.0.0.1',NULL),(167,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873786,'web','127.0.0.1',NULL),(168,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413873881,'web','127.0.0.1',NULL),(169,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:1:\"8\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413873881,'web','127.0.0.1',NULL),(170,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:9;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413873881,'web','127.0.0.1',NULL),(171,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873881,'web','127.0.0.1',NULL),(172,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873927,'web','127.0.0.1',NULL),(173,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873928,'web','127.0.0.1',NULL),(174,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873929,'web','127.0.0.1',NULL),(175,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873929,'web','127.0.0.1',NULL),(176,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873930,'web','127.0.0.1',NULL),(177,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873930,'web','127.0.0.1',NULL),(178,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873942,'web','127.0.0.1',NULL),(179,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413873960,'web','127.0.0.1',NULL),(180,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:1:\"9\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413873960,'web','127.0.0.1',NULL),(181,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:10;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413873960,'web','127.0.0.1',NULL),(182,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413873960,'web','127.0.0.1',NULL),(183,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874010,'web','127.0.0.1',NULL),(184,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874012,'web','127.0.0.1',NULL),(185,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874024,'web','127.0.0.1',NULL),(186,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874025,'web','127.0.0.1',NULL),(187,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874026,'web','127.0.0.1',NULL),(188,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874027,'web','127.0.0.1',NULL),(189,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874027,'web','127.0.0.1',NULL),(190,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"hkv5d4m2jtepp0sd1s1l3f0si2\";}',1413874229,'web','127.0.0.1',NULL),(191,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413874236,'web','127.0.0.1',NULL),(192,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413874236,'web','127.0.0.1',NULL),(193,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413874415,'web','127.0.0.1',NULL),(194,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"10\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413874415,'web','127.0.0.1',NULL),(195,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:11;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413874415,'web','127.0.0.1',NULL),(196,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874416,'web','127.0.0.1',NULL),(197,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874426,'web','127.0.0.1',NULL),(198,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874427,'web','127.0.0.1',NULL),(199,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874427,'web','127.0.0.1',NULL),(200,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874428,'web','127.0.0.1',NULL),(201,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874428,'web','127.0.0.1',NULL),(202,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413874589,'web','127.0.0.1',NULL),(203,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"11\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413874589,'web','127.0.0.1',NULL),(204,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:12;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413874589,'web','127.0.0.1',NULL),(205,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874589,'web','127.0.0.1',NULL),(206,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874593,'web','127.0.0.1',NULL),(207,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874597,'web','127.0.0.1',NULL),(208,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874629,'web','127.0.0.1',NULL),(209,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874636,'web','127.0.0.1',NULL),(210,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874638,'web','127.0.0.1',NULL),(211,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874639,'web','127.0.0.1',NULL),(212,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413874640,'web','127.0.0.1',NULL),(213,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413874975,'web','127.0.0.1',NULL),(214,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413874975,'web','127.0.0.1',NULL),(215,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413874999,'web','127.0.0.1',NULL),(216,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"12\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413874999,'web','127.0.0.1',NULL),(217,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:13;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413875000,'web','127.0.0.1',NULL),(218,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413875000,'web','127.0.0.1',NULL),(219,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413875004,'web','127.0.0.1',NULL),(220,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413875059,'web','127.0.0.1',NULL),(221,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413875260,'web','127.0.0.1',NULL),(222,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413875263,'web','127.0.0.1',NULL),(223,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413876152,'web','127.0.0.1',NULL),(224,'\\core\\event\\user_loggedout','core','loggedout','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"hf3dmd1q67ssk3eck7ljh7vmu3\";}',1413876189,'web','127.0.0.1',NULL),(225,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413876199,'web','127.0.0.1',NULL),(226,'\\core\\event\\user_loggedout','core','loggedout','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"pnu07t498i5g1tmffcv94esng5\";}',1413876385,'web','127.0.0.1',NULL),(227,'\\core\\event\\user_loggedout','core','loggedout','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"3f4lim3ptn7a45vroo0tn9oje2\";}',1413876395,'web','127.0.0.1',NULL),(228,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413878154,'web','127.0.0.1',NULL),(229,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"13\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413878154,'web','127.0.0.1',NULL),(230,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:14;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413878155,'web','127.0.0.1',NULL),(231,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413878176,'web','127.0.0.1',NULL),(232,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"14\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413878176,'web','127.0.0.1',NULL),(233,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:15;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413878176,'web','127.0.0.1',NULL),(234,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413878189,'web','127.0.0.1',NULL),(235,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413878234,'web','127.0.0.1',NULL),(236,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413878248,'web','127.0.0.1',NULL),(237,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"15\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413878248,'web','127.0.0.1',NULL),(238,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:16;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413878248,'web','127.0.0.1',NULL),(239,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878258,'web','127.0.0.1',NULL),(240,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878259,'web','127.0.0.1',NULL),(241,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878260,'web','127.0.0.1',NULL),(242,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878260,'web','127.0.0.1',NULL),(243,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878260,'web','127.0.0.1',NULL),(244,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878260,'web','127.0.0.1',NULL),(245,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878261,'web','127.0.0.1',NULL),(246,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878261,'web','127.0.0.1',NULL),(247,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878261,'web','127.0.0.1',NULL),(248,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878261,'web','127.0.0.1',NULL),(249,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413878291,'web','127.0.0.1',NULL),(250,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"16\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413878291,'web','127.0.0.1',NULL),(251,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:17;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413878291,'web','127.0.0.1',NULL),(252,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878291,'web','127.0.0.1',NULL),(253,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878293,'web','127.0.0.1',NULL),(254,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878304,'web','127.0.0.1',NULL),(255,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878306,'web','127.0.0.1',NULL),(256,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878306,'web','127.0.0.1',NULL),(257,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878308,'web','127.0.0.1',NULL),(258,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878317,'web','127.0.0.1',NULL),(259,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878327,'web','127.0.0.1',NULL),(260,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878328,'web','127.0.0.1',NULL),(261,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878329,'web','127.0.0.1',NULL),(262,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878329,'web','127.0.0.1',NULL),(263,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878330,'web','127.0.0.1',NULL),(264,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878330,'web','127.0.0.1',NULL),(265,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878338,'web','127.0.0.1',NULL),(266,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878411,'web','127.0.0.1',NULL),(267,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878413,'web','127.0.0.1',NULL),(268,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413878427,'web','127.0.0.1',NULL),(269,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"17\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413878427,'web','127.0.0.1',NULL),(270,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:18;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413878428,'web','127.0.0.1',NULL),(271,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413878455,'web','127.0.0.1',NULL),(272,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"18\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413878456,'web','127.0.0.1',NULL),(273,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:19;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413878456,'web','127.0.0.1',NULL),(274,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413878482,'web','127.0.0.1',NULL),(275,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"19\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413878482,'web','127.0.0.1',NULL),(276,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:20;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413878482,'web','127.0.0.1',NULL),(277,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413878528,'web','127.0.0.1',NULL),(278,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"20\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413878528,'web','127.0.0.1',NULL),(279,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:21;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413878529,'web','127.0.0.1',NULL),(280,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413878553,'web','127.0.0.1',NULL),(281,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"21\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413878553,'web','127.0.0.1',NULL),(282,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:22;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413878553,'web','127.0.0.1',NULL),(283,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413878761,'web','127.0.0.1',NULL),(284,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"22\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413878762,'web','127.0.0.1',NULL),(285,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:23;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413878762,'web','127.0.0.1',NULL),(286,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878773,'web','127.0.0.1',NULL),(287,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878776,'web','127.0.0.1',NULL),(288,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878776,'web','127.0.0.1',NULL),(289,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878777,'web','127.0.0.1',NULL),(290,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878777,'web','127.0.0.1',NULL),(291,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413878777,'web','127.0.0.1',NULL),(292,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413880465,'web','127.0.0.1',NULL),(293,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413883209,'web','127.0.0.1',NULL),(294,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"23\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413883209,'web','127.0.0.1',NULL),(295,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:24;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413883209,'web','127.0.0.1',NULL),(296,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413883209,'web','127.0.0.1',NULL),(297,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"tcn4c44ortk57hpsj2d26ck4c0\";}',1413883213,'web','127.0.0.1',NULL),(298,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413883813,'web','127.0.0.1',NULL),(299,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"24\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413883813,'web','127.0.0.1',NULL),(300,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:25;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413883813,'web','127.0.0.1',NULL),(301,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413883813,'web','127.0.0.1',NULL),(302,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413883818,'web','127.0.0.1',NULL),(303,'\\core\\event\\user_loggedout','core','loggedout','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:9:\"sessionid\";s:26:\"ud1denmn2d1s4gsgmlu5o9de52\";}',1413883820,'web','127.0.0.1',NULL),(304,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413885678,'web','127.0.0.1',NULL),(305,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"25\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413885678,'web','127.0.0.1',NULL),(306,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:26;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413885678,'web','127.0.0.1',NULL),(307,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885678,'web','127.0.0.1',NULL),(308,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885703,'web','127.0.0.1',NULL),(309,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413885729,'web','127.0.0.1',NULL),(310,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"26\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413885729,'web','127.0.0.1',NULL),(311,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:27;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413885730,'web','127.0.0.1',NULL),(312,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885730,'web','127.0.0.1',NULL),(313,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885733,'web','127.0.0.1',NULL),(314,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885734,'web','127.0.0.1',NULL),(315,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413885761,'web','127.0.0.1',NULL),(316,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"27\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413885761,'web','127.0.0.1',NULL),(317,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:28;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413885761,'web','127.0.0.1',NULL),(318,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885762,'web','127.0.0.1',NULL),(319,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885764,'web','127.0.0.1',NULL),(320,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413885779,'web','127.0.0.1',NULL),(321,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"28\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413885779,'web','127.0.0.1',NULL),(322,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:29;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413885779,'web','127.0.0.1',NULL),(323,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885779,'web','127.0.0.1',NULL),(324,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885787,'web','127.0.0.1',NULL),(325,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885788,'web','127.0.0.1',NULL),(326,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885789,'web','127.0.0.1',NULL),(327,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885790,'web','127.0.0.1',NULL),(328,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885796,'web','127.0.0.1',NULL),(329,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885797,'web','127.0.0.1',NULL),(330,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885799,'web','127.0.0.1',NULL),(331,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885799,'web','127.0.0.1',NULL),(332,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885799,'web','127.0.0.1',NULL),(333,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885799,'web','127.0.0.1',NULL),(334,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885816,'web','127.0.0.1',NULL),(335,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885835,'web','127.0.0.1',NULL),(336,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885843,'web','127.0.0.1',NULL),(337,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885891,'web','127.0.0.1',NULL),(338,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413885898,'web','127.0.0.1',NULL),(339,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"29\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413885899,'web','127.0.0.1',NULL),(340,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:30;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413885899,'web','127.0.0.1',NULL),(341,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885899,'web','127.0.0.1',NULL),(342,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885901,'web','127.0.0.1',NULL),(343,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885903,'web','127.0.0.1',NULL),(344,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413885908,'web','127.0.0.1',NULL),(345,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"30\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413885909,'web','127.0.0.1',NULL),(346,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:31;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413885909,'web','127.0.0.1',NULL),(347,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885909,'web','127.0.0.1',NULL),(348,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885911,'web','127.0.0.1',NULL),(349,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885912,'web','127.0.0.1',NULL),(350,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885913,'web','127.0.0.1',NULL),(351,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885913,'web','127.0.0.1',NULL),(352,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885913,'web','127.0.0.1',NULL),(353,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885914,'web','127.0.0.1',NULL),(354,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885914,'web','127.0.0.1',NULL),(355,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885914,'web','127.0.0.1',NULL),(356,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885914,'web','127.0.0.1',NULL),(357,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413885922,'web','127.0.0.1',NULL),(358,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"31\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413885922,'web','127.0.0.1',NULL),(359,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:32;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413885923,'web','127.0.0.1',NULL),(360,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885923,'web','127.0.0.1',NULL),(361,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413885933,'web','127.0.0.1',NULL),(362,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"32\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413885933,'web','127.0.0.1',NULL),(363,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:33;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413885933,'web','127.0.0.1',NULL),(364,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885933,'web','127.0.0.1',NULL),(365,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413885950,'web','127.0.0.1',NULL),(366,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"33\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413885950,'web','127.0.0.1',NULL),(367,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:34;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413885950,'web','127.0.0.1',NULL),(368,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885950,'web','127.0.0.1',NULL),(369,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885953,'web','127.0.0.1',NULL),(370,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885965,'web','127.0.0.1',NULL),(371,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885966,'web','127.0.0.1',NULL),(372,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413885999,'web','127.0.0.1',NULL),(373,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413886017,'web','127.0.0.1',NULL),(374,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413886072,'web','127.0.0.1',NULL),(375,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413886079,'web','127.0.0.1',NULL),(376,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"34\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413886079,'web','127.0.0.1',NULL),(377,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:35;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413886079,'web','127.0.0.1',NULL),(378,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413886079,'web','127.0.0.1',NULL),(379,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413886101,'web','127.0.0.1',NULL),(380,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"35\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413886101,'web','127.0.0.1',NULL),(381,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:36;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413886101,'web','127.0.0.1',NULL),(382,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413886102,'web','127.0.0.1',NULL),(383,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1413886157,'web','127.0.0.1',NULL),(384,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413886158,'web','127.0.0.1',NULL),(385,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413886164,'web','127.0.0.1',NULL),(386,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413886164,'web','127.0.0.1',NULL),(387,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413886165,'web','127.0.0.1',NULL),(388,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413886165,'web','127.0.0.1',NULL),(389,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413886165,'web','127.0.0.1',NULL),(390,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413886170,'web','127.0.0.1',NULL),(391,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413886182,'web','127.0.0.1',NULL),(392,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413886182,'web','127.0.0.1',NULL),(393,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413886182,'web','127.0.0.1',NULL),(394,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413886183,'web','127.0.0.1',NULL),(395,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413886183,'web','127.0.0.1',NULL),(396,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413886183,'web','127.0.0.1',NULL),(397,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413886193,'web','127.0.0.1',NULL),(398,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1413886198,'web','127.0.0.1',NULL),(399,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413887172,'web','127.0.0.1',NULL),(400,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"36\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413887172,'web','127.0.0.1',NULL),(401,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:37;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413887172,'web','127.0.0.1',NULL),(402,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413887172,'web','127.0.0.1',NULL),(403,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413887179,'web','127.0.0.1',NULL),(404,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413887180,'web','127.0.0.1',NULL),(405,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413887182,'web','127.0.0.1',NULL),(406,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413887183,'web','127.0.0.1',NULL),(407,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413888842,'web','127.0.0.1',NULL),(408,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"37\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413888842,'web','127.0.0.1',NULL),(409,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:38;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413888842,'web','127.0.0.1',NULL),(410,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413888842,'web','127.0.0.1',NULL),(411,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413888845,'web','127.0.0.1',NULL),(412,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413939676,'web','127.0.0.1',NULL),(413,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"38\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413939676,'web','127.0.0.1',NULL),(414,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:39;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413939677,'web','127.0.0.1',NULL),(415,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413940104,'web','127.0.0.1',NULL),(416,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"39\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413940104,'web','127.0.0.1',NULL),(417,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:40;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413940104,'web','127.0.0.1',NULL),(418,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413940118,'web','127.0.0.1',NULL),(419,'\\tool_capability\\event\\report_viewed','tool_capability','viewed','report',NULL,NULL,'r',0,1,10,0,4,0,NULL,0,'N;',1413940257,'web','127.0.0.1',NULL),(420,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413940834,'web','127.0.0.1',NULL),(421,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"40\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413940834,'web','127.0.0.1',NULL),(422,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:41;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413940835,'web','127.0.0.1',NULL),(423,'\\tool_capability\\event\\report_viewed','tool_capability','viewed','report',NULL,NULL,'r',0,1,10,0,4,0,NULL,0,'N;',1413940835,'web','127.0.0.1',NULL),(424,'\\tool_capability\\event\\report_viewed','tool_capability','viewed','report',NULL,NULL,'r',0,1,10,0,4,0,NULL,0,'N;',1413940838,'web','127.0.0.1',NULL),(425,'\\tool_capability\\event\\report_viewed','tool_capability','viewed','report',NULL,NULL,'r',0,1,10,0,4,0,NULL,0,'N;',1413940841,'web','127.0.0.1',NULL),(426,'\\tool_capability\\event\\report_viewed','tool_capability','viewed','report',NULL,NULL,'r',0,1,10,0,4,0,NULL,0,'N;',1413940855,'web','127.0.0.1',NULL),(427,'\\tool_capability\\event\\report_viewed','tool_capability','viewed','report',NULL,NULL,'r',0,1,10,0,4,0,NULL,0,'N;',1413940857,'web','127.0.0.1',NULL),(428,'\\tool_capability\\event\\report_viewed','tool_capability','viewed','report',NULL,NULL,'r',0,1,10,0,4,0,NULL,0,'N;',1413940858,'web','127.0.0.1',NULL),(429,'\\tool_capability\\event\\report_viewed','tool_capability','viewed','report',NULL,NULL,'r',0,1,10,0,4,0,NULL,0,'N;',1413940865,'web','127.0.0.1',NULL),(430,'\\tool_capability\\event\\report_viewed','tool_capability','viewed','report',NULL,NULL,'r',0,1,10,0,4,0,NULL,0,'N;',1413940869,'web','127.0.0.1',NULL),(431,'\\tool_capability\\event\\report_viewed','tool_capability','viewed','report',NULL,NULL,'r',0,1,10,0,4,0,NULL,0,'N;',1413940871,'web','127.0.0.1',NULL),(432,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413940873,'web','127.0.0.1',NULL),(433,'\\core\\event\\user_profile_viewed','core','viewed','user_profile','user',4,'r',0,17,30,4,4,0,4,0,'N;',1413940954,'web','127.0.0.1',NULL),(434,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413941974,'web','127.0.0.1',NULL),(435,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"41\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413941974,'web','127.0.0.1',NULL),(436,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:42;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413941974,'web','127.0.0.1',NULL),(437,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413941975,'web','127.0.0.1',NULL),(438,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413943145,'web','127.0.0.1',NULL),(439,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"42\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413943146,'web','127.0.0.1',NULL),(440,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:43;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413943146,'web','127.0.0.1',NULL),(441,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413943147,'web','127.0.0.1',NULL),(442,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413944726,'web','127.0.0.1',NULL),(443,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"43\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413944726,'web','127.0.0.1',NULL),(444,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:44;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413944726,'web','127.0.0.1',NULL),(445,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413944768,'web','127.0.0.1',NULL),(446,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"44\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413944768,'web','127.0.0.1',NULL),(447,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:45;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413944768,'web','127.0.0.1',NULL),(448,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413944768,'web','127.0.0.1',NULL),(449,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413947609,'web','127.0.0.1',NULL),(450,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"45\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413947609,'web','127.0.0.1',NULL),(451,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:46;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413947609,'web','127.0.0.1',NULL),(452,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413947610,'web','127.0.0.1',NULL),(453,'\\core\\event\\user_loggedin','core','loggedin','user','user',4,'r',0,1,10,0,4,0,NULL,0,'a:1:{s:8:\"username\";s:19:\"clisunnet@gmail.com\";}',1413975353,'web','127.0.0.1',NULL),(454,'\\core\\event\\role_unassigned','core','unassigned','role','role',1,'d',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";s:2:\"46\";s:9:\"component\";s:0:\"\";s:6:\"itemid\";s:1:\"0\";}',1413975353,'web','127.0.0.1',NULL),(455,'\\core\\event\\role_assigned','core','assigned','role','role',1,'c',0,1,10,0,4,0,4,0,'a:3:{s:2:\"id\";i:47;s:9:\"component\";s:0:\"\";s:6:\"itemid\";i:0;}',1413975354,'web','127.0.0.1',NULL),(456,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413975395,'web','127.0.0.1',NULL),(457,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,4,1,NULL,0,'N;',1413975403,'web','127.0.0.1',NULL),(458,'\\core\\event\\user_loggedin','core','loggedin','user','user',2,'r',0,1,10,0,2,0,NULL,0,'a:1:{s:8:\"username\";s:5:\"admin\";}',1414028443,'web','127.0.0.1',NULL),(459,'\\core\\event\\course_viewed','core','viewed','course',NULL,NULL,'r',2,2,50,1,2,1,NULL,0,'N;',1414028445,'web','127.0.0.1',NULL);

/*Table structure for table `mdl_lti` */

DROP TABLE IF EXISTS `mdl_lti`;

CREATE TABLE `mdl_lti` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci,
  `introformat` smallint(4) DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `typeid` bigint(10) DEFAULT NULL,
  `toolurl` longtext COLLATE utf8_unicode_ci NOT NULL,
  `securetoolurl` longtext COLLATE utf8_unicode_ci,
  `instructorchoicesendname` tinyint(1) DEFAULT NULL,
  `instructorchoicesendemailaddr` tinyint(1) DEFAULT NULL,
  `instructorchoiceallowroster` tinyint(1) DEFAULT NULL,
  `instructorchoiceallowsetting` tinyint(1) DEFAULT NULL,
  `instructorcustomparameters` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `instructorchoiceacceptgrades` tinyint(1) DEFAULT NULL,
  `grade` decimal(10,5) NOT NULL DEFAULT '100.00000',
  `launchcontainer` tinyint(2) NOT NULL DEFAULT '1',
  `resourcekey` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `password` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `debuglaunch` tinyint(1) NOT NULL DEFAULT '0',
  `showtitlelaunch` tinyint(1) NOT NULL DEFAULT '0',
  `showdescriptionlaunch` tinyint(1) NOT NULL DEFAULT '0',
  `servicesalt` varchar(40) COLLATE utf8_unicode_ci DEFAULT NULL,
  `icon` longtext COLLATE utf8_unicode_ci,
  `secureicon` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `mdl_lti_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table contains Basic LTI activities instances';

/*Data for the table `mdl_lti` */

/*Table structure for table `mdl_lti_submission` */

DROP TABLE IF EXISTS `mdl_lti_submission`;

CREATE TABLE `mdl_lti_submission` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `ltiid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `datesubmitted` bigint(10) NOT NULL,
  `dateupdated` bigint(10) NOT NULL,
  `gradepercent` decimal(10,5) NOT NULL,
  `originalgrade` decimal(10,5) NOT NULL,
  `launchid` bigint(10) NOT NULL,
  `state` tinyint(2) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_ltisubm_lti_ix` (`ltiid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Keeps track of individual submissions for LTI activities.';

/*Data for the table `mdl_lti_submission` */

/*Table structure for table `mdl_lti_types` */

DROP TABLE IF EXISTS `mdl_lti_types`;

CREATE TABLE `mdl_lti_types` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'basiclti Activity',
  `baseurl` longtext COLLATE utf8_unicode_ci NOT NULL,
  `tooldomain` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `state` tinyint(2) NOT NULL DEFAULT '2',
  `course` bigint(10) NOT NULL,
  `coursevisible` tinyint(1) NOT NULL DEFAULT '0',
  `createdby` bigint(10) NOT NULL,
  `timecreated` bigint(10) NOT NULL,
  `timemodified` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_ltitype_cou_ix` (`course`),
  KEY `mdl_ltitype_too_ix` (`tooldomain`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Basic LTI pre-configured activities';

/*Data for the table `mdl_lti_types` */

/*Table structure for table `mdl_lti_types_config` */

DROP TABLE IF EXISTS `mdl_lti_types_config`;

CREATE TABLE `mdl_lti_types_config` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `typeid` bigint(10) NOT NULL,
  `name` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_ltitypeconf_typ_ix` (`typeid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Basic LTI types configuration';

/*Data for the table `mdl_lti_types_config` */

/*Table structure for table `mdl_message` */

DROP TABLE IF EXISTS `mdl_message`;

CREATE TABLE `mdl_message` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `useridfrom` bigint(10) NOT NULL DEFAULT '0',
  `useridto` bigint(10) NOT NULL DEFAULT '0',
  `subject` longtext COLLATE utf8_unicode_ci,
  `fullmessage` longtext COLLATE utf8_unicode_ci,
  `fullmessageformat` smallint(4) DEFAULT '0',
  `fullmessagehtml` longtext COLLATE utf8_unicode_ci,
  `smallmessage` longtext COLLATE utf8_unicode_ci,
  `notification` tinyint(1) DEFAULT '0',
  `contexturl` longtext COLLATE utf8_unicode_ci,
  `contexturlname` longtext COLLATE utf8_unicode_ci,
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_mess_use_ix` (`useridfrom`),
  KEY `mdl_mess_use2_ix` (`useridto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores all unread messages';

/*Data for the table `mdl_message` */

/*Table structure for table `mdl_message_airnotifier_devices` */

DROP TABLE IF EXISTS `mdl_message_airnotifier_devices`;

CREATE TABLE `mdl_message_airnotifier_devices` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userdeviceid` bigint(10) NOT NULL,
  `enable` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_messairndevi_use_uix` (`userdeviceid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Store information about the devices registered in Airnotifie';

/*Data for the table `mdl_message_airnotifier_devices` */

/*Table structure for table `mdl_message_contacts` */

DROP TABLE IF EXISTS `mdl_message_contacts`;

CREATE TABLE `mdl_message_contacts` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `contactid` bigint(10) NOT NULL DEFAULT '0',
  `blocked` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_messcont_usecon_uix` (`userid`,`contactid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Maintains lists of relationships between users';

/*Data for the table `mdl_message_contacts` */

/*Table structure for table `mdl_message_processors` */

DROP TABLE IF EXISTS `mdl_message_processors`;

CREATE TABLE `mdl_message_processors` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(166) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `enabled` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='List of message output plugins';

/*Data for the table `mdl_message_processors` */

insert  into `mdl_message_processors`(`id`,`name`,`enabled`) values (1,'airnotifier',0),(2,'email',1),(3,'jabber',1),(4,'popup',1);

/*Table structure for table `mdl_message_providers` */

DROP TABLE IF EXISTS `mdl_message_providers`;

CREATE TABLE `mdl_message_providers` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `component` varchar(200) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `capability` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_messprov_comnam_uix` (`component`,`name`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table stores the message providers (modules and core sy';

/*Data for the table `mdl_message_providers` */

insert  into `mdl_message_providers`(`id`,`name`,`component`,`capability`) values (1,'notices','moodle','moodle/site:config'),(2,'errors','moodle','moodle/site:config'),(3,'availableupdate','moodle','moodle/site:config'),(4,'instantmessage','moodle',NULL),(5,'backup','moodle','moodle/site:config'),(6,'courserequested','moodle','moodle/site:approvecourse'),(7,'courserequestapproved','moodle','moodle/course:request'),(8,'courserequestrejected','moodle','moodle/course:request'),(9,'badgerecipientnotice','moodle','moodle/badges:earnbadge'),(10,'badgecreatornotice','moodle',NULL),(11,'assign_notification','mod_assign',NULL),(12,'assignment_updates','mod_assignment',NULL),(13,'submission','mod_feedback',NULL),(14,'message','mod_feedback',NULL),(15,'posts','mod_forum',NULL),(16,'graded_essay','mod_lesson',NULL),(17,'submission','mod_quiz','mod/quiz:emailnotifysubmission'),(18,'confirmation','mod_quiz','mod/quiz:emailconfirmsubmission'),(19,'attempt_overdue','mod_quiz','mod/quiz:emailwarnoverdue'),(20,'flatfile_enrolment','enrol_flatfile',NULL),(21,'imsenterprise_enrolment','enrol_imsenterprise',NULL),(22,'expiry_notification','enrol_manual',NULL),(23,'paypal_enrolment','enrol_paypal',NULL),(24,'expiry_notification','enrol_self',NULL),(25,'notify_pm','local_elisprogram',NULL);

/*Table structure for table `mdl_message_read` */

DROP TABLE IF EXISTS `mdl_message_read`;

CREATE TABLE `mdl_message_read` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `useridfrom` bigint(10) NOT NULL DEFAULT '0',
  `useridto` bigint(10) NOT NULL DEFAULT '0',
  `subject` longtext COLLATE utf8_unicode_ci,
  `fullmessage` longtext COLLATE utf8_unicode_ci,
  `fullmessageformat` smallint(4) DEFAULT '0',
  `fullmessagehtml` longtext COLLATE utf8_unicode_ci,
  `smallmessage` longtext COLLATE utf8_unicode_ci,
  `notification` tinyint(1) DEFAULT '0',
  `contexturl` longtext COLLATE utf8_unicode_ci,
  `contexturlname` longtext COLLATE utf8_unicode_ci,
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timeread` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_messread_use_ix` (`useridfrom`),
  KEY `mdl_messread_use2_ix` (`useridto`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores all messages that have been read';

/*Data for the table `mdl_message_read` */

/*Table structure for table `mdl_message_working` */

DROP TABLE IF EXISTS `mdl_message_working`;

CREATE TABLE `mdl_message_working` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `unreadmessageid` bigint(10) NOT NULL,
  `processorid` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_messwork_unr_ix` (`unreadmessageid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Lists all the messages and processors that need to be proces';

/*Data for the table `mdl_message_working` */

/*Table structure for table `mdl_mnet_application` */

DROP TABLE IF EXISTS `mdl_mnet_application`;

CREATE TABLE `mdl_mnet_application` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `display_name` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `xmlrpc_server_url` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `sso_land_url` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `sso_jump_url` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Information about applications on remote hosts';

/*Data for the table `mdl_mnet_application` */

insert  into `mdl_mnet_application`(`id`,`name`,`display_name`,`xmlrpc_server_url`,`sso_land_url`,`sso_jump_url`) values (1,'moodle','Moodle','/mnet/xmlrpc/server.php','/auth/mnet/land.php','/auth/mnet/jump.php'),(2,'mahara','Mahara','/api/xmlrpc/server.php','/auth/xmlrpc/land.php','/auth/xmlrpc/jump.php'),(3,'java','Java servlet','/mnet/server','/mnet/land.jsp','');

/*Table structure for table `mdl_mnet_host` */

DROP TABLE IF EXISTS `mdl_mnet_host`;

CREATE TABLE `mdl_mnet_host` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `deleted` tinyint(1) NOT NULL DEFAULT '0',
  `wwwroot` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `ip_address` varchar(45) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `name` varchar(80) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `public_key` longtext COLLATE utf8_unicode_ci NOT NULL,
  `public_key_expires` bigint(10) NOT NULL DEFAULT '0',
  `transport` tinyint(2) NOT NULL DEFAULT '0',
  `portno` mediumint(5) NOT NULL DEFAULT '0',
  `last_connect_time` bigint(10) NOT NULL DEFAULT '0',
  `last_log_id` bigint(10) NOT NULL DEFAULT '0',
  `force_theme` tinyint(1) NOT NULL DEFAULT '0',
  `theme` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `applicationid` bigint(10) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `mdl_mnethost_app_ix` (`applicationid`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Information about the local and remote hosts for RPC';

/*Data for the table `mdl_mnet_host` */

insert  into `mdl_mnet_host`(`id`,`deleted`,`wwwroot`,`ip_address`,`name`,`public_key`,`public_key_expires`,`transport`,`portno`,`last_connect_time`,`last_log_id`,`force_theme`,`theme`,`applicationid`) values (1,0,'http://lms.sunnet.cc','127.0.0.1','','',0,0,0,0,0,0,NULL,1),(2,0,'','','All Hosts','',0,0,0,0,0,0,NULL,1);

/*Table structure for table `mdl_mnet_host2service` */

DROP TABLE IF EXISTS `mdl_mnet_host2service`;

CREATE TABLE `mdl_mnet_host2service` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `hostid` bigint(10) NOT NULL DEFAULT '0',
  `serviceid` bigint(10) NOT NULL DEFAULT '0',
  `publish` tinyint(1) NOT NULL DEFAULT '0',
  `subscribe` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_mnethost_hosser_uix` (`hostid`,`serviceid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Information about the services for a given host';

/*Data for the table `mdl_mnet_host2service` */

/*Table structure for table `mdl_mnet_log` */

DROP TABLE IF EXISTS `mdl_mnet_log`;

CREATE TABLE `mdl_mnet_log` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `hostid` bigint(10) NOT NULL DEFAULT '0',
  `remoteid` bigint(10) NOT NULL DEFAULT '0',
  `time` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `ip` varchar(45) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `course` bigint(10) NOT NULL DEFAULT '0',
  `coursename` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `module` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `cmid` bigint(10) NOT NULL DEFAULT '0',
  `action` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `url` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `info` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_mnetlog_hosusecou_ix` (`hostid`,`userid`,`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Store session data from users migrating to other sites';

/*Data for the table `mdl_mnet_log` */

/*Table structure for table `mdl_mnet_remote_rpc` */

DROP TABLE IF EXISTS `mdl_mnet_remote_rpc`;

CREATE TABLE `mdl_mnet_remote_rpc` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `functionname` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `xmlrpcpath` varchar(80) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `plugintype` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `pluginname` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `enabled` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table describes functions that might be called remotely';

/*Data for the table `mdl_mnet_remote_rpc` */

insert  into `mdl_mnet_remote_rpc`(`id`,`functionname`,`xmlrpcpath`,`plugintype`,`pluginname`,`enabled`) values (1,'user_authorise','auth/mnet/auth.php/user_authorise','auth','mnet',1),(2,'keepalive_server','auth/mnet/auth.php/keepalive_server','auth','mnet',1),(3,'kill_children','auth/mnet/auth.php/kill_children','auth','mnet',1),(4,'refresh_log','auth/mnet/auth.php/refresh_log','auth','mnet',1),(5,'fetch_user_image','auth/mnet/auth.php/fetch_user_image','auth','mnet',1),(6,'fetch_theme_info','auth/mnet/auth.php/fetch_theme_info','auth','mnet',1),(7,'update_enrolments','auth/mnet/auth.php/update_enrolments','auth','mnet',1),(8,'keepalive_client','auth/mnet/auth.php/keepalive_client','auth','mnet',1),(9,'kill_child','auth/mnet/auth.php/kill_child','auth','mnet',1),(10,'available_courses','enrol/mnet/enrol.php/available_courses','enrol','mnet',1),(11,'user_enrolments','enrol/mnet/enrol.php/user_enrolments','enrol','mnet',1),(12,'enrol_user','enrol/mnet/enrol.php/enrol_user','enrol','mnet',1),(13,'unenrol_user','enrol/mnet/enrol.php/unenrol_user','enrol','mnet',1),(14,'course_enrolments','enrol/mnet/enrol.php/course_enrolments','enrol','mnet',1),(15,'send_content_intent','portfolio/mahara/lib.php/send_content_intent','portfolio','mahara',1),(16,'send_content_ready','portfolio/mahara/lib.php/send_content_ready','portfolio','mahara',1);

/*Table structure for table `mdl_mnet_remote_service2rpc` */

DROP TABLE IF EXISTS `mdl_mnet_remote_service2rpc`;

CREATE TABLE `mdl_mnet_remote_service2rpc` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `serviceid` bigint(10) NOT NULL DEFAULT '0',
  `rpcid` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_mnetremoserv_rpcser_uix` (`rpcid`,`serviceid`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Group functions or methods under a service';

/*Data for the table `mdl_mnet_remote_service2rpc` */

insert  into `mdl_mnet_remote_service2rpc`(`id`,`serviceid`,`rpcid`) values (1,1,1),(2,1,2),(3,1,3),(4,1,4),(5,1,5),(6,1,6),(7,1,7),(8,2,8),(9,2,9),(10,3,10),(11,3,11),(12,3,12),(13,3,13),(14,3,14),(15,4,15),(16,4,16);

/*Table structure for table `mdl_mnet_rpc` */

DROP TABLE IF EXISTS `mdl_mnet_rpc`;

CREATE TABLE `mdl_mnet_rpc` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `functionname` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `xmlrpcpath` varchar(80) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `plugintype` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `pluginname` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `enabled` tinyint(1) NOT NULL DEFAULT '0',
  `help` longtext COLLATE utf8_unicode_ci NOT NULL,
  `profile` longtext COLLATE utf8_unicode_ci NOT NULL,
  `filename` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `classname` varchar(150) COLLATE utf8_unicode_ci DEFAULT NULL,
  `static` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_mnetrpc_enaxml_ix` (`enabled`,`xmlrpcpath`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Functions or methods that we may publish or subscribe to';

/*Data for the table `mdl_mnet_rpc` */

insert  into `mdl_mnet_rpc`(`id`,`functionname`,`xmlrpcpath`,`plugintype`,`pluginname`,`enabled`,`help`,`profile`,`filename`,`classname`,`static`) values (1,'user_authorise','auth/mnet/auth.php/user_authorise','auth','mnet',1,'Return user data for the provided token, compare with user_agent string.','a:2:{s:10:\"parameters\";a:2:{i:0;a:3:{s:4:\"name\";s:5:\"token\";s:4:\"type\";s:6:\"string\";s:11:\"description\";s:37:\"The unique ID provided by remotehost.\";}i:1;a:3:{s:4:\"name\";s:9:\"useragent\";s:4:\"type\";s:6:\"string\";s:11:\"description\";s:0:\"\";}}s:6:\"return\";a:2:{s:4:\"type\";s:5:\"array\";s:11:\"description\";s:44:\"$userdata Array of user info for remote host\";}}','auth.php','auth_plugin_mnet',0),(2,'keepalive_server','auth/mnet/auth.php/keepalive_server','auth','mnet',1,'Receives an array of usernames from a remote machine and prods their\nsessions to keep them alive','a:2:{s:10:\"parameters\";a:1:{i:0;a:3:{s:4:\"name\";s:5:\"array\";s:4:\"type\";s:5:\"array\";s:11:\"description\";s:21:\"An array of usernames\";}}s:6:\"return\";a:2:{s:4:\"type\";s:6:\"string\";s:11:\"description\";s:28:\"\"All ok\" or an error message\";}}','auth.php','auth_plugin_mnet',0),(3,'kill_children','auth/mnet/auth.php/kill_children','auth','mnet',1,'The IdP uses this function to kill child sessions on other hosts','a:2:{s:10:\"parameters\";a:2:{i:0;a:3:{s:4:\"name\";s:8:\"username\";s:4:\"type\";s:6:\"string\";s:11:\"description\";s:28:\"Username for session to kill\";}i:1;a:3:{s:4:\"name\";s:9:\"useragent\";s:4:\"type\";s:6:\"string\";s:11:\"description\";s:0:\"\";}}s:6:\"return\";a:2:{s:4:\"type\";s:6:\"string\";s:11:\"description\";s:39:\"A plaintext report of what has happened\";}}','auth.php','auth_plugin_mnet',0),(4,'refresh_log','auth/mnet/auth.php/refresh_log','auth','mnet',1,'Receives an array of log entries from an SP and adds them to the mnet_log\ntable','a:2:{s:10:\"parameters\";a:1:{i:0;a:3:{s:4:\"name\";s:5:\"array\";s:4:\"type\";s:5:\"array\";s:11:\"description\";s:21:\"An array of usernames\";}}s:6:\"return\";a:2:{s:4:\"type\";s:6:\"string\";s:11:\"description\";s:28:\"\"All ok\" or an error message\";}}','auth.php','auth_plugin_mnet',0),(5,'fetch_user_image','auth/mnet/auth.php/fetch_user_image','auth','mnet',1,'Returns the user\'s profile image info\nIf the user exists and has a profile picture, the returned array will contain keys:\n f1          - the content of the default 100x100px image\n f1_mimetype - the mimetype of the f1 file\n f2          - the content of the 35x35px variant of the image\n f2_mimetype - the mimetype of the f2 file\nThe mimetype information was added in Moodle 2.0. In Moodle 1.x, images are always jpegs.','a:2:{s:10:\"parameters\";a:1:{i:0;a:3:{s:4:\"name\";s:8:\"username\";s:4:\"type\";s:3:\"int\";s:11:\"description\";s:18:\"The id of the user\";}}s:6:\"return\";a:2:{s:4:\"type\";s:5:\"array\";s:11:\"description\";s:84:\"false if user not found, empty array if no picture exists, array with data otherwise\";}}','auth.php','auth_plugin_mnet',0),(6,'fetch_theme_info','auth/mnet/auth.php/fetch_theme_info','auth','mnet',1,'Returns the theme information and logo url as strings.','a:2:{s:10:\"parameters\";a:0:{}s:6:\"return\";a:2:{s:4:\"type\";s:6:\"string\";s:11:\"description\";s:14:\"The theme info\";}}','auth.php','auth_plugin_mnet',0),(7,'update_enrolments','auth/mnet/auth.php/update_enrolments','auth','mnet',1,'Invoke this function _on_ the IDP to update it with enrolment info local to\nthe SP right after calling user_authorise()\nNormally called by the SP after calling user_authorise()','a:2:{s:10:\"parameters\";a:2:{i:0;a:3:{s:4:\"name\";s:8:\"username\";s:4:\"type\";s:6:\"string\";s:11:\"description\";s:12:\"The username\";}i:1;a:3:{s:4:\"name\";s:7:\"courses\";s:4:\"type\";s:5:\"array\";s:11:\"description\";s:0:\"\";}}s:6:\"return\";a:2:{s:4:\"type\";s:4:\"bool\";s:11:\"description\";s:0:\"\";}}','auth.php','auth_plugin_mnet',0),(8,'keepalive_client','auth/mnet/auth.php/keepalive_client','auth','mnet',1,'Poll the IdP server to let it know that a user it has authenticated is still\nonline','a:2:{s:10:\"parameters\";a:0:{}s:6:\"return\";a:2:{s:4:\"type\";s:4:\"void\";s:11:\"description\";s:0:\"\";}}','auth.php','auth_plugin_mnet',0),(9,'kill_child','auth/mnet/auth.php/kill_child','auth','mnet',1,'When the IdP requests that child sessions are terminated,\nthis function will be called on each of the child hosts. The machine that\ncalls the function (over xmlrpc) provides us with the mnethostid we need.','a:2:{s:10:\"parameters\";a:2:{i:0;a:3:{s:4:\"name\";s:8:\"username\";s:4:\"type\";s:6:\"string\";s:11:\"description\";s:28:\"Username for session to kill\";}i:1;a:3:{s:4:\"name\";s:9:\"useragent\";s:4:\"type\";s:6:\"string\";s:11:\"description\";s:0:\"\";}}s:6:\"return\";a:2:{s:4:\"type\";s:4:\"bool\";s:11:\"description\";s:15:\"True on success\";}}','auth.php','auth_plugin_mnet',0),(10,'available_courses','enrol/mnet/enrol.php/available_courses','enrol','mnet',1,'Returns list of courses that we offer to the caller for remote enrolment of their users\nSince Moodle 2.0, courses are made available for MNet peers by creating an instance\nof enrol_mnet plugin for the course. Hidden courses are not returned. If there are two\ninstances - one specific for the host and one for \'All hosts\', the setting of the specific\none is used. The id of the peer is kept in customint1, no other custom fields are used.','a:2:{s:10:\"parameters\";a:0:{}s:6:\"return\";a:2:{s:4:\"type\";s:5:\"array\";s:11:\"description\";s:0:\"\";}}','enrol.php','enrol_mnet_mnetservice_enrol',0),(11,'user_enrolments','enrol/mnet/enrol.php/user_enrolments','enrol','mnet',1,'This method has never been implemented in Moodle MNet API','a:2:{s:10:\"parameters\";a:0:{}s:6:\"return\";a:2:{s:4:\"type\";s:5:\"array\";s:11:\"description\";s:11:\"empty array\";}}','enrol.php','enrol_mnet_mnetservice_enrol',0),(12,'enrol_user','enrol/mnet/enrol.php/enrol_user','enrol','mnet',1,'Enrol remote user to our course\nIf we do not have local record for the remote user in our database,\nit gets created here.','a:2:{s:10:\"parameters\";a:2:{i:0;a:3:{s:4:\"name\";s:8:\"userdata\";s:4:\"type\";s:5:\"array\";s:11:\"description\";s:14:\"user details {\";}i:1;a:3:{s:4:\"name\";s:8:\"courseid\";s:4:\"type\";s:3:\"int\";s:11:\"description\";s:19:\"our local course id\";}}s:6:\"return\";a:2:{s:4:\"type\";s:4:\"bool\";s:11:\"description\";s:69:\"true if the enrolment has been successful, throws exception otherwise\";}}','enrol.php','enrol_mnet_mnetservice_enrol',0),(13,'unenrol_user','enrol/mnet/enrol.php/unenrol_user','enrol','mnet',1,'Unenrol remote user from our course\nOnly users enrolled via enrol_mnet plugin can be unenrolled remotely. If the\nremote user is enrolled into the local course via some other enrol plugin\n(enrol_manual for example), the remote host can\'t touch such enrolment. Please\ndo not report this behaviour as bug, it is a feature ;-)','a:2:{s:10:\"parameters\";a:2:{i:0;a:3:{s:4:\"name\";s:8:\"username\";s:4:\"type\";s:6:\"string\";s:11:\"description\";s:18:\"of the remote user\";}i:1;a:3:{s:4:\"name\";s:8:\"courseid\";s:4:\"type\";s:3:\"int\";s:11:\"description\";s:0:\"\";}}s:6:\"return\";a:2:{s:4:\"type\";s:4:\"bool\";s:11:\"description\";s:71:\"true if the unenrolment has been successful, throws exception otherwise\";}}','enrol.php','enrol_mnet_mnetservice_enrol',0),(14,'course_enrolments','enrol/mnet/enrol.php/course_enrolments','enrol','mnet',1,'Returns a list of users from the client server who are enrolled in our course\nSuitable instance of enrol_mnet must be created in the course. This method will not\nreturn any information about the enrolments in courses that are not available for\nremote enrolment, even if their users are enrolled into them via other plugin\n(note the difference from {@link self::user_enrolments()}).\nThis method will return enrolment information for users from hosts regardless\nthe enrolment plugin. It does not matter if the user was enrolled remotely by\ntheir admin or locally. Once the course is available for remote enrolments, we\nwill tell them everything about their users.\nIn Moodle 1.x the returned array used to be indexed by username. The side effect\nof MDL-19219 fix is that we do not need to use such index and therefore we can\nreturn all enrolment records. MNet clients 1.x will only use the last record for\nthe student, if she is enrolled via multiple plugins.','a:2:{s:10:\"parameters\";a:2:{i:0;a:3:{s:4:\"name\";s:8:\"courseid\";s:4:\"type\";s:3:\"int\";s:11:\"description\";s:16:\"ID of our course\";}i:1;a:3:{s:4:\"name\";s:5:\"roles\";s:4:\"type\";s:5:\"array\";s:11:\"description\";s:0:\"\";}}s:6:\"return\";a:2:{s:4:\"type\";s:5:\"array\";s:11:\"description\";s:0:\"\";}}','enrol.php','enrol_mnet_mnetservice_enrol',0),(15,'fetch_file','portfolio/mahara/lib.php/fetch_file','portfolio','mahara',1,'xmlrpc (mnet) function to get the file.\nreads in the file and returns it base_64 encoded\nso that it can be enrypted by mnet.','a:2:{s:10:\"parameters\";a:1:{i:0;a:3:{s:4:\"name\";s:5:\"token\";s:4:\"type\";s:6:\"string\";s:11:\"description\";s:56:\"the token recieved previously during send_content_intent\";}}s:6:\"return\";a:2:{s:4:\"type\";s:4:\"void\";s:11:\"description\";s:0:\"\";}}','lib.php','portfolio_plugin_mahara',1);

/*Table structure for table `mdl_mnet_service` */

DROP TABLE IF EXISTS `mdl_mnet_service`;

CREATE TABLE `mdl_mnet_service` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `apiversion` varchar(10) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `offer` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='A service is a group of functions';

/*Data for the table `mdl_mnet_service` */

insert  into `mdl_mnet_service`(`id`,`name`,`description`,`apiversion`,`offer`) values (1,'sso_idp','','1',1),(2,'sso_sp','','1',1),(3,'mnet_enrol','','1',1),(4,'pf','','1',1);

/*Table structure for table `mdl_mnet_service2rpc` */

DROP TABLE IF EXISTS `mdl_mnet_service2rpc`;

CREATE TABLE `mdl_mnet_service2rpc` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `serviceid` bigint(10) NOT NULL DEFAULT '0',
  `rpcid` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_mnetserv_rpcser_uix` (`rpcid`,`serviceid`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Group functions or methods under a service';

/*Data for the table `mdl_mnet_service2rpc` */

insert  into `mdl_mnet_service2rpc`(`id`,`serviceid`,`rpcid`) values (1,1,1),(2,1,2),(3,1,3),(4,1,4),(5,1,5),(6,1,6),(7,1,7),(8,2,8),(9,2,9),(10,3,10),(11,3,11),(12,3,12),(13,3,13),(14,3,14),(15,4,15);

/*Table structure for table `mdl_mnet_session` */

DROP TABLE IF EXISTS `mdl_mnet_session`;

CREATE TABLE `mdl_mnet_session` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `username` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `token` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `mnethostid` bigint(10) NOT NULL DEFAULT '0',
  `useragent` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `confirm_timeout` bigint(10) NOT NULL DEFAULT '0',
  `session_id` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `expires` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_mnetsess_tok_uix` (`token`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Store session data from users migrating to other sites';

/*Data for the table `mdl_mnet_session` */

/*Table structure for table `mdl_mnet_sso_access_control` */

DROP TABLE IF EXISTS `mdl_mnet_sso_access_control`;

CREATE TABLE `mdl_mnet_sso_access_control` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `username` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `mnet_host_id` bigint(10) NOT NULL DEFAULT '0',
  `accessctrl` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'allow',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_mnetssoaccecont_mneuse_uix` (`mnet_host_id`,`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Users by host permitted (or not) to login from a remote prov';

/*Data for the table `mdl_mnet_sso_access_control` */

/*Table structure for table `mdl_mnetservice_enrol_courses` */

DROP TABLE IF EXISTS `mdl_mnetservice_enrol_courses`;

CREATE TABLE `mdl_mnetservice_enrol_courses` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `hostid` bigint(10) NOT NULL,
  `remoteid` bigint(10) NOT NULL,
  `categoryid` bigint(10) NOT NULL,
  `categoryname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  `fullname` varchar(254) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `shortname` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `idnumber` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `summary` longtext COLLATE utf8_unicode_ci NOT NULL,
  `summaryformat` smallint(3) DEFAULT '0',
  `startdate` bigint(10) NOT NULL,
  `roleid` bigint(10) NOT NULL,
  `rolename` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_mnetenrocour_hosrem_uix` (`hostid`,`remoteid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Caches the information fetched via XML-RPC about courses on ';

/*Data for the table `mdl_mnetservice_enrol_courses` */

/*Table structure for table `mdl_mnetservice_enrol_enrolments` */

DROP TABLE IF EXISTS `mdl_mnetservice_enrol_enrolments`;

CREATE TABLE `mdl_mnetservice_enrol_enrolments` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `hostid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `remotecourseid` bigint(10) NOT NULL,
  `rolename` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `enroltime` bigint(10) NOT NULL DEFAULT '0',
  `enroltype` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_mnetenroenro_use_ix` (`userid`),
  KEY `mdl_mnetenroenro_hos_ix` (`hostid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Caches the information about enrolments of our local users i';

/*Data for the table `mdl_mnetservice_enrol_enrolments` */

/*Table structure for table `mdl_modules` */

DROP TABLE IF EXISTS `mdl_modules`;

CREATE TABLE `mdl_modules` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `cron` bigint(10) NOT NULL DEFAULT '0',
  `lastcron` bigint(10) NOT NULL DEFAULT '0',
  `search` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `visible` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `mdl_modu_nam_ix` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='modules available in the site';

/*Data for the table `mdl_modules` */

insert  into `mdl_modules`(`id`,`name`,`cron`,`lastcron`,`search`,`visible`) values (1,'assign',60,0,'',1),(2,'assignment',60,0,'',0),(3,'book',0,0,'',1),(4,'chat',300,0,'',1),(5,'choice',0,0,'',1),(6,'data',0,0,'',1),(7,'feedback',0,0,'',0),(8,'folder',0,0,'',1),(9,'forum',0,0,'',1),(10,'glossary',0,0,'',1),(11,'imscp',0,0,'',1),(12,'label',0,0,'',1),(13,'lesson',0,0,'',1),(14,'lti',0,0,'',1),(15,'page',0,0,'',1),(16,'quiz',60,0,'',1),(17,'resource',0,0,'',1),(18,'scorm',300,0,'',1),(19,'survey',0,0,'',1),(20,'url',0,0,'',1),(21,'wiki',0,0,'',1),(22,'workshop',60,0,'',1);

/*Table structure for table `mdl_my_pages` */

DROP TABLE IF EXISTS `mdl_my_pages`;

CREATE TABLE `mdl_my_pages` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) DEFAULT '0',
  `name` varchar(200) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `private` tinyint(1) NOT NULL DEFAULT '1',
  `sortorder` mediumint(6) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_mypage_usepri_ix` (`userid`,`private`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Extra user pages for the My Moodle system';

/*Data for the table `mdl_my_pages` */

insert  into `mdl_my_pages`(`id`,`userid`,`name`,`private`,`sortorder`) values (1,NULL,'__default',0,0),(2,NULL,'__default',1,0);

/*Table structure for table `mdl_page` */

DROP TABLE IF EXISTS `mdl_page`;

CREATE TABLE `mdl_page` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `content` longtext COLLATE utf8_unicode_ci,
  `contentformat` smallint(4) NOT NULL DEFAULT '0',
  `legacyfiles` smallint(4) NOT NULL DEFAULT '0',
  `legacyfileslast` bigint(10) DEFAULT NULL,
  `display` smallint(4) NOT NULL DEFAULT '0',
  `displayoptions` longtext COLLATE utf8_unicode_ci,
  `revision` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_page_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Each record is one page and its config data';

/*Data for the table `mdl_page` */

/*Table structure for table `mdl_portfolio_instance` */

DROP TABLE IF EXISTS `mdl_portfolio_instance`;

CREATE TABLE `mdl_portfolio_instance` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `plugin` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `visible` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='base table (not including config data) for instances of port';

/*Data for the table `mdl_portfolio_instance` */

/*Table structure for table `mdl_portfolio_instance_config` */

DROP TABLE IF EXISTS `mdl_portfolio_instance_config`;

CREATE TABLE `mdl_portfolio_instance_config` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `instance` bigint(10) NOT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `mdl_portinstconf_nam_ix` (`name`),
  KEY `mdl_portinstconf_ins_ix` (`instance`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='config for portfolio plugin instances';

/*Data for the table `mdl_portfolio_instance_config` */

/*Table structure for table `mdl_portfolio_instance_user` */

DROP TABLE IF EXISTS `mdl_portfolio_instance_user`;

CREATE TABLE `mdl_portfolio_instance_user` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `instance` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  KEY `mdl_portinstuser_ins_ix` (`instance`),
  KEY `mdl_portinstuser_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='user data for portfolio instances.';

/*Data for the table `mdl_portfolio_instance_user` */

/*Table structure for table `mdl_portfolio_log` */

DROP TABLE IF EXISTS `mdl_portfolio_log`;

CREATE TABLE `mdl_portfolio_log` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL,
  `time` bigint(10) NOT NULL,
  `portfolio` bigint(10) NOT NULL,
  `caller_class` varchar(150) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `caller_file` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `caller_component` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `caller_sha1` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `tempdataid` bigint(10) NOT NULL DEFAULT '0',
  `returnurl` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `continueurl` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_portlog_use_ix` (`userid`),
  KEY `mdl_portlog_por_ix` (`portfolio`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='log of portfolio transfers (used to later check for duplicat';

/*Data for the table `mdl_portfolio_log` */

/*Table structure for table `mdl_portfolio_mahara_queue` */

DROP TABLE IF EXISTS `mdl_portfolio_mahara_queue`;

CREATE TABLE `mdl_portfolio_mahara_queue` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `transferid` bigint(10) NOT NULL,
  `token` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_portmahaqueu_tok_ix` (`token`),
  KEY `mdl_portmahaqueu_tra_ix` (`transferid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='maps mahara tokens to transfer ids';

/*Data for the table `mdl_portfolio_mahara_queue` */

/*Table structure for table `mdl_portfolio_tempdata` */

DROP TABLE IF EXISTS `mdl_portfolio_tempdata`;

CREATE TABLE `mdl_portfolio_tempdata` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `data` longtext COLLATE utf8_unicode_ci,
  `expirytime` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `instance` bigint(10) DEFAULT '0',
  `queued` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_porttemp_use_ix` (`userid`),
  KEY `mdl_porttemp_ins_ix` (`instance`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='stores temporary data for portfolio exports. the id of this ';

/*Data for the table `mdl_portfolio_tempdata` */

/*Table structure for table `mdl_post` */

DROP TABLE IF EXISTS `mdl_post`;

CREATE TABLE `mdl_post` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `module` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `groupid` bigint(10) NOT NULL DEFAULT '0',
  `moduleid` bigint(10) NOT NULL DEFAULT '0',
  `coursemoduleid` bigint(10) NOT NULL DEFAULT '0',
  `subject` varchar(128) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `summary` longtext COLLATE utf8_unicode_ci,
  `content` longtext COLLATE utf8_unicode_ci,
  `uniquehash` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `rating` bigint(10) NOT NULL DEFAULT '0',
  `format` bigint(10) NOT NULL DEFAULT '0',
  `summaryformat` tinyint(2) NOT NULL DEFAULT '0',
  `attachment` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `publishstate` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'draft',
  `lastmodified` bigint(10) NOT NULL DEFAULT '0',
  `created` bigint(10) NOT NULL DEFAULT '0',
  `usermodified` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_post_iduse_uix` (`id`,`userid`),
  KEY `mdl_post_las_ix` (`lastmodified`),
  KEY `mdl_post_mod_ix` (`module`),
  KEY `mdl_post_sub_ix` (`subject`),
  KEY `mdl_post_use_ix` (`usermodified`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Generic post table to hold data blog entries etc in differen';

/*Data for the table `mdl_post` */

/*Table structure for table `mdl_profiling` */

DROP TABLE IF EXISTS `mdl_profiling`;

CREATE TABLE `mdl_profiling` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `runid` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `url` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `data` longtext COLLATE utf8_unicode_ci NOT NULL,
  `totalexecutiontime` bigint(10) NOT NULL,
  `totalcputime` bigint(10) NOT NULL,
  `totalcalls` bigint(10) NOT NULL,
  `totalmemory` bigint(10) NOT NULL,
  `runreference` tinyint(2) NOT NULL DEFAULT '0',
  `runcomment` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timecreated` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_prof_run_uix` (`runid`),
  KEY `mdl_prof_urlrun_ix` (`url`,`runreference`),
  KEY `mdl_prof_timrun_ix` (`timecreated`,`runreference`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores the results of all the profiling runs';

/*Data for the table `mdl_profiling` */

/*Table structure for table `mdl_qtype_essay_options` */

DROP TABLE IF EXISTS `mdl_qtype_essay_options`;

CREATE TABLE `mdl_qtype_essay_options` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `questionid` bigint(10) NOT NULL,
  `responseformat` varchar(16) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'editor',
  `responserequired` tinyint(2) NOT NULL DEFAULT '1',
  `responsefieldlines` smallint(4) NOT NULL DEFAULT '15',
  `attachments` smallint(4) NOT NULL DEFAULT '0',
  `attachmentsrequired` smallint(4) NOT NULL DEFAULT '0',
  `graderinfo` longtext COLLATE utf8_unicode_ci,
  `graderinfoformat` smallint(4) NOT NULL DEFAULT '0',
  `responsetemplate` longtext COLLATE utf8_unicode_ci,
  `responsetemplateformat` smallint(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_qtypessaopti_que_uix` (`questionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Extra options for essay questions.';

/*Data for the table `mdl_qtype_essay_options` */

/*Table structure for table `mdl_qtype_match_options` */

DROP TABLE IF EXISTS `mdl_qtype_match_options`;

CREATE TABLE `mdl_qtype_match_options` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `questionid` bigint(10) NOT NULL DEFAULT '0',
  `shuffleanswers` smallint(4) NOT NULL DEFAULT '1',
  `correctfeedback` longtext COLLATE utf8_unicode_ci NOT NULL,
  `correctfeedbackformat` tinyint(2) NOT NULL DEFAULT '0',
  `partiallycorrectfeedback` longtext COLLATE utf8_unicode_ci NOT NULL,
  `partiallycorrectfeedbackformat` tinyint(2) NOT NULL DEFAULT '0',
  `incorrectfeedback` longtext COLLATE utf8_unicode_ci NOT NULL,
  `incorrectfeedbackformat` tinyint(2) NOT NULL DEFAULT '0',
  `shownumcorrect` tinyint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_qtypmatcopti_que_uix` (`questionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines the question-type specific options for matching ques';

/*Data for the table `mdl_qtype_match_options` */

/*Table structure for table `mdl_qtype_match_subquestions` */

DROP TABLE IF EXISTS `mdl_qtype_match_subquestions`;

CREATE TABLE `mdl_qtype_match_subquestions` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `questionid` bigint(10) NOT NULL DEFAULT '0',
  `questiontext` longtext COLLATE utf8_unicode_ci NOT NULL,
  `questiontextformat` tinyint(2) NOT NULL DEFAULT '0',
  `answertext` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_qtypmatcsubq_que_ix` (`questionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='The subquestions that make up a matching question';

/*Data for the table `mdl_qtype_match_subquestions` */

/*Table structure for table `mdl_qtype_multichoice_options` */

DROP TABLE IF EXISTS `mdl_qtype_multichoice_options`;

CREATE TABLE `mdl_qtype_multichoice_options` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `questionid` bigint(10) NOT NULL DEFAULT '0',
  `layout` smallint(4) NOT NULL DEFAULT '0',
  `single` smallint(4) NOT NULL DEFAULT '0',
  `shuffleanswers` smallint(4) NOT NULL DEFAULT '1',
  `correctfeedback` longtext COLLATE utf8_unicode_ci NOT NULL,
  `correctfeedbackformat` tinyint(2) NOT NULL DEFAULT '0',
  `partiallycorrectfeedback` longtext COLLATE utf8_unicode_ci NOT NULL,
  `partiallycorrectfeedbackformat` tinyint(2) NOT NULL DEFAULT '0',
  `incorrectfeedback` longtext COLLATE utf8_unicode_ci NOT NULL,
  `incorrectfeedbackformat` tinyint(2) NOT NULL DEFAULT '0',
  `answernumbering` varchar(10) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'abc',
  `shownumcorrect` tinyint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_qtypmultopti_que_uix` (`questionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Options for multiple choice questions';

/*Data for the table `mdl_qtype_multichoice_options` */

/*Table structure for table `mdl_qtype_randomsamatch_options` */

DROP TABLE IF EXISTS `mdl_qtype_randomsamatch_options`;

CREATE TABLE `mdl_qtype_randomsamatch_options` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `questionid` bigint(10) NOT NULL DEFAULT '0',
  `choose` bigint(10) NOT NULL DEFAULT '4',
  `subcats` tinyint(2) NOT NULL DEFAULT '1',
  `correctfeedback` longtext COLLATE utf8_unicode_ci NOT NULL,
  `correctfeedbackformat` tinyint(2) NOT NULL DEFAULT '0',
  `partiallycorrectfeedback` longtext COLLATE utf8_unicode_ci NOT NULL,
  `partiallycorrectfeedbackformat` tinyint(2) NOT NULL DEFAULT '0',
  `incorrectfeedback` longtext COLLATE utf8_unicode_ci NOT NULL,
  `incorrectfeedbackformat` tinyint(2) NOT NULL DEFAULT '0',
  `shownumcorrect` tinyint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_qtyprandopti_que_uix` (`questionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Info about a random short-answer matching question';

/*Data for the table `mdl_qtype_randomsamatch_options` */

/*Table structure for table `mdl_qtype_shortanswer_options` */

DROP TABLE IF EXISTS `mdl_qtype_shortanswer_options`;

CREATE TABLE `mdl_qtype_shortanswer_options` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `questionid` bigint(10) NOT NULL DEFAULT '0',
  `usecase` tinyint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_qtypshoropti_que_uix` (`questionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Options for short answer questions';

/*Data for the table `mdl_qtype_shortanswer_options` */

/*Table structure for table `mdl_question` */

DROP TABLE IF EXISTS `mdl_question`;

CREATE TABLE `mdl_question` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `category` bigint(10) NOT NULL DEFAULT '0',
  `parent` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `questiontext` longtext COLLATE utf8_unicode_ci NOT NULL,
  `questiontextformat` tinyint(2) NOT NULL DEFAULT '0',
  `generalfeedback` longtext COLLATE utf8_unicode_ci NOT NULL,
  `generalfeedbackformat` tinyint(2) NOT NULL DEFAULT '0',
  `defaultmark` decimal(12,7) NOT NULL DEFAULT '1.0000000',
  `penalty` decimal(12,7) NOT NULL DEFAULT '0.3333333',
  `qtype` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `length` bigint(10) NOT NULL DEFAULT '1',
  `stamp` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `version` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `hidden` tinyint(1) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `createdby` bigint(10) DEFAULT NULL,
  `modifiedby` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_ques_cat_ix` (`category`),
  KEY `mdl_ques_par_ix` (`parent`),
  KEY `mdl_ques_cre_ix` (`createdby`),
  KEY `mdl_ques_mod_ix` (`modifiedby`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='The questions themselves';

/*Data for the table `mdl_question` */

/*Table structure for table `mdl_question_answers` */

DROP TABLE IF EXISTS `mdl_question_answers`;

CREATE TABLE `mdl_question_answers` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `question` bigint(10) NOT NULL DEFAULT '0',
  `answer` longtext COLLATE utf8_unicode_ci NOT NULL,
  `answerformat` tinyint(2) NOT NULL DEFAULT '0',
  `fraction` decimal(12,7) NOT NULL DEFAULT '0.0000000',
  `feedback` longtext COLLATE utf8_unicode_ci NOT NULL,
  `feedbackformat` tinyint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_quesansw_que_ix` (`question`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Answers, with a fractional grade (0-1) and feedback';

/*Data for the table `mdl_question_answers` */

/*Table structure for table `mdl_question_attempt_step_data` */

DROP TABLE IF EXISTS `mdl_question_attempt_step_data`;

CREATE TABLE `mdl_question_attempt_step_data` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `attemptstepid` bigint(10) NOT NULL,
  `name` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_quesattestepdata_attna_uix` (`attemptstepid`,`name`),
  KEY `mdl_quesattestepdata_att_ix` (`attemptstepid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Each question_attempt_step has an associative array of the d';

/*Data for the table `mdl_question_attempt_step_data` */

/*Table structure for table `mdl_question_attempt_steps` */

DROP TABLE IF EXISTS `mdl_question_attempt_steps`;

CREATE TABLE `mdl_question_attempt_steps` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `questionattemptid` bigint(10) NOT NULL,
  `sequencenumber` bigint(10) NOT NULL,
  `state` varchar(13) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `fraction` decimal(12,7) DEFAULT NULL,
  `timecreated` bigint(10) NOT NULL,
  `userid` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_quesattestep_queseq_uix` (`questionattemptid`,`sequencenumber`),
  KEY `mdl_quesattestep_que_ix` (`questionattemptid`),
  KEY `mdl_quesattestep_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores one step in in a question attempt. As well as the dat';

/*Data for the table `mdl_question_attempt_steps` */

/*Table structure for table `mdl_question_attempts` */

DROP TABLE IF EXISTS `mdl_question_attempts`;

CREATE TABLE `mdl_question_attempts` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `questionusageid` bigint(10) NOT NULL,
  `slot` bigint(10) NOT NULL,
  `behaviour` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `questionid` bigint(10) NOT NULL,
  `variant` bigint(10) NOT NULL DEFAULT '1',
  `maxmark` decimal(12,7) NOT NULL,
  `minfraction` decimal(12,7) NOT NULL,
  `maxfraction` decimal(12,7) NOT NULL DEFAULT '1.0000000',
  `flagged` tinyint(1) NOT NULL DEFAULT '0',
  `questionsummary` longtext COLLATE utf8_unicode_ci,
  `rightanswer` longtext COLLATE utf8_unicode_ci,
  `responsesummary` longtext COLLATE utf8_unicode_ci,
  `timemodified` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_quesatte_queslo_uix` (`questionusageid`,`slot`),
  KEY `mdl_quesatte_beh_ix` (`behaviour`),
  KEY `mdl_quesatte_que_ix` (`questionid`),
  KEY `mdl_quesatte_que2_ix` (`questionusageid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Each row here corresponds to an attempt at one question, as ';

/*Data for the table `mdl_question_attempts` */

/*Table structure for table `mdl_question_calculated` */

DROP TABLE IF EXISTS `mdl_question_calculated`;

CREATE TABLE `mdl_question_calculated` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `question` bigint(10) NOT NULL DEFAULT '0',
  `answer` bigint(10) NOT NULL DEFAULT '0',
  `tolerance` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '0.0',
  `tolerancetype` bigint(10) NOT NULL DEFAULT '1',
  `correctanswerlength` bigint(10) NOT NULL DEFAULT '2',
  `correctanswerformat` bigint(10) NOT NULL DEFAULT '2',
  PRIMARY KEY (`id`),
  KEY `mdl_quescalc_ans_ix` (`answer`),
  KEY `mdl_quescalc_que_ix` (`question`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Options for questions of type calculated';

/*Data for the table `mdl_question_calculated` */

/*Table structure for table `mdl_question_calculated_options` */

DROP TABLE IF EXISTS `mdl_question_calculated_options`;

CREATE TABLE `mdl_question_calculated_options` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `question` bigint(10) NOT NULL DEFAULT '0',
  `synchronize` tinyint(2) NOT NULL DEFAULT '0',
  `single` smallint(4) NOT NULL DEFAULT '0',
  `shuffleanswers` smallint(4) NOT NULL DEFAULT '0',
  `correctfeedback` longtext COLLATE utf8_unicode_ci,
  `correctfeedbackformat` tinyint(2) NOT NULL DEFAULT '0',
  `partiallycorrectfeedback` longtext COLLATE utf8_unicode_ci,
  `partiallycorrectfeedbackformat` tinyint(2) NOT NULL DEFAULT '0',
  `incorrectfeedback` longtext COLLATE utf8_unicode_ci,
  `incorrectfeedbackformat` tinyint(2) NOT NULL DEFAULT '0',
  `answernumbering` varchar(10) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'abc',
  `shownumcorrect` tinyint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_quescalcopti_que_ix` (`question`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Options for questions of type calculated';

/*Data for the table `mdl_question_calculated_options` */

/*Table structure for table `mdl_question_categories` */

DROP TABLE IF EXISTS `mdl_question_categories`;

CREATE TABLE `mdl_question_categories` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `contextid` bigint(10) NOT NULL DEFAULT '0',
  `info` longtext COLLATE utf8_unicode_ci NOT NULL,
  `infoformat` tinyint(2) NOT NULL DEFAULT '0',
  `stamp` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `parent` bigint(10) NOT NULL DEFAULT '0',
  `sortorder` bigint(10) NOT NULL DEFAULT '999',
  PRIMARY KEY (`id`),
  KEY `mdl_quescate_con_ix` (`contextid`),
  KEY `mdl_quescate_par_ix` (`parent`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Categories are for grouping questions';

/*Data for the table `mdl_question_categories` */

/*Table structure for table `mdl_question_dataset_definitions` */

DROP TABLE IF EXISTS `mdl_question_dataset_definitions`;

CREATE TABLE `mdl_question_dataset_definitions` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `category` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `type` bigint(10) NOT NULL DEFAULT '0',
  `options` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `itemcount` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_quesdatadefi_cat_ix` (`category`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Organises and stores properties for dataset items';

/*Data for the table `mdl_question_dataset_definitions` */

/*Table structure for table `mdl_question_dataset_items` */

DROP TABLE IF EXISTS `mdl_question_dataset_items`;

CREATE TABLE `mdl_question_dataset_items` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `definition` bigint(10) NOT NULL DEFAULT '0',
  `itemnumber` bigint(10) NOT NULL DEFAULT '0',
  `value` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_quesdataitem_def_ix` (`definition`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Individual dataset items';

/*Data for the table `mdl_question_dataset_items` */

/*Table structure for table `mdl_question_datasets` */

DROP TABLE IF EXISTS `mdl_question_datasets`;

CREATE TABLE `mdl_question_datasets` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `question` bigint(10) NOT NULL DEFAULT '0',
  `datasetdefinition` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_quesdata_quedat_ix` (`question`,`datasetdefinition`),
  KEY `mdl_quesdata_que_ix` (`question`),
  KEY `mdl_quesdata_dat_ix` (`datasetdefinition`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Many-many relation between questions and dataset definitions';

/*Data for the table `mdl_question_datasets` */

/*Table structure for table `mdl_question_hints` */

DROP TABLE IF EXISTS `mdl_question_hints`;

CREATE TABLE `mdl_question_hints` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `questionid` bigint(10) NOT NULL,
  `hint` longtext COLLATE utf8_unicode_ci NOT NULL,
  `hintformat` smallint(4) NOT NULL DEFAULT '0',
  `shownumcorrect` tinyint(1) DEFAULT NULL,
  `clearwrong` tinyint(1) DEFAULT NULL,
  `options` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_queshint_que_ix` (`questionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores the the part of the question definition that gives di';

/*Data for the table `mdl_question_hints` */

/*Table structure for table `mdl_question_multianswer` */

DROP TABLE IF EXISTS `mdl_question_multianswer`;

CREATE TABLE `mdl_question_multianswer` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `question` bigint(10) NOT NULL DEFAULT '0',
  `sequence` longtext COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_quesmult_que_ix` (`question`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Options for multianswer questions';

/*Data for the table `mdl_question_multianswer` */

/*Table structure for table `mdl_question_numerical` */

DROP TABLE IF EXISTS `mdl_question_numerical`;

CREATE TABLE `mdl_question_numerical` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `question` bigint(10) NOT NULL DEFAULT '0',
  `answer` bigint(10) NOT NULL DEFAULT '0',
  `tolerance` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '0.0',
  PRIMARY KEY (`id`),
  KEY `mdl_quesnume_ans_ix` (`answer`),
  KEY `mdl_quesnume_que_ix` (`question`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Options for numerical questions.';

/*Data for the table `mdl_question_numerical` */

/*Table structure for table `mdl_question_numerical_options` */

DROP TABLE IF EXISTS `mdl_question_numerical_options`;

CREATE TABLE `mdl_question_numerical_options` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `question` bigint(10) NOT NULL DEFAULT '0',
  `showunits` smallint(4) NOT NULL DEFAULT '0',
  `unitsleft` smallint(4) NOT NULL DEFAULT '0',
  `unitgradingtype` smallint(4) NOT NULL DEFAULT '0',
  `unitpenalty` decimal(12,7) NOT NULL DEFAULT '0.1000000',
  PRIMARY KEY (`id`),
  KEY `mdl_quesnumeopti_que_ix` (`question`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Options for questions of type numerical This table is also u';

/*Data for the table `mdl_question_numerical_options` */

/*Table structure for table `mdl_question_numerical_units` */

DROP TABLE IF EXISTS `mdl_question_numerical_units`;

CREATE TABLE `mdl_question_numerical_units` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `question` bigint(10) NOT NULL DEFAULT '0',
  `multiplier` decimal(40,20) NOT NULL DEFAULT '1.00000000000000000000',
  `unit` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_quesnumeunit_queuni_uix` (`question`,`unit`),
  KEY `mdl_quesnumeunit_que_ix` (`question`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Optional unit options for numerical questions. This table is';

/*Data for the table `mdl_question_numerical_units` */

/*Table structure for table `mdl_question_response_analysis` */

DROP TABLE IF EXISTS `mdl_question_response_analysis`;

CREATE TABLE `mdl_question_response_analysis` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `hashcode` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `whichtries` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timemodified` bigint(10) NOT NULL,
  `questionid` bigint(10) NOT NULL,
  `variant` bigint(10) DEFAULT NULL,
  `subqid` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `aid` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `response` longtext COLLATE utf8_unicode_ci,
  `credit` decimal(15,5) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Analysis of student responses given to questions.';

/*Data for the table `mdl_question_response_analysis` */

/*Table structure for table `mdl_question_response_count` */

DROP TABLE IF EXISTS `mdl_question_response_count`;

CREATE TABLE `mdl_question_response_count` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `analysisid` bigint(10) NOT NULL,
  `try` bigint(10) NOT NULL,
  `rcount` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_quesrespcoun_ana_ix` (`analysisid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Count for each responses for each try at a question.';

/*Data for the table `mdl_question_response_count` */

/*Table structure for table `mdl_question_statistics` */

DROP TABLE IF EXISTS `mdl_question_statistics`;

CREATE TABLE `mdl_question_statistics` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `hashcode` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timemodified` bigint(10) NOT NULL,
  `questionid` bigint(10) NOT NULL,
  `slot` bigint(10) DEFAULT NULL,
  `subquestion` smallint(4) NOT NULL,
  `variant` bigint(10) DEFAULT NULL,
  `s` bigint(10) NOT NULL DEFAULT '0',
  `effectiveweight` decimal(15,5) DEFAULT NULL,
  `negcovar` tinyint(2) NOT NULL DEFAULT '0',
  `discriminationindex` decimal(15,5) DEFAULT NULL,
  `discriminativeefficiency` decimal(15,5) DEFAULT NULL,
  `sd` decimal(15,10) DEFAULT NULL,
  `facility` decimal(15,10) DEFAULT NULL,
  `subquestions` longtext COLLATE utf8_unicode_ci,
  `maxmark` decimal(12,7) DEFAULT NULL,
  `positions` longtext COLLATE utf8_unicode_ci,
  `randomguessscore` decimal(12,7) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Statistics for individual questions used in an activity.';

/*Data for the table `mdl_question_statistics` */

/*Table structure for table `mdl_question_truefalse` */

DROP TABLE IF EXISTS `mdl_question_truefalse`;

CREATE TABLE `mdl_question_truefalse` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `question` bigint(10) NOT NULL DEFAULT '0',
  `trueanswer` bigint(10) NOT NULL DEFAULT '0',
  `falseanswer` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_questrue_que_ix` (`question`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Options for True-False questions';

/*Data for the table `mdl_question_truefalse` */

/*Table structure for table `mdl_question_usages` */

DROP TABLE IF EXISTS `mdl_question_usages`;

CREATE TABLE `mdl_question_usages` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `contextid` bigint(10) NOT NULL,
  `component` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `preferredbehaviour` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_quesusag_con_ix` (`contextid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table''s main purpose it to assign a unique id to each a';

/*Data for the table `mdl_question_usages` */

/*Table structure for table `mdl_quiz` */

DROP TABLE IF EXISTS `mdl_quiz`;

CREATE TABLE `mdl_quiz` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci NOT NULL,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `timeopen` bigint(10) NOT NULL DEFAULT '0',
  `timeclose` bigint(10) NOT NULL DEFAULT '0',
  `timelimit` bigint(10) NOT NULL DEFAULT '0',
  `overduehandling` varchar(16) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'autoabandon',
  `graceperiod` bigint(10) NOT NULL DEFAULT '0',
  `preferredbehaviour` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `attempts` mediumint(6) NOT NULL DEFAULT '0',
  `attemptonlast` smallint(4) NOT NULL DEFAULT '0',
  `grademethod` smallint(4) NOT NULL DEFAULT '1',
  `decimalpoints` smallint(4) NOT NULL DEFAULT '2',
  `questiondecimalpoints` smallint(4) NOT NULL DEFAULT '-1',
  `reviewattempt` mediumint(6) NOT NULL DEFAULT '0',
  `reviewcorrectness` mediumint(6) NOT NULL DEFAULT '0',
  `reviewmarks` mediumint(6) NOT NULL DEFAULT '0',
  `reviewspecificfeedback` mediumint(6) NOT NULL DEFAULT '0',
  `reviewgeneralfeedback` mediumint(6) NOT NULL DEFAULT '0',
  `reviewrightanswer` mediumint(6) NOT NULL DEFAULT '0',
  `reviewoverallfeedback` mediumint(6) NOT NULL DEFAULT '0',
  `questionsperpage` bigint(10) NOT NULL DEFAULT '0',
  `navmethod` varchar(16) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'free',
  `shufflequestions` smallint(4) NOT NULL DEFAULT '0',
  `shuffleanswers` smallint(4) NOT NULL DEFAULT '0',
  `sumgrades` decimal(10,5) NOT NULL DEFAULT '0.00000',
  `grade` decimal(10,5) NOT NULL DEFAULT '0.00000',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `password` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `subnet` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `browsersecurity` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `delay1` bigint(10) NOT NULL DEFAULT '0',
  `delay2` bigint(10) NOT NULL DEFAULT '0',
  `showuserpicture` smallint(4) NOT NULL DEFAULT '0',
  `showblocks` smallint(4) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_quiz_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='The settings for each quiz.';

/*Data for the table `mdl_quiz` */

/*Table structure for table `mdl_quiz_attempts` */

DROP TABLE IF EXISTS `mdl_quiz_attempts`;

CREATE TABLE `mdl_quiz_attempts` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `quiz` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `attempt` mediumint(6) NOT NULL DEFAULT '0',
  `uniqueid` bigint(10) NOT NULL DEFAULT '0',
  `layout` longtext COLLATE utf8_unicode_ci NOT NULL,
  `currentpage` bigint(10) NOT NULL DEFAULT '0',
  `preview` smallint(3) NOT NULL DEFAULT '0',
  `state` varchar(16) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'inprogress',
  `timestart` bigint(10) NOT NULL DEFAULT '0',
  `timefinish` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `timecheckstate` bigint(10) DEFAULT '0',
  `sumgrades` decimal(10,5) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_quizatte_quiuseatt_uix` (`quiz`,`userid`,`attempt`),
  UNIQUE KEY `mdl_quizatte_uni_uix` (`uniqueid`),
  KEY `mdl_quizatte_statim_ix` (`state`,`timecheckstate`),
  KEY `mdl_quizatte_qui_ix` (`quiz`),
  KEY `mdl_quizatte_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores users attempts at quizzes.';

/*Data for the table `mdl_quiz_attempts` */

/*Table structure for table `mdl_quiz_feedback` */

DROP TABLE IF EXISTS `mdl_quiz_feedback`;

CREATE TABLE `mdl_quiz_feedback` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `quizid` bigint(10) NOT NULL DEFAULT '0',
  `feedbacktext` longtext COLLATE utf8_unicode_ci NOT NULL,
  `feedbacktextformat` tinyint(2) NOT NULL DEFAULT '0',
  `mingrade` decimal(10,5) NOT NULL DEFAULT '0.00000',
  `maxgrade` decimal(10,5) NOT NULL DEFAULT '0.00000',
  PRIMARY KEY (`id`),
  KEY `mdl_quizfeed_qui_ix` (`quizid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Feedback given to students based on which grade band their o';

/*Data for the table `mdl_quiz_feedback` */

/*Table structure for table `mdl_quiz_grades` */

DROP TABLE IF EXISTS `mdl_quiz_grades`;

CREATE TABLE `mdl_quiz_grades` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `quiz` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `grade` decimal(10,5) NOT NULL DEFAULT '0.00000',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_quizgrad_use_ix` (`userid`),
  KEY `mdl_quizgrad_qui_ix` (`quiz`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores the overall grade for each user on the quiz, based on';

/*Data for the table `mdl_quiz_grades` */

/*Table structure for table `mdl_quiz_overrides` */

DROP TABLE IF EXISTS `mdl_quiz_overrides`;

CREATE TABLE `mdl_quiz_overrides` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `quiz` bigint(10) NOT NULL DEFAULT '0',
  `groupid` bigint(10) DEFAULT NULL,
  `userid` bigint(10) DEFAULT NULL,
  `timeopen` bigint(10) DEFAULT NULL,
  `timeclose` bigint(10) DEFAULT NULL,
  `timelimit` bigint(10) DEFAULT NULL,
  `attempts` mediumint(6) DEFAULT NULL,
  `password` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_quizover_qui_ix` (`quiz`),
  KEY `mdl_quizover_gro_ix` (`groupid`),
  KEY `mdl_quizover_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='The overrides to quiz settings on a per-user and per-group b';

/*Data for the table `mdl_quiz_overrides` */

/*Table structure for table `mdl_quiz_overview_regrades` */

DROP TABLE IF EXISTS `mdl_quiz_overview_regrades`;

CREATE TABLE `mdl_quiz_overview_regrades` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `questionusageid` bigint(10) NOT NULL,
  `slot` bigint(10) NOT NULL,
  `newfraction` decimal(12,7) DEFAULT NULL,
  `oldfraction` decimal(12,7) DEFAULT NULL,
  `regraded` smallint(4) NOT NULL,
  `timemodified` bigint(10) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table records which question attempts need regrading an';

/*Data for the table `mdl_quiz_overview_regrades` */

/*Table structure for table `mdl_quiz_reports` */

DROP TABLE IF EXISTS `mdl_quiz_reports`;

CREATE TABLE `mdl_quiz_reports` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `displayorder` bigint(10) NOT NULL,
  `capability` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_quizrepo_nam_uix` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Lists all the installed quiz reports and their display order';

/*Data for the table `mdl_quiz_reports` */

insert  into `mdl_quiz_reports`(`id`,`name`,`displayorder`,`capability`) values (1,'grading',6000,'mod/quiz:grade'),(2,'overview',10000,NULL),(3,'responses',9000,NULL),(4,'statistics',8000,'quiz/statistics:view');

/*Table structure for table `mdl_quiz_slots` */

DROP TABLE IF EXISTS `mdl_quiz_slots`;

CREATE TABLE `mdl_quiz_slots` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `slot` bigint(10) NOT NULL,
  `quizid` bigint(10) NOT NULL DEFAULT '0',
  `page` bigint(10) NOT NULL,
  `questionid` bigint(10) NOT NULL DEFAULT '0',
  `maxmark` decimal(12,7) NOT NULL DEFAULT '0.0000000',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_quizslot_quislo_uix` (`quizid`,`slot`),
  KEY `mdl_quizslot_qui_ix` (`quizid`),
  KEY `mdl_quizslot_que_ix` (`questionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores the question used in a quiz, with the order, and for ';

/*Data for the table `mdl_quiz_slots` */

/*Table structure for table `mdl_quiz_statistics` */

DROP TABLE IF EXISTS `mdl_quiz_statistics`;

CREATE TABLE `mdl_quiz_statistics` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `hashcode` varchar(40) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `whichattempts` smallint(4) NOT NULL,
  `timemodified` bigint(10) NOT NULL,
  `firstattemptscount` bigint(10) NOT NULL,
  `highestattemptscount` bigint(10) NOT NULL,
  `lastattemptscount` bigint(10) NOT NULL,
  `allattemptscount` bigint(10) NOT NULL,
  `firstattemptsavg` decimal(15,5) DEFAULT NULL,
  `highestattemptsavg` decimal(15,5) DEFAULT NULL,
  `lastattemptsavg` decimal(15,5) DEFAULT NULL,
  `allattemptsavg` decimal(15,5) DEFAULT NULL,
  `median` decimal(15,5) DEFAULT NULL,
  `standarddeviation` decimal(15,5) DEFAULT NULL,
  `skewness` decimal(15,10) DEFAULT NULL,
  `kurtosis` decimal(15,5) DEFAULT NULL,
  `cic` decimal(15,10) DEFAULT NULL,
  `errorratio` decimal(15,10) DEFAULT NULL,
  `standarderror` decimal(15,10) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='table to cache results from analysis done in statistics repo';

/*Data for the table `mdl_quiz_statistics` */

/*Table structure for table `mdl_rating` */

DROP TABLE IF EXISTS `mdl_rating`;

CREATE TABLE `mdl_rating` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `contextid` bigint(10) NOT NULL,
  `component` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `ratingarea` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `itemid` bigint(10) NOT NULL,
  `scaleid` bigint(10) NOT NULL,
  `rating` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `timecreated` bigint(10) NOT NULL,
  `timemodified` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_rati_comratconite_ix` (`component`,`ratingarea`,`contextid`,`itemid`),
  KEY `mdl_rati_con_ix` (`contextid`),
  KEY `mdl_rati_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='moodle ratings';

/*Data for the table `mdl_rating` */

/*Table structure for table `mdl_registration_hubs` */

DROP TABLE IF EXISTS `mdl_registration_hubs`;

CREATE TABLE `mdl_registration_hubs` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `token` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `hubname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `huburl` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `confirmed` tinyint(1) NOT NULL DEFAULT '0',
  `secret` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='hub where the site is registered on with their associated to';

/*Data for the table `mdl_registration_hubs` */

/*Table structure for table `mdl_repository` */

DROP TABLE IF EXISTS `mdl_repository`;

CREATE TABLE `mdl_repository` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `type` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `visible` tinyint(1) DEFAULT '1',
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table contains one entry for every configured external ';

/*Data for the table `mdl_repository` */

insert  into `mdl_repository`(`id`,`type`,`visible`,`sortorder`) values (1,'areafiles',1,1),(2,'local',1,2),(3,'recent',1,3),(4,'upload',1,4),(5,'url',1,5),(6,'user',1,6),(7,'wikimedia',1,7),(8,'youtube',1,8);

/*Table structure for table `mdl_repository_elisfiles_cats` */

DROP TABLE IF EXISTS `mdl_repository_elisfiles_cats`;

CREATE TABLE `mdl_repository_elisfiles_cats` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `parent` bigint(10) NOT NULL DEFAULT '0',
  `uuid` varchar(36) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `path` longtext COLLATE utf8_unicode_ci NOT NULL,
  `title` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_repoeliscats_uui_uix` (`uuid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Store ELIS Files categories';

/*Data for the table `mdl_repository_elisfiles_cats` */

/*Table structure for table `mdl_repository_elisfiles_course` */

DROP TABLE IF EXISTS `mdl_repository_elisfiles_course`;

CREATE TABLE `mdl_repository_elisfiles_course` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `uuid` varchar(36) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_repoeliscour_couuui_uix` (`courseid`,`uuid`),
  KEY `mdl_repoeliscour_cou_ix` (`courseid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores course storage UUID values';

/*Data for the table `mdl_repository_elisfiles_course` */

/*Table structure for table `mdl_repository_elisfiles_userset` */

DROP TABLE IF EXISTS `mdl_repository_elisfiles_userset`;

CREATE TABLE `mdl_repository_elisfiles_userset` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `usersetid` bigint(10) NOT NULL DEFAULT '0',
  `uuid` varchar(36) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_repoelisuser_useuui_uix` (`usersetid`,`uuid`),
  KEY `mdl_repoelisuser_use_ix` (`usersetid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores userset shared storage UUID values';

/*Data for the table `mdl_repository_elisfiles_userset` */

/*Table structure for table `mdl_repository_instance_config` */

DROP TABLE IF EXISTS `mdl_repository_instance_config`;

CREATE TABLE `mdl_repository_instance_config` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `instanceid` bigint(10) NOT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='The config for intances';

/*Data for the table `mdl_repository_instance_config` */

/*Table structure for table `mdl_repository_instances` */

DROP TABLE IF EXISTS `mdl_repository_instances`;

CREATE TABLE `mdl_repository_instances` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `typeid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `contextid` bigint(10) NOT NULL,
  `username` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `password` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `timecreated` bigint(10) DEFAULT NULL,
  `timemodified` bigint(10) DEFAULT NULL,
  `readonly` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table contains one entry for every configured external ';

/*Data for the table `mdl_repository_instances` */

insert  into `mdl_repository_instances`(`id`,`name`,`typeid`,`userid`,`contextid`,`username`,`password`,`timecreated`,`timemodified`,`readonly`) values (1,'',1,0,1,NULL,NULL,1413857018,1413857018,0),(2,'',2,0,1,NULL,NULL,1413857024,1413857024,0),(3,'',3,0,1,NULL,NULL,1413857026,1413857026,0),(4,'',4,0,1,NULL,NULL,1413857028,1413857028,0),(5,'',5,0,1,NULL,NULL,1413857029,1413857029,0),(6,'',6,0,1,NULL,NULL,1413857030,1413857030,0),(7,'',7,0,1,NULL,NULL,1413857032,1413857032,0),(8,'',8,0,1,NULL,NULL,1413857033,1413857033,0);

/*Table structure for table `mdl_resource` */

DROP TABLE IF EXISTS `mdl_resource`;

CREATE TABLE `mdl_resource` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `tobemigrated` smallint(4) NOT NULL DEFAULT '0',
  `legacyfiles` smallint(4) NOT NULL DEFAULT '0',
  `legacyfileslast` bigint(10) DEFAULT NULL,
  `display` smallint(4) NOT NULL DEFAULT '0',
  `displayoptions` longtext COLLATE utf8_unicode_ci,
  `filterfiles` smallint(4) NOT NULL DEFAULT '0',
  `revision` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_reso_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Each record is one resource and its config data';

/*Data for the table `mdl_resource` */

/*Table structure for table `mdl_resource_old` */

DROP TABLE IF EXISTS `mdl_resource_old`;

CREATE TABLE `mdl_resource_old` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `type` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `reference` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `alltext` longtext COLLATE utf8_unicode_ci NOT NULL,
  `popup` longtext COLLATE utf8_unicode_ci NOT NULL,
  `options` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `oldid` bigint(10) NOT NULL,
  `cmid` bigint(10) DEFAULT NULL,
  `newmodule` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `newid` bigint(10) DEFAULT NULL,
  `migrated` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_resoold_old_uix` (`oldid`),
  KEY `mdl_resoold_cmi_ix` (`cmid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='backup of all old resource instances from 1.9';

/*Data for the table `mdl_resource_old` */

/*Table structure for table `mdl_role` */

DROP TABLE IF EXISTS `mdl_role`;

CREATE TABLE `mdl_role` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `shortname` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci NOT NULL,
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  `archetype` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_role_sor_uix` (`sortorder`),
  UNIQUE KEY `mdl_role_sho_uix` (`shortname`)
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='moodle roles';

/*Data for the table `mdl_role` */

insert  into `mdl_role`(`id`,`name`,`shortname`,`description`,`sortorder`,`archetype`) values (1,'Super Admin','1','<p>Super Admin<br></p>',1,'manager'),(3,'Teacher','145','<p>Teacher<br></p>',3,'editingteacher'),(6,'','guest','',6,'guest'),(7,'','user','',7,'user'),(9,'Program Administrator','curriculumadmin','Manage all program functions.',9,''),(10,'Content Personnel','5','',10,''),(14,'Statisticians','10','',11,''),(15,'Administrative Personnel','15','',12,''),(16,'Intervention Manager','20','',13,''),(17,'Video Coding Analyst','25','',14,''),(18,'Intervention Support Personnel','30','',15,''),(19,'Coordinator','35','',16,''),(20,'Mentor / Coach','40','',17,''),(21,'Auditor','101','',18,''),(22,'Statewide','105','',19,''),(23,'Community / District','110','',20,''),(24,'Community / District Delegate','120','',21,''),(25,'Community / District Specialist','115','',22,''),(26,'Community / District Specialist Delegate','140','',23,''),(27,'School Specialist','130','',24,''),(28,'School Specialist Delegate','142','',25,''),(29,'Principal / Director','125','',26,''),(30,'Principal / Director Delegate','135','',27,'');

/*Table structure for table `mdl_role_allow_assign` */

DROP TABLE IF EXISTS `mdl_role_allow_assign`;

CREATE TABLE `mdl_role_allow_assign` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `roleid` bigint(10) NOT NULL DEFAULT '0',
  `allowassign` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_rolealloassi_rolall_uix` (`roleid`,`allowassign`),
  KEY `mdl_rolealloassi_rol_ix` (`roleid`),
  KEY `mdl_rolealloassi_all_ix` (`allowassign`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='this defines what role can assign what role';

/*Data for the table `mdl_role_allow_assign` */

insert  into `mdl_role_allow_assign`(`id`,`roleid`,`allowassign`) values (1,1,1),(2,1,2),(3,1,3),(4,1,4),(5,1,5),(6,3,4),(7,3,5);

/*Table structure for table `mdl_role_allow_override` */

DROP TABLE IF EXISTS `mdl_role_allow_override`;

CREATE TABLE `mdl_role_allow_override` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `roleid` bigint(10) NOT NULL DEFAULT '0',
  `allowoverride` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_rolealloover_rolall_uix` (`roleid`,`allowoverride`),
  KEY `mdl_rolealloover_rol_ix` (`roleid`),
  KEY `mdl_rolealloover_all_ix` (`allowoverride`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='this defines what role can override what role';

/*Data for the table `mdl_role_allow_override` */

insert  into `mdl_role_allow_override`(`id`,`roleid`,`allowoverride`) values (1,1,1),(2,1,2),(3,1,3),(4,1,4),(5,1,5),(6,1,6),(7,1,7),(8,1,8),(9,3,4),(10,3,5),(11,3,6);

/*Table structure for table `mdl_role_allow_switch` */

DROP TABLE IF EXISTS `mdl_role_allow_switch`;

CREATE TABLE `mdl_role_allow_switch` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `roleid` bigint(10) NOT NULL,
  `allowswitch` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_rolealloswit_rolall_uix` (`roleid`,`allowswitch`),
  KEY `mdl_rolealloswit_rol_ix` (`roleid`),
  KEY `mdl_rolealloswit_all_ix` (`allowswitch`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table stores which which other roles a user is allowed ';

/*Data for the table `mdl_role_allow_switch` */

insert  into `mdl_role_allow_switch`(`id`,`roleid`,`allowswitch`) values (1,1,3),(2,1,4),(3,1,5),(4,1,6),(5,3,4),(6,3,5),(7,3,6),(8,4,5),(9,4,6);

/*Table structure for table `mdl_role_assignments` */

DROP TABLE IF EXISTS `mdl_role_assignments`;

CREATE TABLE `mdl_role_assignments` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `roleid` bigint(10) NOT NULL DEFAULT '0',
  `contextid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `modifierid` bigint(10) NOT NULL DEFAULT '0',
  `component` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `itemid` bigint(10) NOT NULL DEFAULT '0',
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_roleassi_sor_ix` (`sortorder`),
  KEY `mdl_roleassi_rolcon_ix` (`roleid`,`contextid`),
  KEY `mdl_roleassi_useconrol_ix` (`userid`,`contextid`,`roleid`),
  KEY `mdl_roleassi_comiteuse_ix` (`component`,`itemid`,`userid`),
  KEY `mdl_roleassi_rol_ix` (`roleid`),
  KEY `mdl_roleassi_con_ix` (`contextid`),
  KEY `mdl_roleassi_use_ix` (`userid`)
) ENGINE=InnoDB AUTO_INCREMENT=48 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='assigning roles in different context';

/*Data for the table `mdl_role_assignments` */

insert  into `mdl_role_assignments`(`id`,`roleid`,`contextid`,`userid`,`timemodified`,`modifierid`,`component`,`itemid`,`sortorder`) values (47,1,1,4,1413975354,4,'',0,0);

/*Table structure for table `mdl_role_capabilities` */

DROP TABLE IF EXISTS `mdl_role_capabilities`;

CREATE TABLE `mdl_role_capabilities` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `contextid` bigint(10) NOT NULL DEFAULT '0',
  `roleid` bigint(10) NOT NULL DEFAULT '0',
  `capability` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `permission` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `modifierid` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_rolecapa_rolconcap_uix` (`roleid`,`contextid`,`capability`),
  KEY `mdl_rolecapa_rol_ix` (`roleid`),
  KEY `mdl_rolecapa_con_ix` (`contextid`),
  KEY `mdl_rolecapa_mod_ix` (`modifierid`),
  KEY `mdl_rolecapa_cap_ix` (`capability`)
) ENGINE=InnoDB AUTO_INCREMENT=1228 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='permission has to be signed, overriding a capability for a p';

/*Data for the table `mdl_role_capabilities` */

insert  into `mdl_role_capabilities`(`id`,`contextid`,`roleid`,`capability`,`permission`,`timemodified`,`modifierid`) values (1,1,1,'moodle/site:readallmessages',1,1413856724,0),(2,1,3,'moodle/site:readallmessages',1,1413856724,0),(3,1,1,'moodle/site:sendmessage',1,1413856725,0),(4,1,7,'moodle/site:sendmessage',1,1413856725,0),(5,1,1,'moodle/site:approvecourse',1,1413856725,0),(6,1,3,'moodle/backup:backupcourse',1,1413856725,0),(7,1,1,'moodle/backup:backupcourse',1,1413856725,0),(8,1,3,'moodle/backup:backupsection',1,1413856725,0),(9,1,1,'moodle/backup:backupsection',1,1413856725,0),(10,1,3,'moodle/backup:backupactivity',1,1413856725,0),(11,1,1,'moodle/backup:backupactivity',1,1413856725,0),(12,1,3,'moodle/backup:backuptargethub',1,1413856725,0),(13,1,1,'moodle/backup:backuptargethub',1,1413856725,0),(14,1,3,'moodle/backup:backuptargetimport',1,1413856725,0),(15,1,1,'moodle/backup:backuptargetimport',1,1413856726,0),(16,1,3,'moodle/backup:downloadfile',1,1413856726,0),(17,1,1,'moodle/backup:downloadfile',1,1413856726,0),(18,1,3,'moodle/backup:configure',1,1413856726,0),(19,1,1,'moodle/backup:configure',1,1413856726,0),(20,1,1,'moodle/backup:userinfo',1,1413856726,0),(21,1,1,'moodle/backup:anonymise',1,1413856726,0),(22,1,3,'moodle/restore:restorecourse',1,1413856726,0),(23,1,1,'moodle/restore:restorecourse',1,1413856726,0),(24,1,3,'moodle/restore:restoresection',1,1413856726,0),(25,1,1,'moodle/restore:restoresection',1,1413856727,0),(26,1,3,'moodle/restore:restoreactivity',1,1413856727,0),(27,1,1,'moodle/restore:restoreactivity',1,1413856727,0),(28,1,3,'moodle/restore:restoretargethub',1,1413856727,0),(29,1,1,'moodle/restore:restoretargethub',1,1413856727,0),(30,1,3,'moodle/restore:restoretargetimport',1,1413856727,0),(31,1,1,'moodle/restore:restoretargetimport',1,1413856727,0),(32,1,3,'moodle/restore:uploadfile',1,1413856727,0),(33,1,1,'moodle/restore:uploadfile',1,1413856727,0),(34,1,3,'moodle/restore:configure',1,1413856728,0),(35,1,1,'moodle/restore:configure',1,1413856728,0),(36,1,2,'moodle/restore:rolldates',1,1413856728,0),(37,1,1,'moodle/restore:rolldates',1,1413856728,0),(38,1,1,'moodle/restore:userinfo',1,1413856728,0),(39,1,1,'moodle/restore:createuser',1,1413856728,0),(40,1,3,'moodle/site:manageblocks',1,1413856728,0),(41,1,1,'moodle/site:manageblocks',1,1413856728,0),(42,1,4,'moodle/site:accessallgroups',1,1413856728,0),(43,1,3,'moodle/site:accessallgroups',1,1413856728,0),(44,1,1,'moodle/site:accessallgroups',1,1413856728,0),(45,1,4,'moodle/site:viewfullnames',1,1413856728,0),(46,1,3,'moodle/site:viewfullnames',1,1413856729,0),(47,1,1,'moodle/site:viewfullnames',1,1413856729,0),(48,1,4,'moodle/site:viewuseridentity',1,1413856729,0),(49,1,3,'moodle/site:viewuseridentity',1,1413856729,0),(50,1,1,'moodle/site:viewuseridentity',1,1413856729,0),(51,1,4,'moodle/site:viewreports',1,1413856729,0),(52,1,3,'moodle/site:viewreports',1,1413856729,0),(53,1,1,'moodle/site:viewreports',1,1413856729,0),(54,1,3,'moodle/site:trustcontent',1,1413856730,0),(55,1,1,'moodle/site:trustcontent',1,1413856730,0),(56,1,1,'moodle/site:uploadusers',1,1413856730,0),(57,1,3,'moodle/filter:manage',1,1413856730,0),(58,1,1,'moodle/filter:manage',1,1413856730,0),(59,1,1,'moodle/user:create',1,1413856730,0),(60,1,1,'moodle/user:delete',1,1413856730,0),(61,1,1,'moodle/user:update',1,1413856730,0),(62,1,6,'moodle/user:viewdetails',1,1413856730,0),(63,1,5,'moodle/user:viewdetails',1,1413856730,0),(64,1,4,'moodle/user:viewdetails',1,1413856730,0),(65,1,3,'moodle/user:viewdetails',1,1413856731,0),(66,1,1,'moodle/user:viewdetails',1,1413856731,0),(67,1,1,'moodle/user:viewalldetails',1,1413856731,0),(68,1,1,'moodle/user:viewlastip',1,1413856731,0),(69,1,4,'moodle/user:viewhiddendetails',1,1413856731,0),(70,1,3,'moodle/user:viewhiddendetails',1,1413856731,0),(71,1,1,'moodle/user:viewhiddendetails',1,1413856731,0),(72,1,1,'moodle/user:loginas',1,1413856731,0),(73,1,1,'moodle/user:managesyspages',1,1413856731,0),(74,1,7,'moodle/user:manageownblocks',1,1413856731,0),(75,1,7,'moodle/user:manageownfiles',1,1413856731,0),(76,1,1,'moodle/my:configsyspages',1,1413856732,0),(77,1,3,'moodle/role:assign',1,1413856732,0),(78,1,1,'moodle/role:assign',1,1413856732,0),(79,1,4,'moodle/role:review',1,1413856732,0),(80,1,3,'moodle/role:review',1,1413856732,0),(81,1,1,'moodle/role:review',1,1413856732,0),(82,1,1,'moodle/role:override',1,1413856732,0),(83,1,3,'moodle/role:safeoverride',1,1413856732,0),(84,1,1,'moodle/role:manage',1,1413856732,0),(85,1,3,'moodle/role:switchroles',1,1413856733,0),(86,1,1,'moodle/role:switchroles',1,1413856733,0),(87,1,1,'moodle/category:manage',1,1413856733,0),(88,1,2,'moodle/category:viewhiddencategories',1,1413856733,0),(89,1,1,'moodle/category:viewhiddencategories',1,1413856733,0),(90,1,1,'moodle/cohort:manage',1,1413856733,0),(91,1,1,'moodle/cohort:assign',1,1413856733,0),(92,1,3,'moodle/cohort:view',1,1413856733,0),(93,1,1,'moodle/cohort:view',1,1413856734,0),(94,1,2,'moodle/course:create',1,1413856734,0),(95,1,1,'moodle/course:create',1,1413856734,0),(96,1,7,'moodle/course:request',1,1413856734,0),(97,1,1,'moodle/course:delete',1,1413856734,0),(98,1,3,'moodle/course:update',1,1413856734,0),(99,1,1,'moodle/course:update',1,1413856734,0),(100,1,1,'moodle/course:view',1,1413856734,0),(101,1,3,'moodle/course:enrolreview',1,1413856735,0),(102,1,1,'moodle/course:enrolreview',1,1413856735,0),(103,1,3,'moodle/course:enrolconfig',1,1413856735,0),(104,1,1,'moodle/course:enrolconfig',1,1413856735,0),(105,1,3,'moodle/course:reviewotherusers',1,1413856735,0),(106,1,1,'moodle/course:reviewotherusers',1,1413856735,0),(107,1,4,'moodle/course:bulkmessaging',1,1413856735,0),(108,1,3,'moodle/course:bulkmessaging',1,1413856735,0),(109,1,1,'moodle/course:bulkmessaging',1,1413856735,0),(110,1,4,'moodle/course:viewhiddenuserfields',1,1413856735,0),(111,1,3,'moodle/course:viewhiddenuserfields',1,1413856735,0),(112,1,1,'moodle/course:viewhiddenuserfields',1,1413856735,0),(113,1,2,'moodle/course:viewhiddencourses',1,1413856735,0),(114,1,4,'moodle/course:viewhiddencourses',1,1413856736,0),(115,1,3,'moodle/course:viewhiddencourses',1,1413856736,0),(116,1,1,'moodle/course:viewhiddencourses',1,1413856736,0),(117,1,3,'moodle/course:visibility',1,1413856736,0),(118,1,1,'moodle/course:visibility',1,1413856736,0),(119,1,3,'moodle/course:managefiles',1,1413856736,0),(120,1,1,'moodle/course:managefiles',1,1413856736,0),(121,1,3,'moodle/course:manageactivities',1,1413856736,0),(122,1,1,'moodle/course:manageactivities',1,1413856736,0),(123,1,3,'moodle/course:activityvisibility',1,1413856736,0),(124,1,1,'moodle/course:activityvisibility',1,1413856736,0),(125,1,4,'moodle/course:viewhiddenactivities',1,1413856736,0),(126,1,3,'moodle/course:viewhiddenactivities',1,1413856736,0),(127,1,1,'moodle/course:viewhiddenactivities',1,1413856737,0),(128,1,5,'moodle/course:viewparticipants',1,1413856737,0),(129,1,4,'moodle/course:viewparticipants',1,1413856737,0),(130,1,3,'moodle/course:viewparticipants',1,1413856737,0),(131,1,1,'moodle/course:viewparticipants',1,1413856737,0),(132,1,3,'moodle/course:changefullname',1,1413856737,0),(133,1,1,'moodle/course:changefullname',1,1413856737,0),(134,1,3,'moodle/course:changeshortname',1,1413856737,0),(135,1,1,'moodle/course:changeshortname',1,1413856737,0),(136,1,3,'moodle/course:changeidnumber',1,1413856737,0),(137,1,1,'moodle/course:changeidnumber',1,1413856737,0),(138,1,3,'moodle/course:changecategory',1,1413856737,0),(139,1,1,'moodle/course:changecategory',1,1413856737,0),(140,1,3,'moodle/course:changesummary',1,1413856738,0),(141,1,1,'moodle/course:changesummary',1,1413856738,0),(142,1,1,'moodle/site:viewparticipants',1,1413856738,0),(143,1,5,'moodle/course:isincompletionreports',1,1413856738,0),(144,1,5,'moodle/course:viewscales',1,1413856738,0),(145,1,4,'moodle/course:viewscales',1,1413856738,0),(146,1,3,'moodle/course:viewscales',1,1413856738,0),(147,1,1,'moodle/course:viewscales',1,1413856738,0),(148,1,3,'moodle/course:managescales',1,1413856738,0),(149,1,1,'moodle/course:managescales',1,1413856739,0),(150,1,3,'moodle/course:managegroups',1,1413856739,0),(151,1,1,'moodle/course:managegroups',1,1413856739,0),(152,1,3,'moodle/course:reset',1,1413856739,0),(153,1,1,'moodle/course:reset',1,1413856739,0),(154,1,3,'moodle/course:viewsuspendedusers',1,1413856739,0),(155,1,1,'moodle/course:viewsuspendedusers',1,1413856739,0),(156,1,6,'moodle/blog:view',1,1413856740,0),(157,1,7,'moodle/blog:view',1,1413856740,0),(158,1,5,'moodle/blog:view',1,1413856740,0),(159,1,4,'moodle/blog:view',1,1413856740,0),(160,1,3,'moodle/blog:view',1,1413856740,0),(161,1,1,'moodle/blog:view',1,1413856740,0),(162,1,6,'moodle/blog:search',1,1413856740,0),(163,1,7,'moodle/blog:search',1,1413856740,0),(164,1,5,'moodle/blog:search',1,1413856740,0),(165,1,4,'moodle/blog:search',1,1413856741,0),(166,1,3,'moodle/blog:search',1,1413856741,0),(167,1,1,'moodle/blog:search',1,1413856741,0),(168,1,1,'moodle/blog:viewdrafts',1,1413856741,0),(169,1,7,'moodle/blog:create',1,1413856741,0),(170,1,1,'moodle/blog:create',1,1413856741,0),(171,1,4,'moodle/blog:manageentries',1,1413856741,0),(172,1,3,'moodle/blog:manageentries',1,1413856741,0),(173,1,1,'moodle/blog:manageentries',1,1413856741,0),(174,1,5,'moodle/blog:manageexternal',1,1413856741,0),(175,1,7,'moodle/blog:manageexternal',1,1413856741,0),(176,1,4,'moodle/blog:manageexternal',1,1413856742,0),(177,1,3,'moodle/blog:manageexternal',1,1413856742,0),(178,1,1,'moodle/blog:manageexternal',1,1413856742,0),(179,1,7,'moodle/calendar:manageownentries',1,1413856742,0),(180,1,1,'moodle/calendar:manageownentries',1,1413856742,0),(181,1,4,'moodle/calendar:managegroupentries',1,1413856742,0),(182,1,3,'moodle/calendar:managegroupentries',1,1413856742,0),(183,1,1,'moodle/calendar:managegroupentries',1,1413856742,0),(184,1,4,'moodle/calendar:manageentries',1,1413856742,0),(185,1,3,'moodle/calendar:manageentries',1,1413856742,0),(186,1,1,'moodle/calendar:manageentries',1,1413856742,0),(187,1,1,'moodle/user:editprofile',1,1413856742,0),(188,1,6,'moodle/user:editownprofile',-1000,1413856743,0),(189,1,7,'moodle/user:editownprofile',1,1413856743,0),(190,1,1,'moodle/user:editownprofile',1,1413856743,0),(191,1,6,'moodle/user:changeownpassword',-1000,1413856743,0),(192,1,7,'moodle/user:changeownpassword',1,1413856743,0),(193,1,1,'moodle/user:changeownpassword',1,1413856743,0),(194,1,5,'moodle/user:readuserposts',1,1413856743,0),(195,1,4,'moodle/user:readuserposts',1,1413856743,0),(196,1,3,'moodle/user:readuserposts',1,1413856743,0),(197,1,1,'moodle/user:readuserposts',1,1413856743,0),(198,1,5,'moodle/user:readuserblogs',1,1413856743,0),(199,1,4,'moodle/user:readuserblogs',1,1413856743,0),(200,1,3,'moodle/user:readuserblogs',1,1413856744,0),(201,1,1,'moodle/user:readuserblogs',1,1413856744,0),(202,1,1,'moodle/user:editmessageprofile',1,1413856744,0),(203,1,6,'moodle/user:editownmessageprofile',-1000,1413856744,0),(204,1,7,'moodle/user:editownmessageprofile',1,1413856744,0),(205,1,1,'moodle/user:editownmessageprofile',1,1413856744,0),(206,1,3,'moodle/question:managecategory',1,1413856744,0),(207,1,1,'moodle/question:managecategory',1,1413856744,0),(208,1,3,'moodle/question:add',1,1413856744,0),(209,1,1,'moodle/question:add',1,1413856745,0),(210,1,3,'moodle/question:editmine',1,1413856745,0),(211,1,1,'moodle/question:editmine',1,1413856745,0),(212,1,3,'moodle/question:editall',1,1413856745,0),(213,1,1,'moodle/question:editall',1,1413856745,0),(214,1,3,'moodle/question:viewmine',1,1413856745,0),(215,1,1,'moodle/question:viewmine',1,1413856745,0),(216,1,3,'moodle/question:viewall',1,1413856745,0),(217,1,1,'moodle/question:viewall',1,1413856745,0),(218,1,3,'moodle/question:usemine',1,1413856745,0),(219,1,1,'moodle/question:usemine',1,1413856746,0),(220,1,3,'moodle/question:useall',1,1413856746,0),(221,1,1,'moodle/question:useall',1,1413856746,0),(222,1,3,'moodle/question:movemine',1,1413856746,0),(223,1,1,'moodle/question:movemine',1,1413856746,0),(224,1,3,'moodle/question:moveall',1,1413856746,0),(225,1,1,'moodle/question:moveall',1,1413856746,0),(226,1,1,'moodle/question:config',1,1413856746,0),(227,1,5,'moodle/question:flag',1,1413856746,0),(228,1,4,'moodle/question:flag',1,1413856746,0),(229,1,3,'moodle/question:flag',1,1413856746,0),(230,1,1,'moodle/question:flag',1,1413856746,0),(231,1,4,'moodle/site:doclinks',1,1413856747,0),(232,1,3,'moodle/site:doclinks',1,1413856747,0),(233,1,1,'moodle/site:doclinks',1,1413856747,0),(234,1,3,'moodle/course:sectionvisibility',1,1413856747,0),(235,1,1,'moodle/course:sectionvisibility',1,1413856747,0),(236,1,3,'moodle/course:useremail',1,1413856747,0),(237,1,1,'moodle/course:useremail',1,1413856747,0),(238,1,3,'moodle/course:viewhiddensections',1,1413856747,0),(239,1,1,'moodle/course:viewhiddensections',1,1413856747,0),(240,1,3,'moodle/course:setcurrentsection',1,1413856747,0),(241,1,1,'moodle/course:setcurrentsection',1,1413856747,0),(242,1,3,'moodle/course:movesections',1,1413856748,0),(243,1,1,'moodle/course:movesections',1,1413856748,0),(244,1,4,'moodle/grade:viewall',1,1413856748,0),(245,1,3,'moodle/grade:viewall',1,1413856748,0),(246,1,1,'moodle/grade:viewall',1,1413856748,0),(247,1,5,'moodle/grade:view',1,1413856748,0),(248,1,4,'moodle/grade:viewhidden',1,1413856748,0),(249,1,3,'moodle/grade:viewhidden',1,1413856748,0),(250,1,1,'moodle/grade:viewhidden',1,1413856748,0),(251,1,3,'moodle/grade:import',1,1413856749,0),(252,1,1,'moodle/grade:import',1,1413856749,0),(253,1,4,'moodle/grade:export',1,1413856749,0),(254,1,3,'moodle/grade:export',1,1413856749,0),(255,1,1,'moodle/grade:export',1,1413856749,0),(256,1,3,'moodle/grade:manage',1,1413856749,0),(257,1,1,'moodle/grade:manage',1,1413856749,0),(258,1,3,'moodle/grade:edit',1,1413856749,0),(259,1,1,'moodle/grade:edit',1,1413856749,0),(260,1,3,'moodle/grade:managegradingforms',1,1413856749,0),(261,1,1,'moodle/grade:managegradingforms',1,1413856750,0),(262,1,1,'moodle/grade:sharegradingforms',1,1413856750,0),(263,1,1,'moodle/grade:managesharedforms',1,1413856750,0),(264,1,3,'moodle/grade:manageoutcomes',1,1413856750,0),(265,1,1,'moodle/grade:manageoutcomes',1,1413856750,0),(266,1,3,'moodle/grade:manageletters',1,1413856750,0),(267,1,1,'moodle/grade:manageletters',1,1413856750,0),(268,1,3,'moodle/grade:hide',1,1413856751,0),(269,1,1,'moodle/grade:hide',1,1413856751,0),(270,1,3,'moodle/grade:lock',1,1413856751,0),(271,1,1,'moodle/grade:lock',1,1413856751,0),(272,1,3,'moodle/grade:unlock',1,1413856751,0),(273,1,1,'moodle/grade:unlock',1,1413856751,0),(274,1,7,'moodle/my:manageblocks',1,1413856751,0),(275,1,4,'moodle/notes:view',1,1413856752,0),(276,1,3,'moodle/notes:view',1,1413856752,0),(277,1,1,'moodle/notes:view',1,1413856752,0),(278,1,4,'moodle/notes:manage',1,1413856752,0),(279,1,3,'moodle/notes:manage',1,1413856752,0),(280,1,1,'moodle/notes:manage',1,1413856752,0),(281,1,4,'moodle/tag:manage',1,1413856752,0),(282,1,3,'moodle/tag:manage',1,1413856752,0),(283,1,1,'moodle/tag:manage',1,1413856752,0),(284,1,1,'moodle/tag:create',1,1413856752,0),(285,1,7,'moodle/tag:create',1,1413856752,0),(286,1,1,'moodle/tag:edit',1,1413856752,0),(287,1,7,'moodle/tag:edit',1,1413856753,0),(288,1,1,'moodle/tag:flag',1,1413856753,0),(289,1,7,'moodle/tag:flag',1,1413856753,0),(290,1,4,'moodle/tag:editblocks',1,1413856753,0),(291,1,3,'moodle/tag:editblocks',1,1413856753,0),(292,1,1,'moodle/tag:editblocks',1,1413856753,0),(293,1,6,'moodle/block:view',1,1413856753,0),(294,1,7,'moodle/block:view',1,1413856753,0),(295,1,5,'moodle/block:view',1,1413856753,0),(296,1,4,'moodle/block:view',1,1413856753,0),(297,1,3,'moodle/block:view',1,1413856753,0),(298,1,3,'moodle/block:edit',1,1413856754,0),(299,1,1,'moodle/block:edit',1,1413856754,0),(300,1,7,'moodle/portfolio:export',1,1413856754,0),(301,1,5,'moodle/portfolio:export',1,1413856754,0),(302,1,4,'moodle/portfolio:export',1,1413856754,0),(303,1,3,'moodle/portfolio:export',1,1413856754,0),(304,1,8,'moodle/comment:view',1,1413856754,0),(305,1,6,'moodle/comment:view',1,1413856754,0),(306,1,7,'moodle/comment:view',1,1413856754,0),(307,1,5,'moodle/comment:view',1,1413856755,0),(308,1,4,'moodle/comment:view',1,1413856755,0),(309,1,3,'moodle/comment:view',1,1413856755,0),(310,1,1,'moodle/comment:view',1,1413856755,0),(311,1,7,'moodle/comment:post',1,1413856755,0),(312,1,5,'moodle/comment:post',1,1413856755,0),(313,1,4,'moodle/comment:post',1,1413856755,0),(314,1,3,'moodle/comment:post',1,1413856755,0),(315,1,1,'moodle/comment:post',1,1413856755,0),(316,1,3,'moodle/comment:delete',1,1413856755,0),(317,1,1,'moodle/comment:delete',1,1413856755,0),(318,1,1,'moodle/webservice:createtoken',1,1413856755,0),(319,1,7,'moodle/webservice:createmobiletoken',1,1413856756,0),(320,1,7,'moodle/rating:view',1,1413856756,0),(321,1,5,'moodle/rating:view',1,1413856756,0),(322,1,4,'moodle/rating:view',1,1413856756,0),(323,1,3,'moodle/rating:view',1,1413856756,0),(324,1,1,'moodle/rating:view',1,1413856756,0),(325,1,7,'moodle/rating:viewany',1,1413856756,0),(326,1,5,'moodle/rating:viewany',1,1413856756,0),(327,1,4,'moodle/rating:viewany',1,1413856756,0),(328,1,3,'moodle/rating:viewany',1,1413856756,0),(329,1,1,'moodle/rating:viewany',1,1413856756,0),(330,1,7,'moodle/rating:viewall',1,1413856756,0),(331,1,5,'moodle/rating:viewall',1,1413856756,0),(332,1,4,'moodle/rating:viewall',1,1413856756,0),(333,1,3,'moodle/rating:viewall',1,1413856756,0),(334,1,1,'moodle/rating:viewall',1,1413856757,0),(335,1,7,'moodle/rating:rate',1,1413856757,0),(336,1,5,'moodle/rating:rate',1,1413856757,0),(337,1,4,'moodle/rating:rate',1,1413856757,0),(338,1,3,'moodle/rating:rate',1,1413856757,0),(339,1,1,'moodle/rating:rate',1,1413856757,0),(340,1,1,'moodle/course:publish',1,1413856757,0),(341,1,4,'moodle/course:markcomplete',1,1413856757,0),(342,1,3,'moodle/course:markcomplete',1,1413856757,0),(343,1,1,'moodle/course:markcomplete',1,1413856757,0),(344,1,1,'moodle/community:add',1,1413856757,0),(345,1,4,'moodle/community:add',1,1413856757,0),(346,1,3,'moodle/community:add',1,1413856758,0),(347,1,1,'moodle/community:download',1,1413856758,0),(348,1,3,'moodle/community:download',1,1413856758,0),(349,1,1,'moodle/badges:manageglobalsettings',1,1413856758,0),(350,1,7,'moodle/badges:viewbadges',1,1413856758,0),(351,1,7,'moodle/badges:manageownbadges',1,1413856758,0),(352,1,7,'moodle/badges:viewotherbadges',1,1413856758,0),(353,1,7,'moodle/badges:earnbadge',1,1413856759,0),(354,1,1,'moodle/badges:createbadge',1,1413856759,0),(355,1,3,'moodle/badges:createbadge',1,1413856759,0),(356,1,1,'moodle/badges:deletebadge',1,1413856759,0),(357,1,3,'moodle/badges:deletebadge',1,1413856759,0),(358,1,1,'moodle/badges:configuredetails',1,1413856759,0),(359,1,3,'moodle/badges:configuredetails',1,1413856759,0),(360,1,1,'moodle/badges:configurecriteria',1,1413856759,0),(361,1,3,'moodle/badges:configurecriteria',1,1413856759,0),(362,1,1,'moodle/badges:configuremessages',1,1413856759,0),(363,1,3,'moodle/badges:configuremessages',1,1413856759,0),(364,1,1,'moodle/badges:awardbadge',1,1413856760,0),(365,1,4,'moodle/badges:awardbadge',1,1413856760,0),(366,1,3,'moodle/badges:awardbadge',1,1413856760,0),(367,1,1,'moodle/badges:viewawarded',1,1413856760,0),(368,1,4,'moodle/badges:viewawarded',1,1413856760,0),(369,1,3,'moodle/badges:viewawarded',1,1413856760,0),(370,1,6,'mod/assign:view',1,1413856842,0),(371,1,5,'mod/assign:view',1,1413856842,0),(372,1,4,'mod/assign:view',1,1413856842,0),(373,1,3,'mod/assign:view',1,1413856842,0),(374,1,1,'mod/assign:view',1,1413856843,0),(375,1,5,'mod/assign:submit',1,1413856843,0),(376,1,4,'mod/assign:grade',1,1413856843,0),(377,1,3,'mod/assign:grade',1,1413856843,0),(378,1,1,'mod/assign:grade',1,1413856843,0),(379,1,4,'mod/assign:exportownsubmission',1,1413856843,0),(380,1,3,'mod/assign:exportownsubmission',1,1413856843,0),(381,1,1,'mod/assign:exportownsubmission',1,1413856843,0),(382,1,5,'mod/assign:exportownsubmission',1,1413856843,0),(383,1,3,'mod/assign:addinstance',1,1413856843,0),(384,1,1,'mod/assign:addinstance',1,1413856843,0),(385,1,4,'mod/assign:grantextension',1,1413856844,0),(386,1,3,'mod/assign:grantextension',1,1413856844,0),(387,1,1,'mod/assign:grantextension',1,1413856844,0),(388,1,3,'mod/assign:revealidentities',1,1413856844,0),(389,1,1,'mod/assign:revealidentities',1,1413856844,0),(390,1,3,'mod/assign:reviewgrades',1,1413856844,0),(391,1,1,'mod/assign:reviewgrades',1,1413856844,0),(392,1,3,'mod/assign:releasegrades',1,1413856844,0),(393,1,1,'mod/assign:releasegrades',1,1413856844,0),(394,1,3,'mod/assign:managegrades',1,1413856844,0),(395,1,1,'mod/assign:managegrades',1,1413856844,0),(396,1,3,'mod/assign:manageallocations',1,1413856845,0),(397,1,1,'mod/assign:manageallocations',1,1413856845,0),(398,1,3,'mod/assign:viewgrades',1,1413856845,0),(399,1,1,'mod/assign:viewgrades',1,1413856845,0),(400,1,4,'mod/assign:viewgrades',1,1413856845,0),(401,1,6,'mod/assignment:view',1,1413856849,0),(402,1,5,'mod/assignment:view',1,1413856849,0),(403,1,4,'mod/assignment:view',1,1413856849,0),(404,1,3,'mod/assignment:view',1,1413856849,0),(405,1,1,'mod/assignment:view',1,1413856849,0),(406,1,3,'mod/assignment:addinstance',1,1413856849,0),(407,1,1,'mod/assignment:addinstance',1,1413856849,0),(408,1,5,'mod/assignment:submit',1,1413856849,0),(409,1,4,'mod/assignment:grade',1,1413856849,0),(410,1,3,'mod/assignment:grade',1,1413856849,0),(411,1,1,'mod/assignment:grade',1,1413856850,0),(412,1,4,'mod/assignment:exportownsubmission',1,1413856850,0),(413,1,3,'mod/assignment:exportownsubmission',1,1413856850,0),(414,1,1,'mod/assignment:exportownsubmission',1,1413856850,0),(415,1,5,'mod/assignment:exportownsubmission',1,1413856850,0),(416,1,3,'mod/book:addinstance',1,1413856851,0),(417,1,1,'mod/book:addinstance',1,1413856851,0),(418,1,6,'mod/book:read',1,1413856851,0),(419,1,8,'mod/book:read',1,1413856851,0),(420,1,5,'mod/book:read',1,1413856851,0),(421,1,4,'mod/book:read',1,1413856851,0),(422,1,3,'mod/book:read',1,1413856852,0),(423,1,1,'mod/book:read',1,1413856852,0),(424,1,4,'mod/book:viewhiddenchapters',1,1413856852,0),(425,1,3,'mod/book:viewhiddenchapters',1,1413856852,0),(426,1,1,'mod/book:viewhiddenchapters',1,1413856852,0),(427,1,3,'mod/book:edit',1,1413856852,0),(428,1,1,'mod/book:edit',1,1413856852,0),(429,1,3,'mod/chat:addinstance',1,1413856853,0),(430,1,1,'mod/chat:addinstance',1,1413856853,0),(431,1,5,'mod/chat:chat',1,1413856853,0),(432,1,4,'mod/chat:chat',1,1413856853,0),(433,1,3,'mod/chat:chat',1,1413856853,0),(434,1,1,'mod/chat:chat',1,1413856854,0),(435,1,5,'mod/chat:readlog',1,1413856854,0),(436,1,4,'mod/chat:readlog',1,1413856854,0),(437,1,3,'mod/chat:readlog',1,1413856854,0),(438,1,1,'mod/chat:readlog',1,1413856854,0),(439,1,4,'mod/chat:deletelog',1,1413856854,0),(440,1,3,'mod/chat:deletelog',1,1413856854,0),(441,1,1,'mod/chat:deletelog',1,1413856854,0),(442,1,4,'mod/chat:exportparticipatedsession',1,1413856854,0),(443,1,3,'mod/chat:exportparticipatedsession',1,1413856854,0),(444,1,1,'mod/chat:exportparticipatedsession',1,1413856854,0),(445,1,4,'mod/chat:exportsession',1,1413856854,0),(446,1,3,'mod/chat:exportsession',1,1413856854,0),(447,1,1,'mod/chat:exportsession',1,1413856854,0),(448,1,3,'mod/choice:addinstance',1,1413856856,0),(449,1,1,'mod/choice:addinstance',1,1413856856,0),(450,1,5,'mod/choice:choose',1,1413856856,0),(451,1,4,'mod/choice:choose',1,1413856856,0),(452,1,3,'mod/choice:choose',1,1413856856,0),(453,1,4,'mod/choice:readresponses',1,1413856857,0),(454,1,3,'mod/choice:readresponses',1,1413856857,0),(455,1,1,'mod/choice:readresponses',1,1413856857,0),(456,1,4,'mod/choice:deleteresponses',1,1413856857,0),(457,1,3,'mod/choice:deleteresponses',1,1413856857,0),(458,1,1,'mod/choice:deleteresponses',1,1413856857,0),(459,1,4,'mod/choice:downloadresponses',1,1413856857,0),(460,1,3,'mod/choice:downloadresponses',1,1413856857,0),(461,1,1,'mod/choice:downloadresponses',1,1413856857,0),(462,1,3,'mod/data:addinstance',1,1413856858,0),(463,1,1,'mod/data:addinstance',1,1413856858,0),(464,1,8,'mod/data:viewentry',1,1413856858,0),(465,1,6,'mod/data:viewentry',1,1413856858,0),(466,1,5,'mod/data:viewentry',1,1413856859,0),(467,1,4,'mod/data:viewentry',1,1413856859,0),(468,1,3,'mod/data:viewentry',1,1413856859,0),(469,1,1,'mod/data:viewentry',1,1413856859,0),(470,1,5,'mod/data:writeentry',1,1413856859,0),(471,1,4,'mod/data:writeentry',1,1413856859,0),(472,1,3,'mod/data:writeentry',1,1413856859,0),(473,1,1,'mod/data:writeentry',1,1413856859,0),(474,1,5,'mod/data:comment',1,1413856859,0),(475,1,4,'mod/data:comment',1,1413856859,0),(476,1,3,'mod/data:comment',1,1413856859,0),(477,1,1,'mod/data:comment',1,1413856860,0),(478,1,4,'mod/data:rate',1,1413856860,0),(479,1,3,'mod/data:rate',1,1413856860,0),(480,1,1,'mod/data:rate',1,1413856860,0),(481,1,4,'mod/data:viewrating',1,1413856860,0),(482,1,3,'mod/data:viewrating',1,1413856860,0),(483,1,1,'mod/data:viewrating',1,1413856860,0),(484,1,4,'mod/data:viewanyrating',1,1413856860,0),(485,1,3,'mod/data:viewanyrating',1,1413856860,0),(486,1,1,'mod/data:viewanyrating',1,1413856860,0),(487,1,4,'mod/data:viewallratings',1,1413856860,0),(488,1,3,'mod/data:viewallratings',1,1413856860,0),(489,1,1,'mod/data:viewallratings',1,1413856861,0),(490,1,4,'mod/data:approve',1,1413856861,0),(491,1,3,'mod/data:approve',1,1413856861,0),(492,1,1,'mod/data:approve',1,1413856861,0),(493,1,4,'mod/data:manageentries',1,1413856861,0),(494,1,3,'mod/data:manageentries',1,1413856861,0),(495,1,1,'mod/data:manageentries',1,1413856861,0),(496,1,4,'mod/data:managecomments',1,1413856861,0),(497,1,3,'mod/data:managecomments',1,1413856861,0),(498,1,1,'mod/data:managecomments',1,1413856861,0),(499,1,3,'mod/data:managetemplates',1,1413856861,0),(500,1,1,'mod/data:managetemplates',1,1413856861,0),(501,1,4,'mod/data:viewalluserpresets',1,1413856862,0),(502,1,3,'mod/data:viewalluserpresets',1,1413856862,0),(503,1,1,'mod/data:viewalluserpresets',1,1413856862,0),(504,1,1,'mod/data:manageuserpresets',1,1413856862,0),(505,1,1,'mod/data:exportentry',1,1413856862,0),(506,1,4,'mod/data:exportentry',1,1413856862,0),(507,1,3,'mod/data:exportentry',1,1413856862,0),(508,1,1,'mod/data:exportownentry',1,1413856862,0),(509,1,4,'mod/data:exportownentry',1,1413856862,0),(510,1,3,'mod/data:exportownentry',1,1413856862,0),(511,1,5,'mod/data:exportownentry',1,1413856862,0),(512,1,1,'mod/data:exportallentries',1,1413856863,0),(513,1,4,'mod/data:exportallentries',1,1413856863,0),(514,1,3,'mod/data:exportallentries',1,1413856863,0),(515,1,1,'mod/data:exportuserinfo',1,1413856863,0),(516,1,4,'mod/data:exportuserinfo',1,1413856863,0),(517,1,3,'mod/data:exportuserinfo',1,1413856863,0),(518,1,3,'mod/feedback:addinstance',1,1413856865,0),(519,1,1,'mod/feedback:addinstance',1,1413856865,0),(520,1,6,'mod/feedback:view',1,1413856865,0),(521,1,5,'mod/feedback:view',1,1413856865,0),(522,1,4,'mod/feedback:view',1,1413856866,0),(523,1,3,'mod/feedback:view',1,1413856866,0),(524,1,1,'mod/feedback:view',1,1413856866,0),(525,1,5,'mod/feedback:complete',1,1413856866,0),(526,1,5,'mod/feedback:viewanalysepage',1,1413856866,0),(527,1,3,'mod/feedback:viewanalysepage',1,1413856866,0),(528,1,1,'mod/feedback:viewanalysepage',1,1413856866,0),(529,1,3,'mod/feedback:deletesubmissions',1,1413856866,0),(530,1,1,'mod/feedback:deletesubmissions',1,1413856866,0),(531,1,1,'mod/feedback:mapcourse',1,1413856866,0),(532,1,3,'mod/feedback:edititems',1,1413856867,0),(533,1,1,'mod/feedback:edititems',1,1413856867,0),(534,1,3,'mod/feedback:createprivatetemplate',1,1413856867,0),(535,1,1,'mod/feedback:createprivatetemplate',1,1413856867,0),(536,1,3,'mod/feedback:createpublictemplate',1,1413856867,0),(537,1,1,'mod/feedback:createpublictemplate',1,1413856867,0),(538,1,3,'mod/feedback:deletetemplate',1,1413856868,0),(539,1,1,'mod/feedback:deletetemplate',1,1413856868,0),(540,1,4,'mod/feedback:viewreports',1,1413856868,0),(541,1,3,'mod/feedback:viewreports',1,1413856868,0),(542,1,1,'mod/feedback:viewreports',1,1413856868,0),(543,1,4,'mod/feedback:receivemail',1,1413856868,0),(544,1,3,'mod/feedback:receivemail',1,1413856868,0),(545,1,3,'mod/folder:addinstance',1,1413856869,0),(546,1,1,'mod/folder:addinstance',1,1413856870,0),(547,1,6,'mod/folder:view',1,1413856870,0),(548,1,7,'mod/folder:view',1,1413856870,0),(549,1,3,'mod/folder:managefiles',1,1413856870,0),(550,1,3,'mod/forum:addinstance',1,1413856871,0),(551,1,1,'mod/forum:addinstance',1,1413856871,0),(552,1,8,'mod/forum:viewdiscussion',1,1413856871,0),(553,1,6,'mod/forum:viewdiscussion',1,1413856871,0),(554,1,5,'mod/forum:viewdiscussion',1,1413856872,0),(555,1,4,'mod/forum:viewdiscussion',1,1413856872,0),(556,1,3,'mod/forum:viewdiscussion',1,1413856872,0),(557,1,1,'mod/forum:viewdiscussion',1,1413856872,0),(558,1,4,'mod/forum:viewhiddentimedposts',1,1413856872,0),(559,1,3,'mod/forum:viewhiddentimedposts',1,1413856872,0),(560,1,1,'mod/forum:viewhiddentimedposts',1,1413856872,0),(561,1,5,'mod/forum:startdiscussion',1,1413856872,0),(562,1,4,'mod/forum:startdiscussion',1,1413856872,0),(563,1,3,'mod/forum:startdiscussion',1,1413856872,0),(564,1,1,'mod/forum:startdiscussion',1,1413856872,0),(565,1,5,'mod/forum:replypost',1,1413856872,0),(566,1,4,'mod/forum:replypost',1,1413856873,0),(567,1,3,'mod/forum:replypost',1,1413856873,0),(568,1,1,'mod/forum:replypost',1,1413856873,0),(569,1,4,'mod/forum:addnews',1,1413856873,0),(570,1,3,'mod/forum:addnews',1,1413856873,0),(571,1,1,'mod/forum:addnews',1,1413856873,0),(572,1,4,'mod/forum:replynews',1,1413856873,0),(573,1,3,'mod/forum:replynews',1,1413856873,0),(574,1,1,'mod/forum:replynews',1,1413856873,0),(575,1,5,'mod/forum:viewrating',1,1413856873,0),(576,1,4,'mod/forum:viewrating',1,1413856873,0),(577,1,3,'mod/forum:viewrating',1,1413856874,0),(578,1,1,'mod/forum:viewrating',1,1413856874,0),(579,1,4,'mod/forum:viewanyrating',1,1413856874,0),(580,1,3,'mod/forum:viewanyrating',1,1413856874,0),(581,1,1,'mod/forum:viewanyrating',1,1413856874,0),(582,1,4,'mod/forum:viewallratings',1,1413856874,0),(583,1,3,'mod/forum:viewallratings',1,1413856874,0),(584,1,1,'mod/forum:viewallratings',1,1413856874,0),(585,1,4,'mod/forum:rate',1,1413856874,0),(586,1,3,'mod/forum:rate',1,1413856874,0),(587,1,1,'mod/forum:rate',1,1413856874,0),(588,1,5,'mod/forum:createattachment',1,1413856874,0),(589,1,4,'mod/forum:createattachment',1,1413856875,0),(590,1,3,'mod/forum:createattachment',1,1413856875,0),(591,1,1,'mod/forum:createattachment',1,1413856875,0),(592,1,5,'mod/forum:deleteownpost',1,1413856875,0),(593,1,4,'mod/forum:deleteownpost',1,1413856875,0),(594,1,3,'mod/forum:deleteownpost',1,1413856875,0),(595,1,1,'mod/forum:deleteownpost',1,1413856875,0),(596,1,4,'mod/forum:deleteanypost',1,1413856875,0),(597,1,3,'mod/forum:deleteanypost',1,1413856875,0),(598,1,1,'mod/forum:deleteanypost',1,1413856875,0),(599,1,4,'mod/forum:splitdiscussions',1,1413856875,0),(600,1,3,'mod/forum:splitdiscussions',1,1413856875,0),(601,1,1,'mod/forum:splitdiscussions',1,1413856875,0),(602,1,4,'mod/forum:movediscussions',1,1413856876,0),(603,1,3,'mod/forum:movediscussions',1,1413856876,0),(604,1,1,'mod/forum:movediscussions',1,1413856876,0),(605,1,4,'mod/forum:editanypost',1,1413856876,0),(606,1,3,'mod/forum:editanypost',1,1413856876,0),(607,1,1,'mod/forum:editanypost',1,1413856876,0),(608,1,4,'mod/forum:viewqandawithoutposting',1,1413856876,0),(609,1,3,'mod/forum:viewqandawithoutposting',1,1413856876,0),(610,1,1,'mod/forum:viewqandawithoutposting',1,1413856876,0),(611,1,4,'mod/forum:viewsubscribers',1,1413856876,0),(612,1,3,'mod/forum:viewsubscribers',1,1413856876,0),(613,1,1,'mod/forum:viewsubscribers',1,1413856876,0),(614,1,4,'mod/forum:managesubscriptions',1,1413856877,0),(615,1,3,'mod/forum:managesubscriptions',1,1413856877,0),(616,1,1,'mod/forum:managesubscriptions',1,1413856877,0),(617,1,4,'mod/forum:postwithoutthrottling',1,1413856877,0),(618,1,3,'mod/forum:postwithoutthrottling',1,1413856877,0),(619,1,1,'mod/forum:postwithoutthrottling',1,1413856877,0),(620,1,4,'mod/forum:exportdiscussion',1,1413856877,0),(621,1,3,'mod/forum:exportdiscussion',1,1413856877,0),(622,1,1,'mod/forum:exportdiscussion',1,1413856877,0),(623,1,4,'mod/forum:exportpost',1,1413856878,0),(624,1,3,'mod/forum:exportpost',1,1413856878,0),(625,1,1,'mod/forum:exportpost',1,1413856878,0),(626,1,4,'mod/forum:exportownpost',1,1413856878,0),(627,1,3,'mod/forum:exportownpost',1,1413856878,0),(628,1,1,'mod/forum:exportownpost',1,1413856878,0),(629,1,5,'mod/forum:exportownpost',1,1413856878,0),(630,1,4,'mod/forum:addquestion',1,1413856878,0),(631,1,3,'mod/forum:addquestion',1,1413856879,0),(632,1,1,'mod/forum:addquestion',1,1413856879,0),(633,1,5,'mod/forum:allowforcesubscribe',1,1413856879,0),(634,1,4,'mod/forum:allowforcesubscribe',1,1413856879,0),(635,1,3,'mod/forum:allowforcesubscribe',1,1413856879,0),(636,1,8,'mod/forum:allowforcesubscribe',1,1413856879,0),(637,1,3,'mod/glossary:addinstance',1,1413856881,0),(638,1,1,'mod/glossary:addinstance',1,1413856881,0),(639,1,8,'mod/glossary:view',1,1413856881,0),(640,1,6,'mod/glossary:view',1,1413856882,0),(641,1,5,'mod/glossary:view',1,1413856882,0),(642,1,4,'mod/glossary:view',1,1413856882,0),(643,1,3,'mod/glossary:view',1,1413856882,0),(644,1,1,'mod/glossary:view',1,1413856882,0),(645,1,5,'mod/glossary:write',1,1413856882,0),(646,1,4,'mod/glossary:write',1,1413856882,0),(647,1,3,'mod/glossary:write',1,1413856882,0),(648,1,1,'mod/glossary:write',1,1413856882,0),(649,1,4,'mod/glossary:manageentries',1,1413856882,0),(650,1,3,'mod/glossary:manageentries',1,1413856882,0),(651,1,1,'mod/glossary:manageentries',1,1413856882,0),(652,1,4,'mod/glossary:managecategories',1,1413856882,0),(653,1,3,'mod/glossary:managecategories',1,1413856883,0),(654,1,1,'mod/glossary:managecategories',1,1413856883,0),(655,1,5,'mod/glossary:comment',1,1413856883,0),(656,1,4,'mod/glossary:comment',1,1413856883,0),(657,1,3,'mod/glossary:comment',1,1413856883,0),(658,1,1,'mod/glossary:comment',1,1413856883,0),(659,1,4,'mod/glossary:managecomments',1,1413856883,0),(660,1,3,'mod/glossary:managecomments',1,1413856883,0),(661,1,1,'mod/glossary:managecomments',1,1413856883,0),(662,1,4,'mod/glossary:import',1,1413856883,0),(663,1,3,'mod/glossary:import',1,1413856883,0),(664,1,1,'mod/glossary:import',1,1413856883,0),(665,1,4,'mod/glossary:export',1,1413856884,0),(666,1,3,'mod/glossary:export',1,1413856884,0),(667,1,1,'mod/glossary:export',1,1413856884,0),(668,1,4,'mod/glossary:approve',1,1413856884,0),(669,1,3,'mod/glossary:approve',1,1413856884,0),(670,1,1,'mod/glossary:approve',1,1413856884,0),(671,1,4,'mod/glossary:rate',1,1413856884,0),(672,1,3,'mod/glossary:rate',1,1413856884,0),(673,1,1,'mod/glossary:rate',1,1413856884,0),(674,1,4,'mod/glossary:viewrating',1,1413856884,0),(675,1,3,'mod/glossary:viewrating',1,1413856884,0),(676,1,1,'mod/glossary:viewrating',1,1413856884,0),(677,1,4,'mod/glossary:viewanyrating',1,1413856885,0),(678,1,3,'mod/glossary:viewanyrating',1,1413856885,0),(679,1,1,'mod/glossary:viewanyrating',1,1413856885,0),(680,1,4,'mod/glossary:viewallratings',1,1413856885,0),(681,1,3,'mod/glossary:viewallratings',1,1413856885,0),(682,1,1,'mod/glossary:viewallratings',1,1413856885,0),(683,1,4,'mod/glossary:exportentry',1,1413856885,0),(684,1,3,'mod/glossary:exportentry',1,1413856885,0),(685,1,1,'mod/glossary:exportentry',1,1413856885,0),(686,1,4,'mod/glossary:exportownentry',1,1413856885,0),(687,1,3,'mod/glossary:exportownentry',1,1413856886,0),(688,1,1,'mod/glossary:exportownentry',1,1413856886,0),(689,1,5,'mod/glossary:exportownentry',1,1413856886,0),(690,1,6,'mod/imscp:view',1,1413856887,0),(691,1,7,'mod/imscp:view',1,1413856887,0),(692,1,3,'mod/imscp:addinstance',1,1413856887,0),(693,1,1,'mod/imscp:addinstance',1,1413856887,0),(694,1,3,'mod/label:addinstance',1,1413856888,0),(695,1,1,'mod/label:addinstance',1,1413856889,0),(696,1,3,'mod/lesson:addinstance',1,1413856890,0),(697,1,1,'mod/lesson:addinstance',1,1413856890,0),(698,1,3,'mod/lesson:edit',1,1413856890,0),(699,1,1,'mod/lesson:edit',1,1413856890,0),(700,1,4,'mod/lesson:manage',1,1413856890,0),(701,1,3,'mod/lesson:manage',1,1413856890,0),(702,1,1,'mod/lesson:manage',1,1413856890,0),(703,1,5,'mod/lti:view',1,1413856891,0),(704,1,4,'mod/lti:view',1,1413856892,0),(705,1,3,'mod/lti:view',1,1413856892,0),(706,1,1,'mod/lti:view',1,1413856892,0),(707,1,3,'mod/lti:addinstance',1,1413856892,0),(708,1,1,'mod/lti:addinstance',1,1413856892,0),(709,1,4,'mod/lti:grade',1,1413856892,0),(710,1,3,'mod/lti:grade',1,1413856892,0),(711,1,1,'mod/lti:grade',1,1413856892,0),(712,1,4,'mod/lti:manage',1,1413856892,0),(713,1,3,'mod/lti:manage',1,1413856892,0),(714,1,1,'mod/lti:manage',1,1413856892,0),(715,1,3,'mod/lti:addcoursetool',1,1413856893,0),(716,1,1,'mod/lti:addcoursetool',1,1413856893,0),(717,1,3,'mod/lti:requesttooladd',1,1413856893,0),(718,1,1,'mod/lti:requesttooladd',1,1413856893,0),(719,1,6,'mod/page:view',1,1413856894,0),(720,1,7,'mod/page:view',1,1413856894,0),(721,1,3,'mod/page:addinstance',1,1413856894,0),(722,1,1,'mod/page:addinstance',1,1413856894,0),(723,1,6,'mod/quiz:view',1,1413856895,0),(724,1,5,'mod/quiz:view',1,1413856895,0),(725,1,4,'mod/quiz:view',1,1413856895,0),(726,1,3,'mod/quiz:view',1,1413856896,0),(727,1,1,'mod/quiz:view',1,1413856896,0),(728,1,3,'mod/quiz:addinstance',1,1413856896,0),(729,1,1,'mod/quiz:addinstance',1,1413856896,0),(730,1,5,'mod/quiz:attempt',1,1413856896,0),(731,1,5,'mod/quiz:reviewmyattempts',1,1413856896,0),(732,1,3,'mod/quiz:manage',1,1413856896,0),(733,1,1,'mod/quiz:manage',1,1413856896,0),(734,1,3,'mod/quiz:manageoverrides',1,1413856896,0),(735,1,1,'mod/quiz:manageoverrides',1,1413856896,0),(736,1,4,'mod/quiz:preview',1,1413856897,0),(737,1,3,'mod/quiz:preview',1,1413856897,0),(738,1,1,'mod/quiz:preview',1,1413856897,0),(739,1,4,'mod/quiz:grade',1,1413856897,0),(740,1,3,'mod/quiz:grade',1,1413856897,0),(741,1,1,'mod/quiz:grade',1,1413856897,0),(742,1,4,'mod/quiz:regrade',1,1413856897,0),(743,1,3,'mod/quiz:regrade',1,1413856897,0),(744,1,1,'mod/quiz:regrade',1,1413856897,0),(745,1,4,'mod/quiz:viewreports',1,1413856897,0),(746,1,3,'mod/quiz:viewreports',1,1413856897,0),(747,1,1,'mod/quiz:viewreports',1,1413856898,0),(748,1,3,'mod/quiz:deleteattempts',1,1413856898,0),(749,1,1,'mod/quiz:deleteattempts',1,1413856898,0),(750,1,6,'mod/resource:view',1,1413856901,0),(751,1,7,'mod/resource:view',1,1413856901,0),(752,1,3,'mod/resource:addinstance',1,1413856901,0),(753,1,1,'mod/resource:addinstance',1,1413856901,0),(754,1,3,'mod/scorm:addinstance',1,1413856903,0),(755,1,1,'mod/scorm:addinstance',1,1413856903,0),(756,1,4,'mod/scorm:viewreport',1,1413856903,0),(757,1,3,'mod/scorm:viewreport',1,1413856903,0),(758,1,1,'mod/scorm:viewreport',1,1413856903,0),(759,1,5,'mod/scorm:skipview',1,1413856903,0),(760,1,5,'mod/scorm:savetrack',1,1413856903,0),(761,1,4,'mod/scorm:savetrack',1,1413856903,0),(762,1,3,'mod/scorm:savetrack',1,1413856903,0),(763,1,1,'mod/scorm:savetrack',1,1413856903,0),(764,1,5,'mod/scorm:viewscores',1,1413856904,0),(765,1,4,'mod/scorm:viewscores',1,1413856904,0),(766,1,3,'mod/scorm:viewscores',1,1413856904,0),(767,1,1,'mod/scorm:viewscores',1,1413856904,0),(768,1,4,'mod/scorm:deleteresponses',1,1413856904,0),(769,1,3,'mod/scorm:deleteresponses',1,1413856904,0),(770,1,1,'mod/scorm:deleteresponses',1,1413856904,0),(771,1,3,'mod/survey:addinstance',1,1413856910,0),(772,1,1,'mod/survey:addinstance',1,1413856911,0),(773,1,5,'mod/survey:participate',1,1413856911,0),(774,1,4,'mod/survey:participate',1,1413856911,0),(775,1,3,'mod/survey:participate',1,1413856911,0),(776,1,1,'mod/survey:participate',1,1413856911,0),(777,1,4,'mod/survey:readresponses',1,1413856911,0),(778,1,3,'mod/survey:readresponses',1,1413856911,0),(779,1,1,'mod/survey:readresponses',1,1413856911,0),(780,1,4,'mod/survey:download',1,1413856911,0),(781,1,3,'mod/survey:download',1,1413856911,0),(782,1,1,'mod/survey:download',1,1413856911,0),(783,1,6,'mod/url:view',1,1413856912,0),(784,1,7,'mod/url:view',1,1413856912,0),(785,1,3,'mod/url:addinstance',1,1413856912,0),(786,1,1,'mod/url:addinstance',1,1413856912,0),(787,1,3,'mod/wiki:addinstance',1,1413856914,0),(788,1,1,'mod/wiki:addinstance',1,1413856914,0),(789,1,6,'mod/wiki:viewpage',1,1413856914,0),(790,1,5,'mod/wiki:viewpage',1,1413856914,0),(791,1,4,'mod/wiki:viewpage',1,1413856915,0),(792,1,3,'mod/wiki:viewpage',1,1413856915,0),(793,1,1,'mod/wiki:viewpage',1,1413856915,0),(794,1,5,'mod/wiki:editpage',1,1413856915,0),(795,1,4,'mod/wiki:editpage',1,1413856915,0),(796,1,3,'mod/wiki:editpage',1,1413856915,0),(797,1,1,'mod/wiki:editpage',1,1413856915,0),(798,1,5,'mod/wiki:createpage',1,1413856915,0),(799,1,4,'mod/wiki:createpage',1,1413856915,0),(800,1,3,'mod/wiki:createpage',1,1413856915,0),(801,1,1,'mod/wiki:createpage',1,1413856915,0),(802,1,5,'mod/wiki:viewcomment',1,1413856915,0),(803,1,4,'mod/wiki:viewcomment',1,1413856915,0),(804,1,3,'mod/wiki:viewcomment',1,1413856915,0),(805,1,1,'mod/wiki:viewcomment',1,1413856915,0),(806,1,5,'mod/wiki:editcomment',1,1413856916,0),(807,1,4,'mod/wiki:editcomment',1,1413856916,0),(808,1,3,'mod/wiki:editcomment',1,1413856916,0),(809,1,1,'mod/wiki:editcomment',1,1413856916,0),(810,1,4,'mod/wiki:managecomment',1,1413856916,0),(811,1,3,'mod/wiki:managecomment',1,1413856916,0),(812,1,1,'mod/wiki:managecomment',1,1413856916,0),(813,1,4,'mod/wiki:managefiles',1,1413856916,0),(814,1,3,'mod/wiki:managefiles',1,1413856916,0),(815,1,1,'mod/wiki:managefiles',1,1413856916,0),(816,1,4,'mod/wiki:overridelock',1,1413856916,0),(817,1,3,'mod/wiki:overridelock',1,1413856916,0),(818,1,1,'mod/wiki:overridelock',1,1413856916,0),(819,1,4,'mod/wiki:managewiki',1,1413856916,0),(820,1,3,'mod/wiki:managewiki',1,1413856917,0),(821,1,1,'mod/wiki:managewiki',1,1413856917,0),(822,1,6,'mod/workshop:view',1,1413856919,0),(823,1,5,'mod/workshop:view',1,1413856919,0),(824,1,4,'mod/workshop:view',1,1413856919,0),(825,1,3,'mod/workshop:view',1,1413856919,0),(826,1,1,'mod/workshop:view',1,1413856919,0),(827,1,3,'mod/workshop:addinstance',1,1413856919,0),(828,1,1,'mod/workshop:addinstance',1,1413856919,0),(829,1,4,'mod/workshop:switchphase',1,1413856919,0),(830,1,3,'mod/workshop:switchphase',1,1413856919,0),(831,1,1,'mod/workshop:switchphase',1,1413856919,0),(832,1,3,'mod/workshop:editdimensions',1,1413856920,0),(833,1,1,'mod/workshop:editdimensions',1,1413856920,0),(834,1,5,'mod/workshop:submit',1,1413856920,0),(835,1,5,'mod/workshop:peerassess',1,1413856920,0),(836,1,4,'mod/workshop:manageexamples',1,1413856920,0),(837,1,3,'mod/workshop:manageexamples',1,1413856920,0),(838,1,1,'mod/workshop:manageexamples',1,1413856920,0),(839,1,4,'mod/workshop:allocate',1,1413856920,0),(840,1,3,'mod/workshop:allocate',1,1413856920,0),(841,1,1,'mod/workshop:allocate',1,1413856920,0),(842,1,4,'mod/workshop:publishsubmissions',1,1413856920,0),(843,1,3,'mod/workshop:publishsubmissions',1,1413856921,0),(844,1,1,'mod/workshop:publishsubmissions',1,1413856921,0),(845,1,5,'mod/workshop:viewauthornames',1,1413856921,0),(846,1,4,'mod/workshop:viewauthornames',1,1413856921,0),(847,1,3,'mod/workshop:viewauthornames',1,1413856921,0),(848,1,1,'mod/workshop:viewauthornames',1,1413856921,0),(849,1,4,'mod/workshop:viewreviewernames',1,1413856922,0),(850,1,3,'mod/workshop:viewreviewernames',1,1413856922,0),(851,1,1,'mod/workshop:viewreviewernames',1,1413856922,0),(852,1,4,'mod/workshop:viewallsubmissions',1,1413856922,0),(853,1,3,'mod/workshop:viewallsubmissions',1,1413856922,0),(854,1,1,'mod/workshop:viewallsubmissions',1,1413856922,0),(855,1,5,'mod/workshop:viewpublishedsubmissions',1,1413856922,0),(856,1,4,'mod/workshop:viewpublishedsubmissions',1,1413856922,0),(857,1,3,'mod/workshop:viewpublishedsubmissions',1,1413856922,0),(858,1,1,'mod/workshop:viewpublishedsubmissions',1,1413856922,0),(859,1,5,'mod/workshop:viewauthorpublished',1,1413856923,0),(860,1,4,'mod/workshop:viewauthorpublished',1,1413856923,0),(861,1,3,'mod/workshop:viewauthorpublished',1,1413856923,0),(862,1,1,'mod/workshop:viewauthorpublished',1,1413856923,0),(863,1,4,'mod/workshop:viewallassessments',1,1413856923,0),(864,1,3,'mod/workshop:viewallassessments',1,1413856923,0),(865,1,1,'mod/workshop:viewallassessments',1,1413856923,0),(866,1,4,'mod/workshop:overridegrades',1,1413856923,0),(867,1,3,'mod/workshop:overridegrades',1,1413856923,0),(868,1,1,'mod/workshop:overridegrades',1,1413856923,0),(869,1,4,'mod/workshop:ignoredeadlines',1,1413856923,0),(870,1,3,'mod/workshop:ignoredeadlines',1,1413856923,0),(871,1,1,'mod/workshop:ignoredeadlines',1,1413856923,0),(872,1,3,'enrol/cohort:config',1,1413856938,0),(873,1,1,'enrol/cohort:config',1,1413856938,0),(874,1,1,'enrol/cohort:unenrol',1,1413856938,0),(875,1,1,'enrol/database:unenrol',1,1413856938,0),(876,1,1,'enrol/guest:config',1,1413856940,0),(877,1,3,'enrol/guest:config',1,1413856940,0),(878,1,1,'enrol/ldap:manage',1,1413856942,0),(879,1,1,'enrol/manual:config',1,1413856942,0),(880,1,1,'enrol/manual:enrol',1,1413856942,0),(881,1,3,'enrol/manual:enrol',1,1413856942,0),(882,1,1,'enrol/manual:manage',1,1413856942,0),(883,1,3,'enrol/manual:manage',1,1413856943,0),(884,1,1,'enrol/manual:unenrol',1,1413856943,0),(885,1,3,'enrol/manual:unenrol',1,1413856943,0),(886,1,1,'enrol/meta:config',1,1413856944,0),(887,1,3,'enrol/meta:config',1,1413856944,0),(888,1,1,'enrol/meta:selectaslinked',1,1413856944,0),(889,1,1,'enrol/meta:unenrol',1,1413856945,0),(890,1,1,'enrol/paypal:config',1,1413856947,0),(891,1,1,'enrol/paypal:manage',1,1413856947,0),(892,1,3,'enrol/paypal:manage',1,1413856947,0),(893,1,1,'enrol/paypal:unenrol',1,1413856948,0),(894,1,3,'enrol/self:config',1,1413856948,0),(895,1,1,'enrol/self:config',1,1413856948,0),(896,1,3,'enrol/self:manage',1,1413856949,0),(897,1,1,'enrol/self:manage',1,1413856949,0),(898,1,5,'enrol/self:unenrolself',1,1413856949,0),(899,1,3,'enrol/self:unenrol',1,1413856949,0),(900,1,1,'enrol/self:unenrol',1,1413856949,0),(901,1,7,'message/airnotifier:managedevice',1,1413856950,0),(902,1,3,'block/activity_modules:addinstance',1,1413856953,0),(903,1,1,'block/activity_modules:addinstance',1,1413856953,0),(904,1,7,'block/admin_bookmarks:myaddinstance',1,1413856954,0),(905,1,3,'block/admin_bookmarks:addinstance',1,1413856954,0),(906,1,1,'block/admin_bookmarks:addinstance',1,1413856955,0),(907,1,3,'block/badges:addinstance',1,1413856955,0),(908,1,1,'block/badges:addinstance',1,1413856955,0),(909,1,7,'block/badges:myaddinstance',1,1413856955,0),(910,1,3,'block/blog_menu:addinstance',1,1413856956,0),(911,1,1,'block/blog_menu:addinstance',1,1413856956,0),(912,1,3,'block/blog_recent:addinstance',1,1413856956,0),(913,1,1,'block/blog_recent:addinstance',1,1413856956,0),(914,1,3,'block/blog_tags:addinstance',1,1413856957,0),(915,1,1,'block/blog_tags:addinstance',1,1413856957,0),(916,1,7,'block/calendar_month:myaddinstance',1,1413856958,0),(917,1,3,'block/calendar_month:addinstance',1,1413856958,0),(918,1,1,'block/calendar_month:addinstance',1,1413856958,0),(919,1,7,'block/calendar_upcoming:myaddinstance',1,1413856959,0),(920,1,3,'block/calendar_upcoming:addinstance',1,1413856959,0),(921,1,1,'block/calendar_upcoming:addinstance',1,1413856959,0),(922,1,7,'block/comments:myaddinstance',1,1413856959,0),(923,1,3,'block/comments:addinstance',1,1413856959,0),(924,1,1,'block/comments:addinstance',1,1413856960,0),(925,1,7,'block/community:myaddinstance',1,1413856960,0),(926,1,3,'block/community:addinstance',1,1413856960,0),(927,1,1,'block/community:addinstance',1,1413856960,0),(928,1,3,'block/completionstatus:addinstance',1,1413856961,0),(929,1,1,'block/completionstatus:addinstance',1,1413856961,0),(930,1,7,'block/course_list:myaddinstance',1,1413856962,0),(931,1,3,'block/course_list:addinstance',1,1413856962,0),(932,1,1,'block/course_list:addinstance',1,1413856962,0),(933,1,7,'block/course_overview:myaddinstance',1,1413856963,0),(934,1,3,'block/course_overview:addinstance',1,1413856963,0),(935,1,1,'block/course_overview:addinstance',1,1413856963,0),(936,1,3,'block/course_summary:addinstance',1,1413856963,0),(937,1,1,'block/course_summary:addinstance',1,1413856963,0),(938,1,3,'block/feedback:addinstance',1,1413856964,0),(939,1,1,'block/feedback:addinstance',1,1413856964,0),(940,1,7,'block/glossary_random:myaddinstance',1,1413856964,0),(941,1,3,'block/glossary_random:addinstance',1,1413856964,0),(942,1,1,'block/glossary_random:addinstance',1,1413856965,0),(943,1,7,'block/html:myaddinstance',1,1413856966,0),(944,1,3,'block/html:addinstance',1,1413856966,0),(945,1,1,'block/html:addinstance',1,1413856966,0),(946,1,3,'block/login:addinstance',1,1413856966,0),(947,1,1,'block/login:addinstance',1,1413856966,0),(948,1,7,'block/mentees:myaddinstance',1,1413856967,0),(949,1,3,'block/mentees:addinstance',1,1413856967,0),(950,1,1,'block/mentees:addinstance',1,1413856967,0),(951,1,7,'block/messages:myaddinstance',1,1413856968,0),(952,1,3,'block/messages:addinstance',1,1413856968,0),(953,1,1,'block/messages:addinstance',1,1413856968,0),(954,1,7,'block/mnet_hosts:myaddinstance',1,1413856968,0),(955,1,3,'block/mnet_hosts:addinstance',1,1413856968,0),(956,1,1,'block/mnet_hosts:addinstance',1,1413856968,0),(957,1,7,'block/myprofile:myaddinstance',1,1413856969,0),(958,1,3,'block/myprofile:addinstance',1,1413856969,0),(959,1,1,'block/myprofile:addinstance',1,1413856969,0),(960,1,7,'block/navigation:myaddinstance',1,1413856970,0),(961,1,3,'block/navigation:addinstance',1,1413856970,0),(962,1,1,'block/navigation:addinstance',1,1413856970,0),(963,1,7,'block/news_items:myaddinstance',1,1413856971,0),(964,1,3,'block/news_items:addinstance',1,1413856971,0),(965,1,1,'block/news_items:addinstance',1,1413856971,0),(966,1,7,'block/online_users:myaddinstance',1,1413856972,0),(967,1,3,'block/online_users:addinstance',1,1413856972,0),(968,1,1,'block/online_users:addinstance',1,1413856972,0),(969,1,7,'block/online_users:viewlist',1,1413856972,0),(970,1,6,'block/online_users:viewlist',1,1413856972,0),(971,1,5,'block/online_users:viewlist',1,1413856972,0),(972,1,4,'block/online_users:viewlist',1,1413856972,0),(973,1,3,'block/online_users:viewlist',1,1413856972,0),(974,1,1,'block/online_users:viewlist',1,1413856972,0),(975,1,3,'block/participants:addinstance',1,1413856973,0),(976,1,1,'block/participants:addinstance',1,1413856973,0),(977,1,7,'block/private_files:myaddinstance',1,1413856973,0),(978,1,3,'block/private_files:addinstance',1,1413856973,0),(979,1,1,'block/private_files:addinstance',1,1413856973,0),(980,1,3,'block/quiz_results:addinstance',1,1413856974,0),(981,1,1,'block/quiz_results:addinstance',1,1413856974,0),(982,1,3,'block/recent_activity:addinstance',1,1413856975,0),(983,1,1,'block/recent_activity:addinstance',1,1413856975,0),(984,1,7,'block/recent_activity:viewaddupdatemodule',1,1413856975,0),(985,1,7,'block/recent_activity:viewdeletemodule',1,1413856975,0),(986,1,7,'block/rss_client:myaddinstance',1,1413856976,0),(987,1,3,'block/rss_client:addinstance',1,1413856976,0),(988,1,1,'block/rss_client:addinstance',1,1413856976,0),(989,1,4,'block/rss_client:manageownfeeds',1,1413856976,0),(990,1,3,'block/rss_client:manageownfeeds',1,1413856976,0),(991,1,1,'block/rss_client:manageownfeeds',1,1413856977,0),(992,1,1,'block/rss_client:manageanyfeeds',1,1413856977,0),(993,1,3,'block/search_forums:addinstance',1,1413856977,0),(994,1,1,'block/search_forums:addinstance',1,1413856977,0),(995,1,3,'block/section_links:addinstance',1,1413856978,0),(996,1,1,'block/section_links:addinstance',1,1413856978,0),(997,1,3,'block/selfcompletion:addinstance',1,1413856978,0),(998,1,1,'block/selfcompletion:addinstance',1,1413856978,0),(999,1,7,'block/settings:myaddinstance',1,1413856979,0),(1000,1,3,'block/settings:addinstance',1,1413856979,0),(1001,1,1,'block/settings:addinstance',1,1413856979,0),(1002,1,3,'block/site_main_menu:addinstance',1,1413856980,0),(1003,1,1,'block/site_main_menu:addinstance',1,1413856980,0),(1004,1,3,'block/social_activities:addinstance',1,1413856980,0),(1005,1,1,'block/social_activities:addinstance',1,1413856980,0),(1006,1,3,'block/tag_flickr:addinstance',1,1413856981,0),(1007,1,1,'block/tag_flickr:addinstance',1,1413856981,0),(1008,1,3,'block/tag_youtube:addinstance',1,1413856982,0),(1009,1,1,'block/tag_youtube:addinstance',1,1413856982,0),(1010,1,7,'block/tags:myaddinstance',1,1413856982,0),(1011,1,3,'block/tags:addinstance',1,1413856982,0),(1012,1,1,'block/tags:addinstance',1,1413856982,0),(1013,1,4,'report/completion:view',1,1413856998,0),(1014,1,3,'report/completion:view',1,1413856998,0),(1015,1,1,'report/completion:view',1,1413856998,0),(1016,1,4,'report/courseoverview:view',1,1413856999,0),(1017,1,3,'report/courseoverview:view',1,1413856999,0),(1018,1,1,'report/courseoverview:view',1,1413856999,0),(1019,1,4,'report/log:view',1,1413857000,0),(1020,1,3,'report/log:view',1,1413857000,0),(1021,1,1,'report/log:view',1,1413857000,0),(1022,1,4,'report/log:viewtoday',1,1413857000,0),(1023,1,3,'report/log:viewtoday',1,1413857000,0),(1024,1,1,'report/log:viewtoday',1,1413857000,0),(1025,1,4,'report/loglive:view',1,1413857001,0),(1026,1,3,'report/loglive:view',1,1413857001,0),(1027,1,1,'report/loglive:view',1,1413857001,0),(1028,1,4,'report/outline:view',1,1413857002,0),(1029,1,3,'report/outline:view',1,1413857002,0),(1030,1,1,'report/outline:view',1,1413857002,0),(1031,1,4,'report/participation:view',1,1413857003,0),(1032,1,3,'report/participation:view',1,1413857003,0),(1033,1,1,'report/participation:view',1,1413857003,0),(1034,1,1,'report/performance:view',1,1413857003,0),(1035,1,4,'report/progress:view',1,1413857004,0),(1036,1,3,'report/progress:view',1,1413857004,0),(1037,1,1,'report/progress:view',1,1413857004,0),(1038,1,1,'report/security:view',1,1413857005,0),(1039,1,4,'report/stats:view',1,1413857006,0),(1040,1,3,'report/stats:view',1,1413857006,0),(1041,1,1,'report/stats:view',1,1413857006,0),(1042,1,4,'gradeexport/ods:view',1,1413857006,0),(1043,1,3,'gradeexport/ods:view',1,1413857006,0),(1044,1,1,'gradeexport/ods:view',1,1413857006,0),(1045,1,1,'gradeexport/ods:publish',1,1413857007,0),(1046,1,4,'gradeexport/txt:view',1,1413857007,0),(1047,1,3,'gradeexport/txt:view',1,1413857007,0),(1048,1,1,'gradeexport/txt:view',1,1413857007,0),(1049,1,1,'gradeexport/txt:publish',1,1413857007,0),(1050,1,4,'gradeexport/xls:view',1,1413857008,0),(1051,1,3,'gradeexport/xls:view',1,1413857008,0),(1052,1,1,'gradeexport/xls:view',1,1413857008,0),(1053,1,1,'gradeexport/xls:publish',1,1413857008,0),(1054,1,4,'gradeexport/xml:view',1,1413857009,0),(1055,1,3,'gradeexport/xml:view',1,1413857009,0),(1056,1,1,'gradeexport/xml:view',1,1413857009,0),(1057,1,1,'gradeexport/xml:publish',1,1413857009,0),(1058,1,3,'gradeimport/csv:view',1,1413857010,0),(1059,1,1,'gradeimport/csv:view',1,1413857010,0),(1060,1,3,'gradeimport/xml:view',1,1413857011,0),(1061,1,1,'gradeimport/xml:view',1,1413857011,0),(1062,1,1,'gradeimport/xml:publish',1,1413857011,0),(1063,1,4,'gradereport/grader:view',1,1413857011,0),(1064,1,3,'gradereport/grader:view',1,1413857011,0),(1065,1,1,'gradereport/grader:view',1,1413857012,0),(1066,1,4,'gradereport/outcomes:view',1,1413857012,0),(1067,1,3,'gradereport/outcomes:view',1,1413857012,0),(1068,1,1,'gradereport/outcomes:view',1,1413857012,0),(1069,1,5,'gradereport/overview:view',1,1413857013,0),(1070,1,1,'gradereport/overview:view',1,1413857013,0),(1071,1,5,'gradereport/user:view',1,1413857013,0),(1072,1,4,'gradereport/user:view',1,1413857013,0),(1073,1,3,'gradereport/user:view',1,1413857013,0),(1074,1,1,'gradereport/user:view',1,1413857014,0),(1075,1,7,'repository/alfresco:view',1,1413857018,0),(1076,1,7,'repository/areafiles:view',1,1413857018,0),(1077,1,7,'repository/boxnet:view',1,1413857020,0),(1078,1,2,'repository/coursefiles:view',1,1413857020,0),(1079,1,4,'repository/coursefiles:view',1,1413857020,0),(1080,1,3,'repository/coursefiles:view',1,1413857020,0),(1081,1,1,'repository/coursefiles:view',1,1413857020,0),(1082,1,7,'repository/dropbox:view',1,1413857021,0),(1083,1,7,'repository/equella:view',1,1413857021,0),(1084,1,2,'repository/filesystem:view',1,1413857022,0),(1085,1,4,'repository/filesystem:view',1,1413857022,0),(1086,1,3,'repository/filesystem:view',1,1413857022,0),(1087,1,1,'repository/filesystem:view',1,1413857022,0),(1088,1,7,'repository/flickr:view',1,1413857023,0),(1089,1,7,'repository/flickr_public:view',1,1413857023,0),(1090,1,7,'repository/googledocs:view',1,1413857024,0),(1091,1,2,'repository/local:view',1,1413857025,0),(1092,1,4,'repository/local:view',1,1413857025,0),(1093,1,3,'repository/local:view',1,1413857025,0),(1094,1,1,'repository/local:view',1,1413857025,0),(1095,1,7,'repository/merlot:view',1,1413857025,0),(1096,1,7,'repository/picasa:view',1,1413857026,0),(1097,1,7,'repository/recent:view',1,1413857026,0),(1098,1,7,'repository/s3:view',1,1413857027,0),(1099,1,7,'repository/skydrive:view',1,1413857027,0),(1100,1,7,'repository/upload:view',1,1413857028,0),(1101,1,7,'repository/url:view',1,1413857029,0),(1102,1,7,'repository/user:view',1,1413857031,0),(1103,1,2,'repository/webdav:view',1,1413857031,0),(1104,1,4,'repository/webdav:view',1,1413857031,0),(1105,1,3,'repository/webdav:view',1,1413857031,0),(1106,1,1,'repository/webdav:view',1,1413857031,0),(1107,1,7,'repository/wikimedia:view',1,1413857032,0),(1108,1,7,'repository/youtube:view',1,1413857033,0),(1109,1,1,'tool/customlang:view',1,1413857047,0),(1110,1,1,'tool/customlang:edit',1,1413857047,0),(1111,1,1,'tool/uploaduser:uploaduserpictures',1,1413857056,0),(1112,1,3,'booktool/importhtml:import',1,1413857069,0),(1113,1,1,'booktool/importhtml:import',1,1413857069,0),(1114,1,6,'booktool/print:print',1,1413857070,0),(1115,1,8,'booktool/print:print',1,1413857070,0),(1116,1,5,'booktool/print:print',1,1413857070,0),(1117,1,4,'booktool/print:print',1,1413857070,0),(1118,1,3,'booktool/print:print',1,1413857070,0),(1119,1,1,'booktool/print:print',1,1413857070,0),(1120,1,4,'quiz/grading:viewstudentnames',1,1413857076,0),(1121,1,3,'quiz/grading:viewstudentnames',1,1413857076,0),(1122,1,1,'quiz/grading:viewstudentnames',1,1413857076,0),(1123,1,4,'quiz/grading:viewidnumber',1,1413857077,0),(1124,1,3,'quiz/grading:viewidnumber',1,1413857077,0),(1125,1,1,'quiz/grading:viewidnumber',1,1413857077,0),(1126,1,4,'quiz/statistics:view',1,1413857079,0),(1127,1,3,'quiz/statistics:view',1,1413857079,0),(1128,1,1,'quiz/statistics:view',1,1413857079,0),(1129,1,1,'enrol/elis:config',1,1413861461,2),(1130,1,1,'enrol/elis:unenrol',1,1413861461,2),(1131,1,1,'block/courserequest:request',1,1413861463,2),(1132,1,2,'block/courserequest:request',1,1413861463,2),(1133,1,4,'block/courserequest:request',1,1413861463,2),(1134,1,1,'block/courserequest:config',1,1413861463,2),(1135,1,2,'block/courserequest:config',1,1413861463,2),(1136,1,1,'block/courserequest:approve',1,1413861463,2),(1137,1,2,'block/courserequest:approve',1,1413861464,2),(1138,1,1,'block/courserequest:addinstance',1,1413861464,2),(1139,1,1,'block/elisadmin:addinstance',1,1413861466,2),(1140,1,1,'block/enrolsurvey:edit',1,1413861468,2),(1141,1,7,'block/enrolsurvey:take',1,1413861469,2),(1142,1,5,'block/enrolsurvey:take',1,1413861469,2),(1143,1,4,'block/enrolsurvey:take',1,1413861469,2),(1144,1,3,'block/enrolsurvey:take',1,1413861469,2),(1145,1,1,'block/enrolsurvey:take',1,1413861469,2),(1146,1,1,'block/enrolsurvey:addinstance',1,1413861469,2),(1147,1,7,'block/repository:addinstance',1,1413861470,2),(1148,1,7,'repository/elisfiles:view',1,1413861472,2),(1149,1,1,'repository/elisfiles:createsitecontent',1,1413861472,2),(1150,1,1,'repository/elisfiles:viewsitecontent',1,1413861472,2),(1151,1,3,'repository/elisfiles:createsharedcontent',1,1413861473,2),(1152,1,2,'repository/elisfiles:createsharedcontent',1,1413861473,2),(1153,1,1,'repository/elisfiles:createsharedcontent',1,1413861473,2),(1154,1,3,'repository/elisfiles:viewsharedcontent',1,1413861473,2),(1155,1,2,'repository/elisfiles:viewsharedcontent',1,1413861473,2),(1156,1,1,'repository/elisfiles:viewsharedcontent',1,1413861473,2),(1157,1,3,'repository/elisfiles:createcoursecontent',1,1413861474,2),(1158,1,2,'repository/elisfiles:createcoursecontent',1,1413861474,2),(1159,1,1,'repository/elisfiles:createcoursecontent',1,1413861474,2),(1160,1,3,'repository/elisfiles:viewcoursecontent',1,1413861474,2),(1161,1,2,'repository/elisfiles:viewcoursecontent',1,1413861474,2),(1162,1,1,'repository/elisfiles:viewcoursecontent',1,1413861474,2),(1163,1,3,'repository/elisfiles:createowncontent',1,1413861475,2),(1164,1,2,'repository/elisfiles:createowncontent',1,1413861475,2),(1165,1,1,'repository/elisfiles:createowncontent',1,1413861475,2),(1166,1,3,'repository/elisfiles:viewowncontent',1,1413861475,2),(1167,1,2,'repository/elisfiles:viewowncontent',1,1413861475,2),(1168,1,1,'repository/elisfiles:viewowncontent',1,1413861475,2),(1169,1,1,'repository/elisfiles:createusersetcontent',1,1413861475,2),(1170,1,1,'repository/elisfiles:viewusersetcontent',1,1413861476,2),(1171,1,1,'local/datahub:addinstance',1,1413861480,2),(1172,1,1,'local/elisprogram:config',1,1413861495,2),(1173,1,1,'local/elisprogram:manage',1,1413861495,2),(1174,1,1,'local/elisprogram:program_view',1,1413861496,2),(1175,1,1,'local/elisprogram:program_create',1,1413861496,2),(1176,1,1,'local/elisprogram:program_edit',1,1413861496,2),(1177,1,1,'local/elisprogram:program_delete',1,1413861496,2),(1178,1,1,'local/elisprogram:program_enrol',1,1413861496,2),(1179,1,1,'local/elisprogram:track_view',1,1413861497,2),(1180,1,1,'local/elisprogram:track_create',1,1413861497,2),(1181,1,1,'local/elisprogram:track_edit',1,1413861497,2),(1182,1,1,'local/elisprogram:track_delete',1,1413861498,2),(1183,1,1,'local/elisprogram:track_enrol',1,1413861498,2),(1184,1,1,'local/elisprogram:userset_view',1,1413861498,2),(1185,1,1,'local/elisprogram:userset_create',1,1413861499,2),(1186,1,1,'local/elisprogram:userset_edit',1,1413861499,2),(1187,1,1,'local/elisprogram:userset_delete',1,1413861499,2),(1188,1,1,'local/elisprogram:userset_enrol',1,1413861499,2),(1189,1,1,'local/elisprogram:course_view',1,1413861500,2),(1190,1,1,'local/elisprogram:course_create',1,1413861500,2),(1191,1,1,'local/elisprogram:course_edit',1,1413861500,2),(1192,1,1,'local/elisprogram:course_delete',1,1413861500,2),(1193,1,1,'local/elisprogram:class_view',1,1413861501,2),(1194,1,1,'local/elisprogram:class_create',1,1413861501,2),(1195,1,1,'local/elisprogram:class_edit',1,1413861502,2),(1196,1,1,'local/elisprogram:class_delete',1,1413861502,2),(1197,1,1,'local/elisprogram:class_enrol',1,1413861502,2),(1198,1,1,'local/elisprogram:assign_class_instructor',1,1413861503,2),(1199,1,1,'local/elisprogram:user_view',1,1413861503,2),(1200,1,1,'local/elisprogram:user_create',1,1413861503,2),(1201,1,1,'local/elisprogram:user_edit',1,1413861503,2),(1202,1,1,'local/elisprogram:user_delete',1,1413861503,2),(1203,1,7,'local/elisprogram:viewownreports',1,1413861504,2),(1204,1,1,'local/elisprogram:viewownreports',1,1413861504,2),(1205,1,1,'local/elisprogram:managefiles',1,1413861504,2),(1206,1,1,'local/elisprogram:notify_trackenrol',1,1413861504,2),(1207,1,1,'local/elisprogram:notify_classenrol',1,1413861504,2),(1208,1,1,'local/elisprogram:notify_classcomplete',1,1413861504,2),(1209,1,1,'local/elisprogram:notify_classnotstart',1,1413861505,2),(1210,1,1,'local/elisprogram:notify_classnotcomplete',1,1413861505,2),(1211,1,1,'local/elisprogram:notify_courserecurrence',1,1413861505,2),(1212,1,1,'local/elisprogram:notify_programrecurrence',1,1413861505,2),(1213,1,1,'local/elisprogram:notify_programcomplete',1,1413861506,2),(1214,1,1,'local/elisprogram:notify_programnotcomplete',1,1413861506,2),(1215,1,1,'local/elisprogram:notify_programdue',1,1413861506,2),(1216,1,1,'local/elisprogram:notify_coursedue',1,1413861506,2),(1217,1,1,'local/elisprogram:program_enrol_userset_user',1,1413861507,2),(1218,1,1,'local/elisprogram:track_enrol_userset_user',1,1413861507,2),(1219,1,1,'local/elisprogram:userset_enrol_userset_user',1,1413861507,2),(1220,1,1,'local/elisprogram:class_enrol_userset_user',1,1413861507,2),(1221,1,1,'local/elisprogram:assign_userset_user_class_instructor',1,1413861508,2),(1222,1,7,'local/elisprogram:viewcoursecatalog',1,1413861508,2),(1223,1,1,'local/elisprogram:overrideclasslimit',1,1413861508,2),(1224,1,1,'local/elisprogram:associate',1,1413861509,2),(1225,1,1,'local/elisreports:view',1,1413861517,2),(1226,1,1,'local/elisreports:schedule',1,1413861517,2),(1227,1,1,'local/elisreports:manageschedules',1,1413861517,2);

/*Table structure for table `mdl_role_context_levels` */

DROP TABLE IF EXISTS `mdl_role_context_levels`;

CREATE TABLE `mdl_role_context_levels` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `roleid` bigint(10) NOT NULL,
  `contextlevel` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_rolecontleve_conrol_uix` (`contextlevel`,`roleid`),
  KEY `mdl_rolecontleve_rol_ix` (`roleid`)
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Lists which roles can be assigned at which context levels. T';

/*Data for the table `mdl_role_context_levels` */

insert  into `mdl_role_context_levels`(`id`,`roleid`,`contextlevel`) values (1,1,10),(4,2,10),(21,9,10),(12,1,11),(22,9,11),(13,1,12),(23,9,12),(14,1,13),(24,9,13),(15,1,14),(25,9,14),(16,1,15),(26,9,15),(17,1,16),(27,9,16),(18,1,30),(28,9,30),(2,1,40),(5,2,40),(29,9,40),(3,1,50),(6,3,50),(8,4,50),(10,5,50),(30,9,50),(19,1,70),(7,3,70),(9,4,70),(11,5,70),(31,9,70),(20,1,80),(32,9,80);

/*Table structure for table `mdl_role_names` */

DROP TABLE IF EXISTS `mdl_role_names`;

CREATE TABLE `mdl_role_names` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `roleid` bigint(10) NOT NULL DEFAULT '0',
  `contextid` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_rolename_rolcon_uix` (`roleid`,`contextid`),
  KEY `mdl_rolename_rol_ix` (`roleid`),
  KEY `mdl_rolename_con_ix` (`contextid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='role names in native strings';

/*Data for the table `mdl_role_names` */

/*Table structure for table `mdl_role_sortorder` */

DROP TABLE IF EXISTS `mdl_role_sortorder`;

CREATE TABLE `mdl_role_sortorder` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL,
  `roleid` bigint(10) NOT NULL,
  `contextid` bigint(10) NOT NULL,
  `sortoder` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_rolesort_userolcon_uix` (`userid`,`roleid`,`contextid`),
  KEY `mdl_rolesort_use_ix` (`userid`),
  KEY `mdl_rolesort_rol_ix` (`roleid`),
  KEY `mdl_rolesort_con_ix` (`contextid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='sort order of course managers in a course';

/*Data for the table `mdl_role_sortorder` */

/*Table structure for table `mdl_scale` */

DROP TABLE IF EXISTS `mdl_scale`;

CREATE TABLE `mdl_scale` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `scale` longtext COLLATE utf8_unicode_ci NOT NULL,
  `description` longtext COLLATE utf8_unicode_ci NOT NULL,
  `descriptionformat` tinyint(2) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_scal_cou_ix` (`courseid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Defines grading scales';

/*Data for the table `mdl_scale` */

/*Table structure for table `mdl_scale_history` */

DROP TABLE IF EXISTS `mdl_scale_history`;

CREATE TABLE `mdl_scale_history` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `action` bigint(10) NOT NULL DEFAULT '0',
  `oldid` bigint(10) NOT NULL,
  `source` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `timemodified` bigint(10) DEFAULT NULL,
  `loggeduser` bigint(10) DEFAULT NULL,
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `scale` longtext COLLATE utf8_unicode_ci NOT NULL,
  `description` longtext COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_scalhist_act_ix` (`action`),
  KEY `mdl_scalhist_old_ix` (`oldid`),
  KEY `mdl_scalhist_cou_ix` (`courseid`),
  KEY `mdl_scalhist_log_ix` (`loggeduser`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='History table';

/*Data for the table `mdl_scale_history` */

/*Table structure for table `mdl_scorm` */

DROP TABLE IF EXISTS `mdl_scorm`;

CREATE TABLE `mdl_scorm` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `scormtype` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'local',
  `reference` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci NOT NULL,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `version` varchar(9) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `maxgrade` double NOT NULL DEFAULT '0',
  `grademethod` tinyint(2) NOT NULL DEFAULT '0',
  `whatgrade` bigint(10) NOT NULL DEFAULT '0',
  `maxattempt` bigint(10) NOT NULL DEFAULT '1',
  `forcecompleted` tinyint(1) NOT NULL DEFAULT '1',
  `forcenewattempt` tinyint(1) NOT NULL DEFAULT '0',
  `lastattemptlock` tinyint(1) NOT NULL DEFAULT '0',
  `displayattemptstatus` tinyint(1) NOT NULL DEFAULT '1',
  `displaycoursestructure` tinyint(1) NOT NULL DEFAULT '1',
  `updatefreq` tinyint(1) NOT NULL DEFAULT '0',
  `sha1hash` varchar(40) COLLATE utf8_unicode_ci DEFAULT NULL,
  `md5hash` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `revision` bigint(10) NOT NULL DEFAULT '0',
  `launch` bigint(10) NOT NULL DEFAULT '0',
  `skipview` tinyint(1) NOT NULL DEFAULT '1',
  `hidebrowse` tinyint(1) NOT NULL DEFAULT '0',
  `hidetoc` tinyint(1) NOT NULL DEFAULT '0',
  `nav` tinyint(1) NOT NULL DEFAULT '1',
  `navpositionleft` bigint(10) DEFAULT '-100',
  `navpositiontop` bigint(10) DEFAULT '-100',
  `auto` tinyint(1) NOT NULL DEFAULT '0',
  `popup` tinyint(1) NOT NULL DEFAULT '0',
  `options` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `width` bigint(10) NOT NULL DEFAULT '100',
  `height` bigint(10) NOT NULL DEFAULT '600',
  `timeopen` bigint(10) NOT NULL DEFAULT '0',
  `timeclose` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `completionstatusrequired` tinyint(1) DEFAULT NULL,
  `completionscorerequired` tinyint(2) DEFAULT NULL,
  `displayactivityname` smallint(4) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `mdl_scor_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='each table is one SCORM module and its configuration';

/*Data for the table `mdl_scorm` */

/*Table structure for table `mdl_scorm_aicc_session` */

DROP TABLE IF EXISTS `mdl_scorm_aicc_session`;

CREATE TABLE `mdl_scorm_aicc_session` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `scormid` bigint(10) NOT NULL DEFAULT '0',
  `hacpsession` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `scoid` bigint(10) DEFAULT '0',
  `scormmode` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `scormstatus` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `attempt` bigint(10) DEFAULT NULL,
  `lessonstatus` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `sessiontime` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_scoraiccsess_sco_ix` (`scormid`),
  KEY `mdl_scoraiccsess_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Used by AICC HACP to store session information';

/*Data for the table `mdl_scorm_aicc_session` */

/*Table structure for table `mdl_scorm_scoes` */

DROP TABLE IF EXISTS `mdl_scorm_scoes`;

CREATE TABLE `mdl_scorm_scoes` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `scorm` bigint(10) NOT NULL DEFAULT '0',
  `manifest` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `organization` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `parent` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `identifier` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `launch` longtext COLLATE utf8_unicode_ci NOT NULL,
  `scormtype` varchar(5) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `title` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_scorscoe_sco_ix` (`scorm`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='each SCO part of the SCORM module';

/*Data for the table `mdl_scorm_scoes` */

/*Table structure for table `mdl_scorm_scoes_data` */

DROP TABLE IF EXISTS `mdl_scorm_scoes_data`;

CREATE TABLE `mdl_scorm_scoes_data` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `scoid` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` longtext COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_scorscoedata_sco_ix` (`scoid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Contains variable data get from packages';

/*Data for the table `mdl_scorm_scoes_data` */

/*Table structure for table `mdl_scorm_scoes_track` */

DROP TABLE IF EXISTS `mdl_scorm_scoes_track`;

CREATE TABLE `mdl_scorm_scoes_track` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `scormid` bigint(10) NOT NULL DEFAULT '0',
  `scoid` bigint(10) NOT NULL DEFAULT '0',
  `attempt` bigint(10) NOT NULL DEFAULT '1',
  `element` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` longtext COLLATE utf8_unicode_ci NOT NULL,
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_scorscoetrac_usescosco_uix` (`userid`,`scormid`,`scoid`,`attempt`,`element`),
  KEY `mdl_scorscoetrac_use_ix` (`userid`),
  KEY `mdl_scorscoetrac_ele_ix` (`element`),
  KEY `mdl_scorscoetrac_sco_ix` (`scormid`),
  KEY `mdl_scorscoetrac_sco2_ix` (`scoid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='to track SCOes';

/*Data for the table `mdl_scorm_scoes_track` */

/*Table structure for table `mdl_scorm_seq_mapinfo` */

DROP TABLE IF EXISTS `mdl_scorm_seq_mapinfo`;

CREATE TABLE `mdl_scorm_seq_mapinfo` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `scoid` bigint(10) NOT NULL DEFAULT '0',
  `objectiveid` bigint(10) NOT NULL DEFAULT '0',
  `targetobjectiveid` bigint(10) NOT NULL DEFAULT '0',
  `readsatisfiedstatus` tinyint(1) NOT NULL DEFAULT '1',
  `readnormalizedmeasure` tinyint(1) NOT NULL DEFAULT '1',
  `writesatisfiedstatus` tinyint(1) NOT NULL DEFAULT '0',
  `writenormalizedmeasure` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_scorseqmapi_scoidobj_uix` (`scoid`,`id`,`objectiveid`),
  KEY `mdl_scorseqmapi_sco_ix` (`scoid`),
  KEY `mdl_scorseqmapi_obj_ix` (`objectiveid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='SCORM2004 objective mapinfo description';

/*Data for the table `mdl_scorm_seq_mapinfo` */

/*Table structure for table `mdl_scorm_seq_objective` */

DROP TABLE IF EXISTS `mdl_scorm_seq_objective`;

CREATE TABLE `mdl_scorm_seq_objective` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `scoid` bigint(10) NOT NULL DEFAULT '0',
  `primaryobj` tinyint(1) NOT NULL DEFAULT '0',
  `objectiveid` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `satisfiedbymeasure` tinyint(1) NOT NULL DEFAULT '1',
  `minnormalizedmeasure` float(11,4) NOT NULL DEFAULT '0.0000',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_scorseqobje_scoid_uix` (`scoid`,`id`),
  KEY `mdl_scorseqobje_sco_ix` (`scoid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='SCORM2004 objective description';

/*Data for the table `mdl_scorm_seq_objective` */

/*Table structure for table `mdl_scorm_seq_rolluprule` */

DROP TABLE IF EXISTS `mdl_scorm_seq_rolluprule`;

CREATE TABLE `mdl_scorm_seq_rolluprule` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `scoid` bigint(10) NOT NULL DEFAULT '0',
  `childactivityset` varchar(15) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `minimumcount` bigint(10) NOT NULL DEFAULT '0',
  `minimumpercent` float(11,4) NOT NULL DEFAULT '0.0000',
  `conditioncombination` varchar(3) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'all',
  `action` varchar(15) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_scorseqroll_scoid_uix` (`scoid`,`id`),
  KEY `mdl_scorseqroll_sco_ix` (`scoid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='SCORM2004 sequencing rule';

/*Data for the table `mdl_scorm_seq_rolluprule` */

/*Table structure for table `mdl_scorm_seq_rolluprulecond` */

DROP TABLE IF EXISTS `mdl_scorm_seq_rolluprulecond`;

CREATE TABLE `mdl_scorm_seq_rolluprulecond` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `scoid` bigint(10) NOT NULL DEFAULT '0',
  `rollupruleid` bigint(10) NOT NULL DEFAULT '0',
  `operator` varchar(5) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'noOp',
  `cond` varchar(25) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_scorseqroll_scorolid_uix` (`scoid`,`rollupruleid`,`id`),
  KEY `mdl_scorseqroll_sco2_ix` (`scoid`),
  KEY `mdl_scorseqroll_rol_ix` (`rollupruleid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='SCORM2004 sequencing rule';

/*Data for the table `mdl_scorm_seq_rolluprulecond` */

/*Table structure for table `mdl_scorm_seq_rulecond` */

DROP TABLE IF EXISTS `mdl_scorm_seq_rulecond`;

CREATE TABLE `mdl_scorm_seq_rulecond` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `scoid` bigint(10) NOT NULL DEFAULT '0',
  `ruleconditionsid` bigint(10) NOT NULL DEFAULT '0',
  `refrencedobjective` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `measurethreshold` float(11,4) NOT NULL DEFAULT '0.0000',
  `operator` varchar(5) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'noOp',
  `cond` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'always',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_scorseqrule_idscorul_uix` (`id`,`scoid`,`ruleconditionsid`),
  KEY `mdl_scorseqrule_sco2_ix` (`scoid`),
  KEY `mdl_scorseqrule_rul_ix` (`ruleconditionsid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='SCORM2004 rule condition';

/*Data for the table `mdl_scorm_seq_rulecond` */

/*Table structure for table `mdl_scorm_seq_ruleconds` */

DROP TABLE IF EXISTS `mdl_scorm_seq_ruleconds`;

CREATE TABLE `mdl_scorm_seq_ruleconds` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `scoid` bigint(10) NOT NULL DEFAULT '0',
  `conditioncombination` varchar(3) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'all',
  `ruletype` tinyint(2) NOT NULL DEFAULT '0',
  `action` varchar(25) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_scorseqrule_scoid_uix` (`scoid`,`id`),
  KEY `mdl_scorseqrule_sco_ix` (`scoid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='SCORM2004 rule conditions';

/*Data for the table `mdl_scorm_seq_ruleconds` */

/*Table structure for table `mdl_sessions` */

DROP TABLE IF EXISTS `mdl_sessions`;

CREATE TABLE `mdl_sessions` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `state` bigint(10) NOT NULL DEFAULT '0',
  `sid` varchar(128) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `userid` bigint(10) NOT NULL,
  `sessdata` longtext COLLATE utf8_unicode_ci,
  `timecreated` bigint(10) NOT NULL,
  `timemodified` bigint(10) NOT NULL,
  `firstip` varchar(45) COLLATE utf8_unicode_ci DEFAULT NULL,
  `lastip` varchar(45) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_sess_sid_uix` (`sid`),
  KEY `mdl_sess_sta_ix` (`state`),
  KEY `mdl_sess_tim_ix` (`timecreated`),
  KEY `mdl_sess_tim2_ix` (`timemodified`),
  KEY `mdl_sess_use_ix` (`userid`)
) ENGINE=InnoDB AUTO_INCREMENT=518 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Database based session storage - now recommended';

/*Data for the table `mdl_sessions` */

insert  into `mdl_sessions`(`id`,`state`,`sid`,`userid`,`sessdata`,`timecreated`,`timemodified`,`firstip`,`lastip`) values (32,0,'180n1ujicst1eug806m857knt2',4,NULL,1413863744,1413863744,'127.0.0.1','127.0.0.1'),(34,0,'ctd6k40222pbu3oc046ag0oia1',4,NULL,1413863744,1413863744,'127.0.0.1','127.0.0.1'),(36,0,'e63mke2b4vkk28npqs44osc801',4,NULL,1413863744,1413863744,'127.0.0.1','127.0.0.1'),(47,0,'h6s48uuth4t67erqc3c4fsm7l7',4,NULL,1413863825,1413863825,'127.0.0.1','127.0.0.1'),(50,0,'79ea4hspp2h1o3djo5aaq0mn57',4,NULL,1413863825,1413863825,'127.0.0.1','127.0.0.1'),(54,0,'qrtsoe9opbh2c7c1rs6oqoe102',4,NULL,1413863825,1413863825,'127.0.0.1','127.0.0.1'),(55,0,'3a6dvg3cqn9vvk2srek24g0mt5',4,NULL,1413863826,1413863826,'127.0.0.1','127.0.0.1'),(64,0,'nfadqsut3948ltcj7605a9rkf4',4,NULL,1413864150,1413864150,'127.0.0.1','127.0.0.1'),(66,0,'m3aurrnklin31g8cucl1od8ea1',4,NULL,1413864151,1413864151,'127.0.0.1','127.0.0.1'),(68,0,'690nk4jn0s4359a5fdf50k1hs3',4,NULL,1413864151,1413864151,'127.0.0.1','127.0.0.1'),(71,0,'sbpi1d6lo3kthsfguhiek81243',4,NULL,1413864151,1413864151,'127.0.0.1','127.0.0.1'),(72,0,'ko9v6kj5qkuucja1voeiegacr3',4,NULL,1413864151,1413864151,'127.0.0.1','127.0.0.1'),(79,0,'39id8admuan0cg9gftun4d8oe3',4,NULL,1413864250,1413865403,'127.0.0.1','127.0.0.1'),(80,0,'psocr8epfljdt2snkbhues9cc0',0,NULL,1413864572,1413864572,'127.0.0.1','127.0.0.1'),(81,0,'utj5hom3cj4a81jft460crpes7',0,NULL,1413864573,1413873837,'127.0.0.1','127.0.0.1'),(82,0,'vtpe868rmig2htgtfqtleac2g4',0,NULL,1413872741,1413872741,'127.0.0.1','127.0.0.1'),(83,0,'jg9js8j9o8omga0hr0hlud39p7',0,NULL,1413872741,1413872741,'127.0.0.1','127.0.0.1'),(84,0,'nuoh75dn1qttmrnig7uoqt6cv1',0,NULL,1413872741,1413872741,'127.0.0.1','127.0.0.1'),(85,0,'orn24u9p7bhg65q0apo40m2s74',0,NULL,1413872741,1413872741,'127.0.0.1','127.0.0.1'),(86,0,'b69fdn4d2cv45qubaatd363702',0,NULL,1413872741,1413872741,'127.0.0.1','127.0.0.1'),(87,0,'a9gb6lj14jfc07nb0ou4vfv8b3',0,NULL,1413872741,1413872741,'127.0.0.1','127.0.0.1'),(88,0,'qt9ac3ve2ud70k4unactdoej10',0,NULL,1413872741,1413872741,'127.0.0.1','127.0.0.1'),(89,0,'ivvvrc6sk0aqbs8iiuvlhqp6f2',0,NULL,1413872741,1413872741,'127.0.0.1','127.0.0.1'),(90,0,'hvf9vodg4v00cshhh1fm8sh8q6',0,NULL,1413872741,1413872741,'127.0.0.1','127.0.0.1'),(91,0,'4tee9990kq6sa6gbdbspmhpnf5',0,NULL,1413872741,1413872741,'127.0.0.1','127.0.0.1'),(92,0,'ool9f7dq79ephr6ijb77bbv4u7',0,NULL,1413872741,1413872741,'127.0.0.1','127.0.0.1'),(93,0,'3336krb2i8qirrlhr9gq8tkib2',0,NULL,1413872741,1413872741,'127.0.0.1','127.0.0.1'),(94,0,'d1ejjm3tjttj5d832kgfd2j6f0',0,NULL,1413872742,1413872742,'127.0.0.1','127.0.0.1'),(95,0,'ea6nbb25o2d1sb94g2oql64vj6',0,NULL,1413872742,1413872742,'127.0.0.1','127.0.0.1'),(96,0,'97pbvs245i75khdm958sfbl6s4',0,NULL,1413872742,1413872742,'127.0.0.1','127.0.0.1'),(97,0,'2rodo0m59sdie5npopsbrg7v94',0,NULL,1413872742,1413872742,'127.0.0.1','127.0.0.1'),(98,0,'qkg3mi8kgc1uijjs0na8dtr124',0,NULL,1413872742,1413872742,'127.0.0.1','127.0.0.1'),(99,0,'ca4tj1ae1sik7gon3avl13u8o7',0,NULL,1413872742,1413872742,'127.0.0.1','127.0.0.1'),(100,0,'3vd7hu16c2fod0tb2lmrdjqdd3',0,NULL,1413872742,1413872742,'127.0.0.1','127.0.0.1'),(101,0,'v4vhvllnl2a3ek6m47ircqfks5',0,NULL,1413872742,1413872742,'127.0.0.1','127.0.0.1'),(102,0,'e8t1e92m96jd8iolt0upn90r43',0,NULL,1413872742,1413872742,'127.0.0.1','127.0.0.1'),(103,0,'jhrs6uvggq4se03a87r0epbgk5',0,NULL,1413872778,1413872778,'127.0.0.1','127.0.0.1'),(104,0,'olcklimikc2c3m97stej1p8ti4',0,NULL,1413872778,1413872778,'127.0.0.1','127.0.0.1'),(105,0,'v9udk64e38g2q1s6jflro9al97',0,NULL,1413872778,1413872778,'127.0.0.1','127.0.0.1'),(106,0,'cobe7iprv03hkla2vjesh6lkc7',0,NULL,1413872778,1413872778,'127.0.0.1','127.0.0.1'),(107,0,'h1vs7jabdr1g8oonnicpv4g9s5',0,NULL,1413872778,1413872778,'127.0.0.1','127.0.0.1'),(108,0,'kb8n5o67kd5312ottosovtml16',0,NULL,1413872778,1413872778,'127.0.0.1','127.0.0.1'),(109,0,'295j3kgbt5f34u96shdivus3i6',0,NULL,1413872778,1413872778,'127.0.0.1','127.0.0.1'),(110,0,'4a6d5o4pj0d99b7e9mee559fv6',0,NULL,1413872778,1413872778,'127.0.0.1','127.0.0.1'),(111,0,'td0v7gbbooh463inrv1tuoq140',0,NULL,1413872778,1413872778,'127.0.0.1','127.0.0.1'),(112,0,'1p5pr8e2c9mmcd8d0nnb76nra6',0,NULL,1413872778,1413872778,'127.0.0.1','127.0.0.1'),(113,0,'4v5ih1uiog9l1vseefqsur68d2',0,NULL,1413872778,1413872778,'127.0.0.1','127.0.0.1'),(114,0,'uspt5m0vsqtc62bbgtc5ouakd1',0,NULL,1413872778,1413872778,'127.0.0.1','127.0.0.1'),(115,0,'p2dn3qm15anejcqjrqptrbogj4',0,NULL,1413872778,1413872778,'127.0.0.1','127.0.0.1'),(116,0,'4p8cf1tkd1u22q3uljq39lasm3',0,NULL,1413872779,1413872779,'127.0.0.1','127.0.0.1'),(117,0,'1nklb2pm73ui17vpqhimspqc71',0,NULL,1413872779,1413872779,'127.0.0.1','127.0.0.1'),(118,0,'dlhibi1nctg9ho4lcnnonsffd3',0,NULL,1413872779,1413872779,'127.0.0.1','127.0.0.1'),(119,0,'53chof0j0bd7kbafgd6f9du0n2',0,NULL,1413872779,1413872779,'127.0.0.1','127.0.0.1'),(120,0,'c8bjuf53ujk6igemopd92vqv06',0,NULL,1413872779,1413872779,'127.0.0.1','127.0.0.1'),(121,0,'m3mk50a5lsftk86dcsm6u7sjb4',0,NULL,1413872779,1413872779,'127.0.0.1','127.0.0.1'),(122,0,'995ubnrobbptsh9n8tfcodcab6',0,NULL,1413872779,1413872779,'127.0.0.1','127.0.0.1'),(123,0,'ciqb3g45r5mln8pjhlhca4i467',0,NULL,1413872779,1413872779,'127.0.0.1','127.0.0.1'),(124,0,'dbhfh9rml4vnh4g4dsooh6t4t2',0,NULL,1413872781,1413872781,'127.0.0.1','127.0.0.1'),(125,0,'j6krddpbidfkqhbjjhkb66h2o0',0,NULL,1413872781,1413872781,'127.0.0.1','127.0.0.1'),(126,0,'0eb83u5khd8kio3m7h7gtrv296',0,NULL,1413872781,1413872781,'127.0.0.1','127.0.0.1'),(127,0,'edd8v7hj53tieujk5tggn6fkb4',0,NULL,1413872781,1413872781,'127.0.0.1','127.0.0.1'),(128,0,'cos0770gpul9snpooch5hau7m0',0,NULL,1413872781,1413872781,'127.0.0.1','127.0.0.1'),(129,0,'vidclf40v40b0c4md1rs61hmk5',0,NULL,1413872781,1413872781,'127.0.0.1','127.0.0.1'),(130,0,'10k5lpl4mss201ftm1ihpc8nq0',0,NULL,1413872781,1413872781,'127.0.0.1','127.0.0.1'),(131,0,'5tsvt8o341uba1nu9k4erpado0',0,NULL,1413872781,1413872781,'127.0.0.1','127.0.0.1'),(132,0,'p63tfjg25v74og4uctgk007j44',0,NULL,1413872781,1413872781,'127.0.0.1','127.0.0.1'),(133,0,'bof1fah891n05dlvkrlqaueh32',0,NULL,1413872781,1413872781,'127.0.0.1','127.0.0.1'),(134,0,'gpth0d3o30bres95nuql64mga7',0,NULL,1413872781,1413872781,'127.0.0.1','127.0.0.1'),(135,0,'08riim4hi2fv194atrj26ojfb3',0,NULL,1413872781,1413872781,'127.0.0.1','127.0.0.1'),(136,0,'a0mfls808d8rbfiigep8vb6uh0',0,NULL,1413872781,1413872781,'127.0.0.1','127.0.0.1'),(137,0,'681mnv7sjfv161tcp7mv8j0260',0,NULL,1413872782,1413872782,'127.0.0.1','127.0.0.1'),(138,0,'40s3aik7tdo4ul35veg83ap0f7',0,NULL,1413872782,1413872782,'127.0.0.1','127.0.0.1'),(139,0,'7o6u4hrsnk3q94vdp22q7v8b40',0,NULL,1413872782,1413872782,'127.0.0.1','127.0.0.1'),(140,0,'5s4u2hmbtibmlk2l16buno7ig4',0,NULL,1413872782,1413872782,'127.0.0.1','127.0.0.1'),(141,0,'b0cokiu18j6mrps23rui6lejd1',0,NULL,1413872782,1413872782,'127.0.0.1','127.0.0.1'),(142,0,'9a0k65uilva73r8mfqh4p8e511',0,NULL,1413872782,1413872782,'127.0.0.1','127.0.0.1'),(143,0,'0s4b1bms4s5564l31qg2aukv90',0,NULL,1413872782,1413872782,'127.0.0.1','127.0.0.1'),(145,0,'ng9qjmilbqrhnks4i8iuie6pp1',0,NULL,1413872893,1413872893,'127.0.0.1','127.0.0.1'),(146,0,'ne8k6ihh9hvuv00pbgir1u7oe2',0,NULL,1413872905,1413872905,'127.0.0.1','127.0.0.1'),(147,0,'0fvuc886ufupvc7t3d4l3savr3',0,NULL,1413872905,1413872905,'127.0.0.1','127.0.0.1'),(148,0,'1mn49g0u19rvaa9u4ed2fs07s0',0,NULL,1413872905,1413872905,'127.0.0.1','127.0.0.1'),(149,0,'7ho1hiee1034l0mjcvt19tj9n0',0,NULL,1413872905,1413872905,'127.0.0.1','127.0.0.1'),(150,0,'jqml2bdddv2koe0889s7vjka45',0,NULL,1413872905,1413872905,'127.0.0.1','127.0.0.1'),(151,0,'33l78n0iskn2a5iovndeksiil3',0,NULL,1413872905,1413872905,'127.0.0.1','127.0.0.1'),(152,0,'n1kip1mp70d4em433lur4t0di1',0,NULL,1413872905,1413872905,'127.0.0.1','127.0.0.1'),(153,0,'d31kt9ghq0rko55es8qt6g87t7',0,NULL,1413872906,1413872906,'127.0.0.1','127.0.0.1'),(154,0,'1bulu82fjhlcig77rhe8jtuhl7',0,NULL,1413872906,1413872906,'127.0.0.1','127.0.0.1'),(155,0,'8d347sf54evd726sd5g26v1kh0',0,NULL,1413872906,1413872906,'127.0.0.1','127.0.0.1'),(156,0,'jn91g8gccv5vb24ah3jcilk3h7',0,NULL,1413872906,1413872906,'127.0.0.1','127.0.0.1'),(157,0,'4anlonelnv7m65tuboktd4fok0',0,NULL,1413872906,1413872906,'127.0.0.1','127.0.0.1'),(158,0,'ll8annldjhehaqo37v4lmv3hl1',0,NULL,1413872906,1413872906,'127.0.0.1','127.0.0.1'),(159,0,'vghqi4uu5ljpmc1imqeq9bgdu6',0,NULL,1413872906,1413872906,'127.0.0.1','127.0.0.1'),(160,0,'9t9st876q5pvd3snfh3q333gc7',0,NULL,1413872906,1413872906,'127.0.0.1','127.0.0.1'),(161,0,'fgsodgiogsmfmj83jh4feo9n14',0,NULL,1413872906,1413872906,'127.0.0.1','127.0.0.1'),(162,0,'8d7urje36qs7etlqs4rpleofb1',0,NULL,1413872906,1413872906,'127.0.0.1','127.0.0.1'),(163,0,'7jrgcfamn6gj5l7e3dlcavu631',0,NULL,1413872906,1413872906,'127.0.0.1','127.0.0.1'),(164,0,'r7bekh7nmur5ut38ds4rnrp506',0,NULL,1413872906,1413872906,'127.0.0.1','127.0.0.1'),(165,0,'1g2r8dilceofalj6c06tgc9fi4',0,NULL,1413872906,1413872906,'127.0.0.1','127.0.0.1'),(166,0,'cm3cgnl7robgkn5aijgauoffi1',0,NULL,1413872907,1413872907,'127.0.0.1','127.0.0.1'),(167,0,'j8regjstrqd3g5njft6klp2e60',0,NULL,1413872907,1413872907,'127.0.0.1','127.0.0.1'),(168,0,'fkc5ac4o4kle2bp8ls19fl6qq5',0,NULL,1413872907,1413872907,'127.0.0.1','127.0.0.1'),(169,0,'hg6uei9lsbs34tjnl25re3cau7',0,NULL,1413872907,1413872907,'127.0.0.1','127.0.0.1'),(170,0,'mp94r5v3jtsgkma76680bii9l0',0,NULL,1413872907,1413872907,'127.0.0.1','127.0.0.1'),(171,0,'flcif26h5scim8plv872e3g5l1',0,NULL,1413872907,1413872907,'127.0.0.1','127.0.0.1'),(172,0,'m9feqmf2b3v7ftk3241qbgbqp3',0,NULL,1413872907,1413872907,'127.0.0.1','127.0.0.1'),(173,0,'c3va90vdfrkbq56jjmdv68f844',0,NULL,1413872907,1413872907,'127.0.0.1','127.0.0.1'),(174,0,'d7c0r49qoopd9fgjmfq0nrabm7',0,NULL,1413872907,1413872907,'127.0.0.1','127.0.0.1'),(175,0,'l3k6257nrb3kjg43u7drkbrd50',0,NULL,1413872907,1413872907,'127.0.0.1','127.0.0.1'),(176,0,'9jqvidvqp313tkf5a8fi7ulca2',0,NULL,1413872907,1413872907,'127.0.0.1','127.0.0.1'),(177,0,'cdqtet58s8fqm4mgjitljogao1',0,NULL,1413872908,1413872908,'127.0.0.1','127.0.0.1'),(178,0,'76crpg9tnphmn88t30mvsic0u2',0,NULL,1413872908,1413872908,'127.0.0.1','127.0.0.1'),(179,0,'tr1se2r1qdbd1mspc51ruubue2',0,NULL,1413872908,1413872908,'127.0.0.1','127.0.0.1'),(180,0,'8hmg7prk7fivtfb1rhbdkv3i55',0,NULL,1413872908,1413872908,'127.0.0.1','127.0.0.1'),(181,0,'6uuso6l6hs229r5ojderbd3e60',0,NULL,1413872908,1413872908,'127.0.0.1','127.0.0.1'),(182,0,'q5878dbsgaeug1430fgt17h9t0',0,NULL,1413872908,1413872908,'127.0.0.1','127.0.0.1'),(183,0,'8mkd119aiu5n462h9ec9ib74t6',0,NULL,1413872908,1413872908,'127.0.0.1','127.0.0.1'),(184,0,'vgr2poavg84qadthotp63ror51',0,NULL,1413872908,1413872908,'127.0.0.1','127.0.0.1'),(185,0,'ahifaatm2106h9japfhkf9a484',0,NULL,1413872908,1413872908,'127.0.0.1','127.0.0.1'),(186,0,'eemtarpld2lo95it63d6sh1fq6',0,NULL,1413872908,1413872908,'127.0.0.1','127.0.0.1'),(187,0,'bimocj3089ofgh7n2g1uql3kg6',0,NULL,1413872908,1413872908,'127.0.0.1','127.0.0.1'),(188,0,'26m1b3494s3qhm2dfo85o5hu07',0,NULL,1413872908,1413872908,'127.0.0.1','127.0.0.1'),(189,0,'0l2tk9gactv3pb81ogvl0tscl0',0,NULL,1413872908,1413872908,'127.0.0.1','127.0.0.1'),(190,0,'0nhnb6140igkd4jht9o6j2jga6',0,NULL,1413872908,1413872908,'127.0.0.1','127.0.0.1'),(191,0,'v7b265rk9eqlnihs731r1gcsv4',0,NULL,1413872909,1413872909,'127.0.0.1','127.0.0.1'),(192,0,'6sb4jd883d9rlrsu2hcunkvfu5',0,NULL,1413872909,1413872909,'127.0.0.1','127.0.0.1'),(193,0,'f9vddi9qaoskng5r8n5i1a0j71',0,NULL,1413872909,1413872909,'127.0.0.1','127.0.0.1'),(194,0,'ou1u7594a6v1881qgfi74l7u45',0,NULL,1413872909,1413872909,'127.0.0.1','127.0.0.1'),(195,0,'pah1cgi8jdguaa4i9i1b36gni1',0,NULL,1413872909,1413872909,'127.0.0.1','127.0.0.1'),(196,0,'p238ivsrjig5vu6kivrgldsg70',0,NULL,1413872909,1413872909,'127.0.0.1','127.0.0.1'),(197,0,'rjvfckse86t1eo9s735gajlh00',0,NULL,1413872909,1413872909,'127.0.0.1','127.0.0.1'),(198,0,'o0rlsdv83sthsb0ojkkl8ifah6',0,NULL,1413872909,1413872909,'127.0.0.1','127.0.0.1'),(199,0,'d8l06erlmc1p9aalfc4q00uip2',0,NULL,1413872909,1413872909,'127.0.0.1','127.0.0.1'),(200,0,'2lllcept78tb2l42vkmou3a1l0',0,NULL,1413872910,1413872910,'127.0.0.1','127.0.0.1'),(201,0,'g6lri975isl09up78sitddumg1',0,NULL,1413872911,1413872911,'127.0.0.1','127.0.0.1'),(202,0,'o3pgcgkn9kagi3ogjf5uu39pk7',0,NULL,1413872911,1413872911,'127.0.0.1','127.0.0.1'),(203,0,'trv1t5qa8au1jnq0kfq9bin1t4',0,NULL,1413872911,1413872911,'127.0.0.1','127.0.0.1'),(204,0,'u26nnimjq72bh8jn6l5v7hp2f6',0,NULL,1413872911,1413872911,'127.0.0.1','127.0.0.1'),(205,0,'bekv99aqivsvck1egmnufhhh26',0,NULL,1413872911,1413872911,'127.0.0.1','127.0.0.1'),(206,0,'en7movjqaf5aa0jo087dig0mg4',0,NULL,1413872911,1413872911,'127.0.0.1','127.0.0.1'),(207,0,'27c6olkpn2n6dfu6furpn65195',0,NULL,1413872911,1413872911,'127.0.0.1','127.0.0.1'),(208,0,'j4blh8kqjng0uj90j292ojamb3',0,NULL,1413872911,1413872911,'127.0.0.1','127.0.0.1'),(209,0,'0nmfuqgpkqaug1itbsc2j0g9u2',0,NULL,1413872911,1413872911,'127.0.0.1','127.0.0.1'),(210,0,'bj24d7tuguas673p813fdhskc4',0,NULL,1413872911,1413872911,'127.0.0.1','127.0.0.1'),(211,0,'lt8eqvrnsf85v7e48stdmd6034',0,NULL,1413872911,1413872911,'127.0.0.1','127.0.0.1'),(212,0,'kjuv1s2k258cul1v9ac7kd7jp3',0,NULL,1413872911,1413872911,'127.0.0.1','127.0.0.1'),(213,0,'t6pn3nrdhfgpkld8bovs95rg00',0,NULL,1413872912,1413872912,'127.0.0.1','127.0.0.1'),(214,0,'8r12gqg7pa2tnkd5lu98q2ekb5',0,NULL,1413872912,1413872912,'127.0.0.1','127.0.0.1'),(215,0,'jgc8s5s6r1pvrf0up9i68jghc2',0,NULL,1413872912,1413872912,'127.0.0.1','127.0.0.1'),(216,0,'15gui6jjrh7e617lc7g6ohhsd6',0,NULL,1413872912,1413872912,'127.0.0.1','127.0.0.1'),(217,0,'bnuq8vd5a85e03cm2p78g8mug2',0,NULL,1413872912,1413872912,'127.0.0.1','127.0.0.1'),(218,0,'jr43t7ng3mklagg6ifj5fr86l2',0,NULL,1413872912,1413872912,'127.0.0.1','127.0.0.1'),(219,0,'8ubunkcmsha5nb273n4ae78hs6',0,NULL,1413872912,1413872912,'127.0.0.1','127.0.0.1'),(222,0,'mk9bshei3gnbgr1gpkvhsql467',2,NULL,1413872932,1413872986,'127.0.0.1','127.0.0.1'),(227,0,'ah287gee22m9nuerq8c7d6ujj4',4,NULL,1413873277,1413873277,'127.0.0.1','127.0.0.1'),(229,0,'tdbjdj97po01crufg1ulp70503',4,NULL,1413873278,1413873278,'127.0.0.1','127.0.0.1'),(234,0,'hk8fc4nljrf3jdj51jq74a6af5',0,NULL,1413873321,1413873321,'127.0.0.1','127.0.0.1'),(235,0,'1qgj9ps4ngu065fn6n9l2ijm80',0,NULL,1413873324,1413873324,'127.0.0.1','127.0.0.1'),(237,0,'dbd8as8sfleaud6e57hbegh3r7',4,NULL,1413873324,1413873324,'127.0.0.1','127.0.0.1'),(238,0,'mf1svbn26mhllmigkod6r5lg60',0,NULL,1413873324,1413873324,'127.0.0.1','127.0.0.1'),(239,0,'ipmfpfr7ojb67q49vsl3hhlrb1',0,NULL,1413873331,1413873331,'127.0.0.1','127.0.0.1'),(241,0,'2j9vrimpp54k1euei3gl3ahoc4',4,NULL,1413873331,1413873331,'127.0.0.1','127.0.0.1'),(242,0,'7cat9f59h37h3f7hjbp99kn255',0,NULL,1413873331,1413873331,'127.0.0.1','127.0.0.1'),(243,0,'cf9q6mjqqpefig67ailtpskjo0',0,NULL,1413873334,1413873334,'127.0.0.1','127.0.0.1'),(244,0,'5feunaf4ar0gk6ogn41deatkb4',0,NULL,1413873336,1413873336,'127.0.0.1','127.0.0.1'),(245,0,'hlrgenn4719ht8upcefel523e7',0,NULL,1413873336,1413873336,'127.0.0.1','127.0.0.1'),(246,0,'kkb8kpgj56s5h202bhapicse73',0,NULL,1413873336,1413873336,'127.0.0.1','127.0.0.1'),(247,0,'otrqpgr07dccivclv6rt5i06r1',0,NULL,1413873336,1413873336,'127.0.0.1','127.0.0.1'),(248,0,'gl94nb2gsdl2ijf11f3eumb8k6',0,NULL,1413873336,1413873336,'127.0.0.1','127.0.0.1'),(249,0,'o5q7qjuia9lspkfrcj50plh2k0',0,NULL,1413873336,1413873336,'127.0.0.1','127.0.0.1'),(250,0,'pr7q830m71gb3sof9gndh00c01',0,NULL,1413873336,1413873336,'127.0.0.1','127.0.0.1'),(251,0,'g9tfbr616lgq1pv5dv52ris510',0,NULL,1413873336,1413873336,'127.0.0.1','127.0.0.1'),(252,0,'tabq8et1i6kr8hord4rpqoc205',0,NULL,1413873336,1413873336,'127.0.0.1','127.0.0.1'),(253,0,'g1hf3tp5cuj8ioksuag27ksk41',0,NULL,1413873336,1413873336,'127.0.0.1','127.0.0.1'),(254,0,'dio0resffldbikgujpr2ctdjs2',0,NULL,1413873337,1413873337,'127.0.0.1','127.0.0.1'),(255,0,'tu6hvta2tcbhnuv2njn6kcg9t6',0,NULL,1413873337,1413873337,'127.0.0.1','127.0.0.1'),(256,0,'8ou86avv314a5g40gkdj3nugu3',0,NULL,1413873337,1413873337,'127.0.0.1','127.0.0.1'),(257,0,'04molvb8mlrtsph7p75n4kh5r4',0,NULL,1413873337,1413873337,'127.0.0.1','127.0.0.1'),(258,0,'bl8p22nl18qcgi4h4dtp6mb800',0,NULL,1413873337,1413873337,'127.0.0.1','127.0.0.1'),(259,0,'58ei7d7p36pek9gop4a3bg37a5',0,NULL,1413873337,1413873337,'127.0.0.1','127.0.0.1'),(260,0,'9ttkjudstkgdr375bsh9n17624',0,NULL,1413873337,1413873337,'127.0.0.1','127.0.0.1'),(261,0,'p658h16j0uj8sm98sog5mvn060',0,NULL,1413873337,1413873337,'127.0.0.1','127.0.0.1'),(262,0,'vt1r7divrlvcr5t1jbdp6pmj53',0,NULL,1413873337,1413873337,'127.0.0.1','127.0.0.1'),(263,0,'ntrc9un8b4aa0q9d4o3orhc452',0,NULL,1413873337,1413873337,'127.0.0.1','127.0.0.1'),(264,0,'n3fb00iam4v5un3aalasmfr351',0,NULL,1413873337,1413873337,'127.0.0.1','127.0.0.1'),(265,0,'992rnkal8qumt7beasjhqjmsd7',0,NULL,1413873337,1413873337,'127.0.0.1','127.0.0.1'),(266,0,'l1a8dqm7kghb8kgnibc6b0oi77',0,NULL,1413873337,1413873337,'127.0.0.1','127.0.0.1'),(267,0,'tg9mo89bol9l9a92uc2n7echr4',0,NULL,1413873338,1413873338,'127.0.0.1','127.0.0.1'),(268,0,'p0uhbvlbljjv4bfle0o8o8inj6',0,NULL,1413873338,1413873338,'127.0.0.1','127.0.0.1'),(269,0,'0gl2k157gb6vqpvqkro6klsl25',0,NULL,1413873338,1413873338,'127.0.0.1','127.0.0.1'),(270,0,'59lt4aj1n3s89rsb3ch07jrj75',0,NULL,1413873338,1413873338,'127.0.0.1','127.0.0.1'),(271,0,'egkblp86drga6c7u41mrmffpl4',0,NULL,1413873338,1413873338,'127.0.0.1','127.0.0.1'),(272,0,'8m3u7rgi83lc2v7jlq34trfnp1',0,NULL,1413873338,1413873338,'127.0.0.1','127.0.0.1'),(273,0,'2vfbgqvaqpola2pjrj4ldjhe41',0,NULL,1413873338,1413873338,'127.0.0.1','127.0.0.1'),(274,0,'q2tg7u9boijbunfso369iiq662',0,NULL,1413873338,1413873338,'127.0.0.1','127.0.0.1'),(275,0,'t7dd8j7j3e9bm5em4f8qm1ki12',0,NULL,1413873338,1413873338,'127.0.0.1','127.0.0.1'),(276,0,'1ogrrb1nq0sv0td15uupuuokg6',0,NULL,1413873338,1413873338,'127.0.0.1','127.0.0.1'),(277,0,'ftiu2bak3m2j02djft4dhr9pn1',0,NULL,1413873338,1413873338,'127.0.0.1','127.0.0.1'),(278,0,'kfpf08n90eah9vktd7attcchq3',0,NULL,1413873338,1413873338,'127.0.0.1','127.0.0.1'),(279,0,'m1kibs5scambnsnop9vkvfvlc3',0,NULL,1413873338,1413873338,'127.0.0.1','127.0.0.1'),(280,0,'i4rngqk736bj2voovueedeocn4',0,NULL,1413873338,1413873338,'127.0.0.1','127.0.0.1'),(281,0,'iq28up5irha17j6o1q2dsav1t5',0,NULL,1413873339,1413873339,'127.0.0.1','127.0.0.1'),(282,0,'tv8i9ag9f4hcfec87vnhlnsmu2',0,NULL,1413873339,1413873339,'127.0.0.1','127.0.0.1'),(283,0,'ij3gll6hfrf5isppd8v7nfjea0',0,NULL,1413873339,1413873339,'127.0.0.1','127.0.0.1'),(284,0,'uje5g0miku8op9f9cfnj9e5me4',0,NULL,1413873339,1413873339,'127.0.0.1','127.0.0.1'),(285,0,'vjj194c0je9u8tvqasio4jp4q1',0,NULL,1413873339,1413873339,'127.0.0.1','127.0.0.1'),(286,0,'s051in53tma7si1og3l22m8ra0',0,NULL,1413873339,1413873339,'127.0.0.1','127.0.0.1'),(287,0,'sosij140fvegqr8ai0851clck0',0,NULL,1413873339,1413873339,'127.0.0.1','127.0.0.1'),(288,0,'luf1i7vbem965pb93bct831gt6',0,NULL,1413873339,1413873339,'127.0.0.1','127.0.0.1'),(289,0,'gat410379rsu9j08fgrfip8bc4',0,NULL,1413873339,1413873339,'127.0.0.1','127.0.0.1'),(290,0,'v7gm650nv0lqsk4qpjmgqh10h5',0,NULL,1413873339,1413873339,'127.0.0.1','127.0.0.1'),(291,0,'2u6efjr841kj2dmuooa2cbl767',0,NULL,1413873339,1413873339,'127.0.0.1','127.0.0.1'),(292,0,'nrke4im90cg57p3an3rs0atat6',0,NULL,1413873339,1413873339,'127.0.0.1','127.0.0.1'),(293,0,'19jp6v6bohc4buupejiucs5481',0,NULL,1413873339,1413873339,'127.0.0.1','127.0.0.1'),(294,0,'747lghn976tf6jea25h912m7s2',0,NULL,1413873340,1413873340,'127.0.0.1','127.0.0.1'),(295,0,'th5l1ti2u0cfrtegk9bnprf167',0,NULL,1413873340,1413873340,'127.0.0.1','127.0.0.1'),(296,0,'0hc9og0mutl32ln4lvrcflg9c2',0,NULL,1413873340,1413873340,'127.0.0.1','127.0.0.1'),(297,0,'khvcto37sdh1n6s28sda7vquj1',0,NULL,1413873340,1413873340,'127.0.0.1','127.0.0.1'),(298,0,'h5rg90rb7sgf22q8lug2jn6pg1',0,NULL,1413873340,1413873340,'127.0.0.1','127.0.0.1'),(299,0,'g6mk34ld2lft9t9cfte5eoiol6',0,NULL,1413873340,1413873340,'127.0.0.1','127.0.0.1'),(300,0,'q9c7shmm2ec7rmfo9uf5e4qbt5',0,NULL,1413873340,1413873340,'127.0.0.1','127.0.0.1'),(301,0,'srmjvpd41anc2isr5j30neod64',0,NULL,1413873340,1413873340,'127.0.0.1','127.0.0.1'),(302,0,'ftjh4rp5d1683ene5s58l4unu2',0,NULL,1413873340,1413873340,'127.0.0.1','127.0.0.1'),(303,0,'3o6lchdeucd6s2cbqartvag682',0,NULL,1413873340,1413873340,'127.0.0.1','127.0.0.1'),(304,0,'tdh0h7s519bncss3tgr3bh0370',0,NULL,1413873340,1413873340,'127.0.0.1','127.0.0.1'),(305,0,'hgpvnsi4o9b4irccnuid6eviu5',0,NULL,1413873340,1413873340,'127.0.0.1','127.0.0.1'),(306,0,'asamn8mkhqiok150e3lpm2t5k2',0,NULL,1413873340,1413873340,'127.0.0.1','127.0.0.1'),(307,0,'magitan6p1mut9j5l889lu61k5',0,NULL,1413873340,1413873340,'127.0.0.1','127.0.0.1'),(308,0,'tdc0o0gbajd05c8apcof9dg3e0',0,NULL,1413873341,1413873341,'127.0.0.1','127.0.0.1'),(309,0,'ra1pnilu8t2ir74884gciatcg1',0,NULL,1413873341,1413873341,'127.0.0.1','127.0.0.1'),(310,0,'8145u529rg2ff3ascplfkjsjo3',0,NULL,1413873341,1413873341,'127.0.0.1','127.0.0.1'),(311,0,'ovpi0drm09ofdb8eu97k3dnuh0',0,NULL,1413873341,1413873341,'127.0.0.1','127.0.0.1'),(312,0,'3r8givatdrco0o1akhfgdn37k5',0,NULL,1413873341,1413873341,'127.0.0.1','127.0.0.1'),(313,0,'9u0h0rjrljjkpnaaq57mp06lo7',0,NULL,1413873341,1413873341,'127.0.0.1','127.0.0.1'),(314,0,'4790jfumsd7i80bodkjat68ec1',0,NULL,1413873341,1413873341,'127.0.0.1','127.0.0.1'),(315,0,'d5lgp1707n6kqmaedkfdbeqdr7',0,NULL,1413873341,1413873341,'127.0.0.1','127.0.0.1'),(316,0,'fk4b5h585a723ahde6m4kgsqt2',0,NULL,1413873343,1413873343,'127.0.0.1','127.0.0.1'),(317,0,'vi30mhcb0v9fce2mbfaor29951',0,NULL,1413873343,1413873343,'127.0.0.1','127.0.0.1'),(318,0,'f3n94l9q5r1fso1hgemrifv3v2',0,NULL,1413873344,1413873344,'127.0.0.1','127.0.0.1'),(319,0,'cokkm192pfh901j0i5tai46tv1',0,NULL,1413873344,1413873344,'127.0.0.1','127.0.0.1'),(320,0,'3r1mi5883tmqmoslchav1ujp06',0,NULL,1413873344,1413873344,'127.0.0.1','127.0.0.1'),(321,0,'8sdlsjs747ip3ji68k03cj68f0',0,NULL,1413873344,1413873344,'127.0.0.1','127.0.0.1'),(322,0,'r85ipsbom933fnh6obap8gjpl0',0,NULL,1413873344,1413873344,'127.0.0.1','127.0.0.1'),(323,0,'cuplnnkplaq9v4756vth5rd4b5',0,NULL,1413873344,1413873344,'127.0.0.1','127.0.0.1'),(324,0,'a2nhfc0ema94vlovp61a2efan3',0,NULL,1413873344,1413873344,'127.0.0.1','127.0.0.1'),(325,0,'u9bd7ia72f5eiviolmep71s956',0,NULL,1413873344,1413873344,'127.0.0.1','127.0.0.1'),(326,0,'p4s9hgkaqhcaqp9rlmpitvecp3',0,NULL,1413873344,1413873344,'127.0.0.1','127.0.0.1'),(327,0,'frmen1i4fi18ms7bf29drvhoo1',0,NULL,1413873344,1413873344,'127.0.0.1','127.0.0.1'),(328,0,'4cir5aanknues6olti3co75ej3',0,NULL,1413873344,1413873344,'127.0.0.1','127.0.0.1'),(329,0,'l023kbh860f04gopd70h2jept3',0,NULL,1413873344,1413873344,'127.0.0.1','127.0.0.1'),(330,0,'ncmpdocur6qkkl5egiurm1ua12',0,NULL,1413873345,1413873345,'127.0.0.1','127.0.0.1'),(331,0,'fvqrdlb55hsuud15f4sn3hl4i0',0,NULL,1413873345,1413873345,'127.0.0.1','127.0.0.1'),(332,0,'mq0u7aulu3jpdudfs9ucims536',0,NULL,1413873345,1413873345,'127.0.0.1','127.0.0.1'),(333,0,'8eeet5le1gm4luvc31bhco7mg1',0,NULL,1413873345,1413873345,'127.0.0.1','127.0.0.1'),(334,0,'vrmt12k997m1stkggfi8qvfud2',0,NULL,1413873345,1413873345,'127.0.0.1','127.0.0.1'),(335,0,'5gbejhg4f7h2ttldnujd6a0t74',0,NULL,1413873345,1413873345,'127.0.0.1','127.0.0.1'),(339,0,'gsg1rqp104292des2j95qqe8n2',0,NULL,1413873404,1413873404,'127.0.0.1','127.0.0.1'),(340,0,'t54nt0cgjko21gq17ggfajdu51',2,NULL,1413873428,1413873428,'127.0.0.1','127.0.0.1'),(341,0,'i19hr5nja2mflskkeprj6tmnp0',0,NULL,1413873428,1413873428,'127.0.0.1','127.0.0.1'),(343,0,'u3ahrckak19shjert47g6e3833',2,NULL,1413873432,1413873432,'127.0.0.1','127.0.0.1'),(344,0,'9so9tki8r68b5idab307e0fbd0',0,NULL,1413873432,1413873432,'127.0.0.1','127.0.0.1'),(346,0,'no6k2977v0emngu0ljc11gvfa4',2,NULL,1413873434,1413873434,'127.0.0.1','127.0.0.1'),(347,0,'hj2ijs263htl5juejv1rn13f47',0,NULL,1413873434,1413873434,'127.0.0.1','127.0.0.1'),(348,0,'td68tketku18a6tpk7upgundf2',0,NULL,1413873437,1413873437,'127.0.0.1','127.0.0.1'),(349,0,'3bqhsii8celldmhl9avugdg4b1',0,NULL,1413873437,1413873437,'127.0.0.1','127.0.0.1'),(350,0,'1d09ipci2roaqbmc4iurqnbpe4',0,NULL,1413873437,1413873437,'127.0.0.1','127.0.0.1'),(351,0,'j9866fqgqfarl4jr0k3r6j3ak3',0,NULL,1413873437,1413873437,'127.0.0.1','127.0.0.1'),(352,0,'dtvsag4sd4g3oml2r0a34rm5l7',0,NULL,1413873437,1413873437,'127.0.0.1','127.0.0.1'),(353,0,'o7isva041ug68hkegrh3rf20t6',0,NULL,1413873437,1413873437,'127.0.0.1','127.0.0.1'),(354,0,'grn7atetfvidsjjetrbfnuuns1',0,NULL,1413873437,1413873437,'127.0.0.1','127.0.0.1'),(355,0,'nd4kg0tdg0tabbv5g884bp6em7',0,NULL,1413873437,1413873437,'127.0.0.1','127.0.0.1'),(356,0,'ogjuq7hl028pagd5q3d75nrba0',0,NULL,1413873438,1413873438,'127.0.0.1','127.0.0.1'),(357,0,'umkcjgec3vvir61g4u2b39ckv2',0,NULL,1413873438,1413873438,'127.0.0.1','127.0.0.1'),(358,0,'fmp3v3k1jp4bfvabe1ahqubba1',0,NULL,1413873438,1413873438,'127.0.0.1','127.0.0.1'),(359,0,'ogaiph8hkqi6b1rcaisiqm95t4',0,NULL,1413873438,1413873438,'127.0.0.1','127.0.0.1'),(360,0,'2s9dbrqr0fesdgqi5uqjlscjl2',0,NULL,1413873438,1413873438,'127.0.0.1','127.0.0.1'),(361,0,'edpc8u58h2u1qsc7gcpgjb5jb1',0,NULL,1413873438,1413873438,'127.0.0.1','127.0.0.1'),(362,0,'pndudei84cgd68f87avps4shr3',0,NULL,1413873438,1413873438,'127.0.0.1','127.0.0.1'),(363,0,'e49fmpp69bj1tjek0t44stk8s2',0,NULL,1413873438,1413873438,'127.0.0.1','127.0.0.1'),(364,0,'m1ed8v608kvebcumfu9vb96tt0',0,NULL,1413873438,1413873438,'127.0.0.1','127.0.0.1'),(365,0,'8m72r03ogfedv4jb1sqar4rt43',0,NULL,1413873439,1413873439,'127.0.0.1','127.0.0.1'),(366,0,'6uq8v05lp60nl8h9fne44b8eq6',0,NULL,1413873439,1413873439,'127.0.0.1','127.0.0.1'),(367,0,'k5vhtl2f4r0mdll5lddsslomr5',0,NULL,1413873439,1413873439,'127.0.0.1','127.0.0.1'),(368,0,'tp8srbp4gkontm0eenid5kjmh1',0,NULL,1413873439,1413873439,'127.0.0.1','127.0.0.1'),(369,0,'u02nmpdkritkt1na7lra6sagb7',0,NULL,1413873439,1413873439,'127.0.0.1','127.0.0.1'),(370,0,'dm956hr3fnfrljbrlcua7f1gs7',0,NULL,1413873439,1413873439,'127.0.0.1','127.0.0.1'),(371,0,'erq0qsfangp3905s8fb9n4oco7',0,NULL,1413873439,1413873439,'127.0.0.1','127.0.0.1'),(372,0,'pr42pjof2c8uaim6pap5gauo42',0,NULL,1413873439,1413873439,'127.0.0.1','127.0.0.1'),(373,0,'51i7ean5ile1vtfmj1h5cqeor6',0,NULL,1413873439,1413873439,'127.0.0.1','127.0.0.1'),(374,0,'1ennudvut2tn4a6qtmtj75qfu0',0,NULL,1413873439,1413873439,'127.0.0.1','127.0.0.1'),(375,0,'pinrf4trt4h8km5dkmrma4hs74',0,NULL,1413873439,1413873439,'127.0.0.1','127.0.0.1'),(376,0,'2jmbrh8qdvqglo16ven8lko4a4',0,NULL,1413873439,1413873439,'127.0.0.1','127.0.0.1'),(377,0,'n4gf88kj1l3l9070m5if2msu73',0,NULL,1413873440,1413873440,'127.0.0.1','127.0.0.1'),(378,0,'bqtt51cj34rb5blmmt58cjvd26',0,NULL,1413873440,1413873440,'127.0.0.1','127.0.0.1'),(379,0,'k3bbab4qklte5br253kpi75bk5',0,NULL,1413873440,1413873440,'127.0.0.1','127.0.0.1'),(380,0,'dmt84lq2f5ie5f8cslf3pa8m37',0,NULL,1413873440,1413873440,'127.0.0.1','127.0.0.1'),(381,0,'1dmrktm9vrr2bsllcauc71gr80',0,NULL,1413873440,1413873440,'127.0.0.1','127.0.0.1'),(382,0,'2vc20f70vsqs8tl8atm4gporv3',0,NULL,1413873440,1413873440,'127.0.0.1','127.0.0.1'),(383,0,'f5l07rudqlpbsfar75qi9t32h6',0,NULL,1413873440,1413873440,'127.0.0.1','127.0.0.1'),(384,0,'tshmcsd4lomtefeebk5e9hsh36',0,NULL,1413873440,1413873440,'127.0.0.1','127.0.0.1'),(385,0,'jq20eeicuctfht40i20dd7jnl2',0,NULL,1413873440,1413873440,'127.0.0.1','127.0.0.1'),(386,0,'dqesgp0a35bi1bueh0cnk0nkk3',0,NULL,1413873440,1413873440,'127.0.0.1','127.0.0.1'),(387,0,'lf92rl3a3ejionnq1nv5100461',0,NULL,1413873440,1413873440,'127.0.0.1','127.0.0.1'),(388,0,'g5tnnuq9dsmtol9plkiqoqctr1',0,NULL,1413873440,1413873440,'127.0.0.1','127.0.0.1'),(389,0,'elie0hdv2qavhr3ohsoarq0ie3',0,NULL,1413873440,1413873440,'127.0.0.1','127.0.0.1'),(390,0,'pp9j8bc0e1ocfbjbg9s41614q4',0,NULL,1413873441,1413873441,'127.0.0.1','127.0.0.1'),(391,0,'86vm7318t8p2ef5o136cq03tj0',0,NULL,1413873441,1413873441,'127.0.0.1','127.0.0.1'),(392,0,'6kj5gdn4cb6vait9mgvjhf4on7',0,NULL,1413873441,1413873441,'127.0.0.1','127.0.0.1'),(393,0,'6pbk7jv7bsk5c0vqcipef241g1',0,NULL,1413873441,1413873441,'127.0.0.1','127.0.0.1'),(394,0,'di1o55lo08pmcbt8cie6o81qj0',0,NULL,1413873441,1413873441,'127.0.0.1','127.0.0.1'),(395,0,'01d1gcd5gnnlqss80js8mbvg17',0,NULL,1413873441,1413873441,'127.0.0.1','127.0.0.1'),(396,0,'h8j7thoqv9ag1v4ui8t2tolc67',0,NULL,1413873441,1413873441,'127.0.0.1','127.0.0.1'),(397,0,'sp38sbcvf5o7r53ngl5apjs832',0,NULL,1413873441,1413873441,'127.0.0.1','127.0.0.1'),(398,0,'9cb87rlrnfuaqpbg7dnatmh0l6',0,NULL,1413873441,1413873441,'127.0.0.1','127.0.0.1'),(399,0,'t54102nuor43gcm2qvnbqf4u47',0,NULL,1413873441,1413873441,'127.0.0.1','127.0.0.1'),(400,0,'jt00lfehh7k54kfa90orn2aqd3',0,NULL,1413873441,1413873441,'127.0.0.1','127.0.0.1'),(401,0,'ps1nosdpuih7ofo0lmon64n6h5',0,NULL,1413873442,1413873442,'127.0.0.1','127.0.0.1'),(402,0,'d1bmqo3r3r66k55ts33t0d98o5',0,NULL,1413873442,1413873442,'127.0.0.1','127.0.0.1'),(403,0,'crmji1vr7otarp3uecrbbu0tl3',0,NULL,1413873442,1413873442,'127.0.0.1','127.0.0.1'),(404,0,'m7bvntevvuovtg57idl9csaa91',0,NULL,1413873442,1413873442,'127.0.0.1','127.0.0.1'),(405,0,'a0qp8vcb7iobh7fnj6obvol6u3',0,NULL,1413873442,1413873442,'127.0.0.1','127.0.0.1'),(406,0,'8t4nto85ovlkjik7jmt95d3ad4',0,NULL,1413873442,1413873442,'127.0.0.1','127.0.0.1'),(407,0,'ib99pkmva6tdjs8tg1av8do042',0,NULL,1413873442,1413873442,'127.0.0.1','127.0.0.1'),(408,0,'qu1lh4koeemb0rvk1pmlmnf632',0,NULL,1413873442,1413873442,'127.0.0.1','127.0.0.1'),(409,0,'vh16ah0i2r4aqp5sslgh27qon0',0,NULL,1413873442,1413873442,'127.0.0.1','127.0.0.1'),(410,0,'vpsoqgsngc77ghjbt9bjrbnpb5',0,NULL,1413873447,1413873447,'127.0.0.1','127.0.0.1'),(411,0,'aivfkoan86f308v54mb7lvvob5',0,NULL,1413873447,1413873447,'127.0.0.1','127.0.0.1'),(412,0,'hobcfc3e00bbrgkq248afanvh3',0,NULL,1413873447,1413873447,'127.0.0.1','127.0.0.1'),(413,0,'58ir0fmnjm9tj2tcvukk8fck50',0,NULL,1413873448,1413873448,'127.0.0.1','127.0.0.1'),(414,0,'nrtdfecf8sgqap9qk0gi38u2r4',0,NULL,1413873448,1413873448,'127.0.0.1','127.0.0.1'),(415,0,'vsmu23v2grrm2hldq9e0564hp5',0,NULL,1413873448,1413873448,'127.0.0.1','127.0.0.1'),(416,0,'j2fp3l9gdmo819fnm8j8bv6mk2',0,NULL,1413873448,1413873448,'127.0.0.1','127.0.0.1'),(417,0,'pbth4m27k1gsifetgjhkp096m0',0,NULL,1413873448,1413873448,'127.0.0.1','127.0.0.1'),(418,0,'ma85cl99bgckciia9fthdfg8f3',0,NULL,1413873448,1413873448,'127.0.0.1','127.0.0.1'),(419,0,'iqpk0gpen285niqudc36cu6i57',0,NULL,1413873448,1413873448,'127.0.0.1','127.0.0.1'),(420,0,'s9t3qbl7c8ch4s9uulqai0n183',0,NULL,1413873448,1413873448,'127.0.0.1','127.0.0.1'),(421,0,'85s8vqn0ifpdoq0bjc0dlo57b7',0,NULL,1413873448,1413873448,'127.0.0.1','127.0.0.1'),(422,0,'4q1saha84uhshos407ef6a1qu7',0,NULL,1413873448,1413873448,'127.0.0.1','127.0.0.1'),(423,0,'umcitnf5lg93o6pje577fao664',0,NULL,1413873448,1413873448,'127.0.0.1','127.0.0.1'),(424,0,'5r1ulerkf23a4uinf813o0i0u6',0,NULL,1413873449,1413873449,'127.0.0.1','127.0.0.1'),(425,0,'vdprduhskq3k6pv60sf05742l5',0,NULL,1413873449,1413873449,'127.0.0.1','127.0.0.1'),(426,0,'ni2lppofvlfapdpdehrgei1ad3',0,NULL,1413873449,1413873449,'127.0.0.1','127.0.0.1'),(427,0,'6gm8ffr394c4leaep8mp3nbtm2',0,NULL,1413873449,1413873449,'127.0.0.1','127.0.0.1'),(428,0,'h4kmfhviu720qh2jh7860qhrb3',0,NULL,1413873449,1413873449,'127.0.0.1','127.0.0.1'),(429,0,'fc5ifbasaar95jefu4m55be2d2',0,NULL,1413873449,1413873449,'127.0.0.1','127.0.0.1'),(443,0,'0n10dv4vj9n4tln4jcr4lai622',2,NULL,1413874236,1413874948,'127.0.0.1','127.0.0.1'),(445,0,'8q8ssg38ng4eruvdh2tbvjvo85',4,NULL,1413874589,1413874629,'127.0.0.1','127.0.0.1'),(446,0,'7m6vkps9hmneiso7g1a103i781',0,NULL,1413874648,1413874648,'127.0.0.1','127.0.0.1'),(447,0,'bpa296bvhv48r631bqocn0oh14',2,NULL,1413874974,1413875044,'127.0.0.1','127.0.0.1'),(479,0,'5rq614mqj12t295eqnbckprsm7',4,NULL,1413885678,1413885703,'127.0.0.1','127.0.0.1'),(482,0,'emgorboc6an6gus1pljqs3ik41',4,NULL,1413885761,1413885764,'127.0.0.1','127.0.0.1'),(491,0,'1ouhtn5hh27orb97puaptu71v5',4,NULL,1413886079,1413886079,'127.0.0.1','127.0.0.1'),(493,0,'62lmgjnngk5o2nu1fl81p789a5',4,NULL,1413886101,1413886101,'127.0.0.1','127.0.0.1'),(495,0,'tudmmmr7o10h046du59ugbjm20',2,NULL,1413886157,1413886193,'127.0.0.1','127.0.0.1'),(515,0,'8ncjcps3sbadetbl928gf554b6',4,NULL,1413975353,1413975417,'127.0.0.1','127.0.0.1'),(517,0,'ni98vip53lqh7qv2l0uvc0ma77',2,NULL,1414028442,1414028491,'127.0.0.1','127.0.0.1');

/*Table structure for table `mdl_stats_daily` */

DROP TABLE IF EXISTS `mdl_stats_daily`;

CREATE TABLE `mdl_stats_daily` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `timeend` bigint(10) NOT NULL DEFAULT '0',
  `roleid` bigint(10) NOT NULL DEFAULT '0',
  `stattype` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'activity',
  `stat1` bigint(10) NOT NULL DEFAULT '0',
  `stat2` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_statdail_cou_ix` (`courseid`),
  KEY `mdl_statdail_tim_ix` (`timeend`),
  KEY `mdl_statdail_rol_ix` (`roleid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='to accumulate daily stats';

/*Data for the table `mdl_stats_daily` */

/*Table structure for table `mdl_stats_monthly` */

DROP TABLE IF EXISTS `mdl_stats_monthly`;

CREATE TABLE `mdl_stats_monthly` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `timeend` bigint(10) NOT NULL DEFAULT '0',
  `roleid` bigint(10) NOT NULL DEFAULT '0',
  `stattype` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'activity',
  `stat1` bigint(10) NOT NULL DEFAULT '0',
  `stat2` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_statmont_cou_ix` (`courseid`),
  KEY `mdl_statmont_tim_ix` (`timeend`),
  KEY `mdl_statmont_rol_ix` (`roleid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='To accumulate monthly stats';

/*Data for the table `mdl_stats_monthly` */

/*Table structure for table `mdl_stats_user_daily` */

DROP TABLE IF EXISTS `mdl_stats_user_daily`;

CREATE TABLE `mdl_stats_user_daily` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `roleid` bigint(10) NOT NULL DEFAULT '0',
  `timeend` bigint(10) NOT NULL DEFAULT '0',
  `statsreads` bigint(10) NOT NULL DEFAULT '0',
  `statswrites` bigint(10) NOT NULL DEFAULT '0',
  `stattype` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_statuserdail_cou_ix` (`courseid`),
  KEY `mdl_statuserdail_use_ix` (`userid`),
  KEY `mdl_statuserdail_rol_ix` (`roleid`),
  KEY `mdl_statuserdail_tim_ix` (`timeend`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='To accumulate daily stats per course/user';

/*Data for the table `mdl_stats_user_daily` */

/*Table structure for table `mdl_stats_user_monthly` */

DROP TABLE IF EXISTS `mdl_stats_user_monthly`;

CREATE TABLE `mdl_stats_user_monthly` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `roleid` bigint(10) NOT NULL DEFAULT '0',
  `timeend` bigint(10) NOT NULL DEFAULT '0',
  `statsreads` bigint(10) NOT NULL DEFAULT '0',
  `statswrites` bigint(10) NOT NULL DEFAULT '0',
  `stattype` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_statusermont_cou_ix` (`courseid`),
  KEY `mdl_statusermont_use_ix` (`userid`),
  KEY `mdl_statusermont_rol_ix` (`roleid`),
  KEY `mdl_statusermont_tim_ix` (`timeend`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='To accumulate monthly stats per course/user';

/*Data for the table `mdl_stats_user_monthly` */

/*Table structure for table `mdl_stats_user_weekly` */

DROP TABLE IF EXISTS `mdl_stats_user_weekly`;

CREATE TABLE `mdl_stats_user_weekly` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `roleid` bigint(10) NOT NULL DEFAULT '0',
  `timeend` bigint(10) NOT NULL DEFAULT '0',
  `statsreads` bigint(10) NOT NULL DEFAULT '0',
  `statswrites` bigint(10) NOT NULL DEFAULT '0',
  `stattype` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_statuserweek_cou_ix` (`courseid`),
  KEY `mdl_statuserweek_use_ix` (`userid`),
  KEY `mdl_statuserweek_rol_ix` (`roleid`),
  KEY `mdl_statuserweek_tim_ix` (`timeend`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='To accumulate weekly stats per course/user';

/*Data for the table `mdl_stats_user_weekly` */

/*Table structure for table `mdl_stats_weekly` */

DROP TABLE IF EXISTS `mdl_stats_weekly`;

CREATE TABLE `mdl_stats_weekly` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `timeend` bigint(10) NOT NULL DEFAULT '0',
  `roleid` bigint(10) NOT NULL DEFAULT '0',
  `stattype` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'activity',
  `stat1` bigint(10) NOT NULL DEFAULT '0',
  `stat2` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_statweek_cou_ix` (`courseid`),
  KEY `mdl_statweek_tim_ix` (`timeend`),
  KEY `mdl_statweek_rol_ix` (`roleid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='To accumulate weekly stats';

/*Data for the table `mdl_stats_weekly` */

/*Table structure for table `mdl_survey` */

DROP TABLE IF EXISTS `mdl_survey`;

CREATE TABLE `mdl_survey` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `template` bigint(10) NOT NULL DEFAULT '0',
  `days` mediumint(6) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci NOT NULL,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `questions` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_surv_cou_ix` (`course`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Each record is one SURVEY module with its configuration';

/*Data for the table `mdl_survey` */

insert  into `mdl_survey`(`id`,`course`,`template`,`days`,`timecreated`,`timemodified`,`name`,`intro`,`introformat`,`questions`) values (1,0,0,0,985017600,985017600,'collesaname','collesaintro',0,'25,26,27,28,29,30,43,44'),(2,0,0,0,985017600,985017600,'collespname','collespintro',0,'31,32,33,34,35,36,43,44'),(3,0,0,0,985017600,985017600,'collesapname','collesapintro',0,'37,38,39,40,41,42,43,44'),(4,0,0,0,985017600,985017600,'attlsname','attlsintro',0,'65,67,68'),(5,0,0,0,985017600,985017600,'ciqname','ciqintro',0,'69,70,71,72,73');

/*Table structure for table `mdl_survey_analysis` */

DROP TABLE IF EXISTS `mdl_survey_analysis`;

CREATE TABLE `mdl_survey_analysis` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `survey` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `notes` longtext COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_survanal_use_ix` (`userid`),
  KEY `mdl_survanal_sur_ix` (`survey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='text about each survey submission';

/*Data for the table `mdl_survey_analysis` */

/*Table structure for table `mdl_survey_answers` */

DROP TABLE IF EXISTS `mdl_survey_answers`;

CREATE TABLE `mdl_survey_answers` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `survey` bigint(10) NOT NULL DEFAULT '0',
  `question` bigint(10) NOT NULL DEFAULT '0',
  `time` bigint(10) NOT NULL DEFAULT '0',
  `answer1` longtext COLLATE utf8_unicode_ci NOT NULL,
  `answer2` longtext COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_survansw_use_ix` (`userid`),
  KEY `mdl_survansw_sur_ix` (`survey`),
  KEY `mdl_survansw_que_ix` (`question`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='the answers to each questions filled by the users';

/*Data for the table `mdl_survey_answers` */

/*Table structure for table `mdl_survey_questions` */

DROP TABLE IF EXISTS `mdl_survey_questions`;

CREATE TABLE `mdl_survey_questions` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `text` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `shorttext` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `multi` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `type` smallint(3) NOT NULL DEFAULT '0',
  `options` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=74 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='the questions conforming one survey';

/*Data for the table `mdl_survey_questions` */

insert  into `mdl_survey_questions`(`id`,`text`,`shorttext`,`multi`,`intro`,`type`,`options`) values (1,'colles1','colles1short','','',1,'scaletimes5'),(2,'colles2','colles2short','','',1,'scaletimes5'),(3,'colles3','colles3short','','',1,'scaletimes5'),(4,'colles4','colles4short','','',1,'scaletimes5'),(5,'colles5','colles5short','','',1,'scaletimes5'),(6,'colles6','colles6short','','',1,'scaletimes5'),(7,'colles7','colles7short','','',1,'scaletimes5'),(8,'colles8','colles8short','','',1,'scaletimes5'),(9,'colles9','colles9short','','',1,'scaletimes5'),(10,'colles10','colles10short','','',1,'scaletimes5'),(11,'colles11','colles11short','','',1,'scaletimes5'),(12,'colles12','colles12short','','',1,'scaletimes5'),(13,'colles13','colles13short','','',1,'scaletimes5'),(14,'colles14','colles14short','','',1,'scaletimes5'),(15,'colles15','colles15short','','',1,'scaletimes5'),(16,'colles16','colles16short','','',1,'scaletimes5'),(17,'colles17','colles17short','','',1,'scaletimes5'),(18,'colles18','colles18short','','',1,'scaletimes5'),(19,'colles19','colles19short','','',1,'scaletimes5'),(20,'colles20','colles20short','','',1,'scaletimes5'),(21,'colles21','colles21short','','',1,'scaletimes5'),(22,'colles22','colles22short','','',1,'scaletimes5'),(23,'colles23','colles23short','','',1,'scaletimes5'),(24,'colles24','colles24short','','',1,'scaletimes5'),(25,'collesm1','collesm1short','1,2,3,4','collesmintro',1,'scaletimes5'),(26,'collesm2','collesm2short','5,6,7,8','collesmintro',1,'scaletimes5'),(27,'collesm3','collesm3short','9,10,11,12','collesmintro',1,'scaletimes5'),(28,'collesm4','collesm4short','13,14,15,16','collesmintro',1,'scaletimes5'),(29,'collesm5','collesm5short','17,18,19,20','collesmintro',1,'scaletimes5'),(30,'collesm6','collesm6short','21,22,23,24','collesmintro',1,'scaletimes5'),(31,'collesm1','collesm1short','1,2,3,4','collesmintro',2,'scaletimes5'),(32,'collesm2','collesm2short','5,6,7,8','collesmintro',2,'scaletimes5'),(33,'collesm3','collesm3short','9,10,11,12','collesmintro',2,'scaletimes5'),(34,'collesm4','collesm4short','13,14,15,16','collesmintro',2,'scaletimes5'),(35,'collesm5','collesm5short','17,18,19,20','collesmintro',2,'scaletimes5'),(36,'collesm6','collesm6short','21,22,23,24','collesmintro',2,'scaletimes5'),(37,'collesm1','collesm1short','1,2,3,4','collesmintro',3,'scaletimes5'),(38,'collesm2','collesm2short','5,6,7,8','collesmintro',3,'scaletimes5'),(39,'collesm3','collesm3short','9,10,11,12','collesmintro',3,'scaletimes5'),(40,'collesm4','collesm4short','13,14,15,16','collesmintro',3,'scaletimes5'),(41,'collesm5','collesm5short','17,18,19,20','collesmintro',3,'scaletimes5'),(42,'collesm6','collesm6short','21,22,23,24','collesmintro',3,'scaletimes5'),(43,'howlong','','','',1,'howlongoptions'),(44,'othercomments','','','',0,NULL),(45,'attls1','attls1short','','',1,'scaleagree5'),(46,'attls2','attls2short','','',1,'scaleagree5'),(47,'attls3','attls3short','','',1,'scaleagree5'),(48,'attls4','attls4short','','',1,'scaleagree5'),(49,'attls5','attls5short','','',1,'scaleagree5'),(50,'attls6','attls6short','','',1,'scaleagree5'),(51,'attls7','attls7short','','',1,'scaleagree5'),(52,'attls8','attls8short','','',1,'scaleagree5'),(53,'attls9','attls9short','','',1,'scaleagree5'),(54,'attls10','attls10short','','',1,'scaleagree5'),(55,'attls11','attls11short','','',1,'scaleagree5'),(56,'attls12','attls12short','','',1,'scaleagree5'),(57,'attls13','attls13short','','',1,'scaleagree5'),(58,'attls14','attls14short','','',1,'scaleagree5'),(59,'attls15','attls15short','','',1,'scaleagree5'),(60,'attls16','attls16short','','',1,'scaleagree5'),(61,'attls17','attls17short','','',1,'scaleagree5'),(62,'attls18','attls18short','','',1,'scaleagree5'),(63,'attls19','attls19short','','',1,'scaleagree5'),(64,'attls20','attls20short','','',1,'scaleagree5'),(65,'attlsm1','attlsm1','45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,64','attlsmintro',1,'scaleagree5'),(66,'-','-','-','-',0,'-'),(67,'attlsm2','attlsm2','63,62,59,57,55,49,52,50,48,47','attlsmintro',-1,'scaleagree5'),(68,'attlsm3','attlsm3','46,54,45,51,60,53,56,58,61,64','attlsmintro',-1,'scaleagree5'),(69,'ciq1','ciq1short','','',0,NULL),(70,'ciq2','ciq2short','','',0,NULL),(71,'ciq3','ciq3short','','',0,NULL),(72,'ciq4','ciq4short','','',0,NULL),(73,'ciq5','ciq5short','','',0,NULL);

/*Table structure for table `mdl_tag` */

DROP TABLE IF EXISTS `mdl_tag`;

CREATE TABLE `mdl_tag` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `rawname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `tagtype` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` tinyint(2) NOT NULL DEFAULT '0',
  `flag` smallint(4) DEFAULT '0',
  `timemodified` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_tag_nam_uix` (`name`),
  UNIQUE KEY `mdl_tag_idnam_uix` (`id`,`name`),
  KEY `mdl_tag_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Tag table - this generic table will replace the old "tags" t';

/*Data for the table `mdl_tag` */

/*Table structure for table `mdl_tag_correlation` */

DROP TABLE IF EXISTS `mdl_tag_correlation`;

CREATE TABLE `mdl_tag_correlation` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `tagid` bigint(10) NOT NULL,
  `correlatedtags` longtext COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_tagcorr_tag_ix` (`tagid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='The rationale for the ''tag_correlation'' table is performance';

/*Data for the table `mdl_tag_correlation` */

/*Table structure for table `mdl_tag_instance` */

DROP TABLE IF EXISTS `mdl_tag_instance`;

CREATE TABLE `mdl_tag_instance` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `tagid` bigint(10) NOT NULL,
  `component` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `itemtype` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `itemid` bigint(10) NOT NULL,
  `contextid` bigint(10) DEFAULT NULL,
  `tiuserid` bigint(10) NOT NULL DEFAULT '0',
  `ordering` bigint(10) DEFAULT NULL,
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_taginst_iteitetagtiu_uix` (`itemtype`,`itemid`,`tagid`,`tiuserid`),
  KEY `mdl_taginst_tag_ix` (`tagid`),
  KEY `mdl_taginst_con_ix` (`contextid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='tag_instance table holds the information of associations bet';

/*Data for the table `mdl_tag_instance` */

/*Table structure for table `mdl_task_adhoc` */

DROP TABLE IF EXISTS `mdl_task_adhoc`;

CREATE TABLE `mdl_task_adhoc` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `component` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `classname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `nextruntime` bigint(10) NOT NULL,
  `faildelay` bigint(10) DEFAULT NULL,
  `customdata` longtext COLLATE utf8_unicode_ci,
  `blocking` tinyint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_taskadho_nex_ix` (`nextruntime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='List of adhoc tasks waiting to run.';

/*Data for the table `mdl_task_adhoc` */

/*Table structure for table `mdl_task_scheduled` */

DROP TABLE IF EXISTS `mdl_task_scheduled`;

CREATE TABLE `mdl_task_scheduled` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `component` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `classname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `lastruntime` bigint(10) DEFAULT NULL,
  `nextruntime` bigint(10) DEFAULT NULL,
  `blocking` tinyint(2) NOT NULL DEFAULT '0',
  `minute` varchar(25) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `hour` varchar(25) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `day` varchar(25) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `month` varchar(25) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `dayofweek` varchar(25) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `faildelay` bigint(10) DEFAULT NULL,
  `customised` tinyint(2) NOT NULL DEFAULT '0',
  `disabled` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_tasksche_cla_uix` (`classname`)
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='List of scheduled tasks to be run by cron.';

/*Data for the table `mdl_task_scheduled` */

insert  into `mdl_task_scheduled`(`id`,`component`,`classname`,`lastruntime`,`nextruntime`,`blocking`,`minute`,`hour`,`day`,`month`,`dayofweek`,`faildelay`,`customised`,`disabled`) values (1,'moodle','\\core\\task\\session_cleanup_task',0,0,0,'*','*','*','*','*',0,0,0),(2,'moodle','\\core\\task\\delete_unconfirmed_users_task',0,0,0,'0','*','*','*','*',0,0,0),(3,'moodle','\\core\\task\\delete_incomplete_users_task',0,0,0,'5','*','*','*','*',0,0,0),(4,'moodle','\\core\\task\\backup_cleanup_task',0,0,0,'10','*','*','*','*',0,0,0),(5,'moodle','\\core\\task\\tag_cron_task',0,0,0,'20','*','*','*','*',0,0,0),(6,'moodle','\\core\\task\\context_cleanup_task',0,0,0,'25','*','*','*','*',0,0,0),(7,'moodle','\\core\\task\\cache_cleanup_task',0,0,0,'30','*','*','*','*',0,0,0),(8,'moodle','\\core\\task\\messaging_cleanup_task',0,0,0,'35','*','*','*','*',0,0,0),(9,'moodle','\\core\\task\\send_new_user_passwords_task',0,0,0,'*','*','*','*','*',0,0,0),(10,'moodle','\\core\\task\\send_failed_login_notifications_task',0,0,0,'*','*','*','*','*',0,0,0),(11,'moodle','\\core\\task\\create_contexts_task',0,0,1,'*','*','*','*','*',0,0,0),(12,'moodle','\\core\\task\\legacy_plugin_cron_task',0,0,0,'*','*','*','*','*',0,0,0),(13,'moodle','\\core\\task\\grade_cron_task',0,0,0,'*','*','*','*','*',0,0,0),(14,'moodle','\\core\\task\\events_cron_task',0,0,0,'*','*','*','*','*',0,0,0),(15,'moodle','\\core\\task\\completion_cron_task',0,0,0,'*','*','*','*','*',0,0,0),(16,'moodle','\\core\\task\\portfolio_cron_task',0,0,0,'*','*','*','*','*',0,0,0),(17,'moodle','\\core\\task\\plagiarism_cron_task',0,0,0,'*','*','*','*','*',0,0,0),(18,'moodle','\\core\\task\\calendar_cron_task',0,0,0,'*','*','*','*','*',0,0,0),(19,'moodle','\\core\\task\\blog_cron_task',0,0,0,'*','*','*','*','*',0,0,0),(20,'moodle','\\core\\task\\question_cron_task',0,0,0,'*','*','*','*','*',0,0,0),(21,'moodle','\\core\\task\\registration_cron_task',0,0,0,'0','3','*','*','*',0,0,0),(22,'moodle','\\core\\task\\check_for_updates_task',0,0,0,'0','*/2','*','*','*',0,0,0),(23,'moodle','\\core\\task\\cache_cron_task',0,0,0,'50','*','*','*','*',0,0,0),(24,'moodle','\\core\\task\\automated_backup_task',0,0,0,'50','*','*','*','*',0,0,0),(25,'moodle','\\core\\task\\badges_cron_task',0,0,0,'*/5','*','*','*','*',0,0,0),(26,'moodle','\\core\\task\\file_temp_cleanup_task',0,0,0,'55','*/6','*','*','*',0,0,0),(27,'moodle','\\core\\task\\file_trash_cleanup_task',0,0,0,'55','*/6','*','*','*',0,0,0),(28,'moodle','\\core\\task\\stats_cron_task',0,0,0,'0','*','*','*','*',0,0,0),(29,'moodle','\\core\\task\\password_reset_cleanup_task',0,0,0,'0','*/6','*','*','*',0,0,0),(30,'mod_forum','\\mod_forum\\task\\cron_task',0,0,0,'*','*','*','*','*',0,0,0),(31,'logstore_legacy','\\logstore_legacy\\task\\cleanup_task',0,0,0,'*','5','*','*','*',0,0,0),(32,'logstore_standard','\\logstore_standard\\task\\cleanup_task',0,0,0,'*','4','*','*','*',0,0,0);

/*Table structure for table `mdl_timezone` */

DROP TABLE IF EXISTS `mdl_timezone`;

CREATE TABLE `mdl_timezone` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `year` bigint(11) NOT NULL DEFAULT '0',
  `tzrule` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `gmtoff` bigint(11) NOT NULL DEFAULT '0',
  `dstoff` bigint(11) NOT NULL DEFAULT '0',
  `dst_month` tinyint(2) NOT NULL DEFAULT '0',
  `dst_startday` smallint(3) NOT NULL DEFAULT '0',
  `dst_weekday` smallint(3) NOT NULL DEFAULT '0',
  `dst_skipweeks` smallint(3) NOT NULL DEFAULT '0',
  `dst_time` varchar(6) COLLATE utf8_unicode_ci NOT NULL DEFAULT '00:00',
  `std_month` tinyint(2) NOT NULL DEFAULT '0',
  `std_startday` smallint(3) NOT NULL DEFAULT '0',
  `std_weekday` smallint(3) NOT NULL DEFAULT '0',
  `std_skipweeks` smallint(3) NOT NULL DEFAULT '0',
  `std_time` varchar(6) COLLATE utf8_unicode_ci NOT NULL DEFAULT '00:00',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Rules for calculating local wall clock time for users';

/*Data for the table `mdl_timezone` */

/*Table structure for table `mdl_tool_customlang` */

DROP TABLE IF EXISTS `mdl_tool_customlang`;

CREATE TABLE `mdl_tool_customlang` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `lang` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `componentid` bigint(10) NOT NULL,
  `stringid` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `original` longtext COLLATE utf8_unicode_ci NOT NULL,
  `master` longtext COLLATE utf8_unicode_ci,
  `local` longtext COLLATE utf8_unicode_ci,
  `timemodified` bigint(10) NOT NULL,
  `timecustomized` bigint(10) DEFAULT NULL,
  `outdated` smallint(3) DEFAULT '0',
  `modified` smallint(3) DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_toolcust_lancomstr_uix` (`lang`,`componentid`,`stringid`),
  KEY `mdl_toolcust_com_ix` (`componentid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Contains the working checkout of all strings and their custo';

/*Data for the table `mdl_tool_customlang` */

/*Table structure for table `mdl_tool_customlang_components` */

DROP TABLE IF EXISTS `mdl_tool_customlang_components`;

CREATE TABLE `mdl_tool_customlang_components` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `version` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Contains the list of all installed plugins that provide thei';

/*Data for the table `mdl_tool_customlang_components` */

/*Table structure for table `mdl_upgrade_log` */

DROP TABLE IF EXISTS `mdl_upgrade_log`;

CREATE TABLE `mdl_upgrade_log` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `type` bigint(10) NOT NULL,
  `plugin` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `version` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `targetversion` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `info` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `details` longtext COLLATE utf8_unicode_ci,
  `backtrace` longtext COLLATE utf8_unicode_ci,
  `userid` bigint(10) NOT NULL,
  `timemodified` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_upgrlog_tim_ix` (`timemodified`),
  KEY `mdl_upgrlog_typtim_ix` (`type`,`timemodified`),
  KEY `mdl_upgrlog_use_ix` (`userid`)
) ENGINE=InnoDB AUTO_INCREMENT=1192 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Upgrade logging';

/*Data for the table `mdl_upgrade_log` */

insert  into `mdl_upgrade_log`(`id`,`type`,`plugin`,`version`,`targetversion`,`info`,`details`,`backtrace`,`userid`,`timemodified`) values (1,0,'core','2014051202','2014051202','Upgrade savepoint reached',NULL,'',0,1413856764),(2,0,'core','2014051202','2014051202','Core installed',NULL,'',0,1413856831),(3,0,'availability_completion',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856832),(4,0,'availability_completion','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856832),(5,0,'availability_completion','2014051200','2014051200','Plugin installed',NULL,'',0,1413856832),(6,0,'availability_date',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856832),(7,0,'availability_date','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856832),(8,0,'availability_date','2014051200','2014051200','Plugin installed',NULL,'',0,1413856832),(9,0,'availability_grade',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856832),(10,0,'availability_grade','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856833),(11,0,'availability_grade','2014051200','2014051200','Plugin installed',NULL,'',0,1413856833),(12,0,'availability_group',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856833),(13,0,'availability_group','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856833),(14,0,'availability_group','2014051200','2014051200','Plugin installed',NULL,'',0,1413856833),(15,0,'availability_grouping',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856833),(16,0,'availability_grouping','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856834),(17,0,'availability_grouping','2014051200','2014051200','Plugin installed',NULL,'',0,1413856834),(18,0,'availability_profile',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856834),(19,0,'availability_profile','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856834),(20,0,'availability_profile','2014051200','2014051200','Plugin installed',NULL,'',0,1413856834),(21,0,'qtype_calculated',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856834),(22,0,'qtype_calculated','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856835),(23,0,'qtype_calculated','2014051200','2014051200','Plugin installed',NULL,'',0,1413856835),(24,0,'qtype_calculatedmulti',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856835),(25,0,'qtype_calculatedmulti','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856835),(26,0,'qtype_calculatedmulti','2014051200','2014051200','Plugin installed',NULL,'',0,1413856835),(27,0,'qtype_calculatedsimple',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856836),(28,0,'qtype_calculatedsimple','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856836),(29,0,'qtype_calculatedsimple','2014051200','2014051200','Plugin installed',NULL,'',0,1413856836),(30,0,'qtype_description',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856836),(31,0,'qtype_description','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856836),(32,0,'qtype_description','2014051200','2014051200','Plugin installed',NULL,'',0,1413856836),(33,0,'qtype_essay',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856836),(34,0,'qtype_essay','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856837),(35,0,'qtype_essay','2014051200','2014051200','Plugin installed',NULL,'',0,1413856837),(36,0,'qtype_match',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856837),(37,0,'qtype_match','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856837),(38,0,'qtype_match','2014051200','2014051200','Plugin installed',NULL,'',0,1413856838),(39,0,'qtype_missingtype',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856838),(40,0,'qtype_missingtype','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856838),(41,0,'qtype_missingtype','2014051200','2014051200','Plugin installed',NULL,'',0,1413856838),(42,0,'qtype_multianswer',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856838),(43,0,'qtype_multianswer','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856838),(44,0,'qtype_multianswer','2014051200','2014051200','Plugin installed',NULL,'',0,1413856838),(45,0,'qtype_multichoice',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856839),(46,0,'qtype_multichoice','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856839),(47,0,'qtype_multichoice','2014051200','2014051200','Plugin installed',NULL,'',0,1413856839),(48,0,'qtype_numerical',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856839),(49,0,'qtype_numerical','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856839),(50,0,'qtype_numerical','2014051200','2014051200','Plugin installed',NULL,'',0,1413856840),(51,0,'qtype_random',NULL,'2014051201','Starting plugin installation',NULL,'',0,1413856840),(52,0,'qtype_random','2014051201','2014051201','Upgrade savepoint reached',NULL,'',0,1413856840),(53,0,'qtype_random','2014051201','2014051201','Plugin installed',NULL,'',0,1413856840),(54,0,'qtype_randomsamatch',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856840),(55,0,'qtype_randomsamatch','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856840),(56,0,'qtype_randomsamatch','2014051200','2014051200','Plugin installed',NULL,'',0,1413856840),(57,0,'qtype_shortanswer',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856841),(58,0,'qtype_shortanswer','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856841),(59,0,'qtype_shortanswer','2014051200','2014051200','Plugin installed',NULL,'',0,1413856841),(60,0,'qtype_truefalse',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856841),(61,0,'qtype_truefalse','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856841),(62,0,'qtype_truefalse','2014051200','2014051200','Plugin installed',NULL,'',0,1413856841),(63,0,'mod_assign',NULL,'2014051201','Starting plugin installation',NULL,'',0,1413856841),(64,0,'mod_assign','2014051201','2014051201','Upgrade savepoint reached',NULL,'',0,1413856842),(65,0,'mod_assign','2014051201','2014051201','Plugin installed',NULL,'',0,1413856848),(66,0,'mod_assignment',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856848),(67,0,'mod_assignment','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856848),(68,0,'mod_assignment','2014051200','2014051200','Plugin installed',NULL,'',0,1413856850),(69,0,'mod_book',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856850),(70,0,'mod_book','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856851),(71,0,'mod_book','2014051200','2014051200','Plugin installed',NULL,'',0,1413856852),(72,0,'mod_chat',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856853),(73,0,'mod_chat','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856853),(74,0,'mod_chat','2014051200','2014051200','Plugin installed',NULL,'',0,1413856855),(75,0,'mod_choice',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856855),(76,0,'mod_choice','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856855),(77,0,'mod_choice','2014051200','2014051200','Plugin installed',NULL,'',0,1413856858),(78,0,'mod_data',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856858),(79,0,'mod_data','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856858),(80,0,'mod_data','2014051200','2014051200','Plugin installed',NULL,'',0,1413856864),(81,0,'mod_feedback',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856864),(82,0,'mod_feedback','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856865),(83,0,'mod_feedback','2014051200','2014051200','Plugin installed',NULL,'',0,1413856869),(84,0,'mod_folder',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856869),(85,0,'mod_folder','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856869),(86,0,'mod_folder','2014051200','2014051200','Plugin installed',NULL,'',0,1413856870),(87,0,'mod_forum',NULL,'2014051202','Starting plugin installation',NULL,'',0,1413856870),(88,0,'mod_forum','2014051202','2014051202','Upgrade savepoint reached',NULL,'',0,1413856871),(89,0,'mod_forum','2014051202','2014051202','Plugin installed',NULL,'',0,1413856880),(90,0,'mod_glossary',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856880),(91,0,'mod_glossary','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856881),(92,0,'mod_glossary','2014051200','2014051200','Plugin installed',NULL,'',0,1413856887),(93,0,'mod_imscp',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856887),(94,0,'mod_imscp','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856887),(95,0,'mod_imscp','2014051200','2014051200','Plugin installed',NULL,'',0,1413856888),(96,0,'mod_label',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856888),(97,0,'mod_label','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856888),(98,0,'mod_label','2014051200','2014051200','Plugin installed',NULL,'',0,1413856889),(99,0,'mod_lesson',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856889),(100,0,'mod_lesson','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856890),(101,0,'mod_lesson','2014051200','2014051200','Plugin installed',NULL,'',0,1413856891),(102,0,'mod_lti',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856891),(103,0,'mod_lti','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856891),(104,0,'mod_lti','2014051200','2014051200','Plugin installed',NULL,'',0,1413856893),(105,0,'mod_page',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856893),(106,0,'mod_page','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856894),(107,0,'mod_page','2014051200','2014051200','Plugin installed',NULL,'',0,1413856894),(108,0,'mod_quiz',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856894),(109,0,'mod_quiz','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856895),(110,0,'mod_quiz','2014051200','2014051200','Plugin installed',NULL,'',0,1413856900),(111,0,'mod_resource',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856900),(112,0,'mod_resource','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856900),(113,0,'mod_resource','2014051200','2014051200','Plugin installed',NULL,'',0,1413856901),(114,0,'mod_scorm',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856901),(115,0,'mod_scorm','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856903),(116,0,'mod_scorm','2014051200','2014051200','Plugin installed',NULL,'',0,1413856904),(117,0,'mod_survey',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856905),(118,0,'mod_survey','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856905),(119,0,'mod_survey','2014051200','2014051200','Plugin installed',NULL,'',0,1413856912),(120,0,'mod_url',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856912),(121,0,'mod_url','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856912),(122,0,'mod_url','2014051200','2014051200','Plugin installed',NULL,'',0,1413856913),(123,0,'mod_wiki',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856913),(124,0,'mod_wiki','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856914),(125,0,'mod_wiki','2014051200','2014051200','Plugin installed',NULL,'',0,1413856917),(126,0,'mod_workshop',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856917),(127,0,'mod_workshop','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856919),(128,0,'mod_workshop','2014051200','2014051200','Plugin installed',NULL,'',0,1413856925),(129,0,'auth_cas',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856925),(130,0,'auth_cas','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856925),(131,0,'auth_cas','2014051200','2014051200','Plugin installed',NULL,'',0,1413856925),(132,0,'auth_db',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856926),(133,0,'auth_db','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856926),(134,0,'auth_db','2014051200','2014051200','Plugin installed',NULL,'',0,1413856926),(135,0,'auth_email',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856926),(136,0,'auth_email','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856926),(137,0,'auth_email','2014051200','2014051200','Plugin installed',NULL,'',0,1413856926),(138,0,'auth_fc',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856926),(139,0,'auth_fc','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856927),(140,0,'auth_fc','2014051200','2014051200','Plugin installed',NULL,'',0,1413856927),(141,0,'auth_imap',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856927),(142,0,'auth_imap','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856927),(143,0,'auth_imap','2014051200','2014051200','Plugin installed',NULL,'',0,1413856927),(144,0,'auth_ldap',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856927),(145,0,'auth_ldap','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856928),(146,0,'auth_ldap','2014051200','2014051200','Plugin installed',NULL,'',0,1413856928),(147,0,'auth_manual',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856928),(148,0,'auth_manual','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856928),(149,0,'auth_manual','2014051200','2014051200','Plugin installed',NULL,'',0,1413856928),(150,0,'auth_mnet',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856928),(151,0,'auth_mnet','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856928),(152,0,'auth_mnet','2014051200','2014051200','Plugin installed',NULL,'',0,1413856933),(153,0,'auth_nntp',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856933),(154,0,'auth_nntp','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856933),(155,0,'auth_nntp','2014051200','2014051200','Plugin installed',NULL,'',0,1413856933),(156,0,'auth_nologin',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856933),(157,0,'auth_nologin','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856933),(158,0,'auth_nologin','2014051200','2014051200','Plugin installed',NULL,'',0,1413856933),(159,0,'auth_none',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856933),(160,0,'auth_none','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856934),(161,0,'auth_none','2014051200','2014051200','Plugin installed',NULL,'',0,1413856934),(162,0,'auth_pam',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856934),(163,0,'auth_pam','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856934),(164,0,'auth_pam','2014051200','2014051200','Plugin installed',NULL,'',0,1413856934),(165,0,'auth_pop3',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856934),(166,0,'auth_pop3','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856934),(167,0,'auth_pop3','2014051200','2014051200','Plugin installed',NULL,'',0,1413856935),(168,0,'auth_radius',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856935),(169,0,'auth_radius','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856935),(170,0,'auth_radius','2014051200','2014051200','Plugin installed',NULL,'',0,1413856935),(171,0,'auth_shibboleth',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856936),(172,0,'auth_shibboleth','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856936),(173,0,'auth_shibboleth','2014051200','2014051200','Plugin installed',NULL,'',0,1413856936),(174,0,'auth_webservice',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856936),(175,0,'auth_webservice','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856936),(176,0,'auth_webservice','2014051200','2014051200','Plugin installed',NULL,'',0,1413856936),(177,0,'calendartype_gregorian',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856936),(178,0,'calendartype_gregorian','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856937),(179,0,'calendartype_gregorian','2014051200','2014051200','Plugin installed',NULL,'',0,1413856937),(180,0,'enrol_category',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856937),(181,0,'enrol_category','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856937),(182,0,'enrol_category','2014051200','2014051200','Plugin installed',NULL,'',0,1413856937),(183,0,'enrol_cohort',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856937),(184,0,'enrol_cohort','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856938),(185,0,'enrol_cohort','2014051200','2014051200','Plugin installed',NULL,'',0,1413856938),(186,0,'enrol_database',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856938),(187,0,'enrol_database','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856938),(188,0,'enrol_database','2014051200','2014051200','Plugin installed',NULL,'',0,1413856939),(189,0,'enrol_flatfile',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856939),(190,0,'enrol_flatfile','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856939),(191,0,'enrol_flatfile','2014051200','2014051200','Plugin installed',NULL,'',0,1413856940),(192,0,'enrol_guest',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856940),(193,0,'enrol_guest','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856940),(194,0,'enrol_guest','2014051200','2014051200','Plugin installed',NULL,'',0,1413856940),(195,0,'enrol_imsenterprise',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856941),(196,0,'enrol_imsenterprise','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856941),(197,0,'enrol_imsenterprise','2014051200','2014051200','Plugin installed',NULL,'',0,1413856941),(198,0,'enrol_ldap',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856941),(199,0,'enrol_ldap','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856941),(200,0,'enrol_ldap','2014051200','2014051200','Plugin installed',NULL,'',0,1413856942),(201,0,'enrol_manual',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856942),(202,0,'enrol_manual','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856942),(203,0,'enrol_manual','2014051200','2014051200','Plugin installed',NULL,'',0,1413856944),(204,0,'enrol_meta',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856944),(205,0,'enrol_meta','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856944),(206,0,'enrol_meta','2014051200','2014051200','Plugin installed',NULL,'',0,1413856945),(207,0,'enrol_mnet',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856945),(208,0,'enrol_mnet','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856945),(209,0,'enrol_mnet','2014051200','2014051200','Plugin installed',NULL,'',0,1413856947),(210,0,'enrol_paypal',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856947),(211,0,'enrol_paypal','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856947),(212,0,'enrol_paypal','2014051200','2014051200','Plugin installed',NULL,'',0,1413856948),(213,0,'enrol_self',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856948),(214,0,'enrol_self','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856948),(215,0,'enrol_self','2014051200','2014051200','Plugin installed',NULL,'',0,1413856949),(216,0,'message_airnotifier',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856949),(217,0,'message_airnotifier','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856949),(218,0,'message_airnotifier','2014051200','2014051200','Plugin installed',NULL,'',0,1413856950),(219,0,'message_email',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856950),(220,0,'message_email','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856951),(221,0,'message_email','2014051200','2014051200','Plugin installed',NULL,'',0,1413856951),(222,0,'message_jabber',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856951),(223,0,'message_jabber','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856951),(224,0,'message_jabber','2014051200','2014051200','Plugin installed',NULL,'',0,1413856952),(225,0,'message_popup',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856952),(226,0,'message_popup','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856952),(227,0,'message_popup','2014051200','2014051200','Plugin installed',NULL,'',0,1413856953),(228,0,'block_activity_modules',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856953),(229,0,'block_activity_modules','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856953),(230,0,'block_activity_modules','2014051200','2014051200','Plugin installed',NULL,'',0,1413856953),(231,0,'block_admin_bookmarks',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856954),(232,0,'block_admin_bookmarks','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856954),(233,0,'block_admin_bookmarks','2014051200','2014051200','Plugin installed',NULL,'',0,1413856955),(234,0,'block_badges',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856955),(235,0,'block_badges','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856955),(236,0,'block_badges','2014051200','2014051200','Plugin installed',NULL,'',0,1413856955),(237,0,'block_blog_menu',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856955),(238,0,'block_blog_menu','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856956),(239,0,'block_blog_menu','2014051200','2014051200','Plugin installed',NULL,'',0,1413856956),(240,0,'block_blog_recent',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856956),(241,0,'block_blog_recent','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856956),(242,0,'block_blog_recent','2014051200','2014051200','Plugin installed',NULL,'',0,1413856957),(243,0,'block_blog_tags',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856957),(244,0,'block_blog_tags','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856957),(245,0,'block_blog_tags','2014051200','2014051200','Plugin installed',NULL,'',0,1413856957),(246,0,'block_calendar_month',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856957),(247,0,'block_calendar_month','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856958),(248,0,'block_calendar_month','2014051200','2014051200','Plugin installed',NULL,'',0,1413856958),(249,0,'block_calendar_upcoming',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856958),(250,0,'block_calendar_upcoming','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856958),(251,0,'block_calendar_upcoming','2014051200','2014051200','Plugin installed',NULL,'',0,1413856959),(252,0,'block_comments',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856959),(253,0,'block_comments','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856959),(254,0,'block_comments','2014051200','2014051200','Plugin installed',NULL,'',0,1413856960),(255,0,'block_community',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856960),(256,0,'block_community','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856960),(257,0,'block_community','2014051200','2014051200','Plugin installed',NULL,'',0,1413856961),(258,0,'block_completionstatus',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856961),(259,0,'block_completionstatus','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856961),(260,0,'block_completionstatus','2014051200','2014051200','Plugin installed',NULL,'',0,1413856961),(261,0,'block_course_list',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856961),(262,0,'block_course_list','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856961),(263,0,'block_course_list','2014051200','2014051200','Plugin installed',NULL,'',0,1413856962),(264,0,'block_course_overview',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856962),(265,0,'block_course_overview','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856962),(266,0,'block_course_overview','2014051200','2014051200','Plugin installed',NULL,'',0,1413856963),(267,0,'block_course_summary',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856963),(268,0,'block_course_summary','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856963),(269,0,'block_course_summary','2014051200','2014051200','Plugin installed',NULL,'',0,1413856963),(270,0,'block_feedback',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856963),(271,0,'block_feedback','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856963),(272,0,'block_feedback','2014051200','2014051200','Plugin installed',NULL,'',0,1413856964),(273,0,'block_glossary_random',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856964),(274,0,'block_glossary_random','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856964),(275,0,'block_glossary_random','2014051200','2014051200','Plugin installed',NULL,'',0,1413856965),(276,0,'block_html',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856965),(277,0,'block_html','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856965),(278,0,'block_html','2014051200','2014051200','Plugin installed',NULL,'',0,1413856966),(279,0,'block_login',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856966),(280,0,'block_login','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856966),(281,0,'block_login','2014051200','2014051200','Plugin installed',NULL,'',0,1413856966),(282,0,'block_mentees',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856967),(283,0,'block_mentees','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856967),(284,0,'block_mentees','2014051200','2014051200','Plugin installed',NULL,'',0,1413856967),(285,0,'block_messages',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856967),(286,0,'block_messages','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856967),(287,0,'block_messages','2014051200','2014051200','Plugin installed',NULL,'',0,1413856968),(288,0,'block_mnet_hosts',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856968),(289,0,'block_mnet_hosts','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856968),(290,0,'block_mnet_hosts','2014051200','2014051200','Plugin installed',NULL,'',0,1413856969),(291,0,'block_myprofile',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856969),(292,0,'block_myprofile','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856969),(293,0,'block_myprofile','2014051200','2014051200','Plugin installed',NULL,'',0,1413856969),(294,0,'block_navigation',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856969),(295,0,'block_navigation','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856970),(296,0,'block_navigation','2014051200','2014051200','Plugin installed',NULL,'',0,1413856970),(297,0,'block_news_items',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856970),(298,0,'block_news_items','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856970),(299,0,'block_news_items','2014051200','2014051200','Plugin installed',NULL,'',0,1413856971),(300,0,'block_online_users',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856971),(301,0,'block_online_users','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856971),(302,0,'block_online_users','2014051200','2014051200','Plugin installed',NULL,'',0,1413856972),(303,0,'block_participants',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856972),(304,0,'block_participants','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856973),(305,0,'block_participants','2014051200','2014051200','Plugin installed',NULL,'',0,1413856973),(306,0,'block_private_files',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856973),(307,0,'block_private_files','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856973),(308,0,'block_private_files','2014051200','2014051200','Plugin installed',NULL,'',0,1413856974),(309,0,'block_quiz_results',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856974),(310,0,'block_quiz_results','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856974),(311,0,'block_quiz_results','2014051200','2014051200','Plugin installed',NULL,'',0,1413856974),(312,0,'block_recent_activity',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856974),(313,0,'block_recent_activity','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856975),(314,0,'block_recent_activity','2014051200','2014051200','Plugin installed',NULL,'',0,1413856976),(315,0,'block_rss_client',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856976),(316,0,'block_rss_client','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856976),(317,0,'block_rss_client','2014051200','2014051200','Plugin installed',NULL,'',0,1413856977),(318,0,'block_search_forums',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856977),(319,0,'block_search_forums','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856977),(320,0,'block_search_forums','2014051200','2014051200','Plugin installed',NULL,'',0,1413856977),(321,0,'block_section_links',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856977),(322,0,'block_section_links','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856978),(323,0,'block_section_links','2014051200','2014051200','Plugin installed',NULL,'',0,1413856978),(324,0,'block_selfcompletion',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856978),(325,0,'block_selfcompletion','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856978),(326,0,'block_selfcompletion','2014051200','2014051200','Plugin installed',NULL,'',0,1413856979),(327,0,'block_settings',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856979),(328,0,'block_settings','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856979),(329,0,'block_settings','2014051200','2014051200','Plugin installed',NULL,'',0,1413856979),(330,0,'block_site_main_menu',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856979),(331,0,'block_site_main_menu','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856980),(332,0,'block_site_main_menu','2014051200','2014051200','Plugin installed',NULL,'',0,1413856980),(333,0,'block_social_activities',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856980),(334,0,'block_social_activities','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856980),(335,0,'block_social_activities','2014051200','2014051200','Plugin installed',NULL,'',0,1413856980),(336,0,'block_tag_flickr',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856981),(337,0,'block_tag_flickr','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856981),(338,0,'block_tag_flickr','2014051200','2014051200','Plugin installed',NULL,'',0,1413856981),(339,0,'block_tag_youtube',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856981),(340,0,'block_tag_youtube','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856981),(341,0,'block_tag_youtube','2014051200','2014051200','Plugin installed',NULL,'',0,1413856982),(342,0,'block_tags',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856982),(343,0,'block_tags','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856982),(344,0,'block_tags','2014051200','2014051200','Plugin installed',NULL,'',0,1413856982),(345,0,'filter_activitynames',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856984),(346,0,'filter_activitynames','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856984),(347,0,'filter_activitynames','2014051200','2014051200','Plugin installed',NULL,'',0,1413856985),(348,0,'filter_algebra',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856985),(349,0,'filter_algebra','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856985),(350,0,'filter_algebra','2014051200','2014051200','Plugin installed',NULL,'',0,1413856985),(351,0,'filter_censor',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856985),(352,0,'filter_censor','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856985),(353,0,'filter_censor','2014051200','2014051200','Plugin installed',NULL,'',0,1413856985),(354,0,'filter_data',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856985),(355,0,'filter_data','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856986),(356,0,'filter_data','2014051200','2014051200','Plugin installed',NULL,'',0,1413856986),(357,0,'filter_emailprotect',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856986),(358,0,'filter_emailprotect','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856987),(359,0,'filter_emailprotect','2014051200','2014051200','Plugin installed',NULL,'',0,1413856987),(360,0,'filter_emoticon',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856987),(361,0,'filter_emoticon','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856987),(362,0,'filter_emoticon','2014051200','2014051200','Plugin installed',NULL,'',0,1413856987),(363,0,'filter_glossary',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856987),(364,0,'filter_glossary','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856988),(365,0,'filter_glossary','2014051200','2014051200','Plugin installed',NULL,'',0,1413856988),(366,0,'filter_mathjaxloader',NULL,'2014051201','Starting plugin installation',NULL,'',0,1413856988),(367,0,'filter_mathjaxloader','2014051201','2014051201','Upgrade savepoint reached',NULL,'',0,1413856988),(368,0,'filter_mathjaxloader','2014051201','2014051201','Plugin installed',NULL,'',0,1413856988),(369,0,'filter_mediaplugin',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856989),(370,0,'filter_mediaplugin','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856989),(371,0,'filter_mediaplugin','2014051200','2014051200','Plugin installed',NULL,'',0,1413856989),(372,0,'filter_multilang',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856989),(373,0,'filter_multilang','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856989),(374,0,'filter_multilang','2014051200','2014051200','Plugin installed',NULL,'',0,1413856989),(375,0,'filter_tex',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856990),(376,0,'filter_tex','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856990),(377,0,'filter_tex','2014051200','2014051200','Plugin installed',NULL,'',0,1413856990),(378,0,'filter_tidy',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856990),(379,0,'filter_tidy','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856990),(380,0,'filter_tidy','2014051200','2014051200','Plugin installed',NULL,'',0,1413856991),(381,0,'filter_urltolink',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856991),(382,0,'filter_urltolink','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856991),(383,0,'filter_urltolink','2014051200','2014051200','Plugin installed',NULL,'',0,1413856991),(384,0,'editor_atto',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856991),(385,0,'editor_atto','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856991),(386,0,'editor_atto','2014051200','2014051200','Plugin installed',NULL,'',0,1413856992),(387,0,'editor_textarea',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856992),(388,0,'editor_textarea','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856992),(389,0,'editor_textarea','2014051200','2014051200','Plugin installed',NULL,'',0,1413856992),(390,0,'editor_tinymce',NULL,'2014051201','Starting plugin installation',NULL,'',0,1413856992),(391,0,'editor_tinymce','2014051201','2014051201','Upgrade savepoint reached',NULL,'',0,1413856992),(392,0,'editor_tinymce','2014051201','2014051201','Plugin installed',NULL,'',0,1413856992),(393,0,'format_singleactivity',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856993),(394,0,'format_singleactivity','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856993),(395,0,'format_singleactivity','2014051200','2014051200','Plugin installed',NULL,'',0,1413856993),(396,0,'format_social',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856993),(397,0,'format_social','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856993),(398,0,'format_social','2014051200','2014051200','Plugin installed',NULL,'',0,1413856993),(399,0,'format_topics',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856993),(400,0,'format_topics','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856993),(401,0,'format_topics','2014051200','2014051200','Plugin installed',NULL,'',0,1413856994),(402,0,'format_weeks',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856994),(403,0,'format_weeks','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856994),(404,0,'format_weeks','2014051200','2014051200','Plugin installed',NULL,'',0,1413856994),(405,0,'profilefield_checkbox',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856994),(406,0,'profilefield_checkbox','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856994),(407,0,'profilefield_checkbox','2014051200','2014051200','Plugin installed',NULL,'',0,1413856995),(408,0,'profilefield_datetime',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856995),(409,0,'profilefield_datetime','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856995),(410,0,'profilefield_datetime','2014051200','2014051200','Plugin installed',NULL,'',0,1413856995),(411,0,'profilefield_menu',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856995),(412,0,'profilefield_menu','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856995),(413,0,'profilefield_menu','2014051200','2014051200','Plugin installed',NULL,'',0,1413856996),(414,0,'profilefield_text',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856996),(415,0,'profilefield_text','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856996),(416,0,'profilefield_text','2014051200','2014051200','Plugin installed',NULL,'',0,1413856996),(417,0,'profilefield_textarea',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856996),(418,0,'profilefield_textarea','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856996),(419,0,'profilefield_textarea','2014051200','2014051200','Plugin installed',NULL,'',0,1413856997),(420,0,'report_backups',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856997),(421,0,'report_backups','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856997),(422,0,'report_backups','2014051200','2014051200','Plugin installed',NULL,'',0,1413856997),(423,0,'report_completion',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856997),(424,0,'report_completion','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856997),(425,0,'report_completion','2014051200','2014051200','Plugin installed',NULL,'',0,1413856998),(426,0,'report_configlog',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856998),(427,0,'report_configlog','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856998),(428,0,'report_configlog','2014051200','2014051200','Plugin installed',NULL,'',0,1413856998),(429,0,'report_courseoverview',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856999),(430,0,'report_courseoverview','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856999),(431,0,'report_courseoverview','2014051200','2014051200','Plugin installed',NULL,'',0,1413856999),(432,0,'report_eventlist',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413856999),(433,0,'report_eventlist','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413856999),(434,0,'report_eventlist','2014051200','2014051200','Plugin installed',NULL,'',0,1413857000),(435,0,'report_log',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857000),(436,0,'report_log','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857000),(437,0,'report_log','2014051200','2014051200','Plugin installed',NULL,'',0,1413857001),(438,0,'report_loglive',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857001),(439,0,'report_loglive','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857001),(440,0,'report_loglive','2014051200','2014051200','Plugin installed',NULL,'',0,1413857001),(441,0,'report_outline',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857001),(442,0,'report_outline','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857001),(443,0,'report_outline','2014051200','2014051200','Plugin installed',NULL,'',0,1413857002),(444,0,'report_participation',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857002),(445,0,'report_participation','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857002),(446,0,'report_participation','2014051200','2014051200','Plugin installed',NULL,'',0,1413857003),(447,0,'report_performance',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857003),(448,0,'report_performance','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857003),(449,0,'report_performance','2014051200','2014051200','Plugin installed',NULL,'',0,1413857003),(450,0,'report_progress',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857003),(451,0,'report_progress','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857004),(452,0,'report_progress','2014051200','2014051200','Plugin installed',NULL,'',0,1413857004),(453,0,'report_questioninstances',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857004),(454,0,'report_questioninstances','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857004),(455,0,'report_questioninstances','2014051200','2014051200','Plugin installed',NULL,'',0,1413857005),(456,0,'report_security',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857005),(457,0,'report_security','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857005),(458,0,'report_security','2014051200','2014051200','Plugin installed',NULL,'',0,1413857005),(459,0,'report_stats',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857005),(460,0,'report_stats','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857005),(461,0,'report_stats','2014051200','2014051200','Plugin installed',NULL,'',0,1413857006),(462,0,'gradeexport_ods',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857006),(463,0,'gradeexport_ods','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857006),(464,0,'gradeexport_ods','2014051200','2014051200','Plugin installed',NULL,'',0,1413857007),(465,0,'gradeexport_txt',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857007),(466,0,'gradeexport_txt','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857007),(467,0,'gradeexport_txt','2014051200','2014051200','Plugin installed',NULL,'',0,1413857007),(468,0,'gradeexport_xls',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857008),(469,0,'gradeexport_xls','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857008),(470,0,'gradeexport_xls','2014051200','2014051200','Plugin installed',NULL,'',0,1413857009),(471,0,'gradeexport_xml',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857009),(472,0,'gradeexport_xml','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857009),(473,0,'gradeexport_xml','2014051200','2014051200','Plugin installed',NULL,'',0,1413857010),(474,0,'gradeimport_csv',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857010),(475,0,'gradeimport_csv','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857010),(476,0,'gradeimport_csv','2014051200','2014051200','Plugin installed',NULL,'',0,1413857010),(477,0,'gradeimport_xml',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857010),(478,0,'gradeimport_xml','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857010),(479,0,'gradeimport_xml','2014051200','2014051200','Plugin installed',NULL,'',0,1413857011),(480,0,'gradereport_grader',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857011),(481,0,'gradereport_grader','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857011),(482,0,'gradereport_grader','2014051200','2014051200','Plugin installed',NULL,'',0,1413857012),(483,0,'gradereport_outcomes',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857012),(484,0,'gradereport_outcomes','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857012),(485,0,'gradereport_outcomes','2014051200','2014051200','Plugin installed',NULL,'',0,1413857012),(486,0,'gradereport_overview',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857012),(487,0,'gradereport_overview','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857013),(488,0,'gradereport_overview','2014051200','2014051200','Plugin installed',NULL,'',0,1413857013),(489,0,'gradereport_user',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857013),(490,0,'gradereport_user','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857013),(491,0,'gradereport_user','2014051200','2014051200','Plugin installed',NULL,'',0,1413857014),(492,0,'gradingform_guide',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857014),(493,0,'gradingform_guide','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857014),(494,0,'gradingform_guide','2014051200','2014051200','Plugin installed',NULL,'',0,1413857014),(495,0,'gradingform_rubric',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857014),(496,0,'gradingform_rubric','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857015),(497,0,'gradingform_rubric','2014051200','2014051200','Plugin installed',NULL,'',0,1413857015),(498,0,'mnetservice_enrol',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857015),(499,0,'mnetservice_enrol','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857015),(500,0,'mnetservice_enrol','2014051200','2014051200','Plugin installed',NULL,'',0,1413857016),(501,0,'webservice_amf',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857016),(502,0,'webservice_amf','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857016),(503,0,'webservice_amf','2014051200','2014051200','Plugin installed',NULL,'',0,1413857016),(504,0,'webservice_rest',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857016),(505,0,'webservice_rest','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857016),(506,0,'webservice_rest','2014051200','2014051200','Plugin installed',NULL,'',0,1413857016),(507,0,'webservice_soap',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857016),(508,0,'webservice_soap','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857016),(509,0,'webservice_soap','2014051200','2014051200','Plugin installed',NULL,'',0,1413857017),(510,0,'webservice_xmlrpc',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857017),(511,0,'webservice_xmlrpc','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857017),(512,0,'webservice_xmlrpc','2014051200','2014051200','Plugin installed',NULL,'',0,1413857017),(513,0,'repository_alfresco',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857017),(514,0,'repository_alfresco','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857017),(515,0,'repository_alfresco','2014051200','2014051200','Plugin installed',NULL,'',0,1413857018),(516,0,'repository_areafiles',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857018),(517,0,'repository_areafiles','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857018),(518,0,'repository_areafiles','2014051200','2014051200','Plugin installed',NULL,'',0,1413857019),(519,0,'repository_boxnet',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857019),(520,0,'repository_boxnet','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857019),(521,0,'repository_boxnet','2014051200','2014051200','Plugin installed',NULL,'',0,1413857020),(522,0,'repository_coursefiles',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857020),(523,0,'repository_coursefiles','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857020),(524,0,'repository_coursefiles','2014051200','2014051200','Plugin installed',NULL,'',0,1413857020),(525,0,'repository_dropbox',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857020),(526,0,'repository_dropbox','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857021),(527,0,'repository_dropbox','2014051200','2014051200','Plugin installed',NULL,'',0,1413857021),(528,0,'repository_equella',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857021),(529,0,'repository_equella','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857021),(530,0,'repository_equella','2014051200','2014051200','Plugin installed',NULL,'',0,1413857022),(531,0,'repository_filesystem',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857022),(532,0,'repository_filesystem','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857022),(533,0,'repository_filesystem','2014051200','2014051200','Plugin installed',NULL,'',0,1413857022),(534,0,'repository_flickr',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857022),(535,0,'repository_flickr','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857022),(536,0,'repository_flickr','2014051200','2014051200','Plugin installed',NULL,'',0,1413857023),(537,0,'repository_flickr_public',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857023),(538,0,'repository_flickr_public','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857023),(539,0,'repository_flickr_public','2014051200','2014051200','Plugin installed',NULL,'',0,1413857023),(540,0,'repository_googledocs',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857023),(541,0,'repository_googledocs','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857023),(542,0,'repository_googledocs','2014051200','2014051200','Plugin installed',NULL,'',0,1413857024),(543,0,'repository_local',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857024),(544,0,'repository_local','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857024),(545,0,'repository_local','2014051200','2014051200','Plugin installed',NULL,'',0,1413857025),(546,0,'repository_merlot',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857025),(547,0,'repository_merlot','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857025),(548,0,'repository_merlot','2014051200','2014051200','Plugin installed',NULL,'',0,1413857025),(549,0,'repository_picasa',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857025),(550,0,'repository_picasa','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857025),(551,0,'repository_picasa','2014051200','2014051200','Plugin installed',NULL,'',0,1413857026),(552,0,'repository_recent',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857026),(553,0,'repository_recent','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857026),(554,0,'repository_recent','2014051200','2014051200','Plugin installed',NULL,'',0,1413857027),(555,0,'repository_s3',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857027),(556,0,'repository_s3','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857027),(557,0,'repository_s3','2014051200','2014051200','Plugin installed',NULL,'',0,1413857027),(558,0,'repository_skydrive',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857027),(559,0,'repository_skydrive','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857027),(560,0,'repository_skydrive','2014051200','2014051200','Plugin installed',NULL,'',0,1413857028),(561,0,'repository_upload',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857028),(562,0,'repository_upload','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857028),(563,0,'repository_upload','2014051200','2014051200','Plugin installed',NULL,'',0,1413857028),(564,0,'repository_url',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857028),(565,0,'repository_url','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857029),(566,0,'repository_url','2014051200','2014051200','Plugin installed',NULL,'',0,1413857029),(567,0,'repository_user',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857029),(568,0,'repository_user','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857030),(569,0,'repository_user','2014051200','2014051200','Plugin installed',NULL,'',0,1413857031),(570,0,'repository_webdav',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857031),(571,0,'repository_webdav','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857031),(572,0,'repository_webdav','2014051200','2014051200','Plugin installed',NULL,'',0,1413857031),(573,0,'repository_wikimedia',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857031),(574,0,'repository_wikimedia','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857031),(575,0,'repository_wikimedia','2014051200','2014051200','Plugin installed',NULL,'',0,1413857032),(576,0,'repository_youtube',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857032),(577,0,'repository_youtube','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857032),(578,0,'repository_youtube','2014051200','2014051200','Plugin installed',NULL,'',0,1413857033),(579,0,'portfolio_boxnet',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857033),(580,0,'portfolio_boxnet','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857033),(581,0,'portfolio_boxnet','2014051200','2014051200','Plugin installed',NULL,'',0,1413857033),(582,0,'portfolio_download',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857033),(583,0,'portfolio_download','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857033),(584,0,'portfolio_download','2014051200','2014051200','Plugin installed',NULL,'',0,1413857034),(585,0,'portfolio_flickr',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857034),(586,0,'portfolio_flickr','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857034),(587,0,'portfolio_flickr','2014051200','2014051200','Plugin installed',NULL,'',0,1413857034),(588,0,'portfolio_googledocs',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857034),(589,0,'portfolio_googledocs','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857034),(590,0,'portfolio_googledocs','2014051200','2014051200','Plugin installed',NULL,'',0,1413857034),(591,0,'portfolio_mahara',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857034),(592,0,'portfolio_mahara','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857035),(593,0,'portfolio_mahara','2014051200','2014051200','Plugin installed',NULL,'',0,1413857035),(594,0,'portfolio_picasa',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857035),(595,0,'portfolio_picasa','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857035),(596,0,'portfolio_picasa','2014051200','2014051200','Plugin installed',NULL,'',0,1413857036),(597,0,'qbehaviour_adaptive',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857036),(598,0,'qbehaviour_adaptive','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857036),(599,0,'qbehaviour_adaptive','2014051200','2014051200','Plugin installed',NULL,'',0,1413857036),(600,0,'qbehaviour_adaptivenopenalty',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857036),(601,0,'qbehaviour_adaptivenopenalty','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857036),(602,0,'qbehaviour_adaptivenopenalty','2014051200','2014051200','Plugin installed',NULL,'',0,1413857036),(603,0,'qbehaviour_deferredcbm',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857036),(604,0,'qbehaviour_deferredcbm','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857036),(605,0,'qbehaviour_deferredcbm','2014051200','2014051200','Plugin installed',NULL,'',0,1413857037),(606,0,'qbehaviour_deferredfeedback',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857037),(607,0,'qbehaviour_deferredfeedback','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857037),(608,0,'qbehaviour_deferredfeedback','2014051200','2014051200','Plugin installed',NULL,'',0,1413857037),(609,0,'qbehaviour_immediatecbm',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857037),(610,0,'qbehaviour_immediatecbm','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857037),(611,0,'qbehaviour_immediatecbm','2014051200','2014051200','Plugin installed',NULL,'',0,1413857037),(612,0,'qbehaviour_immediatefeedback',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857037),(613,0,'qbehaviour_immediatefeedback','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857038),(614,0,'qbehaviour_immediatefeedback','2014051200','2014051200','Plugin installed',NULL,'',0,1413857038),(615,0,'qbehaviour_informationitem',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857038),(616,0,'qbehaviour_informationitem','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857038),(617,0,'qbehaviour_informationitem','2014051200','2014051200','Plugin installed',NULL,'',0,1413857038),(618,0,'qbehaviour_interactive',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857038),(619,0,'qbehaviour_interactive','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857039),(620,0,'qbehaviour_interactive','2014051200','2014051200','Plugin installed',NULL,'',0,1413857039),(621,0,'qbehaviour_interactivecountback',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857039),(622,0,'qbehaviour_interactivecountback','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857039),(623,0,'qbehaviour_interactivecountback','2014051200','2014051200','Plugin installed',NULL,'',0,1413857039),(624,0,'qbehaviour_manualgraded',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857039),(625,0,'qbehaviour_manualgraded','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857040),(626,0,'qbehaviour_manualgraded','2014051200','2014051200','Plugin installed',NULL,'',0,1413857040),(627,0,'qbehaviour_missing',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857040),(628,0,'qbehaviour_missing','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857040),(629,0,'qbehaviour_missing','2014051200','2014051200','Plugin installed',NULL,'',0,1413857040),(630,0,'qformat_aiken',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857041),(631,0,'qformat_aiken','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857041),(632,0,'qformat_aiken','2014051200','2014051200','Plugin installed',NULL,'',0,1413857041),(633,0,'qformat_blackboard_six',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857041),(634,0,'qformat_blackboard_six','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857041),(635,0,'qformat_blackboard_six','2014051200','2014051200','Plugin installed',NULL,'',0,1413857042),(636,0,'qformat_examview',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857042),(637,0,'qformat_examview','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857042),(638,0,'qformat_examview','2014051200','2014051200','Plugin installed',NULL,'',0,1413857042),(639,0,'qformat_gift',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857042),(640,0,'qformat_gift','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857042),(641,0,'qformat_gift','2014051200','2014051200','Plugin installed',NULL,'',0,1413857042),(642,0,'qformat_learnwise',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857042),(643,0,'qformat_learnwise','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857043),(644,0,'qformat_learnwise','2014051200','2014051200','Plugin installed',NULL,'',0,1413857043),(645,0,'qformat_missingword',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857043),(646,0,'qformat_missingword','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857043),(647,0,'qformat_missingword','2014051200','2014051200','Plugin installed',NULL,'',0,1413857043),(648,0,'qformat_multianswer',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857043),(649,0,'qformat_multianswer','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857043),(650,0,'qformat_multianswer','2014051200','2014051200','Plugin installed',NULL,'',0,1413857043),(651,0,'qformat_webct',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857044),(652,0,'qformat_webct','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857044),(653,0,'qformat_webct','2014051200','2014051200','Plugin installed',NULL,'',0,1413857044),(654,0,'qformat_xhtml',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857044),(655,0,'qformat_xhtml','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857044),(656,0,'qformat_xhtml','2014051200','2014051200','Plugin installed',NULL,'',0,1413857044),(657,0,'qformat_xml',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857044),(658,0,'qformat_xml','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857045),(659,0,'qformat_xml','2014051200','2014051200','Plugin installed',NULL,'',0,1413857045),(660,0,'tool_assignmentupgrade',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857045),(661,0,'tool_assignmentupgrade','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857045),(662,0,'tool_assignmentupgrade','2014051200','2014051200','Plugin installed',NULL,'',0,1413857045),(663,0,'tool_availabilityconditions',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857045),(664,0,'tool_availabilityconditions','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857045),(665,0,'tool_availabilityconditions','2014051200','2014051200','Plugin installed',NULL,'',0,1413857045),(666,0,'tool_behat',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857045),(667,0,'tool_behat','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857046),(668,0,'tool_behat','2014051200','2014051200','Plugin installed',NULL,'',0,1413857046),(669,0,'tool_capability',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857046),(670,0,'tool_capability','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857046),(671,0,'tool_capability','2014051200','2014051200','Plugin installed',NULL,'',0,1413857046),(672,0,'tool_customlang',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857046),(673,0,'tool_customlang','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857047),(674,0,'tool_customlang','2014051200','2014051200','Plugin installed',NULL,'',0,1413857047),(675,0,'tool_dbtransfer',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857047),(676,0,'tool_dbtransfer','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857048),(677,0,'tool_dbtransfer','2014051200','2014051200','Plugin installed',NULL,'',0,1413857048),(678,0,'tool_generator',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857048),(679,0,'tool_generator','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857048),(680,0,'tool_generator','2014051200','2014051200','Plugin installed',NULL,'',0,1413857048),(681,0,'tool_health',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857048),(682,0,'tool_health','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857048),(683,0,'tool_health','2014051200','2014051200','Plugin installed',NULL,'',0,1413857049),(684,0,'tool_innodb',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857049),(685,0,'tool_innodb','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857049),(686,0,'tool_innodb','2014051200','2014051200','Plugin installed',NULL,'',0,1413857049),(687,0,'tool_installaddon',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857049),(688,0,'tool_installaddon','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857049),(689,0,'tool_installaddon','2014051200','2014051200','Plugin installed',NULL,'',0,1413857050),(690,0,'tool_langimport',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857050),(691,0,'tool_langimport','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857050),(692,0,'tool_langimport','2014051200','2014051200','Plugin installed',NULL,'',0,1413857050),(693,0,'tool_log',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857050),(694,0,'tool_log','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857050),(695,0,'tool_log','2014051200','2014051200','Plugin installed',NULL,'',0,1413857051),(696,0,'tool_multilangupgrade',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857051),(697,0,'tool_multilangupgrade','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857051),(698,0,'tool_multilangupgrade','2014051200','2014051200','Plugin installed',NULL,'',0,1413857051),(699,0,'tool_phpunit',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857051),(700,0,'tool_phpunit','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857052),(701,0,'tool_phpunit','2014051200','2014051200','Plugin installed',NULL,'',0,1413857052),(702,0,'tool_profiling',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857052),(703,0,'tool_profiling','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857052),(704,0,'tool_profiling','2014051200','2014051200','Plugin installed',NULL,'',0,1413857053),(705,0,'tool_replace',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857053),(706,0,'tool_replace','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857053),(707,0,'tool_replace','2014051200','2014051200','Plugin installed',NULL,'',0,1413857053),(708,0,'tool_spamcleaner',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857053),(709,0,'tool_spamcleaner','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857053),(710,0,'tool_spamcleaner','2014051200','2014051200','Plugin installed',NULL,'',0,1413857053),(711,0,'tool_task',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857054),(712,0,'tool_task','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857054),(713,0,'tool_task','2014051200','2014051200','Plugin installed',NULL,'',0,1413857054),(714,0,'tool_timezoneimport',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857054),(715,0,'tool_timezoneimport','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857054),(716,0,'tool_timezoneimport','2014051200','2014051200','Plugin installed',NULL,'',0,1413857054),(717,0,'tool_unsuproles',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857054),(718,0,'tool_unsuproles','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857055),(719,0,'tool_unsuproles','2014051200','2014051200','Plugin installed',NULL,'',0,1413857055),(720,0,'tool_uploadcourse',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857055),(721,0,'tool_uploadcourse','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857055),(722,0,'tool_uploadcourse','2014051200','2014051200','Plugin installed',NULL,'',0,1413857055),(723,0,'tool_uploaduser',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857055),(724,0,'tool_uploaduser','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857055),(725,0,'tool_uploaduser','2014051200','2014051200','Plugin installed',NULL,'',0,1413857056),(726,0,'tool_xmldb',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857056),(727,0,'tool_xmldb','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857056),(728,0,'tool_xmldb','2014051200','2014051200','Plugin installed',NULL,'',0,1413857056),(729,0,'cachestore_file',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857056),(730,0,'cachestore_file','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857056),(731,0,'cachestore_file','2014051200','2014051200','Plugin installed',NULL,'',0,1413857057),(732,0,'cachestore_memcache',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857057),(733,0,'cachestore_memcache','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857057),(734,0,'cachestore_memcache','2014051200','2014051200','Plugin installed',NULL,'',0,1413857057),(735,0,'cachestore_memcached',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857057),(736,0,'cachestore_memcached','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857057),(737,0,'cachestore_memcached','2014051200','2014051200','Plugin installed',NULL,'',0,1413857057),(738,0,'cachestore_mongodb',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857058),(739,0,'cachestore_mongodb','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857058),(740,0,'cachestore_mongodb','2014051200','2014051200','Plugin installed',NULL,'',0,1413857058),(741,0,'cachestore_session',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857058),(742,0,'cachestore_session','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857058),(743,0,'cachestore_session','2014051200','2014051200','Plugin installed',NULL,'',0,1413857058),(744,0,'cachestore_static',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857058),(745,0,'cachestore_static','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857058),(746,0,'cachestore_static','2014051200','2014051200','Plugin installed',NULL,'',0,1413857059),(747,0,'cachelock_file',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857059),(748,0,'cachelock_file','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857059),(749,0,'cachelock_file','2014051200','2014051200','Plugin installed',NULL,'',0,1413857059),(750,0,'theme_base',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857059),(751,0,'theme_base','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857059),(752,0,'theme_base','2014051200','2014051200','Plugin installed',NULL,'',0,1413857060),(753,0,'theme_bootstrapbase',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857060),(754,0,'theme_bootstrapbase','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857060),(755,0,'theme_bootstrapbase','2014051200','2014051200','Plugin installed',NULL,'',0,1413857060),(756,0,'theme_canvas',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857060),(757,0,'theme_canvas','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857060),(758,0,'theme_canvas','2014051200','2014051200','Plugin installed',NULL,'',0,1413857060),(759,0,'theme_clean',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857060),(760,0,'theme_clean','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857060),(761,0,'theme_clean','2014051200','2014051200','Plugin installed',NULL,'',0,1413857061),(762,0,'theme_more',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857061),(763,0,'theme_more','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857061),(764,0,'theme_more','2014051200','2014051200','Plugin installed',NULL,'',0,1413857061),(765,0,'assignsubmission_comments',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857061),(766,0,'assignsubmission_comments','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857061),(767,0,'assignsubmission_comments','2014051200','2014051200','Plugin installed',NULL,'',0,1413857062),(768,0,'assignsubmission_file',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857062),(769,0,'assignsubmission_file','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857063),(770,0,'assignsubmission_file','2014051200','2014051200','Plugin installed',NULL,'',0,1413857063),(771,0,'assignsubmission_onlinetext',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857063),(772,0,'assignsubmission_onlinetext','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857063),(773,0,'assignsubmission_onlinetext','2014051200','2014051200','Plugin installed',NULL,'',0,1413857064),(774,0,'assignfeedback_comments',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857064),(775,0,'assignfeedback_comments','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857064),(776,0,'assignfeedback_comments','2014051200','2014051200','Plugin installed',NULL,'',0,1413857065),(777,0,'assignfeedback_editpdf',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857065),(778,0,'assignfeedback_editpdf','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857065),(779,0,'assignfeedback_editpdf','2014051200','2014051200','Plugin installed',NULL,'',0,1413857066),(780,0,'assignfeedback_file',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857066),(781,0,'assignfeedback_file','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857066),(782,0,'assignfeedback_file','2014051200','2014051200','Plugin installed',NULL,'',0,1413857066),(783,0,'assignfeedback_offline',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857066),(784,0,'assignfeedback_offline','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857067),(785,0,'assignfeedback_offline','2014051200','2014051200','Plugin installed',NULL,'',0,1413857067),(786,0,'assignment_offline',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857067),(787,0,'assignment_offline','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857067),(788,0,'assignment_offline','2014051200','2014051200','Plugin installed',NULL,'',0,1413857067),(789,0,'assignment_online',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857067),(790,0,'assignment_online','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857067),(791,0,'assignment_online','2014051200','2014051200','Plugin installed',NULL,'',0,1413857067),(792,0,'assignment_upload',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857068),(793,0,'assignment_upload','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857068),(794,0,'assignment_upload','2014051200','2014051200','Plugin installed',NULL,'',0,1413857068),(795,0,'assignment_uploadsingle',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857068),(796,0,'assignment_uploadsingle','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857068),(797,0,'assignment_uploadsingle','2014051200','2014051200','Plugin installed',NULL,'',0,1413857068),(798,0,'booktool_exportimscp',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857068),(799,0,'booktool_exportimscp','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857068),(800,0,'booktool_exportimscp','2014051200','2014051200','Plugin installed',NULL,'',0,1413857069),(801,0,'booktool_importhtml',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857069),(802,0,'booktool_importhtml','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857069),(803,0,'booktool_importhtml','2014051200','2014051200','Plugin installed',NULL,'',0,1413857069),(804,0,'booktool_print',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857069),(805,0,'booktool_print','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857069),(806,0,'booktool_print','2014051200','2014051200','Plugin installed',NULL,'',0,1413857070),(807,0,'datafield_checkbox',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857070),(808,0,'datafield_checkbox','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857070),(809,0,'datafield_checkbox','2014051200','2014051200','Plugin installed',NULL,'',0,1413857070),(810,0,'datafield_date',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857071),(811,0,'datafield_date','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857071),(812,0,'datafield_date','2014051200','2014051200','Plugin installed',NULL,'',0,1413857071),(813,0,'datafield_file',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857071),(814,0,'datafield_file','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857071),(815,0,'datafield_file','2014051200','2014051200','Plugin installed',NULL,'',0,1413857071),(816,0,'datafield_latlong',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857071),(817,0,'datafield_latlong','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857071),(818,0,'datafield_latlong','2014051200','2014051200','Plugin installed',NULL,'',0,1413857072),(819,0,'datafield_menu',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857072),(820,0,'datafield_menu','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857072),(821,0,'datafield_menu','2014051200','2014051200','Plugin installed',NULL,'',0,1413857072),(822,0,'datafield_multimenu',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857072),(823,0,'datafield_multimenu','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857072),(824,0,'datafield_multimenu','2014051200','2014051200','Plugin installed',NULL,'',0,1413857072),(825,0,'datafield_number',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857073),(826,0,'datafield_number','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857073),(827,0,'datafield_number','2014051200','2014051200','Plugin installed',NULL,'',0,1413857073),(828,0,'datafield_picture',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857073),(829,0,'datafield_picture','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857074),(830,0,'datafield_picture','2014051200','2014051200','Plugin installed',NULL,'',0,1413857074),(831,0,'datafield_radiobutton',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857074),(832,0,'datafield_radiobutton','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857074),(833,0,'datafield_radiobutton','2014051200','2014051200','Plugin installed',NULL,'',0,1413857074),(834,0,'datafield_text',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857074),(835,0,'datafield_text','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857074),(836,0,'datafield_text','2014051200','2014051200','Plugin installed',NULL,'',0,1413857075),(837,0,'datafield_textarea',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857075),(838,0,'datafield_textarea','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857075),(839,0,'datafield_textarea','2014051200','2014051200','Plugin installed',NULL,'',0,1413857075),(840,0,'datafield_url',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857075),(841,0,'datafield_url','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857075),(842,0,'datafield_url','2014051200','2014051200','Plugin installed',NULL,'',0,1413857075),(843,0,'datapreset_imagegallery',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857075),(844,0,'datapreset_imagegallery','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857076),(845,0,'datapreset_imagegallery','2014051200','2014051200','Plugin installed',NULL,'',0,1413857076),(846,0,'quiz_grading',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857076),(847,0,'quiz_grading','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857076),(848,0,'quiz_grading','2014051200','2014051200','Plugin installed',NULL,'',0,1413857077),(849,0,'quiz_overview',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857077),(850,0,'quiz_overview','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857077),(851,0,'quiz_overview','2014051200','2014051200','Plugin installed',NULL,'',0,1413857078),(852,0,'quiz_responses',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857078),(853,0,'quiz_responses','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857078),(854,0,'quiz_responses','2014051200','2014051200','Plugin installed',NULL,'',0,1413857078),(855,0,'quiz_statistics',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857078),(856,0,'quiz_statistics','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857078),(857,0,'quiz_statistics','2014051200','2014051200','Plugin installed',NULL,'',0,1413857079),(858,0,'quizaccess_delaybetweenattempts',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857079),(859,0,'quizaccess_delaybetweenattempts','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857079),(860,0,'quizaccess_delaybetweenattempts','2014051200','2014051200','Plugin installed',NULL,'',0,1413857079),(861,0,'quizaccess_ipaddress',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857079),(862,0,'quizaccess_ipaddress','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857080),(863,0,'quizaccess_ipaddress','2014051200','2014051200','Plugin installed',NULL,'',0,1413857080),(864,0,'quizaccess_numattempts',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857080),(865,0,'quizaccess_numattempts','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857080),(866,0,'quizaccess_numattempts','2014051200','2014051200','Plugin installed',NULL,'',0,1413857080),(867,0,'quizaccess_openclosedate',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857080),(868,0,'quizaccess_openclosedate','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857080),(869,0,'quizaccess_openclosedate','2014051200','2014051200','Plugin installed',NULL,'',0,1413857080),(870,0,'quizaccess_password',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857081),(871,0,'quizaccess_password','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857081),(872,0,'quizaccess_password','2014051200','2014051200','Plugin installed',NULL,'',0,1413857081),(873,0,'quizaccess_safebrowser',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857081),(874,0,'quizaccess_safebrowser','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857081),(875,0,'quizaccess_safebrowser','2014051200','2014051200','Plugin installed',NULL,'',0,1413857081),(876,0,'quizaccess_securewindow',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857081),(877,0,'quizaccess_securewindow','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857081),(878,0,'quizaccess_securewindow','2014051200','2014051200','Plugin installed',NULL,'',0,1413857082),(879,0,'quizaccess_timelimit',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857082),(880,0,'quizaccess_timelimit','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857082),(881,0,'quizaccess_timelimit','2014051200','2014051200','Plugin installed',NULL,'',0,1413857082),(882,0,'scormreport_basic',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857082),(883,0,'scormreport_basic','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857082),(884,0,'scormreport_basic','2014051200','2014051200','Plugin installed',NULL,'',0,1413857082),(885,0,'scormreport_graphs',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857082),(886,0,'scormreport_graphs','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857083),(887,0,'scormreport_graphs','2014051200','2014051200','Plugin installed',NULL,'',0,1413857083),(888,0,'scormreport_interactions',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857083),(889,0,'scormreport_interactions','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857083),(890,0,'scormreport_interactions','2014051200','2014051200','Plugin installed',NULL,'',0,1413857083),(891,0,'scormreport_objectives',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857083),(892,0,'scormreport_objectives','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857083),(893,0,'scormreport_objectives','2014051200','2014051200','Plugin installed',NULL,'',0,1413857084),(894,0,'workshopform_accumulative',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857084),(895,0,'workshopform_accumulative','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857084),(896,0,'workshopform_accumulative','2014051200','2014051200','Plugin installed',NULL,'',0,1413857085),(897,0,'workshopform_comments',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857085),(898,0,'workshopform_numerrors',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857218),(899,0,'workshopform_numerrors','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857218),(900,0,'workshopform_numerrors','2014051200','2014051200','Plugin installed',NULL,'',0,1413857219),(901,0,'workshopform_rubric',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857219),(902,0,'workshopform_rubric','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857219),(903,0,'workshopform_rubric','2014051200','2014051200','Plugin installed',NULL,'',0,1413857219),(904,0,'workshopallocation_manual',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857219),(905,0,'workshopallocation_manual','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857220),(906,0,'workshopallocation_manual','2014051200','2014051200','Plugin installed',NULL,'',0,1413857220),(907,0,'workshopallocation_random',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857220),(908,0,'workshopallocation_random','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857220),(909,0,'workshopallocation_random','2014051200','2014051200','Plugin installed',NULL,'',0,1413857220),(910,0,'workshopallocation_scheduled',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857220),(911,0,'workshopallocation_scheduled','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857220),(912,0,'workshopallocation_scheduled','2014051200','2014051200','Plugin installed',NULL,'',0,1413857220),(913,0,'workshopeval_best',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857220),(914,0,'workshopeval_best','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857221),(915,0,'workshopeval_best','2014051200','2014051200','Plugin installed',NULL,'',0,1413857221),(916,0,'atto_accessibilitychecker',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857221),(917,0,'atto_accessibilitychecker','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857221),(918,0,'atto_accessibilitychecker','2014051200','2014051200','Plugin installed',NULL,'',0,1413857221),(919,0,'atto_accessibilityhelper',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857221),(920,0,'atto_accessibilityhelper','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857221),(921,0,'atto_accessibilityhelper','2014051200','2014051200','Plugin installed',NULL,'',0,1413857221),(922,0,'atto_align',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857221),(923,0,'atto_align','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857222),(924,0,'atto_align','2014051200','2014051200','Plugin installed',NULL,'',0,1413857222),(925,0,'atto_backcolor',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857222),(926,0,'atto_backcolor','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857222),(927,0,'atto_backcolor','2014051200','2014051200','Plugin installed',NULL,'',0,1413857222),(928,0,'atto_bold',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857222),(929,0,'atto_bold','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857223),(930,0,'atto_bold','2014051200','2014051200','Plugin installed',NULL,'',0,1413857223),(931,0,'atto_charmap',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857223),(932,0,'atto_charmap','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857223),(933,0,'atto_charmap','2014051200','2014051200','Plugin installed',NULL,'',0,1413857223),(934,0,'atto_clear',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857223),(935,0,'atto_clear','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857223),(936,0,'atto_clear','2014051200','2014051200','Plugin installed',NULL,'',0,1413857224),(937,0,'atto_collapse',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857224),(938,0,'atto_collapse','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857224),(939,0,'atto_collapse','2014051200','2014051200','Plugin installed',NULL,'',0,1413857224),(940,0,'atto_emoticon',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857224),(941,0,'atto_emoticon','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857224),(942,0,'atto_emoticon','2014051200','2014051200','Plugin installed',NULL,'',0,1413857224),(943,0,'atto_equation',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857224),(944,0,'atto_equation','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857225),(945,0,'atto_equation','2014051200','2014051200','Plugin installed',NULL,'',0,1413857225),(946,0,'atto_fontcolor',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857225),(947,0,'atto_fontcolor','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857225),(948,0,'atto_fontcolor','2014051200','2014051200','Plugin installed',NULL,'',0,1413857225),(949,0,'atto_html',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857225),(950,0,'atto_html','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857225),(951,0,'atto_html','2014051200','2014051200','Plugin installed',NULL,'',0,1413857225),(952,0,'atto_image',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857226),(953,0,'atto_image','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857226),(954,0,'atto_image','2014051200','2014051200','Plugin installed',NULL,'',0,1413857226),(955,0,'atto_indent',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857226),(956,0,'atto_indent','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857226),(957,0,'atto_indent','2014051200','2014051200','Plugin installed',NULL,'',0,1413857226),(958,0,'atto_italic',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857226),(959,0,'atto_italic','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857226),(960,0,'atto_italic','2014051200','2014051200','Plugin installed',NULL,'',0,1413857227),(961,0,'atto_link',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857227),(962,0,'atto_link','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857227),(963,0,'atto_link','2014051200','2014051200','Plugin installed',NULL,'',0,1413857227),(964,0,'atto_managefiles',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857227),(965,0,'atto_managefiles','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857227),(966,0,'atto_managefiles','2014051200','2014051200','Plugin installed',NULL,'',0,1413857227),(967,0,'atto_media',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857228),(968,0,'atto_media','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857228),(969,0,'atto_media','2014051200','2014051200','Plugin installed',NULL,'',0,1413857228),(970,0,'atto_noautolink',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857228),(971,0,'atto_noautolink','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857228),(972,0,'atto_noautolink','2014051200','2014051200','Plugin installed',NULL,'',0,1413857228),(973,0,'atto_orderedlist',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857228),(974,0,'atto_orderedlist','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857228),(975,0,'atto_orderedlist','2014051200','2014051200','Plugin installed',NULL,'',0,1413857229),(976,0,'atto_rtl',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857229),(977,0,'atto_rtl','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857229),(978,0,'atto_rtl','2014051200','2014051200','Plugin installed',NULL,'',0,1413857229),(979,0,'atto_strike',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857229),(980,0,'atto_strike','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857229),(981,0,'atto_strike','2014051200','2014051200','Plugin installed',NULL,'',0,1413857229),(982,0,'atto_subscript',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857230),(983,0,'atto_subscript','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857230),(984,0,'atto_subscript','2014051200','2014051200','Plugin installed',NULL,'',0,1413857230),(985,0,'atto_superscript',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857230),(986,0,'atto_superscript','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857230),(987,0,'atto_superscript','2014051200','2014051200','Plugin installed',NULL,'',0,1413857230),(988,0,'atto_table',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857230),(989,0,'atto_table','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857230),(990,0,'atto_table','2014051200','2014051200','Plugin installed',NULL,'',0,1413857231),(991,0,'atto_title',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857231),(992,0,'atto_title','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857231),(993,0,'atto_title','2014051200','2014051200','Plugin installed',NULL,'',0,1413857231),(994,0,'atto_underline',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857231),(995,0,'atto_underline','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857231),(996,0,'atto_underline','2014051200','2014051200','Plugin installed',NULL,'',0,1413857231),(997,0,'atto_undo',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857231),(998,0,'atto_undo','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857232),(999,0,'atto_undo','2014051200','2014051200','Plugin installed',NULL,'',0,1413857232),(1000,0,'atto_unorderedlist',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857232),(1001,0,'atto_unorderedlist','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857232),(1002,0,'atto_unorderedlist','2014051200','2014051200','Plugin installed',NULL,'',0,1413857232),(1003,0,'tinymce_ctrlhelp',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857232),(1004,0,'tinymce_ctrlhelp','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857232),(1005,0,'tinymce_ctrlhelp','2014051200','2014051200','Plugin installed',NULL,'',0,1413857233),(1006,0,'tinymce_dragmath',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857233),(1007,0,'tinymce_dragmath','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857233),(1008,0,'tinymce_dragmath','2014051200','2014051200','Plugin installed',NULL,'',0,1413857234),(1009,0,'tinymce_managefiles',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857234),(1010,0,'tinymce_managefiles','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857234),(1011,0,'tinymce_managefiles','2014051200','2014051200','Plugin installed',NULL,'',0,1413857234),(1012,0,'tinymce_moodleemoticon',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857234),(1013,0,'tinymce_moodleemoticon','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857234),(1014,0,'tinymce_moodleemoticon','2014051200','2014051200','Plugin installed',NULL,'',0,1413857234),(1015,0,'tinymce_moodleimage',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857235),(1016,0,'tinymce_moodleimage','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857235),(1017,0,'tinymce_moodleimage','2014051200','2014051200','Plugin installed',NULL,'',0,1413857235),(1018,0,'tinymce_moodlemedia',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857235),(1019,0,'tinymce_moodlemedia','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857235),(1020,0,'tinymce_moodlemedia','2014051200','2014051200','Plugin installed',NULL,'',0,1413857235),(1021,0,'tinymce_moodlenolink',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857235),(1022,0,'tinymce_moodlenolink','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857235),(1023,0,'tinymce_moodlenolink','2014051200','2014051200','Plugin installed',NULL,'',0,1413857236),(1024,0,'tinymce_pdw',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857236),(1025,0,'tinymce_pdw','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857236),(1026,0,'tinymce_pdw','2014051200','2014051200','Plugin installed',NULL,'',0,1413857236),(1027,0,'tinymce_spellchecker',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857236),(1028,0,'tinymce_spellchecker','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857236),(1029,0,'tinymce_spellchecker','2014051200','2014051200','Plugin installed',NULL,'',0,1413857236),(1030,0,'tinymce_wrap',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857236),(1031,0,'tinymce_wrap','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857237),(1032,0,'tinymce_wrap','2014051200','2014051200','Plugin installed',NULL,'',0,1413857237),(1033,0,'logstore_database',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857237),(1034,0,'logstore_database','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857237),(1035,0,'logstore_database','2014051200','2014051200','Plugin installed',NULL,'',0,1413857237),(1036,0,'logstore_legacy',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857237),(1037,0,'logstore_legacy','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857237),(1038,0,'logstore_legacy','2014051200','2014051200','Plugin installed',NULL,'',0,1413857238),(1039,0,'logstore_standard',NULL,'2014051200','Starting plugin installation',NULL,'',0,1413857238),(1040,0,'logstore_standard','2014051200','2014051200','Upgrade savepoint reached',NULL,'',0,1413857238),(1041,0,'logstore_standard','2014051200','2014051200','Plugin installed',NULL,'',0,1413857238),(1042,0,'auth_cliauth',NULL,'2014091501','Starting plugin installation',NULL,'',2,1413861458),(1043,0,'auth_cliauth','2014091501','2014091501','Upgrade savepoint reached',NULL,'',2,1413861459),(1044,0,'auth_cliauth','2014091501','2014091501','Plugin installed',NULL,'',2,1413861459),(1045,0,'auth_elisfilessso',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861459),(1046,0,'auth_elisfilessso','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861459),(1047,0,'auth_elisfilessso','2014030700','2014030700','Plugin installed',NULL,'',2,1413861460),(1048,0,'enrol_elis',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861460),(1049,0,'enrol_elis','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861460),(1050,0,'enrol_elis','2014030700','2014030700','Plugin installed',NULL,'',2,1413861461),(1051,0,'block_courserequest',NULL,'2014030701','Starting plugin installation',NULL,'',2,1413861461),(1052,0,'block_courserequest','2014030701','2014030701','Upgrade savepoint reached',NULL,'',2,1413861462),(1053,0,'block_courserequest','2014030701','2014030701','Plugin installed',NULL,'',2,1413861464),(1054,0,'block_elisadmin',NULL,'2014030701','Starting plugin installation',NULL,'',2,1413861465),(1055,0,'block_elisadmin','2014030701','2014030701','Upgrade savepoint reached',NULL,'',2,1413861465),(1056,0,'block_elisadmin','2014030701','2014030701','Plugin installed',NULL,'',2,1413861467),(1057,0,'block_enrolsurvey',NULL,'2014030701','Starting plugin installation',NULL,'',2,1413861467),(1058,0,'block_enrolsurvey','2014030701','2014030701','Upgrade savepoint reached',NULL,'',2,1413861468),(1059,0,'block_enrolsurvey','2014030701','2014030701','Plugin installed',NULL,'',2,1413861469),(1060,0,'block_repository',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861470),(1061,0,'block_repository','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861470),(1062,0,'block_repository','2014030700','2014030700','Plugin installed',NULL,'',2,1413861470),(1063,0,'repository_elisfiles',NULL,'2014030702','Starting plugin installation',NULL,'',2,1413861471),(1064,0,'repository_elisfiles','2014030702','2014030702','Upgrade savepoint reached',NULL,'',2,1413861471),(1065,0,'repository_elisfiles','2014030702','2014030702','Plugin installed',NULL,'',2,1413861477),(1066,0,'local_datahub',NULL,'2014030701','Starting plugin installation',NULL,'',2,1413861477),(1067,0,'local_datahub','2014030701','2014030701','Upgrade savepoint reached',NULL,'',2,1413861478),(1068,0,'local_datahub','2014030701','2014030701','Plugin installed',NULL,'',2,1413861485),(1069,0,'local_eliscore',NULL,'2014030701','Starting plugin installation',NULL,'',2,1413861485),(1070,0,'local_eliscore','2014030701','2014030701','Upgrade savepoint reached',NULL,'',2,1413861487),(1071,0,'local_eliscore','2014030701','2014030701','Plugin installed',NULL,'',2,1413861488),(1072,0,'local_elisprogram',NULL,'2014030701','Starting plugin installation',NULL,'',2,1413861488),(1073,0,'local_elisprogram','2014030701','2014030701','Upgrade savepoint reached',NULL,'',2,1413861494),(1074,0,'local_elisprogram','2014030701','2014030701','Plugin installed',NULL,'',2,1413861516),(1075,0,'local_elisreports',NULL,'2014030701','Starting plugin installation',NULL,'',2,1413861516),(1076,0,'local_elisreports','2014030701','2014030701','Upgrade savepoint reached',NULL,'',2,1413861517),(1077,0,'local_elisreports','2014030701','2014030701','Plugin installed',NULL,'',2,1413861518),(1078,0,'dhimport_header',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861518),(1079,0,'dhimport_header','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861518),(1080,0,'dhimport_header','2014030700','2014030700','Plugin installed',NULL,'',2,1413861518),(1081,0,'dhimport_multiple',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861518),(1082,0,'dhimport_multiple','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861519),(1083,0,'dhimport_multiple','2014030700','2014030700','Plugin installed',NULL,'',2,1413861519),(1084,0,'dhimport_sample',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861519),(1085,0,'dhimport_sample','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861519),(1086,0,'dhimport_sample','2014030700','2014030700','Plugin installed',NULL,'',2,1413861520),(1087,0,'dhimport_version1',NULL,'2014030701','Starting plugin installation',NULL,'',2,1413861520),(1088,0,'dhimport_version1','2014030701','2014030701','Upgrade savepoint reached',NULL,'',2,1413861520),(1089,0,'dhimport_version1','2014030701','2014030701','Plugin installed',NULL,'',2,1413861521),(1090,0,'dhimport_version1elis',NULL,'2014030701','Starting plugin installation',NULL,'',2,1413861521),(1091,0,'dhimport_version1elis','2014030701','2014030701','Upgrade savepoint reached',NULL,'',2,1413861521),(1092,0,'dhimport_version1elis','2014030701','2014030701','Plugin installed',NULL,'',2,1413861522),(1093,0,'dhexport_version1',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861522),(1094,0,'dhexport_version1','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861522),(1095,0,'dhexport_version1','2014030700','2014030700','Plugin installed',NULL,'',2,1413861522),(1096,0,'dhexport_version1elis',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861522),(1097,0,'dhexport_version1elis','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861523),(1098,0,'dhexport_version1elis','2014030700','2014030700','Plugin installed',NULL,'',2,1413861523),(1099,0,'dhfile_csv',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861523),(1100,0,'dhfile_csv','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861523),(1101,0,'dhfile_csv','2014030700','2014030700','Plugin installed',NULL,'',2,1413861524),(1102,0,'dhfile_log',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861524),(1103,0,'dhfile_log','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861524),(1104,0,'dhfile_log','2014030700','2014030700','Plugin installed',NULL,'',2,1413861525),(1105,0,'elisfields_manual',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861525),(1106,0,'elisfields_manual','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861526),(1107,0,'elisfields_manual','2014030700','2014030700','Plugin installed',NULL,'',2,1413861526),(1108,0,'elisfields_moodleprofile',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861526),(1109,0,'elisfields_moodleprofile','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861526),(1110,0,'elisfields_moodleprofile','2014030700','2014030700','Plugin installed',NULL,'',2,1413861527),(1111,0,'eliscore_etl',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861527),(1112,0,'eliscore_etl','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861527),(1113,0,'eliscore_etl','2014030700','2014030700','Plugin installed',NULL,'',2,1413861528),(1114,0,'usetenrol_manual',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861528),(1115,0,'usetenrol_manual','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861528),(1116,0,'usetenrol_manual','2014030700','2014030700','Plugin installed',NULL,'',2,1413861529),(1117,0,'usetenrol_moodleprofile',NULL,'2014030701','Starting plugin installation',NULL,'',2,1413861529),(1118,0,'usetenrol_moodleprofile','2014030701','2014030701','Upgrade savepoint reached',NULL,'',2,1413861529),(1119,0,'usetenrol_moodleprofile','2014030701','2014030701','Plugin installed',NULL,'',2,1413861530),(1120,0,'elisprogram_archive',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861530),(1121,0,'elisprogram_archive','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861531),(1122,0,'elisprogram_archive','2014030700','2014030700','Plugin installed',NULL,'',2,1413861532),(1123,0,'elisprogram_enrolrolesync',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861532),(1124,0,'elisprogram_enrolrolesync','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861532),(1125,0,'elisprogram_enrolrolesync','2014030700','2014030700','Plugin installed',NULL,'',2,1413861533),(1126,0,'elisprogram_preposttest',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861533),(1127,0,'elisprogram_preposttest','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861533),(1128,0,'elisprogram_preposttest','2014030700','2014030700','Plugin installed',NULL,'',2,1413861535),(1129,0,'elisprogram_usetclassify',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861535),(1130,0,'elisprogram_usetclassify','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861535),(1131,0,'elisprogram_usetclassify','2014030700','2014030700','Plugin installed',NULL,'',2,1413861537),(1132,0,'elisprogram_usetdisppriority',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861537),(1133,0,'elisprogram_usetdisppriority','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861538),(1134,0,'elisprogram_usetdisppriority','2014030700','2014030700','Plugin installed',NULL,'',2,1413861539),(1135,0,'elisprogram_usetgroups',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861539),(1136,0,'elisprogram_usetgroups','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861539),(1137,0,'elisprogram_usetgroups','2014030700','2014030700','Plugin installed',NULL,'',2,1413861542),(1138,0,'elisprogram_usetthemes',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861542),(1139,0,'elisprogram_usetthemes','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861542),(1140,0,'elisprogram_usetthemes','2014030700','2014030700','Plugin installed',NULL,'',2,1413861544),(1141,0,'rlreport_class_completion_gas_gauge',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861544),(1142,0,'rlreport_class_completion_gas_gauge','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861544),(1143,0,'rlreport_class_completion_gas_gauge','2014030700','2014030700','Plugin installed',NULL,'',2,1413861544),(1144,0,'rlreport_class_roster',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861544),(1145,0,'rlreport_class_roster','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861545),(1146,0,'rlreport_class_roster','2014030700','2014030700','Plugin installed',NULL,'',2,1413861545),(1147,0,'rlreport_course_completion_by_cluster',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861545),(1148,0,'rlreport_course_completion_by_cluster','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861545),(1149,0,'rlreport_course_completion_by_cluster','2014030700','2014030700','Plugin installed',NULL,'',2,1413861546),(1150,0,'rlreport_course_completion_gas_gauge',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861546),(1151,0,'rlreport_course_completion_gas_gauge','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861546),(1152,0,'rlreport_course_completion_gas_gauge','2014030700','2014030700','Plugin installed',NULL,'',2,1413861546),(1153,0,'rlreport_course_progress_summary',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861546),(1154,0,'rlreport_course_progress_summary','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861547),(1155,0,'rlreport_course_progress_summary','2014030700','2014030700','Plugin installed',NULL,'',2,1413861547),(1156,0,'rlreport_course_usage_summary',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861547),(1157,0,'rlreport_course_usage_summary','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861548),(1158,0,'rlreport_course_usage_summary','2014030700','2014030700','Plugin installed',NULL,'',2,1413861548),(1159,0,'rlreport_curricula',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861548),(1160,0,'rlreport_curricula','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861549),(1161,0,'rlreport_curricula','2014030700','2014030700','Plugin installed',NULL,'',2,1413861549),(1162,0,'rlreport_individual_course_progress',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861549),(1163,0,'rlreport_individual_course_progress','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861550),(1164,0,'rlreport_individual_course_progress','2014030700','2014030700','Plugin installed',NULL,'',2,1413861550),(1165,0,'rlreport_individual_user',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861550),(1166,0,'rlreport_individual_user','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861550),(1167,0,'rlreport_individual_user','2014030700','2014030700','Plugin installed',NULL,'',2,1413861550),(1168,0,'rlreport_nonstarter',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861551),(1169,0,'rlreport_nonstarter','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861551),(1170,0,'rlreport_nonstarter','2014030700','2014030700','Plugin installed',NULL,'',2,1413861551),(1171,0,'rlreport_registrants_by_course',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861551),(1172,0,'rlreport_registrants_by_course','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861552),(1173,0,'rlreport_registrants_by_course','2014030700','2014030700','Plugin installed',NULL,'',2,1413861552),(1174,0,'rlreport_registrants_by_student',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861552),(1175,0,'rlreport_registrants_by_student','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861553),(1176,0,'rlreport_registrants_by_student','2014030700','2014030700','Plugin installed',NULL,'',2,1413861553),(1177,0,'rlreport_sitewide_course_completion',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861553),(1178,0,'rlreport_sitewide_course_completion','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861553),(1179,0,'rlreport_sitewide_course_completion','2014030700','2014030700','Plugin installed',NULL,'',2,1413861554),(1180,0,'rlreport_sitewide_time_summary',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861554),(1181,0,'rlreport_sitewide_time_summary','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861554),(1182,0,'rlreport_sitewide_time_summary','2014030700','2014030700','Plugin installed',NULL,'',2,1413861555),(1183,0,'rlreport_sitewide_transcript',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861555),(1184,0,'rlreport_sitewide_transcript','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861555),(1185,0,'rlreport_sitewide_transcript','2014030700','2014030700','Plugin installed',NULL,'',2,1413861555),(1186,0,'rlreport_user_class_completion',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861555),(1187,0,'rlreport_user_class_completion','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861556),(1188,0,'rlreport_user_class_completion','2014030700','2014030700','Plugin installed',NULL,'',2,1413861556),(1189,0,'rlreport_user_class_completion_details',NULL,'2014030700','Starting plugin installation',NULL,'',2,1413861557),(1190,0,'rlreport_user_class_completion_details','2014030700','2014030700','Upgrade savepoint reached',NULL,'',2,1413861557),(1191,0,'rlreport_user_class_completion_details','2014030700','2014030700','Plugin installed',NULL,'',2,1413861557);

/*Table structure for table `mdl_url` */

DROP TABLE IF EXISTS `mdl_url`;

CREATE TABLE `mdl_url` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `externalurl` longtext COLLATE utf8_unicode_ci NOT NULL,
  `display` smallint(4) NOT NULL DEFAULT '0',
  `displayoptions` longtext COLLATE utf8_unicode_ci,
  `parameters` longtext COLLATE utf8_unicode_ci,
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_url_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='each record is one url resource';

/*Data for the table `mdl_url` */

/*Table structure for table `mdl_user` */

DROP TABLE IF EXISTS `mdl_user`;

CREATE TABLE `mdl_user` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `auth` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'manual',
  `confirmed` tinyint(1) NOT NULL DEFAULT '0',
  `policyagreed` tinyint(1) NOT NULL DEFAULT '0',
  `deleted` tinyint(1) NOT NULL DEFAULT '0',
  `suspended` tinyint(1) NOT NULL DEFAULT '0',
  `mnethostid` bigint(10) NOT NULL DEFAULT '0',
  `username` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `password` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `idnumber` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `firstname` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `lastname` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `email` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `emailstop` tinyint(1) NOT NULL DEFAULT '0',
  `icq` varchar(15) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `skype` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `yahoo` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `aim` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `msn` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `phone1` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `phone2` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `institution` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `department` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `address` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `city` varchar(120) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `country` varchar(2) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `lang` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'en',
  `calendartype` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'gregorian',
  `theme` varchar(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timezone` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '99',
  `firstaccess` bigint(10) NOT NULL DEFAULT '0',
  `lastaccess` bigint(10) NOT NULL DEFAULT '0',
  `lastlogin` bigint(10) NOT NULL DEFAULT '0',
  `currentlogin` bigint(10) NOT NULL DEFAULT '0',
  `lastip` varchar(45) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `secret` varchar(15) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `picture` bigint(10) NOT NULL DEFAULT '0',
  `url` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` tinyint(2) NOT NULL DEFAULT '1',
  `mailformat` tinyint(1) NOT NULL DEFAULT '1',
  `maildigest` tinyint(1) NOT NULL DEFAULT '0',
  `maildisplay` tinyint(2) NOT NULL DEFAULT '2',
  `autosubscribe` tinyint(1) NOT NULL DEFAULT '1',
  `trackforums` tinyint(1) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `trustbitmask` bigint(10) NOT NULL DEFAULT '0',
  `imagealt` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `lastnamephonetic` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `firstnamephonetic` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `middlename` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `alternatename` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_user_mneuse_uix` (`mnethostid`,`username`),
  KEY `mdl_user_del_ix` (`deleted`),
  KEY `mdl_user_con_ix` (`confirmed`),
  KEY `mdl_user_fir_ix` (`firstname`),
  KEY `mdl_user_las_ix` (`lastname`),
  KEY `mdl_user_cit_ix` (`city`),
  KEY `mdl_user_cou_ix` (`country`),
  KEY `mdl_user_las2_ix` (`lastaccess`),
  KEY `mdl_user_ema_ix` (`email`),
  KEY `mdl_user_aut_ix` (`auth`),
  KEY `mdl_user_idn_ix` (`idnumber`),
  KEY `mdl_user_fir2_ix` (`firstnamephonetic`),
  KEY `mdl_user_las3_ix` (`lastnamephonetic`),
  KEY `mdl_user_mid_ix` (`middlename`),
  KEY `mdl_user_alt_ix` (`alternatename`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='One record for each person';

/*Data for the table `mdl_user` */

insert  into `mdl_user`(`id`,`auth`,`confirmed`,`policyagreed`,`deleted`,`suspended`,`mnethostid`,`username`,`password`,`idnumber`,`firstname`,`lastname`,`email`,`emailstop`,`icq`,`skype`,`yahoo`,`aim`,`msn`,`phone1`,`phone2`,`institution`,`department`,`address`,`city`,`country`,`lang`,`calendartype`,`theme`,`timezone`,`firstaccess`,`lastaccess`,`lastlogin`,`currentlogin`,`lastip`,`secret`,`picture`,`url`,`description`,`descriptionformat`,`mailformat`,`maildigest`,`maildisplay`,`autosubscribe`,`trackforums`,`timecreated`,`timemodified`,`trustbitmask`,`imagealt`,`lastnamephonetic`,`firstnamephonetic`,`middlename`,`alternatename`) values (1,'manual',1,0,0,0,1,'guest','$2y$10$cJl3Y/EBLgQFmiF2EGDC8.xWWcrjpIECluSUsRfMsg4pH76u4aWki','','访客用户',' ','root@localhost',0,'','','','','','','','','','','','','zh_cn','gregorian','','99',0,0,0,0,'','',0,'','该用户是个特殊用户，允许以只读方式参观一些课程。',1,1,0,2,1,0,0,1413856723,0,NULL,NULL,NULL,NULL,NULL),(2,'manual',1,0,0,0,1,'admin','$2y$10$jLeyKZWWU2Yv7D7pE7gvw.YbJOq2PR0X5ASYmGEe7gx3zN0007e1m','admin','管理','用户','xiaowuq@sunnet.us',0,'','','','','','','','','','','','','zh_cn','gregorian','','99',1413857253,1414028442,1413886157,1414028442,'127.0.0.1','',0,'','',1,1,0,1,1,0,0,1413873059,0,NULL,'','','',''),(4,'cliauth',1,0,0,0,1,'clisunnet@gmail.com','not cached','clisunnet@gmail.com','John','Li','xiaowuq@sunnet.us',0,'','','','','','','','','','','','','zh_cn','gregorian','','99',1413862564,1413975417,1413947609,1413975353,'127.0.0.1','',0,'',NULL,1,1,0,2,1,0,1413862564,1413862564,0,NULL,NULL,NULL,NULL,NULL);

/*Table structure for table `mdl_user_devices` */

DROP TABLE IF EXISTS `mdl_user_devices`;

CREATE TABLE `mdl_user_devices` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `appid` varchar(128) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `name` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `model` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `platform` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `version` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `pushid` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `uuid` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timecreated` bigint(10) NOT NULL,
  `timemodified` bigint(10) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_userdevi_pususe_uix` (`pushid`,`userid`),
  KEY `mdl_userdevi_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table stores user''s mobile devices information in order';

/*Data for the table `mdl_user_devices` */

/*Table structure for table `mdl_user_enrolments` */

DROP TABLE IF EXISTS `mdl_user_enrolments`;

CREATE TABLE `mdl_user_enrolments` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `status` bigint(10) NOT NULL DEFAULT '0',
  `enrolid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `timestart` bigint(10) NOT NULL DEFAULT '0',
  `timeend` bigint(10) NOT NULL DEFAULT '2147483647',
  `modifierid` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_userenro_enruse_uix` (`enrolid`,`userid`),
  KEY `mdl_userenro_enr_ix` (`enrolid`),
  KEY `mdl_userenro_use_ix` (`userid`),
  KEY `mdl_userenro_mod_ix` (`modifierid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Users participating in courses (aka enrolled users) - everyb';

/*Data for the table `mdl_user_enrolments` */

/*Table structure for table `mdl_user_info_category` */

DROP TABLE IF EXISTS `mdl_user_info_category`;

CREATE TABLE `mdl_user_info_category` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Customisable fields categories';

/*Data for the table `mdl_user_info_category` */

/*Table structure for table `mdl_user_info_data` */

DROP TABLE IF EXISTS `mdl_user_info_data`;

CREATE TABLE `mdl_user_info_data` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `fieldid` bigint(10) NOT NULL DEFAULT '0',
  `data` longtext COLLATE utf8_unicode_ci NOT NULL,
  `dataformat` tinyint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_userinfodata_usefie_uix` (`userid`,`fieldid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Data for the customisable user fields';

/*Data for the table `mdl_user_info_data` */

/*Table structure for table `mdl_user_info_field` */

DROP TABLE IF EXISTS `mdl_user_info_field`;

CREATE TABLE `mdl_user_info_field` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `shortname` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'shortname',
  `name` longtext COLLATE utf8_unicode_ci NOT NULL,
  `datatype` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` tinyint(2) NOT NULL DEFAULT '0',
  `categoryid` bigint(10) NOT NULL DEFAULT '0',
  `sortorder` bigint(10) NOT NULL DEFAULT '0',
  `required` tinyint(2) NOT NULL DEFAULT '0',
  `locked` tinyint(2) NOT NULL DEFAULT '0',
  `visible` smallint(4) NOT NULL DEFAULT '0',
  `forceunique` tinyint(2) NOT NULL DEFAULT '0',
  `signup` tinyint(2) NOT NULL DEFAULT '0',
  `defaultdata` longtext COLLATE utf8_unicode_ci,
  `defaultdataformat` tinyint(2) NOT NULL DEFAULT '0',
  `param1` longtext COLLATE utf8_unicode_ci,
  `param2` longtext COLLATE utf8_unicode_ci,
  `param3` longtext COLLATE utf8_unicode_ci,
  `param4` longtext COLLATE utf8_unicode_ci,
  `param5` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Customisable user profile fields';

/*Data for the table `mdl_user_info_field` */

/*Table structure for table `mdl_user_lastaccess` */

DROP TABLE IF EXISTS `mdl_user_lastaccess`;

CREATE TABLE `mdl_user_lastaccess` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `courseid` bigint(10) NOT NULL DEFAULT '0',
  `timeaccess` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_userlast_usecou_uix` (`userid`,`courseid`),
  KEY `mdl_userlast_use_ix` (`userid`),
  KEY `mdl_userlast_cou_ix` (`courseid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='To keep track of course page access times, used in online pa';

/*Data for the table `mdl_user_lastaccess` */

/*Table structure for table `mdl_user_password_resets` */

DROP TABLE IF EXISTS `mdl_user_password_resets`;

CREATE TABLE `mdl_user_password_resets` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL,
  `timerequested` bigint(10) NOT NULL,
  `timererequested` bigint(10) NOT NULL DEFAULT '0',
  `token` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  KEY `mdl_userpassrese_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='table tracking password reset confirmation tokens';

/*Data for the table `mdl_user_password_resets` */

/*Table structure for table `mdl_user_preferences` */

DROP TABLE IF EXISTS `mdl_user_preferences`;

CREATE TABLE `mdl_user_preferences` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` varchar(1333) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_userpref_usenam_uix` (`userid`,`name`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Allows modules to store arbitrary user preferences';

/*Data for the table `mdl_user_preferences` */

insert  into `mdl_user_preferences`(`id`,`userid`,`name`,`value`) values (1,2,'auth_manual_passwordupdatetime','1413858055'),(2,2,'htmleditor',''),(3,2,'email_bounce_count','1'),(4,2,'email_send_count','1');

/*Table structure for table `mdl_user_private_key` */

DROP TABLE IF EXISTS `mdl_user_private_key`;

CREATE TABLE `mdl_user_private_key` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `script` varchar(128) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `value` varchar(128) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `userid` bigint(10) NOT NULL,
  `instance` bigint(10) DEFAULT NULL,
  `iprestriction` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `validuntil` bigint(10) DEFAULT NULL,
  `timecreated` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_userprivkey_scrval_ix` (`script`,`value`),
  KEY `mdl_userprivkey_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='access keys used in cookieless scripts - rss, etc.';

/*Data for the table `mdl_user_private_key` */

/*Table structure for table `mdl_webdav_locks` */

DROP TABLE IF EXISTS `mdl_webdav_locks`;

CREATE TABLE `mdl_webdav_locks` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `token` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `path` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `expiry` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `recursive` tinyint(1) NOT NULL DEFAULT '0',
  `exclusivelock` tinyint(1) NOT NULL DEFAULT '0',
  `created` bigint(10) NOT NULL DEFAULT '0',
  `modified` bigint(10) NOT NULL DEFAULT '0',
  `owner` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_webdlock_tok_uix` (`token`),
  KEY `mdl_webdlock_pat_ix` (`path`),
  KEY `mdl_webdlock_exp_ix` (`expiry`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Resource locks for WebDAV users';

/*Data for the table `mdl_webdav_locks` */

/*Table structure for table `mdl_wiki` */

DROP TABLE IF EXISTS `mdl_wiki`;

CREATE TABLE `mdl_wiki` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'Wiki',
  `intro` longtext COLLATE utf8_unicode_ci,
  `introformat` smallint(4) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `firstpagetitle` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'First Page',
  `wikimode` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'collaborative',
  `defaultformat` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'creole',
  `forceformat` tinyint(1) NOT NULL DEFAULT '1',
  `editbegin` bigint(10) NOT NULL DEFAULT '0',
  `editend` bigint(10) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_wiki_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores Wiki activity configuration';

/*Data for the table `mdl_wiki` */

/*Table structure for table `mdl_wiki_links` */

DROP TABLE IF EXISTS `mdl_wiki_links`;

CREATE TABLE `mdl_wiki_links` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `subwikiid` bigint(10) NOT NULL DEFAULT '0',
  `frompageid` bigint(10) NOT NULL DEFAULT '0',
  `topageid` bigint(10) NOT NULL DEFAULT '0',
  `tomissingpage` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_wikilink_fro_ix` (`frompageid`),
  KEY `mdl_wikilink_sub_ix` (`subwikiid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Page wiki links';

/*Data for the table `mdl_wiki_links` */

/*Table structure for table `mdl_wiki_locks` */

DROP TABLE IF EXISTS `mdl_wiki_locks`;

CREATE TABLE `mdl_wiki_locks` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `pageid` bigint(10) NOT NULL DEFAULT '0',
  `sectionname` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `lockedat` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Manages page locks';

/*Data for the table `mdl_wiki_locks` */

/*Table structure for table `mdl_wiki_pages` */

DROP TABLE IF EXISTS `mdl_wiki_pages`;

CREATE TABLE `mdl_wiki_pages` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `subwikiid` bigint(10) NOT NULL DEFAULT '0',
  `title` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'title',
  `cachedcontent` longtext COLLATE utf8_unicode_ci NOT NULL,
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `timerendered` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `pageviews` bigint(10) NOT NULL DEFAULT '0',
  `readonly` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_wikipage_subtituse_uix` (`subwikiid`,`title`,`userid`),
  KEY `mdl_wikipage_sub_ix` (`subwikiid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores wiki pages';

/*Data for the table `mdl_wiki_pages` */

/*Table structure for table `mdl_wiki_subwikis` */

DROP TABLE IF EXISTS `mdl_wiki_subwikis`;

CREATE TABLE `mdl_wiki_subwikis` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `wikiid` bigint(10) NOT NULL DEFAULT '0',
  `groupid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_wikisubw_wikgrouse_uix` (`wikiid`,`groupid`,`userid`),
  KEY `mdl_wikisubw_wik_ix` (`wikiid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores subwiki instances';

/*Data for the table `mdl_wiki_subwikis` */

/*Table structure for table `mdl_wiki_synonyms` */

DROP TABLE IF EXISTS `mdl_wiki_synonyms`;

CREATE TABLE `mdl_wiki_synonyms` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `subwikiid` bigint(10) NOT NULL DEFAULT '0',
  `pageid` bigint(10) NOT NULL DEFAULT '0',
  `pagesynonym` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'Pagesynonym',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_wikisyno_pagpag_uix` (`pageid`,`pagesynonym`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores wiki pages synonyms';

/*Data for the table `mdl_wiki_synonyms` */

/*Table structure for table `mdl_wiki_versions` */

DROP TABLE IF EXISTS `mdl_wiki_versions`;

CREATE TABLE `mdl_wiki_versions` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `pageid` bigint(10) NOT NULL DEFAULT '0',
  `content` longtext COLLATE utf8_unicode_ci NOT NULL,
  `contentformat` varchar(20) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'creole',
  `version` mediumint(5) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_wikivers_pag_ix` (`pageid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores wiki page history';

/*Data for the table `mdl_wiki_versions` */

/*Table structure for table `mdl_workshop` */

DROP TABLE IF EXISTS `mdl_workshop`;

CREATE TABLE `mdl_workshop` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `intro` longtext COLLATE utf8_unicode_ci,
  `introformat` smallint(3) NOT NULL DEFAULT '0',
  `instructauthors` longtext COLLATE utf8_unicode_ci,
  `instructauthorsformat` smallint(3) NOT NULL DEFAULT '0',
  `instructreviewers` longtext COLLATE utf8_unicode_ci,
  `instructreviewersformat` smallint(3) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL,
  `phase` smallint(3) DEFAULT '0',
  `useexamples` tinyint(2) DEFAULT '0',
  `usepeerassessment` tinyint(2) DEFAULT '0',
  `useselfassessment` tinyint(2) DEFAULT '0',
  `grade` decimal(10,5) DEFAULT '80.00000',
  `gradinggrade` decimal(10,5) DEFAULT '20.00000',
  `strategy` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `evaluation` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `gradedecimals` smallint(3) DEFAULT '0',
  `nattachments` smallint(3) DEFAULT '0',
  `latesubmissions` tinyint(2) DEFAULT '0',
  `maxbytes` bigint(10) DEFAULT '100000',
  `examplesmode` smallint(3) DEFAULT '0',
  `submissionstart` bigint(10) DEFAULT '0',
  `submissionend` bigint(10) DEFAULT '0',
  `assessmentstart` bigint(10) DEFAULT '0',
  `assessmentend` bigint(10) DEFAULT '0',
  `phaseswitchassessment` tinyint(2) NOT NULL DEFAULT '0',
  `conclusion` longtext COLLATE utf8_unicode_ci,
  `conclusionformat` smallint(3) NOT NULL DEFAULT '1',
  `overallfeedbackmode` smallint(3) DEFAULT '1',
  `overallfeedbackfiles` smallint(3) DEFAULT '0',
  `overallfeedbackmaxbytes` bigint(10) DEFAULT '100000',
  PRIMARY KEY (`id`),
  KEY `mdl_work_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This table keeps information about the module instances and ';

/*Data for the table `mdl_workshop` */

/*Table structure for table `mdl_workshop_aggregations` */

DROP TABLE IF EXISTS `mdl_workshop_aggregations`;

CREATE TABLE `mdl_workshop_aggregations` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL,
  `userid` bigint(10) NOT NULL,
  `gradinggrade` decimal(10,5) DEFAULT NULL,
  `timegraded` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_workaggr_woruse_uix` (`workshopid`,`userid`),
  KEY `mdl_workaggr_wor_ix` (`workshopid`),
  KEY `mdl_workaggr_use_ix` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Aggregated grades for assessment are stored here. The aggreg';

/*Data for the table `mdl_workshop_aggregations` */

/*Table structure for table `mdl_workshop_assessments` */

DROP TABLE IF EXISTS `mdl_workshop_assessments`;

CREATE TABLE `mdl_workshop_assessments` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `submissionid` bigint(10) NOT NULL,
  `reviewerid` bigint(10) NOT NULL,
  `weight` bigint(10) NOT NULL DEFAULT '1',
  `timecreated` bigint(10) DEFAULT '0',
  `timemodified` bigint(10) DEFAULT '0',
  `grade` decimal(10,5) DEFAULT NULL,
  `gradinggrade` decimal(10,5) DEFAULT NULL,
  `gradinggradeover` decimal(10,5) DEFAULT NULL,
  `gradinggradeoverby` bigint(10) DEFAULT NULL,
  `feedbackauthor` longtext COLLATE utf8_unicode_ci,
  `feedbackauthorformat` smallint(3) DEFAULT '0',
  `feedbackauthorattachment` smallint(3) DEFAULT '0',
  `feedbackreviewer` longtext COLLATE utf8_unicode_ci,
  `feedbackreviewerformat` smallint(3) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_workasse_sub_ix` (`submissionid`),
  KEY `mdl_workasse_gra_ix` (`gradinggradeoverby`),
  KEY `mdl_workasse_rev_ix` (`reviewerid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Info about the made assessment and automatically calculated ';

/*Data for the table `mdl_workshop_assessments` */

/*Table structure for table `mdl_workshop_assessments_old` */

DROP TABLE IF EXISTS `mdl_workshop_assessments_old`;

CREATE TABLE `mdl_workshop_assessments_old` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL DEFAULT '0',
  `submissionid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `timegraded` bigint(10) NOT NULL DEFAULT '0',
  `timeagreed` bigint(10) NOT NULL DEFAULT '0',
  `grade` double NOT NULL DEFAULT '0',
  `gradinggrade` smallint(3) NOT NULL DEFAULT '0',
  `teachergraded` smallint(3) NOT NULL DEFAULT '0',
  `mailed` smallint(3) NOT NULL DEFAULT '0',
  `resubmission` smallint(3) NOT NULL DEFAULT '0',
  `donotuse` smallint(3) NOT NULL DEFAULT '0',
  `generalcomment` longtext COLLATE utf8_unicode_ci,
  `teachercomment` longtext COLLATE utf8_unicode_ci,
  `newplugin` varchar(28) COLLATE utf8_unicode_ci DEFAULT NULL,
  `newid` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_workasseold_use_ix` (`userid`),
  KEY `mdl_workasseold_mai_ix` (`mailed`),
  KEY `mdl_workasseold_wor_ix` (`workshopid`),
  KEY `mdl_workasseold_sub_ix` (`submissionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Legacy workshop_assessments table to be dropped later in Moo';

/*Data for the table `mdl_workshop_assessments_old` */

/*Table structure for table `mdl_workshop_comments_old` */

DROP TABLE IF EXISTS `mdl_workshop_comments_old`;

CREATE TABLE `mdl_workshop_comments_old` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL DEFAULT '0',
  `assessmentid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `mailed` tinyint(2) NOT NULL DEFAULT '0',
  `comments` longtext COLLATE utf8_unicode_ci NOT NULL,
  `newplugin` varchar(28) COLLATE utf8_unicode_ci DEFAULT NULL,
  `newid` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_workcommold_use_ix` (`userid`),
  KEY `mdl_workcommold_mai_ix` (`mailed`),
  KEY `mdl_workcommold_wor_ix` (`workshopid`),
  KEY `mdl_workcommold_ass_ix` (`assessmentid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Legacy workshop_comments table to be dropped later in Moodle';

/*Data for the table `mdl_workshop_comments_old` */

/*Table structure for table `mdl_workshop_elements_old` */

DROP TABLE IF EXISTS `mdl_workshop_elements_old`;

CREATE TABLE `mdl_workshop_elements_old` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL DEFAULT '0',
  `elementno` smallint(3) NOT NULL DEFAULT '0',
  `description` longtext COLLATE utf8_unicode_ci NOT NULL,
  `scale` smallint(3) NOT NULL DEFAULT '0',
  `maxscore` smallint(3) NOT NULL DEFAULT '1',
  `weight` smallint(3) NOT NULL DEFAULT '11',
  `stddev` double NOT NULL DEFAULT '0',
  `totalassessments` bigint(10) NOT NULL DEFAULT '0',
  `newplugin` varchar(28) COLLATE utf8_unicode_ci DEFAULT NULL,
  `newid` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_workelemold_wor_ix` (`workshopid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Legacy workshop_elements table to be dropped later in Moodle';

/*Data for the table `mdl_workshop_elements_old` */

/*Table structure for table `mdl_workshop_grades` */

DROP TABLE IF EXISTS `mdl_workshop_grades`;

CREATE TABLE `mdl_workshop_grades` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `assessmentid` bigint(10) NOT NULL,
  `strategy` varchar(30) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `dimensionid` bigint(10) NOT NULL,
  `grade` decimal(10,5) NOT NULL,
  `peercomment` longtext COLLATE utf8_unicode_ci,
  `peercommentformat` smallint(3) DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_workgrad_assstrdim_uix` (`assessmentid`,`strategy`,`dimensionid`),
  KEY `mdl_workgrad_ass_ix` (`assessmentid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='How the reviewers filled-up the grading forms, given grades ';

/*Data for the table `mdl_workshop_grades` */

/*Table structure for table `mdl_workshop_grades_old` */

DROP TABLE IF EXISTS `mdl_workshop_grades_old`;

CREATE TABLE `mdl_workshop_grades_old` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL DEFAULT '0',
  `assessmentid` bigint(10) NOT NULL DEFAULT '0',
  `elementno` bigint(10) NOT NULL DEFAULT '0',
  `feedback` longtext COLLATE utf8_unicode_ci NOT NULL,
  `grade` smallint(3) NOT NULL DEFAULT '0',
  `newplugin` varchar(28) COLLATE utf8_unicode_ci DEFAULT NULL,
  `newid` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_workgradold_wor_ix` (`workshopid`),
  KEY `mdl_workgradold_ass_ix` (`assessmentid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Legacy workshop_grades table to be dropped later in Moodle 2';

/*Data for the table `mdl_workshop_grades_old` */

/*Table structure for table `mdl_workshop_old` */

DROP TABLE IF EXISTS `mdl_workshop_old`;

CREATE TABLE `mdl_workshop_old` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `course` bigint(10) NOT NULL DEFAULT '0',
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `description` longtext COLLATE utf8_unicode_ci NOT NULL,
  `wtype` smallint(3) NOT NULL DEFAULT '0',
  `nelements` smallint(3) NOT NULL DEFAULT '1',
  `nattachments` smallint(3) NOT NULL DEFAULT '0',
  `phase` tinyint(2) NOT NULL DEFAULT '0',
  `format` tinyint(2) NOT NULL DEFAULT '0',
  `gradingstrategy` tinyint(2) NOT NULL DEFAULT '1',
  `resubmit` tinyint(2) NOT NULL DEFAULT '0',
  `agreeassessments` tinyint(2) NOT NULL DEFAULT '0',
  `hidegrades` tinyint(2) NOT NULL DEFAULT '0',
  `anonymous` tinyint(2) NOT NULL DEFAULT '0',
  `includeself` tinyint(2) NOT NULL DEFAULT '0',
  `maxbytes` bigint(10) NOT NULL DEFAULT '100000',
  `submissionstart` bigint(10) NOT NULL DEFAULT '0',
  `assessmentstart` bigint(10) NOT NULL DEFAULT '0',
  `submissionend` bigint(10) NOT NULL DEFAULT '0',
  `assessmentend` bigint(10) NOT NULL DEFAULT '0',
  `releasegrades` bigint(10) NOT NULL DEFAULT '0',
  `grade` smallint(3) NOT NULL DEFAULT '0',
  `gradinggrade` smallint(3) NOT NULL DEFAULT '0',
  `ntassessments` smallint(3) NOT NULL DEFAULT '0',
  `assessmentcomps` smallint(3) NOT NULL DEFAULT '2',
  `nsassessments` smallint(3) NOT NULL DEFAULT '0',
  `overallocation` smallint(3) NOT NULL DEFAULT '0',
  `timemodified` bigint(10) NOT NULL DEFAULT '0',
  `teacherweight` smallint(3) NOT NULL DEFAULT '1',
  `showleaguetable` smallint(3) NOT NULL DEFAULT '0',
  `usepassword` smallint(3) NOT NULL DEFAULT '0',
  `password` varchar(32) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `newplugin` varchar(28) COLLATE utf8_unicode_ci DEFAULT NULL,
  `newid` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_workold_cou_ix` (`course`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Legacy workshop table to be dropped later in Moodle 2.x';

/*Data for the table `mdl_workshop_old` */

/*Table structure for table `mdl_workshop_rubrics_old` */

DROP TABLE IF EXISTS `mdl_workshop_rubrics_old`;

CREATE TABLE `mdl_workshop_rubrics_old` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL DEFAULT '0',
  `elementno` bigint(10) NOT NULL DEFAULT '0',
  `rubricno` smallint(3) NOT NULL DEFAULT '0',
  `description` longtext COLLATE utf8_unicode_ci NOT NULL,
  `newplugin` varchar(28) COLLATE utf8_unicode_ci DEFAULT NULL,
  `newid` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_workrubrold_wor_ix` (`workshopid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Legacy workshop_rubrics table to be dropped later in Moodle ';

/*Data for the table `mdl_workshop_rubrics_old` */

/*Table structure for table `mdl_workshop_stockcomments_old` */

DROP TABLE IF EXISTS `mdl_workshop_stockcomments_old`;

CREATE TABLE `mdl_workshop_stockcomments_old` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL DEFAULT '0',
  `elementno` bigint(10) NOT NULL DEFAULT '0',
  `comments` longtext COLLATE utf8_unicode_ci NOT NULL,
  `newplugin` varchar(28) COLLATE utf8_unicode_ci DEFAULT NULL,
  `newid` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_workstocold_wor_ix` (`workshopid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Legacy workshop_stockcomments table to be dropped later in M';

/*Data for the table `mdl_workshop_stockcomments_old` */

/*Table structure for table `mdl_workshop_submissions` */

DROP TABLE IF EXISTS `mdl_workshop_submissions`;

CREATE TABLE `mdl_workshop_submissions` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL,
  `example` tinyint(2) DEFAULT '0',
  `authorid` bigint(10) NOT NULL,
  `timecreated` bigint(10) NOT NULL,
  `timemodified` bigint(10) NOT NULL,
  `title` varchar(255) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `content` longtext COLLATE utf8_unicode_ci,
  `contentformat` smallint(3) NOT NULL DEFAULT '0',
  `contenttrust` smallint(3) NOT NULL DEFAULT '0',
  `attachment` tinyint(2) DEFAULT '0',
  `grade` decimal(10,5) DEFAULT NULL,
  `gradeover` decimal(10,5) DEFAULT NULL,
  `gradeoverby` bigint(10) DEFAULT NULL,
  `feedbackauthor` longtext COLLATE utf8_unicode_ci,
  `feedbackauthorformat` smallint(3) DEFAULT '0',
  `timegraded` bigint(10) DEFAULT NULL,
  `published` tinyint(2) DEFAULT '0',
  `late` tinyint(2) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_worksubm_wor_ix` (`workshopid`),
  KEY `mdl_worksubm_gra_ix` (`gradeoverby`),
  KEY `mdl_worksubm_aut_ix` (`authorid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Info about the submission and the aggregation of the grade f';

/*Data for the table `mdl_workshop_submissions` */

/*Table structure for table `mdl_workshop_submissions_old` */

DROP TABLE IF EXISTS `mdl_workshop_submissions_old`;

CREATE TABLE `mdl_workshop_submissions_old` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL DEFAULT '0',
  `userid` bigint(10) NOT NULL DEFAULT '0',
  `title` varchar(100) COLLATE utf8_unicode_ci NOT NULL DEFAULT '',
  `timecreated` bigint(10) NOT NULL DEFAULT '0',
  `mailed` tinyint(2) NOT NULL DEFAULT '0',
  `description` longtext COLLATE utf8_unicode_ci NOT NULL,
  `gradinggrade` smallint(3) NOT NULL DEFAULT '0',
  `finalgrade` smallint(3) NOT NULL DEFAULT '0',
  `late` smallint(3) NOT NULL DEFAULT '0',
  `nassessments` bigint(10) NOT NULL DEFAULT '0',
  `newplugin` varchar(28) COLLATE utf8_unicode_ci DEFAULT NULL,
  `newid` bigint(10) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mdl_worksubmold_use_ix` (`userid`),
  KEY `mdl_worksubmold_mai_ix` (`mailed`),
  KEY `mdl_worksubmold_wor_ix` (`workshopid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Legacy workshop_submissions table to be dropped later in Moo';

/*Data for the table `mdl_workshop_submissions_old` */

/*Table structure for table `mdl_workshopallocation_scheduled` */

DROP TABLE IF EXISTS `mdl_workshopallocation_scheduled`;

CREATE TABLE `mdl_workshopallocation_scheduled` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL,
  `enabled` tinyint(2) NOT NULL DEFAULT '0',
  `submissionend` bigint(10) NOT NULL,
  `timeallocated` bigint(10) DEFAULT NULL,
  `settings` longtext COLLATE utf8_unicode_ci,
  `resultstatus` bigint(10) DEFAULT NULL,
  `resultmessage` varchar(1333) COLLATE utf8_unicode_ci DEFAULT NULL,
  `resultlog` longtext COLLATE utf8_unicode_ci,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_worksche_wor_uix` (`workshopid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Stores the allocation settings for the scheduled allocator';

/*Data for the table `mdl_workshopallocation_scheduled` */

/*Table structure for table `mdl_workshopeval_best_settings` */

DROP TABLE IF EXISTS `mdl_workshopeval_best_settings`;

CREATE TABLE `mdl_workshopeval_best_settings` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL,
  `comparison` smallint(3) DEFAULT '5',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_workbestsett_wor_uix` (`workshopid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Settings for the grading evaluation subplugin Comparison wit';

/*Data for the table `mdl_workshopeval_best_settings` */

/*Table structure for table `mdl_workshopform_accumulative` */

DROP TABLE IF EXISTS `mdl_workshopform_accumulative`;

CREATE TABLE `mdl_workshopform_accumulative` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL,
  `sort` bigint(10) DEFAULT '0',
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` smallint(3) DEFAULT '0',
  `grade` bigint(10) NOT NULL,
  `weight` mediumint(5) DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `mdl_workaccu_wor_ix` (`workshopid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='The assessment dimensions definitions of Accumulative gradin';

/*Data for the table `mdl_workshopform_accumulative` */

/*Table structure for table `mdl_workshopform_comments` */

DROP TABLE IF EXISTS `mdl_workshopform_comments`;

CREATE TABLE `mdl_workshopform_comments` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL,
  `sort` bigint(10) DEFAULT '0',
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` smallint(3) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_workcomm_wor_ix` (`workshopid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='The assessment dimensions definitions of Comments strategy f';

/*Data for the table `mdl_workshopform_comments` */

/*Table structure for table `mdl_workshopform_numerrors` */

DROP TABLE IF EXISTS `mdl_workshopform_numerrors`;

CREATE TABLE `mdl_workshopform_numerrors` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL,
  `sort` bigint(10) DEFAULT '0',
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` smallint(3) DEFAULT '0',
  `descriptiontrust` bigint(10) DEFAULT NULL,
  `grade0` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `grade1` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `weight` mediumint(5) DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `mdl_worknume_wor_ix` (`workshopid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='The assessment dimensions definitions of Number of errors gr';

/*Data for the table `mdl_workshopform_numerrors` */

/*Table structure for table `mdl_workshopform_numerrors_map` */

DROP TABLE IF EXISTS `mdl_workshopform_numerrors_map`;

CREATE TABLE `mdl_workshopform_numerrors_map` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL,
  `nonegative` bigint(10) NOT NULL,
  `grade` decimal(10,5) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_worknumemap_wornon_uix` (`workshopid`,`nonegative`),
  KEY `mdl_worknumemap_wor_ix` (`workshopid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='This maps the number of errors to a percentual grade for sub';

/*Data for the table `mdl_workshopform_numerrors_map` */

/*Table structure for table `mdl_workshopform_rubric` */

DROP TABLE IF EXISTS `mdl_workshopform_rubric`;

CREATE TABLE `mdl_workshopform_rubric` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL,
  `sort` bigint(10) DEFAULT '0',
  `description` longtext COLLATE utf8_unicode_ci,
  `descriptionformat` smallint(3) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_workrubr_wor_ix` (`workshopid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='The assessment dimensions definitions of Rubric grading stra';

/*Data for the table `mdl_workshopform_rubric` */

/*Table structure for table `mdl_workshopform_rubric_config` */

DROP TABLE IF EXISTS `mdl_workshopform_rubric_config`;

CREATE TABLE `mdl_workshopform_rubric_config` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `workshopid` bigint(10) NOT NULL,
  `layout` varchar(30) COLLATE utf8_unicode_ci DEFAULT 'list',
  PRIMARY KEY (`id`),
  UNIQUE KEY `mdl_workrubrconf_wor_uix` (`workshopid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='Configuration table for the Rubric grading strategy';

/*Data for the table `mdl_workshopform_rubric_config` */

/*Table structure for table `mdl_workshopform_rubric_levels` */

DROP TABLE IF EXISTS `mdl_workshopform_rubric_levels`;

CREATE TABLE `mdl_workshopform_rubric_levels` (
  `id` bigint(10) NOT NULL AUTO_INCREMENT,
  `dimensionid` bigint(10) NOT NULL,
  `grade` decimal(10,5) NOT NULL,
  `definition` longtext COLLATE utf8_unicode_ci,
  `definitionformat` smallint(3) DEFAULT '0',
  PRIMARY KEY (`id`),
  KEY `mdl_workrubrleve_dim_ix` (`dimensionid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci COMMENT='The definition of rubric rating scales';

/*Data for the table `mdl_workshopform_rubric_levels` */

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
