$(document).ready(function () {
    //setEvents();
});

function setEvents() {
    var lstProducts = $('#lstproducts li');
    //Set Drag on Each 'li' in the list 
    $.each(lstProducts, function (idx, val) {
        $('li').on('dragstart', function (evt) {
            evt.target.draggable = false;
        });
    });

    $(".stucontainer").on('drop', function (evt) {
        evt.preventDefault();
        var data = evt.originalEvent.dataTransfer.getData("Text");
        var id = evt.originalEvent.dataTransfer.getData("id").replace("list", "");
        $(this).append();
    });
    $(".deskclose").click(function (evt) {
        var parent = $(this).parent();
        var id = parent.attr("id").replace("desk", "");
        $("#list" + id).attr("draggable", true).removeClass("selectedLI");
        $("#hidedesk").append(parent);
    });

    //The dragover
    $("#dvright").on('dragover', function (evt) {
        evt.preventDefault();
    });
}
