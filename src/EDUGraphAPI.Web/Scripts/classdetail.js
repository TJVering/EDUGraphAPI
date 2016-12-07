$(document).ready(function () {
    iniTiles();
    iniControl();
    iniTableSort();
});
function iniTiles(){
    $(".deskcontainer:not([position='0']").each(function () {
        var position = $(this).attr("position");
        var tile = $(".desktile[position='" + position + "']")
        $(this).appendTo(tile);
    });
}
function iniControl() {
    $("#imgedit").click(function () {
        $(this).hide();
        $("#imgsave").show();
        $("#imgcancel").show();
        $(".deskclose").show();
        // $("#graybg").find(".deskclose").show();
        enableDragAndDrop();
    });
    $("#imgcancel").click(function () {
        window.location.href = addParam(window.location.href,"tab","3");
    });
    $("#imgsave").click(function () {
        $(this).hide();
        $("#imgcancel").hide();
        $("#imgedit").show();
        $(".deskcontainer ").attr("draggable", false);
        $("#lstproducts").find(".seated").hide();
        $("#lstproducts").find("li").attr("draggable", false);
        $(".deskclose").hide();
        SaveEditDesk();
    });

    var tabToActivate = $.urlParam("tab");
    if (tabToActivate) {
        $('.nav-tabs li:eq(' + tabToActivate + ') a').tab('show');
    }
}
function iniTableSort() {
    $("#studentsTable").tablesorter({ sortList: [[0, 0]] });
}


function enableDragAndDrop() {
    $(".deskcontainer").attr("draggable", true);
    var lstProducts = $('#lstproducts li');
    //Set Drag on Each 'li' in the list 
    $.each(lstProducts, function (idx, val) {
        var id = $(this).attr("id");
        var position = $(".deskcontainer[userid='" + id + "']").attr("position");
        if (position == '0') {
            $(this).attr("draggable", true);
        } else {
            $(this).find(".seated ").removeClass("hideitem").show();
            $(this).attr("draggable", false);
        }
        $(this).on('dragstart', function (evt) {
            evt.target.draggable = false;
            var id = $(this).attr("id");
            evt.originalEvent.dataTransfer.setData("text", id);
            $(this).addClass("greenlist");
        });

    });
    $(".deskcontainer").on('dragstart', function (evt) {
        var id = $(this).attr("userid");
        evt.originalEvent.dataTransfer.setData("text", id);
        $("#"+id).addClass("greenlist");
    });

    $(".desktile").on('drop', function (evt) {
        evt.preventDefault();
        var container = $(this).find(".deskcontainer");
        if (container.length>0)
            return;
        $(".greenTileTooltip").remove();
        var id = evt.originalEvent.dataTransfer.getData("text");
        $("#" + id).removeClass("greenlist").attr("draggable", "").find(".seated").show();
        $(".deskcontainer[userid='" + id + "']").addClass("white").appendTo($(this));
        var position = $(this).attr("position");
        $(this).find(".deskcontainer").attr("position", position);
    });

    var greenTileTooltip = $("<div class='greenTileTooltip'>Place student here</div>");
    $(".desktile").on('dragenter', function (evt) {
        var container = $(this).find(".deskcontainer");
        if (container.length > 0)
        {
            $(".greenTileTooltip").remove();
            return;
        }
            
        greenTileTooltip.appendTo($(this));
    }).on("dragend", function (evt) {
        evt.preventDefault();
        $(".greenTileTooltip").remove();
        $(".greenlist").removeClass("greenlist");
    });;
    
    //The dragover
    $("#dvright").on('dragover', function (evt) {
        evt.preventDefault();
    });

    $(".deskclose").click(function (evt) {
        var parent = $(this).closest(".deskcontainer");
        var id = parent.attr("userid");
        var user = $("#" + id);
        user.find(".seated").hide();
        user.attr("draggable", true);
        parent.attr("position", 0);
        parent.appendTo($("#hidtiles"));
    });

}
function SaveEditDesk() {
    var classroomSeatingArrangements = [];
    var classId = $("#hidSectionid").val();
    $(".deskcontainer").each(function () {
        var userid = $(this).attr("userid");
        if (userid) {
            //getSeatingArrangements(userid, $(this).attr("position"))
            var position = $(this).attr("position");
            classroomSeatingArrangements.push({
                O365UserId: userid,
                Position: position,
                ClassId: classId
            });
        }
    });

    $.ajax({
        type: 'POST',
        url: "/Schools/SaveEditSeats",
        dataType: 'json',
        data: JSON.stringify(classroomSeatingArrangements),
        contentType: "application/json; charset=utf-8",
        success: function (responseData) {
            $('<div id="saveResult"><div>Seating map changes saved.</div></div>')
            .insertBefore($('#dvleft'))
           .fadeIn("slow", function () { $(this).delay(3000).fadeOut("slow"); });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            
        }
    });
}
function getSeatingArrangements(O365UserId, Position) {
    return { O365UserId: O365UserId, Position: Position };
}

$.urlParam = function (name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
    if (results == null) {
        return null;
    }
    else {
        return results[1] || 0;
    }
}
function addParam(url, param, value) {
    var a = document.createElement('a'), regex = /(?:\?|&amp;|&)+([^=]+)(?:=([^&]*))*/g;
    var match, str = []; a.href = url; param = encodeURIComponent(param);
    while (match = regex.exec(a.search))
        if (param != match[1]) str.push(match[1] + (match[2] ? "=" + match[2] : ""));
    str.push(param + (value ? "=" + encodeURIComponent(value) : ""));
    a.search = str.join("&");
    return a.href;
}