$(document).ready(function () {
    var prev = { start: 0, stop: 0 }, Paging;
    setPageing();
    $(window).hashchange();

    $("#filterteacher").click(function () {
        ResetFilterClass($(this));
        $('.nodata').remove();
        $(".stubg").appendTo($("#hiditem"));
        if ($(".teacbg").length == 0) {
            ShowNoData();
        }
        else {
            $(".teacbg").appendTo($("#content")).hide();
            ResetPagedContent();
        }
    });
    $("#filterstudnet").click(function () {
        ResetFilterClass($(this));
        $('.nodata').remove();
        $(".teacbg").appendTo($("#hiditem"));
        if ($(".stubg").length == 0) {
            ShowNoData();
        }
        else {
            $(".stubg").appendTo($("#content")).hide();
            ResetPagedContent();
        }
    });
    $("#filterall").click(function () {
        ResetFilterClass($(this));
        if ($(".stubg").length != 0 && $(".teacbg").length != 0) {
            $('.nodata').remove();
        }
        $(".teacbg").appendTo($("#content")).hide();
        $(".stubg").appendTo($("#content")).hide();
        ResetPagedContent();
    });

    $(window).hashchange(function () {

        if (window.location.hash)
            Paging.setPage(window.location.hash.substr(1));
        else
            Paging.setPage(1); // we dropped the initial page selection and need to run it manually
    });

    function ResetFilterClass(obj)
    {
        $(".filterlink").removeClass("selected");
       obj.addClass("selected");
    }
    function ResetPagedContent() {
        setPageing();
        Paging.setPage(1);
    }
    function setPageing()
    {
        $(".pagination").html("");
        var cont = $('#content div.element');
        var pageSize = 12;
        Paging =  $(".pagination").paging(cont.length, {
            format: '[< ncnnn! >]',
            perpage: pageSize,
            lapping: 0,
            onSelect: function (page) {
                var data = this.slice;
                cont.slice(prev[0], prev[1]).css('display', 'none');
                cont.slice(data[0], data[1]).fadeIn("slow");
                prev = data;
                return true; // locate!
            },
            onFormat: function (type) {
                switch (type) {
                    case 'block': // n and c
                        if (!this.active)
                            return '';
                        else if (this.value != this.page)
                            return '<a href="#' + this.value + '">' + this.value + '</a>';
                        return '<span class="current">' + this.value + '</span>';
                    case 'next': // >
                        if (cont.length > pageSize)
                        return '<a href="#">Next</a>';
                    case 'prev': // <
                        if (cont.length > pageSize)
                        return '<a href="#">Previous</a>';
                    case 'first': // [
                        return '<a href="#"></a>';
                    case 'last': // ]
                        return '<a href="#"></a>';
                }
            }

        });
    }

    function ShowNoData()    {
        $(".pagination").html("");
        $("#content").html('');
        $('<div class="nodata"> There is no data available for this page at this time.</div>').appendTo($(".user-container"));

    }
});