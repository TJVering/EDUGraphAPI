﻿;
BingMapHelper = {};
BingMapHelper.BingMap = {
    displayPin: function (latitude, longitude, bingMapKey) {
        var map = new Microsoft.Maps.Map(document.getElementById('myMap'), {
            credentials: bingMapKey,
            center: new Microsoft.Maps.Location(latitude, longitude),
            mapTypeId: Microsoft.Maps.MapTypeId.road,
            showMapTypeSelector:false,
            zoom: 10
        });
        var pushpin = new Microsoft.Maps.Pushpin(map.getCenter(), null);
        map.entities.push(pushpin);
    }
};


sections = {
    $sectionTileId: ".section-tiles .tile-container"
};
sections.bindShowDetail = function () {
    $(sections.$sectionTileId).hover(function inFn(e) {
        $(this).children().last().show();
    }, function outFn(e) {
        $(this).children().last().hide();
    });
};

$(document).ready(function () {
    var bingMapKey = $("#BingMapKey").val();
    $(".bingMapLink").click(function ( evt) {
        evt.stopPropagation ? evt.stopPropagation() : evt.cancelBubble = true;
        var lat = $(this).attr("lat");
        var lon = $(this).attr("lon");
        BingMapHelper.BingMap.displayPin(lat, lon, bingMapKey);
        var offset = $(this).offset();
        $("#myMap").offset({ top: offset.top - 50, left: offset.left + 50 }).css({ width: "200px", height: "200px" }).show();
    });
    $(document).click(function () {
        $("#myMap").offset({ top: 0, left: 0 }).hide();
    });
    sections.bindShowDetail();
});