$(document).ready(function () {
    $("#graduates-listing").DataTable({
        "pageLength": 50
    });

    $("#vacancy-listing").DataTable({
        "pageLength": 50
    });

    $(".graduates-search-input").marcoPolo({
        url: '',
        formatItem: function (data, $item) {
            return data.first_name + ' ' + data.last_name;
        },
        onSelect: function (data, $item) {
            window.location = data.profile_url;
        }
    });
});