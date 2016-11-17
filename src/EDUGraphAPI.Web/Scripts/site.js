$(document).ready(function () {
    $("#userinfolink").click(function (evt) {
        evt.stopPropagation ? evt.stopPropagation() : evt.cancelBubble = true;
        $("#userinfoContainer").toggle();
        $("#caret").toggleClass("transformcaret");
    });
    $(document).click(function () {
        $("#userinfoContainer").hide();
        $("#caret").removeClass("transformcaret");

    });
});