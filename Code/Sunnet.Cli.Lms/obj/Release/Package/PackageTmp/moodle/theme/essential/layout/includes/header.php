<?php
if (strpos($checkuseragent, 'MSIE 8') || strpos($checkuseragent, 'MSIE 7')) {?>
<header id="page-header-IE7-8<?php echo ($oldnavbar)? ' oldnavbar': '';?>" class="clearfix">
     <link href="<?php echo $CFG->static?>/Content/lib/Font-Awesome-3.2.1/css/font-awesome.min.css" rel="stylesheet" />
    <?php
} else { ?>
    <header id="page-header<?php echo ($oldnavbar)? ' oldnavbar': '';?>" class="clearfix">

        <link href="<?php echo $CFG->static?>/Content/lib/Font-Awesome-3.2.1/css/font-awesome.min.css" rel="stylesheet" />

        <?php
} ?>
        <style>
    .header-container {
        margin: auto;
    }

    .header-logo {
        float: left;
    }

    .header-logo img {
        margin: 10px;
    }

    .header-login {
        float: right;
    }

    .header-login a {
        background-color: #fff;
        border: 1px solid #e3e3e3;
        display: inline-block;
        text-align: center;
        padding: 0.7em 1em;
        margin-top: 40px;
        color: #333;
        text-decoration: none;
        float: left;
        margin-left: 10px;
    }

    .header-login a i {
        margin-right: 5px;
    }

    .header-login a:hover, .header-login a:focus {
        background-color: #42b357;
        border: 1px solid #018a01;
        color: #fff;
        text-decoration: none;
    }

    .header-login .dropdown {
        position: relative;
        float: left;
    }

    .dropdown-menu {
        margin: 0px 0 0;
    }

    .dropdown:hover .dropdown-menu {
        display: block;
    }

    .header-login .dropdown-menu > li > a {
        margin-top: 0;
        border: 0;
        padding: 10px 10px;
        text-align: left;
        margin-left: 0;
        width: 145px;
    }

    .header-login .dropdown-menu > li > a:hover {
        margin-top: 0;
        border: 0;
    }

    .header-login .dropdown-menu {
        min-width: 150px;
        border-radius: 0px;
        -webkit-box-shadow: 0 0px 0px rgba(0, 0, 0, .175);
        box-shadow: 0 0px 0px rgba(0, 0, 0, .175);
        margin-left: 10px;
    }

    .dropdown-menu > li > a:hover, 
    .dropdown-menu > li > a:focus {
        color: #018a01;
            background:#eeeeee !important;
    }

    .help-btn a {
        color: #fff;
        float: right;
        margin-top: 40px;
        width: 140px;
        height: 42px;
        background: #018a01 url('<?php echo $CFG->static?>/content/images/help_pic.png') no-repeat center center;
        border: 1px solid #38a132;
        text-decoration: none;
        cursor: pointer;
    }

    .help-btn a:hover {
        float: right;
        margin-top: 40px;
        width: 140px;
        height: 42px;
        background: #32a447 url('<?php echo $CFG->static?>/content/images/help_pic.png') no-repeat center center;
    }
</style>
        <div class="container-fluid">
            <div class="container">
                <div class="header-logo">
                    <a href="<?php echo $CFG->cli?>/">
                        <img src="<?php echo $CFG->static?>/content/images/cli_logo.png" alt="cli_logo"></a>
                </div>
                <div class="header-login">
                    <div class="dropdown">
                        <a class="header-sign-btn dropdown-toggle" type="button" id="dropdownMenu1" data-toggle="dropdown" href="#"><i class="icon-user"></i><?php echo $USER->firstname; ?>
                            <span class="caret"></span>
                        </a>

                        <ul class="dropdown-menu" role="menu" style="z-index: 5000;" aria-labelledby="dropdownMenu1">
                            <li role="presentation"><a role="menuitem" tabindex="-1" href="<?php echo $CFG->cli?>/home/dashboard"><i class="icon-dashboard"></i>Dashboard</a></li>
                            <li role="presentation"><a role="menuitem" tabindex="-1" href="<?php echo $CFG->cli?>/Profile/MyProfile"><i class="icon-bullhorn"></i>My Profile</a></li>
                        </ul>
                    </div>


                    <a href="/login/logout.php?sesskey=<?php echo sesskey()?>"><i class="icon-off"></i>Log out</a>
                </div>
                <div class="help-btn">
                    <a href="http://www.texasschoolready.org/help/MainTicketForm.aspx?Section8" target="_blank">&nbsp;</a>
                </div>
            </div>
        </div>
    </header>

    <header role="banner" class="navbar<?php echo ($oldnavbar)? ' oldnavbar': '';?>">
        <nav role="navigation" class="navbar-inner">
            <div class="container-fluid">
                <a class="brand" href="<?php echo $CFG->wwwroot;?>"><?php echo $SITE->shortname; ?></a>
                <a class="btn btn-navbar" data-toggle="collapse" data-target=".nav-collapse">
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </a>
                <div class="nav-collapse collapse">
                    <?php echo $OUTPUT->custom_menu(); ?>
                    <ul class="nav pull-right">
                        <li><?php echo $OUTPUT->page_heading_menu(); ?></li>
                        <!--<li class="navbar-text"><?php echo $OUTPUT->login_info() ?></li>-->
                    </ul>
                </div>
            </div>
        </nav>
    </header>
