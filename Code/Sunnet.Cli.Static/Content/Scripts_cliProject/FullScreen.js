$(function () {
    document.addEventListener("fullscreenchange", function () {
        if (!document.fullScreen && isFullscreen) {
            isFullscreen = false;
            $(".icon-resize-small").attr("class", "icon-resize-full");
            $("#divLayoutHead").css("display", "");
        }
    }, false);

    document.addEventListener("mozfullscreenchange", function () {
        if (!document.mozFullScreen && isFullscreen) {
            isFullscreen = false;
            $(".icon-resize-small").attr("class", "icon-resize-full");
            $("#divLayoutHead").css("display", "");
        }
    }, false);

    document.addEventListener("webkitfullscreenchange", function () {
        if (!document.webkitIsFullScreen && isFullscreen) {
            isFullscreen = false;
            $(".icon-resize-small").attr("class", "icon-resize-full");
            $("#divLayoutHead").css("display", "");
        }
    }, false);

    //ie
    document.onmsfullscreenchange = function () {
        if (!document.msFullscreenElement && isFullscreen) {
            isFullscreen = false;
            $(".icon-resize-small").attr("class", "icon-resize-full");
            $("#divLayoutHead").css("display", "");
        }
    };
})

var isFullscreen = false;
function switchFullscreen(obj) {
    if (isFullscreen) {
        isFullscreen = false;
        exitFullscreen(obj);
    } else {
        isFullscreen = true;
        requestFullscreen(obj);
    }
};

function requestFullscreen(obj) {
    var de = document.getElementById(obj);
    if (de.requestFullscreen) {  //W3C 
        de.requestFullscreen();
    } else if (de.msRequestFullscreen) {  //IE11
        de.msRequestFullscreen();
    } else if (de.mozRequestFullScreen) {  //FireFox 
        de.mozRequestFullScreen();
    } else if (de.webkitRequestFullScreen) {  //Chrome
        de.webkitRequestFullScreen();
    }
    $(".icon-resize-full").attr("class", "icon-resize-small");
    $("#divLayoutHead").css("display", "none");
}

function exitFullscreen(obj) {
    var de = document;
    if (de.exitFullscreen) {
        de.exitFullscreen();
    } else if (de.msExitFullscreen) {
        de.msExitFullscreen();
    } else if (de.mozCancelFullScreen) {
        de.mozCancelFullScreen();
    } else if (de.webkitCancelFullScreen) {
        de.webkitCancelFullScreen();
    }
    $(".icon-resize-small").attr("class", "icon-resize-full");
    $("#divLayoutHead").css("display", "");
}
