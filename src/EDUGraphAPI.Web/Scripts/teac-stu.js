$(document).ready(function () {
    var prev = { start: 0, stop: 0 }, Paging;
    setPageing();
    $(window).hashchange();

    $("#filterteacher").click(function () {
        ResetFilterClass($(this));
        $(".stubg").appendTo($("#hiditem"));
        $(".teacbg").appendTo($("#content")).hide();
        ResetPagedContent();
    });
    $("#filterstudnet").click(function () {
        ResetFilterClass($(this));
        $(".stubg").appendTo($("#content")).hide();
        $(".teacbg").appendTo($("#hiditem"));
        ResetPagedContent();
    });
    $("#filterall").click(function () {
        ResetFilterClass($(this));
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
        Paging =  $(".pagination").paging(cont.length, {
            format: '[< ncnnn! >]',
            perpage: 12,
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
                        return '<a href="#">Next</a>';
                    case 'prev': // <
                        return '<a href="#">Previous</a>';
                    case 'first': // [
                        return '<a href="#"></a>';
                    case 'last': // ]
                        return '<a href="#"></a>';
                }
            }

        });
    }
});