<div class="footeressential">
    <div class="homelink">
        <a href="https://www.uth.edu/" target="_blank">
            <img src="<?php echo $CFG->cli?>/images/UT.png" alt="UT"></a>
        <a href="http://www.childrenslearninginstitute.org/" target="_blank">
            <img src="<?php echo $CFG->cli?>/images/CLI.png" alt="CLI"></a>
        <a href="http://www.texasschoolready.org/" target="_blank">
            <img src="<?php echo $CFG->cli?>/images/TSR.png" alt="TSR"></a>
    </div>
    <div class="copyright">
        <div class="nav-bottom">
            <a href="https://www.uth.edu/index/policies.htm" target="_blank">Site Policies</a>
            <a href="https://www.uth.edu/index/file-viewing-information.htm" target="_blank">Web File Viewing</a>
            <a href="<?php echo $CFG->cli?>/contact">Contact Us</a>
        </div>
        <p>
            Copyright © 2013 by <a class="main-a" href="https://www.uth.edu/" target="_blank">The University of Texas Health Science Center</a> at Houston (UTHealth)
                <br />
            Webmaster: <a href="mailto:cliengage@uth.tmc.edu">cliengage@@uth.tmc.edu</a> | This document was last modified on: <?php echo date ("F d Y H:i:s.", getlastmod());?>
        </p>
    </div>
</div>

<?php echo $OUTPUT->standard_footer_html(); ?>

<style>
    .footeressential {
        background: #42b357;
        float: left;
        width: 100%;
        margin-top: 30px;
    }

        .footeressential a {
            color: #052e0d;
            font-weight: bold;
            text-decoration: underline;
        }

            .footeressential a:hover {
                color: #fff;
            }

    .homelink {
        width: 100%;
        text-align: center;
        margin: 30px 0;
    }

        .homelink a {
            margin: 10px 70px;
            display: inline-block;
        }

    .copyright {
        width: 100%;
        float: left;
        border-top: 1px solid #65cb78;
        text-align: center;
    }

    .nav-bottom {
        width: 100%;
        text-align: center;
        margin: 20px 0 10px;
    }

        .nav-bottom a {
            padding: 10px 20px;
        }
        .nav-bottom {
    width: 410px;
    margin: 0 auto;
}

    .copyright p {
        width: 100%;
        color: #052e0d;
        text-align: center;
        margin-top: 10px;
        margin-bottom: 20px;
    }
</style>
