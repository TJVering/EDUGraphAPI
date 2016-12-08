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

    function ResetFilterClass(obj) {
        $(".filterlink").removeClass("selected");
        obj.addClass("selected");
    }
    function ResetPagedContent() {
        setPageing();
        Paging.setPage(1);
    }
    function setPageing() {
        $(".pagination").html("");
        var cont = $('#content div.element');
        var pageSize = 12;
        Paging = $(".pagination").paging(cont.length, {
            //format: " [ <  (qq -) nnncnnn (- pp) > ]",
            // format: "[ < > ] . (qq -) nnncnnn (- pp)",
            format: "[ < . (qq -) nnncnnn (- pp)> ] ",
            perpage: pageSize,
            lapping: 0,

            labels: {
                first: "First",
                leap: " &nbsp; ",
                last: "Last",
                prev: "<",
                next: ">",
                fill: "..."
            },
            onSelect: function (page) {
                var data = this.slice;
                cont.slice(prev[0], prev[1]).css('display', 'none');
                cont.slice(data[0], data[1]).fadeIn("slow").each(function (index) {
                    var img = $(this).find("img");
                    img.attr("src", img.attr("realheader"));
                });
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
                        if (cont.length > pageSize) {
                            if (this.page == this.pages) {
                                return '<a class="disabled" disabled = "true"  class="withicon" href="javascript:void(0)">Next</a>';
                            }
                            else {
                                return '<a class="withicon" href="javascript:void(0)">Next</a>';
                            }
                        }

                    case 'prev': // <
                        if (cont.length > pageSize) {
                            if (this.page != 1) {
                                return '<a  class="withicon" href="javascript:void(0)">Previous</a>';
                            }
                            else {
                                return '<a class="disabled" disabled = "true"   class="withicon" href="javascript:void(0)">Previous</a>';
                            }
                        }
                        //Show first and last
                    case 'first': // [
                        if (this.page == 1) {
                            return '<a class="disabled" disabled = "true"  class="withicon" href="javascript:void(0)">First</a>';

                        }
                        else {
                            return '<a  class="withicon" href="javascript:void(0)">First</a>';
                        }

                    case 'last': // ]
                        if (this.page == this.pages) {
                            return '<a class="disabled" disabled = "true"  class="withicon" href="javascript:void(0)">Last</a>';
                        } else {
                            return '<a  class="withicon" href="javascript:void(0)">Last</a>';
                        }
                    case 'fill':
                        if (this.active)
                            return "...";
                        return "";
                    case "leap":
                        return "";
                    case "left":
                        return "";
                    case "right":
                        return "";
                }
            }

        });
    }

    function ShowNoData() {
        $(".pagination").html("");
        $("#content").html('');
        $('<div class="nodata"> There is no data available for this page at this time.</div>').appendTo($(".user-container"));

    }
});