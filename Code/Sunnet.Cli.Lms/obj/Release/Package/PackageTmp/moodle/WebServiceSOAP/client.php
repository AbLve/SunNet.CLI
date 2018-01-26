<?php 
require_once ('lib/nusoap.php'); 
//$param = array( 'name' => '1','display'=>'2','parent_name'=>''); 

//$client = new soapclient('http://sh.sunnet.us:8082/moodle/WebServiceSOAP/server1.php?wsdl'); 
//$ret=$client ->insert_elisprogram_uset("","","");
//print( $ret);
$client = new soapclient('http://engage.sunnet.cc/WebService/Moodle.asmx?wsdl'); 
$ret=$client ->MoodleGlobalCookie();
print_r($ret->MoodleGlobalCookieResult);
?>