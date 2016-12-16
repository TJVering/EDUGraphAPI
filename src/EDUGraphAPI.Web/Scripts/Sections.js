$(document).ready(function () {
    bindShowDetail($(".section-tiles .tile-container"));
    InitPagination(0);
});

function bindShowDetail(tiles) {
    tiles.hover(function inFn(e) {
        $(this).children().last().show();
    }, function outFn(e) {
        $(this).children().last().hide();
    }).find(".detail #termdate").each(function (i, e) {
        var $e = $(e);
        var dateStr = $e.text();
        if (dateStr) {
            $e.text(moment.utc(dateStr).local().format('MMMM D YYYY'));
        }
    });
};

function InitPagination(currentPage) {
    $(".section-tiles").show();
    var entryCount = $('.section-tiles div.tile-container').length;
    var itemsPerPage = 12;
    var nextLink = $(".sections input#next-link").val();
    var hasNextPage = typeof (nextLink) === "string" && nextLink.length > 0;
    var paginationElement = $("#pagination");
    paginationElement.pagination(entryCount, {
        current_page: currentPage,
        show_if_single_page: hasNextPage,
        callback: function (page_index, jq) {
            var start = page_index * this.items_per_page;
            var end = start + this.items_per_page;
            $('.section-tiles div.tile-container').hide().slice(start, end).fadeIn("slow");
            if (hasNextPage) {
                initNextPage(nextLink)
            }
        },
        items_per_page: itemsPerPage,
        next_text: "Next",
        num_display_entries: 10,
        num_edge_entries: 2,
        prev_text: "Previous"
    });

    if (hasNextPage && itemsPerPage == entryCount) {
        paginationElement.find(".next").prevUntil(".prev").hide();
    }
}

function initNextPage(nextLink) {
    var next = $("#pagination .next");
    next.removeClass("current").off("click").click(function () {
        var currentPage = parseInt(next.siblings("span.current:not(.prev):not(.next)").text());
        currentPage = (isNaN(currentPage) ? 0 : currentPage - 1);
        loadNextPage(currentPage, nextLink);
    });
}

function loadNextPage(currentPage, nextLink) {
    var pagination = $("#pagination").hide();
    var loading = $("#loading").show();
    var schoolId = $(".sections input#school-id").val();
    var url = "/Schools/" + schoolId + "/Classes/Next";
    $.ajax({
        type: 'POST',
        url: url,
        dataType: 'json',
        data: JSON.stringify({ schoolId: schoolId, nextLink: nextLink }),
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            var newNextLink = data.NextLinkOfSections;
            $(".sections input#next-link").val(newNextLink);
            var tiles = $(".section-tiles");
            $.each(data.Sections, function (i, s) {
                var isMine = hasSection(s, data.MySections);
                var tileHtml = $('<div class="tile-container"></div>');
                var tileContainer = tileHtml;
                if (isMine) {
                    tileContainer = $('<a class="mysectionlink" href="' + s.ObjectId + '"></a>').appendTo(tileHtml)
                }
                var tile = $('<div class="tile"><h5>' + s.DisplayName + '</h5><h2>' + s.CombinedCourseNumber + '</h2></div>');
                tile.appendTo(tileContainer);
                var tileDetail = $('<div class="detail" style="display: none;">' +
                                        '<h5>Course Id:</h5>' +
                                        '<h6>' + s.CourseId + '</h6>' +
                                        '<h5>Description:</h5>' +
                                        '<h6>' + s.CourseDescription + '</h6>' +
                                        '<h5>Teachers:</h5>' +
                                        ((s.Members instanceof Array) ?
                                        s.Members.reduce(function (accu, cur) {
                                            if (cur.ObjectType == 'Teacher') {
                                                accu += '<h6>' + cur.DisplayName + '</h6>';
                                            }
                                            return accu;
                                        }, '') : '') +

                                        '<h5>Term Name:</h5>' +
                                        '<h6>' + s.TermName + '</h6>' +
                                        '<h5>Start/Finish Date:</h5>' +
                                        ((s.TermStartDate && s.TermEndDate) ?
                                        ('<h6><span id="termdate">' +
                                        (s.TermStartDate ? moment.utc(s.TermStartDate).local().format('MMMM D YYYY') : '') +
                                        '</span><span> - </span><span id="termdate">' +
                                        (s.TermEndDate ? moment.utc(s.TermEndDate).local().format('MMMM D YYYY') : '') +
                                        '</span></h6>') : '') +
                                        '<h5>Period:</h5>' +
                                        '<h6>' + s.Period + '</h6>' +
                                    '</div>');
                tileDetail.appendTo(tileHtml);
                tileHtml.appendTo(tiles);
                bindShowDetail(tileHtml);
            });
            InitPagination(currentPage + 1);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
        },
        complete: function () {
            pagination.show();
            loading.hide();
        }
    });
}

function hasSection(section, sections) {
    if (!(sections instanceof Array)) {
        return false;
    }
    var result = false;
    $.each(sections, function (i, s) {
        if (section.Email == s.Email) {
            return result = true;
        }
    });
    return result;
}
