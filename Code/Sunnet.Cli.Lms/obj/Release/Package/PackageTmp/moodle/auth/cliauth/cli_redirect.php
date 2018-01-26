<?php

/*
 * Get google code and call the normal login page
 * Needed to add the parameter authprovider in order to identify the authentication provider
 */      

require('../../config.php');
$clirole = optional_param('clirole', '', PARAM_TEXT); //Google can return an error
$idnumber = optional_param('idnumber', '', PARAM_TEXT);
$useremail = optional_param('useremail', '', PARAM_TEXT);
$firstname = optional_param('firstname', '', PARAM_TEXT);
$lastname = optional_param('lastname', '', PARAM_TEXT);
$userid = optional_param('userid', '', PARAM_TEXT);
$communityName = optional_param('communityName', '', PARAM_TEXT);
$schoolName = optional_param('schoolName', '', PARAM_TEXT);
$schoolId = optional_param('schoolId', '', PARAM_TEXT);

$loginurl = '/login/index.php';
$url = new moodle_url($loginurl, array('clirole' => $clirole,'idnumber'=>$idnumber,'authprovider' => 'cli','useremail' => $useremail,
'firstname' => $firstname,'lastname' => $lastname,'userid' => $userid,'communityName' => $communityName,'schoolName' => $schoolName,'schoolId'=>$schoolId));

redirect($url);
?>
