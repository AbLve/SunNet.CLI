<?php

/**
 * @author Jerome Mouneyrac
 * @license http://www.gnu.org/copyleft/gpl.html GNU Public License
 * @package moodle multiauth
 *
 * Authentication Plugin: Google/Facebook/Messenger Authentication
 * If the email doesn't exist, then the auth plugin creates the user.
 * If the email exist (and the user has for auth plugin this current one),
 * then the plugin login the user related to this email.
 */

if (!defined('MOODLE_INTERNAL')) {
    die('Direct access to this script is forbidden.');    ///  It must be included from a Moodle page
}

require_once($CFG->libdir.'/authlib.php');

/**
 * Google/Facebook/Messenger Oauth2 authentication plugin.
 */
class auth_plugin_cliauth extends auth_plugin_base {

    /**
     * Constructor.
     */
    function auth_plugin_cliauth() {
        $this->
authtype = 'cliauth';
        $this->roleauth = 'auth_cliauth';
        $this->config = get_config('auth/cliauth');
    }

    /**
     * Prevent authenticate_user_login() to update the password in the DB
     * @return boolean
     */
    function prevent_local_passwords() {
        return true;
    }

    /**
     * Authenticates user against the selected authentication provide (Google, Facebook...)
     *
     * @param string $username The username (with system magic quotes)
     * @param string $password The password (with system magic quotes)
     * @return bool Authentication success or failure.
     */
    function user_login($username, $password) {
        global $DB, $CFG;

        //retrieve the user matching username
        $user = $DB->get_record('user', array('username' => $username,
            'mnethostid' => $CFG->mnet_localhost_id));

        //username must exist and have the right authentication method
        if (!empty($user) && ($user->auth == 'cliauth')) {
            //$code = optional_param('code', false, PARAM_TEXT);
            //if($code === false){
            //    return false;
            //}
            return true;
        }

        return false;
    }

    /**
     * Returns true if this authentication plugin is 'internal'.
     *
     * @return bool
     */
    function is_internal() {
        return false;
    }

    /**
     * Returns true if this authentication plugin can change the user's
     * password.
     *
     * @return bool
     */
    function can_change_password() {
        return false;
    }

