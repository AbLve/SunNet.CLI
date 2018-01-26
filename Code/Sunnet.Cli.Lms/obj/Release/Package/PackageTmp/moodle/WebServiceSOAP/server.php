<?php
require_once ('lib/nusoap.php'); 
$server = new soap_server; 
$server->configureWSDL("moodle"); 
$namespace = "moodle";

//function register
$server->register(
        'insert_elisprogram_uset',
        array('name'=>'xsd:string','display'=>'xsd:string','parent_name'=>'xsd:string'),
        array('return'=>'xsd:string'),
        $namespace,
        false, 
        'rpc',
        'encoded',
        'A web method of insert elisprogram_uset'
);

if (!isset($HTTP_RAW_POST_DATA)) $HTTP_RAW_POST_DATA =file_get_contents( 'php://input' );
$server->service($HTTP_RAW_POST_DATA); 
exit(); 

// create the function 
function insert_elisprogram_uset($name,$display,$parent_name) 
{ 
    require('../config.php');
    global $DB, $CFG;
    $program_uset=new stdClass();
    $program_uset->name=$name;
    $program_uset->display=$display; 
    $program_uset->parent=0;    
    $program_uset->depth=1;
    if($parent_name!="")
    {
        $program_uset_parent=$DB->get_record("local_elisprogram_uset",array('name'=>$parent_name));
        if ($program_uset_parent) {
            $program_uset->parent=$program_uset_parent->id;
            $program_uset->depth=$program_uset_parent->depth+1;
        }
    }
    $program_uset_id=$DB->insert_record("local_elisprogram_uset",$program_uset);
    if($program_uset_id>0)
        return "1";
    return "0";
} 

?> 