var offset = 0;
var searchValue = "";
var isLoading = false;

$(document).ready(function () {
    var scrollTop = $(window).scrollTop();
    var windowHeight = $(window).height();
    var documentHeight = $(document).height();

    $(window).scroll(function () {
        var currentScroll = $(window).scrollTop();
        var remainingHeight = $("#GetInforBooking").height() - (currentScroll + windowHeight);

        if (remainingHeight <= 100 && !isLoading) {
            isLoading = true;
            offset++;
            loadMoreItems();
        }
    });

    async function loadMoreItems() {
        console.log(offset);
        await $.ajax({
            type: 'POST',
            url: '/Home/ExtendScroll',
            data: {
                search: searchValue,
                page: offset,
            },
            dataType: 'JSON',
            async: true,
            success: async function (data) {
                $("#GetInforBooking").append(data);
                isLoading = false;
            },
        });
    }
});

async function onSearch() {
    searchValue = document.getElementById('inputSearchCus').value;
    offset = 0;
    var response = await $.ajax({
        type: 'POST',
        url: '/Home/ExtendScroll',
        data: {
            search: searchValue,
            page: offset,
        },
        dataType: 'JSON',
        async: true,
        success: async function (data) {
            $("#GetInforBooking").html(data);
        },
    });
}