    /**
     * Authentication hook - is called every time user hit the login page
     * The code is run only if the param code is mentionned.
     */
    function loginpage_hook() {
        global $USER, $SESSION, $CFG, $DB;

        //check the Google authorization code
        $useremail = optional_param('useremail', '', PARAM_TEXT);
        $idnumber=   optional_param('idnumber', '', PARAM_TEXT);
        $userid=   optional_param('userid', '', PARAM_TEXT);
        $firstname=   optional_param('firstname', '', PARAM_TEXT);
        $lastname=   optional_param('lastname', '', PARAM_TEXT);
        $clirole = optional_param('clirole', '', PARAM_TEXT);
        $role =$DB->get_record("role",array("shortname"=>$clirole)) ;
        $_SESSION["role"]= $clirole;
        $_SESSION["userid"]=    $userid;
        $_SESSION["idnumber"]= $idnumber;
        
        if (!empty($useremail)) {

            $authprovider = required_param('authprovider', PARAM_ALPHANUMEXT);
            //$useremail = required_param('useremail', PARAM_ALPHANUMEXT);

            
            // Prohibit login if email belongs to the prohibited domain
            if ($err = email_is_not_allowed($useremail)) {
                throw new moodle_exception($err, 'auth_googleoauth2');
            }

            //if email not existing in user database then create a new username (userX).
            if (empty($useremail) or $useremail != clean_param($useremail, PARAM_EMAIL)) {
                throw new moodle_exception('couldnotgetuseremail');
                //TODO: display a link for people to retry
            }
            //get the user - don't bother with auth = googleoauth2 because
            //authenticate_user_login() will fail it if it's not 'googleoauth2'
            //$user = $DB->get_record('user', array('email' => $useremail, 'deleted' => 0, 'mnethostid' => $CFG->mnet_localhost_id));
            $user = $DB->get_record('user', array('username' => $idnumber, 'deleted' => 0, 'mnethostid' => $CFG->mnet_localhost_id));
            //create the user if it doesn't exist
            if (empty($user)) {

                // deny login if setting "Prevent account creation when authenticating" is on
                if($CFG->authpreventaccountcreation) throw new moodle_exception("noaccountyet", "auth_cliauth");


                //get following incremented username
                $lastusernumber = get_config('auth/cliauth', 'lastusernumber');
                $lastusernumber = empty($lastusernumber)?1:$lastusernumber++;
                //check the user doesn't exist
                $nextuser = $DB->get_record('user',
                        array('username' => get_config('auth/cliauth', 'googleuserprefix').$lastusernumber));
                while (!empty($nextuser)) {
                    $lastusernumber = $lastusernumber +1;
                    $nextuser = $DB->get_record('user',
                        array('username' => get_config('auth/cliauth', 'googleuserprefix').$lastusernumber));
                }
                set_config('lastusernumber', $lastusernumber, 'auth/googleoauth2');
                $username = get_config('auth/cliauth', 'googleuserprefix') . $idnumber;

                //retrieve more information from the provider
                $newuser = new stdClass();
                $newuser->email = $useremail;
                $newuser->idnumber=$idnumber;
                $newuser->firstname = $firstname;
                $newuser->lastname = $lastname;

                $user=create_user_record($username, '', 'cliauth');
                
                //insert elis user
                $program_usr = new stdClass();
                $program_usr->username=$newuser->idnumber;
                $program_usr->password='';
                $program_usr->idnumber=$newuser->idnumber;
                $program_usr->firstname=$newuser->firstname;
                $program_usr->lastname=$newuser->lastname;
                $program_usr->email= $newuser->email;
                $program_usr_id=$DB->insert_record("local_elisprogram_usr",$program_usr); 
                
                //insert elis user and moodle user relation table
                $program_usr=$DB->get_record("local_elisprogram_usr",array('idnumber'=>$idnumber));
                $program_usr_mdl=new stdClass();
                $program_usr_mdl->cuserid=$program_usr->id ;
                $program_usr_mdl->muserid=$user->id ;
                $program_usr_mdl->idnumber=$idnumber ;
                $DB->insert_record("local_elisprogram_usr_mdl",$program_usr_mdl);
                
                $elisRole =$DB->get_record("role",array("shortname"=>'elis_viewuserset')) ;
                if($clirole==110 || $clirole==120 || $clirole==115 || $clirole==140)
                {
                    $communityName = optional_param('communityName', '', PARAM_TEXT);
                    //create community usr
                    $elisprogram_uset=$DB->get_record("local_elisprogram_uset",array('name'=>$communityName));
                    if(empty($elisprogram_uset))
                    {
                        $program_uset=new stdClass();
                        $program_uset->name=$communityName;
                        $program_uset->display=$communityName;
                        $program_uset->parent=0;
                        $program_uset->depth=1;
                        $program_uset_id=$DB->insert_record("local_elisprogram_uset",$program_uset);
                    }
                    else
                        $program_uset_id=$elisprogram_uset->id;
                    
                    //automatic assign elis role to community usr
                    $role_assignments=new stdClass();
                    $role_assignments->roleid=$elisRole->id;
                    $role_assignments->contextid=\local_elisprogram\context\userset::instance($program_uset_id)->id; 
                    $role_assignments->userid=$user->id;
                    $role_assignments->timemodified=time();
                    $role_assignments-> modifierid=$program_uset_id;
                    $role_assignments-> component="";
                    $role_assignments->itemid=0;
                    $role_assignments->sortorder=0;
                    $role_assignments_id=$DB->insert_record("role_assignments",$role_assignments);
                    //automatic assign system role
                    $role_assignments->roleid=$role->id;
                    $role_assignments->contextid=context_system::instance()->id;
                    $role_assignments_id=$DB->insert_record("role_assignments",$role_assignments);
                }
                else if($clirole==130 || $clirole==142 || $clirole==125 || $clirole==135)
                {
                    $communityName = optional_param('communityName', '', PARAM_TEXT);
                    $elisprogram_community_uset=$DB->get_record("local_elisprogram_uset",array('name'=>$communityName));
                    //if community uset is not exist,then add
                    if(empty($elisprogram_community_uset))
                    {
                        $program_uset=new stdClass();
                        $program_uset->name=$communityName;
                        $program_uset->display=$communityName;
                        $program_uset->parent=0;
                        $program_uset->depth=1;
                        $program_uset_community_id=$DB->insert_record("local_elisprogram_uset",$program_uset);
                    }
                    else
                        $program_uset_community_id=$elisprogram_community_uset->id;
                    
                    //create principal uset
                    $schoolName = optional_param('schoolName', '', PARAM_TEXT);
                    $display=   'school_'.optional_param('schoolId', '', PARAM_TEXT);
                    $elisprogram_school_uset=$DB->get_record("local_elisprogram_uset",array('display'=>$display));
                    if(empty($elisprogram_school_uset)){
                        $program_uset=new stdClass();
                        $program_uset->name=$schoolName;
                        $program_uset->display=$display;
                        $program_uset->parent=$program_uset_community_id;
                        $program_uset->depth=1;
                        $program_school_uset_id=$DB->insert_record("local_elisprogram_uset",$program_uset);
                    }
                    else
                        $program_school_uset_id=$elisprogram_school_uset->id;
                    
                    //automatic assign elis role to principal usr
                    $role_assignments=new stdClass();
                    $role_assignments->roleid=$elisRole->id;
                    $role_assignments->contextid=\local_elisprogram\context\userset::instance($program_school_uset_id)->id; 
                    $role_assignments->userid=$user->id;
                    $role_assignments->timemodified=time();
                    $role_assignments-> modifierid=$program_school_uset_id;
                    $role_assignments-> component="";
                    $role_assignments->itemid=0;
                    $role_assignments->sortorder=0;
                    $role_assignments_id=$DB->insert_record("role_assignments",$role_assignments);
                    //automatic assign system role
                    $role_assignments->roleid=$role->id;
                    $role_assignments->contextid=context_system::instance()->id;
                    $role_assignments_id=$DB->insert_record("role_assignments",$role_assignments);
                    
                }
                else if ($clirole==145)
                {
                    $communityName = optional_param('communityName', '', PARAM_TEXT);
                    $elisprogram_community_uset=$DB->get_record("local_elisprogram_uset",array('name'=>$communityName));
                    //if community uset is not exist,then add
                    if(empty($elisprogram_community_uset))
                    {
                        $program_uset=new stdClass();
                        $program_uset->name=$communityName;
                        $program_uset->display=$communityName;
                        $program_uset->parent=0;
                        $program_uset->depth=1;
                        $program_uset_community_id=$DB->insert_record("local_elisprogram_uset",$program_uset);
                    }
                    else
                        $program_uset_community_id=$elisprogram_community_uset->id;
                    
                    $schoolName = optional_param('schoolName', '', PARAM_TEXT);
                    $display=   'school_'.optional_param('schoolId', '', PARAM_TEXT);
                    $elisprogram_school_uset=$DB->get_record("local_elisprogram_uset",array('display'=>$display));
                    //if school uset is not exist,then add
                    if(empty($elisprogram_school_uset))
                    {
                        $program_uset=new stdClass();
                        $program_uset->name=$schoolName;
                        $program_uset->display=$display;
                        $program_uset->parent=$program_uset_community_id;
                        $program_uset->depth=1;
                        $program_school_uset_id=$DB->insert_record("local_elisprogram_uset",$program_uset);
                    }
                    else
                        $program_school_uset_id=$elisprogram_school_uset->id;
                    
                    //automatic assign elis role to principal usr
                    $role_assignments=new stdClass();
                    $role_assignments->roleid=$elisRole->id;
                    $role_assignments->contextid=\local_elisprogram\context\userset::instance($program_school_uset_id)->id; 
                    $role_assignments->userid=$user->id;
                    $role_assignments->timemodified=time();
                    $role_assignments-> modifierid=$program_school_uset_id;
                    $role_assignments-> component="";
                    $role_assignments->itemid=0;
                    $role_assignments->sortorder=0;
                    $role_assignments_id=$DB->insert_record("role_assignments",$role_assignments);
                    //automatic assign system role
                    $role_assignments->roleid=$role->id;
                    $role_assignments->contextid=context_system::instance()->id;
                    $role_assignments_id=$DB->insert_record("role_assignments",$role_assignments);
                    
                    //assign teacher to principal uset   
                    //$program_uset_asign   = new stdClass();
                    //$program_uset_asign->clusterid=$program_school_uset_id;
                    //$program_uset_asign->userid=$program_usr_id;
                    //$program_uset_asign->plugin="manual";
                    //$program_uset_asign->autoenrol=1;
                    //$program_uset_asign->leader=0;
                    //$DB->insert_record("local_elisprogram_uset_asign",$program_uset_asign);
                }
                else{
                    role_assign($role->id, $user->id,context_system::instance()->id);
                }

            } else {
                $username = $user->username;
            }

            //authenticate the user
            //TODO: delete this log later
            $userid = empty($user)?'new user':$user->id;
            $user = authenticate_user_login($username, null);
            if ($user) {

                //set a cookie to remember what auth provider was selected
                //setcookie($CFG->sessioncookie, $authprovider,
                //        time()+(DAYSECS*60), $CFG->sessioncookiepath,
                //        $CFG->sessioncookiedomain, $CFG->cookiesecure,
                //        $CFG->cookiehttponly);

                //prefill more user information if new user
                if (!empty($newuser)) {
                    $newuser->id = $user->id;
                    $DB->update_record('user', $newuser);
                    $user = (object) array_merge((array) $user, (array) $newuser);
                }
                else{
                    $user->firstname=$firstname;
                    $user->lastname=$lastname;
                    $user->email=$useremail;
                    $DB->update_record('user', $user);
                }

                complete_user_login($user);   
                
                $user = $DB->get_record('user', array('idnumber' => $idnumber, 'deleted' => 0, 'mnethostid' => $CFG->mnet_localhost_id));
                
                $role_assignments=$DB->get_record('role_assignments',array('roleid'=>$role->id,'contextid'=>context_system::instance()->id,'userid'=>$user->id));
                if(!empty($role_assignments))
                {
                    role_unassign_all(array('roleid'=>$role->id,'contextid'=>context_system::instance()->id,'userid'=>$user->id));
                    role_assign($role->id, $user->id,context_system::instance()->id); 
                }
                
                // Redirection
                if (user_not_fully_set_up($USER)) {
                    $urltogo = $CFG->wwwroot.'/user/edit.php';
                    // We don't delete $SESSION->wantsurl yet, so we get there later
                } 
                else if (isset($SESSION->wantsurl) and (strpos($SESSION->wantsurl, $CFG->wwwroot) === 0)) {
                    $urltogo = $SESSION->wantsurl;    // Because it's an address in this site
                    unset($SESSION->wantsurl);
                }
                else {
                    // No wantsurl stored or external - go to homepage
                    $urltogo = $CFG->wwwroot.'/';
                    unset($SESSION->wantsurl);
                }
                redirect($urltogo);
            }
        }
    }

    /**
     * Called when the user record is updated.
     *
     * We check there is no hack-attempt by a user to change his/her email address
     *
     * @param mixed $olduser     Userobject before modifications    (without system magic quotes)
     * @param mixed $newuser     Userobject new modified userobject (without system magic quotes)
     * @return boolean result
     *
     */
    function user_update($olduser, $newuser) {
        if ($olduser->email != $newuser->email) {
            return false;
        } else {
            return true;
        }
    }

}
