$(document).ready(function () {
    $(".jpost-filter").click(function () {
        $("#search-filter-modal").modal("show");
    });

    $("#btn-filter-search").click(function () {
        var jobTitle = $("#filter_jobTitle").val();
        var company = $("#filter_company option:selected").val();
        var jobType = $("#filter_jobType option:selected").val();
        var location = $("#filter_location option:selected").val();
        var vacancyStatus = $("#filter_vacancyStatus").val();
        var startDate = $("#filter_startDate").val();
        var endDate = $("#filter_endDate").val();
    });
});