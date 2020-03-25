$(document).ready(function () {
    $(window).scroll(function () {
        var scrollPos = $(this).scrollTop();
        var elementHeight = $(this).height();
        var result = 1 - (elementHeight - scrollPos) / elementHeight;
        $('.main-nav').css('background-color', 'rgba(255,255,255,0.2) !important');
    });
});