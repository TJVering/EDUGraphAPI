$(document).ready(function () {
    
    IniPage();
    function IniPage() {
        $("#content").append($("#hiditem .element").clone());
        IniPagiation();
    }
    

    $("#filterteacher").click(function () {
        ResetFilterClass($(this));
        $('.nodata').remove();
        $("#content").html("").append($("#hiditem .teacbg").clone());
        if ($(".teacbg").length == 0) {
            ShowNoData();
        }
        else {
            IniPagiation();
        }
    });
    $("#filterstudnet").click(function () {
        ResetFilterClass($(this));
        $("#content").html("").append($("#hiditem .stubg").clone());

        if ($(".stubg").length == 0) {
            ShowNoData();
        }
        else {
            IniPagiation();
        }
    });
    $("#filterall").click(function () {
        ResetFilterClass($(this));
        $('#content').html("");
        if ($(".stubg").length != 0 && $(".teacbg").length != 0) {
            $('.nodata').remove();
        }
        IniPage();
    });


    
    function IniPagiation() {
        var num_entries = jQuery('#content div.element').length;
        $("#pagination").pagination(num_entries, {
            callback: pageselectCallback,
            items_per_page: 12, 
            next_text: "Next",
            num_display_entries: 10,
            num_edge_entries: 2,
            prev_text: "Previous"
        });
    }
    function pageselectCallback(page_index, jq) {
                var start = page_index * this.items_per_page;
                var end = start + this.items_per_page;
                $('#content div.element').hide()
                    .slice(start, end).fadeIn("slow").each(function (index) {
                        var img = $(this).find("img");
                        img.attr("src", img.attr("realheader"));
                    });

                return false;
            }

    function ResetFilterClass(obj) {
        $(".filterlink").removeClass("selected");
        obj.addClass("selected");
    }
    function ShowNoData() {
        $(".pagination").html("");
        $("#content").html('');
        $('<div class="nodata"> There is no data available for this page at this time.</div>').appendTo($(".user-container"));

    }
});